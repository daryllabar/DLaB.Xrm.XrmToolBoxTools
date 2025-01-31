﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DLaB.Log;
using Source.DLaB.Common;

namespace DLaB.VSSolutionAccelerator.Logic
{
    public class ProjectInfo
    {
        public enum ProjectType
        {
            CsProj,
            LegacyCsProj,
            SharedProj
        }

        public struct Keys
        {
            public const string Common = "Xyz.Xrm";
            public const string Plugin = "Xyz.Xrm.Plugin";
            public const string PluginTests = "Xyz.Xrm.Plugin.Tests";
            public const string Test = "Xyz.Xrm.Test";
            public const string Workflow = "Xyz.Xrm.Workflow";
            public const string WorkflowCommon = "Xyz.Xrm.WorkflowCore";
            public const string WorkflowTests = "Xyz.Xrm.Workflow.Tests";
        }

        public static Dictionary<string,string> IdByKey => new Dictionary<string, string>
        {
            { Keys.Plugin, "2B294DBF-8730-436E-B401-8745FEA632FE" },
            { Keys.PluginTests, "3016D729-1A3B-43C0-AC2F-D4EF6A305FA6" },
            { Keys.Workflow, "5BD39AC9-97F3-47C8-8E1F-6A58A24AFB9E" },
            { Keys.WorkflowCommon, "dd5aa002-c1ff-4c0e-b9a5-3d63c7809b07" },
            { Keys.WorkflowTests, "7056423A-373E-463D-B552-D2F305F5C041" },
            { Keys.Common, "b22b3bc6-0ac6-4cdd-a118-16e318818ad7" },
            { Keys.Test, "F62103E9-D25D-4F99-AABE-ECF348424366" },
        };

        public Guid Id { get; set; }
        public string Key { get; set; }
        public bool AddToSolution { get; set; }
        public string Name { get; set; }
        public ProjectType Type { get; set; }
        public List<ProjectFile> Files { get; set; }
        public List<ProjectInfo> SharedProjectsReferences { get; set; }
        public List<ProjectInfo> ProjectsReferences { get; set; }
        public string NewDirectory { get; set; }
        public string SolutionProjectHeader => string.Format("Project(\"{0}\") = \"{1}\", \"{1}\\{1}.{2}\", \"{{{3}}}\"{4}EndProject", GetTypeId(), Name, GetProjectPostfix(), Id.ToString().ToUpper(), Environment.NewLine );
        public List<ProcessExecutorInfo> PostUpdateCommands { get; set; }
        public List<ProcessExecutorInfo> PostSolutionRestoreCommands { get; set; }
        public List<string> PostUpdateCommandResults { get; private set; }
        public List<string> PostSolutionRestoreCommandResults { get; private set; }
        public List<string> FilesToRemove { get; internal set; }
        public bool HasDevDeployBuild { get; set; }

        public ProjectInfo()
        {
            Type = ProjectType.CsProj;
            AddToSolution = true;
            Files = new List<ProjectFile>();
            FilesToRemove = new List<string>();
            SharedProjectsReferences = new List<ProjectInfo>();
            ProjectsReferences = new List<ProjectInfo>();
            PostUpdateCommands = new List<ProcessExecutorInfo>();
            PostUpdateCommandResults = new List<string>();
            PostSolutionRestoreCommands = new List<ProcessExecutorInfo>();
            PostSolutionRestoreCommandResults = new List<string>();
        }

        internal static string GetTypeId(ProjectType type)
        {
            switch (type)
            {
                case ProjectType.CsProj:
                    return "{9A19103F-16F7-4668-BE54-9A1E7A4F7556}";

                case ProjectType.LegacyCsProj:
                    return "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";

                case ProjectType.SharedProj:
                    return "{D954291E-2A0B-460D-934E-DC6B0785DB48}";
            }

            throw new NotImplementedException(((int)type) + type.ToString());
        }

        private string GetTypeId()
        {
            return GetTypeId(Type);
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
                var localPlatform = platform.Trim();
                if (localPlatform.StartsWith("DevDeploy|") && !HasDevDeployBuild)
                {
                    lines.Add($"\t\t{{{Id.ToString()}}}.{localPlatform.Replace(" = ", ".ActiveCfg = ").Replace(" = DevDeploy", " = Debug")}");
                    continue;
                }
                lines.Add($"\t\t{{{Id.ToString()}}}.{localPlatform.Replace(" = ", ".ActiveCfg = ")}");
                lines.Add($"\t\t{{{Id.ToString()}}}.{localPlatform.Replace(" = ", ".Build.0 = ")}");
            }

            return string.Join(Environment.NewLine, lines);
        }

        public void CopyFromAndUpdate(string templateDirectory, string rootNamespace)
        {
            Logger.AddDetail($"Creating project '{Path.Combine(NewDirectory, Name)}'.");
            Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(Path.Combine(templateDirectory, Key), NewDirectory, true);
            DeleteFiles();
            RenameFiles();
            UpdateProject();
            UpdateProjectFiles();
            UpdateCsNamespaces(rootNamespace);
            ExecutePostUpdateCommands();
        }

        public void UpdatePostSolutionRestore()
        {
            ExecutePostSolutionRestoreCommands();
        }

