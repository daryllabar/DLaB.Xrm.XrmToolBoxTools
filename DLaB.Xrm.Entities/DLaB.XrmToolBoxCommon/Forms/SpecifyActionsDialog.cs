using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DLaB.Xrm.Entities;
using Microsoft.Xrm.Sdk;
using Source.DLaB.Common;
using Source.DLaB.Xrm;
using XrmToolBox.Extensibility;

// ReSharper disable once CheckNamespace
namespace DLaB.XrmToolBoxCommon.Forms
{
    public partial class SpecifyActionsDialog : DialogBase
    {
        public HashSet<string> SpecifiedActions { get; set; }
        private List<Workflow> WorkflowlessActions { get; set; }
        #region Constructor / Load

        public SpecifyActionsDialog()
        {
            InitializeComponent();
            WorkflowlessActions = new List<Workflow>();
        }

        public SpecifyActionsDialog(PluginControlBase callingControl)
            : base(callingControl)
        {
            InitializeComponent();
            WorkflowlessActions = callingControl is IGetEditorSetting getter
                ? GetWorkflowLessActions(getter.GetEditorSetting(EditorSetting.WorkflowlessActions).GetList<string>())
                : new List<Workflow>();
        }

        private void SpecifyActivitiesDialog_Load(object sender, EventArgs e)
        {
            Enable(false);
            RetrieveActionsOnLoad(LoadActions);
        }

        #endregion // Constructor / Load

        private List<Workflow> GetWorkflowLessActions(List<string> names)
        {
            return names.Select(n => new Workflow
            {
                Name = "(" + n.Trim() + ")",
                ["sdklogicalname"] = n.Trim().ToLower(),
                UniqueName = n.Trim().ToLower()
            }).ToList();
        }

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
                localActions.AddRange(WorkflowlessActions);
                

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
            if (!string.IsNullOrWhiteSpace(logicalName)
                && (SpecifiedActions.Contains(logicalName.ToLower())
                    || SpecifiedActions.Contains(logicalName)))
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

        protected void RetrieveActionsOnLoad(Action<IEnumerable<Entity>> loadActions)
        {
            var actions = ((PropertyInterface.IActions)CallingControl).Actions;

            if (actions == null)
            {
                _callBackForRetrieveActions = loadActions;
                ExecuteMethod(RetrieveActions);
            }
            else
            {
                loadActions(actions);
            }
        }

        private Action<IEnumerable<Entity>> _callBackForRetrieveActions;

        private void RetrieveActions()
        {
            WorkAsync(new WorkAsyncInfo("Retrieving Actions...", e =>
            {
                var qe = QueryExpressionFactory.Create<Workflow>(w => new { w.Name, w.UniqueName },
                    Workflow.Fields.Category, (int)Workflow_Category.Action,
                    Workflow.Fields.ParentWorkflowId, null);
                qe.AddLink<SdkMessage>(Workflow.Fields.SdkMessageId, m => new { m.Name });
                var entities = Service.RetrieveMultiple(qe.Query).ToEntityList<Workflow>();
                e.Result = entities.Select(w =>
                {
                    w[@"sdklogicalname"] = w.GetAliasedEntity<SdkMessage>().Name;
                    return w.ToSdkEntity();
                }).ToList();
            })
            {
                PostWorkCallBack = e =>
                {
                    var actionContainer = ((PropertyInterface.IActions)CallingControl);
                    actionContainer.Actions = (List<Entity>)e.Result;
                    _callBackForRetrieveActions(actionContainer.Actions);
                }
            });
        }
    }
}
