using System;
using System.Collections.Generic;
using System.Linq;
using Source.DLaB.Common;

namespace DLaB.VSSolutionAccelerator.Logic
{
    public class ProjectFileParser
    {
        public Guid Id { get; private set; }
        public string Namespace { get; private set; }
        public string AssemblyName { get; private set; }
        private string CurrentOpenItemGroup { get; set; }
        public List<List<string>> Chooses { get; set; }
        public List<string> PrePropertyGroups { get; set; }
        public List<PropertyGroup> PropertyGroups { get; set; }
        public Dictionary<string, List<string>> ItemGroups { get; set; }
        public List<string> Imports { get; set; }
        public List<string> PostImports { get; set; }


        public enum States
        {
            PrePropertyGroup,
            PropertyGroup,
            ItemGroup,
            Choose,
            Imports,
            PostItemGroup,
        }

        public Dictionary<States, Action<string>> StateProcessor => new Dictionary<States, Action<string>>
        {
            {States.PrePropertyGroup, ParsePrePropertyGroup},
            {States.PropertyGroup, ParsePropertyGroup},
            {States.ItemGroup, ParseItemGroup},
            {States.Choose, ParseChoose},
            {States.PostItemGroup, ParsePostItemGroup},
        };

        public struct LineMarkers
        {
            public const string ChooseEnd = "</Choose>";
            public const string ChooseStart = "<Choose>";
            public const string ItemGroupEnd = "</ItemGroup>";
            public const string ItemGroupStart = "<ItemGroup>";
            public const string ImportStart = "<Import Project";
            public const string PropertyGroupEnd = "</PropertyGroup>";
            public const string PropertyGroupStart = "<PropertyGroup";
        }

        public struct ItemGroupTypes
        {
            public const string Analyzer = "Analyzer";
            public const string Compile = "Compile";
            public const string Content = "Content";
            public const string ProjectReference = "ProjectReference";
            public const string None = "None";
            public const string Reference = "Reference";
        }

        public States State { get; set; }

        public ProjectFileParser(IEnumerable<string> project)
        {
            if (project == null)
            {
                project = CreateNewEmptyProject();
            }

            State = States.PrePropertyGroup;
            PrePropertyGroups = new List<string>();
            Imports = new List<string>();
            PropertyGroups = new List<PropertyGroup>();
            ItemGroups = new Dictionary<string, List<string>>();
            PostImports = new List<string>();
            Chooses = new List<List<string>>();

            foreach (var line in project)
            {
                StateProcessor[State](line);
            }
        }

        private void ParsePrePropertyGroup(string line)
        {
            if (!IsStartOfState(line))
            {
                PrePropertyGroups.Add(line);
            }
        }

        private void ParsePropertyGroup(string line)
        {
            if (IsStartOfState(line)) { return; }

            PropertyGroups.Last().AddLine(line);
            if (line.Contains("<RootNamespace>"))
            {
                Namespace = line.SubstringByString("<RootNamespace>", "</RootNamespace>");
            }
            else if (line.Contains("<AssemblyName>"))
            {
                AssemblyName = line.SubstringByString("<AssemblyName>", "</AssemblyName>");
            }else if (line.Contains("<ProjectGuid>"))
            {
                Id = new Guid(line.SubstringByString("<ProjectGuid>", "</ProjectGuid>"));
            }
        }

        private void ParseItemGroup(string line)
        {
            if (IsStartOfState(line) 
                || line.TrimStart().StartsWith(LineMarkers.ItemGroupEnd))
            {
                return;
            }

            var itemGroupType = GetCurrentOrNewItemGroup(line);
            if (!ItemGroups.TryGetValue(itemGroupType, out var lines))
            {
                lines = new List<string>();
                ItemGroups[itemGroupType] = lines;
                
            }

            lines.Add(line);
        }

        private void ParseChoose(string line)
        {
            if (line.TrimStart().StartsWith(LineMarkers.ChooseEnd))
            {
                State = States.ItemGroup;
            }

            Chooses.Last().Add(line);
        }

