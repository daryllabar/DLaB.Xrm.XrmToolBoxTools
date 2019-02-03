using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DLaB.VSSolutionAccelerator.Logic;
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
                try
                {
                    dir.Delete(true);
                }
                catch
                {
                    System.Threading.Thread.Sleep(2000);
                    dir.Delete(true);
                }
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
        public void CreateProject_WithCommon_Should_CreateCommonProject()
        {
            using (var context = InitializeTest())
            {

                var lines = TestSharedProjectCreation(context,
                    ProjectInfo.Keys.Common,
                    context.Info.SharedCommonProject,
                    "Plugin\\PluginBase.cs");

                // Line 12
                lines.IsMissingLineContaining(")Entities\\", "The Entities should have been removed.");
            }
        }

        [TestMethod]
        public void CreateProject_WithWorkflowCommon_Should_CreateWorkflowCommonProject()
        {
            using (var context = InitializeTest())
            {
                TestSharedProjectCreation(context,
                    ProjectInfo.Keys.WorkflowCommon,
                    context.Info.SharedCommonWorkflowProject,
                    "CodeActivityBase.cs");
            }
        }

        [TestMethod]
        public void CreateProject_WithSharedTest_Should_CreateTestSharedProject()
        {
            using (var context = InitializeTest())
            {
                TestSharedProjectCreation(context,
                    ProjectInfo.Keys.TestCore,
                    context.Info.SharedTestCoreProject,
                    "TestMethodClassBase.cs", "Abc.Xrm.Test");
            }
        }

        private static string[] TestSharedProjectCreation(InitializeSolutionTestInfo context, string key, string newName, string arbitraryFile, string newNameSpace = null)
        {
            newNameSpace = newNameSpace ?? newName;
            context.Logic.CreateProject(key, context.Info);
            var id = context.Logic.Projects[key].Id;
            // .shproj
            var filePath = Path.Combine(context.SolutionDirectory, newName, newName + ".shproj");
            Assert.IsTrue(File.Exists(filePath), filePath + " was not created!");
            var lines = File.ReadAllLines(filePath);
            // Line 4
            lines.HasLineContaining($"<ProjectGuid>{id}</ProjectGuid>", "The Project Guid should have been replaced!");
            // Line 11
            lines.HasLineContaining($"<Import Project=\"{newName}.projitems\" Label=\"Shared\" />", "The Project items file should have been added!");

            //Check for Namespace Update
            filePath = Path.Combine(context.SolutionDirectory, newName, arbitraryFile);
            Assert.IsTrue(File.Exists(filePath), filePath + " was not created!");
            lines = File.ReadAllLines(filePath);
            lines.HasLineContaining($"namespace {newNameSpace}", "The namespace should have been updated!");

            // .projitems
            filePath = Path.Combine(context.SolutionDirectory, newName, newName + ".projitems");
            Assert.IsTrue(File.Exists(filePath), filePath + " was not created!");
            lines = File.ReadAllLines(filePath);
            // Line 6
            lines.HasLineContaining($"<SharedGUID>{id}</SharedGUID>", "The Shared Guid should have been replaced!");
            // Line 9
            lines.HasLineContaining(($"<Import_RootNamespace>{newNameSpace}</Import_RootNamespace>"), "The Root Namespace should have been updated!");

            return lines;
        }
    }
}
