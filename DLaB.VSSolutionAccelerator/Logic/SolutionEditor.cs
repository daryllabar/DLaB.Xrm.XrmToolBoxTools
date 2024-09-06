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
        public NuGetSettings NuGetSettings { get; set; }
        public string OutputBaseDirectory { get; }
        public Dictionary<string, ProjectInfo> Projects { get; set; }
        public string SolutionPath { get; }
        public string StrongNamePath { get; }
        public string TemplateDirectory { get; }

        public SolutionEditor(string solutionPath, 
                              string templateDirectory, 
                              string strongNamePath = null,
                              NuGetSettings nuGetSettings = null)
        {
            NuGetSettings = nuGetSettings ?? new NuGetSettings(templateDirectory);
            StrongNamePath = strongNamePath ?? Path.Combine(templateDirectory, "bin\\sn.exe");
            TemplateDirectory = templateDirectory;
            SolutionPath = solutionPath;
            OutputBaseDirectory = Path.GetDirectoryName(solutionPath);
        }

        protected void ExecuteNuGetRestoreForSolution()
        {
            var cmd = new ProcessExecutorInfo(NuGetSettings.ExePath, $"restore \"{SolutionPath}\" -NonInteractive");
            Logger.Show("Restoring Nuget for the solution.");
            Logger.AddDetail(cmd.FileName + " " + cmd.Arguments);
            var results = ProcessExecutor.ExecuteCmd(cmd);
            Logger.Show(results);
            UpdateProjectsPostSolutionRestore();
        }

        //protected void AddNugetPostUpdateCommandsToProjects(Version xrmPackageVersion, Dictionary<string, ProjectInfo> projects)
        //{
        //    foreach (var project in projects.Values.Where(p => p.Type != ProjectInfo.ProjectType.SharedProj))
        //    {
        //        var mapper = new NuGetMapper(NuGetSettings, 
        //            xrmPackageVersion, 
        //            Path.Combine(TemplateDirectory, project.Key, "packages.config"), 
        //            Path.Combine(OutputBaseDirectory, project.Name, "packages.config"));
        //        project.AddNugetPostUpdateCommands(mapper,
        //            NuGetSettings.ContentInstallerPath,
        //            OutputBaseDirectory);
        //    }
        //}

        protected ProjectInfo CreateDefaultProjectInfo(string key, string name, SolutionEditorInfo info)
        {
            Logger.AddDetail($"Configuring Project {name} based on {key}.");
            var id = Guid.NewGuid();
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
                            {$"<AssemblyOriginatorKeyFile>{key}.Key.snk</AssemblyOriginatorKeyFile>", $"<AssemblyOriginatorKeyFile>{name}.Key.snk</AssemblyOriginatorKeyFile>"},
                            {@"<Import Project=""..\Xyz.Xrm.WorkflowCore\Xyz.Xrm.WorkflowCore.projitems"" Label=""Shared"" />", $@"<Import Project=""..\{info.SharedCommonWorkflowProject}\{info.SharedCommonWorkflowProject}.projitems"" Label=""Shared"" />"},
                            {@"<ProjectReference Include=""..\Xyz.Xrm\Xyz.Xrm.csproj"" />", $@"<ProjectReference Include=""..\{info.SharedCommonProject}\{info.SharedCommonProject}.csproj"" />" },
                            {@"<ProjectReference Include=""..\Xyz.Xrm.Plugin\Xyz.Xrm.Plugin.csproj"" />", $@"<ProjectReference Include=""..\{info.PluginName}\{info.PluginName}.csproj"" />" },
                            {@"<ProjectReference Include=""..\Xyz.Xrm.Test\Xyz.Xrm.Test.csproj"" />", $@"<ProjectReference Include=""..\{info.TestBaseProject}\{info.TestBaseProject}.csproj"" />" },
                            {@"<ProjectReference Include=""..\Xyz.Xrm.Workflow\Xyz.Xrm.Workflow.csproj"" />", $@"<ProjectReference Include=""..\{info.WorkflowName}\{info.WorkflowName}.csproj"" />" },
                        },
                        Removals = new List<string>()
                    }
                },
            };
            return project;
        }


        protected void AddPlugin(Dictionary<string, ProjectInfo> projects, SolutionEditorInfo info)
        {
            if (!info.CreatePlugin)
            {
                return;
            }

            var project = CreateDefaultProjectInfo(ProjectInfo.Keys.Plugin, info.PluginName, info);
            var pluginProjectFile = project.Files.First();
            pluginProjectFile.Replacements.Add(@"<PackageId>Xyz.Xrm.Plugin</PackageId>", $@"<PackageId>{info.PluginName}</PackageId>");
            pluginProjectFile.Replacements.Add(@"<Authors>Matt Barbour</Authors>", $@"<Authors>{info.PluginPackage.Company}</Authors>");
            pluginProjectFile.Replacements.Add(@"<Company>Xyz</Company>", $@"<Company>{info.PluginPackage.Company}</Company>");
            pluginProjectFile.Replacements.Add(@"<Description>Plugin with Dependent Assemblies</Description>", $@"<Description>{info.PluginPackage.Description}</Description>");
            pluginProjectFile.Replacements.Add(@"<DeploymentPacAuthName>Xyz Dev</DeploymentPacAuthName>", $@"<DeploymentPacAuthName>{info.PluginPackage.PacAuthName}</DeploymentPacAuthName>");

            RemovePluginExampleFiles(info.IncludeExamplePlugins, project);
            projects.Add(project.Key, project);
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
                    @"RaceConditionPlugin.cs",
                    @"RemovePhoneNumberFormatting.cs",
                    @"RenameLogic.cs",
                    @"ServicesExamplePlugin.cs",
                    @"SyncContactToAccount.cs",
                    @"ValidateEmailRouterApproval"
                });
        }

        protected void AddPluginTest(Dictionary<string, ProjectInfo> projects, SolutionEditorInfo info)
        {
            if (!info.CreatePlugin || !info.CreatePluginTest)
            {
                return;
            }
            var project = CreateDefaultProjectInfo(ProjectInfo.Keys.PluginTests, info.PluginTestName, info);

            RemoveExamplePluginTests(info, project);
            project.ProjectsReferences.Add(projects[ProjectInfo.Keys.Plugin]);
            project.ProjectsReferences.Add(projects[ProjectInfo.Keys.Test]);
            project.ProjectsReferences.Add(projects[ProjectInfo.Keys.Common]);
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
                    "EntityBuilderExampleTests.cs",
                    "LocalOrServerPluginTest.cs",
                    "MsFakesVsXrmUnitTestExampleTests.cs",
                    "RaceConditionPluginTests.cs",
                    "RemovePhoneNumberFormattingTests.cs",
                    "ServicesExamplePluginTests.cs",
                    "TestMethodClassExampleTests.cs",
                    "ValidateEmailRouterApprovalTests.cs",
                });
        }

        protected void AddWorkflow(Dictionary<string, ProjectInfo> projects, SolutionEditorInfo info)
        {
            if (!info.CreateWorkflow)
            {
                return;
            }

            var project = CreateDefaultProjectInfo(ProjectInfo.Keys.Workflow, info.WorkflowName, info);
            project.AddRegenKeyPostUpdateCommand(StrongNamePath);
            if (!info.IncludeExampleWorkflow)
            {
                project.FilesToRemove.Add("CreateGuidActivity.cs");
            }
            projects.Add(project.Key, project);
        }

        protected void AddWorkflowTest(Dictionary<string, ProjectInfo> projects, SolutionEditorInfo info)
        {
            if (!info.CreateWorkflow || !info.CreateWorkflowTest)
            {
                return;
            }

            var project = CreateDefaultProjectInfo(ProjectInfo.Keys.WorkflowTests, info.WorkflowTestName, info);

            if (!info.IncludeExampleWorkflow)
            {
                project.FilesToRemove.AddRange(
                    new[]{
                        "WorkflowActivityExampleTests.cs"   
                    });
            }
            project.ProjectsReferences.Add(projects[ProjectInfo.Keys.Workflow]);
            project.ProjectsReferences.Add(projects[ProjectInfo.Keys.Test]);
            projects.Add(project.Key, project);
        }

        private void CreateProject(string projectKey, string rootNamespace)
        {
            Projects[projectKey].CopyFromAndUpdate(TemplateDirectory, rootNamespace);
        }

        protected void CreateProjects(string rootNamespace)
        {
            foreach (var project in Projects.Where(p => p.Value.AddToSolution))
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
