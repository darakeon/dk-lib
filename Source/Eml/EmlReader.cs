using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Keon.Eml
{
	/// <summary>
	/// Process EML files into objects
	/// </summary>
	public class EmlReader
	{
		private const String subjectKey = "Subject";
		private const String encodingKey = "Content-Transfer-Encoding";
		private const String encodingBase64 = "base64";
		private const String contentTypeKey = "Content-Type";
		private const String contentMulti = "multipart/alternative";
		private const String boundaryPattern = @"\s+boundary=""(.+)""";

		/// <summary>
		/// Subject of the e-mail, decoded in case of original being in Base64
		/// </summary>
		public String Subject { get; private set; }

		/// <summary>
		/// E-mail body, decoded in case of original being in Base64
		/// </summary>
		public String Body { get; private set; }

		/// <summary>
		/// Headers of the e-mail
		/// </summary>
		public IDictionary<String, String> Headers =>
			new ReadOnlyDictionary<String, String>(headers);

		private readonly IDictionary<String, String> headers =
			new Dictionary<String, String>();

		/// <summary>
		/// Creation date - only if eml file is given
		/// </summary>
		public DateTime? Creation { get; }

		private String boundary;

		private Boolean isBase64 =>
			headers.ContainsKey(encodingKey)
				&& headers[encodingKey] == encodingBase64;

		/// <summary>
		/// Read the content of the eml from a file
		/// </summary>
		/// <param name="content">Content of the eml file</param>
		/// <param name="creation">Creation date of e-mail</param>
		public EmlReader(String content, DateTime? creation = null)
			: this(content.Split("\n"), creation) { }

		/// <summary>
		/// Read the content of the eml from a file
		/// </summary>
		/// <param name="content">Content of the eml file</param>
		/// <param name="creation">Creation date of e-mail</param>
		public EmlReader(String[] content, DateTime? creation = null)
		{
			content = extractHeaders(content);

			setSubject();

			processBody(content);

			Creation = creation;
		}

		private String[] extractHeaders(String[] content)
		{
			var l = 0;
			for (; l < content.Length; l++)
			{
				var line = content[l];

				if (line.StartsWith(" ") || line.StartsWith("\t"))
					addHeaderOtherLines(line);

				else if (line.Contains(":"))
					addHeaderFirstLine(line);
				
				else
					break;
			}

			return content.Skip(l).ToArray();
		}

		private void addHeaderFirstLine(String line)
		{
			var parts = line.Split(":", 2);
			var key = parts[0];
			var value = parts[1].Trim();

			if (value.EndsWith(";"))
				value = value[..^1];

			if (headers.ContainsKey(key))
				headers[key] += $" {value}";
			else
				headers.Add(key, value);
		}

		private void addHeaderOtherLines(String line)
		{
			var key = headers.Keys.Last();

			if (key == contentTypeKey)
			{
				if (headers[contentTypeKey] != contentMulti) return;
				
				var boundary = new Regex(boundaryPattern).Match(line);
				if (boundary.Success) this.boundary = boundary.Groups[1].Value;

				return;
			}

			headers[key] += line;
		}

		private void setSubject()
		{
			if (!headers.ContainsKey(subjectKey))
				return;

			Subject = headers[subjectKey];

			if (Subject.Contains("utf-8"))
			{
				Subject = String.Join("",
					Subject
						.Split(" ")
						.Select(s => s[10..^2].FromBase64())
				);
			}
		}

		private void processBody(String[] content)
		{
			Body = boundary == null
				? processSimpleBody(content)
				: processMultiBody(content);
		}

		private String processSimpleBody(String[] content)
		{
			if (!isBase64)
			{
				content = content
					.Where(c => !String.IsNullOrEmpty(c))
					.ToArray();
			}

			var body = String.Join("\n", content);

			if (isBase64)
				body = body.FromBase64();

			return body
				.Replace("\"\"", "\"")
				.Replace("=\n", "")
				.Replace("=0A", "\n")
				.Replace("=3D", "=")
				;
		}

		private String processMultiBody(String[] content)
		{
			var start = content.ToList().IndexOf(boundary);
			content = extractHeaders(content[(start + 1)..]);
			var end = content.ToList().IndexOf(boundary);

			if (end < 0)
				return null;

			var lastAdded = 
				headers["Content-Type"]
					.Split("/").Last();

			var body = "-- " + lastAdded.ToUpper() + "\n"
			    + processSimpleBody(content[..end]);

			var otherBodies = processMultiBody(content);
			if (otherBodies != null) body += "\n\n" + otherBodies;

			return body;
		}

		/// <summary>
		/// Read the content of the eml from a file
		/// </summary>
		/// <param name="path">file info object of the file</param>
		/// <returns>
		///		The content of the file processed, if it exists;
		///		else, returns null
		/// </returns>
		public static EmlReader FromFile(String path)
		{
			var info = new FileInfo(path);

			if (!info.Exists)
				return null;

			var content = File.ReadAllLines(info.FullName);
			var creation = info.CreationTimeUtc;

			return new EmlReader(content, creation);
		}
	}
}
