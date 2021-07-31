using System;
using Keon.Eml;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Eml.Tests
{
	[Binding]
	public class Base64Step : BaseStep
	{
		public Base64Step(ScenarioContext context) : base(context) { }

		private String encoded
		{
			get => get<String>("encoded");
			set => set("encoded", value);
		}

		private String decoded
		{
			get => get<String>("decoded");
			set => set("decoded", value);
		}



		[Given(@"the text (.+)")]
		public void GivenTheText(String base64)
		{
			encoded = base64;
		}

		[When(@"ask to decode it")]
		public void WhenAskToDecodeIt()
		{
			decoded = encoded.FromBase64();
		}

		[Then(@"the new text will be (.+)")]
		public void ThenTheNewTextWillBeTitle(String expected)
		{
			Assert.AreEqual(expected, decoded);
		}
	}
}
