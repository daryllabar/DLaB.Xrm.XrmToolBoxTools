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
            this.BtnCreateOptionSets = new System.Windows.Forms.Button();
            this.BtnCreateAll = new System.Windows.Forms.Button();
            this.BtnCreateEntities = new System.Windows.Forms.Button();
            this.TxtOutput = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ChkAddFilesToProject = new System.Windows.Forms.CheckBox();
            this.ChkRemoveRuntimeComment = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TxtNamespace = new System.Windows.Forms.TextBox();
            this.TxtHelp = new System.Windows.Forms.TextBox();
            this.ChkUseTFS = new System.Windows.Forms.CheckBox();
            this.ChkMaskPassword = new System.Windows.Forms.CheckBox();
            this.ChkIncludeCommandLine = new System.Windows.Forms.CheckBox();
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
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.ChkMakeReadonlyFieldsEditable = new System.Windows.Forms.CheckBox();
            this.ChkGenerateAttributeNameConsts = new System.Windows.Forms.CheckBox();
            this.ChkGenerateAnonymousTypeConstructor = new System.Windows.Forms.CheckBox();
            this.LblEntitiesDirectory = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ChkAddDebuggerNonUserCode = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.LblOptionSetFormat = new System.Windows.Forms.Label();
            this.TxtOptionSetFormat = new System.Windows.Forms.TextBox();
            this.ChkUseDeprecatedOptionSetNaming = new System.Windows.Forms.CheckBox();
            this.TxtInvalidCSharpNamePrefix = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.LblOptionSetPath = new System.Windows.Forms.Label();
            this.ChkCreateOneOptionSetFile = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.actionsTab = new System.Windows.Forms.TabPage();
            this.LblActionsDirectory = new System.Windows.Forms.Label();
            this.ChkCreateOneActionFile = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.BtnCreateActions = new System.Windows.Forms.Button();
            this.TxtActionPath = new System.Windows.Forms.TextBox();
            this.LblActionPath = new System.Windows.Forms.Label();
            this.ChkGenerateEntityRelationships = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.actionsTab.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnCreateOptionSets
            // 
            this.BtnCreateOptionSets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnCreateOptionSets.Location = new System.Drawing.Point(327, 0);
            this.BtnCreateOptionSets.Margin = new System.Windows.Forms.Padding(0);
            this.BtnCreateOptionSets.Name = "BtnCreateOptionSets";
            this.BtnCreateOptionSets.Size = new System.Drawing.Size(204, 23);
            this.BtnCreateOptionSets.TabIndex = 1;
            this.BtnCreateOptionSets.Text = "Create OptionSets";
            this.BtnCreateOptionSets.UseVisualStyleBackColor = true;
            this.BtnCreateOptionSets.Click += new System.EventHandler(this.BtnCreateOptionSets_Click);
            this.BtnCreateOptionSets.MouseEnter += new System.EventHandler(this.BtnCreateOptionSets_MouseEnter);
            // 
            // BtnCreateAll
            // 
            this.BtnCreateAll.Location = new System.Drawing.Point(6, 136);
            this.BtnCreateAll.Name = "BtnCreateAll";
            this.BtnCreateAll.Size = new System.Drawing.Size(204, 21);
            this.BtnCreateAll.TabIndex = 1;
            this.BtnCreateAll.Text = "Create All";
            this.BtnCreateAll.UseVisualStyleBackColor = true;
            this.BtnCreateAll.Click += new System.EventHandler(this.BtnCreateBoth_Click);
            this.BtnCreateAll.MouseEnter += new System.EventHandler(this.BtnCreateAll_MouseEnter);
            // 
            // BtnCreateEntities
            // 
            this.BtnCreateEntities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnCreateEntities.Location = new System.Drawing.Point(327, 0);
            this.BtnCreateEntities.Margin = new System.Windows.Forms.Padding(0);
            this.BtnCreateEntities.Name = "BtnCreateEntities";
            this.BtnCreateEntities.Size = new System.Drawing.Size(204, 23);
            this.BtnCreateEntities.TabIndex = 0;
            this.BtnCreateEntities.Text = "Create Entities";
            this.BtnCreateEntities.UseVisualStyleBackColor = true;
            this.BtnCreateEntities.Click += new System.EventHandler(this.BtnCreateEntities_Click);
            this.BtnCreateEntities.MouseEnter += new System.EventHandler(this.BtnCreateEntities_MouseEnter);
            // 
            // TxtOutput
            // 
            this.TxtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtOutput.Location = new System.Drawing.Point(0, 372);
            this.TxtOutput.Multiline = true;
            this.TxtOutput.Name = "TxtOutput";
            this.TxtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TxtOutput.Size = new System.Drawing.Size(890, 172);
            this.TxtOutput.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.ChkAddFilesToProject);
            this.groupBox1.Controls.Add(this.ChkRemoveRuntimeComment);
            this.groupBox1.Controls.Add(this.BtnCreateAll);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.TxtNamespace);
            this.groupBox1.Controls.Add(this.TxtHelp);
            this.groupBox1.Controls.Add(this.ChkUseTFS);
            this.groupBox1.Controls.Add(this.ChkMaskPassword);
            this.groupBox1.Controls.Add(this.ChkIncludeCommandLine);
            this.groupBox1.Location = new System.Drawing.Point(3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(884, 162);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
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
            this.ChkAddFilesToProject.MouseEnter += new System.EventHandler(this.ChkAddFilesToProject_MouseEnter);
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
            this.ChkRemoveRuntimeComment.MouseEnter += new System.EventHandler(this.ChkRemoveRuntimeComment_MouseEnter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(213, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Namespace:";
            // 
            // TxtNamespace
            // 
            this.TxtNamespace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtNamespace.Location = new System.Drawing.Point(286, 20);
            this.TxtNamespace.Name = "TxtNamespace";
            this.TxtNamespace.Size = new System.Drawing.Size(592, 20);
            this.TxtNamespace.TabIndex = 8;
            // 
            // TxtHelp
            // 
            this.TxtHelp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtHelp.BackColor = System.Drawing.SystemColors.Control;
            this.TxtHelp.Location = new System.Drawing.Point(216, 46);
            this.TxtHelp.Multiline = true;
            this.TxtHelp.Name = "TxtHelp";
            this.TxtHelp.Size = new System.Drawing.Size(662, 111);
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
            this.ChkUseTFS.MouseEnter += new System.EventHandler(this.ChkUseTFS_MouseEnter);
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
            this.ChkMaskPassword.MouseEnter += new System.EventHandler(this.ChkMaskPassword_MouseEnter);
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
            this.ChkIncludeCommandLine.MouseEnter += new System.EventHandler(this.ChkIncludeCommandLine_MouseEnter);
            // 
            // ChkUseXrmClient
            // 
            this.ChkUseXrmClient.AutoSize = true;
            this.ChkUseXrmClient.Location = new System.Drawing.Point(483, 97);
            this.ChkUseXrmClient.Name = "ChkUseXrmClient";
            this.ChkUseXrmClient.Size = new System.Drawing.Size(95, 17);
            this.ChkUseXrmClient.TabIndex = 23;
            this.ChkUseXrmClient.Text = "Use Xrm Client";
            this.ChkUseXrmClient.UseVisualStyleBackColor = true;
            this.ChkUseXrmClient.MouseEnter += new System.EventHandler(this.ChkUseXrmClient_MouseEnter);
            // 
            // BtnEntitesToSkip
            // 
            this.helpProvider1.SetHelpString(this.BtnEntitesToSkip, "Overrides Capitalization of Entity Attributes");
            this.BtnEntitesToSkip.Location = new System.Drawing.Point(6, 35);
            this.BtnEntitesToSkip.Name = "BtnEntitesToSkip";
            this.helpProvider1.SetShowHelp(this.BtnEntitesToSkip, true);
            this.BtnEntitesToSkip.Size = new System.Drawing.Size(152, 23);
            this.BtnEntitesToSkip.TabIndex = 22;
            this.BtnEntitesToSkip.Text = "Entities To Skip";
            this.BtnEntitesToSkip.UseVisualStyleBackColor = true;
            this.BtnEntitesToSkip.Click += new System.EventHandler(this.BtnEntitesToSkip_Click);
            this.BtnEntitesToSkip.MouseEnter += new System.EventHandler(this.BtnEntitesToSkip_MouseEnter);
            // 
            // ChkCreateOneEntityFile
            // 
            this.ChkCreateOneEntityFile.AutoSize = true;
            this.ChkCreateOneEntityFile.Location = new System.Drawing.Point(164, 29);
            this.ChkCreateOneEntityFile.Name = "ChkCreateOneEntityFile";
            this.ChkCreateOneEntityFile.Size = new System.Drawing.Size(147, 17);
            this.ChkCreateOneEntityFile.TabIndex = 20;
            this.ChkCreateOneEntityFile.Text = "Create One File Per Entity";
            this.ChkCreateOneEntityFile.UseVisualStyleBackColor = true;
            this.ChkCreateOneEntityFile.CheckedChanged += new System.EventHandler(this.ChkCreateOneEntityFile_CheckedChanged);
            this.ChkCreateOneEntityFile.MouseEnter += new System.EventHandler(this.ChkCreateOneEntityFile_MouseEnter);
            // 
            // ChkGenerateOptionSetEnums
            // 
            this.ChkGenerateOptionSetEnums.AutoSize = true;
            this.ChkGenerateOptionSetEnums.Location = new System.Drawing.Point(164, 121);
            this.ChkGenerateOptionSetEnums.Name = "ChkGenerateOptionSetEnums";
            this.ChkGenerateOptionSetEnums.Size = new System.Drawing.Size(200, 17);
            this.ChkGenerateOptionSetEnums.TabIndex = 18;
            this.ChkGenerateOptionSetEnums.Text = "Generate OptionSet Enum Properties";
            this.ChkGenerateOptionSetEnums.UseVisualStyleBackColor = true;
            this.ChkGenerateOptionSetEnums.CheckedChanged += new System.EventHandler(this.ChkGenerateOptionSetEnums_CheckedChanged);
            this.ChkGenerateOptionSetEnums.MouseEnter += new System.EventHandler(this.ChkGenerateOptionSetEnums_MouseEnter);
            // 
            // BtnOpenOptionSetPathDialog
            // 
            this.BtnOpenOptionSetPathDialog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpProvider1.SetHelpString(this.BtnOpenOptionSetPathDialog, "Overrides Capitalization of Entity Attributes");
            this.BtnOpenOptionSetPathDialog.Location = new System.Drawing.Point(837, 6);
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
            this.LblOptionSetsDirectory.Location = new System.Drawing.Point(379, 11);
            this.LblOptionSetsDirectory.Name = "LblOptionSetsDirectory";
            this.LblOptionSetsDirectory.Size = new System.Drawing.Size(129, 13);
            this.LblOptionSetsDirectory.TabIndex = 16;
            this.LblOptionSetsDirectory.Text = "OptionSets Relative Path:";
            // 
            // TxtOptionSetPath
            // 
            this.TxtOptionSetPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtOptionSetPath.Location = new System.Drawing.Point(525, 8);
            this.TxtOptionSetPath.Name = "TxtOptionSetPath";
            this.TxtOptionSetPath.Size = new System.Drawing.Size(306, 20);
            this.TxtOptionSetPath.TabIndex = 15;
            // 
            // BtnOpenEntityPathDialog
            // 
            this.BtnOpenEntityPathDialog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpProvider1.SetHelpString(this.BtnOpenEntityPathDialog, "Overrides Capitalization of Entity Attributes");
            this.BtnOpenEntityPathDialog.Location = new System.Drawing.Point(837, 6);
            this.BtnOpenEntityPathDialog.Name = "BtnOpenEntityPathDialog";
            this.helpProvider1.SetShowHelp(this.BtnOpenEntityPathDialog, true);
            this.BtnOpenEntityPathDialog.Size = new System.Drawing.Size(27, 23);
            this.BtnOpenEntityPathDialog.TabIndex = 14;
            this.BtnOpenEntityPathDialog.Text = "...";
            this.BtnOpenEntityPathDialog.UseVisualStyleBackColor = true;
            this.BtnOpenEntityPathDialog.Click += new System.EventHandler(this.BtnOpenEntityPathDialog_Click);
            this.BtnOpenEntityPathDialog.MouseEnter += new System.EventHandler(this.ShowEntityPathText);
            // 
            // LblEntityPath
            // 
            this.LblEntityPath.AutoSize = true;
            this.LblEntityPath.Location = new System.Drawing.Point(374, 11);
            this.LblEntityPath.Name = "LblEntityPath";
            this.LblEntityPath.Size = new System.Drawing.Size(103, 13);
            this.LblEntityPath.TabIndex = 13;
            this.LblEntityPath.Text = "Entity Relative Path:";
            // 
            // TxtEntityPath
            // 
            this.TxtEntityPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtEntityPath.Location = new System.Drawing.Point(483, 8);
            this.TxtEntityPath.Name = "TxtEntityPath";
            this.TxtEntityPath.Size = new System.Drawing.Size(348, 20);
            this.TxtEntityPath.TabIndex = 12;
            this.TxtEntityPath.MouseEnter += new System.EventHandler(this.ShowEntityPathText);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(361, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Service Context Name:";
            // 
            // TxtServiceContextName
            // 
            this.TxtServiceContextName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtServiceContextName.Location = new System.Drawing.Point(483, 35);
            this.TxtServiceContextName.Name = "TxtServiceContextName";
            this.TxtServiceContextName.Size = new System.Drawing.Size(381, 20);
            this.TxtServiceContextName.TabIndex = 10;
            this.TxtServiceContextName.MouseEnter += new System.EventHandler(this.TxtServiceContextName_MouseEnter);
            // 
            // BtnUnmappedProperties
            // 
            this.helpProvider1.SetHelpString(this.BtnUnmappedProperties, "Overrides Capitalization of Entity Attributes");
            this.BtnUnmappedProperties.Location = new System.Drawing.Point(6, 93);
            this.BtnUnmappedProperties.Name = "BtnUnmappedProperties";
            this.helpProvider1.SetShowHelp(this.BtnUnmappedProperties, true);
            this.BtnUnmappedProperties.Size = new System.Drawing.Size(152, 23);
            this.BtnUnmappedProperties.TabIndex = 6;
            this.BtnUnmappedProperties.Text = "Unmapped Properties";
            this.BtnUnmappedProperties.UseVisualStyleBackColor = true;
            this.BtnUnmappedProperties.Click += new System.EventHandler(this.BtnUnmappedProperties_Click);
            this.BtnUnmappedProperties.MouseEnter += new System.EventHandler(this.BtnUnmappedProperties_MouseEnter);
            // 
            // BtnEnumMappings
            // 
            this.helpProvider1.SetHelpString(this.BtnEnumMappings, "Overrides Capitalization of Entity Attributes");
            this.BtnEnumMappings.Location = new System.Drawing.Point(6, 6);
            this.BtnEnumMappings.Name = "BtnEnumMappings";
            this.helpProvider1.SetShowHelp(this.BtnEnumMappings, true);
            this.BtnEnumMappings.Size = new System.Drawing.Size(152, 23);
            this.BtnEnumMappings.TabIndex = 5;
            this.BtnEnumMappings.Text = "Enum Mappings ";
            this.BtnEnumMappings.UseVisualStyleBackColor = true;
            this.BtnEnumMappings.Click += new System.EventHandler(this.BtnEnumMappings_Click);
            this.BtnEnumMappings.MouseEnter += new System.EventHandler(this.BtnEnumMappings_MouseEnter);
            // 
            // BtnOptionSetsToSkip
            // 
            this.helpProvider1.SetHelpString(this.BtnOptionSetsToSkip, "Overrides Capitalization of Entity Attributes");
            this.BtnOptionSetsToSkip.Location = new System.Drawing.Point(6, 6);
            this.BtnOptionSetsToSkip.Name = "BtnOptionSetsToSkip";
            this.helpProvider1.SetShowHelp(this.BtnOptionSetsToSkip, true);
            this.BtnOptionSetsToSkip.Size = new System.Drawing.Size(152, 23);
            this.BtnOptionSetsToSkip.TabIndex = 4;
            this.BtnOptionSetsToSkip.Text = "OptionSets To Skip";
            this.BtnOptionSetsToSkip.UseVisualStyleBackColor = true;
            this.BtnOptionSetsToSkip.Click += new System.EventHandler(this.BtnOptionSetsToSkip_Click);
            this.BtnOptionSetsToSkip.MouseEnter += new System.EventHandler(this.BtnOptionSetsToSkip_MouseEnter);
            // 
            // BtnSpecifyAttributeNames
            // 
            this.helpProvider1.SetHelpString(this.BtnSpecifyAttributeNames, "Overrides Capitalization of Entity Attributes");
            this.BtnSpecifyAttributeNames.Location = new System.Drawing.Point(6, 64);
            this.BtnSpecifyAttributeNames.Name = "BtnSpecifyAttributeNames";
            this.helpProvider1.SetShowHelp(this.BtnSpecifyAttributeNames, true);
            this.BtnSpecifyAttributeNames.Size = new System.Drawing.Size(152, 23);
            this.BtnSpecifyAttributeNames.TabIndex = 3;
            this.BtnSpecifyAttributeNames.Text = "Specify Attribute Names";
            this.BtnSpecifyAttributeNames.UseVisualStyleBackColor = true;
            this.BtnSpecifyAttributeNames.Click += new System.EventHandler(this.BtnSpecifyAttributeNames_Click);
            this.BtnSpecifyAttributeNames.MouseEnter += new System.EventHandler(this.BtnSpecifyAttributeNames_MouseEnter);
            // 
            // BtnActionsToSkip
            // 
            this.helpProvider1.SetHelpString(this.BtnActionsToSkip, "Overrides Capitalization of Entity Attributes");
            this.BtnActionsToSkip.Location = new System.Drawing.Point(6, 6);
            this.BtnActionsToSkip.Name = "BtnActionsToSkip";
            this.helpProvider1.SetShowHelp(this.BtnActionsToSkip, true);
            this.BtnActionsToSkip.Size = new System.Drawing.Size(152, 23);
            this.BtnActionsToSkip.TabIndex = 23;
            this.BtnActionsToSkip.Text = "Actions To Skip";
            this.BtnActionsToSkip.UseVisualStyleBackColor = true;
            this.BtnActionsToSkip.Click += new System.EventHandler(this.BtnActionsToSkip_Click);
            this.BtnActionsToSkip.MouseEnter += new System.EventHandler(this.BtnActionsToSkip_MouseEnter);
            // 
            // BtnOpenActionPathDialog
            // 
            this.BtnOpenActionPathDialog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpProvider1.SetHelpString(this.BtnOpenActionPathDialog, "Overrides Capitalization of Entity Attributes");
            this.BtnOpenActionPathDialog.Location = new System.Drawing.Point(837, 6);
            this.BtnOpenActionPathDialog.Name = "BtnOpenActionPathDialog";
            this.helpProvider1.SetShowHelp(this.BtnOpenActionPathDialog, true);
            this.BtnOpenActionPathDialog.Size = new System.Drawing.Size(27, 23);
            this.BtnOpenActionPathDialog.TabIndex = 26;
            this.BtnOpenActionPathDialog.Text = "...";
            this.BtnOpenActionPathDialog.UseVisualStyleBackColor = true;
            this.BtnOpenActionPathDialog.Click += new System.EventHandler(this.BtnOpenActionPathDialog_Click);
            this.BtnOpenActionPathDialog.MouseEnter += new System.EventHandler(this.ShowActionPathText);
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
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.actionsTab);
            this.tabControl1.Location = new System.Drawing.Point(3, 172);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(878, 194);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.ChkGenerateEntityRelationships);
            this.tabPage1.Controls.Add(this.ChkMakeReadonlyFieldsEditable);
            this.tabPage1.Controls.Add(this.ChkGenerateAttributeNameConsts);
            this.tabPage1.Controls.Add(this.ChkGenerateAnonymousTypeConstructor);
            this.tabPage1.Controls.Add(this.LblEntitiesDirectory);
            this.tabPage1.Controls.Add(this.tableLayoutPanel1);
            this.tabPage1.Controls.Add(this.BtnEntitesToSkip);
            this.tabPage1.Controls.Add(this.ChkUseXrmClient);
            this.tabPage1.Controls.Add(this.BtnOpenEntityPathDialog);
            this.tabPage1.Controls.Add(this.BtnEnumMappings);
            this.tabPage1.Controls.Add(this.BtnSpecifyAttributeNames);
            this.tabPage1.Controls.Add(this.BtnUnmappedProperties);
            this.tabPage1.Controls.Add(this.ChkGenerateOptionSetEnums);
            this.tabPage1.Controls.Add(this.ChkAddDebuggerNonUserCode);
            this.tabPage1.Controls.Add(this.TxtServiceContextName);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.ChkCreateOneEntityFile);
            this.tabPage1.Controls.Add(this.TxtEntityPath);
            this.tabPage1.Controls.Add(this.LblEntityPath);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(870, 168);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Entities";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // ChkMakeReadonlyFieldsEditable
            // 
            this.ChkMakeReadonlyFieldsEditable.AutoSize = true;
            this.ChkMakeReadonlyFieldsEditable.Location = new System.Drawing.Point(483, 70);
            this.ChkMakeReadonlyFieldsEditable.Name = "ChkMakeReadonlyFieldsEditable";
            this.ChkMakeReadonlyFieldsEditable.Size = new System.Drawing.Size(172, 17);
            this.ChkMakeReadonlyFieldsEditable.TabIndex = 28;
            this.ChkMakeReadonlyFieldsEditable.Text = "Make Readonly Fields Editable";
            this.ChkMakeReadonlyFieldsEditable.UseVisualStyleBackColor = true;
            this.ChkMakeReadonlyFieldsEditable.MouseEnter += new System.EventHandler(this.ChkMakeReadonlyFieldsEditable_MouseEnter);
            // 
            // ChkGenerateAttributeNameConsts
            // 
            this.ChkGenerateAttributeNameConsts.AutoSize = true;
            this.ChkGenerateAttributeNameConsts.Location = new System.Drawing.Point(164, 52);
            this.ChkGenerateAttributeNameConsts.Name = "ChkGenerateAttributeNameConsts";
            this.ChkGenerateAttributeNameConsts.Size = new System.Drawing.Size(178, 17);
            this.ChkGenerateAttributeNameConsts.TabIndex = 27;
            this.ChkGenerateAttributeNameConsts.Text = "Generate Attribute Name Consts";
            this.ChkGenerateAttributeNameConsts.UseVisualStyleBackColor = true;
            this.ChkGenerateAttributeNameConsts.MouseEnter += new System.EventHandler(this.ChkGenerateAttributeNameConsts_MouseEnter);
            // 
            // ChkGenerateAnonymousTypeConstructor
            // 
            this.ChkGenerateAnonymousTypeConstructor.AutoSize = true;
            this.ChkGenerateAnonymousTypeConstructor.Location = new System.Drawing.Point(164, 75);
            this.ChkGenerateAnonymousTypeConstructor.Name = "ChkGenerateAnonymousTypeConstructor";
            this.ChkGenerateAnonymousTypeConstructor.Size = new System.Drawing.Size(214, 17);
            this.ChkGenerateAnonymousTypeConstructor.TabIndex = 26;
            this.ChkGenerateAnonymousTypeConstructor.Text = "Generate AnonymousType Constructors";
            this.ChkGenerateAnonymousTypeConstructor.UseVisualStyleBackColor = true;
            this.ChkGenerateAnonymousTypeConstructor.MouseEnter += new System.EventHandler(this.ChkGenerateAnonymousTypeConstructor_MouseEnter);
            // 
            // LblEntitiesDirectory
            // 
            this.LblEntitiesDirectory.AutoSize = true;
            this.LblEntitiesDirectory.Location = new System.Drawing.Point(361, 11);
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
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 145);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(858, 23);
            this.tableLayoutPanel1.TabIndex = 24;
            // 
            // ChkAddDebuggerNonUserCode
            // 
            this.ChkAddDebuggerNonUserCode.AutoSize = true;
            this.ChkAddDebuggerNonUserCode.Location = new System.Drawing.Point(164, 6);
            this.ChkAddDebuggerNonUserCode.Name = "ChkAddDebuggerNonUserCode";
            this.ChkAddDebuggerNonUserCode.Size = new System.Drawing.Size(156, 17);
            this.ChkAddDebuggerNonUserCode.TabIndex = 19;
            this.ChkAddDebuggerNonUserCode.Text = "Add Debug Non User Code";
            this.ChkAddDebuggerNonUserCode.UseVisualStyleBackColor = true;
            this.ChkAddDebuggerNonUserCode.MouseEnter += new System.EventHandler(this.ChkAddDebuggerNonUserCode_MouseEnter);
            // 
            // tabPage2
            // 
            this.tabPage2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tabPage2.Controls.Add(this.LblOptionSetFormat);
            this.tabPage2.Controls.Add(this.TxtOptionSetFormat);
            this.tabPage2.Controls.Add(this.ChkUseDeprecatedOptionSetNaming);
            this.tabPage2.Controls.Add(this.TxtInvalidCSharpNamePrefix);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.LblOptionSetsDirectory);
            this.tabPage2.Controls.Add(this.LblOptionSetPath);
            this.tabPage2.Controls.Add(this.ChkCreateOneOptionSetFile);
            this.tabPage2.Controls.Add(this.tableLayoutPanel2);
            this.tabPage2.Controls.Add(this.TxtOptionSetPath);
            this.tabPage2.Controls.Add(this.BtnOpenOptionSetPathDialog);
            this.tabPage2.Controls.Add(this.BtnOptionSetsToSkip);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(870, 168);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Option Sets";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // LblOptionSetFormat
            // 
            this.LblOptionSetFormat.AutoSize = true;
            this.LblOptionSetFormat.Location = new System.Drawing.Point(174, 55);
            this.LblOptionSetFormat.Name = "LblOptionSetFormat";
            this.LblOptionSetFormat.Size = new System.Drawing.Size(102, 13);
            this.LblOptionSetFormat.TabIndex = 32;
            this.LblOptionSetFormat.Text = "Local Name Format:";
            // 
            // TxtOptionSetFormat
            // 
            this.TxtOptionSetFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtOptionSetFormat.Location = new System.Drawing.Point(282, 53);
            this.TxtOptionSetFormat.Name = "TxtOptionSetFormat";
            this.TxtOptionSetFormat.Size = new System.Drawing.Size(85, 20);
            this.TxtOptionSetFormat.TabIndex = 31;
            this.TxtOptionSetFormat.MouseEnter += new System.EventHandler(this.TxtOptionSetFormat_MouseEnter);
            // 
            // ChkUseDeprecatedOptionSetNaming
            // 
            this.ChkUseDeprecatedOptionSetNaming.AutoSize = true;
            this.ChkUseDeprecatedOptionSetNaming.Location = new System.Drawing.Point(164, 29);
            this.ChkUseDeprecatedOptionSetNaming.Name = "ChkUseDeprecatedOptionSetNaming";
            this.ChkUseDeprecatedOptionSetNaming.Size = new System.Drawing.Size(190, 17);
            this.ChkUseDeprecatedOptionSetNaming.TabIndex = 30;
            this.ChkUseDeprecatedOptionSetNaming.Text = "Use Deprecated OptionSet Names";
            this.ChkUseDeprecatedOptionSetNaming.UseVisualStyleBackColor = true;
            this.ChkUseDeprecatedOptionSetNaming.CheckedChanged += new System.EventHandler(this.ChkUseDeprecatedOptionSetNaming_CheckedChanged);
            this.ChkUseDeprecatedOptionSetNaming.MouseEnter += new System.EventHandler(this.ChkUseDeprecatedOptionSetNaming_MouseEnter);
            // 
            // TxtInvalidCSharpNamePrefix
            // 
            this.TxtInvalidCSharpNamePrefix.Location = new System.Drawing.Point(282, 79);
            this.TxtInvalidCSharpNamePrefix.Name = "TxtInvalidCSharpNamePrefix";
            this.TxtInvalidCSharpNamePrefix.Size = new System.Drawing.Size(45, 20);
            this.TxtInvalidCSharpNamePrefix.TabIndex = 29;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(155, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(121, 13);
            this.label3.TabIndex = 28;
            this.label3.Text = "Prefix For Invalid Names";
            // 
            // LblOptionSetPath
            // 
            this.LblOptionSetPath.AutoSize = true;
            this.LblOptionSetPath.Location = new System.Drawing.Point(379, 11);
            this.LblOptionSetPath.Name = "LblOptionSetPath";
            this.LblOptionSetPath.Size = new System.Drawing.Size(124, 13);
            this.LblOptionSetPath.TabIndex = 27;
            this.LblOptionSetPath.Text = "OptionSet Relative Path:";
            // 
            // ChkCreateOneOptionSetFile
            // 
            this.ChkCreateOneOptionSetFile.AutoSize = true;
            this.ChkCreateOneOptionSetFile.Location = new System.Drawing.Point(164, 6);
            this.ChkCreateOneOptionSetFile.Name = "ChkCreateOneOptionSetFile";
            this.ChkCreateOneOptionSetFile.Size = new System.Drawing.Size(171, 17);
            this.ChkCreateOneOptionSetFile.TabIndex = 26;
            this.ChkCreateOneOptionSetFile.Text = "Create One File Per Option Set";
            this.ChkCreateOneOptionSetFile.UseVisualStyleBackColor = true;
            this.ChkCreateOneOptionSetFile.CheckedChanged += new System.EventHandler(this.ChkCreateOneOptionSetFile_CheckedChanged);
            this.ChkCreateOneOptionSetFile.MouseEnter += new System.EventHandler(this.ChkCreateOneOptionSetFile_MouseEnter);
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
            this.tableLayoutPanel2.Location = new System.Drawing.Point(6, 124);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(858, 23);
            this.tableLayoutPanel2.TabIndex = 25;
            // 
            // actionsTab
            // 
            this.actionsTab.Controls.Add(this.LblActionsDirectory);
            this.actionsTab.Controls.Add(this.ChkCreateOneActionFile);
            this.actionsTab.Controls.Add(this.tableLayoutPanel3);
            this.actionsTab.Controls.Add(this.TxtActionPath);
            this.actionsTab.Controls.Add(this.BtnOpenActionPathDialog);
            this.actionsTab.Controls.Add(this.LblActionPath);
            this.actionsTab.Controls.Add(this.BtnActionsToSkip);
            this.actionsTab.Location = new System.Drawing.Point(4, 22);
            this.actionsTab.Name = "actionsTab";
            this.actionsTab.Padding = new System.Windows.Forms.Padding(3);
            this.actionsTab.Size = new System.Drawing.Size(870, 168);
            this.actionsTab.TabIndex = 2;
            this.actionsTab.Text = "Actions";
            this.actionsTab.UseVisualStyleBackColor = true;
            this.actionsTab.Enter += new System.EventHandler(this.actionsTab_Enter);
            // 
            // LblActionsDirectory
            // 
            this.LblActionsDirectory.AutoSize = true;
            this.LblActionsDirectory.Location = new System.Drawing.Point(353, 11);
            this.LblActionsDirectory.Name = "LblActionsDirectory";
            this.LblActionsDirectory.Size = new System.Drawing.Size(112, 13);
            this.LblActionsDirectory.TabIndex = 29;
            this.LblActionsDirectory.Text = "Actions Relative Path:";
            // 
            // ChkCreateOneActionFile
            // 
            this.ChkCreateOneActionFile.AutoSize = true;
            this.ChkCreateOneActionFile.Location = new System.Drawing.Point(164, 6);
            this.ChkCreateOneActionFile.Name = "ChkCreateOneActionFile";
            this.ChkCreateOneActionFile.Size = new System.Drawing.Size(151, 17);
            this.ChkCreateOneActionFile.TabIndex = 28;
            this.ChkCreateOneActionFile.Text = "Create One File Per Action";
            this.ChkCreateOneActionFile.UseVisualStyleBackColor = true;
            this.ChkCreateOneActionFile.CheckedChanged += new System.EventHandler(this.ChkCreateOneActionFile_CheckedChanged);
            this.ChkCreateOneActionFile.MouseEnter += new System.EventHandler(this.ChkCreateOneActionFile_MouseEnter);
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
            this.tableLayoutPanel3.Location = new System.Drawing.Point(6, 124);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(858, 23);
            this.tableLayoutPanel3.TabIndex = 27;
            // 
            // BtnCreateActions
            // 
            this.BtnCreateActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnCreateActions.Location = new System.Drawing.Point(327, 0);
            this.BtnCreateActions.Margin = new System.Windows.Forms.Padding(0);
            this.BtnCreateActions.Name = "BtnCreateActions";
            this.BtnCreateActions.Size = new System.Drawing.Size(204, 23);
            this.BtnCreateActions.TabIndex = 1;
            this.BtnCreateActions.Text = "Create Actions";
            this.BtnCreateActions.UseVisualStyleBackColor = true;
            this.BtnCreateActions.Click += new System.EventHandler(this.BtnCreateActions_Click);
            this.BtnCreateActions.MouseEnter += new System.EventHandler(this.BtnCreateActions_MouseEnter);
            // 
            // TxtActionPath
            // 
            this.TxtActionPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtActionPath.Location = new System.Drawing.Point(483, 8);
            this.TxtActionPath.Name = "TxtActionPath";
            this.TxtActionPath.Size = new System.Drawing.Size(348, 20);
            this.TxtActionPath.TabIndex = 24;
            this.TxtActionPath.MouseEnter += new System.EventHandler(this.ShowActionPathText);
            // 
            // LblActionPath
            // 
            this.LblActionPath.AutoSize = true;
            this.LblActionPath.Location = new System.Drawing.Point(353, 11);
            this.LblActionPath.Name = "LblActionPath";
            this.LblActionPath.Size = new System.Drawing.Size(107, 13);
            this.LblActionPath.TabIndex = 25;
            this.LblActionPath.Text = "Action Relative Path:";
            // 
            // ChkGenerateEntityRelationships
            // 
            this.ChkGenerateEntityRelationships.AutoSize = true;
            this.ChkGenerateEntityRelationships.Location = new System.Drawing.Point(164, 98);
            this.ChkGenerateEntityRelationships.Name = "ChkGenerateEntityRelationships";
            this.ChkGenerateEntityRelationships.Size = new System.Drawing.Size(165, 17);
            this.ChkGenerateEntityRelationships.TabIndex = 29;
            this.ChkGenerateEntityRelationships.Text = "Generate Entity Relationships";
            this.ChkGenerateEntityRelationships.UseVisualStyleBackColor = true;
            this.ChkGenerateEntityRelationships.MouseEnter += new System.EventHandler(this.ChkGenerateEntityRelationships_MouseEnter);
            // 
            // EarlyBoundGeneratorPlugin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.TxtOutput);
            this.Controls.Add(this.groupBox1);
            this.Name = "EarlyBoundGeneratorPlugin";
            this.helpProvider1.SetShowHelp(this, true);
            this.Size = new System.Drawing.Size(890, 541);
            this.ConnectionUpdated += new XrmToolBox.Extensibility.PluginControlBase.ConnectionUpdatedHandler(this.EarlyBoundGenerator_ConnectionUpdated);
            this.Load += new System.EventHandler(this.EarlyBoundGenerator_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.actionsTab.ResumeLayout(false);
            this.actionsTab.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage actionsTab;
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
    }
}
