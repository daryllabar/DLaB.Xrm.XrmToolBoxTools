using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DLaB.Xrm.Entities;
using Microsoft.Xrm.Sdk;
using XrmToolBox.Extensibility;

// ReSharper disable once CheckNamespace
namespace DLaB.XrmToolBoxCommon.Forms
{
    public partial class SpecifyActionsDialog : DialogBase
    {
        public HashSet<string> SpecifiedActions { get; set; }

        #region Constructor / Load

        public SpecifyActionsDialog()
        {
            InitializeComponent();
        }

        public SpecifyActionsDialog(PluginControlBase callingControl)
            : base(callingControl)
        {
            InitializeComponent();
        }

        private void SpecifyActivitiesDialog_Load(object sender, EventArgs e)
        {
            Enable(false);
            RetrieveActionsOnLoad(LoadActions);
        }

        #endregion // Constructor / Load

        private void LoadActions(IEnumerable<Entity> actions)
        {
            try
            {
                LstAll.BeginUpdate();
                LstSpecified.BeginUpdate();
                Enable(false);
                LstAll.Items.Clear();
                LstSpecified.Items.Clear();
                var localActions = actions.Select(e => e.ToEntity<Workflow>()).ToList(); // Keep from multiple Enumerations

                

                LstSpecified.Items.AddRange(localActions.Where(IsSpecified).Select(e => new ListViewItem(e.Name ?? "N/A") { SubItems = { GetKey(e) }}).ToArray());
                LstAll.Items.AddRange(localActions.Where(e => !IsSpecified(e)).Select(e => new ListViewItem(e.Name ?? "N/A") { SubItems = { GetKey(e) }}).ToArray());
            }
            finally
            {
                LstAll.EndUpdate();
                LstSpecified.EndUpdate();
                Enable(true);
            }
        }

        private bool IsSpecified(Workflow action)
        {
            var logicalName = action.GetAttributeValue<string>("sdklogicalname");
            if (!string.IsNullOrWhiteSpace(logicalName) && SpecifiedActions.Contains(logicalName.ToLower()))
            {
                return true;
            }

            // For Backwards compatibility, Check Unique Name
            return SpecifiedActions.Contains(action.UniqueName) || SpecifiedActions.Contains(action.UniqueName.ToLower());
        }

        private string GetKey(Workflow action)
        {
            return action.GetAttributeValue<string>("sdklogicalname");
        }

        private void Enable(bool enable)
        {
            LstAll.Enabled = enable;
            LstSpecified.Enabled = enable;
            BtnAdd.Enabled = enable;
            BtnRemove.Enabled = enable;
            BtnSave.Enabled = enable;
            BtnRefresh.Enabled = enable;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SpecifiedActions = new HashSet<string>(LstSpecified.Items.Cast<ListViewItem>().Select(i => i.SubItems[1].Text));
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            Enable(false);

            // Clear existing entity list
            ((PropertyInterface.IActions)CallingControl).Actions = null;

            // Retrieve entities
            RetrieveActionsOnLoad(LoadActions);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var values = LstAll.SelectedItems.Cast<ListViewItem>().ToArray();
            foreach (var value in values)
            {
                LstAll.Items.Remove(value);
            }
            LstSpecified.Items.AddRange(values);
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            var values = LstSpecified.SelectedItems.Cast<ListViewItem>().ToArray();
            foreach (var value in values)
            {
                LstSpecified.Items.Remove(value);
            }
            LstAll.Items.AddRange(values);
        }

        private SortOrder _order = SortOrder.Ascending;
        private int _sortedColumn = 0;
        private void ColumnClick(object o, ColumnClickEventArgs e)
        {
            if (e.Column == _sortedColumn)
            {
                _order = _order == SortOrder.Ascending
                    ? SortOrder.Descending
                    : SortOrder.Ascending;
            }
            else
            {
                _order = SortOrder.Ascending;
                _sortedColumn = e.Column;
            }

            // Set the ListViewItemSorter property to a new ListViewItemComparer 
            // object. Setting this property immediately sorts the 
            // ListView using the ListViewItemComparer object.
            ((ListView) o).ListViewItemSorter = new ListViewItemComparer(e.Column, _order);
        }

    }
}
