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
            var id = Guid.NewGuid();
            AddSharedCommonProject(projects, info, id);
            return projects;
        }

        private void AddSharedCommonProject(Dictionary<string, ProjectInfo> projects, InitializeSolutionInfo info, Guid id)
        {
            var project = new ProjectInfo
            {
                Key = ProjectInfo.Keys.Common,
                Id = id,
                Type = ProjectInfo.ProjectType.SharedProj,
                NewDirectory = Path.Combine(OutputBaseDirectory, info.SharedCommonProject),
                Name = info.SharedCommonProject,
                Files = new List<ProjectFile>
                {
                    new ProjectFile
                    {
                        Name = info.SharedCommonProject + ".projitems",
                        Replacements = new Dictionary<string, string>
                        {
                            { "b22b3bc6-0ac6-4cdd-a118-16e318818ad7", id.ToString()},
                            { "<Import_RootNamespace>Xyz.Xrm</Import_RootNamespace>", $"<Import_RootNamespace>{info.RootNamespace}</Import_RootNamespace>" }
                        },
                        Removals = new List<string>
                        {
                            "$(MSBuildThisFileDirectory)Entities"
                        }
                    },
                    new ProjectFile
                    {
                        Name = info.SharedCommonProject + ".shproj",
                        Replacements = new Dictionary<string, string>
                        {
                            { "b22b3bc6-0ac6-4cdd-a118-16e318818ad7", id.ToString()},
                            { "Xyz.Xrm.projitems", $"{info.SharedCommonProject}.projitems" }
                        }
                    },
                }
            };
            projects.Add(project.Key, project);
        }

        public static void Execute(InitializeSolutionInfo info, string templateDirectory)
        {
            var logic = new Logic(info.SolutionPath, templateDirectory);
            logic.Projects = logic.GetProjectInfos(info);
            logic.CreateSharedCommonProject(info);
            // Create Shared Project with the SharedCommonProject name/path
            // - Replace namespace
            // - Add project to ToBeAddedToSolution
            // Create Shared Workflow Project with the SharedCommonWorkflowProject name/path
            // - Replace namespace
            // - Add project to ToBeAddedToSolution
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

            logic.AddProjectsToSolution();
        }

        public void CreateSharedCommonProject(InitializeSolutionInfo info)
        {
            Projects[ProjectInfo.Keys.Common].CopyFromAndUpdate(TemplateDirectory, info.RootNamespace);
        }

        public void AddProjectsToSolution()
        {
            var solution = File.ReadAllLines(SolutionPath);
            File.WriteAllLines(SolutionPath, SolutionFileEditor.AddMissingProjects(solution, Projects.Values));
        }
    }
}
