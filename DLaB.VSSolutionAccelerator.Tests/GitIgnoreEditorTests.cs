using DLaB.VSSolutionAccelerator.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace DLaB.VSSolutionAccelerator.Tests
{
    [TestClass]
    public class GitIgnoreEditorTests
    {
        private string _gitIgnorePath;
        private InitializeSolutionInfo _solutionInfo;

        [TestInitialize]
        public void Initialize()
        {
            var directory = TempDir.Create().Name;
            _solutionInfo = TestBase.InitializeSolutionInfo(Path.Combine(directory, "solution.sln"));
            _solutionInfo.ConfigureXrmUnitTest = true;
            _gitIgnorePath = Path.Combine(directory, ".gitignore");
        }

        [TestMethod]
        public void AddXrmUnitTestConfig_WhenConfigureXrmUnitTestIsFalse_Should_NotCreateGitIgnore()
        {
            // Arrange
            _solutionInfo.ConfigureXrmUnitTest = false;

            // Act
            GitIgnoreEditor.AddXrmUnitTestConfig(_solutionInfo);

            // Assert
            Assert.IsFalse(File.Exists(_gitIgnorePath));
        }

        [TestMethod]
        public void AddXrmUnitTestConfig_WhenGitIgnoreDoesNotExist_Should_CreateGitIgnore()
        {
            // Act
            GitIgnoreEditor.AddXrmUnitTestConfig(_solutionInfo);

            // Assert
            Assert.IsTrue(File.Exists(_gitIgnorePath));
            var gitIgnoreContent = File.ReadAllText(_gitIgnorePath);
            Assert.IsTrue(gitIgnoreContent.Contains(GitIgnoreEditor.UserSpecificSection));
        }

        [TestMethod]
        public void AddXrmUnitTestConfig_WhenGitIgnoreDoesNotContainIgnore_Should_AddIgnoreXrmUnitTestUserConfig()
        {
            // Arrange
            File.WriteAllText(_gitIgnorePath, string.Empty);

            // Act
            GitIgnoreEditor.AddXrmUnitTestConfig(_solutionInfo);

            // Assert
            var gitIgnoreContent = File.ReadAllText(_gitIgnorePath);
            Assert.IsTrue(gitIgnoreContent.Contains(GitIgnoreEditor.IgnoreXrmUnitTestUserConfig));
        }

        [TestMethod]
        public void AddXrmUnitTestConfig_WhenGitIgnoreAlreadyContainsIgnoreXrmUnitTestUserConfig_Should_NotModifyGitIgnore()
        {
            // Arrange
            File.WriteAllText(_gitIgnorePath, GitIgnoreEditor.IgnoreXrmUnitTestUserConfig);

            // Act
            GitIgnoreEditor.AddXrmUnitTestConfig(_solutionInfo);

            // Assert
            var updatedGitIgnoreContent = File.ReadAllText(_gitIgnorePath);
            Assert.AreEqual(GitIgnoreEditor.IgnoreXrmUnitTestUserConfig, updatedGitIgnoreContent);
        }
    }
}
