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
using Source.DLaB.Xrm;
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
            Text = "Specify Attribute Names";
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
