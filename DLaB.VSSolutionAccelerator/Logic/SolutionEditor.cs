using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DLaB.Log;
using Source.DLaB.Common;

namespace DLaB.VSSolutionAccelerator.Logic
{
    public class SolutionEditor
    {
        public string NuGetPath { get; }
        public string NugetContentInstallerPath { get; }
        public string OutputBaseDirectory { get; }
        public Dictionary<string, ProjectInfo> Projects { get; set; }
        public string SolutionPath { get; }
        public string StrongNamePath { get; }
        public string TemplateDirectory { get; }

        public SolutionEditor(string solutionPath, 
                              string templateDirectory, 
                              string strongNamePath = null,  
                              string nugetPath = null, 
                              string nugetContentInstallerPath = null)
        {
            NuGetPath = nugetPath ?? Path.Combine(templateDirectory, "bin\\nuget.exe");
            NugetContentInstallerPath = nugetContentInstallerPath ?? Path.Combine(templateDirectory, "bin\\nugetContentInstaller.exe");
            StrongNamePath = strongNamePath ?? Path.Combine(templateDirectory, "bin\\sn.exe");
            TemplateDirectory = templateDirectory;
            SolutionPath = solutionPath;
            OutputBaseDirectory = Path.GetDirectoryName(solutionPath);
        }

        protected void ExecuteNuGetRestoreForSolution()
        {
            var cmd = new ProcessExecutorInfo(NuGetPath, $"restore \"{SolutionPath}\" -NonInteractive");
            Logger.Show("Restoring Nuget for the solution.");
            Logger.AddDetail(cmd.FileName + " " + cmd.Arguments);
            var results = ProcessExecutor.ExecuteCmd(cmd);
            Logger.Show(results);
            UpdateProjectsPostSolutionRestore();
        }

        protected void AddNugetPostUpdateCommandsToProjects(Version xrmPackageVersion, Dictionary<string, ProjectInfo> projects)
        {
            var mapper = new NuGetMapper(NuGetPath, xrmPackageVersion);
            foreach (var project in projects.Values.Where(p => p.Type != ProjectInfo.ProjectType.SharedProj))
            {
                project.AddNugetPostUpdateCommands(mapper,
                    Path.Combine(TemplateDirectory, project.Key, "packages.config"),
                    Path.Combine(OutputBaseDirectory, project.Name, "packages.config"),
                    NugetContentInstallerPath,
                    OutputBaseDirectory);
            }
        }

        protected ProjectInfo CreateDefaultProjectInfo(string key, string name, string dotNetFramework, string sharedCommonProject)
        {
            Logger.AddDetail($"Configuring Project {name} based on {key}.");
            var id = Guid.NewGuid();
            var trademark = "[assembly: AssemblyTrademark(\"\")]";
            var project = new ProjectInfo
            {
                Key = key,
                Id = id,
                Type = ProjectInfo.ProjectType.CsProj,
                NewDirectory = Path.Combine(OutputBaseDirectory, name),
                Name = name,
                Files = new List<ProjectFile>
                {
                    new ProjectFile
                    {
                        Name = name + ".csproj",
                        Replacements = new Dictionary<string, string>
                        {
                            {ProjectInfo.IdByKey[key], id.ToString().ToUpper()},
                            {$"<RootNamespace>{key}</RootNamespace>", $"<RootNamespace>{name}</RootNamespace>"},
                            {$"<AssemblyName>{key}</AssemblyName>", $"<AssemblyName>{name}</AssemblyName>"},
                            {"<TargetFrameworkVersion>v4.6.2</TargetFrameworkVersion>", $"<TargetFrameworkVersion>{dotNetFramework}</TargetFrameworkVersion>"},
                            {"<TargetFrameworkVersion>v4.5.2</TargetFrameworkVersion>", $"<TargetFrameworkVersion>{dotNetFramework}</TargetFrameworkVersion>"},
                            {$"<AssemblyOriginatorKeyFile>{key}.Key.snk</AssemblyOriginatorKeyFile>", $"<AssemblyOriginatorKeyFile>{name}.Key.snk</AssemblyOriginatorKeyFile>"},
                            {$"<None Include=\"{key}.Key.snk\" />", $"<None Include=\"{name}.Key.snk\" />"},
                            {@"<Import Project=""..\Xyz.Xrm\Xyz.Xrm.projitems"" Label=""Shared"" />", $@"<Import Project=""..\{sharedCommonProject}\{sharedCommonProject}.projitems"" Label=""Shared"" />"},
                            {@"<Import Project=""..\Xyz.Xrm.TestCore\Xyz.Xrm.TestCore.projitems"" Label=""Shared"" />", $@"<Import Project=""..\{sharedCommonProject}\{sharedCommonProject}.projitems"" Label=""Shared"" />"},
                        },
                        Removals = new List<string>
                        {
                            "<CodeAnalysisRuleSet>"
                        }
                    },
                    new ProjectFile
                    {
                        Name = "Properties\\AssemblyInfo.cs",
                        Replacements = new Dictionary<string, string>
                        {
                            {ProjectInfo.IdByKey[key].ToLower(), id.ToString()},
                            {$"(\"{key}\")]", $"(\"{name}\")]"},
                            {trademark, $"[assembly: AssemblyCopyright(\"Copyright ©  {DateTime.Now.Year}\")]{Environment.NewLine}{trademark}"}
                        },
                        Removals = new List<string>
                        {
                            "Copyright ©"
                        },
                    }
                },
            };
            return project;
        }


