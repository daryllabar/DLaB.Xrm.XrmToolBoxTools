using System.ComponentModel;
using System.Windows.Forms;

namespace DLaB.AttributeManager
{
    partial class AttributeManagerPlugin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AttributeManagerPlugin));
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabStringAttribute = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.pStringType = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.strAttTxtMaximumLength = new System.Windows.Forms.TextBox();
            this.strAttLblFormat = new System.Windows.Forms.Label();
            this.strAttCmbFormat = new System.Windows.Forms.ComboBox();
            this.tabNumberAttribute = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.pNumberType = new System.Windows.Forms.Panel();
            this.tabOptionSetAttribute = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.pOptionType = new System.Windows.Forms.Panel();
            this.tabLog = new System.Windows.Forms.TabPage();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.strAttCmbImeMode = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            this.toolStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.grpSettings.SuspendLayout();
            this.grpSteps.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabStringAttribute.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.pStringType.SuspendLayout();
            this.tabNumberAttribute.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tabOptionSetAttribute.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tabLog.SuspendLayout();
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
            this.cmbNewAttributeType.SelectedIndexChanged += new System.EventHandler(this.cmbNewAttributeType_SelectedIndexChanged);
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
            this.txtLog.Size = new System.Drawing.Size(676, 227);
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
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabStringAttribute);
            this.tabControl.Controls.Add(this.tabNumberAttribute);
            this.tabControl.Controls.Add(this.tabOptionSetAttribute);
            this.tabControl.Controls.Add(this.tabLog);
            this.tabControl.Location = new System.Drawing.Point(16, 309);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(690, 259);
            this.tabControl.TabIndex = 11;
            // 
            // tabStringAttribute
            // 
            this.tabStringAttribute.Controls.Add(this.tableLayoutPanel2);
            this.tabStringAttribute.Location = new System.Drawing.Point(4, 22);
            this.tabStringAttribute.Name = "tabStringAttribute";
            this.tabStringAttribute.Padding = new System.Windows.Forms.Padding(3);
            this.tabStringAttribute.Size = new System.Drawing.Size(682, 233);
            this.tabStringAttribute.TabIndex = 1;
            this.tabStringAttribute.Text = "String Type Settings";
            this.tabStringAttribute.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 538F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.pStringType, 1, 1);
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
            // pStringType
            // 
            this.pStringType.Controls.Add(this.label6);
            this.pStringType.Controls.Add(this.strAttCmbImeMode);
            this.pStringType.Controls.Add(this.label5);
            this.pStringType.Controls.Add(this.strAttTxtMaximumLength);
            this.pStringType.Controls.Add(this.strAttLblFormat);
            this.pStringType.Controls.Add(this.strAttCmbFormat);
            this.pStringType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pStringType.Location = new System.Drawing.Point(72, 41);
            this.pStringType.Name = "pStringType";
            this.pStringType.Size = new System.Drawing.Size(532, 144);
            this.pStringType.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(54, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(90, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Maximum Length:";
            // 
            // strAttTxtMaximumLength
            // 
            this.strAttTxtMaximumLength.Location = new System.Drawing.Point(151, 39);
            this.strAttTxtMaximumLength.Name = "strAttTxtMaximumLength";
            this.strAttTxtMaximumLength.Size = new System.Drawing.Size(171, 20);
            this.strAttTxtMaximumLength.TabIndex = 2;
            // 
            // strAttLblFormat
            // 
            this.strAttLblFormat.AutoSize = true;
            this.strAttLblFormat.Location = new System.Drawing.Point(103, 19);
            this.strAttLblFormat.Name = "strAttLblFormat";
            this.strAttLblFormat.Size = new System.Drawing.Size(42, 13);
            this.strAttLblFormat.TabIndex = 1;
            this.strAttLblFormat.Text = "Format:";
            // 
            // strAttCmbFormat
            // 
            this.strAttCmbFormat.FormattingEnabled = true;
            this.strAttCmbFormat.Items.AddRange(new object[] {
            "Email",
            "Phone",
            "PhoneticGuide",
            "Text",
            "TextArea",
            "TickerSymbol",
            "Url",
            "VersionNumber"});
            this.strAttCmbFormat.Location = new System.Drawing.Point(151, 16);
            this.strAttCmbFormat.Name = "strAttCmbFormat";
            this.strAttCmbFormat.Size = new System.Drawing.Size(171, 21);
            this.strAttCmbFormat.TabIndex = 0;
            // 
            // tabNumberAttribute
            // 
            this.tabNumberAttribute.Controls.Add(this.tableLayoutPanel3);
            this.tabNumberAttribute.Location = new System.Drawing.Point(4, 22);
            this.tabNumberAttribute.Name = "tabNumberAttribute";
            this.tabNumberAttribute.Size = new System.Drawing.Size(682, 233);
            this.tabNumberAttribute.TabIndex = 2;
            this.tabNumberAttribute.Text = "Number Type Settings";
            this.tabNumberAttribute.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 538F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.pNumberType, 1, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(682, 233);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // pNumberType
            // 
            this.pNumberType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pNumberType.Location = new System.Drawing.Point(75, 44);
            this.pNumberType.Name = "pNumberType";
            this.pNumberType.Size = new System.Drawing.Size(532, 144);
            this.pNumberType.TabIndex = 0;
            // 
            // tabOptionSetAttribute
            // 
            this.tabOptionSetAttribute.Controls.Add(this.tableLayoutPanel4);
            this.tabOptionSetAttribute.Location = new System.Drawing.Point(4, 22);
            this.tabOptionSetAttribute.Name = "tabOptionSetAttribute";
            this.tabOptionSetAttribute.Size = new System.Drawing.Size(682, 233);
            this.tabOptionSetAttribute.TabIndex = 3;
            this.tabOptionSetAttribute.Text = "Option Type Settings";
            this.tabOptionSetAttribute.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 3;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 538F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.pOptionType, 1, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(682, 233);
            this.tableLayoutPanel4.TabIndex = 2;
            // 
            // pOptionType
            // 
            this.pOptionType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pOptionType.Location = new System.Drawing.Point(75, 44);
            this.pOptionType.Name = "pOptionType";
            this.pOptionType.Size = new System.Drawing.Size(532, 144);
            this.pOptionType.TabIndex = 0;
            // 
            // tabLog
            // 
            this.tabLog.Controls.Add(this.txtLog);
            this.tabLog.Location = new System.Drawing.Point(4, 22);
            this.tabLog.Name = "tabLog";
            this.tabLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabLog.Size = new System.Drawing.Size(682, 233);
            this.tabLog.TabIndex = 0;
            this.tabLog.Text = "Log";
            this.tabLog.UseVisualStyleBackColor = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // strAttCmbImeMode
            // 
            this.strAttCmbImeMode.FormattingEnabled = true;
            this.strAttCmbImeMode.Items.AddRange(new object[] {
            "Auto",
            "Inactive",
            "Active",
            "Disabled"});
            this.strAttCmbImeMode.Location = new System.Drawing.Point(151, 65);
            this.strAttCmbImeMode.Name = "strAttCmbImeMode";
            this.strAttCmbImeMode.Size = new System.Drawing.Size(171, 21);
            this.strAttCmbImeMode.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(86, 68);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "IME Mode:";
            // 
            // AttributeManagerPlugin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.cmbNewAttribute);
            this.Controls.Add(this.lblNewAttribute);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.cmbAttributes);
            this.Controls.Add(label2);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.btnLoadEntities);
            this.Controls.Add(this.cmbEntities);
            this.Controls.Add(label1);
            this.Name = "AttributeManagerPlugin";
            this.Size = new System.Drawing.Size(724, 571);
            this.Load += new System.EventHandler(this.AttributeManager_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.grpSettings.ResumeLayout(false);
            this.grpSettings.PerformLayout();
            this.grpSteps.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabStringAttribute.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.pStringType.ResumeLayout(false);
            this.pStringType.PerformLayout();
            this.tabNumberAttribute.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tabOptionSetAttribute.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tabLog.ResumeLayout(false);
            this.tabLog.PerformLayout();
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
        private TabControl tabControl;
        private TabPage tabLog;
        private TabPage tabStringAttribute;
        private TableLayoutPanel tableLayoutPanel2;
        private TabPage tabNumberAttribute;
        private TabPage tabOptionSetAttribute;
        private Panel pStringType;
        private TableLayoutPanel tableLayoutPanel3;
        private Panel pNumberType;
        private TableLayoutPanel tableLayoutPanel4;
        private Panel pOptionType;
        private Label strAttLblFormat;
        private ComboBox strAttCmbFormat;
        private Label label5;
        private TextBox strAttTxtMaximumLength;
        private ContextMenuStrip contextMenuStrip1;
        private Label label6;
        private ComboBox strAttCmbImeMode;
    }
}
