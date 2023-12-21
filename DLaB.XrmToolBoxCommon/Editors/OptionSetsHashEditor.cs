using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using DLaB.XrmToolBoxCommon.Forms;

namespace DLaB.XrmToolBoxCommon.Editors
{
    public class OptionSetsHashEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var set = (HashSet<string>) value ?? new HashSet<string>();
            if (!(context?.Instance is IGetPluginControl getter))
            {
                throw new InvalidOperationException("Context Instance did not implement IGetPluginControl.  Unable to determine plugin to connect with.");
            }
            using (var dialog = new SpecifyOptionSetsDialog(getter.GetPluginControl()) { OptionSets = set})
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    set = dialog.OptionSets;
                }

            }
            return set; // can also replace the wrapper object here
        }
    }
}