        protected void AddPlugin(Dictionary<string, ProjectInfo> projects, SolutionEditorInfo info)
        {
            var project = CreateDefaultProjectInfo(
                ProjectInfo.Keys.Plugin,
                info.PluginName,
                info.GetPluginAssemblyVersionForSdk(),
                info.SharedCommonProject);
            project.AddRegenKeyPostUpdateCommand(StrongNamePath);
            project.SharedProjectsReferences.Add(projects[ProjectInfo.Keys.Common]);
            RemovePluginExampleFiles(info.IncludeExamplePlugins, project);
            AddPluginCompileTimeConstants(project, info);
            projects.Add(project.Key, project);
        }

        private void AddPluginCompileTimeConstants(ProjectInfo project, SolutionEditorInfo info)
        {
            if (info.XrmVersion >= new Version(7, 0, 0, 0))
            {
                return;
            }

            project.CompileConstants = "PRE_KEYATTRIBUTE";
        }

        private void RemovePluginExampleFiles(bool includeExamples, ProjectInfo project)
        {
            if (includeExamples)
            {
                return;
            }
            project.FilesToRemove.AddRange(
                new[]
                {
                    @"PluginBaseExamples\EntityAccess.cs",
                    @"PluginBaseExamples\ContextExample.cs",
                    @"PluginBaseExamples\VoidPayment.cs",
                    @"RemovePhoneNumberFormatting.cs",
                    @"RenameLogic.cs",
                    @"SyncContactToAccount.cs"
                });
        }

        protected void AddPluginTest(Dictionary<string, ProjectInfo> projects, SolutionEditorInfo info)
        {
            var project = CreateDefaultProjectInfo(
                ProjectInfo.Keys.PluginTests,
                info.PluginTestName,
                info.GetPluginAssemblyVersionForSdk(),
                info.SharedTestCoreProject);

            RemoveExamplePluginTests(info, project);
            project.ProjectsReferences.Add(projects[ProjectInfo.Keys.Plugin]);
            project.ProjectsReferences.Add(projects[ProjectInfo.Keys.Test]);
            project.SharedProjectsReferences.Add(projects[ProjectInfo.Keys.TestCore]);
            projects.Add(project.Key, project);
        }

        private static void RemoveExamplePluginTests(SolutionEditorInfo info, ProjectInfo project)
        {
            if (info.IncludeExamplePlugins)
            {
                return;
            }
            project.FilesToRemove.AddRange(
                new[]
                {
                    "AssumptionExampleTests.cs",
                    "TestMethodClassExampleTests.cs",
                    "EntityBuilderExampleTests.cs",
                    "RemovePhoneNumberFormattingTests.cs",
                    "MsFakesVsXrmUnitTestExampleTests.cs",
                    "LocalOrServerPluginTest.cs",
                });
        }

        protected void AddWorkflow(Dictionary<string, ProjectInfo> projects, SolutionEditorInfo info)
        {
            var project = CreateDefaultProjectInfo(
                ProjectInfo.Keys.Workflow,
                info.WorkflowName,
                info.GetPluginAssemblyVersionForSdk(),
                info.SharedCommonProject);
            project.AddRegenKeyPostUpdateCommand(StrongNamePath);
            project.SharedProjectsReferences.Add(projects[ProjectInfo.Keys.Common]);
            project.Files.First().Replacements.Add(
                @"<Import Project=""..\Xyz.Xrm.WorkflowCore\Xyz.Xrm.WorkflowCore.projitems"" Label=""Shared"" />",
                $@"<Import Project=""..\{info.SharedCommonWorkflowProject}\{info.SharedCommonWorkflowProject}.projitems"" Label=""Shared"" />");
            if (!info.IncludeExampleWorkflow)
            {
                project.FilesToRemove.Add("CreateGuidActivity.cs");
            }
            projects.Add(project.Key, project);
        }

        protected void AddWorkflowTest(Dictionary<string, ProjectInfo> projects, SolutionEditorInfo info)
        {
            var project = CreateDefaultProjectInfo(
                ProjectInfo.Keys.WorkflowTests,
                info.WorkflowTestName,
                info.GetPluginAssemblyVersionForSdk(),
                info.SharedTestCoreProject);

            if (!info.IncludeExampleWorkflow)
            {
                project.FilesToRemove.AddRange(
                    new[]{
                        "WorkflowActivityExampleTests.cs"
                    });
            }
            project.ProjectsReferences.Add(projects[ProjectInfo.Keys.Workflow]);
            project.ProjectsReferences.Add(projects[ProjectInfo.Keys.Test]);
            project.SharedProjectsReferences.Add(projects[ProjectInfo.Keys.TestCore]);
            projects.Add(project.Key, project);
        }

        private void CreateProject(string projectKey, string rootNamespace)
        {
            Projects[projectKey].CopyFromAndUpdate(TemplateDirectory, rootNamespace);
        }

        protected void CreateProjects(string rootNamespace)
        {
            foreach (var project in Projects)
            {
                CreateProject(project.Key, rootNamespace);
            }
        }

        private void UpdateProjectsPostSolutionRestore()
        {
            foreach (var project in Projects)
            {
                project.Value.UpdatePostSolutionRestore();
            }
        }
    }
}
