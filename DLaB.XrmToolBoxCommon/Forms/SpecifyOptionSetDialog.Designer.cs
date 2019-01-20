namespace DLaB.XrmToolBoxCommon.Forms
{
    partial class SpecifyOptionSetDialog
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
            this.CmbEntities = new System.Windows.Forms.ComboBox();
            this.CmbAttributes = new System.Windows.Forms.ComboBox();
            this.LblEntity = new System.Windows.Forms.Label();
            this.LblAttribute = new System.Windows.Forms.Label();
            this.BtnAdd = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.GrpType = new System.Windows.Forms.GroupBox();
            this.RdoGlobal = new System.Windows.Forms.RadioButton();
            this.RdoLocal = new System.Windows.Forms.RadioButton();
            this.CmbOptionSets = new System.Windows.Forms.ComboBox();
            this.LblOptionSet = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.GrpType.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // CmbEntities
            // 
            this.CmbEntities.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CmbEntities.DisplayMember = "DisplayName";
            this.CmbEntities.FormattingEnabled = true;
            this.CmbEntities.Location = new System.Drawing.Point(63, 1);
            this.CmbEntities.Name = "CmbEntities";
            this.CmbEntities.Size = new System.Drawing.Size(204, 21);
            this.CmbEntities.TabIndex = 0;
            this.CmbEntities.ValueMember = "LogicalName";
            this.CmbEntities.SelectedIndexChanged += new System.EventHandler(this.CmbEntities_SelectedIndexChanged);
            // 
            // CmbAttributes
            // 
            this.CmbAttributes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CmbAttributes.DisplayMember = "DisplayName";
            this.CmbAttributes.FormattingEnabled = true;
            this.CmbAttributes.Location = new System.Drawing.Point(63, 28);
            this.CmbAttributes.Name = "CmbAttributes";
            this.CmbAttributes.Size = new System.Drawing.Size(204, 21);
            this.CmbAttributes.TabIndex = 1;
            this.CmbAttributes.ValueMember = "LogicalName";
            this.CmbAttributes.SelectedIndexChanged += new System.EventHandler(this.CmbAttributes_SelectedIndexChanged);
            // 
            // LblEntity
            // 
            this.LblEntity.AutoSize = true;
            this.LblEntity.Location = new System.Drawing.Point(13, 4);
            this.LblEntity.Name = "LblEntity";
            this.LblEntity.Size = new System.Drawing.Size(44, 13);
            this.LblEntity.TabIndex = 2;
            this.LblEntity.Text = "Entities:";
            // 
            // LblAttribute
            // 
            this.LblAttribute.AutoSize = true;
            this.LblAttribute.Location = new System.Drawing.Point(3, 31);
            this.LblAttribute.Name = "LblAttribute";
            this.LblAttribute.Size = new System.Drawing.Size(54, 13);
            this.LblAttribute.TabIndex = 3;
            this.LblAttribute.Text = "Attributes:";
            // 
            // BtnAdd
            // 
            this.BtnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnAdd.Location = new System.Drawing.Point(0, 0);
            this.BtnAdd.Name = "BtnAdd";
            this.BtnAdd.Size = new System.Drawing.Size(69, 21);
            this.BtnAdd.TabIndex = 6;
            this.BtnAdd.Text = "Add";
            this.BtnAdd.UseVisualStyleBackColor = true;
            this.BtnAdd.Click += new System.EventHandler(this.BtnAdd_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 122);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(420, 27);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.BtnAdd);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(175, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(69, 21);
            this.panel1.TabIndex = 0;
            // 
            // GrpType
            // 
            this.GrpType.Controls.Add(this.RdoGlobal);
            this.GrpType.Controls.Add(this.RdoLocal);
            this.GrpType.Location = new System.Drawing.Point(3, 3);
            this.GrpType.Name = "GrpType";
            this.GrpType.Size = new System.Drawing.Size(139, 75);
            this.GrpType.TabIndex = 8;
            this.GrpType.TabStop = false;
            this.GrpType.Text = "Type";
            // 
            // RdoGlobal
            // 
            this.RdoGlobal.AutoSize = true;
            this.RdoGlobal.Location = new System.Drawing.Point(7, 44);
            this.RdoGlobal.Name = "RdoGlobal";
            this.RdoGlobal.Size = new System.Drawing.Size(55, 17);
            this.RdoGlobal.TabIndex = 1;
            this.RdoGlobal.TabStop = true;
            this.RdoGlobal.Text = "Global";
            this.RdoGlobal.UseVisualStyleBackColor = true;
            this.RdoGlobal.CheckedChanged += new System.EventHandler(this.RdoGlobal_CheckedChanged);
            // 
            // RdoLocal
            // 
            this.RdoLocal.AutoSize = true;
            this.RdoLocal.Location = new System.Drawing.Point(7, 20);
            this.RdoLocal.Name = "RdoLocal";
            this.RdoLocal.Size = new System.Drawing.Size(127, 17);
            this.RdoLocal.TabIndex = 0;
            this.RdoLocal.TabStop = true;
            this.RdoLocal.Text = "Entity Specific (Local)";
            this.RdoLocal.UseVisualStyleBackColor = true;
            this.RdoLocal.CheckedChanged += new System.EventHandler(this.RdoLocal_CheckedChanged);
            // 
            // CmbOptionSets
            // 
            this.CmbOptionSets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CmbOptionSets.DisplayMember = "DisplayName";
            this.CmbOptionSets.FormattingEnabled = true;
            this.CmbOptionSets.Location = new System.Drawing.Point(74, 55);
            this.CmbOptionSets.Name = "CmbOptionSets";
            this.CmbOptionSets.Size = new System.Drawing.Size(193, 21);
            this.CmbOptionSets.TabIndex = 9;
            this.CmbOptionSets.ValueMember = "LogicalName";
            this.CmbOptionSets.SelectedIndexChanged += new System.EventHandler(this.CmbOptionSets_SelectedIndexChanged);
            // 
            // LblOptionSet
            // 
            this.LblOptionSet.AutoSize = true;
            this.LblOptionSet.Location = new System.Drawing.Point(3, 58);
            this.LblOptionSet.Name = "LblOptionSet";
            this.LblOptionSet.Size = new System.Drawing.Size(65, 13);
            this.LblOptionSet.TabIndex = 10;
            this.LblOptionSet.Text = "Option Sets:";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.panel3, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(420, 87);
            this.tableLayoutPanel2.TabIndex = 11;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.GrpType);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(150, 87);
            this.panel2.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.LblEntity);
            this.panel3.Controls.Add(this.LblOptionSet);
            this.panel3.Controls.Add(this.CmbEntities);
            this.panel3.Controls.Add(this.CmbOptionSets);
            this.panel3.Controls.Add(this.CmbAttributes);
            this.panel3.Controls.Add(this.LblAttribute);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(150, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(270, 87);
            this.panel3.TabIndex = 1;
            // 
            // OptionSetSpecifierDialog
            // 
            this.AcceptButton = this.BtnAdd;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 161);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximumSize = new System.Drawing.Size(100000, 200);
            this.MinimumSize = new System.Drawing.Size(400, 200);
            this.Name = "OptionSetSpecifierDialog";
            this.Text = "Specify Option Set";
            this.Load += new System.EventHandler(this.LocalOptionSetSpecifierDialog_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.GrpType.ResumeLayout(false);
            this.GrpType.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox CmbEntities;
        private System.Windows.Forms.ComboBox CmbAttributes;
        private System.Windows.Forms.Label LblEntity;
        private System.Windows.Forms.Label LblAttribute;
        private System.Windows.Forms.Button BtnAdd;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox GrpType;
        private System.Windows.Forms.RadioButton RdoGlobal;
        private System.Windows.Forms.RadioButton RdoLocal;
        private System.Windows.Forms.ComboBox CmbOptionSets;
        private System.Windows.Forms.Label LblOptionSet;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
    }
}