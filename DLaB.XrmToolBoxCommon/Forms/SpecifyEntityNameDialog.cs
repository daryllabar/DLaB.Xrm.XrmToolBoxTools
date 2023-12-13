using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xrm.Sdk.Metadata;
using XrmToolBox.Extensibility;

// ReSharper disable once CheckNamespace
namespace DLaB.XrmToolBoxCommon.Forms
{
    public partial class SpecifyEntityNameDialog : DialogBase
    {
        public Dictionary<string, string> ClassNamesByLogicalName { get; set; }
        private ListViewItem.ListViewSubItem ClassNameItemClicked => _listItemClicked?.SubItems[chClassName.Index];
        private ListViewItem _listItemClicked;
        private ListViewColumnSorter _columnSorter;

        #region Constructor / Load

        public SpecifyEntityNameDialog()
        {
            InitializeComponent();
        }

        public SpecifyEntityNameDialog(PluginControlBase callingControl)
            : base(callingControl)
        {
            InitializeComponent();
        }

        private void SpecifyEntityNameDialog_Load(object sender, EventArgs e)
        {
            _columnSorter = new ListViewColumnSorter();
            lvEntities.ListViewItemSorter = _columnSorter;
            TxtEdit.Visible = false;
            Enable(false);
            RetrieveEntityMetadatasOnLoad(LoadEntities);
        }

        #endregion Constructor / Load

        private void LoadEntities(IEnumerable<EntityMetadata> entities)
        {
            try
            {
                lvEntities.BeginUpdate();
                Enable(false);
                lvEntities.Items.Clear();
                var localEntities = entities.ToList(); // Keep from multiple Enumerations


                lvEntities.Items.AddRange(localEntities.Select(e => new ListViewItem(e.DisplayName.UserLocalizedLabel?.Label ?? "N/A") { 
                    SubItems = {
                            e.LogicalName,
                            ClassNamesByLogicalName.TryGetValue(e.LogicalName, out var displayName) ? displayName : string.Empty
                        }
                    }).ToArray());
            }
            finally
            {
                ResizeColumns();
                lvEntities.EndUpdate();
                Enable(true);
            }
        }

        private void Enable(bool enable)
        {
            lvEntities.Enabled = enable;
            BtnSave.Enabled = enable;
            BtnRefresh.Enabled = enable;
        }

        private void HideTextEditor()
        {
            TxtEdit.Visible = false;
            if (ClassNameItemClicked != null)
                ClassNameItemClicked.Text = TxtEdit.Text;
            _listItemClicked = null;
            TxtEdit.Text = string.Empty;
        }

        private void ResizeColumns()
        {
            lvEntities.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lvEntities.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }


        private void BtnSave_Click(object sender, EventArgs e)
        {
            ClassNamesByLogicalName = new Dictionary<string, string>();
            foreach(var item in lvEntities.Items.Cast<ListViewItem>())
            {
                var className = item.SubItems[chClassName.Index].Text;
                if (string.IsNullOrEmpty(className))
                {
                    continue;
                }
                ClassNamesByLogicalName.Add(item.SubItems[chLogicalName.Index].Text, className);
            }
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

        private void SpecifyEntityNameDialog_Resize(object sender, EventArgs e)
        {
            ResizeColumns();
        }

        private void LvEntities_MouseUp(object sender, MouseEventArgs e)
        {
            var i = lvEntities.HitTest(e.X, e.Y);
            _listItemClicked = i.Item;
            if (ClassNameItemClicked == null)
                return;
            var cellWidth = ClassNameItemClicked.Bounds.Width;
            var cellHeight = ClassNameItemClicked.Bounds.Height;
            var cellLeft = (int)lvEntities.BorderStyle + lvEntities.Left + ClassNameItemClicked.Bounds.Left;
            var cellTop = lvEntities.Top + ClassNameItemClicked.Bounds.Top;

            TxtEdit.Location = new Point(cellLeft, cellTop);
            TxtEdit.Size = new Size(cellWidth, cellHeight);
            TxtEdit.Visible = true;
            TxtEdit.BringToFront();
            TxtEdit.Text = ClassNameItemClicked.Text;
            TxtEdit.Select();
            TxtEdit.SelectAll();
        }

        private void TxtEdit_Leave(object sender, EventArgs e)
        {
            HideTextEditor();
        }

        private void TxtEdit_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return) {
                HideTextEditor();
            }
        }

        private void lvEntities_MouseDown(object sender, MouseEventArgs e)
        {
            HideTextEditor();
        }

        private void lvEntities_Scroll(object sender, ScrollEventArgs e)
        {
            HideTextEditor();
        }

        private void lvEntities_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == _columnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (_columnSorter.Order == SortOrder.Ascending)
                {
                    _columnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    _columnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                _columnSorter.SortColumn = e.Column;
                _columnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            lvEntities.Sort();
        }
    }
}