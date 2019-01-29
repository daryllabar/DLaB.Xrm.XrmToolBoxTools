using System;
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

                if (YesRadio.Checked)
                {
                    return YesNoInfo.YesRow < 0
                           || !string.IsNullOrWhiteSpace(ResponseText.Text + Path.Text);
                }

                if (NoRadio.Checked)
                {
                    return YesNoInfo.NoRow < 0
                           || !string.IsNullOrWhiteSpace(ResponseText.Text + Path.Text);
                }

                return false;
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
