using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using DLaB.XrmToolBoxCommon.Forms;

namespace DLaB.XrmToolBoxCommon.Editors
{
    public class DictionaryEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var set = (Dictionary<string,string>) value ?? new Dictionary<string, string>();

            using (var dialog = new DictionaryDialog { Mapping = set})
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    set = dialog.Mapping;
                }
            }
            return set; // can also replace the wrapper object here
        }
    }
}
