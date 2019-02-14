using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DLaB.VSSolutionAccelerator.Wizard
{
    public partial class NuGetVersionSelectorPage : UserControl
    {
        public NuGetVersionSelectorPage()
        {
            InitializeComponent();
        }

        public static NuGetVersionSelectorPage Create(string question, string packageId, string description = null, bool onlyDisplayLatestMajorVersion = true)
        {
            var page = new NuGetVersionSelectorPage
            {
                QuestionLabel = {Text = question},
                DescriptionText = {Text = description}
            };


            var packages = PackageLister.GetPackagesbyId(packageId).OrderByDescending(p => p.Version).ToList();
            var validPackages = packages;
            if (onlyDisplayLatestMajorVersion)
            {
                validPackages = new List<NuGetPackage>();
                var majorVersions = new HashSet<int>(packages.Select(p => p.Version.Major).Distinct());
                foreach (var package in packages)
                {
                    if (majorVersions.Remove(package.Version.Major))
                    {
                        validPackages.Add(package);
                    }
                }
            }
            page.PackageSelector.Items.AddRange(validPackages.Cast<object>().ToArray());
            if (page.PackageSelector.Items.Count > 0)
            {
                page.PackageSelector.SelectedIndex = 0;
            }
            return page;
        }
    }
}
