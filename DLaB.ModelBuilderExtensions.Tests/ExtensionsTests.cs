using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace DLaB.ModelBuilderExtensions.Tests
{
    [TestClass]
    public class ExtensionsTests
    {
        private const int English = 1033;
        private const int German = 1031;

        [TestMethod]
        public void GetLocalOrDefaultText_WithLanguageCode_WhenLabelExistsForCode_ReturnsCorrectLabel()
        {
            var label = new Label();
            label.LocalizedLabels.Add(new LocalizedLabel("English Label", English));
            label.LocalizedLabels.Add(new LocalizedLabel("German Label", German));

            var result = label.GetLocalOrDefaultText(English);

            Assert.AreEqual("English Label", result, "Should return English label when English code is specified and English label exists.");
        }

        [TestMethod]
        public void GetLocalOrDefaultText_WithLanguageCode_WhenGermanUserLocalizedAndEnglishInLocalized_ReturnsEnglish()
        {
            // Simulate a German user: UserLocalizedLabel = German, LocalizedLabels = [German, English]
            var label = new Label(new LocalizedLabel("German Label", German), new[]
            {
                new LocalizedLabel("German Label", German),
                new LocalizedLabel("English Label", English)
            });

            var result = label.GetLocalOrDefaultText(English);

            Assert.AreEqual("English Label", result, "Should return English label even when UserLocalizedLabel is German.");
        }

        [TestMethod]
        public void GetLocalOrDefaultText_WithLanguageCode_WhenRequestedCodeNotFound_FallsBackToUserLocalizedLabel()
        {
            // Simulate a German-only org: UserLocalizedLabel = German, LocalizedLabels = [German only]
            var label = new Label(new LocalizedLabel("German Label", German), new[]
            {
                new LocalizedLabel("German Label", German)
            });

            var result = label.GetLocalOrDefaultText(English);

            Assert.AreEqual("German Label", result, "Should fall back to UserLocalizedLabel when English is not found in LocalizedLabels.");
        }

        [TestMethod]
        public void GetLocalOrDefaultText_WithZeroLanguageCode_UsesSameDefaultBehavior()
        {
            var label = new Label(new LocalizedLabel("User Label", English), new[]
            {
                new LocalizedLabel("Localized Label", German)
            });

            var resultWithZero = label.GetLocalOrDefaultText(0);
            var resultDefault = label.GetLocalOrDefaultText();

            Assert.AreEqual(resultDefault, resultWithZero, "Language code <= 0 should use the same behavior as no language code.");
        }

        [TestMethod]
        public void GetLocalOrDefaultText_WithLanguageCode_WhenLabelIsEmpty_FallsBackToUserLocalizedLabel()
        {
            // Language code found but label text is empty - should fall back
            var label = new Label(new LocalizedLabel("German Label", German), new[]
            {
                new LocalizedLabel(string.Empty, English)
            });

            var result = label.GetLocalOrDefaultText(English);

            Assert.AreEqual("German Label", result, "Should fall back to UserLocalizedLabel when LocalizedLabel has empty text.");
        }
    }
}
