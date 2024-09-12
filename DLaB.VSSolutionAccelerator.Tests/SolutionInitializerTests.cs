using DLaB.VSSolutionAccelerator.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DLaB.VSSolutionAccelerator.Tests
{
    [TestClass]
    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Local")]
    public class SolutionInitializerTests
    {
        private InitializeSolutionTestInfo InitializeTest(Action<InitializeSolutionInfo> setCustomSettings = null, string tempDirectoryName = null)
        {
            var tempDir = tempDirectoryName == null 
                ? TempDir.Create()
                : new TempDir(tempDirectoryName);
            TestBase.ClearDirectory(tempDir.Name);
            var solutionPath = Path.Combine(tempDir.Name, @"Abc.Xrm\Abc.Xrm.sln");
            var solutionDirectory = Path.GetDirectoryName(solutionPath) ?? "";
            var ebgPath = Path.Combine(solutionDirectory + @"DLaB.EBG.Settings.xml");
            var testSolutionTemplatePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Directory.CreateDirectory(solutionDirectory);
            // Copy Solution and EarlyBoundSettings Xml to Temp Folder
            File.Copy(Path.Combine(testSolutionTemplatePath ?? "", @"SolutionTemplate\Abc.Xrm.sln"), solutionPath, true);
            File.Copy(Path.Combine(testSolutionTemplatePath ?? "", @"SolutionTemplate\DLaB.EBG.Settings.xml"), ebgPath, true);

            var info = TestBase.InitializeSolutionInfo(solutionPath);
            info.PluginPackage.PackageId = Guid.NewGuid().ToString();
            setCustomSettings?.Invoke(info);

            var templatePath = TestBase.GetTemplatePath();
            var context = new InitializeSolutionTestInfo
            {
                Info = info,
                TempDir = tempDir,
                TemplatePath = templatePath,
                SolutionDirectory = solutionDirectory
            };

            var logic = new SolutionInitializer(context.Info.SolutionPath, context.TemplatePath);
            logic.Projects = logic.GetProjectInfos(context.Info);
            context.SolutionInitializer = logic;
            return context;
        }

        [TestMethod]
        public void CreateProject_WithCommon_Should_CreateCommonProject()
        {
            using (var context = InitializeTest())
            {
                TestProjectCreation(context,
                    ProjectInfo.Keys.Common,
                    context.Info.SharedCommonProject,
                    "Plugin\\PluginBase.cs");
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
                var lines = TestProjectCreation(context,
                    ProjectInfo.Keys.PluginTests,
                    context.Info.PluginTestName,
                    "AssumptionExampleTests.cs",
                    "Abc.Xrm.Plugin.Tests");

                Assert.That.ALineContains(lines, "<ProjectReference Include=\"..\\Abc.Xrm.Plugin\\Abc.Xrm.Plugin.csproj\" />", "Plugin Project reference should have been updated.");
                Assert.That.ALineContains(lines, "<ProjectReference Include=\"..\\Abc.Xrm.Test\\Abc.Xrm.Test.csproj\" />", "Shared Test Project reference should have been updated.");
                Assert.That.ALineContains(lines, "<ProjectReference Include=\"..\\Abc.Xrm\\Abc.Xrm.csproj\" />", "Shared Project reference should have been updated.");
            }
        }

        [TestMethod]
        public void CreateProject_WithPluginWithoutExampleFiles_Should_CreatePluginProjectSansPluginFiles()
        {
            using (var context = InitializeTest(i => { i.IncludeExamplePlugins = false; }))
            {
                var project = context.SolutionInitializer.Projects[ProjectInfo.Keys.Plugin];
                TestPluginProjectCreation(context,
                    project.Key,
                    context.Info.PluginName,
                    null,
                    "Abc.Xrm.Plugin");
                Assert.IsTrue(project.FilesToRemove.Count > 0, "There should have existed files to be removed.");
                var files = Directory.GetFiles(project.NewDirectory);
                Assert.AreEqual(1, files.Length, $"Only the .csproj file should exist in the directory!  Files found: {string.Join(", ", files.Select(Path.GetFileName))}");
                Assert.IsFalse(Directory.Exists(Path.Combine(project.NewDirectory, "PluginBaseExamples")), "The PluginBaseExamples file should have been removed.");
            }
        }

        [TestMethod]
        public void CreateProject_WithSharedTest_Should_CreateTestSharedProject()
        {
            using (var context = InitializeTest())
            {
                TestProjectCreation(context,
                    ProjectInfo.Keys.Test,
                    "Abc.Xrm.Test",
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
                TestWorkflowProjectCreation(context,
                    ProjectInfo.Keys.Workflow,
                    context.Info.WorkflowName,
                    "CreateGuidActivity.cs",
                    context.Info.SharedCommonWorkflowProject,
                    "Abc.Xrm.Workflow");
            }
        }

        [TestMethod]
        public void CreateProject_WithWorkflowTest_Should_CreateTestWorkflowProject()
        {
            using (var context = InitializeTest())
            {
                var lines = TestProjectCreation(context,
                    ProjectInfo.Keys.WorkflowTests,
                    context.Info.WorkflowTestName,
                    "WorkflowActivityExampleTests.cs",
                    "Abc.Xrm.Workflow.Tests");

                Assert.That.ALineContains(lines, @"Include=""..\Abc.Xrm.Test\Abc.Xrm.Test.csproj"" />", "The shared test Project should have been updated!");
                Assert.That.ALineContains(lines, @"Include=""..\Abc.Xrm.Workflow\Abc.Xrm.Workflow.csproj"" />", "The shared workflow Project should have been updated!");
            }
        }

        [TestMethod]
        public void CreateProject_WithWorkflowWithoutExampleFiles_Should_CreateWorkflowProjectSansExamples()
        {
            using (var context = InitializeTest(i => { i.IncludeExampleWorkflow = false; }))
            {
                var project = context.SolutionInitializer.Projects[ProjectInfo.Keys.Workflow];
                TestWorkflowProjectCreation(context,
                    ProjectInfo.Keys.Workflow,
                    context.Info.WorkflowName,
                    null,
                    context.Info.SharedCommonWorkflowProject,
                    "Abc.Xrm.Workflow");

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
            context.SolutionInitializer.CreateProject(key, context.Info);
            var id = context.SolutionInitializer.Projects[key].Id;
            // .shproj
            var filePath = Path.Combine(context.SolutionDirectory, newName, newName + ".shproj");
            Assert.IsTrue(File.Exists(filePath), filePath + " was not created!");
            var lines = File.ReadAllLines(filePath);
            // Line 4
            Assert.That.ALineContains(lines, $"<ProjectGuid>{id}</ProjectGuid>", "The Project Guid should have been replaced!");
            // Line 11
            Assert.That.ALineContains(lines, $"<Import Project=\"{newName}.projitems\" Label=\"Shared\" />", "The Project items file should have been added!");

            AssertCsFileNamespaceUpdated(context, newName, arbitraryFile, newNameSpace);

            // .projitems
            filePath = Path.Combine(context.SolutionDirectory, newName, newName + ".projitems");
            Assert.IsTrue(File.Exists(filePath), filePath + " was not created!");
            lines = File.ReadAllLines(filePath);
            // Line 6
            Assert.That.ALineContains(lines, $"<SharedGUID>{id}</SharedGUID>", "The Shared Guid should have been replaced!");
            // Line 9
            Assert.That.ALineContains(lines, ($"<Import_RootNamespace>{newNameSpace}</Import_RootNamespace>"), $"The Root Namespace should have been updated to {newNameSpace}!");

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
            Assert.That.ALineContains(lines, $"namespace {newNameSpace}", $"The namespace should have been updated to {newNameSpace}!");
            Assert.That.NoLineContains(lines, "using Xyz.Xrm", "Using Namespaces should have been updated!");
        }

        private static string[] TestPluginProjectCreation(InitializeSolutionTestInfo context, string key, string newName, string arbitraryFile, string newNameSpace = null)
        {
            var lines = TestProjectCreation(context, key, newName, arbitraryFile, newNameSpace);
            var plugin = context.Info.PluginPackage;

            // PropertyGroup
            Assert.That.ALineContains(lines, $"<PackageId>{newName}</PackageId>", $"The <PackageId/> should have been updated to {newName}");
            Assert.That.ALineContains(lines, $"<Authors>{plugin.Company}</Authors>", $"The <Authors/> should have been updated to {plugin.Company}");
            Assert.That.ALineContains(lines, $"<Company>{plugin.Company}</Company>", $"The <Company/> should have been updated to {plugin.Company}");
            Assert.That.ALineContains(lines, $"<Description>{plugin.Description}</Description>", $"The <Description/> should have been updated to {plugin.Description}");
            Assert.That.ALineContains(lines, $"<DeploymentPacAuthName>{plugin.PacAuthName}</DeploymentPacAuthName>", $"The <DeploymentPacAuthName/> should have been updated to {plugin.PacAuthName}");
            Assert.That.ALineContains(lines, $"<PluginPackageId>{plugin.PackageId}</PluginPackageId>", $"The <PluginPackageId/> should have been updated to {plugin.PackageId}");

            return lines;
        }

        private static string[] TestWorkflowProjectCreation(InitializeSolutionTestInfo context, string key, string newName, string arbitraryFile, string sharedWorkflowProject, string newNameSpace)
        {
            var lines = TestProjectCreation(context, key, newName, arbitraryFile, newNameSpace);

            Assert.That.ALineContains(lines, $"<AssemblyOriginatorKeyFile>{newName}.Key.snk</AssemblyOriginatorKeyFile>", $"The <AssemblyOriginatorKeyFile/> should have been updated to {newName}.Key.snk");
            Assert.That.ALineContains(lines, $"<Import Project=\"..\\{sharedWorkflowProject}\\{sharedWorkflowProject}.projitems\" Label=\"Shared\" />", $"The shared Workflow project should have been updated to \"..\\{sharedWorkflowProject}\\{sharedWorkflowProject}.projitems\"");

            return lines;
        }

        private static string[] TestProjectCreation(InitializeSolutionTestInfo context, string key, string newName, string arbitraryFile, string newNameSpace = null)
        {
            newNameSpace = newNameSpace ?? newName;
            context.SolutionInitializer.CreateProject(key, context.Info);
            var filePath = Path.Combine(context.SolutionDirectory, newName, newName + ".csproj");
            Assert.IsTrue(File.Exists(filePath), filePath + " was not created!");

            AssertCsFileNamespaceUpdated(context, newName, arbitraryFile, newNameSpace);

            return File.ReadAllLines(filePath);
        }
    }
}
