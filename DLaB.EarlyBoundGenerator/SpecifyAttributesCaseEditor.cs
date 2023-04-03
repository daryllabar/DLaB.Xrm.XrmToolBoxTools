using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using DLaB.XrmToolBoxCommon.Editors;

namespace DLaB.EarlyBoundGenerator
{
    internal class SpecifyAttributesCaseEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var set = (Dictionary<string,HashSet<string>>) value ?? new Dictionary<string, HashSet<string>>();
            if (!(context?.Instance is IGetPluginControl getter))
            {
                throw new InvalidOperationException("Context Instance did not implement IGetPluginControl.  Unable to determine plugin to connect with.");
            }
            using (var dialog = new SpecifyAttributeNamesDialog(getter.GetPluginControl()) { AttributesByEntity = set})
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    set = dialog.AttributesByEntity;
                }

            }
            return set; // can also replace the wrapper object here
        }
    }
}
