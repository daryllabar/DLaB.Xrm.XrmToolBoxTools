using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DLaB.VSSolutionAccelerator.Tests
{
    [TestClass]
    public class LogicTests
    {
        private void ClearDirectory(string directory)
        {
            var di = new DirectoryInfo(directory);
            foreach (FileInfo file in di.EnumerateFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.EnumerateDirectories())
            {
                dir.Delete(true);
            }
        }

        private InitializeSolutionTestInfo InitializeTest()
        {
            var tempDir = TempDir.Create();
            ClearDirectory(tempDir.Name);
            var output = Path.GetFileName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            var solutionPath = Path.Combine(tempDir.Name, @"Abc.Xrm\Abc.Xrm.sln");
            var solutionDirectory = Path.GetDirectoryName(solutionPath) ?? "";
            var ebgPath = Path.Combine(solutionDirectory + @"DLaB.EBG.Settings.xml");
            var testSolutionTemplatePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Directory.CreateDirectory(solutionDirectory);
            // Copy Solution and EarlyBoundSettings Xml to Temp Folder
            File.Copy(Path.Combine(testSolutionTemplatePath ?? "", @"SolutionTemplate\Abc.Xrm.sln"), solutionPath, true);
            File.Copy(Path.Combine(testSolutionTemplatePath ?? "", @"SolutionTemplate\DLaB.EBG.Settings.xml"), ebgPath, true);

            var results = new object[]
            {
                solutionPath,
                "Abc.Xrm",
                new NuGetPackage
                {
                    Id = "Microsoft.CrmSdk.CoreAssemblies",
                    LicenseUrl = "http://download.microsoft.com/download/E/1/8/E18C0FAD-FEC8-44CD-9A16-98EDC4DAC7A2/LicenseTerms.docx",
                    Name = "Microsoft Dynamics 365 SDK core assemblies",
                    Version = new Version("9.0.2.5"),
                    VersionText = "9.0.2.5",
                    XrmToolingClient = false
                },
                new List<string> {"Y", ebgPath },
                "Abc.Xrm",
                "Abc.Xrm.Workflow",
                new List<string> {"Y", "Abc.Xrm.Test", "Abc.Xrm.TestCore" },
                new List<string> {"Y", "Abc.Xrm.Plugin"},
                new List<string> {"Y", "Abc.Xrm.Workflow"}
            };

            var pluginsPath = Path.Combine(Assembly.GetExecutingAssembly().Location, $@"..\..\..\..\DLaB.VSSolutionAccelerator\bin\{output}\Plugins");
            var context = new InitializeSolutionTestInfo
            {
                Info = InitializeSolutionInfo.InitializeSolution(results),
                TempDir = tempDir,
                TemplatePath = Path.GetFullPath(Path.Combine(pluginsPath, "DLaB.VSSolutionAccelerator")),
                SolutionDirectory = solutionDirectory
            };

            var logic = new Logic.Logic(context.Info.SolutionPath, context.TemplatePath);
            logic.Projects = logic.GetProjectInfos(context.Info);
            context.Logic = logic;
            return context;
        }

        [TestMethod]
        public void CreateSharedCommonProject_Should_Work()
        {
            using (var context = InitializeTest())
            {
                context.Logic.CreateSharedCommonProject(context.Info);
                
                //Xyz.Xrm.shproj
                var filePath = Path.Combine(context.SolutionDirectory, context.Info.SharedCommonProject, context.Info.SharedCommonProject + ".shproj");
                Assert.IsTrue(File.Exists(filePath), filePath + " was not created!");
                var lines = File.ReadAllLines(filePath);
                // Line 2
                lines.IsMissingLineContaining("<ProjectGuid>b22b3bc6-0ac6-4cdd-a118-16e318818ad7</ProjectGuid>", "The Project Guid should have been replaced!");
                // Line 11
                lines.HasLineContaining("<Import Project=\"Abc.Xrm.projitems\" Label=\"Shared\" />", "The Project items file should have been added!");

                //Xyz.Xrm.projitems
                filePath = Path.Combine(context.SolutionDirectory, context.Info.SharedCommonProject, context.Info.SharedCommonProject + ".projitems");
                Assert.IsTrue(File.Exists(filePath), filePath + " was not created!");
                lines = File.ReadAllLines(filePath);
                // Line 6
                lines.IsMissingLineContaining("<SharedGUID>b22b3bc6-0ac6-4cdd-a118-16e318818ad7</SharedGUID>", "The Shared Guid should have been replaced!");
                // Line 9
                lines.HasLineContaining(("<Import_RootNamespace>Abc.Xrm</Import_RootNamespace>"), "The Root Namespace should have been updated!");
                // Line 12
                lines.IsMissingLineContaining(")Entities\\", "The Entities should have been removed.");

                //Check for Namespace Update
                filePath = Path.Combine(context.SolutionDirectory, context.Info.SharedCommonProject, "Plugin", "PluginBase.cs");
                Assert.IsTrue(File.Exists(filePath), filePath + " was not created!");
                lines = File.ReadAllLines(filePath);
                lines.HasLineContaining("namespace Abc.Xrm.Plugin", "The Plugin Namespace should have been updated!");
            }
        }
    }
}
