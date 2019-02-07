using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using Source.DLaB.Common;

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
            public const string Plugin = "Xyz.Xrm.Plugin";
            public const string PluginTests = "Xyz.Xrm.Plugin.Tests";
            public const string TestCore = "Xyz.Xrm.TestCore";
            public const string Test = "Xyz.Xrm.Test";
            public const string Workflow = "Xyz.Xrm.Workflow";
            public const string WorkflowCommon = "Xyz.Xrm.WorkflowCore";
            public const string WorkflowTests = "Xyz.Xrm.Workflow.Tests";
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
        public List<ProcessExecutorInfo> PostUpdateCommands { get; set; }
        public List<string> PostUpdateCommandResults { get; private set; }
        public List<string> FilesToRemove { get; internal set; }

        public ProjectInfo()
        {
            Type = ProjectType.CsProj;
            AddToSolution = true;
            Files = new List<ProjectFile>();
            FilesToRemove = new List<string>();
            SharedProjectsReferences = new List<ProjectInfo>();
            PostUpdateCommands = new List<ProcessExecutorInfo>();
            PostUpdateCommandResults = new List<string>();
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

        public void CopyFromAndUpdate(string templateDirectory, string rootNamespace)
        {
            Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(Path.Combine(templateDirectory, Key), NewDirectory, true);
            DeleteFiles();
            RenameFiles();
            UpdateProject(rootNamespace);
            UpdateProjectFiles();
            UpdateCsNamespaces(rootNamespace);
            ExecutePostUpdateCommands();
        }

        private void DeleteFiles()
        {
            foreach (var file in FilesToRemove)
            {
                var path = Path.Combine(NewDirectory, file);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        private void UpdateProjectFiles()
        {
            foreach (var file in Files)
            {
                file.Update(NewDirectory);
            }
        }

        private void UpdateCsNamespaces(string rootNamespace)
        {
            foreach (var file in Directory.EnumerateFiles(NewDirectory, "*.cs", SearchOption.AllDirectories))
            {
                var newLines = File.ReadAllLines(file)
                    .Select(line => line.Contains("namespace Xyz.Xrm")
                        ? line.Replace("namespace Xyz.Xrm", "namespace " + rootNamespace)
                        : line).ToList();
                File.WriteAllLines(file, newLines);
            }
        }

        private void ExecutePostUpdateCommands()
        {

            foreach (var cmd in PostUpdateCommands)
            {
                PostUpdateCommandResults.Add(ProcessExecutor.ExecuteCmd(cmd));
            }
        }

        public void AddNugetPostUpdateCommands(NuGetMapper nuGetMapper, string templatePackagesPath, string packagesPath)
        {
            if (!File.Exists(templatePackagesPath))
            {
                return;
            }

            nuGetMapper.AddUpdateCommands(PostUpdateCommands, templatePackagesPath, packagesPath);
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

        public void UpdateProject(string rootNamespace)
        {
            if (Type != ProjectType.CsProj)
            {
                return;
            }

            //Perform updates
            var projectFilePath = Path.Combine(NewDirectory, $"{Name}.csproj");
            var parser = new ProjectFileParser(File.ReadAllLines(projectFilePath));
            var localNugetBuild = parser.PropertyGroups.FirstOrDefault(g => g.OpenTag.Contains("'LocalNuget|AnyCPU'"));
            parser.PropertyGroups.Remove(localNugetBuild);
            foreach (var group in parser.PropertyGroups.Where(g => g.Type == PropertyGroupType.CompileConfiguration))
            {
                group.Lines.RemoveAll(l => l.Contains("<CodeAnalysisRuleSet>"));
            }
            var keyGroup = parser.PropertyGroups.FirstOrDefault(g => g.Type == PropertyGroupType.KeyFile);
            if (keyGroup != null)
            {
                var openTag = keyGroup.Lines.First();
                var closeTag = keyGroup.Lines.Last();
                keyGroup.Lines.Clear();
                keyGroup.Lines.Add(openTag);
                keyGroup.Lines.Add($"    <AssemblyOriginatorKeyFile>{Name}.Key.snk</AssemblyOriginatorKeyFile>");
                keyGroup.Lines.Add(closeTag);
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
            if (parser.ItemGroups.TryGetValue(ProjectFileParser.ItemGroupTypes.Compile, out var compileGroup))
            {
                compileGroup.RemoveAll(l => FilesToRemove.Any(f => l.Contains($@"<Compile Include=""{f}"" />")));
            }
            File.WriteAllLines(projectFilePath, parser.GetProject());
        }

        private void RenameRootProjectFile(string oldName, string newName)
        {
            var newPath = Path.Combine(NewDirectory, newName);
            if (File.Exists(newPath))
            {
                File.Delete(newPath);
            }
            File.Move(Path.Combine(NewDirectory, oldName), newPath);
        }

        public void AddRegenKeyPostUpdateCommand(string strongNamePath)
        {
            PostUpdateCommands.Add(new ProcessExecutorInfo(strongNamePath, $"-k {Name}.Key.snk"));
        }
    }
}
