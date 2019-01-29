using System.Windows.Forms;

namespace DLaB.VSSolutionAccelerator.Wizard
{
    public partial class GenericPage : IWizardPage
    {
        public UserControl Content => this;
        public bool IsBusy => false;

        public bool PageValid
        {
            get
            {
                if (YesNoInfo == null)
                {
                    return !string.IsNullOrWhiteSpace(ResponseText.Text + Path.Text);
                }

                var selectedInfo = YesRadio.Checked
                    ? YesNoInfo.Yes
                    : YesNoInfo.No;

                if (selectedInfo is PathQuestionInfo)
                {
                    return !string.IsNullOrWhiteSpace(Path.Text);
                }
                return !string.IsNullOrWhiteSpace(ResponseText.Text);
            }
        }

        public string ValidationMessage => "Please enter or select a value!";

        public void Cancel()
        {
        }

        public object Save()
        {
            var response = string.Empty;
            if (YesNoInfo != null)
            {
                response += YesRadio.Checked ? "Y" : "N";
            }
            return response + ResponseText.Text + Path.Text;
        }

        void IWizardPage.Load()
        {
            OnLoadAction?.Invoke(this);
        }
    }
}
