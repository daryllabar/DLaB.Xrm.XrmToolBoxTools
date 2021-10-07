using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DLaB.XrmToolBoxCommon.Forms
{
    public partial class DictionaryDialog : Form
    {
        public Dictionary<string, string> Mapping { get; set; }

        public DictionaryDialog()
        {
            InitializeComponent();
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            var local = new Dictionary<string, string>();
            var commaStandIn = Guid.NewGuid().ToString();
            var lines = MainTxt.Text.Replace(",,", commaStandIn).Split(new [] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length > 0)
            {
                foreach(var line in lines.Where(l => !string.IsNullOrWhiteSpace(l)))
                {
                    string error = null;
                    var parts = line.Split(',');
                    if (parts.Length != 2)
                    {
                        error = $"Unable to parse line: {line.Replace(commaStandIn, ",")}!";
                    }

                    var key = parts[0].Replace(commaStandIn, ",").Trim();
                    var value = parts[1].Replace(commaStandIn, ",").Trim();
                    if (local.ContainsKey(key))
                    {
                        error = $"Duplicate key \"{key}\" found!  Please remove the duplicate key!";
                    }
                    if(error == null)
                    {
                        local[key] = value;
                    }
                    else
                    {
                        MessageBox.Show(error + Environment.NewLine + @"Please update the text and try again.", @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                }
            }

            Mapping = local;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void DictionaryDialog_Load(object sender, EventArgs e)
        {
            Mapping = Mapping ?? new Dictionary<string, string>();
            MainTxt.Text = string.Join(Environment.NewLine, Mapping.Select(kvp => EscapeCommas(kvp.Key).Trim() + ", " + EscapeCommas(kvp.Value).Trim()));
        }

        private static string EscapeCommas(string value)
        {
            return value.Replace(",", ",,");
        }
    }
}
