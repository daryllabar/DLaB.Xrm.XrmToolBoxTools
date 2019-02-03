using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;

namespace DLaB.VSSolutionAccelerator.Logic
{
    public class ProjectInfo
    {
        public enum ProjectType
        {
            CsProj,
            SharedProj
        }
        public struct Keys
        {
            public const string Common = "Xyz.Xrm";
            public const string WorkflowCommon = "Xyz.Xrm.Workflow";
            public const string TestCore = "Xyz.Xrm.TestCore";
            public const string Test = "Xyz.Xrm.Test";
        }

        public Guid Id { get; set; }
        public string Key { get; set; }
        public bool AddToSolution { get; set; }
        public string Name { get; set; }
        public ProjectType Type { get; set; }
        public List<ProjectFile> Files { get; set; }
        public List<ProjectInfo> SharedProjectsReferences { get; set; }
        public string NewDirectory { get; set; }
        public string SolutionProjectHeader => string.Format("Project(\"{0}\") = \"{1}\", \"{1}\\{1}.{2}\", \"{{{3}}}\"{4}EndProject", GetTypeId(), Name, GetProjectPostfix(), Id.ToString().ToUpper(), Environment.NewLine );

        public ProjectInfo()
        {
            Type = ProjectType.CsProj;
            AddToSolution = true;
            Files = new List<ProjectFile>();
            SharedProjectsReferences = new List<ProjectInfo>();
        }

        private string GetTypeId()
        {
            switch (Type)
            {
                case ProjectType.CsProj:
                    return "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";

                case ProjectType.SharedProj:
                    return "{D954291E-2A0B-460D-934E-DC6B0785DB48}";
            }

            throw new NotImplementedException(((int)Type) + Type.ToString());
        }

        private string GetProjectPostfix()
        {
            switch (Type)
            {
                case ProjectType.CsProj:
                    return "csproj";

                case ProjectType.SharedProj:
                    return "shproj";
            }

            throw new NotImplementedException(((int)Type) + Type.ToString());
        }

        public string GetSharedMsBuildProjectFiles()
        {
            switch (Type)
            {
                case ProjectType.CsProj:
                    var lines = SharedProjectsReferences.OrderBy(p => p.Name)
                        .Select(p => $"\t\t{p.Name}\\{p.Name}.projitems*{{{Id.ToString()}}}*SharedItemsImports = 4")
                        .ToList();

                    if (lines.Any())
                    {
                        return string.Join(Environment.NewLine, lines);
                    }

                    return null;
                case ProjectType.SharedProj:
                    return $"\t\t{Name}\\{Name}.projitems*{{{Id.ToString()}}}*SharedItemsImports = 13";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public string GetProjectConfigurationPlatforms(IEnumerable<string> solutionConfigurationPlatforms)
        {
            if (Type == ProjectType.SharedProj)
            {
                return null;
            }

            var lines = new List<string>();
            foreach (var platform in solutionConfigurationPlatforms)
            {
                lines.Add($"\t\t{{{Id.ToString()}}}.{platform.Trim().Replace(" = ", ".ActiveCfg = ")}");
                lines.Add($"\t\t{{{Id.ToString()}}}.{platform.Trim().Replace(" = ", ".Build.0 = ")}");
            }

            return string.Join(Environment.NewLine, lines);
        }

        public void CopyFromAndUpdate(string templateDirectory, string rootNamespace, Version xrmVersion)
        {

            Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(Path.Combine(templateDirectory, Key), NewDirectory);
            RenameFiles();
            if (Type == ProjectType.CsProj)
            {
                UpdateProject(rootNamespace, xrmVersion.Major);
            }
            foreach (var file in Files)
            {
                file.Update(NewDirectory);
            }

            foreach (var file in Directory.EnumerateFiles(NewDirectory, "*.cs", SearchOption.AllDirectories))
            {
                var newLines = File.ReadAllLines(file)
                    .Select(line => line.Contains("namespace Xyz.Xrm")
                        ? line.Replace("namespace Xyz.Xrm", "namespace " + rootNamespace)
                        : line).ToList();
                File.WriteAllLines(file, newLines);
            }
        }

        public void RenameFiles()
        {
            switch (Type)
            {
                case ProjectType.CsProj:
                    RenameRootProjectFile($"{Key}.csproj", $"{Name}.csproj");

                    break;
                case ProjectType.SharedProj:
                    RenameRootProjectFile($"{Key}.projitems", $"{Name}.projitems");
                    RenameRootProjectFile($"{Key}.shproj", $"{Name}.shproj");
                    break;
                default:
                    break;
            }
        }

        public void UpdateProject(string rootNamespace, int majorXrmVersion)
        {
            //Perform updates
            var parser = new ProjectFileParser(File.ReadAllLines(Path.Combine(NewDirectory, $"{Name}.csproj")));
            parser.UpdateCompileConfiguration(Id, parser.Namespace.Replace("Xyz.Xrm", rootNamespace), Name, majorXrmVersion >= 9 ? "4.6.2" : "4.5.2");
            var localNugetBuild = parser.PropertyGroups.FirstOrDefault(g => g.OpenTag.Contains("'LocalNuget|AnyCPU'"));
            parser.PropertyGroups.Remove(localNugetBuild);
            foreach (var group in parser.PropertyGroups.Where(g => g.Type == PropertyGroupType.CompileConfiguration))
            {
                group.Lines.RemoveAll(l => l.Contains("<CodeAnalysisRuleSet>"));
            }
            var keyGroup = parser.PropertyGroups.FirstOrDefault(g => g.Type == PropertyGroupType.KeyFile);
            if (keyGroup != null)
            {
                keyGroup.Lines.Clear();
                keyGroup.Lines.Add($"<AssemblyOriginatorKeyFile>{Name}.Key.snk</AssemblyOriginatorKeyFile>");
            }
            // ItemGroup references to the SDK might need to be updated...
            //   <Reference Include="Microsoft.Crm.Sdk.Proxy, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL">
            //     <HintPath>..\packages\Microsoft.CrmSdk.CoreAssemblies.9.0.2.4\lib\net452\Microsoft.Crm.Sdk.Proxy.dll</HintPath>
            //   </Reference>
            if (parser.ItemGroups.ContainsKey(ProjectFileParser.ItemGroupTypes.Analyzer))
            {
                parser.ItemGroups.Remove(ProjectFileParser.ItemGroupTypes.Analyzer);
            }
            if (parser.ItemGroups.ContainsKey(ProjectFileParser.ItemGroupTypes.Content))
            {
                parser.ItemGroups.Remove(ProjectFileParser.ItemGroupTypes.Content);
            }
        }

        private void RenameRootProjectFile(string oldName, string newName)
        {
            File.Move(Path.Combine(NewDirectory, oldName), Path.Combine(NewDirectory, newName));
        }
    }
}
