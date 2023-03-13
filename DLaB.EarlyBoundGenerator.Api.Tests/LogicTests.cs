using DLaB.EarlyBoundGenerator.Settings;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace DLaB.EarlyBoundGenerator.Api.Tests
{
    [TestClass]
    public class LogicTests
    {
        private static readonly EarlyBoundGeneratorConfig Config = CreateConfig();
        private readonly LogicV2 _sut = new LogicV2(Config);

        [TestMethod]
        public void UpdateBuilderSettingsJson_ShouldCreateFile_WhenNoFileExists()
        {
            _sut.UpdateBuilderSettingsJson();
            var settings = File.ReadAllLines(Path.Combine(Config.RootPath, "builderSettings.json"));
            var expected = TestProject.GetResourceText("Resources.DefaultBuilderSettings.json").Split(new []{Environment.NewLine}, StringSplitOptions.None);
            for (var i = 0; i < settings.Length; i++)
            {
                var line = settings[i];
                Assert.AreEqual(expected[i].Trim(), line.Trim(), $"Line {i+1} does not match the expected!");
            }
        }

        [TestMethod]
        public void GetParameters_ShouldRemoveParametersInTemplateFile()
        {
            var modelParameters = new ModelBuilderInvokeParameters
            {
                OutputFile = "TestOutputFilePath",
                SettingsTemplateFile = "SettingsTemplateFilePath"
            };
            var parameters = _sut.GetParameters(modelParameters);
            var expected = new []
            {
                "/out:TestOutputFilePath",
                "/settingsTemplateFile:SettingsTemplateFilePath"
            };
            for (var i = 0; i < parameters.Length; i++)
            {
                var line = parameters[i];
                Assert.AreEqual(expected[i].Trim(), line.Trim(), $"Line {i + 1} does not match the expected!");
            }
        }

        private static EarlyBoundGeneratorConfig CreateConfig()
        {
            var config = EarlyBoundGeneratorConfig.GetDefault();
            config.RootPath = Path.Combine(Path.GetTempPath(), nameof(LogicTests));
            return config;
        }
    }
}
