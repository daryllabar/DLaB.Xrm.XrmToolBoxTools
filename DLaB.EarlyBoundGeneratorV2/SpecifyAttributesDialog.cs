using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DLaB.XrmToolBoxCommon;
using DLaB.XrmToolBoxCommon.Forms;
using XrmToolBox.Extensibility;

namespace DLaB.EarlyBoundGeneratorV2.Forms
{
    public partial class SpecifyAttributesDialog : DialogBase
    {
        public Dictionary<string,HashSet<string>> AttributesByEntity { get; set; }

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
            EntitiesEdd.Service = Service;
            AttributesAlc.Service = Service; 
            EntitiesEdd.LoadData();
            if (AttributesByEntity == null)
            {
                AttributesByEntity = new Dictionary<string, HashSet<string>>();
                return;
            }


            foreach (var attributes in AttributesByEntity)
            {
                foreach (var attribute in attributes.Value.OrderBy(v => v))
                {
                    AddRow(attributes.Key.ToLower(), attribute);
                }
            }
        }


        #endregion // Constructor / Load

        protected void AddRow(string entityName, string attribute)
        {
            
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
                {
                    AddRow(values[0], values[1]);
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            //var rows = dataGridView1.Rows.Cast<DataGridViewRow>().
            //    Select(row => new Tuple<string, string>(row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString())).ToList();
            //
            //var values = new Dictionary<string,HashSet<string>>();
            //foreach (var entity in rows.GroupBy(k => k.Item1, v => v.Item2))
            //{
            //    var hashSet = new HashSet<string>();
            //    values.Add(entity.Key, hashSet);
            //    foreach (var attribute in entity.Where(a => !hashSet.Contains(a)))
            //    {
            //        hashSet.Add(attribute);
            //    }
            //}
            //
            //AttributesByEntity = values;
            //DialogResult = DialogResult.OK;
            //Close();
        }
    }
}
