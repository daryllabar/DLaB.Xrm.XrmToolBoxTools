using DLaB.XrmToolBoxCommon.Controls;

namespace DLaB.XrmToolBoxCommon.Forms
{
    partial class SpecifyEntityNameDialog
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
            this.BtnRefresh = new System.Windows.Forms.Button();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.lvEntities = new DLaB.XrmToolBoxCommon.Controls.DLaBListView();
            this.chDisplayName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chLogicalName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chClassName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TxtEdit = new System.Windows.Forms.TextBox();
            this.pnlFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnSave
            // 
            this.BtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnSave.Location = new System.Drawing.Point(818, 23);
            this.BtnSave.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(112, 35);
            this.BtnSave.TabIndex = 0;
            this.BtnSave.Text = "Save";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // BtnRefresh
            // 
            this.BtnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnRefresh.Location = new System.Drawing.Point(940, 23);
            this.BtnRefresh.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.BtnRefresh.Name = "BtnRefresh";
            this.BtnRefresh.Size = new System.Drawing.Size(112, 35);
            this.BtnRefresh.TabIndex = 8;
            this.BtnRefresh.Text = "Refresh";
            this.BtnRefresh.UseVisualStyleBackColor = true;
            this.BtnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // pnlFooter
            // 
            this.pnlFooter.Controls.Add(this.BtnRefresh);
            this.pnlFooter.Controls.Add(this.BtnSave);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 577);
            this.pnlFooter.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(1056, 77);
            this.pnlFooter.TabIndex = 10;
            // 
            // lvEntities
            // 
            this.lvEntities.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvEntities.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chDisplayName,
            this.chLogicalName,
            this.chClassName});
            this.lvEntities.FullRowSelect = true;
            this.lvEntities.HideSelection = false;
            this.lvEntities.Location = new System.Drawing.Point(4, 14);
            this.lvEntities.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lvEntities.Name = "lvEntities";
            this.lvEntities.Size = new System.Drawing.Size(1048, 563);
            this.lvEntities.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvEntities.TabIndex = 11;
            this.lvEntities.UseCompatibleStateImageBehavior = false;
            this.lvEntities.View = System.Windows.Forms.View.Details;
            this.lvEntities.Scroll += new System.Windows.Forms.ScrollEventHandler(this.lvEntities_Scroll);
            this.lvEntities.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvEntities_ColumnClick);
            this.lvEntities.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lvEntities_MouseDown);
            this.lvEntities.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LvEntities_MouseUp);
            // 
            // chDisplayName
            // 
            this.chDisplayName.Text = "Display Name";
            this.chDisplayName.Width = 250;
            // 
            // chLogicalName
            // 
            this.chLogicalName.Text = "Logical Name";
            this.chLogicalName.Width = 250;
            // 
            // chClassName
            // 
            this.chClassName.Text = "Class Name";
            this.chClassName.Width = 250;
            // 
            // TxtEdit
            // 
            this.TxtEdit.Location = new System.Drawing.Point(505, 39);
            this.TxtEdit.Name = "TxtEdit";
            this.TxtEdit.Size = new System.Drawing.Size(251, 26);
            this.TxtEdit.TabIndex = 12;
            this.TxtEdit.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TxtEdit_KeyUp);
            this.TxtEdit.Leave += new System.EventHandler(this.TxtEdit_Leave);
            // 
            // SpecifyEntityNameDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1056, 654);
            this.Controls.Add(this.TxtEdit);
            this.Controls.Add(this.lvEntities);
            this.Controls.Add(this.pnlFooter);
            this.Margin = new System.Windows.Forms.Padding(12, 11, 12, 11);
            this.MinimumSize = new System.Drawing.Size(804, 633);
            this.Name = "SpecifyEntityNameDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Entities";
            this.Load += new System.EventHandler(this.SpecifyEntityNameDialog_Load);
            this.Resize += new System.EventHandler(this.SpecifyEntityNameDialog_Resize);
            this.pnlFooter.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.Button BtnRefresh;
        private System.Windows.Forms.Panel pnlFooter;
        private DLaBListView lvEntities;
        private System.Windows.Forms.ColumnHeader chDisplayName;
        private System.Windows.Forms.ColumnHeader chLogicalName;
        private System.Windows.Forms.ColumnHeader chClassName;
        private System.Windows.Forms.TextBox TxtEdit;
    }
}