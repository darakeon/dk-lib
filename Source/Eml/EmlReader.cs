using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Keon.Eml
{
	/// <summary>
	/// Process EML files into objects
	/// </summary>
	public class EmlReader
	{
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

		private IDictionary<String, String> headers { get; set; }

		/// <summary>
		/// Creation date - only if eml file is given
		/// </summary>
		public DateTime? Creation { get; }

		private Boolean isBase64
		{
			get
			{
				var key = "Content-Transfer-Encoding";
				return headers.ContainsKey(key)
				       && headers[key] == "base64";
			}
		}

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
			headers = new Dictionary<String, String>();

			var l = 0;
			for (; l < content.Length; l++)
			{
				var line = content[l];

				if (line.StartsWith(" ") || line.StartsWith("\t"))
				{
					var key = headers.Keys.Last();
					headers[key] += line;
				}
				else if (line.Contains(":"))
				{
					var parts = line.Split(":", 2);
					headers.Add(parts[0], parts[1].Trim());
				}
				else
				{
					break;
				}
			}

			return content.Skip(l).ToArray();
		}

		private void setSubject()
		{
			var key = "Subject";

			if (!headers.ContainsKey(key))
				return;

			Subject = headers[key];

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
			var base64 = isBase64;

			if (!base64)
			{
				content = content
					.Where(c => !String.IsNullOrEmpty(c))
					.ToArray();
			}

			Body = String.Join("\n", content);

			if (base64)
				Body = Body.FromBase64();

			Body = Body
					.Replace("\"\"", "\"")
					.Replace("=\n", "")
					.Replace("=0A", "\n")
					.Replace("=3D", "=")
				;
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
