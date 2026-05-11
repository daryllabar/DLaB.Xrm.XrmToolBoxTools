using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace DLaB.ModelBuilderExtensions.Tests
{
    [TestClass]
    public class CodeGenerationServiceTests
    {
        [TestMethod]
        public void CorrectGeneratedText_ShouldReplaceFieldsCommentTypo_WhenPresent()
        {
            using (var tmp = TempDir.Create())
            {
                var typoFile = Path.Combine(tmp.Name, "Typo.cs");
                var untouchedFile = Path.Combine(tmp.Name, "Untouched.cs");
                var missingFile = Path.Combine(tmp.Name, "Missing.cs");
                File.WriteAllText(typoFile, "/// Available fields, a the time of codegen, for the account entity");
                File.WriteAllText(untouchedFile, "/// Available fields, at the time of codegen, for the contact entity");
                Assert.IsFalse(File.Exists(missingFile));

                var method = typeof(CodeGenerationService).GetMethod("CorrectGeneratedText", BindingFlags.NonPublic | BindingFlags.Static);
                Assert.IsNotNull(method);

                method.Invoke(null, new object[] { new[] { typoFile, untouchedFile, missingFile } });

                var typoContents = File.ReadAllText(typoFile);
                Assert.IsTrue(typoContents.Contains("Available fields, at the time of codegen, for the account entity"));
                Assert.IsFalse(typoContents.Contains("Available fields, a the time of codegen, for the account entity"));
                Assert.AreEqual("/// Available fields, at the time of codegen, for the contact entity", File.ReadAllText(untouchedFile));
                Assert.IsFalse(File.Exists(missingFile));
            }
        }
    }
}
