using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Keon.Eml
{
	/// <summary>
	/// Process EML files into objects
	/// </summary>
	public class EmlReader
	{
		private EmlReader(
			String subject, String body,
			IDictionary<String, String> headers,
			DateTime? creation
		)
		{
			Subject = subject;
			Body = body;
			Headers = headers;
			Creation = creation;
		}

		/// <summary>
		/// Subject of the e-mail, decoded in case of original being in Base64
		/// </summary>
		public String Subject { get; }

		/// <summary>
		/// E-mail body, decoded in case of original being in Base64
		/// </summary>
		public String Body { get; }

		/// <summary>
		/// Headers of the e-mail
		/// </summary>
		public IDictionary<String, String> Headers { get; }

		/// <summary>
		/// Creation date - only if eml file is given
		/// </summary>
		public DateTime? Creation { get; }

		/// <summary>
		/// Read the content of the eml from a file
		/// </summary>
		/// <param name="path">file info object of the file</param>
		/// <returns>
		///		The content of the file processed, if it exists;
		///		else, returns null
		/// </returns>
		public static EmlReader ReadFromFile(String path)
		{
			return Read(new FileInfo(path));
		}

		/// <summary>
		/// Read the content of the eml from a file
		/// </summary>
		/// <param name="email">file info object of the file</param>
		/// <returns>
		///		The content of the file processed, if it exists;
		///		else, returns null
		/// </returns>
		public static EmlReader Read(FileInfo email)
		{
			if (!email.Exists)
				return null;

			var content = File.ReadAllLines(email.FullName);
			var creation = email.CreationTimeUtc;

			return Read(content, creation);
		}

		/// <summary>
		/// Read the content of the eml from a file
		/// </summary>
		/// <param name="content">Content of the eml file</param>
		/// <param name="creation">Creation date of e-mail</param>
		public static EmlReader Read(String content, DateTime? creation = null)
		{
			return Read(content.Split("\n"), creation);
		}

		/// <summary>
		/// Read the content of the eml from a file
		/// </summary>
		/// <param name="content">Content of the eml file</param>
		/// <param name="creation">Creation date of e-mail</param>
		public static EmlReader Read(String[] content, DateTime? creation = null)
		{
			var headers = new Dictionary<String, String>();

			var l = 0;
			for (; l < content.Length && content[l] != ""; l++)
			{
				var line = content[l];

				if (line.StartsWith(" "))
				{
					var key = headers.Keys.Last();
					headers[key] += line;
				}
				else
				{
					var parts = line.Split(":", 2);
					headers.Add(parts[0], parts[1].Trim());
				}
			}

			var subject = headers["Subject"];

			if (subject.Contains("utf-8"))
			{
				subject = String.Join("",
					subject
						.Split(" ")
						.Select(s => convert(s[10..^2]))
				);
			}

			var base64 = headers["Content-Transfer-Encoding"] == "base64";

			content = content.Skip(l).ToArray();

			if (!base64)
			{
				content = content
					.Where(c => !String.IsNullOrEmpty(c))
					.ToArray();
			}

			var body = String.Join("\n", content);

			if (base64)
				body = convert(body);

			body = body
				.Replace("\"\"", "\"")
				.Replace("=\n", "")
				.Replace("=0A", "\n")
				.Replace("=3D", "=")
				;

			return new EmlReader(subject, body, headers, creation);
		}

		private static String convert(String text)
		{
			var bytes = Convert.FromBase64String(text);
			return Encoding.UTF8.GetString(bytes);
		}
	}
}
