using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Source.DLaB.Xrm;
using XrmToolBox.Extensibility;

// ReSharper disable once CheckNamespace
namespace DLaB.XrmToolBoxCommon.Forms
{
    public partial class SpecifyEntitiesDialog : DialogBase
    {
        private List<EntityMetadata> allEntities;
        private List<Entity> solutions;
        private List<Entity> publishers;
        private List<Guid> filteredEntityGuids;

        public HashSet<string> SpecifiedEntities { get; set; }

        #region Constructor / Load

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
                lvAvailableEntities.BeginUpdate();
                lvSelectedEntities.BeginUpdate();
                Enable(false);
                lvAvailableEntities.Items.Clear();
                lvSelectedEntities.Items.Clear();
                allEntities = entities.ToList(); // Keep from multiple Enumerations

                lvSelectedEntities.Items.AddRange(allEntities
                    .Where(e => SpecifiedEntities.Contains(e.LogicalName))
                    .Select(e => new ListViewItem(e.GetDisplayName("N/A")) { SubItems = { e.LogicalName }, Tag = e })
                    .ToArray());
                PopulateFilter();
            }
            finally
            {
                lvAvailableEntities.EndUpdate();
                lvSelectedEntities.EndUpdate();
                Enable(true);
            }
        }

        private void Enable(bool enable)
        {
            panFilterAvailable.Enabled = enable;
            lvAvailableEntities.Enabled = enable;
            lvSelectedEntities.Enabled = enable;
            BtnAdd.Enabled = enable;
            BtnRemove.Enabled = enable;
            BtnSave.Enabled = enable;
            BtnRefresh.Enabled = enable;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SpecifiedEntities = new HashSet<string>(lvSelectedEntities.Items.Cast<ListViewItem>().Select(i => i.SubItems[1].Text));
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            Enable(false);

            allEntities = null;
            solutions = null;
            publishers = null;

            // Clear existing entity list
            ((PropertyInterface.IEntityMetadatas)CallingControl).EntityMetadatas = null;

            // Retrieve entities
            RetrieveEntityMetadatasOnLoad(LoadEntities);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var values = lvAvailableEntities.SelectedItems.Cast<ListViewItem>().ToArray();
            foreach (var value in values)
            {
                lvAvailableEntities.Items.Remove(value);
            }
            lvSelectedEntities.Items.AddRange(values);
            UpdateCounts();
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            var values = lvSelectedEntities.SelectedItems.Cast<ListViewItem>().ToArray();
            foreach (var value in values)
            {
                lvSelectedEntities.Items.Remove(value);
            }
            lvAvailableEntities.Items.AddRange(values);
            UpdateCounts();
        }

        private void PopulateFilter(object sender = null, EventArgs ea = null)
        {
            lvAvailableEntities.Items.Clear();
            if (allEntities == null)
            {
                return; // No entities loaded yet
            }
            if (rbAvailSolution.Checked)
            {
                if (solutions == null)
                {
                    LoadSolutions();
                }
                cmbFilterBy.Items.Clear();
                cmbFilterBy.Items.AddRange(solutions
                    .Select(s => new EntityProxy(s, $"{s.GetAttributeValue<string>("friendlyname")} ({s.GetAliasedValue<string>("P.friendlyname")})"))
                    .ToArray());
            }
            else if (rbAvailPublisher.Checked)
            {
                if (publishers == null)
                {
                    LoadPublishers();
                }
                cmbFilterBy.Items.Clear();
                cmbFilterBy.Items.AddRange(publishers
                    .Select(p => new EntityProxy(p, $"{p.GetAttributeValue<string>("friendlyname")} ({p.GetAttributeValue<string>("customizationprefix")})"))
                    .ToArray());
            }
            ShowEntitiesByFiltering();
        }

        private void ShowEntitiesByFiltering(object sender = null, EventArgs ea = null)
        {
            lvAvailableEntities.Items.Clear();
            filteredEntityGuids = new List<Guid>();
            if (rbAvailAll.Checked)
            {
                filteredEntityGuids = allEntities.Select(e => e.MetadataId ?? Guid.Empty).ToList();
            }
            else if (cmbFilterBy.SelectedItem is EntityProxy proxy && proxy.Entity is Entity selected)
            {
                var query = new QueryExpression("solutioncomponent");
                query.ColumnSet.AddColumns("objectid", "solutioncomponentid", "rootcomponentbehavior", "rootsolutioncomponentid", "ismetadata", "componenttype");
                if (selected.LogicalName == "solution")
                {
                    query.Criteria.AddCondition("solutionid", ConditionOperator.Equal, selected.Id);
                }
                else if (selected.LogicalName == "publisher")
                {
                    var query_solution = query.AddLink("solution", "solutionid", "solutionid");
                    query_solution.LinkCriteria.AddCondition("publisherid", ConditionOperator.Equal, selected.Id);
                }
                else
                {
                    query.Criteria.AddCondition("solutionid", ConditionOperator.Null);
                }
                query.Criteria.AddCondition("componenttype", ConditionOperator.Equal, 1);
                try
                {
                    var result = CallingControl.Service.RetrieveMultiple(query);
                    filteredEntityGuids = result.Entities.Select(e => e.GetAttributeValue<Guid>("objectid")).ToList();
                }
                catch (Exception ex)
                {
                    CallingControl.ShowErrorDialog(ex, "Loading Solutions Entities");
                }
            }

            filteredEntityGuids = filteredEntityGuids.Distinct().ToList();
            var selectedentities = lvSelectedEntities.Items.Cast<ListViewItem>().Select(i => i.Tag as EntityMetadata).ToList();
            var filtertext = panFilter.Visible ? txtFilter.Text.Trim() : string.Empty;

            lvAvailableEntities.Items.AddRange(allEntities
                .Where(e => filteredEntityGuids.Contains(e.MetadataId ?? Guid.Empty))
                .Where(e => !selectedentities.Contains(e))
                .Where(e =>
                    string.IsNullOrEmpty(filtertext) ||
                    e.GetDisplayName().IndexOf(filtertext, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    e.LogicalName.IndexOf(filtertext, StringComparison.OrdinalIgnoreCase) >= 0)
                .Select(e => new ListViewItem(e.GetDisplayName()) { Tag = e, SubItems = { e.LogicalName } })
                .ToArray());
            panFilterAvailable.Height = rbAvailAll.Checked ? 24 : 48;
            cmbFilterBy.Enabled = !rbAvailAll.Checked;
            UpdateCounts();
        }

        private void UpdateCounts()
        {
            if ((allEntities?.Count ?? 0) == lvAvailableEntities.Items.Count)
            {
                gbEntitiesAvailable.Text = $"Available Entities: {allEntities?.Count ?? 0}";
                toolTip1.SetToolTip(gbEntitiesAvailable, "Total number of entities");
            }
            else if (allEntities?.Count == filteredEntityGuids?.Count)
            {
                gbEntitiesAvailable.Text = $"Available Entities: {allEntities?.Count ?? 0} / {lvAvailableEntities.Items.Count}";
                toolTip1.SetToolTip(gbEntitiesAvailable, "Total number of entities / Not added entities");
            }
            else
            {
                gbEntitiesAvailable.Text = $"Available Entities: {allEntities?.Count ?? 0} / {filteredEntityGuids.Count} / {lvAvailableEntities.Items.Count}";
                toolTip1.SetToolTip(gbEntitiesAvailable, "Total number of entities / Filtered out entities / Not added entities");
            }
            gbEntitiesSelected.Text = $"Selected Entities: {lvSelectedEntities.Items.Count}";
        }

        private void LoadSolutions()
        {
            if (CallingControl.Service == null)
            {
                solutions = new List<Entity>();
                return;
            }
            var query = new QueryExpression("solution");
            query.ColumnSet.AddColumns("friendlyname", "uniquename", "ismanaged", "version");
            query.Criteria.AddCondition("isvisible", ConditionOperator.Equal, true);
            query.Criteria.AddCondition("uniquename", ConditionOperator.NotEqual, "Default");
            var query_solutioncomponent = new LinkEntity("solution", "solutioncomponent", "solutionid", "solutionid", JoinOperator.Any);
            query_solutioncomponent.LinkCriteria.AddCondition("componenttype", ConditionOperator.Equal, 1);
            query.Criteria.AnyAllFilterLinkEntity = query_solutioncomponent;
            query.AddOrder("ismanaged", OrderType.Descending);
            query.AddOrder("friendlyname", OrderType.Ascending);
            var publisher = query.AddLink("publisher", "publisherid", "publisherid");
            publisher.EntityAlias = "P";
            publisher.Columns.AddColumns("customizationprefix", "uniquename", "friendlyname");
            try
            {
                solutions = CallingControl.Service.RetrieveMultiple(query).Entities.ToList();
            }
            catch (Exception ex)
            {
                CallingControl.ShowErrorDialog(ex, "Loading Solutions");
            }
        }

        private void LoadPublishers()
        {
            if (CallingControl.Service == null)
            {
                publishers = new List<Entity>();
                return;
            }
            var query = new QueryExpression("publisher");
            query.ColumnSet.AddColumns("publisherid", "friendlyname", "customizationprefix");
            var query_solution = new LinkEntity("publisher", "solution", "publisherid", "publisherid", JoinOperator.Any);
            query.Criteria.AnyAllFilterLinkEntity = query_solution;
            query_solution.LinkCriteria.AddCondition("isvisible", ConditionOperator.Equal, true);
            query.AddOrder("friendlyname", OrderType.Ascending);
            try
            {
                publishers = CallingControl.Service.RetrieveMultiple(query).Entities.ToList();
            }
            catch (Exception ex)
            {
                CallingControl.ShowErrorDialog(ex, "Loading Publishers");
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            panFilter.Visible = !panFilter.Visible;
            btnFilter.FlatStyle = panFilter.Visible ? FlatStyle.Flat : FlatStyle.Standard;
            if (panFilter.Visible)
            {
                txtFilter.Focus();
            }
            else
            {
                lvAvailableEntities.Focus();
            }
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            tmFilter.Start();
        }

        private void tmFilter_Tick(object sender, EventArgs e)
        {
            tmFilter.Stop();
            ShowEntitiesByFiltering();
        }
    }

    public class EntityProxy
    {
        private string displayName;
        public Entity Entity;

        public EntityProxy(Entity entity, string displayname)
        {
            Entity = entity ?? throw new ArgumentNullException(nameof(entity));
            displayName = displayname;
        }

        public override string ToString() => displayName ?? Entity?.Id.ToString() ?? "N/A";
    }
}