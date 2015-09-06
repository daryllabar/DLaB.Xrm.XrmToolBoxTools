using System.ComponentModel;
using System.Windows.Forms;

namespace DLaB.AttributeManager
{
    partial class AttributeManager
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AttributeManager));
            this.lblSchemaName = new System.Windows.Forms.Label();
            this.cmbEntities = new System.Windows.Forms.ComboBox();
            this.btnLoadEntities = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.cmbAttributes = new System.Windows.Forms.ComboBox();
            this.clbSteps = new System.Windows.Forms.CheckedListBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.grpSettings = new System.Windows.Forms.GroupBox();
            this.chkConvertAttributeType = new System.Windows.Forms.CheckBox();
            this.lblNewAttributeType = new System.Windows.Forms.Label();
            this.cmbNewAttributeType = new System.Windows.Forms.ComboBox();
            this.txtDisplayName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtNewAttributeName = new System.Windows.Forms.TextBox();
            this.chkMigrate = new System.Windows.Forms.CheckBox();
            this.btnExecuteSteps = new System.Windows.Forms.Button();
            this.grpSteps = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.Tip = new System.Windows.Forms.ToolTip(this.components);
            this.cmbNewAttribute = new System.Windows.Forms.ComboBox();
            this.lblNewAttribute = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabAttributeType = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            this.toolStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.grpSettings.SuspendLayout();
            this.grpSteps.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabAttributeType.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(13, 33);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(36, 13);
            label1.TabIndex = 0;
            label1.Text = "Entity:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(13, 60);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(49, 13);
            label2.TabIndex = 4;
            label2.Text = "Attribute:";
            // 
            // lblSchemaName
            // 
            this.lblSchemaName.AutoSize = true;
            this.lblSchemaName.Location = new System.Drawing.Point(6, 47);
            this.lblSchemaName.Name = "lblSchemaName";
            this.lblSchemaName.Size = new System.Drawing.Size(105, 13);
            this.lblSchemaName.TabIndex = 16;
            this.lblSchemaName.Text = "New Schema Name:";
            // 
            // cmbEntities
            // 
            this.cmbEntities.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbEntities.DisplayMember = "DisplayName";
            this.cmbEntities.FormattingEnabled = true;
            this.cmbEntities.Location = new System.Drawing.Point(93, 30);
            this.cmbEntities.Name = "cmbEntities";
            this.cmbEntities.Size = new System.Drawing.Size(509, 21);
            this.cmbEntities.TabIndex = 1;
            this.cmbEntities.ValueMember = "LogicalName";
            this.cmbEntities.SelectedIndexChanged += new System.EventHandler(this.cmbEntities_SelectedIndexChanged);
            this.cmbEntities.Leave += new System.EventHandler(this.cmbEntities_Leave);
            // 
            // btnLoadEntities
            // 
            this.btnLoadEntities.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadEntities.Location = new System.Drawing.Point(608, 28);
            this.btnLoadEntities.Name = "btnLoadEntities";
            this.btnLoadEntities.Size = new System.Drawing.Size(98, 23);
            this.btnLoadEntities.TabIndex = 2;
            this.btnLoadEntities.Text = "Retrieve Entities";
            this.btnLoadEntities.UseVisualStyleBackColor = true;
            this.btnLoadEntities.Click += new System.EventHandler(this.btnLoadEntities_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbClose});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(724, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbClose
            // 
            this.tsbClose.Image = ((System.Drawing.Image)(resources.GetObject("tsbClose.Image")));
            this.tsbClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(56, 22);
            this.tsbClose.Text = "Close";
            this.tsbClose.ToolTipText = "Close this tab";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            // 
            // cmbAttributes
            // 
            this.cmbAttributes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbAttributes.DisplayMember = "DisplayName";
            this.cmbAttributes.FormattingEnabled = true;
            this.cmbAttributes.Location = new System.Drawing.Point(93, 57);
            this.cmbAttributes.Name = "cmbAttributes";
            this.cmbAttributes.Size = new System.Drawing.Size(613, 21);
            this.cmbAttributes.TabIndex = 5;
            this.cmbAttributes.ValueMember = "LogicalName";
            this.cmbAttributes.SelectedIndexChanged += new System.EventHandler(this.cmbAttributes_SelectedIndexChanged);
            this.cmbAttributes.MouseEnter += new System.EventHandler(this.cmbAttributes_MouseEnter);
            // 
            // clbSteps
            // 
            this.clbSteps.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clbSteps.CheckOnClick = true;
            this.clbSteps.FormattingEnabled = true;
            this.clbSteps.Items.AddRange(new object[] {
            "Create Temporary Attribute",
            "Migrate to Temporary Attribute",
            "Remove Existing Attribute",
            "Create New Attribute",
            "Migrate to New Attribute",
            "Remove Temporary Attribute"});
            this.clbSteps.Location = new System.Drawing.Point(8, 19);
            this.clbSteps.Name = "clbSteps";
            this.clbSteps.Size = new System.Drawing.Size(205, 109);
            this.clbSteps.TabIndex = 6;
            this.Tip.SetToolTip(this.clbSteps, "Allows stopping and the migation process at any point");
            this.clbSteps.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbSteps_ItemCheck);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 538F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(16, 125);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(690, 178);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.grpSettings);
            this.panel1.Controls.Add(this.btnExecuteSteps);
            this.panel1.Controls.Add(this.grpSteps);
            this.panel1.Location = new System.Drawing.Point(79, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(532, 172);
            this.panel1.TabIndex = 9;
            // 
            // grpSettings
            // 
            this.grpSettings.Controls.Add(this.chkConvertAttributeType);
            this.grpSettings.Controls.Add(this.lblNewAttributeType);
            this.grpSettings.Controls.Add(this.cmbNewAttributeType);
            this.grpSettings.Controls.Add(this.txtDisplayName);
            this.grpSettings.Controls.Add(this.label3);
            this.grpSettings.Controls.Add(this.txtNewAttributeName);
            this.grpSettings.Controls.Add(this.lblSchemaName);
            this.grpSettings.Controls.Add(this.chkMigrate);
            this.grpSettings.Location = new System.Drawing.Point(3, 3);
            this.grpSettings.Name = "grpSettings";
            this.grpSettings.Size = new System.Drawing.Size(301, 137);
            this.grpSettings.TabIndex = 8;
            this.grpSettings.TabStop = false;
            this.grpSettings.Text = "Settings";
            // 
            // chkConvertAttributeType
            // 
            this.chkConvertAttributeType.AutoSize = true;
            this.chkConvertAttributeType.Location = new System.Drawing.Point(148, 19);
            this.chkConvertAttributeType.Name = "chkConvertAttributeType";
            this.chkConvertAttributeType.Size = new System.Drawing.Size(132, 17);
            this.chkConvertAttributeType.TabIndex = 22;
            this.chkConvertAttributeType.Text = "Convert Attribute Type";
            this.Tip.SetToolTip(this.chkConvertAttributeType, "Copy the value for each entity to the new Option Set field?");
            this.chkConvertAttributeType.UseVisualStyleBackColor = true;
            this.chkConvertAttributeType.CheckedChanged += new System.EventHandler(this.chkConvertAttributeType_CheckedChanged);
            // 
            // lblNewAttributeType
            // 
            this.lblNewAttributeType.AutoSize = true;
            this.lblNewAttributeType.Location = new System.Drawing.Point(6, 99);
            this.lblNewAttributeType.Name = "lblNewAttributeType";
            this.lblNewAttributeType.Size = new System.Drawing.Size(101, 13);
            this.lblNewAttributeType.TabIndex = 21;
            this.lblNewAttributeType.Text = "New Attribute Type:";
            // 
            // cmbNewAttributeType
            // 
            this.cmbNewAttributeType.FormattingEnabled = true;
            this.cmbNewAttributeType.Items.AddRange(new object[] {
            "Single Line of Text",
            "Option Set",
            "Two Options",
            "Image",
            "Whole Number",
            "Floating Point Number",
            "Decimal Number",
            "Currency",
            "Multiple Lines of Text",
            "Date and Time",
            "Lookup"});
            this.cmbNewAttributeType.Location = new System.Drawing.Point(148, 96);
            this.cmbNewAttributeType.Name = "cmbNewAttributeType";
            this.cmbNewAttributeType.Size = new System.Drawing.Size(147, 21);
            this.cmbNewAttributeType.TabIndex = 20;
            // 
            // txtDisplayName
            // 
            this.txtDisplayName.Location = new System.Drawing.Point(148, 70);
            this.txtDisplayName.Name = "txtDisplayName";
            this.txtDisplayName.Size = new System.Drawing.Size(147, 20);
            this.txtDisplayName.TabIndex = 19;
            this.Tip.SetToolTip(this.txtDisplayName, "Postfix to append to the Option Set Value Name to create an unsued temporary name" +
        "");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Display Name:";
            // 
            // txtNewAttributeName
            // 
            this.txtNewAttributeName.Location = new System.Drawing.Point(148, 44);
            this.txtNewAttributeName.Name = "txtNewAttributeName";
            this.txtNewAttributeName.Size = new System.Drawing.Size(147, 20);
            this.txtNewAttributeName.TabIndex = 17;
            this.Tip.SetToolTip(this.txtNewAttributeName, "Postfix to append to the Option Set Value Name to create an unsued temporary name" +
        "");
            this.txtNewAttributeName.TextChanged += new System.EventHandler(this.txtNewAttributeName_TextChanged);
            // 
            // chkMigrate
            // 
            this.chkMigrate.AutoSize = true;
            this.chkMigrate.Checked = true;
            this.chkMigrate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMigrate.Location = new System.Drawing.Point(6, 19);
            this.chkMigrate.Name = "chkMigrate";
            this.chkMigrate.Size = new System.Drawing.Size(87, 17);
            this.chkMigrate.TabIndex = 10;
            this.chkMigrate.Text = "Migrate Data";
            this.Tip.SetToolTip(this.chkMigrate, "Copy the value for each entity to the new Option Set field?");
            this.chkMigrate.UseVisualStyleBackColor = true;
            // 
            // btnExecuteSteps
            // 
            this.btnExecuteSteps.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecuteSteps.Location = new System.Drawing.Point(181, 146);
            this.btnExecuteSteps.Name = "btnExecuteSteps";
            this.btnExecuteSteps.Size = new System.Drawing.Size(177, 20);
            this.btnExecuteSteps.TabIndex = 8;
            this.btnExecuteSteps.Text = "Execute Steps";
            this.btnExecuteSteps.UseVisualStyleBackColor = true;
            this.btnExecuteSteps.Click += new System.EventHandler(this.btnExecuteSteps_Click);
            // 
            // grpSteps
            // 
            this.grpSteps.Controls.Add(this.clbSteps);
            this.grpSteps.Location = new System.Drawing.Point(310, 3);
            this.grpSteps.Name = "grpSteps";
            this.grpSteps.Size = new System.Drawing.Size(219, 137);
            this.grpSteps.TabIndex = 7;
            this.grpSteps.TabStop = false;
            this.grpSteps.Text = "Steps";
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(3, 3);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(676, 104);
            this.txtLog.TabIndex = 0;
            // 
            // cmbNewAttribute
            // 
            this.cmbNewAttribute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbNewAttribute.DisplayMember = "DisplayName";
            this.cmbNewAttribute.FormattingEnabled = true;
            this.cmbNewAttribute.Location = new System.Drawing.Point(93, 84);
            this.cmbNewAttribute.Name = "cmbNewAttribute";
            this.cmbNewAttribute.Size = new System.Drawing.Size(613, 21);
            this.cmbNewAttribute.TabIndex = 10;
            this.cmbNewAttribute.ValueMember = "LogicalName";
            this.cmbNewAttribute.SelectedIndexChanged += new System.EventHandler(this.cmbNewAttribute_SelectedIndexChanged);
            // 
            // lblNewAttribute
            // 
            this.lblNewAttribute.AutoSize = true;
            this.lblNewAttribute.Location = new System.Drawing.Point(13, 87);
            this.lblNewAttribute.Name = "lblNewAttribute";
            this.lblNewAttribute.Size = new System.Drawing.Size(74, 13);
            this.lblNewAttribute.TabIndex = 9;
            this.lblNewAttribute.Text = "New Attribute:";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabAttributeType);
            this.tabControl1.Location = new System.Drawing.Point(16, 309);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(690, 259);
            this.tabControl1.TabIndex = 11;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.txtLog);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(682, 110);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Log";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabAttributeType
            // 
            this.tabAttributeType.Controls.Add(this.tableLayoutPanel2);
            this.tabAttributeType.Location = new System.Drawing.Point(4, 22);
            this.tabAttributeType.Name = "tabAttributeType";
            this.tabAttributeType.Padding = new System.Windows.Forms.Padding(3);
            this.tabAttributeType.Size = new System.Drawing.Size(682, 233);
            this.tabAttributeType.TabIndex = 1;
            this.tabAttributeType.Text = "Attribute Type Settings";
            this.tabAttributeType.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 538F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.panel2, 1, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(676, 227);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(72, 41);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(532, 144);
            this.panel2.TabIndex = 0;
            // 
            // AttributeManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.cmbNewAttribute);
            this.Controls.Add(this.lblNewAttribute);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.cmbAttributes);
            this.Controls.Add(label2);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.btnLoadEntities);
            this.Controls.Add(this.cmbEntities);
            this.Controls.Add(label1);
            this.Name = "AttributeManager";
            this.Size = new System.Drawing.Size(724, 571);
            this.Load += new System.EventHandler(this.AttributeManager_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.grpSettings.ResumeLayout(false);
            this.grpSettings.PerformLayout();
            this.grpSteps.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabAttributeType.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComboBox cmbEntities;
        private Button btnLoadEntities;
        private ToolStrip toolStrip1;
        private ToolStripButton tsbClose;
        private ComboBox cmbAttributes;
        private CheckedListBox clbSteps;
        private Button btnExecuteSteps;
        private TableLayoutPanel tableLayoutPanel1;
        private Panel panel1;
        private TextBox txtLog;
        private GroupBox grpSettings;
        private CheckBox chkMigrate;
        private GroupBox grpSteps;
        private ToolTip Tip;
        private TextBox txtNewAttributeName;
        private ComboBox cmbNewAttribute;
        private Label lblNewAttribute;
        private Label lblSchemaName;
        private TextBox txtDisplayName;
        private Label label3;
        private CheckBox chkConvertAttributeType;
        private Label lblNewAttributeType;
        private ComboBox cmbNewAttributeType;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabAttributeType;
        private TableLayoutPanel tableLayoutPanel2;
        private Panel panel2;
    }
}
