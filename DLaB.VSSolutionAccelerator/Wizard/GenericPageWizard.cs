
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DLaB.VSSolutionAccelerator.Wizard
{
    public partial class GenericPage : IWizardPage
    {
        public UserControl Content => this;

        /// <summary>
        /// Contains index of the saved value and the string value that if the saved value is equal to, triggers the page as required.
        /// </summary>
        public List<KeyValuePair<Tuple<int,int>, string>> SavedValueRequiredValue { get; set; }

        /// <summary>
        /// Allows for the page to calculate a value post save of the default controls on the page.
        /// </summary>
        public Action<List<string>> PostSave { get; set; }

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
                    && ResponseTextIsValid(Response3Text)
                    && ResponseTextIsValid(Response4Text)
                    && PathTextIsValid(Path, CheckFileExists)
                    && PathTextIsValid(Path2, CheckFile2Exists)
                    && PathTextIsValid(Path3, CheckFile3Exists)
                    && PathTextIsValid(Path4, CheckFile4Exists);
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
            AddValue(response, Response3Text);
            AddValue(response, Path3);
            AddValue(response, Combo3);
            AddValue(response, Response4Text);
            AddValue(response, Path4);
            AddValue(response, Combo4);

            PostSave?.Invoke(response);

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

        public void AddSavedValuedRequiredCondition(int savedPageIndex, string equalToValue)
        {
            SavedValueRequiredValue.Add(new KeyValuePair<Tuple<int,int>, string>(new Tuple<int, int>(savedPageIndex, 0), equalToValue));
        }

        public void AddSavedValuedRequiredCondition(int savedPageIndex, int savedValueIndex, string equalToValue)
        {
            SavedValueRequiredValue.Add(new KeyValuePair<Tuple<int, int>, string>(new Tuple<int, int>(savedPageIndex, savedValueIndex), equalToValue));
        }

        bool IWizardPage.IsRequired(object[] saveResults)
        {
            if (SavedValueRequiredValue == null)
            {
                return true;
            }

            var isRequired = true;
            foreach (var condition in SavedValueRequiredValue)
            {
                var value = saveResults[condition.Key.Item1];
                if (value is List<string> list)
                {
                    value = list[condition.Key.Item2];
                }

                if (!string.Equals(value?.ToString(), condition.Value))
                {
                    isRequired = false;
                    break;
                }
            }

            return isRequired;
        }
    }
}
