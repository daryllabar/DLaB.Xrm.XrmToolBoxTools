using System.Web.UI;
using UserControl = System.Windows.Forms.UserControl;

namespace DLaB.VSSolutionAccelerator.Wizard
{
    public interface IWizardPage
    {
        UserControl Content { get; }
        void Load(object[] saveResults);
        object Save();
        void Cancel();

        bool PageValid { get; }
        string ValidationMessage { get; }
        bool IsRequired(object[] saveResults);
    }
}