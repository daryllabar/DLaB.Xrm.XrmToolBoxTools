using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DLaB.Xrm.Entities;
using DLaB.XrmToolboxCommon;
using Microsoft.Xrm.Sdk;
using XrmToolBox.Extensibility;

namespace DLaB.EarlyBoundGenerator
{
    public partial class SpecifyActionsDialog : DialogBase
    {
        public string SpecifiedActions { get; set; }

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

            if (string.IsNullOrWhiteSpace(SpecifiedActions))
            {
                SpecifiedActions = string.Empty;
            }
            else
            {
                SpecifiedActions = SpecifiedActions.Replace("\n", string.Empty);
            }

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
                var localActions = actions.Select(e => e.ToEntity<Workflow>()).OrderBy(a => a.Name + a.Id).ToList(); // Keep from mulitiple Enumerations
                var specified = new HashSet<string>(SpecifiedActions.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries));

                LstSpecified.Items.AddRange(GetObjectCollection(localActions.Where((a, i) => specified.Contains(a.Name))));
                LstAll.Items.AddRange(GetObjectCollection(localActions.Where((a, i) => !specified.Contains(a.Name))));
            }
            finally
            {
                LstAll.EndUpdate();
                LstSpecified.EndUpdate();
                Enable(true);
            }
        }

        private static object[] GetObjectCollection(IEnumerable<Workflow> localActions)
        {
            return localActions.
                Select(e => new ObjectCollectionItem<Workflow>(e.Name, e)).
                Cast<object>().
                ToArray();
        }

        private void Enable(bool enable)
        {
            LstAll.Enabled = enable;
            LstSpecified.Enabled = enable;
            BtnAdd.Enabled = enable;
            BtnRemove.Enabled = enable;
            BtnSave.Enabled = enable;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SpecifiedActions = string.Join("|", LstSpecified.Items.Cast<ObjectCollectionItem<Workflow>>().Select(i => i.DisplayName));
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var values = LstAll.SelectedItems.Cast<object>().ToArray();
            LstSpecified.Items.AddRange(values);
            foreach (var value in values)
            {
                LstAll.Items.Remove(value);
            }
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            var values = LstSpecified.SelectedItems.Cast<object>().ToArray();
            LstAll.Items.AddRange(values);
            foreach (var value in values)
            {
                LstSpecified.Items.Remove(value);
            }
        }
    }
}
