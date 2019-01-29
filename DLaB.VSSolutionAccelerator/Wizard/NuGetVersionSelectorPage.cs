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

        public static NuGetVersionSelectorPage Create(string question, string packageId, string description = null)
        {
            var page = new NuGetVersionSelectorPage
            {
                QuestionLabel = {Text = question},
                DescriptionText = {Text = description}
            };

            page.PackageSelector.Items.AddRange(PackageLister.GetPackagesbyId(packageId).OrderByDescending(p => p.Version).Cast<object>().ToArray());
            return page;
        }
    }
}
