using System;
using System.Collections.Generic;

namespace DLaB.VSSolutionAccelerator.Logic
{
    public class SolutionFileParser
    {
        public List<string> PreProjects { get; set; }
        public List<string> Projects { get; set; }
        public List<string> GlobalSharedProjects{ get; set; }
        public List<string> GlobalSolutionConfigs{ get; set; }
        public List<string> GlobalProjectConfigs{ get; set; }
        public List<string> PostGlobalProjectConfigs{ get; set; }

        public enum States
        {
            PreProject,
            Project,
            GlobalSharedProject,
            GlobalSolutionConfig,
            GlobalProjectConfig,
            PostGlobalProjectConfig,
        }

        public Dictionary<States, Action<string>> StateProcessor => new Dictionary<States, Action<string>>
        {
            {States.PreProject, ParsePreProject },
            {States.Project, ParseProject },
            {States.GlobalSharedProject, ParseGlobalSharedProjects },
            {States.GlobalSolutionConfig, ParseGlobalSolutionConfigs },
            {States.GlobalProjectConfig, ParseGlobalProjectConfigs },
            {States.PostGlobalProjectConfig, ParsePostGlobalProjectConfigs },
        };

        public struct LineMarkers
        {
            public const string GlobalProjectConfigsStart = "GlobalSection(ProjectConfigurationPlatforms) = postSolution";
            public const string GlobalSharedProjectStart = "GlobalSection(SharedMSBuildProjectFiles) = preSolution";
            public const string GlobalStart = "Global";
            public const string GlobalSectionEnd = "EndGlobalSection";
            public const string GlobalSolutionConfigsStart = "GlobalSection(SolutionConfigurationPlatforms) = preSolution";
            public const string ProjectStart = "Project(\"{";
        }

        public States State { get; set; }

        public SolutionFileParser(IEnumerable<string> solution)
        {
            if (solution == null)
            {
                solution = CreateNewEmptySolution();
            }
            foreach (var line in solution)
            {
                StateProcessor[State](line);
            }
        }

        private void ParsePreProject(string line)
        {
            PreProjects = PreProjects ?? new List<string>();
            if (line.TrimStart().StartsWith(LineMarkers.ProjectStart))
            {
                State = States.Project;
                StateProcessor[State](line);
            }
            else if (line.Trim() == LineMarkers.GlobalStart)
            {
                State = States.GlobalSharedProject;
            }
            else
            {
                Projects.Add(line);
            }
        }

        private void ParseProject(string line)
        {
            if (Projects == null)
            {
                if (line.TrimStart().StartsWith(LineMarkers.ProjectStart))
                {
                    Projects = new List<string>();
                    StateProcessor[State](line);
                }
                else
                {
                    State = States.GlobalSharedProject;
                    StateProcessor[State](line);
                }
            }
            else if (line.Trim() == LineMarkers.GlobalStart)
            {
                State = States.GlobalSharedProject;
            }
            else
            {
                Projects.Add(line);
            }
        }

        private void ParseGlobalSharedProjects(string line)
        {
            if (GlobalSharedProjects == null)
            {
                if (line.Trim() == LineMarkers.GlobalSharedProjectStart)
                {
                    GlobalSharedProjects = new List<string>();
                }
                else
                {
                    State = States.GlobalSolutionConfig;
                    StateProcessor[State](line);
                }
            }
            else if (line.Trim() == LineMarkers.GlobalSectionEnd)
            {
                State = States.GlobalSolutionConfig;
            }
            else
            {
                GlobalSharedProjects.Add(line);
            }
        }

        private void ParseGlobalSolutionConfigs(string line)
        {
            if (GlobalSolutionConfigs == null)
            {
                if (line.Trim() == LineMarkers.GlobalSolutionConfigsStart)
                {
                    GlobalSolutionConfigs = new List<string>();
                }
                else
                {
                    State = States.GlobalProjectConfig;
                    StateProcessor[State](line);
                }
            }
            else if (line.Trim() == LineMarkers.GlobalSectionEnd)
            {
                State = States.GlobalProjectConfig;
            }
            else
            {
                GlobalSolutionConfigs.Add(line);
            }
        }

        private void ParseGlobalProjectConfigs(string line)
        {
            if (GlobalProjectConfigs == null)
            {
                if (line.Trim() == LineMarkers.GlobalProjectConfigsStart)
                {
                    GlobalProjectConfigs = new List<string>();
                }
                else
                {
                    State = States.PostGlobalProjectConfig;
                    StateProcessor[State](line);
                }
            }
            else if (line.Trim() == LineMarkers.GlobalSectionEnd)
            {
                State = States.PostGlobalProjectConfig;
            }
            else
            {
                GlobalProjectConfigs.Add(line);
            }
        }

        private void ParsePostGlobalProjectConfigs(string line)
        {
            PostGlobalProjectConfigs = PostGlobalProjectConfigs ?? new List<string>();
            PostGlobalProjectConfigs.Add(line);
        }

        private static IEnumerable<string> CreateNewEmptySolution()
        {
            return $@"Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio 15
VisualStudioVersion = 15.0.28307.329
MinimumVisualStudioVersion = 10.0.40219.1
Global
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(ExtensibilityGlobals) = postSolution
		SolutionGuid = {{{Guid.NewGuid().ToString().ToUpper()}}}
	EndGlobalSection
EndGlobal
".Split(new[] {Environment.NewLine}, StringSplitOptions.None);
        }
    }
}
