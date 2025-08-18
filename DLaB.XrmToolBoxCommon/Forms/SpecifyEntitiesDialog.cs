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

                lvSelectedEntities.Items.AddRange(allEntities.Where(e => SpecifiedEntities.Contains(e.LogicalName)).Select(e => new ListViewItem(e.DisplayName.UserLocalizedLabel?.Label ?? "N/A") { SubItems = { e.LogicalName }, Tag = e }).ToArray());
                ShowEntitiesByFiltering();
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
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            var values = lvSelectedEntities.SelectedItems.Cast<ListViewItem>().ToArray();
            foreach (var value in values)
            {
                lvSelectedEntities.Items.Remove(value);
            }
            lvAvailableEntities.Items.AddRange(values);
        }

        private void PlummingFilterAvailable(object sender = null, EventArgs ea = null)
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
                cmbFilterBy.Items.Add("");
                cmbFilterBy.Items.AddRange(solutions
                    .Select(s => new EntityProxy(s, $"{s.GetAttributeValue<string>("friendlyname")} ({s.GetAliasedValue<string>("P.friendlyname")})"))
                    .ToArray());
                cmbFilterBy.Enabled = true;
            }
            else if (rbAvailPublisher.Checked)
            {
                if (publishers == null)
                {
                    LoadPublishers();
                }
                cmbFilterBy.Items.Clear();
                cmbFilterBy.Items.Add("");
                cmbFilterBy.Items.AddRange(publishers
                    .Select(p => new EntityProxy(p, $"{p.GetAttributeValue<string>("friendlyname")} ({p.GetAliasedValue<string>("customizationprefix")})"))
                    .ToArray());
                cmbFilterBy.Enabled = true;
            }
            else
            {
                // Show all entities
                lvAvailableEntities.Items.AddRange(allEntities
                    .Select(e => new ListViewItem(e.DisplayName.UserLocalizedLabel?.Label ?? "N/A") { Tag = e, SubItems = { e.LogicalName } })
                    .ToArray());
                cmbFilterBy.Enabled = false;
            }
            ShowEntitiesByFiltering();
        }

        private void ShowEntitiesByFiltering(object sender = null, EventArgs ea = null)
        {
            lvAvailableEntities.Items.Clear();
            var selectedentityguids = new List<Guid>();
            if (rbAvailAll.Checked)
            {
                selectedentityguids = allEntities.Select(e => e.MetadataId ?? Guid.Empty).ToList();
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
                    selectedentityguids = result.Entities.Select(e => e.GetAttributeValue<Guid>("objectid")).ToList();
                }
                catch (Exception ex)
                {
                    CallingControl.ShowErrorDialog(ex, "Loading Solutions Entities");
                }
            }

            var selectedentities = lvSelectedEntities.Items.Cast<ListViewItem>().Select(i => i.Tag as EntityMetadata).ToList();

            lvAvailableEntities.Items.AddRange(allEntities
                .Where(e => selectedentityguids.Contains(e.MetadataId ?? Guid.Empty))
                .Where(e => !selectedentities.Contains(e))
                .Select(e => new ListViewItem(e.DisplayName.UserLocalizedLabel?.Label ?? "N/A") { Tag = e, SubItems = { e.LogicalName } })
                .ToArray());
        }

        private void LoadSolutions()
        {
            if (CallingControl.Service == null)
            {
                solutions = new List<Entity>();
                return;
            }
            var query = new QueryExpression("solution");
            query.ColumnSet.AddColumns("friendlyname", "uniquename", "ismanaged", "isvisible", "version");
            query.Criteria.AddCondition("isvisible", ConditionOperator.Equal, true);
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
            finally
            {
                Cursor = Cursors.Default;
            }
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