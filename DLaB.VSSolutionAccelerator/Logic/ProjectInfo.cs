using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        public void CopyFromAndUpdate(string templateDirectory, string rootNamespace)
        {
            Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(Path.Combine(templateDirectory, Key), NewDirectory);
            RenameFiles();
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

        private void RenameRootProjectFile(string oldName, string newName)
        {
            File.Move(Path.Combine(NewDirectory, oldName), Path.Combine(NewDirectory, newName));
        }
    }
}
