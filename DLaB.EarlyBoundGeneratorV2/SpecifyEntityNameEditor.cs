using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using DLaB.XrmToolBoxCommon.Editors;
using DLaB.XrmToolBoxCommon.Forms;

namespace DLaB.EarlyBoundGeneratorV2
{
    internal class SpecifyEntityNameEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var set = (Dictionary<string, string>) value ?? new Dictionary<string, string>();
            if (!(context?.Instance is IGetPluginControl getter))
            {
                throw new InvalidOperationException("Context Instance did not implement IGetPluginControl.  Unable to determine plugin to connect with.");
            }
            using (var dialog = new SpecifyEntityNameDialog(getter.GetPluginControl()) { ClassNamesByLogicalName = set })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    set = dialog.ClassNamesByLogicalName;
                }
            }
            return set; // can also replace the wrapper object here
        }
    }
}
