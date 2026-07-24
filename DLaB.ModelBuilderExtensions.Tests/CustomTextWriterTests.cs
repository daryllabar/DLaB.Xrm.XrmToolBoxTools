using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DLaB.ModelBuilderExtensions.Tests
{
    [TestClass]
    public class CustomTextWriterTests
    {
        [TestMethod]
        public void UpdateLineForNullablePropertyTypes_Should_AppendNullForgivingOperator_ToSetRelatedEntityValueParameter()
        {
            using (var tempDir = TempDir.Create())
            {
                var outputFile = Path.Combine(tempDir.Name, "Account.cs");
                var sourceLines = File.ReadAllLines(GetAccountFilePath());

                using (var writer = new CustomTextWriter(outputFile, false, false, true, new System.Collections.Generic.List<string>()))
                {
                    foreach (var line in sourceLines)
                    {
                        writer.WriteLine(line);
                    }
                }

                var updatedLines = File.ReadAllLines(outputFile);
                var setRelatedEntityLines = updatedLines.Where(l => l.Contains("SetRelatedEntity<") && !l.EndsWith(" +")).ToList();

                Assert.IsTrue(setRelatedEntityLines.Count > 0, "Expected to find SetRelatedEntity calls in the test file.");
                foreach (var line in setRelatedEntityLines)
                {
                    Assert.IsTrue(line.TrimEnd().EndsWith(", value!);"), $"Expected line to end with ', value!);' but was: {line}");
                }

                Assert.AreEqual(1, updatedLines.Count(l => l.Trim() == @""""", null, value!);"));
                Assert.AreEqual(0, updatedLines.Count(l => l.Trim() == @""""", null, value);"));
            }
        }

        [TestMethod]
        public void UpdateLineForNullablePropertyTypes_WhenMakeReferenceTypesNullableIsFalse_Should_NotModifySetRelatedEntityValueParameter()
        {
            using (var tempDir = TempDir.Create())
            {
                var outputFile = Path.Combine(tempDir.Name, "Account.cs");
                var sourceLines = File.ReadAllLines(GetAccountFilePath());

                using (var writer = new CustomTextWriter(outputFile, false, false, false, new System.Collections.Generic.List<string>()))
                {
                    foreach (var line in sourceLines)
                    {
                        writer.WriteLine(line);
                    }
                }

                var updatedLines = File.ReadAllLines(outputFile);
                var setRelatedEntityLines = updatedLines.Where(l => l.Contains("SetRelatedEntity<") && !l.EndsWith(" +")).ToList();

                Assert.IsTrue(setRelatedEntityLines.Count > 0, "Expected to find SetRelatedEntity calls in the test file.");
                foreach (var line in setRelatedEntityLines)
                {
                    Assert.IsFalse(line.TrimEnd().EndsWith(", value!);"), $"Expected line to NOT end with ', value!);' but was: {line}");
                    Assert.IsTrue(line.TrimEnd().EndsWith(", value);"), $"Expected line to end with ', value);' but was: {line}");
                }

                Assert.AreEqual(0, updatedLines.Count(l => l.Trim() == @""""", null, value!);"));
                Assert.AreEqual(1, updatedLines.Count(l => l.Trim() == @""""", null, value);"));
            }
        }

        private static string GetAccountFilePath()
        {
            // Walk up from the test assembly's output directory to find the solution directory,
            // then locate the DLaB.Xrm.Entities\Entities\Account.cs file used as sample source.
            var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (dir != null && !File.Exists(Path.Combine(dir.FullName, "DLaB.Xrm.XrmToolBoxTools.sln")))
            {
                dir = dir.Parent;
            }

            if (dir == null)
            {
                throw new InvalidOperationException("Unable to locate solution directory to find Account.cs test file.");
            }

            var accountFilePath = Path.Combine(dir.FullName, "DLaB.Xrm.Entities", "Entities", "Account.cs");
            if (!File.Exists(accountFilePath))
            {
                throw new FileNotFoundException("Unable to find Account.cs test file.", accountFilePath);
            }

            return accountFilePath;
        }
    }
}
