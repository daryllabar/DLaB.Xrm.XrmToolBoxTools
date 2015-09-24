using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DLaB.XrmToolboxCommon;
using DLaB.Xrm;
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

            foreach (var entity in ConfigValue.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).OrderBy(v => v))
            {
                var splitAttributes = entity.Split(',');

                foreach (var attribute in splitAttributes.Skip(1).OrderBy(v => v))
                {
                    AddRow(splitAttributes[0].ToLower(), attribute);
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
            var dialog = new OptionSetSpecifierDialog(CallingControl, true);

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
            var entities = new List<string>();
            var rows = dataGridView1.Rows.Cast<DataGridViewRow>().
                Select(row => new Tuple<string, string>(row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString())).ToList();

            foreach (var entity in rows.GroupBy(k => k.Item1, v => v.Item2))
            {
                var sb = new StringBuilder();
                sb.Append(entity.Key);
                foreach (var attribute in entity)
                {
                    sb.Append("," + attribute);
                }
                entities.Add(sb.ToString());
            }

            ConfigValue = string.Join("|", entities);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
