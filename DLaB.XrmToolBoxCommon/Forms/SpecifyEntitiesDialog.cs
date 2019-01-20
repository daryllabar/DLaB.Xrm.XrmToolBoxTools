using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xrm.Sdk.Metadata;
using XrmToolBox.Extensibility;

// ReSharper disable once CheckNamespace
namespace DLaB.XrmToolBoxCommon.Forms
{
    public partial class SpecifyEntitiesDialog : DialogBase
    {
        public HashSet<string> SpecifiedEntities { get; set; }

        #region Constructor / Load

        public SpecifyEntitiesDialog()
        {
            InitializeComponent();
        }

        public SpecifyEntitiesDialog(PluginControlBase callingControl)
            : base(callingControl)
        {
            InitializeComponent();
        }

        private void SpecifyEntitiesDialog_Load(object sender, EventArgs e)
        {
            Enable(false);
            RetrieveEntityMetadatasOnLoad(LoadEntities);
        }

        #endregion Constructor / Load

        private void LoadEntities(IEnumerable<EntityMetadata> entities)
        {
            try
            {
                lvKeptEntities.BeginUpdate();
                lvExcludedEntities.BeginUpdate();
                Enable(false);
                lvKeptEntities.Items.Clear();
                lvExcludedEntities.Items.Clear();
                var localEntities = entities.ToList(); // Keep from multiple Enumerations

                lvExcludedEntities.Items.AddRange(localEntities.Where(e => SpecifiedEntities.Contains(e.LogicalName)).Select(e => new ListViewItem(e.DisplayName.UserLocalizedLabel?.Label ?? "N/A") { SubItems = { e.LogicalName } }).ToArray());
                lvKeptEntities.Items.AddRange(localEntities.Where(e => !SpecifiedEntities.Contains(e.LogicalName)).Select(e => new ListViewItem(e.DisplayName.UserLocalizedLabel?.Label ?? "N/A") { SubItems = { e.LogicalName } }).ToArray());
            }
            finally
            {
                lvKeptEntities.EndUpdate();
                lvExcludedEntities.EndUpdate();
                Enable(true);
            }
        }

        private void Enable(bool enable)
        {
            lvKeptEntities.Enabled = enable;
            lvExcludedEntities.Enabled = enable;
            BtnAdd.Enabled = enable;
            BtnRemove.Enabled = enable;
            BtnSave.Enabled = enable;
            BtnRefresh.Enabled = enable;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SpecifiedEntities = new HashSet<string>(lvExcludedEntities.Items.Cast<ListViewItem>().Select(i => i.SubItems[1].Text));
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            Enable(false);

            // Clear existing entity list
            ((PropertyInterface.IEntityMetadatas)CallingControl).EntityMetadatas = null;

            // Retrieve entities
            RetrieveEntityMetadatasOnLoad(LoadEntities);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var values = lvKeptEntities.SelectedItems.Cast<ListViewItem>().ToArray();
            foreach (var value in values)
            {
                lvKeptEntities.Items.Remove(value);
            }
            lvExcludedEntities.Items.AddRange(values);
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            var values = lvExcludedEntities.SelectedItems.Cast<ListViewItem>().ToArray();
            foreach (var value in values)
            {
                lvExcludedEntities.Items.Remove(value);
            }
            lvKeptEntities.Items.AddRange(values);
        }
    }
}