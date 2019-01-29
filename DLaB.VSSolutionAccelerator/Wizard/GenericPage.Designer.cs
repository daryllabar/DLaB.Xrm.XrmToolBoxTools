namespace DLaB.VSSolutionAccelerator.Wizard
{
    partial class GenericPage
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
            this.QuestionLabel = new System.Windows.Forms.Label();
            this.Table = new System.Windows.Forms.TableLayoutPanel();
            this.YesNoGroup = new System.Windows.Forms.GroupBox();
            this.NoRadio = new System.Windows.Forms.RadioButton();
            this.YesRadio = new System.Windows.Forms.RadioButton();
            this.ResponseText = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.OpenFileBtn = new System.Windows.Forms.Button();
            this.Path = new System.Windows.Forms.TextBox();
            this.DescriptionText = new System.Windows.Forms.TextBox();
            this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.Table.SuspendLayout();
            this.YesNoGroup.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // QuestionLabel
            // 
            this.QuestionLabel.AutoSize = true;
            this.QuestionLabel.Location = new System.Drawing.Point(53, 120);
            this.QuestionLabel.Name = "QuestionLabel";
            this.QuestionLabel.Size = new System.Drawing.Size(55, 13);
            this.QuestionLabel.TabIndex = 0;
            this.QuestionLabel.Text = "Question?";
            // 
            // Table
            // 
            this.Table.ColumnCount = 3;
            this.Table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.Table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.Table.Controls.Add(this.YesNoGroup, 1, 1);
            this.Table.Controls.Add(this.QuestionLabel, 1, 2);
            this.Table.Controls.Add(this.ResponseText, 1, 3);
            this.Table.Controls.Add(this.panel1, 1, 4);
            this.Table.Controls.Add(this.DescriptionText, 1, 5);
            this.Table.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Table.Location = new System.Drawing.Point(0, 0);
            this.Table.Name = "Table";
            this.Table.RowCount = 6;
            this.Table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.Table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.Table.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.Table.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.Table.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.Table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Table.Size = new System.Drawing.Size(594, 412);
            this.Table.TabIndex = 1;
            // 
            // YesNoGroup
            // 
            this.YesNoGroup.Controls.Add(this.NoRadio);
            this.YesNoGroup.Controls.Add(this.YesRadio);
            this.YesNoGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.YesNoGroup.Location = new System.Drawing.Point(53, 23);
            this.YesNoGroup.Name = "YesNoGroup";
            this.YesNoGroup.Size = new System.Drawing.Size(488, 94);
            this.YesNoGroup.TabIndex = 1;
            this.YesNoGroup.TabStop = false;
            this.YesNoGroup.Text = "Yes No Question?";
            // 
            // NoRadio
            // 
            this.NoRadio.AutoSize = true;
            this.NoRadio.Location = new System.Drawing.Point(194, 50);
            this.NoRadio.Name = "NoRadio";
            this.NoRadio.Size = new System.Drawing.Size(39, 17);
            this.NoRadio.TabIndex = 1;
            this.NoRadio.TabStop = true;
            this.NoRadio.Text = "No";
            this.NoRadio.UseVisualStyleBackColor = true;
            this.NoRadio.CheckedChanged += new System.EventHandler(this.YesNoRadio_CheckedChanged);
            // 
            // YesRadio
            // 
            this.YesRadio.AutoSize = true;
            this.YesRadio.Location = new System.Drawing.Point(145, 50);
            this.YesRadio.Name = "YesRadio";
            this.YesRadio.Size = new System.Drawing.Size(43, 17);
            this.YesRadio.TabIndex = 0;
            this.YesRadio.TabStop = true;
            this.YesRadio.Text = "Yes";
            this.YesRadio.UseVisualStyleBackColor = true;
            this.YesRadio.CheckedChanged += new System.EventHandler(this.YesNoRadio_CheckedChanged);
            // 
            // ResponseText
            // 
            this.ResponseText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ResponseText.Location = new System.Drawing.Point(53, 136);
            this.ResponseText.Name = "ResponseText";
            this.ResponseText.Size = new System.Drawing.Size(488, 20);
            this.ResponseText.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.OpenFileBtn);
            this.panel1.Controls.Add(this.Path);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(50, 159);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(494, 30);
            this.panel1.TabIndex = 3;
            // 
            // OpenFileBtn
            // 
            this.OpenFileBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OpenFileBtn.Location = new System.Drawing.Point(465, 4);
            this.OpenFileBtn.Name = "OpenFileBtn";
            this.OpenFileBtn.Size = new System.Drawing.Size(26, 23);
            this.OpenFileBtn.TabIndex = 1;
            this.OpenFileBtn.Text = "...";
            this.OpenFileBtn.UseVisualStyleBackColor = true;
            this.OpenFileBtn.Click += new System.EventHandler(this.OpenFileBtn_Click);
            // 
            // Path
            // 
            this.Path.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Path.Location = new System.Drawing.Point(4, 4);
            this.Path.Name = "Path";
            this.Path.Size = new System.Drawing.Size(455, 20);
            this.Path.TabIndex = 0;
            // 
            // DescriptionText
            // 
            this.DescriptionText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DescriptionText.Location = new System.Drawing.Point(53, 192);
            this.DescriptionText.Multiline = true;
            this.DescriptionText.Name = "DescriptionText";
            this.DescriptionText.ReadOnly = true;
            this.DescriptionText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.DescriptionText.Size = new System.Drawing.Size(488, 217);
            this.DescriptionText.TabIndex = 4;
            // 
            // GenericPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Table);
            this.Name = "GenericPage";
            this.Size = new System.Drawing.Size(594, 412);
            this.Table.ResumeLayout(false);
            this.Table.PerformLayout();
            this.YesNoGroup.ResumeLayout(false);
            this.YesNoGroup.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label QuestionLabel;
        private System.Windows.Forms.TableLayoutPanel Table;
        private System.Windows.Forms.GroupBox YesNoGroup;
        private System.Windows.Forms.RadioButton NoRadio;
        private System.Windows.Forms.RadioButton YesRadio;
        private System.Windows.Forms.TextBox ResponseText;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button OpenFileBtn;
        private System.Windows.Forms.TextBox Path;
        private System.Windows.Forms.TextBox DescriptionText;
        private System.Windows.Forms.OpenFileDialog OpenFileDialog;
    }
}
