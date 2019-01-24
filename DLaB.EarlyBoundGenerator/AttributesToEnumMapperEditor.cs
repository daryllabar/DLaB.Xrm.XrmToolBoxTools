using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using DLaB.XrmToolBoxCommon.Editors;

namespace DLaB.EarlyBoundGenerator
{
    internal class AttributesToEnumMapperEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var set = (List<string>) value ?? new List<string>();
            if (!(context?.Instance is IGetPluginControl getter))
            {
                throw new InvalidOperationException("Context Instance did not implement IGetPluginControl.  Unable to determine plugin to connect with.");
            }
            using (var dialog = new AttributesToEnumMapperDialog(getter.GetPluginControl()) { CsvLines = set})
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    set = dialog.CsvLines;
                }

            }
            return set; // can also replace the wrapper object here
        }
    }
}
