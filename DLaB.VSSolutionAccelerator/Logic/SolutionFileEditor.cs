using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Source.DLaB.Common;

namespace DLaB.VSSolutionAccelerator.Logic
{
    public class SolutionFileEditor
    {
        private string SolutionFolderId = "2150E333-8FDC-42A3-9474-1A3956D46DE8";
        public enum States
        {
            Start,
            Project,
            PostProject,
            Global,
            SharedMsBuildProjectFiles,
            SolutionConfigurationPlatforms,
            ProjectConfigurationPlatforms,
            Final,
            PostSolutionConfig
        }

        public Dictionary<States, Action<string>> StateProcessor => new Dictionary<States, Action<string>>
        {
            {States.Start, ProcessStart},
            {States.Project, ProcessProject },
            {States.PostProject, ProcessPostProject },
            {States.Global, ProcessGlobal },
            {States.SharedMsBuildProjectFiles, ProcessSharedMsBuildProjectFiles },
            {States.SolutionConfigurationPlatforms, ProcessSolutionConfigurationPlatforms },
            {States.PostSolutionConfig, ProcessPostSolutionConfig },
            {States.ProjectConfigurationPlatforms, ProcessProjectConfigurationPlatforms },
            {States.Final, ProcessFinal },
        };

        public States State { get; set; }
        private List<ProjectInfo> ProjectsToAddHeader { get; }
        private List<ProjectInfo> ProjectsToAddShared { get; }
        private List<ProjectInfo> ProjectsToInsertShared { get; }
        private List<ProjectInfo> ProjectsToAddConfig { get; }
        private List<string> SolutionConfigurationPlatforms { get; }
        public List<string> Result { get; }

        public SolutionFileEditor(IEnumerable<ProjectInfo> projectsToAdd)
        {
            State = States.Start;
            Result = new List<string>();
            SolutionConfigurationPlatforms = new List<string>();
            var projects = projectsToAdd.ToList();
            ProjectsToAddHeader = projects.Where(p => p.AddToSolution)
                                          .OrderBy(p => p.Name).ToList();
            ProjectsToAddShared = projects.Where(p => p.AddToSolution 
                                                      && (p.Type == ProjectInfo.ProjectType.SharedProj || p.SharedProjectsReferences.Any()))
                                          .OrderBy(p => p.Id).ToList();
            ProjectsToInsertShared = projects.Where(p => !p.AddToSolution
                                                      && p.SharedProjectsReferences.Any(s => s.AddToSolution))
                                             .OrderBy(p => p.Id).ToList();
            ProjectsToAddConfig = projects.Where(p => p.AddToSolution).ToList();

        }

        private void Process(IEnumerable<string> solution)
        {
            foreach (var line in solution)
            {
                StateProcessor[State](line);
            }
        }

        private void ProcessStart(string line)
        {
            if (line.StartsWith("MinimumVisualStudioVersion"))
            {
                State = States.Project;
            }

            Result.Add(line);
        }

        private void ProcessProject(string line)
        {
            if (EndOfProjectSection(line))
            {
                State = line.Contains(SolutionFolderId) ? States.PostProject : States.Global;
                foreach (var p in ProjectsToAddHeader)
                {
                    Result.Add(p.SolutionHeader);
                }

                Result.Add(line);
            }
            else if (line.TrimStart().StartsWith("EndProject"))
            {
                Result.Add(line);
            }
            else if (ProjectsToAddHeader.Any())
            {
                var projectName = GetProjectName(line);
                var project = ProjectsToAddHeader.First();
                while (project != null && string.Compare(projectName, project.Name, StringComparison.OrdinalIgnoreCase) > 0)
                {
                    Result.Add(project.SolutionHeader);
                    ProjectsToAddHeader.RemoveAt(0);
                    project = ProjectsToAddHeader.FirstOrDefault();
                }

                Result.Add(line);
            }
            else
            {
                Result.Add(line);
            }
        }

        private void ProcessPostProject(string line)
        {
            if (line.TrimStart().StartsWith("Global"))
            {
                State = States.Global;
            }

            Result.Add(line);
        }

        private bool EndOfProjectSection(string line)
        {
            return line.Contains(SolutionFolderId)
                   || line.TrimStart().StartsWith("Global");
        }

