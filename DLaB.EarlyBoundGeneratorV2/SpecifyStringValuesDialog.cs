using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DLaB.Common;
using DLaB.XrmToolBoxCommon;
using XrmToolBox.Extensibility;

namespace DLaB.EarlyBoundGeneratorV2
{
    public partial class SpecifyStringValuesDialog : DialogBase
    {
        public string SpecifiedValues { get; set; }
        private string Title { get; set; }

        #region Constructor / Load

        public SpecifyStringValuesDialog()
        {
            InitializeComponent();
        }

        public SpecifyStringValuesDialog(PluginControlBase callingControl, string title)
            : base(callingControl)
        {
            Title = title;
            InitializeComponent();
        }

        private void SpecifyStringValuesDialog_Load(object sender, EventArgs e)
        {
            Text = Title;
            Enable(false);

            SpecifiedValues = string.IsNullOrWhiteSpace(SpecifiedValues) ? string.Empty : SpecifiedValues.Replace("\n", string.Empty);

            LoadSpecifiedValues();
        }

        #endregion // Constructor / Load

        private void LoadSpecifiedValues()
        {
            try
            {
                LstValues.BeginUpdate();
                Enable(false);
                LstValues.Items.Clear();
                
                LstValues.Items.AddRange(GetObjectCollection(SpecifiedValues.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)));
            }
            finally
            {
                LstValues.EndUpdate();
                Enable(true);
            }
        }

        private static object[] GetObjectCollection(IEnumerable<string> values)
        {
            return values.
                Select(e => new ObjectCollectionItem<string>(e, e)).
                Cast<object>().
                ToArray();
        }

        private void Enable(bool enable)
        {
            LstValues.Enabled = enable;
            BtnSave.Enabled = enable;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SpecifiedValues = Config.ToString(LstValues.Items.Cast<ObjectCollectionItem<string>>().Select(i => i.DisplayName));
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
