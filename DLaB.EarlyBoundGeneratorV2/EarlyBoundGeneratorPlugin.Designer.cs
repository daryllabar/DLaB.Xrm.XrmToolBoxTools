namespace DLaB.EarlyBoundGeneratorV2
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label label4;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EarlyBoundGeneratorPlugin));
            this.BtnCreateAll = new System.Windows.Forms.Button();
            this.TxtOutput = new System.Windows.Forms.TextBox();
            this.BtnOpenSettingsPathDialog = new System.Windows.Forms.Button();
            this.TxtSettingsPath = new System.Windows.Forms.TextBox();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.BtnSaveSettingsPathDialog = new System.Windows.Forms.Button();
            this.BtnReloadSettings = new System.Windows.Forms.Button();
            this.BtnResetSettings = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.PropertiesGrid = new DLaB.XrmToolBoxCommon.Controls.SearchablePropertyGrid();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(7, 18);
            label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(73, 13);
            label4.TabIndex = 14;
            label4.Text = "Settings Path:";
            // 
            // BtnCreateAll
            // 
            this.BtnCreateAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnCreateAll.Location = new System.Drawing.Point(245, 0);
            this.BtnCreateAll.Margin = new System.Windows.Forms.Padding(0);
            this.BtnCreateAll.Name = "BtnCreateAll";
            this.BtnCreateAll.Size = new System.Drawing.Size(200, 42);
            this.BtnCreateAll.TabIndex = 1;
            this.BtnCreateAll.Text = "Generate";
            this.BtnCreateAll.UseVisualStyleBackColor = true;
            this.BtnCreateAll.Click += new System.EventHandler(this.BtnCreateBoth_Click);
            // 
            // TxtOutput
            // 
            this.TxtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtOutput.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtOutput.Location = new System.Drawing.Point(6, 60);
            this.TxtOutput.Margin = new System.Windows.Forms.Padding(6);
            this.TxtOutput.Multiline = true;
            this.TxtOutput.Name = "TxtOutput";
            this.TxtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TxtOutput.Size = new System.Drawing.Size(688, 930);
            this.TxtOutput.TabIndex = 1;
            this.TxtOutput.WordWrap = false;
            this.TxtOutput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtOutput_KeyDown);
            // 
            // BtnOpenSettingsPathDialog
            // 
            this.BtnOpenSettingsPathDialog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpProvider1.SetHelpString(this.BtnOpenSettingsPathDialog, "Overrides Capitalization of Entity Attributes");
            this.BtnOpenSettingsPathDialog.Image = ((System.Drawing.Image)(resources.GetObject("BtnOpenSettingsPathDialog.Image")));
            this.BtnOpenSettingsPathDialog.Location = new System.Drawing.Point(779, 4);
            this.BtnOpenSettingsPathDialog.Margin = new System.Windows.Forms.Padding(6);
            this.BtnOpenSettingsPathDialog.Name = "BtnOpenSettingsPathDialog";
            this.helpProvider1.SetShowHelp(this.BtnOpenSettingsPathDialog, true);
            this.BtnOpenSettingsPathDialog.Size = new System.Drawing.Size(50, 41);
            this.BtnOpenSettingsPathDialog.TabIndex = 15;
            this.toolTip1.SetToolTip(this.BtnOpenSettingsPathDialog, "Open existing settings file");
            this.BtnOpenSettingsPathDialog.UseVisualStyleBackColor = true;
            this.BtnOpenSettingsPathDialog.Click += new System.EventHandler(this.BtnOpenSettingsPathDialog_Click);
            // 
            // TxtSettingsPath
            // 
            this.TxtSettingsPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtSettingsPath.Location = new System.Drawing.Point(92, 15);
            this.TxtSettingsPath.Margin = new System.Windows.Forms.Padding(6);
            this.TxtSettingsPath.Name = "TxtSettingsPath";
            this.TxtSettingsPath.Size = new System.Drawing.Size(551, 20);
            this.TxtSettingsPath.TabIndex = 13;
            this.TxtSettingsPath.Leave += new System.EventHandler(this.TxtSettingsPath_Leave);
            // 
            // BtnSaveSettingsPathDialog
            // 
            this.BtnSaveSettingsPathDialog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpProvider1.SetHelpString(this.BtnSaveSettingsPathDialog, "Overrides Capitalization of Entity Attributes");
            this.BtnSaveSettingsPathDialog.Image = ((System.Drawing.Image)(resources.GetObject("BtnSaveSettingsPathDialog.Image")));
            this.BtnSaveSettingsPathDialog.Location = new System.Drawing.Point(841, 4);
            this.BtnSaveSettingsPathDialog.Margin = new System.Windows.Forms.Padding(6);
            this.BtnSaveSettingsPathDialog.Name = "BtnSaveSettingsPathDialog";
            this.helpProvider1.SetShowHelp(this.BtnSaveSettingsPathDialog, true);
            this.BtnSaveSettingsPathDialog.Size = new System.Drawing.Size(50, 41);
            this.BtnSaveSettingsPathDialog.TabIndex = 37;
            this.toolTip1.SetToolTip(this.BtnSaveSettingsPathDialog, "Save to new settings file");
            this.BtnSaveSettingsPathDialog.UseVisualStyleBackColor = true;
            this.BtnSaveSettingsPathDialog.Click += new System.EventHandler(this.BtnSaveSettingsPathDialog_Click);
            // 
            // BtnReloadSettings
            // 
            this.BtnReloadSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpProvider1.SetHelpString(this.BtnReloadSettings, "Overrides Capitalization of Entity Attributes");
            this.BtnReloadSettings.Image = ((System.Drawing.Image)(resources.GetObject("BtnReloadSettings.Image")));
            this.BtnReloadSettings.Location = new System.Drawing.Point(717, 6);
            this.BtnReloadSettings.Margin = new System.Windows.Forms.Padding(6);
            this.BtnReloadSettings.Name = "BtnReloadSettings";
            this.helpProvider1.SetShowHelp(this.BtnReloadSettings, true);
            this.BtnReloadSettings.Size = new System.Drawing.Size(50, 41);
            this.BtnReloadSettings.TabIndex = 38;
            this.toolTip1.SetToolTip(this.BtnReloadSettings, "Reload Settings From File");
            this.BtnReloadSettings.UseVisualStyleBackColor = true;
            this.BtnReloadSettings.Click += new System.EventHandler(this.BtnRefreshSettings_Click);
            // 
            // BtnResetSettings
            // 
            this.BtnResetSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpProvider1.SetHelpString(this.BtnResetSettings, "Overrides Capitalization of Entity Attributes");
            this.BtnResetSettings.Image = ((System.Drawing.Image)(resources.GetObject("BtnResetSettings.Image")));
            this.BtnResetSettings.Location = new System.Drawing.Point(655, 7);
            this.BtnResetSettings.Margin = new System.Windows.Forms.Padding(6);
            this.BtnResetSettings.Name = "BtnResetSettings";
            this.helpProvider1.SetShowHelp(this.BtnResetSettings, true);
            this.BtnResetSettings.Size = new System.Drawing.Size(50, 41);
            this.BtnResetSettings.TabIndex = 39;
            this.toolTip1.SetToolTip(this.BtnResetSettings, "Reset Settings to Default");
            this.BtnResetSettings.UseVisualStyleBackColor = true;
            this.BtnResetSettings.Click += new System.EventHandler(this.BtnResetSettings_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.CheckFileExists = false;
            this.openFileDialog1.DefaultExt = "cs";
            this.openFileDialog1.Filter = "C# files|*.cs";
            // 
            // PropertiesGrid
            // 
            this.PropertiesGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PropertiesGrid.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PropertiesGrid.Location = new System.Drawing.Point(4, 54);
            this.PropertiesGrid.Margin = new System.Windows.Forms.Padding(6);
            this.PropertiesGrid.Name = "PropertiesGrid";
            this.PropertiesGrid.Size = new System.Drawing.Size(887, 945);
            this.PropertiesGrid.TabIndex = 36;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(6);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Panel1.Controls.Add(this.TxtOutput);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.BtnResetSettings);
            this.splitContainer1.Panel2.Controls.Add(this.BtnSaveSettingsPathDialog);
            this.splitContainer1.Panel2.Controls.Add(this.BtnReloadSettings);
            this.splitContainer1.Panel2.Controls.Add(this.PropertiesGrid);
            this.splitContainer1.Panel2.Controls.Add(this.BtnOpenSettingsPathDialog);
            this.splitContainer1.Panel2.Controls.Add(this.TxtSettingsPath);
            this.splitContainer1.Panel2.Controls.Add(label4);
            this.splitContainer1.Panel2MinSize = 500;
            this.splitContainer1.Size = new System.Drawing.Size(1632, 999);
            this.splitContainer1.SplitterDistance = 702;
            this.splitContainer1.SplitterWidth = 7;
            this.splitContainer1.TabIndex = 37;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.BtnCreateAll, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 6);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(690, 42);
            this.tableLayoutPanel1.TabIndex = 28;
            // 
            // EarlyBoundGeneratorPlugin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "EarlyBoundGeneratorPlugin";
            this.helpProvider1.SetShowHelp(this, true);
            this.Size = new System.Drawing.Size(1632, 999);
            this.ConnectionUpdated += new XrmToolBox.Extensibility.PluginControlBase.ConnectionUpdatedHandler(this.EarlyBoundGenerator_ConnectionUpdated);
            this.Load += new System.EventHandler(this.EarlyBoundGenerator_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TextBox TxtOutput;
        private System.Windows.Forms.Button BtnCreateAll;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button BtnOpenSettingsPathDialog;
        private System.Windows.Forms.TextBox TxtSettingsPath;
        private DLaB.XrmToolBoxCommon.Controls.SearchablePropertyGrid PropertiesGrid;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button BtnSaveSettingsPathDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button BtnReloadSettings;
        private System.Windows.Forms.Button BtnResetSettings;
    }
}
