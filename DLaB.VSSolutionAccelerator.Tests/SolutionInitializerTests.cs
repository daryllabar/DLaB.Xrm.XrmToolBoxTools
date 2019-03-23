using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using DLaB.VSSolutionAccelerator.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            var results = new object[]
            {
                new List<string>{"Y", solutionPath },
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
                "Y",
                "Abc.Xrm",
                "Abc.Xrm.WorkflowCore",
                new List<string> {"Y", "Abc.Xrm.Test", "Abc.Xrm.TestCore" },
                new List<string> {"Y", "Abc.Xrm.Plugin", "0"},
                "Abc.Xrm.Plugin.Tests",
                new List<string> {"Y", "Abc.Xrm.Workflow", "0"},
                "Abc.Xrm.Workflow.Tests",
                new List<string> {"0", "0"},
            };
            var info = InitializeSolutionInfo.InitializeSolution(results);
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

                var lines = TestSharedProjectCreation(context,
                    ProjectInfo.Keys.Common,
                    context.Info.SharedCommonProject,
                    "Plugin\\PluginBase.cs").ToList();

                // Line 12
                lines.RemoveAll(l =>
                    l.Contains("xyz_VoidPaymentRequest")
                    ||
                    l.Contains("xyz_VoidPaymentResponse"));

                Assert.That.NotExistsLineContaining(lines.ToArray(), ")Entities\\", "The Entities should have been removed.");
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
        public void CreateProject_WithPluginWithOldNuget_Should_UpdateVersion()
        {
            using (var context = InitializeTest(c =>
            {
                c.XrmPackage.Version = new Version(8, 2, 0, 2);
                c.XrmPackage.VersionText = c.XrmPackage.Version.ToString();
            }, "NuGet"))
            {
                // Plugin requires Shared Project to be created first
                context.SolutionInitializer.CreateProject(ProjectInfo.Keys.Common, context.Info);
                TestPluginProjectCreation(context,
                    ProjectInfo.Keys.Plugin,
                    context.Info.PluginName,
                    "RenameLogic.cs",
                    "Abc.Xrm.Plugin");
                var packagesPath = Path.Combine(context.SolutionDirectory, context.Info.PluginName, "packages.config");
                var packages = File.ReadAllLines(packagesPath);
                Assert.That.ExistsLineContaining(packages, "\"Microsoft.CrmSdk.CoreAssemblies\" version=\"8", "Microsoft.CrmSdk.CoreAssemblies should have been updated to v8.");

            }
        }

        [TestMethod]
        public void CreateProject_WithPluginWithPreAttributeKeyNuget_Should_AddCompilationSymbol()
        {
            using (var context = InitializeTest(c =>
            {
                c.XrmPackage.Version = new Version(6, 1, 2, 0);
                c.XrmPackage.VersionText = c.XrmPackage.Version.ToString();
            }, "NuGet"))
            {
                // Plugin requires Shared Project to be created first
                context.SolutionInitializer.CreateProject(ProjectInfo.Keys.Common, context.Info);
                TestPluginProjectCreation(context,
                    ProjectInfo.Keys.Plugin,
                    context.Info.PluginName,
                    "RenameLogic.cs",
                    "Abc.Xrm.Plugin");
                var pluginProjectPath = Path.Combine(context.SolutionDirectory, context.Info.PluginName, context.Info.PluginName + ".csproj");
                var pluginProject = File.ReadAllLines(pluginProjectPath);
                Assert.That.ExistsLineContaining(pluginProject, "<DefineConstants>DEBUG;TRACE;PRE_KEYATTRIBUTE</DefineConstants>", "Pre Key Attribute should have been added to the project debug statement");
                Assert.That.ExistsLineContaining(pluginProject, "<DefineConstants>TRACE;PRE_KEYATTRIBUTE</DefineConstants>", "Pre Key Attribute should have been added to the project release statement");

            }
        }

        [TestMethod]
        public void CreateProject_WithPluginTest_Should_CreateTestPluginProject()
        {
            using (var context = InitializeTest())
            {
                var lines = TestTestProjectCreation(context,
                    ProjectInfo.Keys.PluginTests,
                    context.Info.PluginTestName,
                    "AssumptionExampleTests.cs",
                    "Abc.Xrm.Plugin.Tests");

                Assert.That.NotExistsLineContaining(lines, ProjectInfo.Keys.Plugin + ".csproj", "Project reference should have been updated.");
            }
        }

        [TestMethod]
        public void CreateProject_WithPluginWithoutExampleFiles_Should_CreatePluginProjectSansPluginFiles()
        {
            using (var context = InitializeTest(i => { i.IncludeExamplePlugins = false; }))
            {
                var project = context.SolutionInitializer.Projects[ProjectInfo.Keys.Plugin];
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
        public void CreateProject_WithWorkflowTest_Should_CreateTestWorkflowProject()
        {
            using (var context = InitializeTest())
            {
                TestTestProjectCreation(context,
                    ProjectInfo.Keys.WorkflowTests,
                    context.Info.WorkflowTestName,
                    "WorkflowActivityExampleTests.cs",
                    "Abc.Xrm.Workflow.Tests");
            }
        }

        [TestMethod]
        public void CreateProject_WithWorkflowWithoutExampleFiles_Should_CreateWorkflowProjectSansExamples()
        {
            using (var context = InitializeTest(i => { i.IncludeExampleWorkflow = false; }))
            {
                var project = context.SolutionInitializer.Projects[ProjectInfo.Keys.Workflow];
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
            context.SolutionInitializer.CreateProject(key, context.Info);
            var id = context.SolutionInitializer.Projects[key].Id;
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
            Assert.That.NotExistsLineContaining(lines, "using Xyz.Xrm", "Using Namespaces should have been updated!");
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
            context.SolutionInitializer.CreateProject(key, context.Info);
            var framework = context.Info.XrmPackage.Version.Major >= 9 ? "v4.6.2" : "v4.5.2";
            var id = context.SolutionInitializer.Projects[key].Id;
            var filePath = Path.Combine(context.SolutionDirectory, newName, newName + ".csproj");
            Assert.IsTrue(File.Exists(filePath), filePath + " was not created!");
            var lines = File.ReadAllLines(filePath);
            // PropertyGroup
            Assert.That.ExistsLineContaining(lines, $"<ProjectGuid>{{{id.ToString().ToUpper()}}}</ProjectGuid>", "The Project Guid should have been replaced!");
            Assert.That.ExistsLineContaining(lines, $"<RootNamespace>{newNameSpace}</RootNamespace>", $"The Root Namespace should have been updated to {newNameSpace}!");
            Assert.That.ExistsLineContaining(lines, $"<AssemblyName>{newName}</AssemblyName>", $"The Assembly Name should have been updated to {newName}!");
            Assert.That.ExistsLineContaining(lines, $"<TargetFrameworkVersion>{framework}</TargetFrameworkVersion>", $"The Target Framework should have been updated to {framework}!");
            Assert.That.NotExistsLineContaining(lines, "CodeAnalysisRuleSet", "The CodeAnalysisRuleSet should have been removed");
            Assert.That.NotExistsLineContaining(lines, "LocalNuget", "LocalNuget should have been removed");

            AssertCsFileNamespaceUpdated(context, newName, arbitraryFile, newNameSpace);

            AssertAssemblyInfoUpdated(context, newName, id);

            return lines;
        }

        private static void AssertAssemblyInfoUpdated(InitializeSolutionTestInfo context, string newName, Guid projectId)
        {
            var filePath = Path.Combine(context.SolutionDirectory, newName, "Properties", "AssemblyInfo.cs");
            var file = File.ReadAllLines(filePath);
            Assert.That.ExistsLineContaining(file, $"[assembly: AssemblyTitle(\"{newName}\")]");
            Assert.That.ExistsLineContaining(file, $"[assembly: AssemblyProduct(\"{newName}\")]");
            Assert.That.ExistsLineContaining(file, $"[assembly: AssemblyCopyright(\"Copyright ©  {DateTime.Now.Year}\")]");
            Assert.That.ExistsLineContaining(file, $"[assembly: Guid(\"{projectId}\")]");
        }
    }
}
