using System;
using System.Collections.Generic;
using System.Linq;
using Source.DLaB.Common;

namespace DLaB.VSSolutionAccelerator.Logic
{
    public class ProjectFileParser
    {
        public enum States
        {
            Root,
            PropertyGroup,
            ItemGroup,
            Generic,
            Target
        }

        private bool _isNewProjectSdkFormat;
        public string AssemblyDescription { get; private set; }
        public string Authors { get; private set; }
        public string Company { get; private set; }
        public string Configurations { get; private set; }
        private FileGroup CurrentGroup => Groups.Last();
        public bool GenerateAssemblyInfo { get; private set; }
        public List<FileGroup> Groups { get; set; }
        public string Namespace { get; private set; }
        public string Path { get; set; }
        public string PackageName { get; private set; }
        public Dictionary<string, string> PackageReferences { get; } = new Dictionary<string, string>();
        public List<string> ProjectReferences { get; } = new List<string>();
        public List<string> SharedProjectReferences { get; } = new List<string>();
        public string ProjectTypeGuids { get; private set; }
        public bool SignAssembly { get; private set; }
        public string StrongNameKey { get; private set; }
        /// <summary>
        /// Contains a reference to the line that was used to populate a property, so that it can be updated.
        /// This is only used for setting the assembly key, not sure if it is worth maintaining.
        /// </summary>
        private Dictionary<string, ProjectFileLineReference> ProjectFileLineReferencesByProperty { get; set; } = new Dictionary<string, ProjectFileLineReference>();

        public Dictionary<States, Action<string>> StateProcessor => new Dictionary<States, Action<string>>
        {
            {States.Root, ParseRoot},
            {States.PropertyGroup, ParsePropertyGroup},
            {States.ItemGroup, ParseItemGroup},
            {States.Target, ParseTarget},
            {States.Generic, ParseGeneric},
        };

        public struct LineMarkers
        {
            public const string Import = "<Import Project"; // Shared Project
            public const string ItemGroupEnd = "</ItemGroup>";
            public const string ItemGroupStart = "<ItemGroup";
            public const string PropertyGroupEnd = "</PropertyGroup>";
            public const string PropertyGroupStart = "<PropertyGroup";
            public const string TargetGroupStart = "<Target";
            public const string TargetGroupEnd = "</Target>";

        }

        public struct ItemGroupTypes
        {
            public const string Analyzer = "Analyzer";
            public const string Content = "Content";
            public const string ProjectReference = "ProjectReference";
            public const string None = "None";
            public const string Reference = "Reference";
        }

        public States State { get; set; }

        public ProjectFileParser(string path, IEnumerable<string> project)
        {
            Path = path;
            if (project == null)
            {
                project = CreateNewEmptyProject();
            }

            State = States.Root;
            Groups = new List<FileGroup>
            {
                new FileGroup(GroupType.Root)
            };

            foreach (var line in project)
            {
                StateProcessor[State](line);
            }
        }

        private void ParseRoot(string line)
        {
            if (IsStartOfState(line))
            {
                if (!_isNewProjectSdkFormat)
                {
                    _isNewProjectSdkFormat = Groups.First().Lines.Any(l => l.Contains("<Project Sdk=\"Microsoft.NET.Sdk\">"));
                    if (!_isNewProjectSdkFormat)
                    {
                        throw new Exception($"The Project {Path} is not in the new SDK Style format!");
                    }
                }

                return;
            }

            if(CurrentGroup.GroupType != GroupType.Root)
            {
                Groups.Add(new FileGroup(GroupType.Root));
            }
            CurrentGroup.AddLine(line);
        }

        private void ParsePropertyGroup(string line)
        {
            CurrentGroup.AddLine(line);

            if (line.TrimStart().StartsWith(LineMarkers.PropertyGroupEnd))
            {
                State = States.Root;
                return;
            }

            SetPropertyFromLine(CurrentGroup, line);
        }
        public void SetPropertyFromLine(string propertyName, string line, string FutureEnumUsedToDetermineWhereToPlaceBrandNewValues = null)
        {
            SetPropertyFromLine(null, line);
        }

        private void SetPropertyFromLine(FileGroup group, string line)
        {
            SetPropertyFromLine(group, nameof(AssemblyDescription), "<Description>", "</Description>", line);
            SetPropertyFromLine(group, nameof(Authors), "<Authors>", "</Authors>", line);
            SetPropertyFromLine(group, nameof(Company), "<Company>", "</Company>", line);
            SetPropertyFromLine(group, nameof(Configurations), "<Configurations>", "</Configurations>", line);
            SetPropertyFromLine(group, nameof(GenerateAssemblyInfo), "<GenerateAssemblyInfo>", "</GenerateAssemblyInfo>", line);
            SetPropertyFromLine(group, nameof(Namespace), "<RootNamespace>", "</RootNamespace>", line);
            SetPropertyFromLine(group, nameof(PackageName), "<PackageId>", "</PackageId>", line);
            SetPropertyFromLine(group, nameof(ProjectTypeGuids), "<ProjectTypeGuids>", "</ProjectTypeGuids>", line);
            SetPropertyFromLine(group, nameof(SignAssembly), "<SignAssembly>", "</SignAssembly>", line);
            SetPropertyFromLine(group, nameof(StrongNameKey), "<AssemblyOriginatorKeyFile>", "</AssemblyOriginatorKeyFile>", line);
        }

