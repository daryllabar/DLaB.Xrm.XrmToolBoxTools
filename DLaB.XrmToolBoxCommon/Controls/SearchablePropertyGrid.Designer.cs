using System.Windows.Forms;

namespace DLaB.XrmToolBoxCommon.Controls
{
    partial class SearchablePropertyGrid
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
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.FilterableGrid = new DLaB.XrmToolBoxCommon.Controls.FilteredPropertyGrid();
            this.SuspendLayout();
            // 
            // txtSearch
            // 
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtSearch.Location = new System.Drawing.Point(0, 0);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(334, 26);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.TextChanged += new System.EventHandler(this.TxtSearch_TextChanged);
            this.txtSearch.GotFocus += new System.EventHandler(this.TxtSearch_GotFocus);
            this.txtSearch.LostFocus += new System.EventHandler(this.TxtSearch_LostFocus);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Enabled = false;
            this.splitter1.Location = new System.Drawing.Point(0, 26);
            this.splitter1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(334, 4);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // FilterableGrid
            // 
            this.FilterableGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FilterableGrid.FilterProperties = null;
            this.FilterableGrid.Location = new System.Drawing.Point(0, 30);
            this.FilterableGrid.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.FilterableGrid.Name = "FilterableGrid";
            this.FilterableGrid.Size = new System.Drawing.Size(334, 461);
            this.FilterableGrid.TabIndex = 2;
            // 
            // SearchablePropertyGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.FilterableGrid);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.txtSearch);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "SearchablePropertyGrid";
            this.Size = new System.Drawing.Size(334, 491);
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Splitter splitter1;
        public FilteredPropertyGrid FilterableGrid;
    }
}