        private void ProcessGlobal(string line)
        {
            if (line.Contains("(SharedMSBuildProjectFiles)"))
            {
                State = States.SharedMsBuildProjectFiles;
                Result.Add(line);
            }
            else if (line.Contains("(SolutionConfigurationPlatforms)"))
            {
                State = States.SolutionConfigurationPlatforms;
                // No Shared Project Files Section.  Add here
                AddSharedMsBuildProjectFiles();
                Result.Add(line);
            }
            else if (line.Contains("(SolutionProperties)"))
            {
                State = States.Final;
                // No Shared Project Files Or Configuration Platform Section.  Add Here
                AddSharedMsBuildProjectFiles();
                AddConfigPlatform();
                Result.Add(line);
            }
        }

        private void AddSharedMsBuildProjectFiles()
        {
            Result.Add("\tGlobalSection(SharedMSBuildProjectFiles) = preSolution");
            ProjectsToAddShared.AddRange(ProjectsToInsertShared);
            ProjectsToInsertShared.Clear();
            foreach (var project in ProjectsToAddShared.OrderBy(p => p.Id))
            {
                Result.Add(project.GetSharedMsBuildProjectFiles());
            }
            Result.Add("\tEndGlobalSection");
            ProjectsToAddShared.Clear();
        }

        private void AddConfigPlatform()
        {
            Result.Add("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
            var solutionConfigs = new []
            {
                "\t\tDebug|Any CPU = Debug|Any CPU",
                "\t\tRelease|Any CPU = Release|Any CPU"
            };

            Result.Add(solutionConfigs[0]);
            Result.Add(solutionConfigs[1]);
            Result.Add("\tEndGlobalSection");

            foreach (var projectConfig in ProjectsToAddConfig
                .OrderBy(p => p.Id)
                .Select(p => p.GetProjectConfigurationPlatforms(solutionConfigs))
                .Where(p => p != null))
            {
                Result.Add(projectConfig);
            }
        }

        private void ProcessSharedMsBuildProjectFiles(string line)
        {
            // Add ProjectsToAddShared and ProjectsToInsertShared, ordered by Project Id
            throw new NotImplementedException();
        }

        private void ProcessSolutionConfigurationPlatforms(string line)
        {
            if (line.TrimStart().StartsWith("EndGlobalSection"))
            {
                State = States.PostSolutionConfig;
                Result.Add(line);
                return;
            }

            if (line.Contains("ProjectConfigurationPlatforms"))
            {
                State = States.ProjectConfigurationPlatforms;
                Result.Add(line);
            }
            SolutionConfigurationPlatforms.Add(line);
        }

        private void ProcessPostSolutionConfig(string line)
        {
            if (line.Contains("ProjectConfigurationPlatforms"))
            {
                State = States.ProjectConfigurationPlatforms;
            }
            else if (line.Contains("SolutionProperties") 
                     || line.Contains("ExtensibilityGlobals")
                     || line.Contains("EndGlobal"))
            {
                // No Project Configuration Platforms Add
                AddProjectConfigurationPlatforms();
            }

            Result.Add(line);
        }

        private void AddProjectConfigurationPlatforms()
        {
            Result.Add("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
            foreach (var project in ProjectsToAddConfig)
            {
                Result.Add(project.GetProjectConfigurationPlatforms(SolutionConfigurationPlatforms));
            }
            Result.Add("\tEndGlobalSection");
        }

        private void ProcessProjectConfigurationPlatforms(string line)
        {
            if (line.TrimStart().StartsWith("EndGlobalSection"))
            {

                Result.Add(line);
                State = States.Final;
                return;
            }
            throw new NotImplementedException();
        }

        private void ProcessFinal(string line)
        {
            Result.Add(line);
        }

        /// <summary>
        /// Parse:
        ///     Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Xyz.Walkthrough", "Xyz.Walkthrough\Xyz.Walkthrough.csproj", "{3E728B41-02E5-42FD-B8D3-CF2C664E2226}"
        /// Return:
        ///    Xyz.Walkthrough
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static string GetProjectName(string line)
        {
            return line.Split('=')[1].TrimStart().SubstringByString("\"", "\",");
        }

        public static IEnumerable<string> AddMissingProjects(IEnumerable<string> solution, IEnumerable<ProjectInfo> projects)
        {
            var editor = new SolutionFileEditor(projects);
            editor.Process(solution);
            return editor.Result;
        }
    }
}
