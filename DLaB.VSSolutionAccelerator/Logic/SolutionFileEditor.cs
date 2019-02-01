using System.Collections.Generic;
using System.Linq;

namespace DLaB.VSSolutionAccelerator.Logic
{
    public class SolutionFileEditor
    {
        public static IEnumerable<string> AddMissingProjects(IEnumerable<string> solution, IEnumerable<ProjectInfo> projectInfos)
        {
            var parser = new SolutionFileParser(solution);
            var projects = projectInfos.ToList();
            var projectsToAdd = projects.Where(p => p.AddToSolution).ToList();
            foreach (var project in projectsToAdd.OrderBy(p => p.Name))
            {
                parser.Projects.Add(project.SolutionProjectHeader);
            }

            RemoveExistingGlobalSharedProjectsWithNewlyAddedSharedProject(projects, parser);

            foreach (var project in projects.Where(NeedsToUpdateGlobalSharedProjects).OrderBy(p => p.Id))
            {
                var files = project.GetSharedMsBuildProjectFiles();
                if(files != null)
                {
                    parser.GlobalSharedProjects.Add(files);
                }
            }

            foreach (var project in projectsToAdd.OrderBy(p => p.Id))
            {
                var configs = project.GetProjectConfigurationPlatforms(parser.GlobalSolutionConfigs);
                if (configs != null)
                {
                    parser.GlobalProjectConfigs.Add(configs);
                }
            }

            return parser.GetSolution();
        }

        private static bool NeedsToUpdateGlobalSharedProjects(ProjectInfo project)
        {
            return project.AddToSolution 
                   && (
                       project.Type == ProjectInfo.ProjectType.SharedProj 
                       || project.SharedProjectsReferences.Any())
                   || !project.AddToSolution
                   && project.SharedProjectsReferences.Any(shared => shared.AddToSolution);
        }

        private static void RemoveExistingGlobalSharedProjectsWithNewlyAddedSharedProject(List<ProjectInfo> projects, SolutionFileParser parser)
        {
            foreach (var project in projects.Where(p => !p.AddToSolution && NeedsToUpdateGlobalSharedProjects(p)))
            {
                var linesToRemove = new List<string>();
                foreach (var sharedProj in parser.GlobalSharedProjects)
                {
                    if (sharedProj.Contains(project.Id.ToString()))
                    {
                        linesToRemove.Add(sharedProj);
                    }
                }

                foreach (var line in linesToRemove)
                {
                    parser.GlobalSharedProjects.Remove(line);
                }
            }
        }
    }
}
