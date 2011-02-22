﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Palaso.Tests.WritingSystems.Migration.WritingSystemsLdmlV0To1Migration;
using Palaso.TestUtilities;
using Palaso.WritingSystems;
using Palaso.WritingSystems.Migration;
using Palaso.WritingSystems.Migration.WritingSystemsLdmlV0To1Migration;

namespace Palaso.Tests.WritingSystems.Migration
{
	[TestFixture]
	public class WritingSystemDefinitionLdmlMigratorTests
	{
		private class TestEnvironment : IDisposable
		{
			readonly string _pathToLdml = Path.GetTempFileName();

			public TestEnvironment()
			{
				_pathToLdml = Path.GetTempFileName();
			}

			public string PathToWritingSystemLdmlFile
			{
				get { return _pathToLdml; }
			}

			public void WriteContentToWritingSystemLdmlFile(string contentToWrite)
			{
				File.WriteAllText(_pathToLdml, contentToWrite);
			}

			public void Dispose()
			{
				File.Delete(_pathToLdml);
			}
		}

		private TestEnvironment _environment;

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsFonipa_IpaStatusIsSetToIpa()
		{
			using (_environment = new TestEnvironment())
			{
				_environment.WriteContentToWritingSystemLdmlFile(LdmlFileContentForTests.Version0LdmlFile);
				var migrator = new WritingSystemDefinitionLdmlMigrator(1, _environment.PathToWritingSystemLdmlFile);
				migrator.MigrateIfNecassary();
				var versionGetter = new WritingSystemLdmlVersionGetter();
				Assert.AreEqual(1, versionGetter.GetFileVersion(_environment.PathToWritingSystemLdmlFile));
			}
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsFonipa_FonipaIsMovedToVariantSubtag()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsFonipa_FonipaIsRemovedFromLanguageSubtag()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsxDashEtic_IpaStatusIsSetToPhonetic()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsxDashEtic_xDashEticIsMovedToPrivateUseSubtag()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsxDashEtic_xDashEticIsRemovedFromLanguageSubtag()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsxDashEmic_IpaStatusIsSetToPhonemic()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsxDashEmic_xDashEmicIsMovedToPrivateUseSubtag()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsxDashEmic_xDashEmicIsRemovedFromLanguageSubtag()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagInLdmlContainsxDashaudio_VariantSubtagInLdmlContainsxDashaudio()
		{
			using (_environment = new TestEnvironment())
			{
				_environment.WriteContentToWritingSystemLdmlFile(LdmlFileContentForTests.CreateVersion0LdmlContent("en-x-audio",String.Empty,String.Empty,String.Empty));
				var migrator = new WritingSystemDefinitionLdmlMigrator(WritingSystemDefinition.LatestWritingSystemDefinitionVersion, _environment.PathToWritingSystemLdmlFile);
				migrator.MigrateIfNecassary();
				AssertThatXmlIn.File(_environment.PathToWritingSystemLdmlFile).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-audio']");
			}
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsxDashaudioAndScriptSubtagIsNotEmpty_ScriptSubtagPartsAreMovedToPrivateUseAndScriptIsOverwrittenToBecomeZxxx()
		{
			using (_environment = new TestEnvironment())
			{
				_environment.WriteContentToWritingSystemLdmlFile(LdmlFileContentForTests.CreateVersion0LdmlContent("en-x-audio", "th", String.Empty, String.Empty));
				var migrator = new WritingSystemDefinitionLdmlMigrator(WritingSystemDefinition.LatestWritingSystemDefinitionVersion, _environment.PathToWritingSystemLdmlFile);
				migrator.MigrateIfNecassary();
				AssertThatXmlIn.File(_environment.PathToWritingSystemLdmlFile).HasAtLeastOneMatchForXpath("/ldml/identity/script[@type='Zxxx']");
				AssertThatXmlIn.File(_environment.PathToWritingSystemLdmlFile).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-audio-th']");
			}
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsVariableCaseAuDio_IsVoiceIsSetToTrue()
		{
			using (_environment = new TestEnvironment())
			{
				_environment.WriteContentToWritingSystemLdmlFile(LdmlFileContentForTests.CreateVersion0LdmlContent("en-x-audio", String.Empty, String.Empty, String.Empty));
				var migrator = new WritingSystemDefinitionLdmlMigrator(WritingSystemDefinition.LatestWritingSystemDefinitionVersion, _environment.PathToWritingSystemLdmlFile);
				migrator.MigrateIfNecassary();
				AssertThatXmlIn.File(_environment.PathToWritingSystemLdmlFile).HasAtLeastOneMatchForXpath("/ldml/identity/variant[text()='x-AuDio']");
				throw new NotImplementedException();
			}
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsxDashaudio_xDashaudioIsMovedToPrivateUseSubtag()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsxDashaudio_xDashaudioIsRemovedFromLanguageSubtag()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsValidScript_ScriptIsMovedToScript()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsValidScript_ScriptIsRemovedFromLanguageSubtag()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsValidRegion_RegionIsMovedToRegion()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsValidRegion_RegionIsRemovedFromLanguageSubtag()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsValidVariant_VariantIsMovedToVariant()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsValidVariant_VariantIsRemovedFromLanguageSubtag()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsValidPrivateUse_xIsNotDuplicated()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsDataThatIsNotValidLanguageScriptRegionOrVariant_DataIsMovedToPrivateUse()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsMultipleValidLanguageSubtagsAsWellAsDataThatIsNotValidLanguageScriptRegionOrVariant_LanguageIsSetToFirstValidLanguageSubtag()
		{
			using (_environment = new TestEnvironment())
			{
				_environment.WriteContentToWritingSystemLdmlFile(LdmlFileContentForTests.CreateVersion0LdmlContent("x-bogus-en-audio-de-bogus2-x-", "", "", ""));
				var migrator = new WritingSystemDefinitionLdmlMigrator(WritingSystemDefinition.LatestWritingSystemDefinitionVersion, _environment.PathToWritingSystemLdmlFile);
				migrator.MigrateIfNecassary();
				AssertThatXmlIn.File(_environment.PathToWritingSystemLdmlFile).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='en']");
			}
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsMultipleValidLanguageSubtagsAsWellAsDataThatIsNotValidLanguageScriptRegionOrVariant_NonValidDataisMovedToPrivateUse()
		{
			using (_environment = new TestEnvironment())
			{
				_environment.WriteContentToWritingSystemLdmlFile(LdmlFileContentForTests.CreateVersion0LdmlContent("x-bogus-en-audio-de-bogus2-x-", "", "", ""));
				var migrator = new WritingSystemDefinitionLdmlMigrator(WritingSystemDefinition.LatestWritingSystemDefinitionVersion, _environment.PathToWritingSystemLdmlFile);
				migrator.MigrateIfNecassary();
				AssertThatXmlIn.File(_environment.PathToWritingSystemLdmlFile).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-audio-bogus-de-bogus2']");
			}
		}

		[Test]
		public void MigrateIfNecassary_ScriptSubtagContainsAnythingButValidScript_InvalidContentIsMovedToPrivateUse()
		{
			using (_environment = new TestEnvironment())
			{
				_environment.WriteContentToWritingSystemLdmlFile(LdmlFileContentForTests.CreateVersion0LdmlContent("", "x-bogus-en", "", ""));
				var migrator = new WritingSystemDefinitionLdmlMigrator(WritingSystemDefinition.LatestWritingSystemDefinitionVersion, _environment.PathToWritingSystemLdmlFile);
				migrator.MigrateIfNecassary();
				AssertThatXmlIn.File(_environment.PathToWritingSystemLdmlFile).HasNoMatchForXpath("/ldml/identity/script");
				AssertThatXmlIn.File(_environment.PathToWritingSystemLdmlFile).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-bogus-en']");
			}
		}

		[Test]
		public void MigrateIfNecassary_ScriptSubtagContainsValidScript_ScriptIsMigrated()
		{
			using (_environment = new TestEnvironment())
			{
				_environment.WriteContentToWritingSystemLdmlFile(LdmlFileContentForTests.CreateVersion0LdmlContent("", "x-bogus-en", "", ""));
				var migrator = new WritingSystemDefinitionLdmlMigrator(WritingSystemDefinition.LatestWritingSystemDefinitionVersion, _environment.PathToWritingSystemLdmlFile);
				migrator.MigrateIfNecassary();
				AssertThatXmlIn.File(_environment.PathToWritingSystemLdmlFile).HasNoMatchForXpath("/ldml/identity/script");
				AssertThatXmlIn.File(_environment.PathToWritingSystemLdmlFile).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-bogus-en']");
			}
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_ScriptSubtagContainsMultipleValidScriptSubtagsAsWellAsDataThatIsNotValidScript_NonValidDataisMovedToPrivateUse()
		{
			using (_environment = new TestEnvironment())
			{
				_environment.WriteContentToWritingSystemLdmlFile(LdmlFileContentForTests.CreateVersion0LdmlContent("", "x-bogus-Latn-test-Zxxx-bogus2-x-", "", ""));
				var migrator = new WritingSystemDefinitionLdmlMigrator(WritingSystemDefinition.LatestWritingSystemDefinitionVersion, _environment.PathToWritingSystemLdmlFile);
				migrator.MigrateIfNecassary();
				AssertThatXmlIn.File(_environment.PathToWritingSystemLdmlFile).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-bogus-test-Zxxx-bogus2']");
			}
		}

		[Test]
		public void MigrateIfNecassary_ScriptSubtagContainsMultipleValidScriptSubtagsAsWellAsDataThatIsNotValidScriptAsWellAsAudio_NonValidDataisMovedToPrivateUseAsScriptIsZxxx()
		{
			using (_environment = new TestEnvironment())
			{
				_environment.WriteContentToWritingSystemLdmlFile(LdmlFileContentForTests.CreateVersion0LdmlContent("", "x-bogus-Latn-audio-Afak-bogus2-x-", "", ""));
				var migrator = new WritingSystemDefinitionLdmlMigrator(WritingSystemDefinition.LatestWritingSystemDefinitionVersion, _environment.PathToWritingSystemLdmlFile);
				migrator.MigrateIfNecassary();
				AssertThatXmlIn.File(_environment.PathToWritingSystemLdmlFile).HasAtLeastOneMatchForXpath("/ldml/identity/script[@type='Zxxx']");
				AssertThatXmlIn.File(_environment.PathToWritingSystemLdmlFile).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-bogus-Latn-audio-Afak-bogus2']");
			}
		}

		[Test]
		public void MigrateIfNecassary_LanguageAndScriptSubtagsContainInvalidData_AllInvalidDataIsMovedToPrivateUse()
		{
			using (_environment = new TestEnvironment())
			{
				_environment.WriteContentToWritingSystemLdmlFile(LdmlFileContentForTests.CreateVersion0LdmlContent("bogus-stuff", "more-bogus", "", ""));
				var migrator = new WritingSystemDefinitionLdmlMigrator(WritingSystemDefinition.LatestWritingSystemDefinitionVersion, _environment.PathToWritingSystemLdmlFile);
				migrator.MigrateIfNecassary();
				AssertThatXmlIn.File(_environment.PathToWritingSystemLdmlFile).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-bogus-stuff-more']");
			}
		}

		[Test]
		public void MigrateIfNecassary_RegionSubtagContainsAnythingButValidRegion_InvalidContentIsMovedToPrivateUse()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_VariantSubtagContainsAnythingButValidVariantOrPrivateUse_InvalidContentIsMovedToPrivateUse()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsDatathatIsNotValidLanguageScriptRegionOrVariantAndIsDuplicateOfDataInPrivateUse_DataIsNotDuplicatedInPrivateUse()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsDuplicateValidLanguageSubtag_DuplicateIsRemoved()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsDuplicateValidScript_DuplicateIsRemoved()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsDuplicateValidRegion_DuplicateIsRemoved()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsDuplicateValidVariant_DuplicateIsRemoved()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsDuplicatePrivateUse_DuplicateIsRemoved()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_ScriptSubtagContainsAnythingButValidScriptAndIsDuplicateOfDataInPrivateUse_DataIsNotDuplicatedInPrivateUse()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_RegionSubtagContainsAnythingButValidRegionAndIsDuplicateOfDataInPrivateUse_DataIsNotDuplicatedInPrivateUse()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_VariantSubtagContainsAnythingButValidVariantOrprivateUseAndIsDuplicateOfDataInPrivateUse_DataIsNotDuplicatedInPrivateUse()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_OriginalFileContainsCustomLdmlData_DataIsCopied()
		{
			//Test for fallback, localeDisplayNames, characters, delimiters, measurement, dates, numbers, units, listPatterns, posix, segmentations, rbnf and references
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_OriginalFileContainsDefaultFontFamilyInfo_DataIsMigrated()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_OriginalFileIsNotLdmlVWhat_Throw()
		{
			//Need to make sure we are reading/writing the right vrsions of Ldml.rename LdmlAdaptor to LdmlDataMapper
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_LdmlIsVersion0_IsLatestVersion()
		{
			using (_environment = new TestEnvironment())
			{
				_environment.WriteContentToWritingSystemLdmlFile(LdmlFileContentForTests.Version0LdmlFile);
				var migrator = new WritingSystemDefinitionLdmlMigrator(1, _environment.PathToWritingSystemLdmlFile);
				migrator.MigrateIfNecassary();
				var versionGetter = new WritingSystemLdmlVersionGetter();
				Assert.AreEqual(1, versionGetter.GetFileVersion(_environment.PathToWritingSystemLdmlFile));
			}
		}

		[Test]
		public void MigrateIfNecassary_LdmlIsVersion0_NeedsMigratingIsTrue()
		{
			using (_environment = new TestEnvironment())
			{
				_environment.WriteContentToWritingSystemLdmlFile(LdmlFileContentForTests.Version1LdmlFile);
				var migrator = new WritingSystemDefinitionLdmlMigrator(1, _environment.PathToWritingSystemLdmlFile);
				Assert.IsFalse(migrator.FileNeedsMigrating);
			}
		}

		[Test]
		public void MigrateIfNecassary_LdmlIsAlreadyLatestVersion_NeedsMigratingIsFalse()
		{
			using (_environment = new TestEnvironment())
			{
				_environment.WriteContentToWritingSystemLdmlFile(LdmlFileContentForTests.Version1LdmlFile);
				var migrator = new WritingSystemDefinitionLdmlMigrator(1, _environment.PathToWritingSystemLdmlFile);
				Assert.IsFalse(migrator.FileNeedsMigrating);
			}
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsExcessiveXs_XsAreIgnoredAndNonIanaConformDataIsWrittenToPrivateUse()
		{
			using (_environment = new TestEnvironment())
			{
				_environment.WriteContentToWritingSystemLdmlFile(LdmlFileContentForTests.CreateVersion0LdmlContent("en-x-x-audio", "","",""));
				var migrator = new WritingSystemDefinitionLdmlMigrator(WritingSystemDefinition.LatestWritingSystemDefinitionVersion, _environment.PathToWritingSystemLdmlFile);
				migrator.MigrateIfNecassary();
				AssertThatXmlIn.File(_environment.PathToWritingSystemLdmlFile).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='en']");
				AssertThatXmlIn.File(_environment.PathToWritingSystemLdmlFile).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-audio']");
			}
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsValidSubtagWithPrecedingX_LanguageSubtagIsSetToValidSubtag()
		{
			using (_environment = new TestEnvironment())
			{
				_environment.WriteContentToWritingSystemLdmlFile(LdmlFileContentForTests.CreateVersion0LdmlContent("x-en", "", "", ""));
				var migrator = new WritingSystemDefinitionLdmlMigrator(WritingSystemDefinition.LatestWritingSystemDefinitionVersion, _environment.PathToWritingSystemLdmlFile);
				migrator.MigrateIfNecassary();
				AssertThatXmlIn.File(_environment.PathToWritingSystemLdmlFile).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='en']");
			}
		}

		[Test]
		public void MigrateIfNecassary_ScriptSubtagContainsExcessiveXs_XsAreIgnoredAndNonIanaConformDataIsWrittenToPrivateUse()
		{
			using (_environment = new TestEnvironment())
			{
				_environment.WriteContentToWritingSystemLdmlFile(LdmlFileContentForTests.CreateVersion0LdmlContent("", "x-Latn-x-audio", "", ""));
				var migrator = new WritingSystemDefinitionLdmlMigrator(WritingSystemDefinition.LatestWritingSystemDefinitionVersion, _environment.PathToWritingSystemLdmlFile);
				migrator.MigrateIfNecassary();
				AssertThatXmlIn.File(_environment.PathToWritingSystemLdmlFile).HasAtLeastOneMatchForXpath("/ldml/identity/script[@type='Latn']");
				AssertThatXmlIn.File(_environment.PathToWritingSystemLdmlFile).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-audio']");
			}
		}

		[Test]
		public void MigrateIfNecassary_ScriptSubtagContainsValidSubtagWithPrecedingX_ScriptSubtagIsSetToValidSubtag()
		{
			using (_environment = new TestEnvironment())
			{
				_environment.WriteContentToWritingSystemLdmlFile(LdmlFileContentForTests.CreateVersion0LdmlContent("", "x-latn", "", ""));
				var migrator = new WritingSystemDefinitionLdmlMigrator(WritingSystemDefinition.LatestWritingSystemDefinitionVersion, _environment.PathToWritingSystemLdmlFile);
				migrator.MigrateIfNecassary();
				AssertThatXmlIn.File(_environment.PathToWritingSystemLdmlFile).HasAtLeastOneMatchForXpath("/ldml/identity/script[@type='latn']");
			}
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagInLdmlIsEmpty_LanguageSubtagIsSetToQaa()
		{
			using (_environment = new TestEnvironment())
			{
				_environment.WriteContentToWritingSystemLdmlFile(LdmlFileContentForTests.CreateVersion0LdmlContent("", "", "", ""));
				var migrator = new WritingSystemDefinitionLdmlMigrator(WritingSystemDefinition.LatestWritingSystemDefinitionVersion, _environment.PathToWritingSystemLdmlFile);
				migrator.MigrateIfNecassary();
				AssertThatXmlIn.File(_environment.PathToWritingSystemLdmlFile).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='qaa']");
			}
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagEndsInDashXDash_DashXDashIsRemoved()
		{
			using (_environment = new TestEnvironment())
			{
				_environment.WriteContentToWritingSystemLdmlFile(LdmlFileContentForTests.CreateVersion0LdmlContent("en-x-", "", "", ""));
				var migrator = new WritingSystemDefinitionLdmlMigrator(WritingSystemDefinition.LatestWritingSystemDefinitionVersion, _environment.PathToWritingSystemLdmlFile);
				migrator.MigrateIfNecassary();
				AssertThatXmlIn.File(_environment.PathToWritingSystemLdmlFile).HasAtLeastOneMatchForXpath("/ldml/identity/language[@type='en']");
			}
		}

		[Test]
		public void MigrateIfNecassary_ScriptSubtagEndsInDashXDash_DashXDashIsRemoved()
		{
			using (_environment = new TestEnvironment())
			{
				_environment.WriteContentToWritingSystemLdmlFile(LdmlFileContentForTests.CreateVersion0LdmlContent("", "Latn-x-", "", ""));
				var migrator = new WritingSystemDefinitionLdmlMigrator(WritingSystemDefinition.LatestWritingSystemDefinitionVersion, _environment.PathToWritingSystemLdmlFile);
				migrator.MigrateIfNecassary();
				AssertThatXmlIn.File(_environment.PathToWritingSystemLdmlFile).HasAtLeastOneMatchForXpath("/ldml/identity/script[@type='Latn']");
			}

		}

		[Test]
		public void MigrateIfNecassary_RegionSubtagEndsInDashXDash_DashXDashIsRemoved()
		{
			using (_environment = new TestEnvironment())
			{
				_environment.WriteContentToWritingSystemLdmlFile(LdmlFileContentForTests.CreateVersion0LdmlContent("", "", "US-x-", ""));
				var migrator = new WritingSystemDefinitionLdmlMigrator(WritingSystemDefinition.LatestWritingSystemDefinitionVersion, _environment.PathToWritingSystemLdmlFile);
				migrator.MigrateIfNecassary();
				AssertThatXmlIn.File(_environment.PathToWritingSystemLdmlFile).HasAtLeastOneMatchForXpath("/ldml/identity/territory[@type='US']");
			}

		}

		[Test]
		public void MigrateIfNecassary_VariantSubtagEndsInDashXDash_DashXDashIsRemoved()
		{
			using (_environment = new TestEnvironment())
			{
				_environment.WriteContentToWritingSystemLdmlFile(LdmlFileContentForTests.CreateVersion0LdmlContent("", "", "", "x-bogus-x-"));
				var migrator = new WritingSystemDefinitionLdmlMigrator(WritingSystemDefinition.LatestWritingSystemDefinitionVersion, _environment.PathToWritingSystemLdmlFile);
				migrator.MigrateIfNecassary();
				AssertThatXmlIn.File(_environment.PathToWritingSystemLdmlFile).HasAtLeastOneMatchForXpath("/ldml/identity/variant[@type='x-bogus']");
			}
		}

		[Test]
		public void MigrateIfNecassary_LanguageSubtagContainsNonAlphaNumericCharacters_NonAlphaNumericCharactersAreRemoved()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_ScriptSubtagContainsNonAlphaNumericCharacters_NonAlphaNumericCharactersAreRemoved()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_RegionSubtagContainsNonAlphaNumericCharacters_NonAlphaNumericCharactersAreRemoved()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_VariantSubtagContainsNonAlphaNumericCharacters_NonAlphaNumericCharactersAreRemoved()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MigrateIfNecassary_RfcTagContainsOnlyNonAlphaNumericCharactersAndEndsInDashXDash_WritingsystemIsSetToqaa()
		{
			throw new NotImplementedException();
		}
	}
}