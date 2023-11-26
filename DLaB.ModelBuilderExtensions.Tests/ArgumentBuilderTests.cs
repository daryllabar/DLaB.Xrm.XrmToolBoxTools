using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DLaB.ModelBuilderExtensions.Tests
{
    [TestClass]
    public class ArgumentBuilderTests
    {
        private readonly ArgumentBuilder _sut = new ArgumentBuilder("SettingsTemplateFilePath", "TestOutputFilePath");

        [TestMethod]
        public void GetParameters_ShouldRemoveParametersInTemplateFile()
        {
            var parameters = _sut.GetArguments();
            var expected = new[]
            {
                "/outdirectory:TestOutputFilePath",
                "/settingsTemplateFile:SettingsTemplateFilePath",
                "/splitfiles"
            };
            for (var i = 0; i < parameters.Length; i++)
            {
                var line = parameters[i];
                Assert.AreEqual(expected[i].Trim(), line.Trim(), $"Line {i + 1} does not match the expected!");
            }
        }
    }
}
