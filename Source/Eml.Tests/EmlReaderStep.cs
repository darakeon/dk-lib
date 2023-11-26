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
			Assert.That(reader, Is.Null);
		}

		[Then(@"the result is not null")]
		public void ThenTheResultIsNotNull()
		{
			Assert.That(reader, Is.Not.Null);
		}

		[Then(@"the body is")]
		public void ThenTheBodyIs(Table table)
		{
			var expected = table.Rows
				.Select(r => r["Body"])
				.Select(trimAmpersand)
				.ToArray();

			var actual = reader.Body.Split("\n");

			Assert.That(actual.Length, Is.EqualTo(expected.Length));

			for (var l = 0; l < actual.Length; l++)
			{
				Assert.That(actual[l], Is.EqualTo(expected[l]));
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
				Assert.That(actual[l], Is.EqualTo(expected[l]), $"Line {l}");
			}

			Assert.That(actual.Length, Is.EqualTo(expected.Length));
		}

		[Then(@"the creation date is (\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}|null)")]
		public void ThenTheCreationDateIs(DateTime? creation)
		{
			Assert.That(reader.Creation, Is.EqualTo(creation));
		}

		[Then(@"the header is")]
		public void ThenTheHeaderIs(Table table)
		{
			var headers = table
				.CreateSet<KeyValuePair<String, String>>()
				.ToDictionary(p => p.Key, p => p.Value);

			foreach (var key in headers.Keys)
			{
				Assert.That(
					reader.Headers.ContainsKey(key),
					Is.True,
					$"Key '{key}' not found"
				);

				var expected = headers[key]
					.Replace("§", "\t");

				Assert.That(
					reader.Headers[key],
					Is.EqualTo(trimAmpersand(expected)),
					$"Key '{key}' with wrong value"
				);
			}

			Assert.That(reader.Headers.Count, Is.EqualTo(headers.Count));
		}

		[Then(@"the subject is (.+)")]
		public void ThenTheSubjectIs(String subject)
		{
			if (subject == "null")
				Assert.That(reader.Subject, Is.Null);
			else
				Assert.That(reader.Subject, Is.EqualTo(subject));
		}

		[Then(@"these contents will be the attachments")]
		public void ThenTheseContentsWillBeTheAttachments(Table table)
		{
			var fileNames = table.Rows
				.Select(r => r["File"])
				.Select(trimAmpersand)
				.ToArray();

			var actual = reader.Attachments;

			Assert.That(actual.Count, Is.EqualTo(fileNames.Length));

			for (var l = 0; l < actual.Count; l++)
			{
				var fileName = fileNames[l];
				var path = Path.Combine("examples", fileName);

				Assert.That(
					actual[l],
					Is.EqualTo(File.ReadAllText(path))
				);
			}
		}
	}
}
