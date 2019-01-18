using XrmToolBox.Extensibility;

namespace DLaB.EarlyBoundGenerator
{
    partial class EarlyBoundGeneratorPlugin
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            System.Windows.Forms.Label label4;
            this.BtnCreateOptionSets = new System.Windows.Forms.Button();
            this.BtnCreateAll = new System.Windows.Forms.Button();
            this.BtnCreateEntities = new System.Windows.Forms.Button();
            this.TxtOutput = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ChkAudibleCompletion = new System.Windows.Forms.CheckBox();
            this.ChkAddFilesToProject = new System.Windows.Forms.CheckBox();
            this.ChkRemoveRuntimeComment = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TxtNamespace = new System.Windows.Forms.TextBox();
            this.TxtHelp = new System.Windows.Forms.TextBox();
            this.ChkUseTFS = new System.Windows.Forms.CheckBox();
            this.ChkMaskPassword = new System.Windows.Forms.CheckBox();
            this.ChkIncludeCommandLine = new System.Windows.Forms.CheckBox();
            this.BtnOpenSettingsPathDialog = new System.Windows.Forms.Button();
            this.TxtSettingsPath = new System.Windows.Forms.TextBox();
            this.ChkUseXrmClient = new System.Windows.Forms.CheckBox();
            this.BtnEntitesToSkip = new System.Windows.Forms.Button();
            this.ChkCreateOneEntityFile = new System.Windows.Forms.CheckBox();
            this.ChkGenerateOptionSetEnums = new System.Windows.Forms.CheckBox();
            this.BtnOpenOptionSetPathDialog = new System.Windows.Forms.Button();
            this.LblOptionSetsDirectory = new System.Windows.Forms.Label();
            this.TxtOptionSetPath = new System.Windows.Forms.TextBox();
            this.BtnOpenEntityPathDialog = new System.Windows.Forms.Button();
            this.LblEntityPath = new System.Windows.Forms.Label();
            this.TxtEntityPath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TxtServiceContextName = new System.Windows.Forms.TextBox();
            this.BtnUnmappedProperties = new System.Windows.Forms.Button();
            this.BtnEnumMappings = new System.Windows.Forms.Button();
            this.BtnOptionSetsToSkip = new System.Windows.Forms.Button();
            this.BtnSpecifyAttributeNames = new System.Windows.Forms.Button();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.BtnActionsToSkip = new System.Windows.Forms.Button();
            this.BtnOpenActionPathDialog = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.GlobalTab = new System.Windows.Forms.TabPage();
            this.EntityTab = new System.Windows.Forms.TabPage();
            this.ChkGenerateEntityRelationships = new System.Windows.Forms.CheckBox();
            this.ChkMakeReadonlyFieldsEditable = new System.Windows.Forms.CheckBox();
            this.ChkGenerateAttributeNameConsts = new System.Windows.Forms.CheckBox();
            this.ChkGenerateAnonymousTypeConstructor = new System.Windows.Forms.CheckBox();
            this.LblEntitiesDirectory = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ChkAddDebuggerNonUserCode = new System.Windows.Forms.CheckBox();
            this.OptionSetTab = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.TxtLanguageCodeOverride = new System.Windows.Forms.TextBox();
            this.LblOptionSetFormat = new System.Windows.Forms.Label();
            this.TxtOptionSetFormat = new System.Windows.Forms.TextBox();
            this.ChkUseDeprecatedOptionSetNaming = new System.Windows.Forms.CheckBox();
            this.TxtInvalidCSharpNamePrefix = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.LblOptionSetPath = new System.Windows.Forms.Label();
            this.ChkCreateOneOptionSetFile = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.ActionTab = new System.Windows.Forms.TabPage();
            this.ChkGenerateActionAttributeNameConsts = new System.Windows.Forms.CheckBox();
            this.ChkEditableResponseActions = new System.Windows.Forms.CheckBox();
            this.LblActionsDirectory = new System.Windows.Forms.Label();
            this.ChkCreateOneActionFile = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.BtnCreateActions = new System.Windows.Forms.Button();
            this.TxtActionPath = new System.Windows.Forms.TextBox();
            this.LblActionPath = new System.Windows.Forms.Label();
            this.PropertiesGrid = new System.Windows.Forms.PropertyGrid();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.GlobalTab.SuspendLayout();
            this.EntityTab.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.OptionSetTab.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.ActionTab.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(3, 8);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(73, 13);
            label4.TabIndex = 14;
            label4.Text = "Settings Path:";
            // 
            // BtnCreateOptionSets
            // 
            this.BtnCreateOptionSets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnCreateOptionSets.Location = new System.Drawing.Point(152, 0);
            this.BtnCreateOptionSets.Margin = new System.Windows.Forms.Padding(0);
            this.BtnCreateOptionSets.Name = "BtnCreateOptionSets";
            this.BtnCreateOptionSets.Size = new System.Drawing.Size(204, 23);
            this.BtnCreateOptionSets.TabIndex = 1;
            this.BtnCreateOptionSets.Text = "Create OptionSets";
            this.BtnCreateOptionSets.UseVisualStyleBackColor = true;
            this.BtnCreateOptionSets.Click += new System.EventHandler(this.BtnCreateOptionSets_Click);
            // 
            // BtnCreateAll
            // 
            this.BtnCreateAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnCreateAll.Location = new System.Drawing.Point(6, 155);
            this.BtnCreateAll.Name = "BtnCreateAll";
            this.BtnCreateAll.Size = new System.Drawing.Size(219, 21);
            this.BtnCreateAll.TabIndex = 1;
            this.BtnCreateAll.Text = "Create All";
            this.BtnCreateAll.UseVisualStyleBackColor = true;
            this.BtnCreateAll.Click += new System.EventHandler(this.BtnCreateBoth_Click);
            // 
            // BtnCreateEntities
            // 
            this.BtnCreateEntities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnCreateEntities.Location = new System.Drawing.Point(215, 0);
            this.BtnCreateEntities.Margin = new System.Windows.Forms.Padding(0);
            this.BtnCreateEntities.Name = "BtnCreateEntities";
            this.BtnCreateEntities.Size = new System.Drawing.Size(204, 23);
            this.BtnCreateEntities.TabIndex = 0;
            this.BtnCreateEntities.Text = "Create Entities";
            this.BtnCreateEntities.UseVisualStyleBackColor = true;
            this.BtnCreateEntities.Click += new System.EventHandler(this.BtnCreateEntities_Click);
            // 
            // TxtOutput
            // 
            this.TxtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtOutput.Location = new System.Drawing.Point(2, 354);
            this.TxtOutput.Multiline = true;
            this.TxtOutput.Name = "TxtOutput";
            this.TxtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TxtOutput.Size = new System.Drawing.Size(392, 178);
            this.TxtOutput.TabIndex = 1;
            this.TxtOutput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtOutput_KeyDown);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ChkAudibleCompletion);
            this.groupBox1.Controls.Add(this.ChkAddFilesToProject);
            this.groupBox1.Controls.Add(this.ChkRemoveRuntimeComment);
            this.groupBox1.Controls.Add(this.BtnCreateAll);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.TxtNamespace);
            this.groupBox1.Controls.Add(this.TxtHelp);
            this.groupBox1.Controls.Add(this.ChkUseTFS);
            this.groupBox1.Controls.Add(this.ChkMaskPassword);
            this.groupBox1.Controls.Add(this.ChkIncludeCommandLine);
            this.groupBox1.Location = new System.Drawing.Point(10, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(481, 181);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // ChkAudibleCompletion
            // 
            this.ChkAudibleCompletion.AutoSize = true;
            this.ChkAudibleCompletion.Location = new System.Drawing.Point(6, 136);
            this.ChkAudibleCompletion.Name = "ChkAudibleCompletion";
            this.ChkAudibleCompletion.Size = new System.Drawing.Size(194, 17);
            this.ChkAudibleCompletion.TabIndex = 12;
            this.ChkAudibleCompletion.Text = "Use Audible Completion Notification";
            this.ChkAudibleCompletion.UseVisualStyleBackColor = true;
            // 
            // ChkAddFilesToProject
            // 
            this.ChkAddFilesToProject.AutoSize = true;
            this.ChkAddFilesToProject.Location = new System.Drawing.Point(6, 113);
            this.ChkAddFilesToProject.Name = "ChkAddFilesToProject";
            this.ChkAddFilesToProject.Size = new System.Drawing.Size(146, 17);
            this.ChkAddFilesToProject.TabIndex = 11;
            this.ChkAddFilesToProject.Text = "Add New Files To Project";
            this.ChkAddFilesToProject.UseVisualStyleBackColor = true;
            // 
            // ChkRemoveRuntimeComment
            // 
            this.ChkRemoveRuntimeComment.AutoSize = true;
            this.ChkRemoveRuntimeComment.Location = new System.Drawing.Point(6, 67);
            this.ChkRemoveRuntimeComment.Name = "ChkRemoveRuntimeComment";
            this.ChkRemoveRuntimeComment.Size = new System.Drawing.Size(193, 17);
            this.ChkRemoveRuntimeComment.TabIndex = 10;
            this.ChkRemoveRuntimeComment.Text = "Remove Runtime Header Comment";
            this.ChkRemoveRuntimeComment.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(228, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Namespace:";
            // 
            // TxtNamespace
            // 
            this.TxtNamespace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtNamespace.Location = new System.Drawing.Point(301, 41);
            this.TxtNamespace.Name = "TxtNamespace";
            this.TxtNamespace.Size = new System.Drawing.Size(174, 20);
            this.TxtNamespace.TabIndex = 8;
            // 
            // TxtHelp
            // 
            this.TxtHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtHelp.BackColor = System.Drawing.SystemColors.Control;
            this.TxtHelp.Location = new System.Drawing.Point(231, 67);
            this.TxtHelp.Multiline = true;
            this.TxtHelp.Name = "TxtHelp";
            this.TxtHelp.Size = new System.Drawing.Size(244, 109);
            this.TxtHelp.TabIndex = 7;
            // 
            // ChkUseTFS
            // 
            this.ChkUseTFS.AutoSize = true;
            this.ChkUseTFS.Location = new System.Drawing.Point(6, 90);
            this.ChkUseTFS.Name = "ChkUseTFS";
            this.ChkUseTFS.Size = new System.Drawing.Size(158, 17);
            this.ChkUseTFS.TabIndex = 2;
            this.ChkUseTFS.Text = "Use TFS to Check Out Files";
            this.ChkUseTFS.UseVisualStyleBackColor = true;
            // 
            // ChkMaskPassword
            // 
            this.ChkMaskPassword.AutoSize = true;
            this.ChkMaskPassword.Location = new System.Drawing.Point(23, 44);
            this.ChkMaskPassword.Name = "ChkMaskPassword";
            this.ChkMaskPassword.Size = new System.Drawing.Size(101, 17);
            this.ChkMaskPassword.TabIndex = 1;
            this.ChkMaskPassword.Text = "Mask Password";
            this.ChkMaskPassword.UseVisualStyleBackColor = true;
            // 
            // ChkIncludeCommandLine
            // 
            this.ChkIncludeCommandLine.AutoSize = true;
            this.ChkIncludeCommandLine.Location = new System.Drawing.Point(6, 21);
            this.ChkIncludeCommandLine.Name = "ChkIncludeCommandLine";
            this.ChkIncludeCommandLine.Size = new System.Drawing.Size(181, 17);
            this.ChkIncludeCommandLine.TabIndex = 0;
            this.ChkIncludeCommandLine.Text = "Include Command Line In Output";
            this.ChkIncludeCommandLine.UseVisualStyleBackColor = true;
            this.ChkIncludeCommandLine.CheckedChanged += new System.EventHandler(this.ChkIncludeCommandLine_CheckedChanged);
            // 
            // BtnOpenSettingsPathDialog
            // 
            this.BtnOpenSettingsPathDialog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpProvider1.SetHelpString(this.BtnOpenSettingsPathDialog, "Overrides Capitalization of Entity Attributes");
            this.BtnOpenSettingsPathDialog.Location = new System.Drawing.Point(459, 3);
            this.BtnOpenSettingsPathDialog.Name = "BtnOpenSettingsPathDialog";
            this.helpProvider1.SetShowHelp(this.BtnOpenSettingsPathDialog, true);
            this.BtnOpenSettingsPathDialog.Size = new System.Drawing.Size(27, 23);
            this.BtnOpenSettingsPathDialog.TabIndex = 15;
            this.BtnOpenSettingsPathDialog.Text = "...";
            this.BtnOpenSettingsPathDialog.UseVisualStyleBackColor = true;
            this.BtnOpenSettingsPathDialog.Click += new System.EventHandler(this.BtnOpenSettingsPathDialog_Click);
            // 
            // TxtSettingsPath
            // 
            this.TxtSettingsPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtSettingsPath.Location = new System.Drawing.Point(82, 5);
            this.TxtSettingsPath.Name = "TxtSettingsPath";
            this.TxtSettingsPath.Size = new System.Drawing.Size(371, 20);
            this.TxtSettingsPath.TabIndex = 13;
            this.TxtSettingsPath.TextChanged += new System.EventHandler(this.TxtSettingsPath_TextChanged);
            // 
            // ChkUseXrmClient
            // 
            this.ChkUseXrmClient.AutoSize = true;
            this.ChkUseXrmClient.Location = new System.Drawing.Point(164, 102);
            this.ChkUseXrmClient.Name = "ChkUseXrmClient";
            this.ChkUseXrmClient.Size = new System.Drawing.Size(95, 17);
            this.ChkUseXrmClient.TabIndex = 23;
            this.ChkUseXrmClient.Text = "Use Xrm Client";
            this.ChkUseXrmClient.UseVisualStyleBackColor = true;
            // 
            // BtnEntitesToSkip
            // 
            this.helpProvider1.SetHelpString(this.BtnEntitesToSkip, "Overrides Capitalization of Entity Attributes");
            this.BtnEntitesToSkip.Location = new System.Drawing.Point(58, 167);
            this.BtnEntitesToSkip.Name = "BtnEntitesToSkip";
            this.helpProvider1.SetShowHelp(this.BtnEntitesToSkip, true);
            this.BtnEntitesToSkip.Size = new System.Drawing.Size(152, 23);
            this.BtnEntitesToSkip.TabIndex = 22;
            this.BtnEntitesToSkip.Text = "Entities To Skip";
            this.BtnEntitesToSkip.UseVisualStyleBackColor = true;
            this.BtnEntitesToSkip.Click += new System.EventHandler(this.BtnEntitesToSkip_Click);
            // 
            // ChkCreateOneEntityFile
            // 
            this.ChkCreateOneEntityFile.AutoSize = true;
            this.ChkCreateOneEntityFile.Location = new System.Drawing.Point(216, 161);
            this.ChkCreateOneEntityFile.Name = "ChkCreateOneEntityFile";
            this.ChkCreateOneEntityFile.Size = new System.Drawing.Size(147, 17);
            this.ChkCreateOneEntityFile.TabIndex = 20;
            this.ChkCreateOneEntityFile.Text = "Create One File Per Entity";
            this.ChkCreateOneEntityFile.UseVisualStyleBackColor = true;
            this.ChkCreateOneEntityFile.CheckedChanged += new System.EventHandler(this.ChkCreateOneEntityFile_CheckedChanged);
            // 
            // ChkGenerateOptionSetEnums
            // 
            this.ChkGenerateOptionSetEnums.AutoSize = true;
            this.ChkGenerateOptionSetEnums.Location = new System.Drawing.Point(216, 253);
            this.ChkGenerateOptionSetEnums.Name = "ChkGenerateOptionSetEnums";
            this.ChkGenerateOptionSetEnums.Size = new System.Drawing.Size(200, 17);
            this.ChkGenerateOptionSetEnums.TabIndex = 18;
            this.ChkGenerateOptionSetEnums.Text = "Generate OptionSet Enum Properties";
            this.ChkGenerateOptionSetEnums.UseVisualStyleBackColor = true;
            this.ChkGenerateOptionSetEnums.CheckedChanged += new System.EventHandler(this.ChkGenerateOptionSetEnums_CheckedChanged);
            // 
            // BtnOpenOptionSetPathDialog
            // 
            this.BtnOpenOptionSetPathDialog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpProvider1.SetHelpString(this.BtnOpenOptionSetPathDialog, "Overrides Capitalization of Entity Attributes");
            this.BtnOpenOptionSetPathDialog.Location = new System.Drawing.Point(527, 9);
            this.BtnOpenOptionSetPathDialog.Name = "BtnOpenOptionSetPathDialog";
            this.helpProvider1.SetShowHelp(this.BtnOpenOptionSetPathDialog, true);
            this.BtnOpenOptionSetPathDialog.Size = new System.Drawing.Size(27, 23);
            this.BtnOpenOptionSetPathDialog.TabIndex = 17;
            this.BtnOpenOptionSetPathDialog.Text = "...";
            this.BtnOpenOptionSetPathDialog.UseVisualStyleBackColor = true;
            this.BtnOpenOptionSetPathDialog.Click += new System.EventHandler(this.BtnOpenOptionSetPathDialog_Click);
            // 
            // LblOptionSetsDirectory
            // 
            this.LblOptionSetsDirectory.AutoSize = true;
            this.LblOptionSetsDirectory.Location = new System.Drawing.Point(6, 14);
            this.LblOptionSetsDirectory.Name = "LblOptionSetsDirectory";
            this.LblOptionSetsDirectory.Size = new System.Drawing.Size(129, 13);
            this.LblOptionSetsDirectory.TabIndex = 16;
            this.LblOptionSetsDirectory.Text = "OptionSets Relative Path:";
            // 
            // TxtOptionSetPath
            // 
            this.TxtOptionSetPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtOptionSetPath.Location = new System.Drawing.Point(152, 11);
            this.TxtOptionSetPath.Name = "TxtOptionSetPath";
            this.TxtOptionSetPath.Size = new System.Drawing.Size(369, 20);
            this.TxtOptionSetPath.TabIndex = 15;
            // 
            // BtnOpenEntityPathDialog
            // 
            this.BtnOpenEntityPathDialog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpProvider1.SetHelpString(this.BtnOpenEntityPathDialog, "Overrides Capitalization of Entity Attributes");
            this.BtnOpenEntityPathDialog.Location = new System.Drawing.Point(504, 11);
            this.BtnOpenEntityPathDialog.Name = "BtnOpenEntityPathDialog";
            this.helpProvider1.SetShowHelp(this.BtnOpenEntityPathDialog, true);
            this.BtnOpenEntityPathDialog.Size = new System.Drawing.Size(27, 23);
            this.BtnOpenEntityPathDialog.TabIndex = 14;
            this.BtnOpenEntityPathDialog.Text = "...";
            this.BtnOpenEntityPathDialog.UseVisualStyleBackColor = true;
            this.BtnOpenEntityPathDialog.Click += new System.EventHandler(this.BtnOpenEntityPathDialog_Click);
            // 
            // LblEntityPath
            // 
            this.LblEntityPath.AutoSize = true;
            this.LblEntityPath.Location = new System.Drawing.Point(55, 16);
            this.LblEntityPath.Name = "LblEntityPath";
            this.LblEntityPath.Size = new System.Drawing.Size(103, 13);
            this.LblEntityPath.TabIndex = 13;
            this.LblEntityPath.Text = "Entity Relative Path:";
            // 
            // TxtEntityPath
            // 
            this.TxtEntityPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtEntityPath.Location = new System.Drawing.Point(164, 13);
            this.TxtEntityPath.Name = "TxtEntityPath";
            this.TxtEntityPath.Size = new System.Drawing.Size(334, 20);
            this.TxtEntityPath.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(42, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Service Context Name:";
            // 
            // TxtServiceContextName
            // 
            this.TxtServiceContextName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtServiceContextName.Location = new System.Drawing.Point(164, 40);
            this.TxtServiceContextName.Name = "TxtServiceContextName";
            this.TxtServiceContextName.Size = new System.Drawing.Size(334, 20);
            this.TxtServiceContextName.TabIndex = 10;
            // 
            // BtnUnmappedProperties
            // 
            this.helpProvider1.SetHelpString(this.BtnUnmappedProperties, "Overrides Capitalization of Entity Attributes");
            this.BtnUnmappedProperties.Location = new System.Drawing.Point(58, 225);
            this.BtnUnmappedProperties.Name = "BtnUnmappedProperties";
            this.helpProvider1.SetShowHelp(this.BtnUnmappedProperties, true);
            this.BtnUnmappedProperties.Size = new System.Drawing.Size(152, 23);
            this.BtnUnmappedProperties.TabIndex = 6;
            this.BtnUnmappedProperties.Text = "Unmapped Properties";
            this.BtnUnmappedProperties.UseVisualStyleBackColor = true;
            this.BtnUnmappedProperties.Click += new System.EventHandler(this.BtnUnmappedProperties_Click);
            // 
            // BtnEnumMappings
            // 
            this.helpProvider1.SetHelpString(this.BtnEnumMappings, "Overrides Capitalization of Entity Attributes");
            this.BtnEnumMappings.Location = new System.Drawing.Point(58, 138);
            this.BtnEnumMappings.Name = "BtnEnumMappings";
            this.helpProvider1.SetShowHelp(this.BtnEnumMappings, true);
            this.BtnEnumMappings.Size = new System.Drawing.Size(152, 23);
            this.BtnEnumMappings.TabIndex = 5;
            this.BtnEnumMappings.Text = "Enum Mappings ";
            this.BtnEnumMappings.UseVisualStyleBackColor = true;
            this.BtnEnumMappings.Click += new System.EventHandler(this.BtnEnumMappings_Click);
            // 
            // BtnOptionSetsToSkip
            // 
            this.helpProvider1.SetHelpString(this.BtnOptionSetsToSkip, "Overrides Capitalization of Entity Attributes");
            this.BtnOptionSetsToSkip.Location = new System.Drawing.Point(68, 106);
            this.BtnOptionSetsToSkip.Name = "BtnOptionSetsToSkip";
            this.helpProvider1.SetShowHelp(this.BtnOptionSetsToSkip, true);
            this.BtnOptionSetsToSkip.Size = new System.Drawing.Size(152, 23);
            this.BtnOptionSetsToSkip.TabIndex = 4;
            this.BtnOptionSetsToSkip.Text = "OptionSets To Skip";
            this.BtnOptionSetsToSkip.UseVisualStyleBackColor = true;
            this.BtnOptionSetsToSkip.Click += new System.EventHandler(this.BtnOptionSetsToSkip_Click);
            // 
            // BtnSpecifyAttributeNames
            // 
            this.helpProvider1.SetHelpString(this.BtnSpecifyAttributeNames, "Overrides Capitalization of Entity Attributes");
            this.BtnSpecifyAttributeNames.Location = new System.Drawing.Point(58, 196);
            this.BtnSpecifyAttributeNames.Name = "BtnSpecifyAttributeNames";
            this.helpProvider1.SetShowHelp(this.BtnSpecifyAttributeNames, true);
            this.BtnSpecifyAttributeNames.Size = new System.Drawing.Size(152, 23);
            this.BtnSpecifyAttributeNames.TabIndex = 3;
            this.BtnSpecifyAttributeNames.Text = "Specify Attribute Names";
            this.BtnSpecifyAttributeNames.UseVisualStyleBackColor = true;
            this.BtnSpecifyAttributeNames.Click += new System.EventHandler(this.BtnSpecifyAttributeNames_Click);
            // 
            // BtnActionsToSkip
            // 
            this.helpProvider1.SetHelpString(this.BtnActionsToSkip, "Overrides Capitalization of Entity Attributes");
            this.BtnActionsToSkip.Location = new System.Drawing.Point(146, 53);
            this.BtnActionsToSkip.Name = "BtnActionsToSkip";
            this.helpProvider1.SetShowHelp(this.BtnActionsToSkip, true);
            this.BtnActionsToSkip.Size = new System.Drawing.Size(152, 23);
            this.BtnActionsToSkip.TabIndex = 23;
            this.BtnActionsToSkip.Text = "Actions To Skip";
            this.BtnActionsToSkip.UseVisualStyleBackColor = true;
            this.BtnActionsToSkip.Click += new System.EventHandler(this.BtnActionsToSkip_Click);
            // 
            // BtnOpenActionPathDialog
            // 
            this.BtnOpenActionPathDialog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpProvider1.SetHelpString(this.BtnOpenActionPathDialog, "Overrides Capitalization of Entity Attributes");
            this.BtnOpenActionPathDialog.Location = new System.Drawing.Point(531, 16);
            this.BtnOpenActionPathDialog.Name = "BtnOpenActionPathDialog";
            this.helpProvider1.SetShowHelp(this.BtnOpenActionPathDialog, true);
            this.BtnOpenActionPathDialog.Size = new System.Drawing.Size(27, 23);
            this.BtnOpenActionPathDialog.TabIndex = 26;
            this.BtnOpenActionPathDialog.Text = "...";
            this.BtnOpenActionPathDialog.UseVisualStyleBackColor = true;
            this.BtnOpenActionPathDialog.Click += new System.EventHandler(this.BtnOpenActionPathDialog_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.CheckFileExists = false;
            this.openFileDialog1.DefaultExt = "cs";
            this.openFileDialog1.Filter = "C# files|*.cs";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.GlobalTab);
            this.tabControl1.Controls.Add(this.EntityTab);
            this.tabControl1.Controls.Add(this.OptionSetTab);
            this.tabControl1.Controls.Add(this.ActionTab);
            this.tabControl1.Location = new System.Drawing.Point(2, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(389, 352);
            this.tabControl1.TabIndex = 3;
            // 
            // GlobalTab
            // 
            this.GlobalTab.Controls.Add(this.groupBox1);
            this.GlobalTab.Location = new System.Drawing.Point(4, 22);
            this.GlobalTab.Name = "GlobalTab";
            this.GlobalTab.Padding = new System.Windows.Forms.Padding(3);
            this.GlobalTab.Size = new System.Drawing.Size(381, 326);
            this.GlobalTab.TabIndex = 3;
            this.GlobalTab.Text = "Global";
            this.GlobalTab.UseVisualStyleBackColor = true;
            // 
            // EntityTab
            // 
            this.EntityTab.Controls.Add(this.ChkGenerateEntityRelationships);
            this.EntityTab.Controls.Add(this.ChkMakeReadonlyFieldsEditable);
            this.EntityTab.Controls.Add(this.ChkGenerateAttributeNameConsts);
            this.EntityTab.Controls.Add(this.ChkGenerateAnonymousTypeConstructor);
            this.EntityTab.Controls.Add(this.LblEntitiesDirectory);
            this.EntityTab.Controls.Add(this.tableLayoutPanel1);
            this.EntityTab.Controls.Add(this.BtnEntitesToSkip);
            this.EntityTab.Controls.Add(this.ChkUseXrmClient);
            this.EntityTab.Controls.Add(this.BtnOpenEntityPathDialog);
            this.EntityTab.Controls.Add(this.BtnEnumMappings);
            this.EntityTab.Controls.Add(this.BtnSpecifyAttributeNames);
            this.EntityTab.Controls.Add(this.BtnUnmappedProperties);
            this.EntityTab.Controls.Add(this.ChkGenerateOptionSetEnums);
            this.EntityTab.Controls.Add(this.ChkAddDebuggerNonUserCode);
            this.EntityTab.Controls.Add(this.TxtServiceContextName);
            this.EntityTab.Controls.Add(this.label2);
            this.EntityTab.Controls.Add(this.ChkCreateOneEntityFile);
            this.EntityTab.Controls.Add(this.TxtEntityPath);
            this.EntityTab.Controls.Add(this.LblEntityPath);
            this.EntityTab.Location = new System.Drawing.Point(4, 22);
            this.EntityTab.Name = "EntityTab";
            this.EntityTab.Padding = new System.Windows.Forms.Padding(3);
            this.EntityTab.Size = new System.Drawing.Size(381, 326);
            this.EntityTab.TabIndex = 0;
            this.EntityTab.Text = "Entities";
            this.EntityTab.UseVisualStyleBackColor = true;
            // 
            // ChkGenerateEntityRelationships
            // 
            this.ChkGenerateEntityRelationships.AutoSize = true;
            this.ChkGenerateEntityRelationships.Location = new System.Drawing.Point(216, 230);
            this.ChkGenerateEntityRelationships.Name = "ChkGenerateEntityRelationships";
            this.ChkGenerateEntityRelationships.Size = new System.Drawing.Size(165, 17);
            this.ChkGenerateEntityRelationships.TabIndex = 29;
            this.ChkGenerateEntityRelationships.Text = "Generate Entity Relationships";
            this.ChkGenerateEntityRelationships.UseVisualStyleBackColor = true;
            // 
            // ChkMakeReadonlyFieldsEditable
            // 
            this.ChkMakeReadonlyFieldsEditable.AutoSize = true;
            this.ChkMakeReadonlyFieldsEditable.Location = new System.Drawing.Point(164, 75);
            this.ChkMakeReadonlyFieldsEditable.Name = "ChkMakeReadonlyFieldsEditable";
            this.ChkMakeReadonlyFieldsEditable.Size = new System.Drawing.Size(172, 17);
            this.ChkMakeReadonlyFieldsEditable.TabIndex = 28;
            this.ChkMakeReadonlyFieldsEditable.Text = "Make Readonly Fields Editable";
            this.ChkMakeReadonlyFieldsEditable.UseVisualStyleBackColor = true;
            // 
            // ChkGenerateAttributeNameConsts
            // 
            this.ChkGenerateAttributeNameConsts.AutoSize = true;
            this.ChkGenerateAttributeNameConsts.Location = new System.Drawing.Point(216, 184);
            this.ChkGenerateAttributeNameConsts.Name = "ChkGenerateAttributeNameConsts";
            this.ChkGenerateAttributeNameConsts.Size = new System.Drawing.Size(178, 17);
            this.ChkGenerateAttributeNameConsts.TabIndex = 27;
            this.ChkGenerateAttributeNameConsts.Text = "Generate Attribute Name Consts";
            this.ChkGenerateAttributeNameConsts.UseVisualStyleBackColor = true;
            // 
            // ChkGenerateAnonymousTypeConstructor
            // 
            this.ChkGenerateAnonymousTypeConstructor.AutoSize = true;
            this.ChkGenerateAnonymousTypeConstructor.Location = new System.Drawing.Point(216, 207);
            this.ChkGenerateAnonymousTypeConstructor.Name = "ChkGenerateAnonymousTypeConstructor";
            this.ChkGenerateAnonymousTypeConstructor.Size = new System.Drawing.Size(214, 17);
            this.ChkGenerateAnonymousTypeConstructor.TabIndex = 26;
            this.ChkGenerateAnonymousTypeConstructor.Text = "Generate AnonymousType Constructors";
            this.ChkGenerateAnonymousTypeConstructor.UseVisualStyleBackColor = true;
            // 
            // LblEntitiesDirectory
            // 
            this.LblEntitiesDirectory.AutoSize = true;
            this.LblEntitiesDirectory.Location = new System.Drawing.Point(42, 16);
            this.LblEntitiesDirectory.Name = "LblEntitiesDirectory";
            this.LblEntitiesDirectory.Size = new System.Drawing.Size(111, 13);
            this.LblEntitiesDirectory.TabIndex = 25;
            this.LblEntitiesDirectory.Text = "Entities Relative Path:";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 204F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.BtnCreateEntities, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(5, 288);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(634, 23);
            this.tableLayoutPanel1.TabIndex = 24;
            // 
            // ChkAddDebuggerNonUserCode
            // 
            this.ChkAddDebuggerNonUserCode.AutoSize = true;
            this.ChkAddDebuggerNonUserCode.Location = new System.Drawing.Point(216, 138);
            this.ChkAddDebuggerNonUserCode.Name = "ChkAddDebuggerNonUserCode";
            this.ChkAddDebuggerNonUserCode.Size = new System.Drawing.Size(156, 17);
            this.ChkAddDebuggerNonUserCode.TabIndex = 19;
            this.ChkAddDebuggerNonUserCode.Text = "Add Debug Non User Code";
            this.ChkAddDebuggerNonUserCode.UseVisualStyleBackColor = true;
            // 
            // OptionSetTab
            // 
            this.OptionSetTab.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.OptionSetTab.Controls.Add(this.label5);
            this.OptionSetTab.Controls.Add(this.TxtLanguageCodeOverride);
            this.OptionSetTab.Controls.Add(this.LblOptionSetFormat);
            this.OptionSetTab.Controls.Add(this.TxtOptionSetFormat);
            this.OptionSetTab.Controls.Add(this.ChkUseDeprecatedOptionSetNaming);
            this.OptionSetTab.Controls.Add(this.TxtInvalidCSharpNamePrefix);
            this.OptionSetTab.Controls.Add(this.label3);
            this.OptionSetTab.Controls.Add(this.LblOptionSetsDirectory);
            this.OptionSetTab.Controls.Add(this.LblOptionSetPath);
            this.OptionSetTab.Controls.Add(this.ChkCreateOneOptionSetFile);
            this.OptionSetTab.Controls.Add(this.tableLayoutPanel2);
            this.OptionSetTab.Controls.Add(this.TxtOptionSetPath);
            this.OptionSetTab.Controls.Add(this.BtnOpenOptionSetPathDialog);
            this.OptionSetTab.Controls.Add(this.BtnOptionSetsToSkip);
            this.OptionSetTab.Location = new System.Drawing.Point(4, 22);
            this.OptionSetTab.Name = "OptionSetTab";
            this.OptionSetTab.Padding = new System.Windows.Forms.Padding(3);
            this.OptionSetTab.Size = new System.Drawing.Size(381, 326);
            this.OptionSetTab.TabIndex = 1;
            this.OptionSetTab.Text = "Option Sets";
            this.OptionSetTab.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(49, 40);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 13);
            this.label5.TabIndex = 34;
            this.label5.Text = "Language Code:";
            // 
            // TxtLanguageCodeOverride
            // 
            this.TxtLanguageCodeOverride.Location = new System.Drawing.Point(152, 37);
            this.TxtLanguageCodeOverride.Name = "TxtLanguageCodeOverride";
            this.TxtLanguageCodeOverride.Size = new System.Drawing.Size(85, 20);
            this.TxtLanguageCodeOverride.TabIndex = 33;
            this.TxtLanguageCodeOverride.TextChanged += new System.EventHandler(this.TxtLanguageCodeOverride_TextChanged);
            this.TxtLanguageCodeOverride.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtLanguageCodeOverride_KeyPress);
            // 
            // LblOptionSetFormat
            // 
            this.LblOptionSetFormat.AutoSize = true;
            this.LblOptionSetFormat.Location = new System.Drawing.Point(236, 155);
            this.LblOptionSetFormat.Name = "LblOptionSetFormat";
            this.LblOptionSetFormat.Size = new System.Drawing.Size(102, 13);
            this.LblOptionSetFormat.TabIndex = 32;
            this.LblOptionSetFormat.Text = "Local Name Format:";
            // 
            // TxtOptionSetFormat
            // 
            this.TxtOptionSetFormat.Location = new System.Drawing.Point(344, 153);
            this.TxtOptionSetFormat.Name = "TxtOptionSetFormat";
            this.TxtOptionSetFormat.Size = new System.Drawing.Size(85, 20);
            this.TxtOptionSetFormat.TabIndex = 31;
            // 
            // ChkUseDeprecatedOptionSetNaming
            // 
            this.ChkUseDeprecatedOptionSetNaming.AutoSize = true;
            this.ChkUseDeprecatedOptionSetNaming.Location = new System.Drawing.Point(226, 129);
            this.ChkUseDeprecatedOptionSetNaming.Name = "ChkUseDeprecatedOptionSetNaming";
            this.ChkUseDeprecatedOptionSetNaming.Size = new System.Drawing.Size(190, 17);
            this.ChkUseDeprecatedOptionSetNaming.TabIndex = 30;
            this.ChkUseDeprecatedOptionSetNaming.Text = "Use Deprecated OptionSet Names";
            this.ChkUseDeprecatedOptionSetNaming.UseVisualStyleBackColor = true;
            this.ChkUseDeprecatedOptionSetNaming.CheckedChanged += new System.EventHandler(this.ChkUseDeprecatedOptionSetNaming_CheckedChanged);
            // 
            // TxtInvalidCSharpNamePrefix
            // 
            this.TxtInvalidCSharpNamePrefix.Location = new System.Drawing.Point(344, 179);
            this.TxtInvalidCSharpNamePrefix.Name = "TxtInvalidCSharpNamePrefix";
            this.TxtInvalidCSharpNamePrefix.Size = new System.Drawing.Size(45, 20);
            this.TxtInvalidCSharpNamePrefix.TabIndex = 29;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(217, 182);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(121, 13);
            this.label3.TabIndex = 28;
            this.label3.Text = "Prefix For Invalid Names";
            // 
            // LblOptionSetPath
            // 
            this.LblOptionSetPath.AutoSize = true;
            this.LblOptionSetPath.Location = new System.Drawing.Point(6, 14);
            this.LblOptionSetPath.Name = "LblOptionSetPath";
            this.LblOptionSetPath.Size = new System.Drawing.Size(124, 13);
            this.LblOptionSetPath.TabIndex = 27;
            this.LblOptionSetPath.Text = "OptionSet Relative Path:";
            // 
            // ChkCreateOneOptionSetFile
            // 
            this.ChkCreateOneOptionSetFile.AutoSize = true;
            this.ChkCreateOneOptionSetFile.Location = new System.Drawing.Point(226, 106);
            this.ChkCreateOneOptionSetFile.Name = "ChkCreateOneOptionSetFile";
            this.ChkCreateOneOptionSetFile.Size = new System.Drawing.Size(171, 17);
            this.ChkCreateOneOptionSetFile.TabIndex = 26;
            this.ChkCreateOneOptionSetFile.Text = "Create One File Per Option Set";
            this.ChkCreateOneOptionSetFile.UseVisualStyleBackColor = true;
            this.ChkCreateOneOptionSetFile.CheckedChanged += new System.EventHandler(this.ChkCreateOneOptionSetFile_CheckedChanged);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 204F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.BtnCreateOptionSets, 1, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(70, 268);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(509, 23);
            this.tableLayoutPanel2.TabIndex = 25;
            // 
            // ActionTab
            // 
            this.ActionTab.Controls.Add(this.ChkGenerateActionAttributeNameConsts);
            this.ActionTab.Controls.Add(this.ChkEditableResponseActions);
            this.ActionTab.Controls.Add(this.LblActionsDirectory);
            this.ActionTab.Controls.Add(this.ChkCreateOneActionFile);
            this.ActionTab.Controls.Add(this.tableLayoutPanel3);
            this.ActionTab.Controls.Add(this.TxtActionPath);
            this.ActionTab.Controls.Add(this.BtnOpenActionPathDialog);
            this.ActionTab.Controls.Add(this.LblActionPath);
            this.ActionTab.Controls.Add(this.BtnActionsToSkip);
            this.ActionTab.Location = new System.Drawing.Point(4, 22);
            this.ActionTab.Name = "ActionTab";
            this.ActionTab.Padding = new System.Windows.Forms.Padding(3);
            this.ActionTab.Size = new System.Drawing.Size(381, 326);
            this.ActionTab.TabIndex = 2;
            this.ActionTab.Text = "Actions";
            this.ActionTab.UseVisualStyleBackColor = true;
            this.ActionTab.Enter += new System.EventHandler(this.actionsTab_Enter);
            // 
            // ChkGenerateActionAttributeNameConsts
            // 
            this.ChkGenerateActionAttributeNameConsts.AutoSize = true;
            this.ChkGenerateActionAttributeNameConsts.Location = new System.Drawing.Point(304, 76);
            this.ChkGenerateActionAttributeNameConsts.Name = "ChkGenerateActionAttributeNameConsts";
            this.ChkGenerateActionAttributeNameConsts.Size = new System.Drawing.Size(178, 17);
            this.ChkGenerateActionAttributeNameConsts.TabIndex = 31;
            this.ChkGenerateActionAttributeNameConsts.Text = "Generate Attribute Name Consts";
            this.ChkGenerateActionAttributeNameConsts.UseVisualStyleBackColor = true;
            // 
            // ChkEditableResponseActions
            // 
            this.ChkEditableResponseActions.AutoSize = true;
            this.ChkEditableResponseActions.Location = new System.Drawing.Point(304, 99);
            this.ChkEditableResponseActions.Name = "ChkEditableResponseActions";
            this.ChkEditableResponseActions.Size = new System.Drawing.Size(150, 17);
            this.ChkEditableResponseActions.TabIndex = 30;
            this.ChkEditableResponseActions.Text = "Make Responses Editable";
            this.ChkEditableResponseActions.UseVisualStyleBackColor = true;
            // 
            // LblActionsDirectory
            // 
            this.LblActionsDirectory.AutoSize = true;
            this.LblActionsDirectory.Location = new System.Drawing.Point(6, 21);
            this.LblActionsDirectory.Name = "LblActionsDirectory";
            this.LblActionsDirectory.Size = new System.Drawing.Size(112, 13);
            this.LblActionsDirectory.TabIndex = 29;
            this.LblActionsDirectory.Text = "Actions Relative Path:";
            // 
            // ChkCreateOneActionFile
            // 
            this.ChkCreateOneActionFile.AutoSize = true;
            this.ChkCreateOneActionFile.Location = new System.Drawing.Point(304, 53);
            this.ChkCreateOneActionFile.Name = "ChkCreateOneActionFile";
            this.ChkCreateOneActionFile.Size = new System.Drawing.Size(151, 17);
            this.ChkCreateOneActionFile.TabIndex = 28;
            this.ChkCreateOneActionFile.Text = "Create One File Per Action";
            this.ChkCreateOneActionFile.UseVisualStyleBackColor = true;
            this.ChkCreateOneActionFile.CheckedChanged += new System.EventHandler(this.ChkCreateOneActionFile_CheckedChanged);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 204F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.BtnCreateActions, 1, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(9, 186);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(536, 23);
            this.tableLayoutPanel3.TabIndex = 27;
            // 
            // BtnCreateActions
            // 
            this.BtnCreateActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnCreateActions.Location = new System.Drawing.Point(166, 0);
            this.BtnCreateActions.Margin = new System.Windows.Forms.Padding(0);
            this.BtnCreateActions.Name = "BtnCreateActions";
            this.BtnCreateActions.Size = new System.Drawing.Size(204, 23);
            this.BtnCreateActions.TabIndex = 1;
            this.BtnCreateActions.Text = "Create Actions";
            this.BtnCreateActions.UseVisualStyleBackColor = true;
            this.BtnCreateActions.Click += new System.EventHandler(this.BtnCreateActions_Click);
            // 
            // TxtActionPath
            // 
            this.TxtActionPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtActionPath.Location = new System.Drawing.Point(136, 18);
            this.TxtActionPath.Name = "TxtActionPath";
            this.TxtActionPath.Size = new System.Drawing.Size(389, 20);
            this.TxtActionPath.TabIndex = 24;
            // 
            // LblActionPath
            // 
            this.LblActionPath.AutoSize = true;
            this.LblActionPath.Location = new System.Drawing.Point(6, 21);
            this.LblActionPath.Name = "LblActionPath";
            this.LblActionPath.Size = new System.Drawing.Size(107, 13);
            this.LblActionPath.TabIndex = 25;
            this.LblActionPath.Text = "Action Relative Path:";
            // 
            // PropertiesGrid
            // 
            this.PropertiesGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PropertiesGrid.Location = new System.Drawing.Point(0, 32);
            this.PropertiesGrid.Name = "PropertiesGrid";
            this.PropertiesGrid.Size = new System.Drawing.Size(489, 503);
            this.PropertiesGrid.TabIndex = 36;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.BtnOpenSettingsPathDialog);
            this.splitContainer1.Panel1.Controls.Add(label4);
            this.splitContainer1.Panel1.Controls.Add(this.PropertiesGrid);
            this.splitContainer1.Panel1.Controls.Add(this.TxtSettingsPath);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Panel2.Controls.Add(this.TxtOutput);
            this.splitContainer1.Size = new System.Drawing.Size(887, 535);
            this.splitContainer1.SplitterDistance = 489;
            this.splitContainer1.TabIndex = 37;
            // 
            // EarlyBoundGeneratorPlugin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "EarlyBoundGeneratorPlugin";
            this.helpProvider1.SetShowHelp(this, true);
            this.Size = new System.Drawing.Size(890, 541);
            this.ConnectionUpdated += new XrmToolBox.Extensibility.PluginControlBase.ConnectionUpdatedHandler(this.EarlyBoundGenerator_ConnectionUpdated);
            this.Load += new System.EventHandler(this.EarlyBoundGenerator_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.GlobalTab.ResumeLayout(false);
            this.EntityTab.ResumeLayout(false);
            this.EntityTab.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.OptionSetTab.ResumeLayout(false);
            this.OptionSetTab.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ActionTab.ResumeLayout(false);
            this.ActionTab.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtnCreateOptionSets;
        private System.Windows.Forms.Button BtnCreateEntities;
        private System.Windows.Forms.TextBox TxtOutput;
        private System.Windows.Forms.Button BtnCreateAll;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox ChkMaskPassword;
        private System.Windows.Forms.CheckBox ChkIncludeCommandLine;
        private System.Windows.Forms.CheckBox ChkUseTFS;
        private System.Windows.Forms.Button BtnSpecifyAttributeNames;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.TextBox TxtHelp;
        private System.Windows.Forms.Button BtnUnmappedProperties;
        private System.Windows.Forms.Button BtnEnumMappings;
        private System.Windows.Forms.Button BtnOptionSetsToSkip;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TxtServiceContextName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TxtNamespace;
        private System.Windows.Forms.Button BtnOpenOptionSetPathDialog;
        private System.Windows.Forms.Label LblOptionSetsDirectory;
        private System.Windows.Forms.TextBox TxtOptionSetPath;
        private System.Windows.Forms.Button BtnOpenEntityPathDialog;
        private System.Windows.Forms.Label LblEntityPath;
        private System.Windows.Forms.TextBox TxtEntityPath;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.CheckBox ChkGenerateOptionSetEnums;
        private System.Windows.Forms.CheckBox ChkCreateOneEntityFile;
        private System.Windows.Forms.Button BtnEntitesToSkip;
        private System.Windows.Forms.CheckBox ChkUseXrmClient;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage EntityTab;
        private System.Windows.Forms.TabPage OptionSetTab;
        private System.Windows.Forms.TabPage ActionTab;
        private System.Windows.Forms.CheckBox ChkAddDebuggerNonUserCode;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button BtnCreateActions;
        private System.Windows.Forms.TextBox TxtActionPath;
        private System.Windows.Forms.Button BtnOpenActionPathDialog;
        private System.Windows.Forms.Label LblActionPath;
        private System.Windows.Forms.Button BtnActionsToSkip;
        private System.Windows.Forms.CheckBox ChkCreateOneActionFile;
        private System.Windows.Forms.Label LblEntitiesDirectory;
        private System.Windows.Forms.Label LblActionsDirectory;
        private System.Windows.Forms.CheckBox ChkCreateOneOptionSetFile;
        private System.Windows.Forms.Label LblOptionSetPath;
        private System.Windows.Forms.CheckBox ChkGenerateAnonymousTypeConstructor;
        private System.Windows.Forms.CheckBox ChkRemoveRuntimeComment;
        private System.Windows.Forms.CheckBox ChkGenerateAttributeNameConsts;
        private System.Windows.Forms.CheckBox ChkAddFilesToProject;
        private System.Windows.Forms.TextBox TxtInvalidCSharpNamePrefix;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox ChkMakeReadonlyFieldsEditable;
        private System.Windows.Forms.CheckBox ChkUseDeprecatedOptionSetNaming;
        private System.Windows.Forms.Label LblOptionSetFormat;
        private System.Windows.Forms.TextBox TxtOptionSetFormat;
        private System.Windows.Forms.CheckBox ChkGenerateEntityRelationships;
        private System.Windows.Forms.CheckBox ChkAudibleCompletion;
        private System.Windows.Forms.Button BtnOpenSettingsPathDialog;
        private System.Windows.Forms.TextBox TxtSettingsPath;
        private System.Windows.Forms.CheckBox ChkEditableResponseActions;
        private System.Windows.Forms.CheckBox ChkGenerateActionAttributeNameConsts;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox TxtLanguageCodeOverride;
        private System.Windows.Forms.TabPage GlobalTab;
        private System.Windows.Forms.PropertyGrid PropertiesGrid;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}
