using System.Collections.Generic;
using System.IO;
using System.Linq;
using DLaB.Log;

namespace DLaB.VSSolutionAccelerator.Logic
{
    public class SolutionUpdater : SolutionEditor
    {
        public SolutionUpdater(string solutionPath, string templateDirectory, string strongNamePath = null, string nugetPath = null)
            : base(solutionPath, templateDirectory, strongNamePath, nugetPath)
        { }

        public Dictionary<string, ProjectInfo> GetProjectInfos(AddProjectToSolutionInfo info)
        {
            var projects = new Dictionary<string, ProjectInfo>();
            AddExistingSharedProjectDependencies(info, projects);

            if (info.CreatePlugin)
            {
                AddPlugin(projects, info);
                if (info.CreatePluginTest)
                {
                    AddPluginTest(projects, info);
                }
            }
            if (info.CreateWorkflow)
            {
                AddWorkflow(projects, info);
                if (info.CreateWorkflowTest)
                {
                    AddWorkflowTest(projects, info);
                }
            }

            AddNugetPostUpdateCommandsToProjects(info.XrmVersion, projects);
            return projects;
        }

        private static void AddExistingSharedProjectDependencies(AddProjectToSolutionInfo info, Dictionary<string, ProjectInfo> projects)
        {
            AddExistingPluginSharedProjectDependencies(info, projects);
            AddExistingTestSharedProjectDependencies(info, projects);
        }

        private static void AddExistingPluginSharedProjectDependencies(AddProjectToSolutionInfo info, Dictionary<string, ProjectInfo> projects)
        {
            if (info.CreatePlugin || info.CreateWorkflow)
            {
                projects[ProjectInfo.Keys.Common] = new ProjectInfo
                {
                    Type = ProjectInfo.ProjectType.SharedProj,
                    Key = ProjectInfo.Keys.Common,
                    AddToSolution = false,
                    Name = info.SharedCommonProject
                };

                if (info.CreateWorkflow)
                {
                    projects[ProjectInfo.Keys.WorkflowCommon] = new ProjectInfo
                    {
                        Type = ProjectInfo.ProjectType.SharedProj,
                        Key = ProjectInfo.Keys.WorkflowCommon,
                        AddToSolution = false,
                        Name = info.SharedCommonWorkflowProject
                    };
                }
            }
        }

        private static void AddExistingTestSharedProjectDependencies(AddProjectToSolutionInfo info, Dictionary<string, ProjectInfo> projects)
        {
            if (info.CreatePluginTest || info.CreateWorkflowTest)
            {
                projects[ProjectInfo.Keys.TestCore] = new ProjectInfo
                {
                    Type = ProjectInfo.ProjectType.SharedProj,
                    Key = ProjectInfo.Keys.TestCore,
                    AddToSolution = false,
                    Name = info.SharedTestCoreProject
                };

                projects[ProjectInfo.Keys.Test] = new ProjectInfo
                {
                    Type = ProjectInfo.ProjectType.CsProj,
                    Key = ProjectInfo.Keys.Test,
                    AddToSolution = false,
                    Name = info.SharedTestProject.AssemblyName
                };
            }
        }

        public static void Execute(AddProjectToSolutionInfo info, string templateDirectory, string strongNamePath = null)
        {
            Logger.AddDetail($"Starting to process solution '{info.SolutionPath}' using templates from '{templateDirectory}'");
            var adder = new SolutionUpdater(info.SolutionPath, templateDirectory, strongNamePath);
            adder.Projects = adder.GetProjectInfos(info);
            adder.CreateProjects(string.Empty);
            IEnumerable<string> solution = File.ReadAllLines(adder.SolutionPath);
            solution = SolutionFileEditor.AddMissingProjects(solution, adder.Projects.Values);
            File.WriteAllLines(adder.SolutionPath, solution);
            adder.ExecuteNuGetRestoreForSolution();
        }
    }
}
