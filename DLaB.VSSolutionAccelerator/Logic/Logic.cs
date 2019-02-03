using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DLaB.VSSolutionAccelerator.Logic
{
    public class Logic
    {
        public string OutputBaseDirectory { get; }
        public string SolutionPath { get; }
        public string TemplateDirectory { get; }
        public Dictionary<string, ProjectInfo> Projects { get; set; }

        public Logic(string solutionPath, string templateDirectory)
        {
            SolutionPath = solutionPath;
            OutputBaseDirectory = Path.GetDirectoryName(solutionPath);
            TemplateDirectory = templateDirectory;
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
                "dd5aa002-c1ff-4c0e-b9a5-3d63c7809b07");
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
            var key = ProjectInfo.Keys.Test;
            var name = info.TestBaseProject;
            var originalId = "F62103E9-D25D-4F99-AABE-ECF348424366";
            var id = new Guid();
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
                        Name = name + ".csproj",
                        Replacements = new Dictionary<string, string>
                        {
                            {originalId, id.ToString().ToUpper()},
                            {$"<Import_RootNamespace>{key}</Import_RootNamespace>", $"<Import_RootNamespace>{name}</Import_RootNamespace>"}
                        },
                        Removals = new List<string>
                        {
                            "<CodeAnalysisRuleSet>"
                        }
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

            projects.Add(key, project);
        }

        private ProjectInfo CreateDefaultSharedProjectInfo(string key, string name, string originalId, string originalNamespace = null)
        {
            originalNamespace = originalNamespace ?? key;
            var id = new Guid();
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
                            {$"<Import_RootNamespace>{originalNamespace}</Import_RootNamespace>", $"<Import_RootNamespace>{name}</Import_RootNamespace>"}
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

        public static void Execute(InitializeSolutionInfo info, string templateDirectory)
        {
            var logic = new Logic(info.SolutionPath, templateDirectory);
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
