using System;
using System.Windows.Forms;

namespace DLaB.AttributeManager
{
    public static class Extensions
    {
        public static void LoadItems(this ComboBox cmb, Object[] items)
        {
            cmb.BeginUpdate();
            cmb.Items.Clear();

            cmb.Items.AddRange(items);

            cmb.EndUpdate();
        }

        public static void LoadItems(this CheckedListBox listBox, Object[] items)
        {
            listBox.BeginUpdate();
            listBox.Items.Clear();

            listBox.Items.AddRange(items);

            listBox.EndUpdate();
        }
    }
}
