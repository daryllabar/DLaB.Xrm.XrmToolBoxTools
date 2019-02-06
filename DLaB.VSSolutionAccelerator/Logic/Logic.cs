using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Source.DLaB.Common;

namespace DLaB.VSSolutionAccelerator.Logic
{
    public class Logic
    {
        public string OutputBaseDirectory { get; }
        public string SolutionPath { get; }
        public string TemplateDirectory { get; }
        public string StrongNamePath { get; }
        public Dictionary<string, ProjectInfo> Projects { get; set; }

        public Logic(string solutionPath, string templateDirectory, string strongNamePath = null)
        {
            SolutionPath = solutionPath;
            OutputBaseDirectory = Path.GetDirectoryName(solutionPath);
            TemplateDirectory = templateDirectory;
            StrongNamePath = strongNamePath ?? Path.Combine(templateDirectory, "StrongName\\sn.exe"); ;
        }

        public Dictionary<string, ProjectInfo> GetProjectInfos(InitializeSolutionInfo info)
        {
            var projects = new Dictionary<string, ProjectInfo>();
            AddSharedCommonProject(projects, info);
            AddSharedWorkflowProject(projects, info);
            if (info.ConfigureXrmUnitTest)
            {
                AddSharedTestCoreProject(projects, info);
                AddBaseTestProject(projects, info);
            }
            if (info.CreatePlugin)
            {
                AddPlugin(projects, info);
            }
            if (info.CreateWorkflow)
            {
                AddWorkflow(projects, info);
            }
            return projects;
        }

        private void AddSharedCommonProject(Dictionary<string, ProjectInfo> projects, InitializeSolutionInfo info)
        {
            var project = CreateDefaultSharedProjectInfo(
                ProjectInfo.Keys.Common,
                info.SharedCommonProject,
                "b22b3bc6-0ac6-4cdd-a118-16e318818ad7");

            project.Files.First(f => f.Name.EndsWith(".projitems")).Removals.Add("$(MSBuildThisFileDirectory)Entities");
            projects.Add(project.Key, project);
        }

        private void AddSharedWorkflowProject(Dictionary<string, ProjectInfo> projects, InitializeSolutionInfo info)
        {
            var project = CreateDefaultSharedProjectInfo(
                ProjectInfo.Keys.WorkflowCommon,
                info.SharedCommonWorkflowProject,
                "dd5aa002-c1ff-4c0e-b9a5-3d63c7809b07",
                "Xyz.Xrm.Workflow",
                info.RootNamespace + ".Workflow");
            projects.Add(project.Key, project);
        }

        private void AddSharedTestCoreProject(Dictionary<string, ProjectInfo> projects, InitializeSolutionInfo info)
        {
            var project = CreateDefaultSharedProjectInfo(
                ProjectInfo.Keys.TestCore,
                info.SharedTestCoreProject,
                "8f91efc7-351b-4802-99aa-6c6f16110505", ProjectInfo.Keys.Test);
            projects.Add(project.Key, project);
        }

        private void AddBaseTestProject(Dictionary<string, ProjectInfo> projects, InitializeSolutionInfo info)
        {
            var project = CreateDefaultProjectInfo(
                ProjectInfo.Keys.Test,
                info.TestBaseProject,
                "F62103E9-D25D-4F99-AABE-ECF348424366",
                "v.4.6.2",
                info.SharedCommonProject);
            //info.XrmPackage.Version.Major >= 9 ? "v.4.6.2" : "v.4.5.2"
            projects.Add(project.Key, project);
        }

        private void AddPlugin(Dictionary<string, ProjectInfo> projects, InitializeSolutionInfo info)
        {
            var project = CreateDefaultProjectInfo(
                ProjectInfo.Keys.Plugin,
                info.PluginName,
                "2B294DBF-8730-436E-B401-8745FEA632FE",
                GetPluginAssemblyVersionForSdk(info),
                info.SharedCommonProject);
            project.PostUpdateCommands.Add(new ProcessExecutorInfo(StrongNamePath, $"-k {project.Name}.Key.snk"));
            if (!info.IncludeExamplePlugins)
            {
                project.FilesToRemove.AddRange(
                    new []{@"PluginBaseExamples\EntityAccess.cs",
                    @"PluginBaseExamples\ContextExample.cs",
                    @"PluginBaseExamples\VoidPayment.cs",
                    @"Properties\AssemblyInfo.cs",
                    @"RemovePhoneNumberFormatting.cs",
                    @"RenameLogic.cs",
                    @"SyncContactToAccount.cs"
                    });
            }
            projects.Add(project.Key, project);
        }

        private void AddWorkflow(Dictionary<string, ProjectInfo> projects, InitializeSolutionInfo info)
        {
            var project = CreateDefaultProjectInfo(
                ProjectInfo.Keys.Workflow,
                info.WorkflowName,
                "5BD39AC9-97F3-47C8-8E1F-6A58A24AFB9E",
                GetPluginAssemblyVersionForSdk(info),
                info.SharedCommonProject);
            project.PostUpdateCommands.Add(new ProcessExecutorInfo(StrongNamePath, $"-k {project.Name}.Key.snk"));
            project.Files.First().Replacements.Add(
                @"<Import Project=""..\Xyz.Xrm.WorkflowCore\Xyz.Xrm.WorkflowCore.projitems"" Label=""Shared"" />", 
                $@"<Import Project=""..\{info.SharedCommonWorkflowProject}\{info.SharedCommonWorkflowProject}.projitems"" Label=""Shared"" />");
            if (!info.IncludeExampleWorkflow)
            {
                project.FilesToRemove.Add("CreateGuidActivity.cs");
            }
            projects.Add(project.Key, project);
        }

