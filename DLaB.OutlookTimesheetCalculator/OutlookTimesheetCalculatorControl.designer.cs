using System.ComponentModel;
using System.Windows.Forms;

namespace DLaB.OutlookTimesheetCalculator
{
    partial class OutlookTimesheetCalculatorControl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.components = new System.ComponentModel.Container();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tabProjects = new System.Windows.Forms.TabPage();
            this.dgvProjects = new System.Windows.Forms.DataGridView();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.isBillableDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.bsProject = new System.Windows.Forms.BindingSource(this.components);
            this.tabTasks = new System.Windows.Forms.TabPage();
            this.cmbProjects = new System.Windows.Forms.ComboBox();
            this.bsDefaultProject = new System.Windows.Forms.BindingSource(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.dgvTasks = new System.Windows.Forms.DataGridView();
            this.nameDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.isBillableDataGridViewCheckBoxColumn1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.projectDataGridViewComboBoxColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.bsTask = new System.Windows.Forms.BindingSource(this.components);
            this.tabTime = new System.Windows.Forms.TabPage();
            this.calCLB = new System.Windows.Forms.CheckedListBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.TaskTotalsPanel = new System.Windows.Forms.Panel();
            this.txtTaskDailyHours = new System.Windows.Forms.TextBox();
            this.ProjectsPanel = new System.Windows.Forms.Panel();
            this.lstProjects = new System.Windows.Forms.ListBox();
            this.ProjectDailyHoursPanel = new System.Windows.Forms.Panel();
            this.txtTasks = new System.Windows.Forms.TextBox();
            this.txtDailyHours = new System.Windows.Forms.TextBox();
            this.lblHours = new System.Windows.Forms.Label();
            this.btnCalc = new System.Windows.Forms.Button();
            this.dtpEnd = new System.Windows.Forms.DateTimePicker();
            this.dtpStart = new System.Windows.Forms.DateTimePicker();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.label8 = new System.Windows.Forms.Label();
            this.tabProjects.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProjects)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsProject)).BeginInit();
            this.tabTasks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsDefaultProject)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTasks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsTask)).BeginInit();
            this.tabTime.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.TaskTotalsPanel.SuspendLayout();
            this.ProjectsPanel.SuspendLayout();
            this.ProjectDailyHoursPanel.SuspendLayout();
            this.tcMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Time Start:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Time End:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(66, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Task Totals:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(48, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Projects:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Daily Hours:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Project Daily Hours:";
            // 
            // tabProjects
            // 
            this.tabProjects.Controls.Add(this.dgvProjects);
            this.tabProjects.Location = new System.Drawing.Point(4, 22);
            this.tabProjects.Name = "tabProjects";
            this.tabProjects.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabProjects.Size = new System.Drawing.Size(901, 504);
            this.tabProjects.TabIndex = 3;
            this.tabProjects.Text = "Projects";
            this.tabProjects.UseVisualStyleBackColor = true;
            // 
            // dgvProjects
            // 
            this.dgvProjects.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvProjects.AutoGenerateColumns = false;
            this.dgvProjects.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProjects.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn,
            this.isBillableDataGridViewCheckBoxColumn});
            this.dgvProjects.DataSource = this.bsProject;
            this.dgvProjects.Location = new System.Drawing.Point(0, 0);
            this.dgvProjects.Name = "dgvProjects";
            this.dgvProjects.Size = new System.Drawing.Size(903, 536);
            this.dgvProjects.TabIndex = 0;
            this.dgvProjects.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dgvProjects_UserDeletingRow);
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            // 
            // isBillableDataGridViewCheckBoxColumn
            // 
            this.isBillableDataGridViewCheckBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.isBillableDataGridViewCheckBoxColumn.DataPropertyName = "IsBillable";
            this.isBillableDataGridViewCheckBoxColumn.HeaderText = "IsBillable";
            this.isBillableDataGridViewCheckBoxColumn.Name = "isBillableDataGridViewCheckBoxColumn";
            this.isBillableDataGridViewCheckBoxColumn.Width = 54;
            // 
            // bsProject
            // 
            this.bsProject.DataSource = typeof(DLaB.OutlookTimesheetCalculator.Project);
            this.bsProject.ListChanged += new System.ComponentModel.ListChangedEventHandler(this.bsProject_ListChanged);
            // 
            // tabTasks
            // 
            this.tabTasks.Controls.Add(this.cmbProjects);
            this.tabTasks.Controls.Add(this.label1);
            this.tabTasks.Controls.Add(this.dgvTasks);
            this.tabTasks.Location = new System.Drawing.Point(4, 22);
            this.tabTasks.Name = "tabTasks";
            this.tabTasks.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabTasks.Size = new System.Drawing.Size(901, 504);
            this.tabTasks.TabIndex = 1;
            this.tabTasks.Text = "Tasks";
            this.tabTasks.UseVisualStyleBackColor = true;
            // 
            // cmbProjects
            // 
            this.cmbProjects.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbProjects.DataSource = this.bsDefaultProject;
            this.cmbProjects.DisplayMember = "Name";
            this.cmbProjects.FormattingEnabled = true;
            this.cmbProjects.Location = new System.Drawing.Point(97, 6);
            this.cmbProjects.Name = "cmbProjects";
            this.cmbProjects.Size = new System.Drawing.Size(800, 21);
            this.cmbProjects.TabIndex = 5;
            this.cmbProjects.ValueMember = "Id";
            this.cmbProjects.Leave += new System.EventHandler(this.cmbProjects_Leave);
            // 
            // bsDefaultProject
            // 
            this.bsDefaultProject.DataSource = typeof(DLaB.OutlookTimesheetCalculator.Project);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Default Project:";
            // 
            // dgvTasks
            // 
            this.dgvTasks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTasks.AutoGenerateColumns = false;
            this.dgvTasks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTasks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn1,
            this.isBillableDataGridViewCheckBoxColumn1,
            this.projectDataGridViewComboBoxColumn});
            this.dgvTasks.DataSource = this.bsTask;
            this.dgvTasks.Location = new System.Drawing.Point(0, 33);
            this.dgvTasks.Name = "dgvTasks";
            this.dgvTasks.Size = new System.Drawing.Size(903, 503);
            this.dgvTasks.TabIndex = 0;
            this.dgvTasks.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvTasks_CellBeginEdit);
            this.dgvTasks.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTasks_CellEndEdit);
            this.dgvTasks.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.dgvTasks_DefaultValuesNeeded);
            // 
            // nameDataGridViewTextBoxColumn1
            // 
            this.nameDataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.nameDataGridViewTextBoxColumn1.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn1.HeaderText = "Task";
            this.nameDataGridViewTextBoxColumn1.Name = "nameDataGridViewTextBoxColumn1";
            // 
            // isBillableDataGridViewCheckBoxColumn1
            // 
            this.isBillableDataGridViewCheckBoxColumn1.DataPropertyName = "IsBillable";
            this.isBillableDataGridViewCheckBoxColumn1.HeaderText = "IsBillable";
            this.isBillableDataGridViewCheckBoxColumn1.Name = "isBillableDataGridViewCheckBoxColumn1";
            // 
            // projectDataGridViewComboBoxColumn
            // 
            this.projectDataGridViewComboBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.projectDataGridViewComboBoxColumn.DataPropertyName = "Project";
            this.projectDataGridViewComboBoxColumn.DataSource = this.bsProject;
            this.projectDataGridViewComboBoxColumn.DisplayMember = "Name";
            this.projectDataGridViewComboBoxColumn.FillWeight = 80F;
            this.projectDataGridViewComboBoxColumn.HeaderText = "Project";
            this.projectDataGridViewComboBoxColumn.Name = "projectDataGridViewComboBoxColumn";
            this.projectDataGridViewComboBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.projectDataGridViewComboBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.projectDataGridViewComboBoxColumn.ValueMember = "Id";
            // 
            // bsTask
            // 
            this.bsTask.DataSource = typeof(DLaB.OutlookTimesheetCalculator.Task);
            this.bsTask.ListChanged += new System.ComponentModel.ListChangedEventHandler(this.bsTask_ListChanged);
            // 
            // tabTime
            // 
            this.tabTime.Controls.Add(this.label8);
            this.tabTime.Controls.Add(this.calCLB);
            this.tabTime.Controls.Add(this.splitContainer1);
            this.tabTime.Controls.Add(this.lblHours);
            this.tabTime.Controls.Add(this.btnCalc);
            this.tabTime.Controls.Add(this.label3);
            this.tabTime.Controls.Add(this.dtpEnd);
            this.tabTime.Controls.Add(this.label2);
            this.tabTime.Controls.Add(this.dtpStart);
            this.tabTime.Location = new System.Drawing.Point(4, 22);
            this.tabTime.Name = "tabTime";
            this.tabTime.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabTime.Size = new System.Drawing.Size(901, 504);
            this.tabTime.TabIndex = 0;
            this.tabTime.Text = "Time";
            this.tabTime.UseVisualStyleBackColor = true;
            // 
            // calCLB
            // 
            this.calCLB.FormattingEnabled = true;
            this.calCLB.Location = new System.Drawing.Point(415, 9);
            this.calCLB.Name = "calCLB";
            this.calCLB.Size = new System.Drawing.Size(328, 49);
            this.calCLB.TabIndex = 10;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(11, 67);
            this.splitContainer1.MinimumSize = new System.Drawing.Size(300, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitter1);
            this.splitContainer1.Panel1.Controls.Add(this.TaskTotalsPanel);
            this.splitContainer1.Panel1.Controls.Add(this.ProjectsPanel);
            this.splitContainer1.Panel1.Controls.Add(this.ProjectDailyHoursPanel);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label5);
            this.splitContainer1.Panel2.Controls.Add(this.txtDailyHours);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.splitContainer1.Size = new System.Drawing.Size(882, 437);
            this.splitContainer1.SplitterDistance = 300;
            this.splitContainer1.TabIndex = 9;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(3, 303);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(294, 3);
            this.splitter1.TabIndex = 16;
            this.splitter1.TabStop = false;
            // 
            // TaskTotalsPanel
            // 
            this.TaskTotalsPanel.Controls.Add(this.label4);
            this.TaskTotalsPanel.Controls.Add(this.txtTaskDailyHours);
            this.TaskTotalsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TaskTotalsPanel.Location = new System.Drawing.Point(3, 141);
            this.TaskTotalsPanel.Name = "TaskTotalsPanel";
            this.TaskTotalsPanel.Size = new System.Drawing.Size(294, 165);
            this.TaskTotalsPanel.TabIndex = 12;
            // 
            // txtTaskDailyHours
            // 
            this.txtTaskDailyHours.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTaskDailyHours.Font = new System.Drawing.Font("Consolas", 10F);
            this.txtTaskDailyHours.Location = new System.Drawing.Point(3, 19);
            this.txtTaskDailyHours.Multiline = true;
            this.txtTaskDailyHours.Name = "txtTaskDailyHours";
            this.txtTaskDailyHours.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtTaskDailyHours.Size = new System.Drawing.Size(291, 143);
            this.txtTaskDailyHours.TabIndex = 9;
            this.txtTaskDailyHours.WordWrap = false;
            this.txtTaskDailyHours.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTaskDailyHours_KeyDown);
            // 
            // ProjectsPanel
            // 
            this.ProjectsPanel.Controls.Add(this.label7);
            this.ProjectsPanel.Controls.Add(this.lstProjects);
            this.ProjectsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.ProjectsPanel.Location = new System.Drawing.Point(3, 3);
            this.ProjectsPanel.Name = "ProjectsPanel";
            this.ProjectsPanel.Size = new System.Drawing.Size(294, 138);
            this.ProjectsPanel.TabIndex = 15;
            // 
            // lstProjects
            // 
            this.lstProjects.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstProjects.FormattingEnabled = true;
            this.lstProjects.Location = new System.Drawing.Point(0, 16);
            this.lstProjects.Name = "lstProjects";
            this.lstProjects.Size = new System.Drawing.Size(294, 121);
            this.lstProjects.TabIndex = 7;
            this.lstProjects.SelectedIndexChanged += new System.EventHandler(this.lstTasks_SelectedIndexChanged);
            // 
            // ProjectDailyHoursPanel
            // 
            this.ProjectDailyHoursPanel.Controls.Add(this.label6);
            this.ProjectDailyHoursPanel.Controls.Add(this.txtTasks);
            this.ProjectDailyHoursPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ProjectDailyHoursPanel.Location = new System.Drawing.Point(3, 306);
            this.ProjectDailyHoursPanel.Name = "ProjectDailyHoursPanel";
            this.ProjectDailyHoursPanel.Size = new System.Drawing.Size(294, 128);
            this.ProjectDailyHoursPanel.TabIndex = 14;
            // 
            // txtTasks
            // 
            this.txtTasks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTasks.Font = new System.Drawing.Font("Consolas", 10F);
            this.txtTasks.Location = new System.Drawing.Point(0, 19);
            this.txtTasks.Multiline = true;
            this.txtTasks.Name = "txtTasks";
            this.txtTasks.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtTasks.Size = new System.Drawing.Size(294, 106);
            this.txtTasks.TabIndex = 10;
            this.txtTasks.WordWrap = false;
            this.txtTasks.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTasks_KeyDown);
            // 
            // txtDailyHours
            // 
            this.txtDailyHours.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDailyHours.Font = new System.Drawing.Font("Consolas", 10F);
            this.txtDailyHours.Location = new System.Drawing.Point(6, 22);
            this.txtDailyHours.Multiline = true;
            this.txtDailyHours.Name = "txtDailyHours";
            this.txtDailyHours.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtDailyHours.Size = new System.Drawing.Size(561, 412);
            this.txtDailyHours.TabIndex = 5;
            this.txtDailyHours.WordWrap = false;
            this.txtDailyHours.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDailyHours_KeyDown);
            // 
            // lblHours
            // 
            this.lblHours.AutoSize = true;
            this.lblHours.Location = new System.Drawing.Point(10, 65);
            this.lblHours.Name = "lblHours";
            this.lblHours.Size = new System.Drawing.Size(0, 13);
            this.lblHours.TabIndex = 8;
            // 
            // btnCalc
            // 
            this.btnCalc.Location = new System.Drawing.Point(217, 18);
            this.btnCalc.Name = "btnCalc";
            this.btnCalc.Size = new System.Drawing.Size(98, 23);
            this.btnCalc.TabIndex = 4;
            this.btnCalc.Text = "Calculate Time";
            this.btnCalc.UseVisualStyleBackColor = true;
            this.btnCalc.Click += new System.EventHandler(this.btnCalc_Click);
            // 
            // dtpEnd
            // 
            this.dtpEnd.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpEnd.Location = new System.Drawing.Point(72, 32);
            this.dtpEnd.Name = "dtpEnd";
            this.dtpEnd.Size = new System.Drawing.Size(99, 20);
            this.dtpEnd.TabIndex = 2;
            // 
            // dtpStart
            // 
            this.dtpStart.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpStart.Location = new System.Drawing.Point(72, 6);
            this.dtpStart.Name = "dtpStart";
            this.dtpStart.Size = new System.Drawing.Size(99, 20);
            this.dtpStart.TabIndex = 0;
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tabTime);
            this.tcMain.Controls.Add(this.tabTasks);
            this.tcMain.Controls.Add(this.tabProjects);
            this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMain.Location = new System.Drawing.Point(0, 0);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(909, 530);
            this.tcMain.TabIndex = 0;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(352, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(57, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Calendars:";
            // 
            // OutlookTimesheetCalculatorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcMain);
            this.Name = "OutlookTimesheetCalculatorControl";
            this.Size = new System.Drawing.Size(909, 530);
            this.tabProjects.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProjects)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsProject)).EndInit();
            this.tabTasks.ResumeLayout(false);
            this.tabTasks.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsDefaultProject)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTasks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsTask)).EndInit();
            this.tabTime.ResumeLayout(false);
            this.tabTime.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.TaskTotalsPanel.ResumeLayout(false);
            this.TaskTotalsPanel.PerformLayout();
            this.ProjectsPanel.ResumeLayout(false);
            this.ProjectsPanel.PerformLayout();
            this.ProjectDailyHoursPanel.ResumeLayout(false);
            this.ProjectDailyHoursPanel.PerformLayout();
            this.tcMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TabPage tabProjects;
        private DataGridView dgvProjects;
        private TabPage tabTasks;
        private ComboBox cmbProjects;
        private Label label1;
        private DataGridView dgvTasks;
        private TabPage tabTime;
        private Label lblHours;
        private TextBox txtDailyHours;
        private TextBox txtTaskDailyHours;
        private ListBox lstProjects;
        private Button btnCalc;
        private DateTimePicker dtpEnd;
        private DateTimePicker dtpStart;
        private TabControl tcMain;
        private BindingSource bsTask;
        private BindingSource bsProject;
        private BindingSource bsDefaultProject;
        private DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private DataGridViewCheckBoxColumn isBillableDataGridViewCheckBoxColumn;
        private DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn1;
        private DataGridViewCheckBoxColumn isBillableDataGridViewCheckBoxColumn1;
        private DataGridViewComboBoxColumn projectDataGridViewComboBoxColumn;
        private SplitContainer splitContainer1;
        private Panel ProjectsPanel;
        private Panel ProjectDailyHoursPanel;
        private Splitter splitter1;
        private Panel TaskTotalsPanel;
        private TextBox txtTasks;
        private Label label2;
        private Label label3;
        private Label label6;
        private Label label7;
        private Label label5;
        private Label label4;
        private CheckedListBox calCLB;
        private Label label8;
    }
}

