using System;
using System.Collections.Generic;
using System.Linq;

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

            PreProjects = PreProjects ?? new List<string>();
            Projects = Projects ?? new List<string>();
            GlobalSharedProjects = GlobalSharedProjects ?? new List<string>();
            GlobalSolutionConfigs = GlobalSolutionConfigs ?? new List<string>();
            GlobalProjectConfigs = GlobalProjectConfigs ?? new List<string>();
            PostGlobalProjectConfigs = PostGlobalProjectConfigs ?? new List<string>();

            if (GlobalSolutionConfigs.Count == 0)
            {
                GlobalSolutionConfigs = new List<string>
                {
                    "\t\tDebug|Any CPU = Debug|Any CPU",
                    "\t\tRelease|Any CPU = Release|Any CPU"
                };
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
                PreProjects.Add(line);
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

        internal IEnumerable<string> GetSolution()
        {
            IEnumerable<string> SplitByNewLine(IEnumerable<string> lines)
            {
                foreach (var line in lines)
                {
                    var parts = line.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
                    if (parts.Length > 1)
                    {
                        foreach (var part in parts)
                        {
                            yield return part;
                        }
                    }
                    else
                    {
                        yield return line;
                    }
                }
            }

            var blankCount = 0;
            foreach (var line in PreProjects)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    if (blankCount == 1)
                    {
                        continue;
                    }

                    blankCount++;
                }
                else
                {
                    blankCount++; // Don't check anymore
                }

                yield return line;
            }

            foreach (var line in SplitByNewLine(Projects))
            {
                yield return line;
            }

            yield return LineMarkers.GlobalStart;
            if (GlobalSharedProjects.Any())
            {
                yield return "\t" + LineMarkers.GlobalSharedProjectStart;
                foreach (var line in SplitByNewLine(GlobalSharedProjects))
                {
                    yield return line;
                }
                yield return "\t" + LineMarkers.GlobalSectionEnd;
            }

            if (GlobalSolutionConfigs.Any())
            {
                yield return "\t" + LineMarkers.GlobalSolutionConfigsStart;
                foreach (var line in SplitByNewLine(GlobalSolutionConfigs))
                {
                    yield return line;
                }
                yield return "\t" + LineMarkers.GlobalSectionEnd;
            }

            if (GlobalProjectConfigs.Any())
            {
                yield return "\t" + LineMarkers.GlobalProjectConfigsStart;
                foreach (var line in SplitByNewLine(GlobalProjectConfigs))
                {
                    yield return line;
                }
                yield return "\t" + LineMarkers.GlobalSectionEnd;
            }

            foreach (var line in SplitByNewLine(PostGlobalProjectConfigs))
            {
                yield return line;
            }
        }
    }
}
