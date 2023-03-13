using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using DLaB.EarlyBoundGenerator.Settings;

namespace DLaB.EarlyBoundGenerator.Api.Tests
{
    [TestClass]
    public class LogicTests
    {
        [TestMethod]
        public void UpdateBuilderSettingsJson_WithNoFile_Should_CreateFile()
        {
            var config = EarlyBoundGeneratorConfig.GetDefault();
            config.RootPath = Path.Combine(Path.GetTempPath(), nameof(UpdateBuilderSettingsJson_WithNoFile_Should_CreateFile));
            new LogicV2(config).UpdateBuilderSettingsJson();

            var settings = File.ReadAllLines(Path.Combine(config.RootPath, "builderSettings.json"));
            var expected = TestProject.GetResourceText("Resources.DefaultBuilderSettings.json").Split(new []{Environment.NewLine}, StringSplitOptions.None);
            for (var i = 0; i < settings.Length; i++)
            {
                var line = settings[i];
                Assert.AreEqual(expected[i].Trim(), line.Trim(), $"Line {i+1} does not match the expected!");
            }
        }
    }
}
