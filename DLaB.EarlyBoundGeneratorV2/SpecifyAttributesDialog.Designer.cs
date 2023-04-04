namespace DLaB.EarlyBoundGeneratorV2.Forms
{
    partial class SpecifyAttributesDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpecifyAttributesDialog));
            this.BtnSave = new System.Windows.Forms.Button();
            this.BtnAdd = new System.Windows.Forms.Button();
            this.EntitiesEdd = new xrmtb.XrmToolBox.Controls.EntitiesDropdownControl();
            this.AttributesAlc = new xrmtb.XrmToolBox.Controls.AttributeListControl();
            this.SuspendLayout();
            // 
            // BtnSave
            // 
            this.BtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnSave.Location = new System.Drawing.Point(871, 737);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(75, 23);
            this.BtnSave.TabIndex = 0;
            this.BtnSave.Text = "Save";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // BtnAdd
            // 
            this.BtnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnAdd.Location = new System.Drawing.Point(403, 737);
            this.BtnAdd.Name = "BtnAdd";
            this.BtnAdd.Size = new System.Drawing.Size(75, 23);
            this.BtnAdd.TabIndex = 2;
            this.BtnAdd.Text = "Add";
            this.BtnAdd.UseVisualStyleBackColor = true;
            this.BtnAdd.Click += new System.EventHandler(this.BtnAdd_Click);
            // 
            // EntitiesEdd
            // 
            this.EntitiesEdd.AutoLoadData = false;
            this.EntitiesEdd.LanguageCode = 1033;
            this.EntitiesEdd.Location = new System.Drawing.Point(123, 53);
            this.EntitiesEdd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.EntitiesEdd.Name = "EntitiesEdd";
            this.EntitiesEdd.Service = null;
            this.EntitiesEdd.Size = new System.Drawing.Size(374, 25);
            this.EntitiesEdd.SolutionFilter = null;
            this.EntitiesEdd.TabIndex = 3;
            // 
            // AttributesAlc
            // 
            this.AttributesAlc.AutoLoadData = false;
            this.AttributesAlc.AutosizeColumns = System.Windows.Forms.ColumnHeaderAutoResizeStyle.None;
            this.AttributesAlc.Checkboxes = true;
            this.AttributesAlc.DisplayToolbar = true;
            this.AttributesAlc.LanguageCode = 1033;
            this.AttributesAlc.ListViewColDefs = new xrmtb.XrmToolBox.Controls.ListViewColumnDef[] {
        ((xrmtb.XrmToolBox.Controls.ListViewColumnDef)(resources.GetObject("AttributesAlc.ListViewColDefs"))),
        ((xrmtb.XrmToolBox.Controls.ListViewColumnDef)(resources.GetObject("AttributesAlc.ListViewColDefs1"))),
        ((xrmtb.XrmToolBox.Controls.ListViewColumnDef)(resources.GetObject("AttributesAlc.ListViewColDefs2"))),
        ((xrmtb.XrmToolBox.Controls.ListViewColumnDef)(resources.GetObject("AttributesAlc.ListViewColDefs3"))),
        ((xrmtb.XrmToolBox.Controls.ListViewColumnDef)(resources.GetObject("AttributesAlc.ListViewColDefs4"))),
        ((xrmtb.XrmToolBox.Controls.ListViewColumnDef)(resources.GetObject("AttributesAlc.ListViewColDefs5")))};
            this.AttributesAlc.Location = new System.Drawing.Point(123, 153);
            this.AttributesAlc.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.AttributesAlc.Name = "AttributesAlc";
            this.AttributesAlc.ParentEntity = null;
            this.AttributesAlc.ParentEntityLogicalName = null;
            this.AttributesAlc.Service = null;
            this.AttributesAlc.Size = new System.Drawing.Size(700, 474);
            this.AttributesAlc.TabIndex = 4;
            // 
            // SpecifyAttributesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(958, 772);
            this.Controls.Add(this.AttributesAlc);
            this.Controls.Add(this.EntitiesEdd);
            this.Controls.Add(this.BtnAdd);
            this.Controls.Add(this.BtnSave);
            this.MinimumSize = new System.Drawing.Size(600, 39);
            this.Name = "SpecifyAttributesDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Specify Attributes";
            this.Load += new System.EventHandler(this.SpecifyAttributesDialog_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.Button BtnAdd;
        private xrmtb.XrmToolBox.Controls.EntitiesDropdownControl EntitiesEdd;
        private xrmtb.XrmToolBox.Controls.AttributeListControl AttributesAlc;
    }
}