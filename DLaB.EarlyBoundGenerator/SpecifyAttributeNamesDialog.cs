using System;
using System.Windows.Forms;
using DLaB.XrmToolBoxCommon.Forms;
using XrmToolBox.Extensibility;

namespace DLaB.EarlyBoundGenerator
{
    public partial class SpecifyAttributeNamesDialog : SpecifyAttributesDialog
    {
        #region Constructor / Load

        public SpecifyAttributeNamesDialog()
        {
        }

        public SpecifyAttributeNamesDialog(PluginControlBase callingControl) 
            : base(callingControl)
        {
            
        }

        private void SpecifyAttributeNamesDialog_Load(object sender, EventArgs e)
        {
            Text = @"Specify Attribute Names";
        } 

        #endregion // Constructor / Load

        protected override void OpenAddDialog()
        {
            var dialog = new AttributeCaseSpecifierDialog(CallingControl);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                AddRow(dialog.EntityName, dialog.AttributeName);
            }
        }
    }
}
