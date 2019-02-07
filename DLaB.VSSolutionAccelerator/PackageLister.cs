using System;
using System.Collections.Generic;
using System.Linq;
using NuGet;

namespace DLaB.VSSolutionAccelerator
{
    /// <summary>
    /// Special thanks to Jason Lattimer and the D365 DeveloperExtensions from which this came.
    /// https://github.com/jlattimer/D365DeveloperExtensions
    /// </summary>
    public static class PackageLister
    {
        public struct Ids
        {
            public static string CoreXrmAssemblies = "Microsoft.CrmSdk.CoreAssemblies";
            public static string MicrosoftCrmSdkXrmToolingCoreAssembly = "Microsoft.CrmSdk.XrmTooling.CoreAssembly";
            public static string MicrosoftCrmSdkWorkflow = "Microsoft.CrmSdk.Workflow";
        }

        public static List<NuGetPackage> GetPackagesbyId(string packageId)
        {
            var packages = GetPackages(packageId);

            List<NuGetPackage> results = new List<NuGetPackage>();
            foreach (IPackage package in packages)
            {
                if (package.Published != null && package.Published.Value.Year == 1900)
                    continue;

                results.Add(CreateNuGetPackage(package));
            }

            return new List<NuGetPackage>(results.OrderByDescending(v => v.Version.ToString()));
        }

        private static List<IPackage> GetPackages(string packageId)
        {
            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");
            List<IPackage> packages = repo.FindPackagesById(packageId).ToList();

            return packages;
        }

        private static NuGetPackage CreateNuGetPackage(IPackage package)
        {
            return new NuGetPackage
            {
                Id = package.Id,
                Name = package.Title,
                Version = package.Version.Version,
                VersionText = package.Version.ToOriginalString(),
                XrmToolingClient = UsesXrmToolingClient(package),
                LicenseUrl = package.LicenseUrl != null
                    ? package.LicenseUrl.ToString()
                    : null
            };
        }

        private static bool UsesXrmToolingClient(IPackageMetadata package)
        {
            if (package.DependencySets?.Count() != 1)
                return false;

            foreach (PackageDependency dependency in package.DependencySets.First().Dependencies)
                if (dependency.Id == Ids.MicrosoftCrmSdkXrmToolingCoreAssembly)
                    return true;

            return false;
        }

        public static NuGetPackage GetNewest(this List<NuGetPackage> packages)
        {
            return packages.OrderByDescending(p => p.Version).FirstOrDefault();
        }

        public static NuGetPackage GetNewestForMajorVersion(this List<NuGetPackage> packages, int majorVersion)
        {
            return packages.OrderByDescending(p => p.Version).FirstOrDefault(p => p.Version.Major == majorVersion);
        }
    }

    public class NuGetPackage
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Version Version { get; set; }
        public string VersionText { get; set; }
        public bool XrmToolingClient { get; set; }
        public string LicenseUrl { get; set; }

        public override string ToString()
        {
            return $"{Name} {VersionText}";
        }
    }
}
