using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace DLaB.XrmToolBoxCommon.Forms
{
    public partial class SpecifyOptionSetsDialog : DialogBase
    {
        /// <summary>
        /// Pipe Delimited List of OptionSet Logical Names
        /// </summary>
        public HashSet<string> OptionSets { get; set; }

        #region Constructor / Load

        public SpecifyOptionSetsDialog()
        {
            InitializeComponent();
        }

        public SpecifyOptionSetsDialog(PluginControlBase callingControl)
            : base(callingControl)
        {
            InitializeComponent();
        }


        private void OptionSetSelectorDialog_Load(object sender, EventArgs e)
        {
            Enable(false);

            if (OptionSets == null)
            {
                OptionSets = new HashSet<string> {"organization_currencyformatcode", "quote_statuscode"};
            }

            Enable(false);
            ChkListBoxOptionSets.BeginUpdate();
            ChkListBoxOptionSets.Items.Clear();
            try
            {
                ChkListBoxOptionSets.Items.AddRange(OptionSets.Cast<object>().ToArray());
            }
            finally
            {
                ChkListBoxOptionSets.EndUpdate();
                Enable(true);
            }
        }

        #endregion // Constructor / Load


        private void Enable(bool enable)
        {
            ChkListBoxOptionSets.Enabled = enable;
            BtnAdd.Enabled = enable;
            BtnDelete.Enabled = enable;
            BtnSave.Enabled = enable;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var dialog = new SpecifyOptionSetDialog(CallingControl, false);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ChkListBoxOptionSets.Items.Add(dialog.AttributeSchemaName, false);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            foreach (var item in ChkListBoxOptionSets.CheckedItems.Cast<object>().ToList())
            {
                ChkListBoxOptionSets.Items.Remove(item);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            OptionSets = new HashSet<string>(ChkListBoxOptionSets.Items.Cast<object>().Select(o => o.ToString()));
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