        private string GetCurrentOrNewItemGroup(string line)
        {
            var itemGroupType = CurrentOpenItemGroup ?? line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).First();
            if (CurrentOpenItemGroup != null)
            {
                if (line.TrimStart().StartsWith(CurrentOpenItemGroup.Replace("<", "</") + ">"))
                {
                    CurrentOpenItemGroup = null;
                }
            }
            else if(!line.TrimEnd().EndsWith("/>"))
            {
                CurrentOpenItemGroup = itemGroupType;
            }

            if (itemGroupType.StartsWith("<"))
            {
                itemGroupType = itemGroupType.Substring(1, itemGroupType.Length - 1);
            }

            return itemGroupType;
        }

        private void ParsePostItemGroup(string line)
        {
            if (!IsImportPostItemGroupStart(line))
            {
                PostImports.Add(line);
            }
        }

        private bool IsStartOfState(string line)
        {
            return IsPropertyGroupStart(line)
                || IsItemGroupStart(line)
                || IsChooseStart(line)
                || IsImportPostItemGroupStart(line);
        }

        private bool IsPropertyGroupStart(string line)
        {
            var isStart = line.TrimStart().StartsWith(LineMarkers.PropertyGroupStart);
            if (isStart)
            {
                PropertyGroups.Add(new PropertyGroup(line));
                State = States.PropertyGroup;
            }

            return isStart;
        }

        private bool IsItemGroupStart(string line)
        {
            var isStart = line.TrimStart().StartsWith(LineMarkers.ItemGroupStart);
            if (isStart)
            {
                State = States.ItemGroup;
            }
            
            return isStart;
        }

        private bool IsChooseStart(string line)
        {
            var isStart = line.TrimStart().StartsWith(LineMarkers.ChooseStart);
            if (isStart)
            {
                State = States.Choose;
                if (!(Chooses.LastOrDefault()?.Count > 0))
                {
                    Chooses.Add(new List<string> { line });
                }
            }

            return isStart;
        }

        private bool IsImportPostItemGroupStart(string line)
        {
            var isStart = line.TrimStart().StartsWith(LineMarkers.ImportStart)
                && (PropertyGroups.Count > 0 || ItemGroups.Count > 0);
            if (isStart)
            {
                State = States.PostItemGroup;
                Imports.Add(line);
            }

            return isStart;
        }

        private static IEnumerable<string> CreateNewEmptyProject()
        {
            return $@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""15.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <ProjectGuid>{Guid.NewGuid().ToString().ToUpper()}</ProjectGuid>
    <OutputType>Library</OutputType>
    <AppDesignerFolder>Properties</AppDesignerFolder>
    <RootNamespace>Empty</RootNamespace>
    <AssemblyName>Empty</AssemblyName>
    <TargetFrameworkVersion>v4.6.2</TargetFrameworkVersion>
    <FileAlignment>512</FileAlignment>
    <Deterministic>true</Deterministic>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>bin\Debug\</OutputPath>
    <DefineConstants>DEBUG;TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>bin\Release\</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include=""System"" />
    <Reference Include=""System.Core"" />
    <Reference Include=""System.Xml.Linq"" />
    <Reference Include=""System.Data.DataSetExtensions"" />
    <Reference Include=""Microsoft.CSharp"" />
    <Reference Include=""System.Data"" />
    <Reference Include=""System.Net.Http"" />
    <Reference Include=""System.Xml"" />
  </ItemGroup>
  <ItemGroup>
    <Compile Include=""Properties\AssemblyInfo.cs"" />
  </ItemGroup>
  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
</Project>
".Split(new[] {Environment.NewLine}, StringSplitOptions.None);
        }

        public IEnumerable<string> GetProject()
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

            foreach (var line in PrePropertyGroups)
            {
                yield return line;
            }

            foreach (var line in SplitByNewLine(PropertyGroups.SelectMany(g => g.Lines)))
            {
                yield return line;
            }

            foreach (var itemGroup in ItemGroups)
            {
                yield return "  " + LineMarkers.ItemGroupStart;
                foreach (var line in SplitByNewLine(itemGroup.Value))
                {
                    yield return line;
                }
                yield return "  " + LineMarkers.ItemGroupEnd;
            }

            foreach (var choose in Chooses)
            {
                foreach (var line in choose)
                {
                    yield return line;
                }
            }

            foreach (var line in SplitByNewLine(Imports))
            {
                yield return line;
            }

            foreach (var line in SplitByNewLine(PostImports))
            {
                yield return line;
            }
        }
    }  
}