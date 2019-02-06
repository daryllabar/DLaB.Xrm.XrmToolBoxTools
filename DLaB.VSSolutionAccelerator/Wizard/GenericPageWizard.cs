using System.Collections.Generic;
using System.Linq;
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
                if (YesNoInfo != null
                    && !YesRadio.Checked
                    && !NoRadio.Checked)
                {
                    return false;
                }

                return ResponseTextIsValid(ResponseText)
                    && ResponseTextIsValid(Response2Text)
                    && PathTextIsValid(Path, CheckFileExists)
                    && PathTextIsValid(Path2, CheckFile2Exists);
            }
        }

        private static bool ResponseTextIsValid(Control box)
        {
            return !box.Visible 
                   || !string.IsNullOrWhiteSpace(box.Text);
        }

        private static bool PathTextIsValid(Control box, bool checkFileExists)
        {
            return !box.Visible 
                   || checkFileExists && System.IO.File.Exists(box.Text)
                   || !checkFileExists && !string.IsNullOrWhiteSpace(box.Text);
        }


        public string ValidationMessage => "Please enter or select a valid value!";

        public void Cancel()
        {
        }

        public object Save()
        {
            var response = new List<string>();
            if (YesNoInfo != null)
            {
                response.Add(YesRadio.Checked ? "Y" : "N");
            }

            AddValue(response, ResponseText);
            AddValue(response, Path);
            AddValue(response, Combo);
            AddValue(response, Response2Text);
            AddValue(response, Path2);
            AddValue(response, Combo2);
            return response.Count == 1 
                ? (object)response.First()
                : response;
        }


        private static void AddValue(List<string> values, Control box)
        {
            if (box.Visible)
            {
                values.Add(box.Text);
            }
        }

        private static void AddValue(List<string> values, ComboBox box)
        {
            if (box.Visible)
            {
                values.Add(box.SelectedValue.ToString());
            }
        }

        void IWizardPage.Load(object[] saveResults)
        {
            OnLoadAction?.Invoke(this, saveResults);
        }
    }
}
