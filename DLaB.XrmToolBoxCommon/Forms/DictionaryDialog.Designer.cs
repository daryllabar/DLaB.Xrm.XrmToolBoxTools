
namespace DLaB.XrmToolBoxCommon.Forms
{
    partial class DictionaryDialog
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
            this.MainTxt = new System.Windows.Forms.TextBox();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.MainLbl = new System.Windows.Forms.Label();
            this.OkBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // MainTxt
            // 
            this.MainTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainTxt.Location = new System.Drawing.Point(12, 25);
            this.MainTxt.Multiline = true;
            this.MainTxt.Name = "MainTxt";
            this.MainTxt.Size = new System.Drawing.Size(286, 384);
            this.MainTxt.TabIndex = 0;
            // 
            // CancelBtn
            // 
            this.CancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelBtn.Location = new System.Drawing.Point(223, 415);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 23);
            this.CancelBtn.TabIndex = 1;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // MainLbl
            // 
            this.MainLbl.AutoSize = true;
            this.MainLbl.Location = new System.Drawing.Point(12, 9);
            this.MainLbl.Name = "MainLbl";
            this.MainLbl.Size = new System.Drawing.Size(267, 13);
            this.MainLbl.TabIndex = 2;
            this.MainLbl.Text = "Enter the comma seperated mapping (one pair per line):";
            // 
            // OkBtn
            // 
            this.OkBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OkBtn.Location = new System.Drawing.Point(142, 415);
            this.OkBtn.Name = "OkBtn";
            this.OkBtn.Size = new System.Drawing.Size(75, 23);
            this.OkBtn.TabIndex = 3;
            this.OkBtn.Text = "OK";
            this.OkBtn.UseVisualStyleBackColor = true;
            this.OkBtn.Click += new System.EventHandler(this.OkBtn_Click);
            // 
            // DictionaryDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(310, 450);
            this.Controls.Add(this.OkBtn);
            this.Controls.Add(this.MainLbl);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.MainTxt);
            this.Name = "DictionaryDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Dictionary Editor";
            this.Load += new System.EventHandler(this.DictionaryDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox MainTxt;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Label MainLbl;
        private System.Windows.Forms.Button OkBtn;
    }
}