using System.Windows.Forms;

namespace DLaB.VSSolutionAccelerator.Wizard
{
    public partial class NuGetVersionSelectorPage : IWizardPage
    {
        public UserControl Content => this;

        public bool PageValid => PackageSelector.SelectedIndex > -1;

        public string ValidationMessage => "Please enter or select a value!";

        public void Cancel()
        {
        }

        public object Save()
        {
            return (NuGetPackage) PackageSelector.SelectedItem;
        }

        void IWizardPage.Load(object[] saveResults)
        {
            // Called when the Page is loaded in the wizard.
            // Helpful if additional logic is required before loading
        }

        bool IWizardPage.IsRequired(object[] saveResults)
        {
            return true;
        }
    }
}
