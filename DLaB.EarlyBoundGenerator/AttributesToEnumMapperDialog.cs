using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DLaB.XrmToolBoxCommon;
using Source.DLaB.Common;
using Source.DLaB.Xrm;
using XrmToolBox.Extensibility;

namespace DLaB.EarlyBoundGenerator
{
    public partial class AttributesToEnumMapperDialog : DialogBase
    {
        public string ConfigValue { get; set; }

        #region Constructor / Load

        public AttributesToEnumMapperDialog()
        {
            InitializeComponent();
        }

        public AttributesToEnumMapperDialog(PluginControlBase callingControl)
            : base(callingControl)
        {
            InitializeComponent();
        }

        private void AttributesToEnumMapperDialog_Load(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            if (string.IsNullOrWhiteSpace(ConfigValue)) { return; }

            ConfigValue = ConfigValue.Replace(" ", string.Empty);

            foreach (var entity in ConfigValue.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var splitAttributes = entity.Split(new []{',', '.'}, StringSplitOptions.RemoveEmptyEntries);
                if(splitAttributes.Length == 3)
                    AddRow(splitAttributes[0].Trim(), splitAttributes[1].Trim(), splitAttributes[2].Trim());
            }
        }

        #endregion // Constructor / Load

        private void AddRow(string entityName, string attribute, string optionSetSchemaName)
        {
            dataGridView1.Rows.Add(entityName, attribute, optionSetSchemaName, "Delete");
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
            var dialog = new AttributeToEnumMapperDialog(CallingControl);
            
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                AddRow(dialog.EntityName, dialog.AttributeName, dialog.OptionSetSchemaName);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            ConfigValue = string.Empty;
            var mappings = dataGridView1.Rows.Cast<DataGridViewRow>().
                Select(row => string.Format("{0}.{1},{2}", row.Cells[0].Value, row.Cells[1].Value, row.Cells[2].Value)).ToList();

            ConfigValue = Config.ToString(mappings);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