        private string GetPluginAssemblyVersionForSdk(InitializeSolutionInfo info)
        {
            return info.XrmPackage.Version.Major >= 9 ? "v4.6.2" : "v4.5.2";
        }

        private ProjectInfo CreateDefaultProjectInfo(string key, string name, string originalId, string dotNetFramework, string sharedCommonProject)
        {
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
                            {originalId, id.ToString().ToUpper()},
                            {$"<RootNamespace>{key}</RootNamespace>", $"<RootNamespace>{name}</RootNamespace>"},
                            {$"<AssemblyName>{key}</AssemblyName>", $"<AssemblyName>{name}</AssemblyName>"},
                            {"<TargetFrameworkVersion>v4.6.2</TargetFrameworkVersion>", $"<TargetFrameworkVersion>{dotNetFramework}</TargetFrameworkVersion>"},
                            {"<TargetFrameworkVersion>v4.5.2</TargetFrameworkVersion>", $"<TargetFrameworkVersion>{dotNetFramework}</TargetFrameworkVersion>"},
                            {$"<AssemblyOriginatorKeyFile>{key}.Key.snk</AssemblyOriginatorKeyFile>", $"<AssemblyOriginatorKeyFile>{name}.Key.snk</AssemblyOriginatorKeyFile>"},
                            {$"<None Include=\"{key}.Key.snk\" />", $"<None Include=\"{name}.Key.snk\" />"},
                            {@"<Import Project=""..\Xyz.Xrm\Xyz.Xrm.projitems"" Label=""Shared"" />", $@"<Import Project=""..\{sharedCommonProject}\{sharedCommonProject}.projitems"" Label=""Shared"" />"},
                        },
                        Removals = new List<string>
                        {
                            "<CodeAnalysisRuleSet>"
                        }
                    }
                },
            };
            return project;
        }

        private ProjectInfo CreateDefaultSharedProjectInfo(string key, string name, string originalId, string originalNamespace = null, string newNamespace = null)
        {
            originalNamespace = originalNamespace ?? key;
            newNamespace = newNamespace ?? name;
            var id = Guid.NewGuid();
            var project = new ProjectInfo
            {
                Key = key,
                Id = id,
                Type = ProjectInfo.ProjectType.SharedProj,
                NewDirectory = Path.Combine(OutputBaseDirectory, name),
                Name = name,
                Files = new List<ProjectFile>
                {
                    new ProjectFile
                    {
                        Name = name + ".projitems",
                        Replacements = new Dictionary<string, string>
                        {
                            {originalId, id.ToString()},
                            {$"<Import_RootNamespace>{originalNamespace}</Import_RootNamespace>", $"<Import_RootNamespace>{newNamespace}</Import_RootNamespace>"}
                        },
                    },
                    new ProjectFile
                    {
                        Name = name + ".shproj",
                        Replacements = new Dictionary<string, string>
                        {
                            {originalId, id.ToString()},
                            { key +".projitems", name + ".projitems"}
                        }
                    },
                }
            };
            return project;
        }

        public static void Execute(InitializeSolutionInfo info, string templateDirectory, string strongNamePath = null)
        {
            var logic = new Logic(info.SolutionPath, templateDirectory, strongNamePath);
            logic.Projects = logic.GetProjectInfos(info);
            foreach (var project in logic.Projects)
            {
                logic.CreateProject(project.Key, info);
            }

            // Add the Code Generation Files and projects to the solution, and VS install folder
            // If ConfigureXrmUnitTest
            // - Create SharedTestProject and SharedTestCoreProject
            // - Replace namespace
            // - Add project to ToBeAddedToSolution
            // If CreatePlugin
            // - Create Project
            // - Replace Namespace
            // - Add references to shared Common Project
            // - Add project to ToBeAddedToSolution
            // - If ConfigureXrmUnitTest
            // - - Create Test Project
            // - - Replace namespace
            // - - Add project to ToBeAddedToSolution
            // - - Update Reference to Test plugin
            // If CreateWorkflow
            // - Create Project
            // - Replace Namespace
            // - Add references to shared Common Project and Workflow Project
            // - Add project to ToBeAddedToSolution
            // - If ConfigureXrmUnitTest
            // - - Create Test Project
            // - - Replace namespace
            // - - Add project to ToBeAddedToSolution
            // - - Update Reference to Test plugin
            // Update the solution with the ToBeAddedToSolution Projects
            // If EarlyBound
            // - Move the Settings File to the Code Generation Folder add to clipboard, and Update Paths and Open EBG

            IEnumerable<string> solution = File.ReadAllLines(logic.SolutionPath);
            solution = SolutionFileEditor.AddMissingProjects(solution, logic.Projects.Values);
            File.WriteAllLines(logic.SolutionPath, solution);
        }

        public void CreateProject(string projectKey, InitializeSolutionInfo info)
        {
            Projects[projectKey].CopyFromAndUpdate(TemplateDirectory, info.RootNamespace, info.XrmPackage.Version);
        }
    }
}
