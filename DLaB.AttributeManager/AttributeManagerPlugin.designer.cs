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
            System.Windows.Forms.Label label9;
            System.Windows.Forms.Label label10;
            System.Windows.Forms.Label label11;
            System.Windows.Forms.Label label13;
            System.Windows.Forms.Label label7;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AttributeManagerPlugin));
            this.LblOptionSetDescription = new System.Windows.Forms.Label();
            this.lblSchemaName = new System.Windows.Forms.Label();
            this.cmbEntities = new System.Windows.Forms.ComboBox();
            this.btnLoadEntities = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.cmbAttributes = new System.Windows.Forms.ComboBox();
            this.clbSteps = new System.Windows.Forms.CheckedListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.grpSettings = new System.Windows.Forms.GroupBox();
            this.chkIgnoreUpdateErrors = new System.Windows.Forms.CheckBox();
            this.btnMappingFile = new System.Windows.Forms.Button();
            this.lblMappingFile = new System.Windows.Forms.Label();
            this.txtMappingFile = new System.Windows.Forms.TextBox();
            this.chkDelete = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblNewAttributeGrid = new System.Windows.Forms.Label();
            this.txtOldSchema = new System.Windows.Forms.TextBox();
            this.cmbNewAttributeType = new System.Windows.Forms.ComboBox();
            this.txtDisplayName = new System.Windows.Forms.TextBox();
            this.txtNewAttributeName = new System.Windows.Forms.TextBox();
            this.txtOldAttributType = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.txtOldDisplay = new System.Windows.Forms.TextBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.lblNewAttributeType = new System.Windows.Forms.Label();
            this.chkConvertAttributeType = new System.Windows.Forms.CheckBox();
            this.chkMigrate = new System.Windows.Forms.CheckBox();
            this.grpSteps = new System.Windows.Forms.GroupBox();
            this.btnExecuteSteps = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.Tip = new System.Windows.Forms.ToolTip(this.components);
            this.TxtOptionSetLabel = new System.Windows.Forms.TextBox();
            this.TxtOptionSetValue = new System.Windows.Forms.TextBox();
            this.TxtOptionSetDescription = new System.Windows.Forms.TextBox();
            this.TxtOptionSetColor = new System.Windows.Forms.TextBox();
            this.cmbNewAttribute = new System.Windows.Forms.ComboBox();
            this.lblNewAttribute = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabStringAttribute = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.pStringType = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.strAttCmbImeMode = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.strAttTxtMaximumLength = new System.Windows.Forms.TextBox();
            this.strAttLblFormat = new System.Windows.Forms.Label();
            this.strAttCmbFormat = new System.Windows.Forms.ComboBox();
            this.tabNumberAttribute = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.pNumberType = new System.Windows.Forms.Panel();
            this.numAttCurrencyPrecisionLbl = new System.Windows.Forms.Label();
            this.numAttCurrencyPrecisionCmb = new System.Windows.Forms.ComboBox();
            this.numAttPrecisionLbl = new System.Windows.Forms.Label();
            this.numAttPrecisionTxt = new System.Windows.Forms.TextBox();
            this.numAttMaxLbl = new System.Windows.Forms.Label();
            this.numAttMaxTxt = new System.Windows.Forms.TextBox();
            this.numAttMinLbl = new System.Windows.Forms.Label();
            this.numAttMinTxt = new System.Windows.Forms.TextBox();
            this.numAttFormatLbl = new System.Windows.Forms.Label();
            this.numAttFormatCmb = new System.Windows.Forms.ComboBox();
            this.tabOptionSetAttribute = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.PnlLocalOptionSet = new System.Windows.Forms.Panel();
            this.CmbPublisher = new System.Windows.Forms.ComboBox();
            this.PicAttLocalOptionSortAsc = new System.Windows.Forms.PictureBox();
            this.PicAttLocalOptionSortDesc = new System.Windows.Forms.PictureBox();
            this.PicAttLocalOptionMoveDown = new System.Windows.Forms.PictureBox();
            this.PicAttLocalOptionMoveUp = new System.Windows.Forms.PictureBox();
            this.PicAttLocalOptionDelete = new System.Windows.Forms.PictureBox();
            this.PicAttLocalOptionAdd = new System.Windows.Forms.PictureBox();
            this.CmbAttLocalOptionSet = new System.Windows.Forms.ComboBox();
            this.pOptionType = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.optAttDefaultValueCmb = new System.Windows.Forms.ComboBox();
            this.optAttGlobalOptionSetLbl = new System.Windows.Forms.Label();
            this.optAttGlobalOptionSetCmb = new System.Windows.Forms.ComboBox();
            this.tabLog = new System.Windows.Forms.TabPage();
            this.tabDelete = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.pDelete = new System.Windows.Forms.Panel();
            this.delUpdatePlugins = new System.Windows.Forms.CheckBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            label13 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.grpSettings.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            this.grpSteps.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabStringAttribute.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.pStringType.SuspendLayout();
            this.tabNumberAttribute.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.pNumberType.SuspendLayout();
            this.tabOptionSetAttribute.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.PnlLocalOptionSet.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicAttLocalOptionSortAsc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicAttLocalOptionSortDesc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicAttLocalOptionMoveDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicAttLocalOptionMoveUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicAttLocalOptionDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicAttLocalOptionAdd)).BeginInit();
            this.pOptionType.SuspendLayout();
            this.tabLog.SuspendLayout();
            this.tabDelete.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.pDelete.SuspendLayout();
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
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(68, 0);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(60, 13);
            label9.TabIndex = 13;
            label9.Text = "Option Set:";
            // 
            // label10
            // 
            label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(580, 33);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(36, 13);
            label10.TabIndex = 24;
            label10.Text = "Label:";
            // 
            // label11
            // 
            label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(580, 59);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(37, 13);
            label11.TabIndex = 25;
            label11.Text = "Value:";
            // 
            // label13
            // 
            label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            label13.AutoSize = true;
            label13.Location = new System.Drawing.Point(563, 6);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(53, 13);
            label13.TabIndex = 31;
            label13.Text = "Publisher:";
            // 
            // label7
            // 
            label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(579, 85);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(34, 13);
            label7.TabIndex = 33;
            label7.Text = "Color:";
            // 
            // LblOptionSetDescription
            // 
            this.LblOptionSetDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LblOptionSetDescription.AutoSize = true;
            this.LblOptionSetDescription.Location = new System.Drawing.Point(554, 111);
            this.LblOptionSetDescription.Name = "LblOptionSetDescription";
            this.LblOptionSetDescription.Size = new System.Drawing.Size(63, 13);
            this.LblOptionSetDescription.TabIndex = 26;
            this.LblOptionSetDescription.Text = "Description:";
            // 
            // lblSchemaName
            // 
            this.lblSchemaName.AutoSize = true;
            this.lblSchemaName.Location = new System.Drawing.Point(3, 3);
            this.lblSchemaName.Name = "lblSchemaName";
            this.lblSchemaName.Size = new System.Drawing.Size(80, 13);
            this.lblSchemaName.TabIndex = 16;
            this.lblSchemaName.Text = "Schema Name:";
            // 
            // cmbEntities
            // 
            this.cmbEntities.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbEntities.DisplayMember = "DisplayName";
            this.cmbEntities.Enabled = false;
            this.cmbEntities.FormattingEnabled = true;
            this.cmbEntities.Location = new System.Drawing.Point(93, 30);
            this.cmbEntities.Name = "cmbEntities";
            this.cmbEntities.Size = new System.Drawing.Size(662, 21);
            this.cmbEntities.TabIndex = 1;
            this.cmbEntities.ValueMember = "LogicalName";
            this.cmbEntities.SelectedIndexChanged += new System.EventHandler(this.cmbEntities_SelectedIndexChanged);
            this.cmbEntities.Leave += new System.EventHandler(this.cmbEntities_Leave);
            // 
            // btnLoadEntities
            // 
            this.btnLoadEntities.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadEntities.Location = new System.Drawing.Point(761, 28);
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
            this.toolStrip1.Size = new System.Drawing.Size(877, 25);
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
            this.cmbAttributes.Enabled = false;
            this.cmbAttributes.FormattingEnabled = true;
            this.cmbAttributes.Location = new System.Drawing.Point(93, 57);
            this.cmbAttributes.Name = "cmbAttributes";
            this.cmbAttributes.Size = new System.Drawing.Size(766, 21);
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
            this.clbSteps.Size = new System.Drawing.Size(205, 154);
            this.clbSteps.TabIndex = 6;
            this.Tip.SetToolTip(this.clbSteps, "Allows stopping and the migation process at any point");
            this.clbSteps.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbSteps_ItemCheck);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.grpSettings);
            this.panel1.Controls.Add(this.grpSteps);
            this.panel1.Location = new System.Drawing.Point(16, 111);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(843, 219);
            this.panel1.TabIndex = 9;
            // 
            // grpSettings
            // 
            this.grpSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSettings.Controls.Add(this.chkIgnoreUpdateErrors);
            this.grpSettings.Controls.Add(this.btnMappingFile);
            this.grpSettings.Controls.Add(this.lblMappingFile);
            this.grpSettings.Controls.Add(this.txtMappingFile);
            this.grpSettings.Controls.Add(this.chkDelete);
            this.grpSettings.Controls.Add(this.tableLayoutPanel1);
            this.grpSettings.Controls.Add(this.chkConvertAttributeType);
            this.grpSettings.Controls.Add(this.chkMigrate);
            this.grpSettings.Location = new System.Drawing.Point(3, 3);
            this.grpSettings.Name = "grpSettings";
            this.grpSettings.Size = new System.Drawing.Size(612, 208);
            this.grpSettings.TabIndex = 8;
            this.grpSettings.TabStop = false;
            this.grpSettings.Text = "Settings";
            // 
            // chkIgnoreUpdateErrors
            // 
            this.chkIgnoreUpdateErrors.AutoSize = true;
            this.chkIgnoreUpdateErrors.Location = new System.Drawing.Point(482, 13);
            this.chkIgnoreUpdateErrors.Name = "chkIgnoreUpdateErrors";
            this.chkIgnoreUpdateErrors.Size = new System.Drawing.Size(124, 17);
            this.chkIgnoreUpdateErrors.TabIndex = 25;
            this.chkIgnoreUpdateErrors.Text = "Ignore Update Errors";
            this.chkIgnoreUpdateErrors.UseVisualStyleBackColor = true;
            this.chkIgnoreUpdateErrors.CheckedChanged += new System.EventHandler(this.chkIgnoreUpdateErrors_CheckedChanged);
            // 
            // btnMappingFile
            // 
            this.btnMappingFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMappingFile.Location = new System.Drawing.Point(583, 35);
            this.btnMappingFile.Name = "btnMappingFile";
            this.btnMappingFile.Size = new System.Drawing.Size(26, 23);
            this.btnMappingFile.TabIndex = 29;
            this.btnMappingFile.Text = "...";
            this.btnMappingFile.UseVisualStyleBackColor = true;
            this.btnMappingFile.Visible = false;
            this.btnMappingFile.Click += new System.EventHandler(this.btnMappingFile_Click);
            // 
            // lblMappingFile
            // 
            this.lblMappingFile.AutoSize = true;
            this.lblMappingFile.Location = new System.Drawing.Point(-3, 39);
            this.lblMappingFile.Name = "lblMappingFile";
            this.lblMappingFile.Size = new System.Drawing.Size(67, 13);
            this.lblMappingFile.TabIndex = 28;
            this.lblMappingFile.Text = "Mapping File";
            this.lblMappingFile.Visible = false;
            // 
            // txtMappingFile
            // 
            this.txtMappingFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMappingFile.Location = new System.Drawing.Point(74, 36);
            this.txtMappingFile.Name = "txtMappingFile";
            this.txtMappingFile.Size = new System.Drawing.Size(503, 20);
            this.txtMappingFile.TabIndex = 27;
            this.txtMappingFile.Visible = false;
            // 
            // chkDelete
            // 
            this.chkDelete.AutoSize = true;
            this.chkDelete.Location = new System.Drawing.Point(6, 13);
            this.chkDelete.Name = "chkDelete";
            this.chkDelete.Size = new System.Drawing.Size(99, 17);
            this.chkDelete.TabIndex = 26;
            this.chkDelete.Text = "Delete Attribute";
            this.Tip.SetToolTip(this.chkDelete, "Copy the value for each entity to the new Option Set field?");
            this.chkDelete.UseVisualStyleBackColor = true;
            this.chkDelete.CheckedChanged += new System.EventHandler(this.chkDelete_CheckedChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 87F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.panel3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtOldSchema, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.cmbNewAttributeType, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.txtDisplayName, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtNewAttributeName, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtOldAttributType, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtOldDisplay, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel4, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel5, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel6, 0, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 81);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(606, 121);
            this.tableLayoutPanel1.TabIndex = 25;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lblNewAttributeGrid);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(349, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(254, 24);
            this.panel3.TabIndex = 28;
            // 
            // lblNewAttributeGrid
            // 
            this.lblNewAttributeGrid.AutoSize = true;
            this.lblNewAttributeGrid.Location = new System.Drawing.Point(0, 11);
            this.lblNewAttributeGrid.Name = "lblNewAttributeGrid";
            this.lblNewAttributeGrid.Size = new System.Drawing.Size(32, 13);
            this.lblNewAttributeGrid.TabIndex = 24;
            this.lblNewAttributeGrid.Text = "New:";
            // 
            // txtOldSchema
            // 
            this.txtOldSchema.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOldSchema.Enabled = false;
            this.txtOldSchema.Location = new System.Drawing.Point(90, 33);
            this.txtOldSchema.Name = "txtOldSchema";
            this.txtOldSchema.Size = new System.Drawing.Size(253, 20);
            this.txtOldSchema.TabIndex = 24;
            this.Tip.SetToolTip(this.txtOldSchema, "Postfix to append to the Option Set Value Name to create an unsued temporary name" +
        "");
            // 
            // cmbNewAttributeType
            // 
            this.cmbNewAttributeType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbNewAttributeType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNewAttributeType.FormattingEnabled = true;
            this.cmbNewAttributeType.Items.AddRange(new object[] {
            "Single Line of Text",
            "Global Option Set",
            "Local Option Set",
            "Two Options",
            "Image",
            "Whole Number",
            "Floating Point Number",
            "Decimal Number",
            "Currency",
            "Multiple Lines of Text",
            "Date and Time",
            "Lookup"});
            this.cmbNewAttributeType.Location = new System.Drawing.Point(349, 93);
            this.cmbNewAttributeType.Name = "cmbNewAttributeType";
            this.cmbNewAttributeType.Size = new System.Drawing.Size(254, 21);
            this.cmbNewAttributeType.TabIndex = 20;
            this.cmbNewAttributeType.SelectedIndexChanged += new System.EventHandler(this.cmbNewAttributeType_SelectedIndexChanged);
            // 
            // txtDisplayName
            // 
            this.txtDisplayName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDisplayName.Location = new System.Drawing.Point(349, 63);
            this.txtDisplayName.Name = "txtDisplayName";
            this.txtDisplayName.Size = new System.Drawing.Size(254, 20);
            this.txtDisplayName.TabIndex = 19;
            this.Tip.SetToolTip(this.txtDisplayName, "Postfix to append to the Option Set Value Name to create an unsued temporary name" +
        "");
            // 
            // txtNewAttributeName
            // 
            this.txtNewAttributeName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNewAttributeName.Location = new System.Drawing.Point(349, 33);
            this.txtNewAttributeName.Name = "txtNewAttributeName";
            this.txtNewAttributeName.Size = new System.Drawing.Size(254, 20);
            this.txtNewAttributeName.TabIndex = 17;
            this.Tip.SetToolTip(this.txtNewAttributeName, "Postfix to append to the Option Set Value Name to create an unsued temporary name" +
        "");
            this.txtNewAttributeName.TextChanged += new System.EventHandler(this.txtNewAttributeName_TextChanged);
            // 
            // txtOldAttributType
            // 
            this.txtOldAttributType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOldAttributType.Enabled = false;
            this.txtOldAttributType.Location = new System.Drawing.Point(90, 93);
            this.txtOldAttributType.Name = "txtOldAttributType";
            this.txtOldAttributType.Size = new System.Drawing.Size(253, 20);
            this.txtOldAttributType.TabIndex = 26;
            this.Tip.SetToolTip(this.txtOldAttributType, "Postfix to append to the Option Set Value Name to create an unsued temporary name" +
        "");
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(90, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(253, 24);
            this.panel2.TabIndex = 27;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 23;
            this.label4.Text = "Current:";
            // 
            // txtOldDisplay
            // 
            this.txtOldDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOldDisplay.Enabled = false;
            this.txtOldDisplay.Location = new System.Drawing.Point(90, 63);
            this.txtOldDisplay.Name = "txtOldDisplay";
            this.txtOldDisplay.Size = new System.Drawing.Size(253, 20);
            this.txtOldDisplay.TabIndex = 25;
            this.Tip.SetToolTip(this.txtOldDisplay, "Postfix to append to the Option Set Value Name to create an unsued temporary name" +
        "");
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.lblSchemaName);
            this.panel4.Location = new System.Drawing.Point(3, 33);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(81, 24);
            this.panel4.TabIndex = 28;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label3);
            this.panel5.Location = new System.Drawing.Point(3, 63);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(81, 24);
            this.panel5.TabIndex = 29;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Display Name:";
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.lblNewAttributeType);
            this.panel6.Location = new System.Drawing.Point(3, 93);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(81, 24);
            this.panel6.TabIndex = 28;
            // 
            // lblNewAttributeType
            // 
            this.lblNewAttributeType.AutoSize = true;
            this.lblNewAttributeType.Location = new System.Drawing.Point(5, 3);
            this.lblNewAttributeType.Name = "lblNewAttributeType";
            this.lblNewAttributeType.Size = new System.Drawing.Size(76, 13);
            this.lblNewAttributeType.TabIndex = 21;
            this.lblNewAttributeType.Text = "Attribute Type:";
            // 
            // chkConvertAttributeType
            // 
            this.chkConvertAttributeType.AutoSize = true;
            this.chkConvertAttributeType.Location = new System.Drawing.Point(305, 13);
            this.chkConvertAttributeType.Name = "chkConvertAttributeType";
            this.chkConvertAttributeType.Size = new System.Drawing.Size(132, 17);
            this.chkConvertAttributeType.TabIndex = 22;
            this.chkConvertAttributeType.Text = "Convert Attribute Type";
            this.Tip.SetToolTip(this.chkConvertAttributeType, "Copy the value for each entity to the new Option Set field?");
            this.chkConvertAttributeType.UseVisualStyleBackColor = true;
            this.chkConvertAttributeType.CheckedChanged += new System.EventHandler(this.chkConvertAttributeType_CheckedChanged);
            // 
            // chkMigrate
            // 
            this.chkMigrate.AutoSize = true;
            this.chkMigrate.Checked = true;
            this.chkMigrate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMigrate.Location = new System.Drawing.Point(175, 13);
            this.chkMigrate.Name = "chkMigrate";
            this.chkMigrate.Size = new System.Drawing.Size(87, 17);
            this.chkMigrate.TabIndex = 10;
            this.chkMigrate.Text = "Migrate Data";
            this.Tip.SetToolTip(this.chkMigrate, "Copy the value for each entity to the new Option Set field?");
            this.chkMigrate.UseVisualStyleBackColor = true;
            this.chkMigrate.CheckedChanged += new System.EventHandler(this.chkMigrate_CheckedChanged);
            // 
            // grpSteps
            // 
            this.grpSteps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSteps.Controls.Add(this.clbSteps);
            this.grpSteps.Controls.Add(this.btnExecuteSteps);
            this.grpSteps.Location = new System.Drawing.Point(621, 3);
            this.grpSteps.Name = "grpSteps";
            this.grpSteps.Size = new System.Drawing.Size(219, 208);
            this.grpSteps.TabIndex = 7;
            this.grpSteps.TabStop = false;
            this.grpSteps.Text = "Steps";
            // 
            // btnExecuteSteps
            // 
            this.btnExecuteSteps.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecuteSteps.Location = new System.Drawing.Point(8, 179);
            this.btnExecuteSteps.Name = "btnExecuteSteps";
            this.btnExecuteSteps.Size = new System.Drawing.Size(205, 23);
            this.btnExecuteSteps.TabIndex = 8;
            this.btnExecuteSteps.Text = "Execute Steps";
            this.btnExecuteSteps.UseVisualStyleBackColor = true;
            this.btnExecuteSteps.Click += new System.EventHandler(this.btnExecuteSteps_Click);
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLog.Location = new System.Drawing.Point(3, 3);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(829, 249);
            this.txtLog.TabIndex = 0;
            this.txtLog.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SelectAllKeyDownHandler);
            // 
            // TxtOptionSetLabel
            // 
            this.TxtOptionSetLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtOptionSetLabel.Location = new System.Drawing.Point(622, 30);
            this.TxtOptionSetLabel.Name = "TxtOptionSetLabel";
            this.TxtOptionSetLabel.Size = new System.Drawing.Size(201, 20);
            this.TxtOptionSetLabel.TabIndex = 27;
            this.Tip.SetToolTip(this.TxtOptionSetLabel, "Postfix to append to the Option Set Value Name to create an unsued temporary name" +
        "");
            this.TxtOptionSetLabel.TextChanged += new System.EventHandler(this.TxtOptionSetLabel_TextChanged);
            // 
            // TxtOptionSetValue
            // 
            this.TxtOptionSetValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtOptionSetValue.Location = new System.Drawing.Point(622, 56);
            this.TxtOptionSetValue.Name = "TxtOptionSetValue";
            this.TxtOptionSetValue.Size = new System.Drawing.Size(201, 20);
            this.TxtOptionSetValue.TabIndex = 28;
            this.Tip.SetToolTip(this.TxtOptionSetValue, "Postfix to append to the Option Set Value Name to create an unsued temporary name" +
        "");
            this.TxtOptionSetValue.TextChanged += new System.EventHandler(this.TxtOptionSetValue_TextChanged);
            // 
            // TxtOptionSetDescription
            // 
            this.TxtOptionSetDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtOptionSetDescription.Location = new System.Drawing.Point(622, 108);
            this.TxtOptionSetDescription.Multiline = true;
            this.TxtOptionSetDescription.Name = "TxtOptionSetDescription";
            this.TxtOptionSetDescription.Size = new System.Drawing.Size(201, 50);
            this.TxtOptionSetDescription.TabIndex = 29;
            this.Tip.SetToolTip(this.TxtOptionSetDescription, "Postfix to append to the Option Set Value Name to create an unsued temporary name" +
        "");
            this.TxtOptionSetDescription.TextChanged += new System.EventHandler(this.TxtOptionSetDescription_TextChanged);
            // 
            // TxtOptionSetColor
            // 
            this.TxtOptionSetColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtOptionSetColor.Location = new System.Drawing.Point(622, 82);
            this.TxtOptionSetColor.Name = "TxtOptionSetColor";
            this.TxtOptionSetColor.Size = new System.Drawing.Size(201, 20);
            this.TxtOptionSetColor.TabIndex = 32;
            this.TxtOptionSetColor.Text = "#000ff";
            this.Tip.SetToolTip(this.TxtOptionSetColor, "Postfix to append to the Option Set Value Name to create an unsued temporary name" +
        "");
            this.TxtOptionSetColor.TextChanged += new System.EventHandler(this.TxtOptionSetColor_TextChanged);
            // 
            // cmbNewAttribute
            // 
            this.cmbNewAttribute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbNewAttribute.DisplayMember = "DisplayName";
            this.cmbNewAttribute.FormattingEnabled = true;
            this.cmbNewAttribute.Location = new System.Drawing.Point(93, 84);
            this.cmbNewAttribute.Name = "cmbNewAttribute";
            this.cmbNewAttribute.Size = new System.Drawing.Size(766, 21);
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
            this.tabControl.Controls.Add(this.tabDelete);
            this.tabControl.Location = new System.Drawing.Point(13, 336);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(843, 281);
            this.tabControl.TabIndex = 11;
            // 
            // tabStringAttribute
            // 
            this.tabStringAttribute.Controls.Add(this.tableLayoutPanel2);
            this.tabStringAttribute.Location = new System.Drawing.Point(4, 22);
            this.tabStringAttribute.Name = "tabStringAttribute";
            this.tabStringAttribute.Padding = new System.Windows.Forms.Padding(3);
            this.tabStringAttribute.Size = new System.Drawing.Size(835, 255);
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
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(826, 266);
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
            this.pStringType.Location = new System.Drawing.Point(147, 36);
            this.pStringType.Name = "pStringType";
            this.pStringType.Size = new System.Drawing.Size(532, 194);
            this.pStringType.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(35, 52);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "IME Mode:";
            // 
            // strAttCmbImeMode
            // 
            this.strAttCmbImeMode.FormattingEnabled = true;
            this.strAttCmbImeMode.Items.AddRange(new object[] {
            "Auto",
            "Inactive",
            "Active",
            "Disabled"});
            this.strAttCmbImeMode.Location = new System.Drawing.Point(102, 49);
            this.strAttCmbImeMode.Name = "strAttCmbImeMode";
            this.strAttCmbImeMode.Size = new System.Drawing.Size(171, 21);
            this.strAttCmbImeMode.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 29);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(90, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Maximum Length:";
            // 
            // strAttTxtMaximumLength
            // 
            this.strAttTxtMaximumLength.Location = new System.Drawing.Point(102, 26);
            this.strAttTxtMaximumLength.Name = "strAttTxtMaximumLength";
            this.strAttTxtMaximumLength.Size = new System.Drawing.Size(171, 20);
            this.strAttTxtMaximumLength.TabIndex = 2;
            // 
            // strAttLblFormat
            // 
            this.strAttLblFormat.AutoSize = true;
            this.strAttLblFormat.Location = new System.Drawing.Point(53, 6);
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
            this.strAttCmbFormat.Location = new System.Drawing.Point(102, 3);
            this.strAttCmbFormat.Name = "strAttCmbFormat";
            this.strAttCmbFormat.Size = new System.Drawing.Size(171, 21);
            this.strAttCmbFormat.TabIndex = 0;
            // 
            // tabNumberAttribute
            // 
            this.tabNumberAttribute.Controls.Add(this.tableLayoutPanel3);
            this.tabNumberAttribute.Location = new System.Drawing.Point(4, 22);
            this.tabNumberAttribute.Name = "tabNumberAttribute";
            this.tabNumberAttribute.Size = new System.Drawing.Size(835, 255);
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
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(832, 272);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // pNumberType
            // 
            this.pNumberType.Controls.Add(this.numAttCurrencyPrecisionLbl);
            this.pNumberType.Controls.Add(this.numAttCurrencyPrecisionCmb);
            this.pNumberType.Controls.Add(this.numAttPrecisionLbl);
            this.pNumberType.Controls.Add(this.numAttPrecisionTxt);
            this.pNumberType.Controls.Add(this.numAttMaxLbl);
            this.pNumberType.Controls.Add(this.numAttMaxTxt);
            this.pNumberType.Controls.Add(this.numAttMinLbl);
            this.pNumberType.Controls.Add(this.numAttMinTxt);
            this.pNumberType.Controls.Add(this.numAttFormatLbl);
            this.pNumberType.Controls.Add(this.numAttFormatCmb);
            this.pNumberType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pNumberType.Location = new System.Drawing.Point(150, 39);
            this.pNumberType.Name = "pNumberType";
            this.pNumberType.Size = new System.Drawing.Size(532, 194);
            this.pNumberType.TabIndex = 0;
            // 
            // numAttCurrencyPrecisionLbl
            // 
            this.numAttCurrencyPrecisionLbl.AutoSize = true;
            this.numAttCurrencyPrecisionLbl.Location = new System.Drawing.Point(41, 105);
            this.numAttCurrencyPrecisionLbl.Name = "numAttCurrencyPrecisionLbl";
            this.numAttCurrencyPrecisionLbl.Size = new System.Drawing.Size(53, 13);
            this.numAttCurrencyPrecisionLbl.TabIndex = 15;
            this.numAttCurrencyPrecisionLbl.Text = "Precision:";
            // 
            // numAttCurrencyPrecisionCmb
            // 
            this.numAttCurrencyPrecisionCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.numAttCurrencyPrecisionCmb.FormattingEnabled = true;
            this.numAttCurrencyPrecisionCmb.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4"});
            this.numAttCurrencyPrecisionCmb.Location = new System.Drawing.Point(109, 102);
            this.numAttCurrencyPrecisionCmb.Name = "numAttCurrencyPrecisionCmb";
            this.numAttCurrencyPrecisionCmb.Size = new System.Drawing.Size(171, 21);
            this.numAttCurrencyPrecisionCmb.TabIndex = 14;
            // 
            // numAttPrecisionLbl
            // 
            this.numAttPrecisionLbl.AutoSize = true;
            this.numAttPrecisionLbl.Location = new System.Drawing.Point(41, 80);
            this.numAttPrecisionLbl.Name = "numAttPrecisionLbl";
            this.numAttPrecisionLbl.Size = new System.Drawing.Size(53, 13);
            this.numAttPrecisionLbl.TabIndex = 13;
            this.numAttPrecisionLbl.Text = "Precision:";
            // 
            // numAttPrecisionTxt
            // 
            this.numAttPrecisionTxt.Location = new System.Drawing.Point(109, 77);
            this.numAttPrecisionTxt.Name = "numAttPrecisionTxt";
            this.numAttPrecisionTxt.Size = new System.Drawing.Size(171, 20);
            this.numAttPrecisionTxt.TabIndex = 12;
            // 
            // numAttMaxLbl
            // 
            this.numAttMaxLbl.AutoSize = true;
            this.numAttMaxLbl.Location = new System.Drawing.Point(6, 55);
            this.numAttMaxLbl.Name = "numAttMaxLbl";
            this.numAttMaxLbl.Size = new System.Drawing.Size(90, 13);
            this.numAttMaxLbl.TabIndex = 11;
            this.numAttMaxLbl.Text = "Maximum Length:";
            // 
            // numAttMaxTxt
            // 
            this.numAttMaxTxt.Location = new System.Drawing.Point(109, 52);
            this.numAttMaxTxt.Name = "numAttMaxTxt";
            this.numAttMaxTxt.Size = new System.Drawing.Size(171, 20);
            this.numAttMaxTxt.TabIndex = 10;
            this.numAttMaxTxt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RemoveNonNumber_KeyPress);
            // 
            // numAttMinLbl
            // 
            this.numAttMinLbl.AutoSize = true;
            this.numAttMinLbl.Location = new System.Drawing.Point(7, 29);
            this.numAttMinLbl.Name = "numAttMinLbl";
            this.numAttMinLbl.Size = new System.Drawing.Size(87, 13);
            this.numAttMinLbl.TabIndex = 9;
            this.numAttMinLbl.Text = "Minimum Length:";
            // 
            // numAttMinTxt
            // 
            this.numAttMinTxt.Location = new System.Drawing.Point(109, 26);
            this.numAttMinTxt.Name = "numAttMinTxt";
            this.numAttMinTxt.Size = new System.Drawing.Size(171, 20);
            this.numAttMinTxt.TabIndex = 8;
            this.numAttMinTxt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RemoveNonNumber_KeyPress);
            // 
            // numAttFormatLbl
            // 
            this.numAttFormatLbl.AutoSize = true;
            this.numAttFormatLbl.Location = new System.Drawing.Point(52, 6);
            this.numAttFormatLbl.Name = "numAttFormatLbl";
            this.numAttFormatLbl.Size = new System.Drawing.Size(42, 13);
            this.numAttFormatLbl.TabIndex = 7;
            this.numAttFormatLbl.Text = "Format:";
            // 
            // numAttFormatCmb
            // 
            this.numAttFormatCmb.FormattingEnabled = true;
            this.numAttFormatCmb.Items.AddRange(new object[] {
            "None",
            "Duration",
            "TimeZone",
            "Language",
            "Locale"});
            this.numAttFormatCmb.Location = new System.Drawing.Point(109, 3);
            this.numAttFormatCmb.Name = "numAttFormatCmb";
            this.numAttFormatCmb.Size = new System.Drawing.Size(171, 21);
            this.numAttFormatCmb.TabIndex = 6;
            // 
            // tabOptionSetAttribute
            // 
            this.tabOptionSetAttribute.Controls.Add(this.tableLayoutPanel4);
            this.tabOptionSetAttribute.Location = new System.Drawing.Point(4, 22);
            this.tabOptionSetAttribute.Name = "tabOptionSetAttribute";
            this.tabOptionSetAttribute.Size = new System.Drawing.Size(835, 255);
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
            this.tableLayoutPanel4.Controls.Add(this.PnlLocalOptionSet, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.pOptionType, 1, 1);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(832, 272);
            this.tableLayoutPanel4.TabIndex = 2;
            // 
            // PnlLocalOptionSet
            // 
            this.tableLayoutPanel4.SetColumnSpan(this.PnlLocalOptionSet, 3);
            this.PnlLocalOptionSet.Controls.Add(label7);
            this.PnlLocalOptionSet.Controls.Add(this.TxtOptionSetColor);
            this.PnlLocalOptionSet.Controls.Add(label13);
            this.PnlLocalOptionSet.Controls.Add(this.CmbPublisher);
            this.PnlLocalOptionSet.Controls.Add(this.TxtOptionSetDescription);
            this.PnlLocalOptionSet.Controls.Add(this.TxtOptionSetValue);
            this.PnlLocalOptionSet.Controls.Add(this.TxtOptionSetLabel);
            this.PnlLocalOptionSet.Controls.Add(this.LblOptionSetDescription);
            this.PnlLocalOptionSet.Controls.Add(label11);
            this.PnlLocalOptionSet.Controls.Add(label10);
            this.PnlLocalOptionSet.Controls.Add(this.PicAttLocalOptionSortAsc);
            this.PnlLocalOptionSet.Controls.Add(this.PicAttLocalOptionSortDesc);
            this.PnlLocalOptionSet.Controls.Add(this.PicAttLocalOptionMoveDown);
            this.PnlLocalOptionSet.Controls.Add(this.PicAttLocalOptionMoveUp);
            this.PnlLocalOptionSet.Controls.Add(this.PicAttLocalOptionDelete);
            this.PnlLocalOptionSet.Controls.Add(this.PicAttLocalOptionAdd);
            this.PnlLocalOptionSet.Controls.Add(this.CmbAttLocalOptionSet);
            this.PnlLocalOptionSet.Controls.Add(label9);
            this.PnlLocalOptionSet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PnlLocalOptionSet.Location = new System.Drawing.Point(3, 83);
            this.PnlLocalOptionSet.Name = "PnlLocalOptionSet";
            this.PnlLocalOptionSet.Size = new System.Drawing.Size(826, 186);
            this.PnlLocalOptionSet.TabIndex = 12;
            // 
            // CmbPublisher
            // 
            this.CmbPublisher.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CmbPublisher.DisplayMember = "DisplayName";
            this.CmbPublisher.FormattingEnabled = true;
            this.CmbPublisher.Location = new System.Drawing.Point(622, 3);
            this.CmbPublisher.Name = "CmbPublisher";
            this.CmbPublisher.Size = new System.Drawing.Size(201, 21);
            this.CmbPublisher.TabIndex = 30;
            this.CmbPublisher.ValueMember = "Value";
            this.CmbPublisher.SelectedIndexChanged += new System.EventHandler(this.CmbPublisher_SelectedIndexChanged);
            // 
            // PicAttLocalOptionSortAsc
            // 
            this.PicAttLocalOptionSortAsc.Image = ((System.Drawing.Image)(resources.GetObject("PicAttLocalOptionSortAsc.Image")));
            this.PicAttLocalOptionSortAsc.Location = new System.Drawing.Point(181, 20);
            this.PicAttLocalOptionSortAsc.Name = "PicAttLocalOptionSortAsc";
            this.PicAttLocalOptionSortAsc.Size = new System.Drawing.Size(16, 16);
            this.PicAttLocalOptionSortAsc.TabIndex = 23;
            this.PicAttLocalOptionSortAsc.TabStop = false;
            this.PicAttLocalOptionSortAsc.Click += new System.EventHandler(this.PicAttLocalOptionSortAsc_Click);
            this.PicAttLocalOptionSortAsc.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ImageButton_MouseDown);
            this.PicAttLocalOptionSortAsc.MouseEnter += new System.EventHandler(this.ImageButton_MouseEnter);
            this.PicAttLocalOptionSortAsc.MouseLeave += new System.EventHandler(this.ImageButton_MouseLeave);
            this.PicAttLocalOptionSortAsc.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ImageButton_MouseUp);
            // 
            // PicAttLocalOptionSortDesc
            // 
            this.PicAttLocalOptionSortDesc.Image = ((System.Drawing.Image)(resources.GetObject("PicAttLocalOptionSortDesc.Image")));
            this.PicAttLocalOptionSortDesc.Location = new System.Drawing.Point(159, 20);
            this.PicAttLocalOptionSortDesc.Name = "PicAttLocalOptionSortDesc";
            this.PicAttLocalOptionSortDesc.Size = new System.Drawing.Size(16, 16);
            this.PicAttLocalOptionSortDesc.TabIndex = 22;
            this.PicAttLocalOptionSortDesc.TabStop = false;
            this.PicAttLocalOptionSortDesc.Click += new System.EventHandler(this.PicAttLocalOptionSortDesc_Click);
            this.PicAttLocalOptionSortDesc.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ImageButton_MouseDown);
            this.PicAttLocalOptionSortDesc.MouseEnter += new System.EventHandler(this.ImageButton_MouseEnter);
            this.PicAttLocalOptionSortDesc.MouseLeave += new System.EventHandler(this.ImageButton_MouseLeave);
            this.PicAttLocalOptionSortDesc.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ImageButton_MouseUp);
            // 
            // PicAttLocalOptionMoveDown
            // 
            this.PicAttLocalOptionMoveDown.Image = ((System.Drawing.Image)(resources.GetObject("PicAttLocalOptionMoveDown.Image")));
            this.PicAttLocalOptionMoveDown.Location = new System.Drawing.Point(137, 20);
            this.PicAttLocalOptionMoveDown.Name = "PicAttLocalOptionMoveDown";
            this.PicAttLocalOptionMoveDown.Size = new System.Drawing.Size(16, 16);
            this.PicAttLocalOptionMoveDown.TabIndex = 21;
            this.PicAttLocalOptionMoveDown.TabStop = false;
            this.PicAttLocalOptionMoveDown.Click += new System.EventHandler(this.PicAttLocalOptionMoveDown_Click);
            this.PicAttLocalOptionMoveDown.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ImageButton_MouseDown);
            this.PicAttLocalOptionMoveDown.MouseEnter += new System.EventHandler(this.ImageButton_MouseEnter);
            this.PicAttLocalOptionMoveDown.MouseLeave += new System.EventHandler(this.ImageButton_MouseLeave);
            this.PicAttLocalOptionMoveDown.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ImageButton_MouseUp);
            // 
            // PicAttLocalOptionMoveUp
            // 
            this.PicAttLocalOptionMoveUp.Image = ((System.Drawing.Image)(resources.GetObject("PicAttLocalOptionMoveUp.Image")));
            this.PicAttLocalOptionMoveUp.Location = new System.Drawing.Point(115, 20);
            this.PicAttLocalOptionMoveUp.Name = "PicAttLocalOptionMoveUp";
            this.PicAttLocalOptionMoveUp.Size = new System.Drawing.Size(16, 16);
            this.PicAttLocalOptionMoveUp.TabIndex = 20;
            this.PicAttLocalOptionMoveUp.TabStop = false;
            this.PicAttLocalOptionMoveUp.Click += new System.EventHandler(this.PicAttLocalOptionMoveUp_Click);
            this.PicAttLocalOptionMoveUp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ImageButton_MouseDown);
            this.PicAttLocalOptionMoveUp.MouseEnter += new System.EventHandler(this.ImageButton_MouseEnter);
            this.PicAttLocalOptionMoveUp.MouseLeave += new System.EventHandler(this.ImageButton_MouseLeave);
            this.PicAttLocalOptionMoveUp.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ImageButton_MouseUp);
            // 
            // PicAttLocalOptionDelete
            // 
            this.PicAttLocalOptionDelete.Image = ((System.Drawing.Image)(resources.GetObject("PicAttLocalOptionDelete.Image")));
            this.PicAttLocalOptionDelete.Location = new System.Drawing.Point(93, 20);
            this.PicAttLocalOptionDelete.Name = "PicAttLocalOptionDelete";
            this.PicAttLocalOptionDelete.Size = new System.Drawing.Size(16, 16);
            this.PicAttLocalOptionDelete.TabIndex = 19;
            this.PicAttLocalOptionDelete.TabStop = false;
            this.PicAttLocalOptionDelete.Click += new System.EventHandler(this.PicAttLocalOptionDelete_Click);
            this.PicAttLocalOptionDelete.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ImageButton_MouseDown);
            this.PicAttLocalOptionDelete.MouseEnter += new System.EventHandler(this.ImageButton_MouseEnter);
            this.PicAttLocalOptionDelete.MouseLeave += new System.EventHandler(this.ImageButton_MouseLeave);
            this.PicAttLocalOptionDelete.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ImageButton_MouseUp);
            // 
            // PicAttLocalOptionAdd
            // 
            this.PicAttLocalOptionAdd.Image = ((System.Drawing.Image)(resources.GetObject("PicAttLocalOptionAdd.Image")));
            this.PicAttLocalOptionAdd.Location = new System.Drawing.Point(71, 20);
            this.PicAttLocalOptionAdd.Name = "PicAttLocalOptionAdd";
            this.PicAttLocalOptionAdd.Size = new System.Drawing.Size(16, 16);
            this.PicAttLocalOptionAdd.TabIndex = 18;
            this.PicAttLocalOptionAdd.TabStop = false;
            this.PicAttLocalOptionAdd.Click += new System.EventHandler(this.PicAttLocalOptionAdd_Click);
            this.PicAttLocalOptionAdd.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ImageButton_MouseDown);
            this.PicAttLocalOptionAdd.MouseEnter += new System.EventHandler(this.ImageButton_MouseEnter);
            this.PicAttLocalOptionAdd.MouseLeave += new System.EventHandler(this.ImageButton_MouseLeave);
            this.PicAttLocalOptionAdd.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ImageButton_MouseUp);
            // 
            // CmbAttLocalOptionSet
            // 
            this.CmbAttLocalOptionSet.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CmbAttLocalOptionSet.DisplayMember = "DisplayName";
            this.CmbAttLocalOptionSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.CmbAttLocalOptionSet.FormattingEnabled = true;
            this.CmbAttLocalOptionSet.Location = new System.Drawing.Point(71, 42);
            this.CmbAttLocalOptionSet.Name = "CmbAttLocalOptionSet";
            this.CmbAttLocalOptionSet.Size = new System.Drawing.Size(454, 140);
            this.CmbAttLocalOptionSet.TabIndex = 14;
            this.CmbAttLocalOptionSet.ValueMember = "Value";
            this.CmbAttLocalOptionSet.SelectedIndexChanged += new System.EventHandler(this.CmbAttLocalOptionSet_SelectedIndexChanged);
            // 
            // pOptionType
            // 
            this.pOptionType.Controls.Add(this.label8);
            this.pOptionType.Controls.Add(this.optAttDefaultValueCmb);
            this.pOptionType.Controls.Add(this.optAttGlobalOptionSetLbl);
            this.pOptionType.Controls.Add(this.optAttGlobalOptionSetCmb);
            this.pOptionType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pOptionType.Location = new System.Drawing.Point(150, 23);
            this.pOptionType.Name = "pOptionType";
            this.pOptionType.Size = new System.Drawing.Size(532, 54);
            this.pOptionType.TabIndex = 0;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 33);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(74, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Default Value:";
            // 
            // optAttDefaultValueCmb
            // 
            this.optAttDefaultValueCmb.DisplayMember = "DisplayName";
            this.optAttDefaultValueCmb.FormattingEnabled = true;
            this.optAttDefaultValueCmb.Items.AddRange(new object[] {
            "None",
            "Duration",
            "TimeZone",
            "Language",
            "Locale"});
            this.optAttDefaultValueCmb.Location = new System.Drawing.Point(83, 30);
            this.optAttDefaultValueCmb.Name = "optAttDefaultValueCmb";
            this.optAttDefaultValueCmb.Size = new System.Drawing.Size(171, 21);
            this.optAttDefaultValueCmb.TabIndex = 10;
            this.optAttDefaultValueCmb.ValueMember = "Value";
            // 
            // optAttGlobalOptionSetLbl
            // 
            this.optAttGlobalOptionSetLbl.AutoSize = true;
            this.optAttGlobalOptionSetLbl.Location = new System.Drawing.Point(3, 6);
            this.optAttGlobalOptionSetLbl.Name = "optAttGlobalOptionSetLbl";
            this.optAttGlobalOptionSetLbl.Size = new System.Drawing.Size(60, 13);
            this.optAttGlobalOptionSetLbl.TabIndex = 9;
            this.optAttGlobalOptionSetLbl.Text = "Option Set:";
            // 
            // optAttGlobalOptionSetCmb
            // 
            this.optAttGlobalOptionSetCmb.DisplayMember = "DisplayName";
            this.optAttGlobalOptionSetCmb.FormattingEnabled = true;
            this.optAttGlobalOptionSetCmb.Items.AddRange(new object[] {
            "None",
            "Duration",
            "TimeZone",
            "Language",
            "Locale"});
            this.optAttGlobalOptionSetCmb.Location = new System.Drawing.Point(83, 3);
            this.optAttGlobalOptionSetCmb.Name = "optAttGlobalOptionSetCmb";
            this.optAttGlobalOptionSetCmb.Size = new System.Drawing.Size(171, 21);
            this.optAttGlobalOptionSetCmb.TabIndex = 8;
            this.optAttGlobalOptionSetCmb.ValueMember = "Value";
            this.optAttGlobalOptionSetCmb.SelectionChangeCommitted += new System.EventHandler(this.optAttGlobalOptionSetCmb_SelectionChangeCommitted);
            // 
            // tabLog
            // 
            this.tabLog.Controls.Add(this.txtLog);
            this.tabLog.Location = new System.Drawing.Point(4, 22);
            this.tabLog.Name = "tabLog";
            this.tabLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabLog.Size = new System.Drawing.Size(835, 255);
            this.tabLog.TabIndex = 0;
            this.tabLog.Text = "Log";
            this.tabLog.UseVisualStyleBackColor = true;
            // 
            // tabDelete
            // 
            this.tabDelete.Controls.Add(this.tableLayoutPanel5);
            this.tabDelete.Location = new System.Drawing.Point(4, 22);
            this.tabDelete.Name = "tabDelete";
            this.tabDelete.Padding = new System.Windows.Forms.Padding(3);
            this.tabDelete.Size = new System.Drawing.Size(835, 255);
            this.tabDelete.TabIndex = 4;
            this.tabDelete.Text = "Delete Settings";
            this.tabDelete.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 3;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 538F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Controls.Add(this.pDelete, 1, 1);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 3;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(829, 249);
            this.tableLayoutPanel5.TabIndex = 0;
            // 
            // pDelete
            // 
            this.pDelete.Controls.Add(this.delUpdatePlugins);
            this.pDelete.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pDelete.Location = new System.Drawing.Point(148, 77);
            this.pDelete.Name = "pDelete";
            this.pDelete.Size = new System.Drawing.Size(532, 94);
            this.pDelete.TabIndex = 0;
            // 
            // delUpdatePlugins
            // 
            this.delUpdatePlugins.AutoSize = true;
            this.delUpdatePlugins.Location = new System.Drawing.Point(3, 38);
            this.delUpdatePlugins.Name = "delUpdatePlugins";
            this.delUpdatePlugins.Size = new System.Drawing.Size(210, 17);
            this.delUpdatePlugins.TabIndex = 0;
            this.delUpdatePlugins.Text = "Delete Plugin Registration Associations";
            this.delUpdatePlugins.UseVisualStyleBackColor = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // AttributeManagerPlugin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.cmbNewAttribute);
            this.Controls.Add(this.lblNewAttribute);
            this.Controls.Add(this.cmbAttributes);
            this.Controls.Add(label2);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.btnLoadEntities);
            this.Controls.Add(this.cmbEntities);
            this.Controls.Add(label1);
            this.MinimumSize = new System.Drawing.Size(711, 532);
            this.Name = "AttributeManagerPlugin";
            this.Size = new System.Drawing.Size(877, 620);
            this.Load += new System.EventHandler(this.AttributeManager_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.grpSettings.ResumeLayout(false);
            this.grpSettings.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.grpSteps.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabStringAttribute.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.pStringType.ResumeLayout(false);
            this.pStringType.PerformLayout();
            this.tabNumberAttribute.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.pNumberType.ResumeLayout(false);
            this.pNumberType.PerformLayout();
            this.tabOptionSetAttribute.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.PnlLocalOptionSet.ResumeLayout(false);
            this.PnlLocalOptionSet.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicAttLocalOptionSortAsc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicAttLocalOptionSortDesc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicAttLocalOptionMoveDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicAttLocalOptionMoveUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicAttLocalOptionDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicAttLocalOptionAdd)).EndInit();
            this.pOptionType.ResumeLayout(false);
            this.pOptionType.PerformLayout();
            this.tabLog.ResumeLayout(false);
            this.tabLog.PerformLayout();
            this.tabDelete.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.pDelete.ResumeLayout(false);
            this.pDelete.PerformLayout();
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
        private Label numAttMinLbl;
        private TextBox numAttMinTxt;
        private Label numAttFormatLbl;
        private ComboBox numAttFormatCmb;
        private Label numAttMaxLbl;
        private TextBox numAttMaxTxt;
        private BackgroundWorker backgroundWorker1;
        private TableLayoutPanel tableLayoutPanel1;
        private Panel panel3;
        private Label lblNewAttributeGrid;
        private TextBox txtOldSchema;
        private TextBox txtOldAttributType;
        private Panel panel2;
        private Label label4;
        private TextBox txtOldDisplay;
        private Panel panel4;
        private Panel panel5;
        private Panel panel6;
        private Label optAttGlobalOptionSetLbl;
        private ComboBox optAttGlobalOptionSetCmb;
        private Label label8;
        private ComboBox optAttDefaultValueCmb;
        private Panel PnlLocalOptionSet;
        private ComboBox CmbAttLocalOptionSet;
        private PictureBox PicAttLocalOptionAdd;
        private PictureBox PicAttLocalOptionDelete;
        private PictureBox PicAttLocalOptionSortAsc;
        private PictureBox PicAttLocalOptionSortDesc;
        private PictureBox PicAttLocalOptionMoveDown;
        private PictureBox PicAttLocalOptionMoveUp;
        private ComboBox CmbPublisher;
        private TextBox TxtOptionSetDescription;
        private TextBox TxtOptionSetValue;
        private TextBox TxtOptionSetLabel;
        private CheckBox chkDelete;
        private Button btnMappingFile;
        private Label lblMappingFile;
        private TextBox txtMappingFile;
        private OpenFileDialog openFileDialog1;
        private ColorDialog colorDialog1;
        private TextBox TxtOptionSetColor;
        private Label LblOptionSetDescription;
        private TabPage tabDelete;
        private TableLayoutPanel tableLayoutPanel5;
        private Panel pDelete;
        private CheckBox delUpdatePlugins;
        private CheckBox chkIgnoreUpdateErrors;
        private Label numAttPrecisionLbl;
        private TextBox numAttPrecisionTxt;
        private Label numAttCurrencyPrecisionLbl;
        private ComboBox numAttCurrencyPrecisionCmb;
    }
}
