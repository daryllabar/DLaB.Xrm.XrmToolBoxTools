namespace DLaB.EarlyBoundGenerator
{
    partial class AttributeToEnumMapperDialog
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.RdoGlobal = new System.Windows.Forms.RadioButton();
            this.RdoLocal = new System.Windows.Forms.RadioButton();
            this.CmbOptionSets = new System.Windows.Forms.ComboBox();
            this.LblOptionSet = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.CmbAttributeAttributes = new System.Windows.Forms.ComboBox();
            this.CmbAttributeEntities = new System.Windows.Forms.ComboBox();
            this.GrpAttribute = new System.Windows.Forms.GroupBox();
            this.GrpSelectOptionSet = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.GrpAttribute.SuspendLayout();
            this.GrpSelectOptionSet.SuspendLayout();
            this.SuspendLayout();
            // 
            // CmbEntities
            // 
            this.CmbEntities.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CmbEntities.DisplayMember = "DisplayName";
            this.CmbEntities.FormattingEnabled = true;
            this.CmbEntities.Location = new System.Drawing.Point(222, 24);
            this.CmbEntities.Name = "CmbEntities";
            this.CmbEntities.Size = new System.Drawing.Size(233, 21);
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
            this.CmbAttributes.Location = new System.Drawing.Point(222, 51);
            this.CmbAttributes.Name = "CmbAttributes";
            this.CmbAttributes.Size = new System.Drawing.Size(233, 21);
            this.CmbAttributes.TabIndex = 1;
            this.CmbAttributes.ValueMember = "LogicalName";
            this.CmbAttributes.SelectedIndexChanged += new System.EventHandler(this.CmbAttributes_SelectedIndexChanged);
            // 
            // LblEntity
            // 
            this.LblEntity.AutoSize = true;
            this.LblEntity.Location = new System.Drawing.Point(172, 27);
            this.LblEntity.Name = "LblEntity";
            this.LblEntity.Size = new System.Drawing.Size(44, 13);
            this.LblEntity.TabIndex = 2;
            this.LblEntity.Text = "Entities:";
            // 
            // LblAttribute
            // 
            this.LblAttribute.AutoSize = true;
            this.LblAttribute.Location = new System.Drawing.Point(162, 54);
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
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 248);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(461, 27);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.BtnAdd);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(196, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(69, 21);
            this.panel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.RdoGlobal);
            this.groupBox1.Controls.Add(this.RdoLocal);
            this.groupBox1.Location = new System.Drawing.Point(6, 24);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(139, 75);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Type";
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
            this.CmbOptionSets.Location = new System.Drawing.Point(222, 78);
            this.CmbOptionSets.Name = "CmbOptionSets";
            this.CmbOptionSets.Size = new System.Drawing.Size(233, 21);
            this.CmbOptionSets.TabIndex = 9;
            this.CmbOptionSets.ValueMember = "LogicalName";
            this.CmbOptionSets.SelectedIndexChanged += new System.EventHandler(this.CmbOptionSets_SelectedIndexChanged);
            // 
            // LblOptionSet
            // 
            this.LblOptionSet.AutoSize = true;
            this.LblOptionSet.Location = new System.Drawing.Point(151, 81);
            this.LblOptionSet.Name = "LblOptionSet";
            this.LblOptionSet.Size = new System.Drawing.Size(65, 13);
            this.LblOptionSet.TabIndex = 10;
            this.LblOptionSet.Text = "Option Sets:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Attribute:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Entity:";
            // 
            // CmbAttributeAttributes
            // 
            this.CmbAttributeAttributes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CmbAttributeAttributes.DisplayMember = "DisplayName";
            this.CmbAttributeAttributes.FormattingEnabled = true;
            this.CmbAttributeAttributes.Location = new System.Drawing.Point(79, 58);
            this.CmbAttributeAttributes.Name = "CmbAttributeAttributes";
            this.CmbAttributeAttributes.Size = new System.Drawing.Size(377, 21);
            this.CmbAttributeAttributes.TabIndex = 12;
            this.CmbAttributeAttributes.ValueMember = "LogicalName";
            this.CmbAttributeAttributes.SelectedIndexChanged += new System.EventHandler(this.CmbAttributeAttributes_SelectedIndexChanged);
            // 
            // CmbAttributeEntities
            // 
            this.CmbAttributeEntities.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CmbAttributeEntities.DisplayMember = "DisplayName";
            this.CmbAttributeEntities.FormattingEnabled = true;
            this.CmbAttributeEntities.Location = new System.Drawing.Point(79, 31);
            this.CmbAttributeEntities.Name = "CmbAttributeEntities";
            this.CmbAttributeEntities.Size = new System.Drawing.Size(377, 21);
            this.CmbAttributeEntities.TabIndex = 11;
            this.CmbAttributeEntities.ValueMember = "LogicalName";
            this.CmbAttributeEntities.SelectedIndexChanged += new System.EventHandler(this.CmbAttributeEntities_SelectedIndexChanged);
            // 
            // GrpAttribute
            // 
            this.GrpAttribute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GrpAttribute.Controls.Add(this.CmbAttributeEntities);
            this.GrpAttribute.Controls.Add(this.label2);
            this.GrpAttribute.Controls.Add(this.CmbAttributeAttributes);
            this.GrpAttribute.Controls.Add(this.label1);
            this.GrpAttribute.Location = new System.Drawing.Point(12, 12);
            this.GrpAttribute.Name = "GrpAttribute";
            this.GrpAttribute.Size = new System.Drawing.Size(461, 105);
            this.GrpAttribute.TabIndex = 15;
            this.GrpAttribute.TabStop = false;
            this.GrpAttribute.Text = "Select an Attribute to Specifiy the Option Set Used";
            // 
            // GrpSelectOptionSet
            // 
            this.GrpSelectOptionSet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GrpSelectOptionSet.Controls.Add(this.CmbEntities);
            this.GrpSelectOptionSet.Controls.Add(this.CmbAttributes);
            this.GrpSelectOptionSet.Controls.Add(this.LblOptionSet);
            this.GrpSelectOptionSet.Controls.Add(this.LblEntity);
            this.GrpSelectOptionSet.Controls.Add(this.CmbOptionSets);
            this.GrpSelectOptionSet.Controls.Add(this.LblAttribute);
            this.GrpSelectOptionSet.Controls.Add(this.groupBox1);
            this.GrpSelectOptionSet.Location = new System.Drawing.Point(12, 124);
            this.GrpSelectOptionSet.Name = "GrpSelectOptionSet";
            this.GrpSelectOptionSet.Size = new System.Drawing.Size(461, 114);
            this.GrpSelectOptionSet.TabIndex = 16;
            this.GrpSelectOptionSet.TabStop = false;
            this.GrpSelectOptionSet.Text = "Select the Option Set";
            // 
            // AttributeToEnumMapperDialog
            // 
            this.AcceptButton = this.BtnAdd;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 286);
            this.Controls.Add(this.GrpSelectOptionSet);
            this.Controls.Add(this.GrpAttribute);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(500, 325);
            this.Name = "AttributeToEnumMapperDialog";
            this.Text = "Map Attribute to Option Set";
            this.Load += new System.EventHandler(this.LocalAttributeToEnumMapperDialog_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.GrpAttribute.ResumeLayout(false);
            this.GrpAttribute.PerformLayout();
            this.GrpSelectOptionSet.ResumeLayout(false);
            this.GrpSelectOptionSet.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton RdoGlobal;
        private System.Windows.Forms.RadioButton RdoLocal;
        private System.Windows.Forms.ComboBox CmbOptionSets;
        private System.Windows.Forms.Label LblOptionSet;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox CmbAttributeAttributes;
        private System.Windows.Forms.ComboBox CmbAttributeEntities;
        private System.Windows.Forms.GroupBox GrpAttribute;
        private System.Windows.Forms.GroupBox GrpSelectOptionSet;
    }
}