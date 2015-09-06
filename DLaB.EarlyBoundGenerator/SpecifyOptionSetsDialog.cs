using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DLaB.Xrm;
using DLaB.XrmToolboxCommon;
using PropertyInterface = DLaB.XrmToolboxCommon.PropertyInterface;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using XrmToolBox.Extensibility;

namespace DLaB.EarlyBoundGenerator
{
    public partial class SpecifyOptionSetsDialog : DialogBase
    {
        /// <summary>
        /// Pipe Delimited List of OptionSet Logical Names
        /// </summary>
        public String OptionSets { get; set; }

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
                OptionSets = "organization_currencyformatcode|quote_statuscode";
            }

            Enable(false);
            ChkListBoxOptionSets.BeginUpdate();
            ChkListBoxOptionSets.Items.Clear();
            try
            {
                if (String.IsNullOrWhiteSpace(OptionSets)) { return; }

                OptionSets = OptionSets.Replace(" ", String.Empty);

                foreach (var optionSet in OptionSets.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries))
                {
                    ChkListBoxOptionSets.Items.Add(optionSet, false);
                }
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
            var dialog = new OptionSetSpecifierDialog(CallingControl, false);

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
            OptionSets = String.Join("|", ChkListBoxOptionSets.Items.Cast<Object>().Select(o => o.ToString()));
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
