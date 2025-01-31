using System;
using System.Collections.Generic;
using System.Linq;

namespace DLaB.VSSolutionAccelerator.Logic
{
    public class SolutionFileParser
    {
        public List<string> PreProjects { get; } = new List<string>();
        public List<string> Projects { get; } = new List<string>();
        public List<string> GlobalSolutionConfigs{ get; } = new List<string>();
        public List<string> GlobalProjectConfigs{ get; } = new List<string>();
        public List<string> GlobalSolutionProperties { get; } = new List<string>();
        public List<string> GlobalExtensibilityGlobals { get; } = new List<string>();
        public List<string> GlobalSharedProjects{ get; } = new List<string>();
        public List<string> PostGlobalSharedProjects{ get; } = new List<string>();

        public enum States
        {
            PreProject,
            Project,
            Global,
            PostGlobalSharedProject,
        }

        public Dictionary<States, Action<string>> StateProcessor => new Dictionary<States, Action<string>>
        {
            {States.PreProject, ParsePreProject },
            {States.Project, ParseProject },
            {States.Global, ParseGlobal },
            {States.PostGlobalSharedProject, ParsePostGlobalSharedProjects },
        };

        public struct LineMarkers
        {
            public const string GlobalSolutionConfigsStart = "GlobalSection(SolutionConfigurationPlatforms) = preSolution";
            public const string GlobalProjectConfigsStart = "GlobalSection(ProjectConfigurationPlatforms) = postSolution";
            public const string GlobalSolutionPropertiesStart = "GlobalSection(SolutionProperties) = preSolution";
            public const string GlobalExtensibilityGlobalsStart = "GlobalSection(ExtensibilityGlobals) = postSolution";
            public const string GlobalSharedProjectsStart = "GlobalSection(SharedMSBuildProjectFiles) = preSolution";
            public const string GlobalSectionEnd = "EndGlobalSection";
            public const string GlobalStart = "Global";
            public const string GlobalEnd = "EndGlobal";
            public const string ProjectStart = "Project(\"{";
        }

        public States State { get; set; }
        private List<string> _currentGlobalSection;
        private Dictionary<string, List<string>> _globalSectionsByStartMarker;

        public SolutionFileParser(IEnumerable<string> solution)
        {
            // This will move any new global section to output below the known global sections, but at least it won't error
            _currentGlobalSection = PostGlobalSharedProjects;
            _globalSectionsByStartMarker = new Dictionary<string, List<string>>
            {
                { LineMarkers.GlobalSolutionConfigsStart, GlobalSolutionConfigs },
                { LineMarkers.GlobalProjectConfigsStart, GlobalProjectConfigs },
                { LineMarkers.GlobalSolutionPropertiesStart, GlobalSolutionProperties },
                { LineMarkers.GlobalExtensibilityGlobalsStart, GlobalExtensibilityGlobals },
                { LineMarkers.GlobalSharedProjectsStart, GlobalSharedProjects }
            };
            if (solution == null)
            {
                solution = CreateNewEmptySolution();
            }
            foreach (var line in solution)
            {
                StateProcessor[State](line);
            }

            if (GlobalSolutionConfigs.Count == 0)
            {
                GlobalSolutionConfigs = new List<string>
                {
                    "\t\tDebug|Any CPU = Debug|Any CPU",
                    "\t\tDevDeploy|Any CPU = DevDeploy|Any CPU",
                    "\t\tRelease|Any CPU = Release|Any CPU"
                };
            }
        }

        private void ParsePreProject(string line)
        {
            if (line.TrimStart().StartsWith(LineMarkers.ProjectStart))
            {
                State = States.Project;
                StateProcessor[State](line);
            }
            else if (line.Trim() == LineMarkers.GlobalStart)
            {
                State = States.Global;
            }
            else
            {
                PreProjects.Add(line);
            }
        }

        private void ParseProject(string line)
        {
            if (Projects.Count == 0)
            {
                if (line.TrimStart().StartsWith(LineMarkers.ProjectStart))
                {
                    Projects.Add(line);
                }
                else
                {
                    State = States.Global;
                    StateProcessor[State](line);
                }
            }
            else if (line.Trim() == LineMarkers.GlobalStart)
            {
                State = States.Global;
            }
            else
            {
                Projects.Add(line);
            }
        }

        private void ParseGlobal(string line)
        {   
            if (_globalSectionsByStartMarker.TryGetValue(line.Trim(), out var globalSection))
            {
                _currentGlobalSection = globalSection;
            }
            else if (line.Trim() == LineMarkers.GlobalSectionEnd)
            {
                _currentGlobalSection = PostGlobalSharedProjects;
            }
            else if (line.Trim() == LineMarkers.GlobalEnd)
            {
                State = States.PostGlobalSharedProject;
                PostGlobalSharedProjects.Add(line);
            }
            else
            {
                _currentGlobalSection.Add(line);
            }
        }

        private void ParsePostGlobalSharedProjects(string line)
        {
            PostGlobalSharedProjects.Add(line);
        }

        private static IEnumerable<string> CreateNewEmptySolution()
        {
            return $@"Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.10.35201.131
MinimumVisualStudioVersion = 10.0.40219.1
Global
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(ExtensibilityGlobals) = postSolution
		SolutionGuid = {Guid.NewGuid().ToString().ToUpper()}
	EndGlobalSection
EndGlobal
".Split(new[] {Environment.NewLine}, StringSplitOptions.None);
        }

        internal IEnumerable<string> GetSolution()
        {
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

            foreach(var globalOutput in new[]
            {
                new { Lines = GlobalSolutionConfigs, Start = LineMarkers.GlobalSolutionConfigsStart},
                new { Lines = GlobalProjectConfigs, Start = LineMarkers.GlobalProjectConfigsStart},
                new { Lines = GlobalSolutionProperties, Start = LineMarkers.GlobalSolutionPropertiesStart},
                new { Lines = GlobalExtensibilityGlobals, Start = LineMarkers.GlobalExtensibilityGlobalsStart},
                new { Lines = GlobalSharedProjects, Start = LineMarkers.GlobalSharedProjectsStart},
            })
            {
                if (!globalOutput.Lines.Any())
                {
                    continue;
                }

                yield return "\t" + globalOutput.Start;
                foreach (var line in SplitByNewLine(globalOutput.Lines))
                {
                    yield return line;
                }
                yield return "\t" + LineMarkers.GlobalSectionEnd;
            }

            foreach (var line in SplitByNewLine(PostGlobalSharedProjects))
            {
                yield return line;
            }

            yield break;

            IEnumerable<string> SplitByNewLine(IEnumerable<string> lines)
            {
                foreach (var line in lines)
                {
                    var parts = line.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
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
        }
    }
}
