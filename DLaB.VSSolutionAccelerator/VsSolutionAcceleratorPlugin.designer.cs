namespace DLaB.VSSolutionAccelerator
{
    partial class VsSolutionAcceleratorPlugin
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VsSolutionAcceleratorPlugin));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ActionCmb = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ExecuteBttn = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.TxtOutput = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 71.42857F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 154F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1200, 923);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ActionCmb);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.ExecuteBttn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(171, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(857, 154);
            this.panel1.TabIndex = 0;
            // 
            // ActionCmb
            // 
            this.ActionCmb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ActionCmb.FormattingEnabled = true;
            this.ActionCmb.Items.AddRange(new object[] {
            "Add Accelerator Isolation Libraries to my Solution",
            "Add Plugin/Workflow Project to an Accelerated Solution",
            "Install Snippets"});
            this.ActionCmb.Location = new System.Drawing.Point(104, 55);
            this.ActionCmb.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ActionCmb.Name = "ActionCmb";
            this.ActionCmb.Size = new System.Drawing.Size(607, 28);
            this.ActionCmb.TabIndex = 2;
            this.ActionCmb.SelectedIndexChanged += new System.EventHandler(this.ActionCmb_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 60);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "I Want To:";
            // 
            // ExecuteBttn
            // 
            this.ExecuteBttn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ExecuteBttn.Location = new System.Drawing.Point(721, 52);
            this.ExecuteBttn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ExecuteBttn.Name = "ExecuteBttn";
            this.ExecuteBttn.Size = new System.Drawing.Size(135, 35);
            this.ExecuteBttn.TabIndex = 0;
            this.ExecuteBttn.Text = "Execute";
            this.ExecuteBttn.UseVisualStyleBackColor = true;
            this.ExecuteBttn.Click += new System.EventHandler(this.ExecuteBttn_Click);
            // 
            // panel2
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.panel2, 3);
            this.panel2.Controls.Add(this.TxtOutput);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(4, 159);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1192, 759);
            this.panel2.TabIndex = 1;
            // 
            // TxtOutput
            // 
            this.TxtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtOutput.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtOutput.Location = new System.Drawing.Point(28, 6);
            this.TxtOutput.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.TxtOutput.Multiline = true;
            this.TxtOutput.Name = "TxtOutput";
            this.TxtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TxtOutput.Size = new System.Drawing.Size(1136, 751);
            this.TxtOutput.TabIndex = 0;
            this.TxtOutput.Text = resources.GetString("TxtOutput.Text");
            // 
            // VsSolutionAcceleratorPlugin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "VsSolutionAcceleratorPlugin";
            this.Size = new System.Drawing.Size(1200, 923);
            this.OnCloseTool += new System.EventHandler(this.VsSolutionAcceleratorPlugin_OnCloseTool);
            this.Load += new System.EventHandler(this.MyPluginControl_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox ActionCmb;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ExecuteBttn;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox TxtOutput;
    }
}
