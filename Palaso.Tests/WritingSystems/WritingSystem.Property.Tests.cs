using System;
using System.Collections.Generic;
using NUnit.Framework;
using Palaso.WritingSystems;

namespace Palaso.Tests.WritingSystems
{
	[TestFixture]
	public class WritingSystemPropertyTests
	{
		[SetUp]
		public void Setup()
		{

		}

		[TearDown]
		public void TearDown()
		{

		}


		[Test]
		public void DisplayLabelWhenUnknown()
		{
			WritingSystemDefinition ws = new WritingSystemDefinition();
			Assert.AreEqual("???", ws.DisplayLabel);
		}

		[Test]
		public void DisplayLabelWhenJustISO()
		{
			WritingSystemDefinition ws = new WritingSystemDefinition();
			ws.ISO = "abc";
			Assert.AreEqual("abc", ws.DisplayLabel);
		}

		[Test]
		public void DisplayLabelWhenHasAbbreviation()
		{
			WritingSystemDefinition ws = new WritingSystemDefinition();
			ws.ISO = "abc";
			ws.Abbreviation = "xyz";
			Assert.AreEqual("xyz", ws.DisplayLabel);
		}

		[Test]
		public void DisplayLabelWhenJustLanguage()
		{
			WritingSystemDefinition ws = new WritingSystemDefinition();
			ws.LanguageName = "abcdefghijk";
			Assert.AreEqual("abcd", ws.DisplayLabel);
		}

		[Test]
		public void Rfc4646WhenJustISO()
		{
			WritingSystemDefinition ws = new WritingSystemDefinition("iso","","","","","", false);
			Assert.AreEqual("iso", ws.RFC4646);
		}
		[Test]
		public void Rfc4646WhenIsoAndScript()
		{
			WritingSystemDefinition ws = new WritingSystemDefinition("iso", "scrip", "", "", "", "", false);
			Assert.AreEqual("iso-scrip", ws.RFC4646);
		}

		[Test]
		public void Rfc4646WhenIsoAndRegion()
		{
			WritingSystemDefinition ws = new WritingSystemDefinition("iso", "", "where", "", "", "", false);
			Assert.AreEqual("iso-where", ws.RFC4646);
		}
		[Test]
		public void Rfc4646WhenIsoScriptRegionVariant()
		{
			WritingSystemDefinition ws = new WritingSystemDefinition("iso", "scrip", "regn", "var", "", "", false);
			Assert.AreEqual("iso-scrip-regn-var", ws.RFC4646);
		}

		[Test]
		public void ReadsScriptRegistry()
		{
			WritingSystemDefinition ws = new WritingSystemDefinition();
			Assert.Greater(ws.ScriptOptions.Count,4);
		}


		[Test]
		public void ReadsISORegistry()
		{
			Assert.Greater(WritingSystemDefinition.LanguageCodes.Count, 100);
		}


		[Test]
		public void VerboseDescriptionWhenJustISO()
		{
			WritingSystemDefinition ws = new WritingSystemDefinition("iso", "", "", "", "", "", false);
			Assert.AreEqual("???. (iso)", ws.VerboseDescription);
		}
		[Test]
		public void VerboseDescriptionWhenIsoAndScript()
		{
			WritingSystemDefinition ws = new WritingSystemDefinition("iso", "Kore", "", "", "", "", false);
			Assert.AreEqual("??? written in Korean script. (iso-Kore)", ws.VerboseDescription);
		}

		[Test]
		public void VerboseDescriptionWhenIsoAndRegion()
		{
			WritingSystemDefinition ws = new WritingSystemDefinition("iso", "", "flubville", "", "foobar", "", false);
			Assert.AreEqual("foobar in flubville. (iso-flubville)", ws.VerboseDescription);
		}
		[Test]
		public void VerboseDescriptionWhenIsoScriptRegionVariant()
		{
			WritingSystemDefinition ws = new WritingSystemDefinition("iso", "Kore", "regn", "western", "foobar", "", false);
			Assert.AreEqual("western foobar in regn written in Korean script. (iso-Kore-regn-western)", ws.VerboseDescription);
		}

		[Test]
		public void HasLotsOfScriptOptions()
		{
			WritingSystemDefinition ws = new WritingSystemDefinition();
			Assert.Greater(ws.ScriptOptions.Count, 40);
		}


		[Test]
		public void CurrentScriptOptionReturnCorrectScript()
		{
			WritingSystemDefinition ws = new WritingSystemDefinition("iso", "Kore", "", "", "", "", false);
			Assert.AreEqual("Korean", ws.CurrentScriptOption.Label);
		}

		[Test]
		public void CurrentScriptOptionReturnsNullWithUnrecognizedScript()
		{
			WritingSystemDefinition ws = new WritingSystemDefinition("iso", "blah", "", "", "", "", false);
			Assert.IsNull(ws.CurrentScriptOption);
		}
	}
}