using System;
using System.Linq;
using System.Windows.Forms;
using Source.DLaB.Common;

namespace EarlyBoundSettingsGenerator.SettingsUpdater
{
    public partial class AddSettingForm : Form
    {
        #region Properties

        public string Type
        {
            get => TypeCmb.SelectedItem.ToString();
            set => TypeCmb.SelectedIndex = TypeCmb.FindStringExact(value);
        }

        public string DefaultValue
        {
            get => DefaultValueTxt.Text;
            set => DefaultValueTxt.Text = value;
        }

        public string PropertyName
        {
            get => NameTxt.Text;
            set => NameTxt.Text = value;
        }

        public string DisplayName
        {
            get => DisplayNameTxt.Text;
            set => DisplayNameTxt.Text = value;
        }

        public string Category
        {
            get => CategoryCmb.SelectedItem.ToString();
            set => CategoryCmb.SelectedIndex = CategoryCmb.FindStringExact(value);
        }

        public string Description
        {
            get => DescriptionTxt.Text;
            set => DescriptionTxt.Text = value;
        }

        #endregion Properties

        public AddSettingForm()
        {
            InitializeComponent();
        }

        private void AddSetting_Click(object sender, EventArgs e)
        {
            var requiredValues = new[]
            {
                new { Ctrl = (Control)TypeCmb,        Value = Type },
                new { Ctrl = (Control)DefaultValueTxt,Value = DefaultValue },
                new { Ctrl = (Control)NameTxt,        Value = PropertyName },
                new { Ctrl = (Control)DisplayNameTxt, Value = DisplayName },
                new { Ctrl = (Control)CategoryCmb,    Value = Category },
                new { Ctrl = (Control)DescriptionTxt, Value = Description }
            };
            if (requiredValues.Any(v => MissingValue(v.Ctrl, v.Value)))
            {
                return;
            }

            var info = new PropertyInfo
            {
                Type = Type,
                DefaultValue = DefaultValue,
                Name = PropertyName,
                DisplayName = DisplayName,
                Category = Category,
                Description = Description,
            };

            new ExtensionConfigLogic(info).UpdateFile();
            new EarlyBoundGeneratorConfigLogic(info).UpdateFile();
            new SettingsMap(info).UpdateFile();
            new ModelBuilderSettings(info).UpdateFile();
            new TestDefaultBuilderSettingsJson(info).UpdateFile();

            MessageBox.Show("Finished!", "Property Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool MissingValue(Control ctrl, string value)
        {
            var missing = string.IsNullOrWhiteSpace(value);
            if (missing)
            {
                ctrl.Select();
            }

            return missing;
        }

        private void TypeCmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (Type)
            {
                case "string":
                    DefaultValue = "null";
                    break;
                case "bool":
                    DefaultValue = "false";
                    break;
            }
        }

        private void NameTxt_Leave(object sender, EventArgs e)
        {
            DisplayName = PropertyName.SpaceOutCamelCase();
        }
    }
}