        private void DeleteFiles()
        {
            foreach (var file in FilesToRemove)
            {
                if (file.EndsWith(@"\"))
                {
                    var directory = Path.Combine(NewDirectory, file.Substring(0, file.Length -1));
                    if (Directory.Exists(directory))
                    {
                        Logger.AddDetail($"Deleting unused directory '{directory}'.");
                        Directory.Delete(directory, true);
                    }
                }
                else
                {
                    var path = Path.Combine(NewDirectory, file);
                    if (File.Exists(path))
                    {
                        Logger.AddDetail($"Deleting unused file '{path}'.");
                        File.Delete(path);
                    }
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
                    .Select(line => line.Contains("namespace Xyz.Xrm") || line.Contains("using Xyz.Xrm")
                        ? line.Replace("namespace Xyz.Xrm", "namespace " + rootNamespace).Replace("using Xyz.Xrm", "using " + rootNamespace)
                        : line).ToList();
                File.WriteAllLines(file, newLines);
            }
        }

        private void ExecutePostUpdateCommands()
        {
            foreach (var cmd in PostUpdateCommands)
            {
                Logger.AddDetail($"Executing Command: \"{cmd.FileName}\" {cmd.Arguments}");
                var result = ProcessExecutor.ExecuteCmd(cmd);
                Logger.AddDetail(result);
                PostUpdateCommandResults.Add(result);
            }
        }

        private void ExecutePostSolutionRestoreCommands()
        {
            foreach (var cmd in PostSolutionRestoreCommands)
            {
                Logger.AddDetail($"Executing Command: \"{cmd.FileName}\" {cmd.Arguments}");
                var result = ProcessExecutor.ExecuteCmd(cmd);
                Logger.AddDetail(result);
                PostSolutionRestoreCommandResults.Add(result);
            }
        }

        //public void AddNugetPostUpdateCommands(NuGetMapper nuGetMapper, string nugetContentInstallerPath, string solutionDirectory)
        //{
        //    if (!File.Exists(nuGetMapper.SourcePackagesConfigPath))
        //    {
        //        return;
        //    }
        //
        //    nuGetMapper.AddUpdateCommands(PostUpdateCommands);
        //    var packages = File.ReadAllText(nuGetMapper.SourcePackagesConfigPath);
        //    if (packages.Contains("DLaB.Xrm.Source") || packages.Contains("DLaB.Xrm.Common.Source"))
        //    {
        //        PostSolutionRestoreCommands.Add(new ProcessExecutorInfo(nugetContentInstallerPath, $@"""{solutionDirectory}"" ""{Path.Combine(solutionDirectory, Name, Name + "." + GetProjectPostfix())}"""));
        //    }
        //}

        public void RenameFiles()
        {
            switch (Type)
            {
                case ProjectType.CsProj:
                    RenameRootProjectFile($"{Key}.csproj", $"{Name}.csproj");
                    RenameRootProjectFile($"{Key}.Key.snk", $"{Name}.Key.snk");

                    break;
                case ProjectType.SharedProj:
                    RenameRootProjectFile($"{Key}.projitems", $"{Name}.projitems");
                    RenameRootProjectFile($"{Key}.shproj", $"{Name}.shproj");
                    break;
                default:
                    break;
            }
        }

        public void UpdateProject()
        {
            if (Type != ProjectType.CsProj)
            {
                return;
            }

            //Perform updates
            var projectFilePath = Path.Combine(NewDirectory, $"{Name}.csproj");
            var parser = new ProjectFileParser(projectFilePath, File.ReadAllLines(projectFilePath));
            if (!string.IsNullOrWhiteSpace(parser.StrongNameKey))
            {
                parser.SetPropertyFromLine(nameof(parser.StrongNameKey), $"\t\t<AssemblyOriginatorKeyFile>{Name}.Key.snk</AssemblyOriginatorKeyFile>");
            }

            //if (parser.ItemGroups.TryGetValue(ProjectFileParser.ItemGroupTypes.ProjectReference, out var references))
            //{
            //    var newReferences = new List<string>();
            //    foreach (var reference in references)
            //    {
            //        var local = reference;
            //        foreach (var project in ProjectsReferences)
            //        {
            //            while (local.Contains(project.Key))
            //            {
            //                local = local.Replace(project.Key, project.Name);
            //            }
            //        }
            //        newReferences.Add(local);
            //    }
            //
            //    parser.ItemGroups[ProjectFileParser.ItemGroupTypes.ProjectReference] = newReferences;
            //}

            File.WriteAllLines(projectFilePath, parser.GetProject());
        }

        private void RenameRootProjectFile(string oldName, string newName)
        {
            var newPath = Path.Combine(NewDirectory, newName);
            var oldPath = Path.Combine(NewDirectory, oldName);
            if (File.Exists(newPath))
            {
                File.Delete(newPath);
            }
            if (File.Exists(oldPath))
            {
                File.Move(oldPath, newPath);
            }
        }

        public void AddRegenKeyPostUpdateCommand(string strongNamePath)
        {
            PostUpdateCommands.Add(new ProcessExecutorInfo(strongNamePath, $"-k {Name}.Key.snk"));
        }
    }
}
