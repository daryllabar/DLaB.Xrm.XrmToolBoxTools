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
            this.BtnOpenSettingsPathDialog = new System.Windows.Forms.Button();
            this.TxtSettingsPath = new System.Windows.Forms.TextBox();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.BtnCreateActions = new System.Windows.Forms.Button();
            this.PropertiesGrid = new System.Windows.Forms.PropertyGrid();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            label4 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3.SuspendLayout();
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
            label4.Location = new System.Drawing.Point(3, 6);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(73, 13);
            label4.TabIndex = 14;
            label4.Text = "Settings Path:";
            // 
            // BtnCreateOptionSets
            // 
            this.BtnCreateOptionSets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnCreateOptionSets.Location = new System.Drawing.Point(131, 0);
            this.BtnCreateOptionSets.Margin = new System.Windows.Forms.Padding(0);
            this.BtnCreateOptionSets.Name = "BtnCreateOptionSets";
            this.BtnCreateOptionSets.Size = new System.Drawing.Size(125, 23);
            this.BtnCreateOptionSets.TabIndex = 1;
            this.BtnCreateOptionSets.Text = "Create OptionSets";
            this.BtnCreateOptionSets.UseVisualStyleBackColor = true;
            this.BtnCreateOptionSets.Click += new System.EventHandler(this.BtnCreateOptionSets_Click);
            // 
            // BtnCreateAll
            // 
            this.BtnCreateAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnCreateAll.Location = new System.Drawing.Point(92, 0);
            this.BtnCreateAll.Margin = new System.Windows.Forms.Padding(0);
            this.BtnCreateAll.Name = "BtnCreateAll";
            this.BtnCreateAll.Size = new System.Drawing.Size(200, 23);
            this.BtnCreateAll.TabIndex = 1;
            this.BtnCreateAll.Text = "Create All";
            this.BtnCreateAll.UseVisualStyleBackColor = true;
            this.BtnCreateAll.Click += new System.EventHandler(this.BtnCreateBoth_Click);
            // 
            // BtnCreateEntities
            // 
            this.BtnCreateEntities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnCreateEntities.Location = new System.Drawing.Point(6, 0);
            this.BtnCreateEntities.Margin = new System.Windows.Forms.Padding(0);
            this.BtnCreateEntities.Name = "BtnCreateEntities";
            this.BtnCreateEntities.Size = new System.Drawing.Size(125, 23);
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
            this.TxtOutput.Location = new System.Drawing.Point(3, 61);
            this.TxtOutput.Multiline = true;
            this.TxtOutput.Name = "TxtOutput";
            this.TxtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TxtOutput.Size = new System.Drawing.Size(377, 471);
            this.TxtOutput.TabIndex = 1;
            this.TxtOutput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtOutput_KeyDown);
            // 
            // BtnOpenSettingsPathDialog
            // 
            this.BtnOpenSettingsPathDialog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.helpProvider1.SetHelpString(this.BtnOpenSettingsPathDialog, "Overrides Capitalization of Entity Attributes");
            this.BtnOpenSettingsPathDialog.Location = new System.Drawing.Point(470, 3);
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
            this.TxtSettingsPath.Location = new System.Drawing.Point(82, 3);
            this.TxtSettingsPath.Name = "TxtSettingsPath";
            this.TxtSettingsPath.Size = new System.Drawing.Size(382, 20);
            this.TxtSettingsPath.TabIndex = 13;
            this.TxtSettingsPath.Leave += new System.EventHandler(this.TxtSettingsPath_Leave);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.CheckFileExists = false;
            this.openFileDialog1.DefaultExt = "cs";
            this.openFileDialog1.Filter = "C# files|*.cs";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.ColumnCount = 5;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 125F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 125F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 125F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.BtnCreateActions, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.BtnCreateOptionSets, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.BtnCreateEntities, 1, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(-3, 32);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(387, 23);
            this.tableLayoutPanel3.TabIndex = 27;
            // 
            // BtnCreateActions
            // 
            this.BtnCreateActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnCreateActions.Location = new System.Drawing.Point(256, 0);
            this.BtnCreateActions.Margin = new System.Windows.Forms.Padding(0);
            this.BtnCreateActions.Name = "BtnCreateActions";
            this.BtnCreateActions.Size = new System.Drawing.Size(125, 23);
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
            this.PropertiesGrid.Location = new System.Drawing.Point(2, 29);
            this.PropertiesGrid.Name = "PropertiesGrid";
            this.PropertiesGrid.Size = new System.Drawing.Size(495, 506);
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
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Panel1.Controls.Add(this.TxtOutput);
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.PropertiesGrid);
            this.splitContainer1.Panel2.Controls.Add(this.BtnOpenSettingsPathDialog);
            this.splitContainer1.Panel2.Controls.Add(this.TxtSettingsPath);
            this.splitContainer1.Panel2.Controls.Add(label4);
            this.splitContainer1.Panel2MinSize = 500;
            this.splitContainer1.Size = new System.Drawing.Size(887, 535);
            this.splitContainer1.SplitterDistance = 383;
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
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(384, 23);
            this.tableLayoutPanel1.TabIndex = 28;
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
            this.tableLayoutPanel3.ResumeLayout(false);
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

        private System.Windows.Forms.Button BtnCreateOptionSets;
        private System.Windows.Forms.Button BtnCreateEntities;
        private System.Windows.Forms.TextBox TxtOutput;
        private System.Windows.Forms.Button BtnCreateAll;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button BtnCreateActions;
        private System.Windows.Forms.Button BtnOpenSettingsPathDialog;
        private System.Windows.Forms.TextBox TxtSettingsPath;
        private System.Windows.Forms.PropertyGrid PropertiesGrid;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
