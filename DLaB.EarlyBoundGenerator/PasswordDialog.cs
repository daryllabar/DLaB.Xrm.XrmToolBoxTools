using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DLaB.XrmToolBoxCommon;
using XrmToolBox.Extensibility;

namespace DLaB.EarlyBoundGenerator
{
    public partial class PasswordDialog : DialogBase
    {
        public string Password { get; set; }

        #region Constructors

        public PasswordDialog()
        {
            InitializeComponent();
        }

        public PasswordDialog(PluginControlBase callingControl) : base(callingControl)
        {
            InitializeComponent();
        }

        #endregion Constructors

        private void BttnOk_Click(object sender, EventArgs e)
        {
            Password = TxtPassword.Text;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
