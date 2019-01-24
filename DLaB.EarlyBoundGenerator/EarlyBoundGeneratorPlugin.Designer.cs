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
            this.BtnOpenSettingsPathDialog = new System.Windows.Forms.Button();
            this.TxtSettingsPath = new System.Windows.Forms.TextBox();
            this.BtnUnmappedProperties = new System.Windows.Forms.Button();
            this.BtnEnumMappings = new System.Windows.Forms.Button();
            this.BtnSpecifyAttributeNames = new System.Windows.Forms.Button();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.GlobalTab = new System.Windows.Forms.TabPage();
            this.EntityTab = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.OptionSetTab = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.ActionTab = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.BtnCreateActions = new System.Windows.Forms.Button();
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
            this.BtnCreateOptionSets.Location = new System.Drawing.Point(82, 0);
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
            this.BtnCreateAll.Location = new System.Drawing.Point(6, 19);
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
            this.BtnCreateEntities.Location = new System.Drawing.Point(86, 0);
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
            this.groupBox1.Controls.Add(this.BtnCreateAll);
            this.groupBox1.Location = new System.Drawing.Point(10, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(481, 181);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
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
            this.EntityTab.Controls.Add(this.tableLayoutPanel1);
            this.EntityTab.Controls.Add(this.BtnEnumMappings);
            this.EntityTab.Controls.Add(this.BtnSpecifyAttributeNames);
            this.EntityTab.Controls.Add(this.BtnUnmappedProperties);
            this.EntityTab.Location = new System.Drawing.Point(4, 22);
            this.EntityTab.Name = "EntityTab";
            this.EntityTab.Padding = new System.Windows.Forms.Padding(3);
            this.EntityTab.Size = new System.Drawing.Size(381, 326);
            this.EntityTab.TabIndex = 0;
            this.EntityTab.Text = "Entities";
            this.EntityTab.UseVisualStyleBackColor = true;
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
            this.tableLayoutPanel1.Size = new System.Drawing.Size(376, 23);
            this.tableLayoutPanel1.TabIndex = 24;
            // 
            // OptionSetTab
            // 
            this.OptionSetTab.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.OptionSetTab.Controls.Add(this.tableLayoutPanel2);
            this.OptionSetTab.Location = new System.Drawing.Point(4, 22);
            this.OptionSetTab.Name = "OptionSetTab";
            this.OptionSetTab.Padding = new System.Windows.Forms.Padding(3);
            this.OptionSetTab.Size = new System.Drawing.Size(381, 326);
            this.OptionSetTab.TabIndex = 1;
            this.OptionSetTab.Text = "Option Sets";
            this.OptionSetTab.UseVisualStyleBackColor = true;
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
            this.tableLayoutPanel2.Location = new System.Drawing.Point(6, 268);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(369, 23);
            this.tableLayoutPanel2.TabIndex = 25;
            // 
            // ActionTab
            // 
            this.ActionTab.Controls.Add(this.tableLayoutPanel3);
            this.ActionTab.Location = new System.Drawing.Point(4, 22);
            this.ActionTab.Name = "ActionTab";
            this.ActionTab.Padding = new System.Windows.Forms.Padding(3);
            this.ActionTab.Size = new System.Drawing.Size(381, 326);
            this.ActionTab.TabIndex = 2;
            this.ActionTab.Text = "Actions";
            this.ActionTab.UseVisualStyleBackColor = true;
            this.ActionTab.Enter += new System.EventHandler(this.actionsTab_Enter);
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
            this.tableLayoutPanel3.Size = new System.Drawing.Size(366, 23);
            this.tableLayoutPanel3.TabIndex = 27;
            // 
            // BtnCreateActions
            // 
            this.BtnCreateActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnCreateActions.Location = new System.Drawing.Point(81, 0);
            this.BtnCreateActions.Margin = new System.Windows.Forms.Padding(0);
            this.BtnCreateActions.Name = "BtnCreateActions";
            this.BtnCreateActions.Size = new System.Drawing.Size(204, 23);
            this.BtnCreateActions.TabIndex = 1;
            this.BtnCreateActions.Text = "Create Actions";
            this.BtnCreateActions.UseVisualStyleBackColor = true;
            this.BtnCreateActions.Click += new System.EventHandler(this.BtnCreateActions_Click);
            // 
            // PropertiesGrid
            // 
            this.PropertiesGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PropertiesGrid.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PropertiesGrid.Location = new System.Drawing.Point(0, 32);
            this.PropertiesGrid.Name = "PropertiesGrid";
            this.PropertiesGrid.Size = new System.Drawing.Size(489, 503);
            this.PropertiesGrid.TabIndex = 36;
            this.PropertiesGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.PropertiesGrid_PropertyValueChanged);
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
            this.tabControl1.ResumeLayout(false);
            this.GlobalTab.ResumeLayout(false);
            this.EntityTab.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.OptionSetTab.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ActionTab.ResumeLayout(false);
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
        private System.Windows.Forms.Button BtnSpecifyAttributeNames;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.Button BtnUnmappedProperties;
        private System.Windows.Forms.Button BtnEnumMappings;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage EntityTab;
        private System.Windows.Forms.TabPage OptionSetTab;
        private System.Windows.Forms.TabPage ActionTab;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button BtnCreateActions;
        private System.Windows.Forms.Button BtnOpenSettingsPathDialog;
        private System.Windows.Forms.TextBox TxtSettingsPath;
        private System.Windows.Forms.TabPage GlobalTab;
        private System.Windows.Forms.PropertyGrid PropertiesGrid;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}
