using System.Windows.Forms;

namespace DLaB.VSSolutionAccelerator.Wizard
{
    public interface IWizardPage
    {
        UserControl Content { get; }
        void Load();
        object Save();
        void Cancel();
        bool IsBusy { get; }

        bool PageValid { get; }
        string ValidationMessage { get; }
    }
}