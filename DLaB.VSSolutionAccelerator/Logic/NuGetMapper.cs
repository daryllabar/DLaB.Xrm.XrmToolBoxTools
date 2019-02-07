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

        public string NuGetPath { get; set; }
        public Version XrmVersion { get; set; }

        public NuGetMapper(string nuGetPath, Version xrmVersion)
        {
            NuGetPath = nuGetPath;
            XrmVersion = xrmVersion;
        }

        public void AddUpdateCommands(List<ProcessExecutorInfo> commands, string templatePackagesPath, string packagesPath, string[] packages = null)
        {
            packages = packages ?? File.ReadAllLines(templatePackagesPath);
            var count = commands.Count;
            AddUpdateCommandForXrmUnitTest(commands, packagesPath, packages);
            AddUpdateCommandForXrmPackages(commands, packagesPath, packages, PackageLister.Ids.MicrosoftCrmSdkWorkflow, PackageLister.Ids.CoreXrmAssemblies);
            if (count != commands.Count)
            {
                commands.Insert(count, new ProcessExecutorInfo(NuGetPath, $"restore \"{packagesPath}\" -NonInteractive -SolutionDirectory \"{Directory.GetParent(packagesPath).Parent?.FullName}\""));
            }
        }

        private void AddUpdateCommandForXrmUnitTest(List<ProcessExecutorInfo> commands, string packagesPath, string[] packages)
        {
            var line = packages.FirstOrDefault(p => p.Contains("XrmUnitTest"));
            if (line == null)
            {
                return;
            }
            var packageId = line.SubstringByString("id=\"", "\"");
            var version = new Version(line.SubstringByString("version=\"", "\""));
            NuGetPackage newest = null;

            if (packageId == GetXrmUnitTestId())
            {
                // CheckForNewerVersion
                newest = PackageLister.GetPackagesbyId(packageId).GetNewest();
                if (newest.Version == version)
                {
                    // Latest Version of XrmUnitTest is in use, no need to update
                    return;
                }
            }
            newest = newest ?? PackageLister.GetPackagesbyId(GetXrmUnitTestId()).GetNewest();
            commands.Add(new ProcessExecutorInfo(NuGetPath, $"update \"{packagesPath}\" -NonInteractive -Id {newest.Id} -Version {newest.Version}"));
        }

        private void AddUpdateCommandForXrmPackages(List<ProcessExecutorInfo> commands, string packagesPath, string[] packages, params string[] ids)
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
                    AddUpdateCommandForPackage(commands, packagesPath, id);
                }
            }
        }

        private void AddUpdateCommandForPackage(List<ProcessExecutorInfo> commands, string packagesPath, string id)
        {
            var newest = PackageLister.GetPackagesbyId(id).GetNewestForMajorVersion(XrmVersion.Major);
            commands.Add(new ProcessExecutorInfo(NuGetPath, $"update \"{packagesPath}\" -NonInteractive -Id {newest.Id} -Version {newest.Version}"));
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
