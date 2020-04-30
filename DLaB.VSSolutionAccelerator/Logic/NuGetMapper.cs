using Source.DLaB.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DLaB.VSSolutionAccelerator.Logic
{
    public class NuGetMapper
    {
        private static readonly Dictionary<int, string> XrmUnitTestMap = new Dictionary<int, string>
        {
            { 9, "09" },
            { 8, "2016" },
            { 7, "2015" },
            { 6, "2013" },
            { 5, "2011" }
        };

        public NuGetSettings NuGetSettings { get; }
        public Version XrmVersion { get; }
        public string SourcePackagesConfigPath { get; }
        public string DestinationPackagesConfigPath { get; }

        public NuGetMapper(NuGetSettings nuGetSettings, Version xrmVersion, string sourcePackagesConfigPath, string destinationPackagesConfigPath)
        {
            NuGetSettings = nuGetSettings;
            XrmVersion = xrmVersion;
            SourcePackagesConfigPath = sourcePackagesConfigPath;
            DestinationPackagesConfigPath = destinationPackagesConfigPath;
        }

        public void AddUpdateCommands(List<ProcessExecutorInfo> commands)
        {
            var packages = File.ReadAllLines(SourcePackagesConfigPath);
            var count = commands.Count;
            AddUpdateCommandForXrmUnitTest(commands, packages);
            AddUpdateCommandForXrmPackages(commands, packages, PackageLister.Ids.MicrosoftCrmSdkWorkflow, PackageLister.Ids.CoreXrmAssemblies);
            if (count != commands.Count)
            {
                commands.Insert(count, new ProcessExecutorInfo(NuGetSettings.ExePath, GetRestoreCommand()));
            }
        }

        private void AddUpdateCommandForXrmUnitTest(List<ProcessExecutorInfo> commands, string[] packages)
        {
            var line = packages.FirstOrDefault(p => p.Contains("XrmUnitTest"));
            if (line == null)
            {
                return;
            }
            var packageId = line.SubstringByString("id=\"", "\"");
            var version = new Version(line.SubstringByString("version=\"", "\""));
            var newest = PackageLister.GetPackagesbyId(GetXrmUnitTestId()).GetNewest();

            if (packageId == GetXrmUnitTestId() && newest.Version == version)
            {
                // Latest Version of XrmUnitTest is in use, no need to update
                return;
            }

            AddUpdateCommandForPackage(commands, newest);
        }

        private void AddUpdateCommandForXrmPackages(List<ProcessExecutorInfo> commands, string[] packages, params string[] ids)
        {
            foreach (var id in ids)
            {
                var line = packages.FirstOrDefault(p => p.Contains($"id=\"{id}\""));
                if (line == null)
                {
                    continue;
                }
                var version = new Version(line.SubstringByString("version=\"", "\""));
                if (version.Major != XrmVersion.Major)
                {
                    var newestForMajorRevision = PackageLister.GetPackagesbyId(id).GetNewestForMajorVersion(XrmVersion.Major);
                    AddUpdateCommandForPackage(commands, newestForMajorRevision);
                }
            }
        }

        private void AddUpdateCommandForPackage(List<ProcessExecutorInfo> commands, NuGetPackage package)
        {
            commands.Add(new ProcessExecutorInfo(NuGetSettings.ExePath, GetUpdateCommand(package)));
        }

        private string GetUpdateCommand(NuGetPackage package)
        {
            return $"update \"{DestinationPackagesConfigPath}\" {GetNuGetArguments()}-Id {package.Id} -Version {package.Version}";
        }

        private string GetRestoreCommand()
        {
            return $"restore \"{DestinationPackagesConfigPath}\" {GetNuGetArguments()}-SolutionDirectory \"{Directory.GetParent(DestinationPackagesConfigPath).Parent?.FullName}\"";
        }

        private string GetNuGetArguments()
        {
            var sources = NuGetSettings.Sources.Count == 0
                ? string.Empty
                : string.Join(string.Empty, NuGetSettings.Sources.Select(s => "-Source \"" + s + "\" "));
            return "-NonInteractive " + sources;
        }

        private string GetXrmUnitTestId()
        {
            if (XrmUnitTestMap.TryGetValue(XrmVersion.Major, out var postFix))
            {
                return "XrmUnitTest." + postFix;
            }

            return "XrmUnitTest." + XrmVersion.Major;
        }
    }
}
