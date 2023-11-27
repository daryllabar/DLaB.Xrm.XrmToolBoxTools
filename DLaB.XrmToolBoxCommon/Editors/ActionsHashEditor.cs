using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
#if DLAB_XRM_ENTITIES
using System.Windows.Forms;
using DLaB.XrmToolBoxCommon.Forms;
#endif

// ReSharper disable once CheckNamespace
namespace DLaB.XrmToolBoxCommon.Editors
{
#if !DLAB_XRM_ENTITIES
    [Obsolete("Requires DLAB_XRM_ENTITIES", true)]
#endif
    public class ActionsHashEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var set = (HashSet<string>) value ?? new HashSet<string>();
#if DLAB_XRM_ENTITIES
            if (!(context?.Instance is IGetPluginControl getter))
            {
                throw new InvalidOperationException("Context Instance did not implement IGetPluginControl.  Unable to determine plugin to connect with.");
            }
            using (var dialog = new SpecifyActionsDialog(getter.GetPluginControl()) {SpecifiedActions = set})
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    set = dialog.SpecifiedActions;
                }

            }
#endif
            return set; // can also replace the wrapper object here
        }
    }
}
