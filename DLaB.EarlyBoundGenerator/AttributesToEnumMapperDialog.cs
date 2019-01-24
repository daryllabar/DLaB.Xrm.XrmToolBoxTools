using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DLaB.XrmToolBoxCommon;
using XrmToolBox.Extensibility;

namespace DLaB.EarlyBoundGenerator
{
    public partial class AttributesToEnumMapperDialog : DialogBase
    {
        public List<string> CsvLines { get; set; }

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
            if (CsvLines == null || CsvLines.Count == 0) { return; }

            foreach (var entity in CsvLines)
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
            CsvLines = dataGridView1.Rows.Cast<DataGridViewRow>().
                Select(row => $"{row.Cells[0].Value}.{row.Cells[1].Value},{row.Cells[2].Value}").ToList();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