        private void SetPropertyFromLine(FileGroup group, string propertyName, string projectStartString, string projectEndString, string line)
        {
            if (!line.Contains(projectStartString))
            {
                return;
            }

            if (group == null)
            {
                // TODO: External callers that set a value must define the File Group that the value should be in if it does not already exist.
                ProjectFileLineReferencesByProperty[propertyName].Line = line;
            }
            else
            {
                ProjectFileLineReferencesByProperty[propertyName] = new ProjectFileLineReference(group);
            }
            SetPropertyValue(propertyName, projectStartString, projectEndString, line);
        }

        private void SetPropertyValue(string propertyName, string projectStartString, string projectEndString, string line)
        {
            var property = GetType().GetProperty(propertyName);
            if (property == null)
            {
                throw new Exception($"Property with name {propertyName} was not found!");
            }

            if (property.PropertyType == typeof(bool))
            {
                property.SetValue(this, ParseBool(line.SubstringByString(projectStartString, projectEndString)));
                return;
            }

            property.SetValue(this, line.SubstringByString(projectStartString, projectEndString));
        }

        private bool ParseBool(string value)
        {
            return value?.ToLower().Trim() == "true";
        }

        private void ParseItemGroup(string line)
        {
            CurrentGroup.AddLine(line);

            if (line.TrimStart().StartsWith(LineMarkers.ItemGroupEnd))
            {
                State = States.Root;
                return;
            }

            if (line.TrimStart().StartsWith("<ProjectReference Include"))
            {
                ProjectReferences.Add(line.SubstringByString("\"", "\""));
            }
            if (line.TrimStart().StartsWith("<PackageReference Include"))
            {
                PackageReferences.Add(line.SubstringByString("Include=\"", "\""), line.SubstringByString("Version=\"", "\""));
            }
        }

        private void ParseTarget(string line)
        {
            CurrentGroup.AddLine(line);

            if (line.TrimStart().StartsWith(LineMarkers.TargetGroupEnd))
            {
                State = States.Root;
            }
        }

        private void ParseGeneric(string line)
        {
            if (IsStartOfState(line))
            {
                return;
            }

            if (CurrentGroup.GroupType != GroupType.Generic)
            {
                Groups.Add(new FileGroup(GroupType.Generic));
            }
            CurrentGroup.AddLine(line);

            if (line.TrimStart().StartsWith(LineMarkers.Import))
            {
                SharedProjectReferences.Add(line.SubstringByString("\"", "\""));
            }
        }

        private bool IsStartOfState(string line)
        {
            if (line.Contains("/>"))
            {
                // empty element treat as root
                return false;
            }
            return IsPropertyGroupStart(line)
                || IsItemGroupStart(line)
                || IsTargetStart(line);
        }

        private bool IsPropertyGroupStart(string line)
        {
            var isStart = line.TrimStart().StartsWith(LineMarkers.PropertyGroupStart);
            if (isStart)
            {
                Groups.Add(new PropertyGroup(line));
                State = States.PropertyGroup;
            }

            return isStart;
        }

        private bool IsItemGroupStart(string line)
        {
            var isStart = line.TrimStart().StartsWith(LineMarkers.ItemGroupStart);
            if (isStart)
            {
                Groups.Add(new FileGroup(GroupType.ItemGroup, line));
                State = States.ItemGroup;
            }
            
            return isStart;
        }

        private bool IsTargetStart(string line)
        {
            var isStart = line.TrimStart().StartsWith(LineMarkers.TargetGroupStart);
            if (isStart)
            {
                Groups.Add(new FileGroup(GroupType.Target, line));
                State = States.Target;
            }

            return isStart;
        }

        private static IEnumerable<string> CreateNewEmptyProject()
        {
            return @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
	  <AppendTargetFrameworkToOutputPath>false</AppendTargetFrameworkToOutputPath>
	  <Configurations>Debug;Release;DevDeploy</Configurations>
	  <LangVersion>12</LangVersion>
	  <OutputType>Library</OutputType>
	  <SignAssembly>false</SignAssembly>
	  <TargetFramework>net462</TargetFramework>
  </PropertyGroup>
</Project>
".Split(new[] {Environment.NewLine}, StringSplitOptions.None);
        }

        public IEnumerable<string> GetProject()
        {
            foreach (var group in Groups)
            {
                foreach(var line in SplitByNewLine(group.Lines))
                {
                    yield return line;
                }
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