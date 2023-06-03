using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Keon.Eml;
using NUnit.Framework;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Eml.Tests
{
	[Binding]
	public class EmlReaderStep : BaseStep
	{
		public EmlReaderStep(ScenarioContext context) : base(context) { }

		private String file
		{
			get => get<String>("file");
			set => set("file", value);
		}

		private String content
		{
			get => get<String>("content");
			set => set("content", value);
		}

		private String[] contentLines
		{
			get => get<String[]>("contentLines");
			set => set("contentLines", value);
		}

		private DateTime? creation
		{
			get => get<DateTime?>("creation");
			set => set("creation", value);
		}

		private EmlReader reader
		{
			get => get<EmlReader>("reader");
			set => set("reader", value);
		}



		[Given(@"the file name ([\w\-]+\.eml)")]
		public void GivenTheFileName_Eml(String filename)
		{
			file = Path.Combine("examples", filename);
		}

		[Given(@"the content (.+)")]
		public void GivenTheContent(String content)
		{
			this.content = content.Replace("\\n", "\n");
		}

		[Given(@"the content")]
		public void GivenTheContent(Table table)
		{
			contentLines = table.Rows
				.Select(r => r["Content"])
				.Select(trimAmpersand)
				.ToArray();
		}

		[Given(@"the creation date (\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2})")]
		public void GivenTheCreationDate(DateTime creation)
		{
			this.creation = creation;
		}


		[When(@"file read is called")]
		public void WhenFileReadIsCalled()
		{
			reader = EmlReader.FromFile(file);
		}

		[When(@"file read is called without decorations")]
		public void WhenFileReadIsCalledWithoutDecorations()
		{
			reader = EmlReader.FromFile(file, false);
		}

		[When(@"content read is called")]
		public void WhenContentReadIsCalled()
		{
			reader = content != null
				? new EmlReader(content, creation)
				: new EmlReader(contentLines, creation);
		}


		[Then(@"the result is null")]
		public void ThenTheResultIsNull()
		{
			Assert.IsNull(reader);
		}

		[Then(@"the result is not null")]
		public void ThenTheResultIsNotNull()
		{
			Assert.IsNotNull(reader);
		}

		[Then(@"the body is")]
		public void ThenTheBodyIs(Table table)
		{
			var expected = table.Rows
				.Select(r => r["Body"])
				.Select(trimAmpersand)
				.ToArray();

			var actual = reader.Body.Split("\n");

			Assert.AreEqual(expected.Length, actual.Length);

			for (var l = 0; l < actual.Length; l++)
			{
				Assert.AreEqual(expected[l], actual[l]);
			}
		}

		[Then(@"the body is same as content of (.+)")]
		public void ThenTheBodyIsSameAsContentOf(String filename)
		{
			var path = Path.Combine("examples", filename);
			var expected = File.ReadAllLines(path);

			var actual = reader.Body.Split("\n");

			for (var l = 0; l < Math.Min(actual.Length, expected.Length); l++)
			{
				Assert.AreEqual(expected[l], actual[l], $"Line {l}");
			}

			Assert.AreEqual(expected.Length, actual.Length);
		}

		[Then(@"the creation date is (\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}|null)")]
		public void ThenTheCreationDateIs(DateTime? creation)
		{
			Assert.AreEqual(creation, reader.Creation);
		}

		[Then(@"the header is")]
		public void ThenTheHeaderIs(Table table)
		{
			var headers = table
				.CreateSet<KeyValuePair<String, String>>()
				.ToDictionary(p => p.Key, p => p.Value);

			foreach (var key in headers.Keys)
			{
				Assert.True(
					reader.Headers.ContainsKey(key),
					$"Key '{key}' not found"
				);

				var expected = headers[key]
					.Replace("§", "\t");

				Assert.AreEqual(
					trimAmpersand(expected),
					reader.Headers[key],
					$"Key '{key}' with wrong value"
				);
			}

			Assert.AreEqual(headers.Count, reader.Headers.Count);
		}

		[Then(@"the subject is (.+)")]
		public void ThenTheSubjectIs(String subject)
		{
			if (subject == "null")
				Assert.Null(reader.Subject);
			else
				Assert.AreEqual(subject, reader.Subject);
		}

		[Then(@"these contents will be the attachments")]
		public void ThenTheseContentsWillBeTheAttachments(Table table)
		{
			var fileNames = table.Rows
				.Select(r => r["File"])
				.Select(trimAmpersand)
				.ToArray();

			var actual = reader.Attachments;

			Assert.AreEqual(fileNames.Length, actual.Count);

			for (var l = 0; l < actual.Count; l++)
			{
				var fileName = fileNames[l];
				var path = Path.Combine("examples", fileName);

				Assert.AreEqual(
					File.ReadAllText(path),
					actual[l]
				);
			}
		}
	}
}
