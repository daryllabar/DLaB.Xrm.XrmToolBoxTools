using System.Collections.Generic;
using System.IO;
using DLaB.Log;

namespace DLaB.VSSolutionAccelerator.Logic
{
    public class SolutionUpdater : SolutionEditor
    {
        public SolutionUpdater(string solutionPath, string templateDirectory, string strongNamePath = null, NuGetSettings nuGetSettings = null)
            : base(solutionPath, templateDirectory, strongNamePath, nuGetSettings)
        { }

        public Dictionary<string, ProjectInfo> GetProjectInfos(AddProjectToSolutionInfo info)
        {
            var projects = new Dictionary<string, ProjectInfo>();
            AddExistingSharedProjectDependencies(projects, info);
            AddPlugin(projects, info);
            if (info.CreatePluginTest)
            {
                AddPluginTest(projects, info);
            }
            if (info.CreateWorkflow)
            {
                AddWorkflow(projects, info);
                if (info.CreateWorkflowTest)
                {
                    AddWorkflowTest(projects, info);
                }
            }

            //AddNugetPostUpdateCommandsToProjects(info.XrmVersion, projects);
            return projects;
        }

        private void AddExistingSharedProjectDependencies(Dictionary<string, ProjectInfo> projects, AddProjectToSolutionInfo info)
        {
            AddExistingPluginSharedProjectDependencies(info, projects);
            AddExistingTestSharedProjectDependencies(info, projects);
        }

        private void AddExistingPluginSharedProjectDependencies(AddProjectToSolutionInfo info, Dictionary<string, ProjectInfo> projects)
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

            }

            if (info.CreateWorkflow)
            {
                if (info.CreateCommonWorkflowProject)
                {
                    AddSharedWorkflowProject(projects, info, info.WorkflowName);
                }
                else
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
                projects[ProjectInfo.Keys.Test] = new ProjectInfo
                {
                    Type = ProjectInfo.ProjectType.CsProj,
                    Key = ProjectInfo.Keys.Test,
                    AddToSolution = false,
                    Name = info.TestBaseProject
                };
            }
        }

        public static void Execute(AddProjectToSolutionInfo info, string templateDirectory, string strongNamePath = null, NuGetSettings nuGetSettings = null)
        {
            Logger.AddDetail($"Starting to process solution '{info.SolutionPath}' using templates from '{templateDirectory}'");
            var adder = new SolutionUpdater(info.SolutionPath, templateDirectory, strongNamePath, nuGetSettings);
            adder.Projects = adder.GetProjectInfos(info);
            adder.CreateProjects(adder.Projects[ProjectInfo.Keys.Common].Name);
            IEnumerable<string> solution = File.ReadAllLines(adder.SolutionPath);
            solution = SolutionFileEditor.AddMissingProjects(solution, adder.Projects.Values);
            File.WriteAllLines(adder.SolutionPath, solution);
            adder.ExecuteNuGetRestoreForSolution();
        }
    }
}
