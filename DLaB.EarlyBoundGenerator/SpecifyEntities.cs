using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DLaB.Common;
using DLaB.XrmToolboxCommon;
using Microsoft.Xrm.Sdk.Metadata;
using XrmToolBox.Extensibility;

namespace DLaB.EarlyBoundGenerator
{
    public partial class SpecifyEntitiesDialog : DialogBase
    {
        private int columnSortedIndex = 0;
        public string SpecifiedEntities { get; set; }

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

            if (string.IsNullOrWhiteSpace(SpecifiedEntities))
            {
                SpecifiedEntities = string.Empty;
            }
            else
            {
                SpecifiedEntities = SpecifiedEntities.Replace(" ", string.Empty);
                SpecifiedEntities = SpecifiedEntities.Replace("\n", string.Empty);
            }

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
                var localEntites = entities.ToList(); // Keep from mulitiple Enumerations

                var specified = new HashSet<string>(SpecifiedEntities.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries));

                lvExcludedEntities.Items.AddRange(localEntites.Where(e => specified.Contains(e.LogicalName)).Select(e => new ListViewItem(e.DisplayName.UserLocalizedLabel?.Label ?? "N/A") { SubItems = { e.LogicalName } }).ToArray());
                lvKeptEntities.Items.AddRange(localEntites.Where(e => !specified.Contains(e.LogicalName)).Select(e => new ListViewItem(e.DisplayName.UserLocalizedLabel?.Label ?? "N/A") { SubItems = { e.LogicalName } }).ToArray());
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
            SpecifiedEntities = Config.ToString(lvExcludedEntities.Items.Cast<ListViewItem>().Select(i => ((ObjectCollectionItem<EntityMetadata>)i.Tag).Value.LogicalName));
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            Enable(false);

            // Clear existing entity list
            ((XrmToolboxCommon.PropertyInterface.IEntityMetadatas)CallingControl).EntityMetadatas = null;

            // Retrieve entities
            RetrieveEntityMetadatasOnLoad(LoadEntities);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var values = lvKeptEntities.SelectedItems.Cast<ListViewItem>().ToArray();
            foreach (ListViewItem value in values)
            {
                lvKeptEntities.Items.Remove(value);
            }
            lvExcludedEntities.Items.AddRange(values);
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            var values = lvExcludedEntities.SelectedItems.Cast<ListViewItem>().ToArray();
            foreach (ListViewItem value in values)
            {
                lvExcludedEntities.Items.Remove(value);
            }
            lvKeptEntities.Items.AddRange(values);
        }

        private void listview_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            var lv = (ListView)sender;
            if (e.Column == columnSortedIndex)
            {
                lv.Sorting = lv.Sorting == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;

                lv.ListViewItemSorter = new ListViewItemComparer(e.Column, lv.Sorting);
            }
            else
            {
                columnSortedIndex = e.Column;
                lv.ListViewItemSorter = new ListViewItemComparer(e.Column, SortOrder.Ascending);
            }
        }
    }
}