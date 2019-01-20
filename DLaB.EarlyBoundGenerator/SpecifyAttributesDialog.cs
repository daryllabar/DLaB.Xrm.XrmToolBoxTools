using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DLaB.XrmToolBoxCommon;
using DLaB.XrmToolBoxCommon.Forms;
using Source.DLaB.Common;
using XrmToolBox.Extensibility;

namespace DLaB.EarlyBoundGenerator
{
    public partial class SpecifyAttributesDialog : DialogBase
    {
        public string ConfigValue { get; set; }

        #region Constructor / Load

        public SpecifyAttributesDialog()
        {
            InitializeComponent();
        }

        public SpecifyAttributesDialog(PluginControlBase callingControl)
            : base(callingControl)
        {
            InitializeComponent();
        }

        private void SpecifyAttributesDialog_Load(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            if (string.IsNullOrWhiteSpace(ConfigValue)) { return; }

            ConfigValue = ConfigValue.Replace(" ", string.Empty);
            ConfigValue = ConfigValue.Replace("\n", string.Empty);

            var setting = Config.GetDictionaryHash<string, string>(Guid.NewGuid().ToString(), ConfigValue);

            foreach (var kvp in setting)
            {
                foreach (var attribute in kvp.Value.OrderBy(v => v))
                {
                    AddRow(kvp.Key.ToLower(), attribute);
                }
            }
        }

        #endregion // Constructor / Load

        protected void AddRow(string entityName, string attribute)
        {
            dataGridView1.Rows.Add(entityName, attribute, "Delete");
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == ColDelete.Index)
            {
                dataGridView1.Rows.RemoveAt(e.RowIndex);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            OpenAddDialog();
        }

        protected virtual void OpenAddDialog()
        {
            var dialog = new SpecifyOptionSetDialog(CallingControl, true, "Specify Attribute");

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var values = dialog.AttributeSchemaName.Split(new []{'.'}, StringSplitOptions.RemoveEmptyEntries);
                if(values.Length == 2)
                AddRow(values[0], values[1]);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            ConfigValue = string.Empty;
            var rows = dataGridView1.Rows.Cast<DataGridViewRow>().
                Select(row => new Tuple<string, string>(row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString())).ToList();

            var values = new Dictionary<string,HashSet<string>>();
            foreach (var entity in rows.GroupBy(k => k.Item1, v => v.Item2))
            {
                var hashSet = new HashSet<string>();
                values.Add(entity.Key, hashSet);
                foreach (var attribute in entity.Where(a => !hashSet.Contains(a)))
                {
                    hashSet.Add(attribute);
                }
            }

            ConfigValue = Config.ToString(values);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
