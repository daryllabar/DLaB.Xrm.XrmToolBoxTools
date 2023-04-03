namespace DLaB.EarlyBoundGenerator
{
    partial class SpecifyStringValuesDialog
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
            this.LstValues = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // BtnSave
            // 
            this.BtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnSave.Location = new System.Drawing.Point(447, 378);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(75, 23);
            this.BtnSave.TabIndex = 0;
            this.BtnSave.Text = "Save";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // LstValues
            // 
            this.LstValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LstValues.DisplayMember = "DisplayName";
            this.LstValues.FormattingEnabled = true;
            this.LstValues.HorizontalScrollbar = true;
            this.LstValues.Location = new System.Drawing.Point(12, 12);
            this.LstValues.Name = "LstValues";
            this.LstValues.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.LstValues.Size = new System.Drawing.Size(510, 355);
            this.LstValues.Sorted = true;
            this.LstValues.TabIndex = 1;
            this.LstValues.ValueMember = "Value";
            // 
            // SpecifyStringValuesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 411);
            this.Controls.Add(this.LstValues);
            this.Controls.Add(this.BtnSave);
            this.MinimumSize = new System.Drawing.Size(550, 450);
            this.Name = "SpecifyStringValuesDialog";
            this.Text = "Specify";
            this.Load += new System.EventHandler(this.SpecifyStringValuesDialog_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.ListBox LstValues;
    }
}