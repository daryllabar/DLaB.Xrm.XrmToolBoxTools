namespace DLaB.XrmToolBoxCommon.Forms
{
    partial class SpecifyEntitiesDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.BtnSave = new System.Windows.Forms.Button();
            this.BtnAdd = new System.Windows.Forms.Button();
            this.BtnRemove = new System.Windows.Forms.Button();
            this.BtnRefresh = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.bgEntitiesKeep = new System.Windows.Forms.GroupBox();
            this.lvKeptEntities = new System.Windows.Forms.ListView();
            this.chDisplayName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chLogicalName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gbEntitiesExclude = new System.Windows.Forms.GroupBox();
            this.lvExcludedEntities = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel4 = new System.Windows.Forms.Panel();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1.SuspendLayout();
            this.bgEntitiesKeep.SuspendLayout();
            this.gbEntitiesExclude.SuspendLayout();
            this.panel4.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnSave
            // 
            this.BtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnSave.Location = new System.Drawing.Point(540, 15);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(75, 23);
            this.BtnSave.TabIndex = 0;
            this.BtnSave.Text = "Save";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // BtnAdd
            // 
            this.BtnAdd.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.BtnAdd.Location = new System.Drawing.Point(6, 156);
            this.BtnAdd.Name = "BtnAdd";
            this.BtnAdd.Size = new System.Drawing.Size(22, 23);
            this.BtnAdd.TabIndex = 4;
            this.BtnAdd.Text = ">";
            this.BtnAdd.UseVisualStyleBackColor = true;
            this.BtnAdd.Click += new System.EventHandler(this.BtnAdd_Click);
            // 
            // BtnRemove
            // 
            this.BtnRemove.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.BtnRemove.Location = new System.Drawing.Point(6, 185);
            this.BtnRemove.Name = "BtnRemove";
            this.BtnRemove.Size = new System.Drawing.Size(22, 25);
            this.BtnRemove.TabIndex = 5;
            this.BtnRemove.Text = "<";
            this.BtnRemove.UseVisualStyleBackColor = true;
            this.BtnRemove.Click += new System.EventHandler(this.BtnRemove_Click);
            // 
            // BtnRefresh
            // 
            this.BtnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnRefresh.Location = new System.Drawing.Point(621, 15);
            this.BtnRefresh.Name = "BtnRefresh";
            this.BtnRefresh.Size = new System.Drawing.Size(75, 23);
            this.BtnRefresh.TabIndex = 8;
            this.BtnRefresh.Text = "Refresh";
            this.BtnRefresh.UseVisualStyleBackColor = true;
            this.BtnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.bgEntitiesKeep, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.gbEntitiesExclude, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel4, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 370F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(699, 370);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // bgEntitiesKeep
            // 
            this.bgEntitiesKeep.Controls.Add(this.lvKeptEntities);
            this.bgEntitiesKeep.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bgEntitiesKeep.Location = new System.Drawing.Point(3, 3);
            this.bgEntitiesKeep.Name = "bgEntitiesKeep";
            this.bgEntitiesKeep.Size = new System.Drawing.Size(323, 364);
            this.bgEntitiesKeep.TabIndex = 0;
            this.bgEntitiesKeep.TabStop = false;
            this.bgEntitiesKeep.Text = "All Entities";
            // 
            // lvKeptEntities
            // 
            this.lvKeptEntities.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chDisplayName,
            this.chLogicalName});
            this.lvKeptEntities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvKeptEntities.FullRowSelect = true;
            this.lvKeptEntities.Location = new System.Drawing.Point(3, 16);
            this.lvKeptEntities.Name = "lvKeptEntities";
            this.lvKeptEntities.Size = new System.Drawing.Size(317, 345);
            this.lvKeptEntities.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvKeptEntities.TabIndex = 2;
            this.lvKeptEntities.UseCompatibleStateImageBehavior = false;
            this.lvKeptEntities.View = System.Windows.Forms.View.Details;
            this.lvKeptEntities.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(Helper.SortListView_OnColumnClick);
            // 
            // chDisplayName
            // 
            this.chDisplayName.Text = "Display Name";
            this.chDisplayName.Width = 150;
            // 
            // chLogicalName
            // 
            this.chLogicalName.Text = "Logical Name";
            this.chLogicalName.Width = 150;
            // 
            // gbEntitiesExclude
            // 
            this.gbEntitiesExclude.Controls.Add(this.lvExcludedEntities);
            this.gbEntitiesExclude.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbEntitiesExclude.Location = new System.Drawing.Point(372, 3);
            this.gbEntitiesExclude.Name = "gbEntitiesExclude";
            this.gbEntitiesExclude.Size = new System.Drawing.Size(324, 364);
            this.gbEntitiesExclude.TabIndex = 1;
            this.gbEntitiesExclude.TabStop = false;
            this.gbEntitiesExclude.Text = "Selected Entities";
            // 
            // lvExcludedEntities
            // 
            this.lvExcludedEntities.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvExcludedEntities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvExcludedEntities.FullRowSelect = true;
            this.lvExcludedEntities.Location = new System.Drawing.Point(3, 16);
            this.lvExcludedEntities.Name = "lvExcludedEntities";
            this.lvExcludedEntities.Size = new System.Drawing.Size(318, 345);
            this.lvExcludedEntities.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvExcludedEntities.TabIndex = 7;
            this.lvExcludedEntities.UseCompatibleStateImageBehavior = false;
            this.lvExcludedEntities.View = System.Windows.Forms.View.Details;
            this.lvExcludedEntities.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(Helper.SortListView_OnColumnClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Display Name";
            this.columnHeader1.Width = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Logical Name";
            this.columnHeader2.Width = 150;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.BtnRemove);
            this.panel4.Controls.Add(this.BtnAdd);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(332, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(34, 364);
            this.panel4.TabIndex = 2;
            // 
            // pnlFooter
            // 
            this.pnlFooter.Controls.Add(this.BtnRefresh);
            this.pnlFooter.Controls.Add(this.BtnSave);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 370);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(699, 50);
            this.pnlFooter.TabIndex = 10;
            // 
            // SpecifyEntitiesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(699, 420);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.pnlFooter);
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.MinimumSize = new System.Drawing.Size(541, 425);
            this.Name = "SpecifyEntitiesDialog";
            this.Text = "Select Entities";
            this.Load += new System.EventHandler(this.SpecifyEntitiesDialog_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.bgEntitiesKeep.ResumeLayout(false);
            this.gbEntitiesExclude.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.pnlFooter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.Button BtnAdd;
        private System.Windows.Forms.Button BtnRemove;
        private System.Windows.Forms.Button BtnRefresh;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox bgEntitiesKeep;
        private System.Windows.Forms.GroupBox gbEntitiesExclude;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.ListView lvKeptEntities;
        private System.Windows.Forms.ColumnHeader chDisplayName;
        private System.Windows.Forms.ColumnHeader chLogicalName;
        private System.Windows.Forms.ListView lvExcludedEntities;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}