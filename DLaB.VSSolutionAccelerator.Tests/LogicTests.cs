using System;
using System.Collections.Generic;
using System.IO;
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
            foreach (var file in di.EnumerateFiles())
            {
                file.Delete();
            }
            foreach (var dir in di.EnumerateDirectories())
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

        private InitializeSolutionTestInfo InitializeTest(Action<InitializeSolutionInfo> setCustomSettings = null)
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
                "Abc.Xrm.WorkflowCore",
                new List<string> {"Y", "Abc.Xrm.Test", "Abc.Xrm.TestCore" },
                new List<string> {"Y", "Abc.Xrm.Plugin", "0"},
                "Abc.Xrm.Plugin.Tests",
                new List<string> {"Y", "Abc.Xrm.Workflow", "0"},
                "Abc.Xrm.Workflow.Tests"
            };
            var info = InitializeSolutionInfo.InitializeSolution(results);
            setCustomSettings?.Invoke(info);

            var pluginsPath = Path.Combine(Assembly.GetExecutingAssembly().Location, $@"..\..\..\..\DLaB.VSSolutionAccelerator\bin\{output}\Plugins");
            var context = new InitializeSolutionTestInfo
            {
                Info = info,
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
                Assert.That.NotExistsLineContaining(lines, ")Entities\\", "The Entities should have been removed.");
            }
        }

        [TestMethod]
        public void CreateProject_WithPlugin_Should_CreatePluginProject()
        {
            using (var context = InitializeTest())
            {
                TestPluginProjectCreation(context,
                    ProjectInfo.Keys.Plugin,
                    context.Info.PluginName,
                    "RenameLogic.cs", 
                    "Abc.Xrm.Plugin");
            }
        }

        [TestMethod]
        public void CreateProject_WithPluginTest_Should_CreateTestPluginProject()
        {
            using (var context = InitializeTest())
            {
                TestTestProjectCreation(context,
                    ProjectInfo.Keys.PluginTests,
                    context.Info.PluginTestName,
                    "AssumptionExampleTests.cs",
                    "Abc.Xrm.Plugin.Tests");
            }
        }

        [TestMethod]
        public void CreateProject_WithPluginWithoutExampleFiles_Should_CreatePluginProjectSansPluginFiles()
        {
            using (var context = InitializeTest(i => { i.IncludeExamplePlugins = false; }))
            {
                var project = context.Logic.Projects[ProjectInfo.Keys.Plugin];
                var lines = TestPluginProjectCreation(context,
                    project.Key,
                    context.Info.PluginName,
                    null,
                    "Abc.Xrm.Plugin");
                Assert.That.NotExistsLineContaining(lines, "RenameLogic.cs");
                Assert.IsTrue(project.FilesToRemove.Count > 0, "There should have existed files to be removed.");
                foreach (var file in project.FilesToRemove)
                {
                    var path = Path.Combine(project.NewDirectory, file);
                    Assert.IsFalse(File.Exists(path), $"File '{path}' should have been deleted.");
                }
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
                    "TestMethodClassBase.cs",
                    "Abc.Xrm.Test");
                
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
                    "CodeActivityBase.cs",
                    "Abc.Xrm.Workflow");
            }
        }
        [TestMethod]
        public void CreateProject_WithWorkflow_Should_CreateWorkflowProject()
        {
            using (var context = InitializeTest())
            {
                TestPluginProjectCreation(context,
                    ProjectInfo.Keys.Workflow,
                    context.Info.WorkflowName,
                    "CreateGuidActivity.cs",
                    "Abc.Xrm.Workflow");
            }
        }

        [TestMethod]
        public void CreateProject_WithWorkflowWithoutExampleFiles_Should_CreateWorkflowProjectSansExamples()
        {
            using (var context = InitializeTest(i => { i.IncludeExampleWorkflow = false; }))
            {
                var project = context.Logic.Projects[ProjectInfo.Keys.Workflow];
                var lines = TestPluginProjectCreation(context,
                    ProjectInfo.Keys.Workflow,
                    context.Info.WorkflowName,
                    null,
                    "Abc.Xrm.Workflow");
                Assert.That.NotExistsLineContaining(lines, "CreateGuidActivity.cs", "Existed files should have been removed from the project.");
                Assert.IsTrue(project.FilesToRemove.Count > 0, "Existed files should have been removed.");
                foreach (var file in project.FilesToRemove)
                {
                    var path = Path.Combine(project.NewDirectory, file);
                    Assert.IsFalse(File.Exists(path), $"File '{path}' should have been deleted.");
                }
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
            Assert.That.ExistsLineContaining(lines, $"<ProjectGuid>{id}</ProjectGuid>", "The Project Guid should have been replaced!");
            // Line 11
            Assert.That.ExistsLineContaining(lines, $"<Import Project=\"{newName}.projitems\" Label=\"Shared\" />", "The Project items file should have been added!");

            AssertCsFileNamespaceUpdated(context, newName, arbitraryFile, newNameSpace);

            // .projitems
            filePath = Path.Combine(context.SolutionDirectory, newName, newName + ".projitems");
            Assert.IsTrue(File.Exists(filePath), filePath + " was not created!");
            lines = File.ReadAllLines(filePath);
            // Line 6
            Assert.That.ExistsLineContaining(lines, $"<SharedGUID>{id}</SharedGUID>", "The Shared Guid should have been replaced!");
            // Line 9
            Assert.That.ExistsLineContaining(lines, ($"<Import_RootNamespace>{newNameSpace}</Import_RootNamespace>"), $"The Root Namespace should have been updated to {newNameSpace}!");

            return lines;
        }

        private static void AssertCsFileNamespaceUpdated(InitializeSolutionTestInfo context, string newName, string arbitraryFile, string newNameSpace)
        {
            if (arbitraryFile == null)
            {
                return;
            }
            //Check for Namespace Update
            var filePath = Path.Combine(context.SolutionDirectory, newName, arbitraryFile);
            Assert.IsTrue(File.Exists(filePath), filePath + " was not created!");
            var lines = File.ReadAllLines(filePath);
            Assert.That.ExistsLineContaining(lines, $"namespace {newNameSpace}", $"The namespace should have been updated to {newNameSpace}!");
        }

        private static string[] TestPluginProjectCreation(InitializeSolutionTestInfo context, string key, string newName, string arbitraryFile, string newNameSpace = null)
        {
            var lines = TestProjectCreation(context, key, newName, arbitraryFile, newNameSpace);

            // PropertyGroup
            Assert.That.ExistsLineContaining(lines, $"<AssemblyOriginatorKeyFile>{newName}.Key.snk</AssemblyOriginatorKeyFile>", $"The Assembly Key File should have been updated to {newName}.Key.snk!");

            // ItemGroup
            Assert.That.ExistsLineContaining(lines, $"<None Include=\"{newName}.Key.snk\" />", $"The Assembly Key File Item Group Value should have been updated to {newName}.Key.snk!");

            // Imports
            Assert.That.ExistsLineContaining(lines, @"Project=""..\Abc.Xrm\Abc.Xrm.projitems"" Label=""Shared"" />", $"The shared Common Project should have been updated!");

            return lines;
        }

        private static string[] TestTestProjectCreation(InitializeSolutionTestInfo context, string key, string newName, string arbitraryFile, string newNameSpace = null)
        {
            var lines = TestProjectCreation(context, key, newName, arbitraryFile, newNameSpace);

            // Imports
            Assert.That.ExistsLineContaining(lines, @"Project=""..\Abc.Xrm.TestCore\Abc.Xrm.TestCore.projitems"" Label=""Shared"" />", $"The shared test Project should have been updated!");

            return lines;
        }

        private static string[] TestProjectCreation(InitializeSolutionTestInfo context, string key, string newName, string arbitraryFile, string newNameSpace = null)
        {
            newNameSpace = newNameSpace ?? newName;
            context.Logic.CreateProject(key, context.Info);
            var id = context.Logic.Projects[key].Id;
            var filePath = Path.Combine(context.SolutionDirectory, newName, newName + ".csproj");
            Assert.IsTrue(File.Exists(filePath), filePath + " was not created!");
            var lines = File.ReadAllLines(filePath);
            // PropertyGroup
            Assert.That.ExistsLineContaining(lines, $"<ProjectGuid>{{{id.ToString().ToUpper()}}}</ProjectGuid>", "The Project Guid should have been replaced!");
            Assert.That.ExistsLineContaining(lines, $"<RootNamespace>{newNameSpace}</RootNamespace>", $"The Root Namespace should have been updated to {newNameSpace}!");
            Assert.That.ExistsLineContaining(lines, $"<AssemblyName>{newName}</AssemblyName>", $"The Assembly Name should have been updated to {newName}!");
            Assert.That.ExistsLineContaining(lines, "<TargetFrameworkVersion>v4.6.2</TargetFrameworkVersion>", $"The Target Framework should have been updated to v4.6.2!");
            Assert.That.NotExistsLineContaining(lines, "CodeAnalysisRuleSet", "The CodeAnalysisRuleSet should have been removed");
            Assert.That.NotExistsLineContaining(lines, "LocalNuget", "LocalNuget should have been removed");

            AssertCsFileNamespaceUpdated(context, newName, arbitraryFile, newNameSpace);

            return lines;
        }
    }
}
