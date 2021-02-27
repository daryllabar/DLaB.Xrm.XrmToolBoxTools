using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Collections.ObjectModel;
using DLaB.XrmToolBoxCommon;
using XrmToolBox.Extensibility;

namespace DLaB.OutlookTimesheetCalculator
{
    public partial class OutlookTimesheetCalculatorControl : DLaBPluginControlBase
    {
        public Settings Settings { get; set; }
        public OptionSettings OptionSettings { get; set; }
        public ObservableCollection<Task> Tasks { get; set; }
        // public List<Task> TempRegexTasks { get; set; }
        public ObservableCollection<Project> Projects { get; set; }
        private Project DefaultProject => cmbProjects.SelectedItem as Project;

        private List<AppointmentTask> AppointmentTasks { get; set; }

        private void ShowError(string message, Exception ex)
        {
            MessageBox.Show(ex == null ? message : message + Environment.NewLine + ex, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public OutlookTimesheetCalculatorControl()
        {
            InitializeComponent();
            // Loads or creates the settings for the plugin
            if (!SettingsManager.Instance.TryLoad(GetType(), out Settings settings))
            {
                Settings = new Settings();

                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                Settings = settings;
                LogInfo("Settings found and loaded");
            }
            InitializeProjects();
            BindProjects();
            InitializeOptionSettings();
            InitializeTasks();
            BindTasks();

            DateTime start = DateTime.Now.Date.AddDays(-(int)DateTime.Now.DayOfWeek + 1);
            if (start > DateTime.Now)
            {
                start = start.AddDays(-7);
            }
            DateTime end = DateTime.Now.Date;
            dtpStart.Value = start;
            dtpEnd.Value = end;
        }

        private void InitializeProjects()
        {
            if (File.Exists(Settings.ProjectsPath))
            {
                try
                {
                    Projects = new ObservableCollection<Project>(Read<List<Project>>(Settings.ProjectsPath).OrderBy(p=> p.Name));
                }
                catch (Exception ex)
                {
                    ShowError("Error Reading Settings File", ex);
                    throw;
                }
            }
            else
            {
                Projects = new ObservableCollection<Project>
                {
                    new Project {Name = "Client XYZ", IsBillable = true, Id = Guid.NewGuid()},
                    new Project {Name = "Community", IsBillable = false, Id = Guid.NewGuid()},
                    new Project {Name = "Personal", IsBillable = true, Id = Guid.NewGuid()}
                };
            }
        }

        private void BindProjects()
        {
            bsProject.DataSource = Projects;
            //foreach (var project in Projects)
            //{
            //    int index = dgvProjects.Rows.Add(project.Name, project.IsBillable);
            //    dgvProjects.Rows[index].Tag = project;
            //}
            bsDefaultProject.DataSource = Projects;
        }

        private void InitializeTasks()
        {
            if (File.Exists(Settings.TasksPath))
            {
                try
                {
                    //List<Task> temp = new List<Task>();
                    //if (Tasks != null)
                    //{
                    //    temp = Tasks.Where(t => t.IsRegExTemp).ToList();
                    //}
                    Tasks = new ObservableCollection<Task>(OrderTasks(Read<List<Task>>(Settings.TasksPath)));
                    //foreach (var task in Tasks)
                    //{
                    //    dgvTasks.Rows.Add(task.Name, task.IsBillable, task.Project);
                    //}
                }
                catch (Exception ex)
                {
                    ShowError("Erorr Reading Settings File", ex);
                    throw;
                }
            }
            else
            {
                Tasks = new ObservableCollection<Task>();
                var project = Projects.FirstOrDefault();
                if(project != null)
                {
                    Tasks.Add(new Task { Name = "Scrum*", IsBillable = true, Project = project.Id });
                    Tasks.Add(new Task { Name = "XYZ - *", IsBillable = true, Project = project.Id });
                }

                project = Projects.FirstOrDefault(p => p.Name == "Personal");
                if (project != null)
                {
                    Tasks.Add(new Task { Name = "Holiday", IsBillable = false, Project = project.Id });
                    Tasks.Add(new Task { Name = "Lunch", IsBillable = false, Project = project.Id });
                    Tasks.Add(new Task { Name = "Vacation", IsBillable = false, Project = project.Id });
                }

            }
        }

        private void BindTasks()
        {
            bsTask.DataSource = null;
            Tasks = new ObservableCollection<Task>(OrderTasks(Read<List<Task>>(Settings.TasksPath)));
            bsTask.DataSource = Tasks;
        }

        private void InitializeOptionSettings()
        {
            if (File.Exists(Settings.OptionSettingsPath))
            {
                try
                {
                    OptionSettings = Read<OptionSettings>(Settings.OptionSettingsPath);
                }
                catch (Exception ex)
                {
                    ShowError("Error Reading Settings File", ex);
                    throw;
                }
            }
            else
            {
                OptionSettings = new OptionSettings
                {
                    DefaultProject = Projects.First().Id
                };
            }
            cmbProjects.SelectedValue = OptionSettings.DefaultProject;
        }

        private List<Outlook.AppointmentItem> GetOutlookAppointments(DateTime start, DateTime end)
        {
            var outlook = new Microsoft.Office.Interop.Outlook.Application();
            var app = outlook.Application;
            var appSession = app.Session;
            var calendar = appSession.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderCalendar);
            //string filter = String.Format("[Start] >= {0} And [Start] < {1} And [End] > {0} And [End] <= {1}", start.ToString("ddddd h:nn AMPM"), end.ToString("ddddd h:nn AMPM"));
            string filter = string.Format("([Start] >= '{0}' AND [Start] < '{1}') OR ([End] > '{0}' AND [End] <= '{1}')", start.ToString("g"), end.ToString("g"));
            var items = calendar.Items;

            items.Sort("[Start]", Type.Missing);
            items.IncludeRecurrences = true;
            var restrictedItems = items.Restrict(filter);

            Outlook.AppointmentItem appointment;
            var appointments = new List<Outlook.AppointmentItem>();
            if (items.Count <= 0) { return appointments; }
            foreach (var item in restrictedItems)
            {
                appointment = item as Outlook.AppointmentItem;
                if (appointment != null && !appointment.AllDayEvent && (appointment.BusyStatus != Outlook.OlBusyStatus.olOutOfOffice || Tasks.Any(t => string.Equals(t.Name, appointment.Subject, StringComparison.InvariantCultureIgnoreCase))))
                {
                    //if (appointment.IsRecurring)
                    //{
                    //    AddReoccuringAppointments(appointments, start, end, appointment);
                    //}
                    //else 
                        if (appointment.Start.Date != appointment.End.Date)
                    {
                        appointments.AddRange(SplitAppointments(appointment, dtpEnd.Value.AddDays(1).Date));
                    }
                    else if (appointment.Start >= start && appointment.End <= end)
                    {
                        appointments.Add(appointment);
                    }
                }
            }

            //foreach (Outlook.AppointmentItem item in appointments)
            //{
            //    if (item.Conflicts.Count > 0)
            //    {
            //        Console.WriteLine("Never gets hit");
            //    }

            //}

            DateTime appointmentEnd = DateTime.MinValue;
            foreach (var appt in appointments.OrderBy(a => a.Start).ThenBy(a => a.CreationTime).ToList())
            {
                if (appointmentEnd > appt.Start)
                {
                    // remove appointment.  We may also need to edit it, but don't want to update Outlook with the changes...
                    appointments.Remove(appt);
                    if (appointmentEnd < appt.End)
                    {
                        // Update Appointment to be outside of first.
                        Outlook.AppointmentItem copy = new NonOutlookAppointmentItem(appt);
                        copy.Duration = (int)(appt.End - appointmentEnd).TotalMinutes;
                        copy.Start = appointmentEnd;
                        copy.End = copy.Start.AddMinutes(copy.Duration);
                        appointmentEnd = copy.End;
                        appointments.Add(copy);
                    }
                }
                else
                {
                    appointmentEnd = appt.End;
                }
            }

            return appointments.Where(a => a.Start != a.End).OrderBy(a => a.Start).ToList();
        }

        private IEnumerable<Outlook.AppointmentItem> SplitAppointments(Outlook.AppointmentItem appointment, DateTime maxEnd)
        {
            Outlook.AppointmentItem splitAppointment;
            DateTime index = appointment.Start;
            DateTime eodOrAppointment;
            if (appointment.End < maxEnd)
            {
                maxEnd = appointment.End;
            }
            while (index < maxEnd)
            {
                // Set eod Or Appointment to the eod of the start, or if the date of the end and the date of the index are equal, set to end of appointment
                eodOrAppointment = maxEnd.Date == index.Date ? maxEnd : index.AddDays(1).Date;

                splitAppointment = new NonOutlookAppointmentItem(appointment);
                splitAppointment.Duration = (int)(eodOrAppointment - index).TotalMinutes;
                splitAppointment.Start = index;
                splitAppointment.End = index.AddMinutes(splitAppointment.Duration);
                index = eodOrAppointment;
                yield return splitAppointment;
            }
        }

        private void cmbProjects_Leave(object sender, EventArgs e)
        {
            Project project = (Project)cmbProjects.SelectedItem;
            if (project.Id != OptionSettings.DefaultProject)
            {
                OptionSettings.DefaultProject = project.Id;
                Save(OptionSettings, Settings.OptionSettingsPath);
            }
        }

        private void Save<T>(T settings, string path)
        {
            try
            {
                var directory = Path.GetDirectoryName(path) ?? string.Empty;
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                using (var writer = new StreamWriter(path))
                {
                    System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    x.Serialize(writer, settings);
                }
            }
            catch (Exception ex)
            {
                ShowError("Error Saving " + typeof(T).Name + " File", ex);
            }
        }

        private T Read<T>(string path)
        {
            try
            {
                using (var reader = new StreamReader(path))
                {
                    System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    return (T)x.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                ShowError("Error Loading " + typeof(T).Name + " File", ex);
                return default(T);
            }
        }

        private void dgvTasks_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(((string)e.Row.Cells[2].Value)))
            {
                e.Row.Cells[1].Value = DefaultProject.IsBillable;
                e.Row.Cells[2].Value = DefaultProject.Id;
            }
        }

        #region Projects Grid View Updates

        private void dgvProjects_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            e.Cancel = Tasks.Any(t => t.Project == ((Project)e.Row.DataBoundItem).Id);
        }

        //private void UpdateProjects()
        //{
        //    ClearCalculations();
        //    Tasks.Clear();
        //    foreach (DataGridViewRow row in dgvTasks.Rows)
        //    {
        //        if (row.Cells[1].Value == null || String.IsNullOrWhiteSpace(((string)row.Cells[0].Value))) { continue; }

        //        Tasks.Add(new Task()
        //        {
        //            Name = (string)row.Cells[0].Value,
        //            IsBillable = (bool)row.Cells[1].Value,
        //            Project = (string)row.Cells[2].Value
        //        });
        //    }
        //    OrderTasks();
        //    SaveTasks();
        //}

        #endregion // Projects Grid View Updates

        #region Tasks Grid View Updates

        //private void dgvTasks_RowValidated(object sender, DataGridViewCellEventArgs e)
        //{
        //    UpdateTasks();
        //}

        //private void UpdateTasks()
        //{
        //    ClearCalculations();
        //    Tasks.Clear();
        //    foreach (DataGridViewRow row in dgvTasks.Rows)
        //    {
        //        if (row.Cells[1].Value == null || String.IsNullOrWhiteSpace(((string)row.Cells[0].Value))) { continue; }

        //        Tasks.Add(new Task()
        //        {
        //            Name = (string)row.Cells[0].Value,
        //            IsBillable = (bool)row.Cells[1].Value,
        //            Project = (string)row.Cells[2].Value
        //        });
        //    }
        //    OrderTasks();
        //    SaveTasks();
        //}

        #endregion // Tasks Grid View Updates

        private IEnumerable<Task> OrderTasks(IEnumerable<Task> tasks)
        {
            var lookup = Projects.ToDictionary(p => p.Id, p => p.Name);
            return tasks.OrderBy(t => lookup[t.Project]).ThenBy(t => t.Name).ToList();
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            ClearCalculations();

            AppointmentTasks = MatchAppointmentsToTasks(GetOutlookAppointments(dtpStart.Value, dtpEnd.Value.AddDays(1)));

            lblHours.Text = $@"Total Hours {Math.Round(GetTime(AppointmentTasks).TotalHours, 2)} ({Math.Round(GetTime(AppointmentTasks.Where(at => at.Task.IsBillable)).TotalHours, 2)} Billable)";
            
            TimeSpan time;
            foreach (var task in AppointmentTasks.Select(a => a.Task).Distinct().OrderBy(t => t.Name))
            {
                time = GetTime(AppointmentTasks.Where(at => at.Task == task));
                if (time.TotalHours > 0)
                    txtTasks.AppendText(task.Name + " = " + time.TotalHours + Environment.NewLine);
            }

            foreach (var project in AppointmentTasks.GroupBy(k => k.Project).Select(p => new ListBoxItem<Project>(p.Key, p.Key.Name + " (" + GetTime(p).TotalHours + " hours)")).OrderBy(p => p.Text))
            {
                lstProjects.Items.Add(project);
            }

            if (lstProjects.Items.Count > 0)
            {
                lstProjects.SelectedItem = lstProjects.Items[0];
            }

            for (DateTime day = dtpStart.Value; day < dtpEnd.Value.AddDays(1); day = day.AddDays(1))
            {
                var totalHours = 0.0;
                var totalBillableHours = 0.0;
                var sb = new StringBuilder();
                foreach (var project in Projects)
                {
                    var tasksForProjectForDay = AppointmentTasks.Where(at => at.Project?.Equals(project) == true && day <= at.Appointment.Start && day.AddDays(1) >= at.Appointment.End).ToList();
                    time = GetTime(tasksForProjectForDay);
                    if (time.TotalHours > 0)
                    {
                        totalHours += time.TotalHours;
                        if (tasksForProjectForDay.First().Task.IsBillable) {
                            totalBillableHours += time.TotalHours;
                        }
                        sb.AppendLine(project.Name + " = " + time.TotalHours);

                        foreach (var at in tasksForProjectForDay.GroupBy(t => t.Task.Name).OrderBy(g => g.Key))
                        {
                            sb.AppendLine($"\t- {at.Key} = {GetTime(at).TotalHours}");
                        }
                    }
                }
                if (totalHours > 0.0)
                {
                    txtDailyHours.AppendText($"{day.DayOfWeek} {day.ToShortDateString()} Total Hours {Math.Round(totalHours, 2)} ({Math.Round(totalBillableHours, 2)} Billable){Environment.NewLine}");
                    sb.AppendLine();
                    txtDailyHours.AppendText(sb.ToString());
                }
            }

            txtTasks.SelectionStart = 0;
            txtTasks.ScrollToCaret();
            txtDailyHours.SelectionStart = 0;
            txtDailyHours.ScrollToCaret();
        }

        private void ClearCalculations()
        {
            lstProjects.Items.Clear();
            txtTasks.Clear();
            txtTaskDailyHours.Clear();
            txtDailyHours.Clear();
            lblHours.Text = string.Empty;

            //var outlook = new Microsoft.Office.Interop.Outlook.Application();
            //var app = outlook.Application;
            //var appSession = app.Session;
            //var inbox = appSession.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderInbox);
            //foreach (Outlook.Folder folder in inbox.Folders)
            //{
            //    if (folder.FolderPath == @"\\dlabar@allegient.com\Inbox\POA")
            //    {
            //        StringBuilder sb = new StringBuilder();
            //        List<Outlook.MailItem> items = new List<Outlook.MailItem>();

            //        foreach (Outlook.MailItem email in folder.Items)
            //        {
            //            items.Add(email);
            //        }
            //        foreach (Outlook.MailItem email in items.OrderBy(e => e.SentOn))
            //        {
            //            DateTime i = email.SentOn;
            //            String b = email.Body;
            //            sb.AppendLine(i.ToString() + "\t" + b.Substring(0, b.IndexOf(' ')));
            //        }
            //    }
            //}

        }

        private List<AppointmentTask> MatchAppointmentsToTasks(List<Outlook.AppointmentItem> appointments)
        {
            var projectLookup = Projects.ToDictionary(p => p.Id);
            List<Task> tempTasks = new List<Task>(Tasks);
            List<AppointmentTask> appointmentTasks = new List<AppointmentTask>(appointments.Count);
            List<Task> defaultProjectTasks = Tasks.Where(t => t.Project == DefaultProject.Id).ToList();
            bool newTask = false;
            foreach (var appt in appointments)
            {
                var subject = string.IsNullOrWhiteSpace(appt.Subject)
                    ? "No Subject!"
                    : appt.Subject;
                // Everything Matches
                Task task = defaultProjectTasks.FirstOrDefault(t => string.Equals(t.Name, subject));
                if (task == null)
                {
                    task = tempTasks.FirstOrDefault(t => string.Equals(t.Name, subject));
                }
                // Starts with
                if (task == null)
                {
                    task = defaultProjectTasks.FirstOrDefault(t => subject.ToUpper().StartsWith(t.Name.ToUpper()));
                }
                if (task == null)
                {
                    task = tempTasks.FirstOrDefault(t => subject.ToUpper().StartsWith(t.Name.ToUpper()));
                }
                // Matches Reg Ex, create Temp
                if (task == null)
                {
                    foreach (var regExTask in tempTasks.Where(t => !string.IsNullOrWhiteSpace(t.Regex)))
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch(subject, regExTask.Regex, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                        {
                            task = new Task()
                            {
                                Name = subject,
                                IsBillable = regExTask.IsBillable,
                                Project = regExTask.Project,
                                IsRegExTemp = true
                            };
                            tempTasks.Add(task);
                            break;
                        }
                    }
                }
                // Is a Meeting
                if (task == null && !string.IsNullOrWhiteSpace(appt.Location))
                {
                    task = defaultProjectTasks.FirstOrDefault(t => t.Name.ToUpper() == "MEETING");
                }
                if (task == null && !string.IsNullOrWhiteSpace(appt.Location))
                {
                    task = tempTasks.FirstOrDefault(t => t.Name.ToUpper() == "MEETING");
                }

                if (task == null)
                {
                    task = new Task()
                    {
                        Name = subject.TrimEnd(),
                        IsBillable = true,
                        Project = DefaultProject.Id
                    };
                    newTask = true;
                    Tasks.Add(task);
                    tempTasks.Add(task);
                }
                appointmentTasks.Add(new AppointmentTask() { Task = task, Appointment = appt, Project = projectLookup[task.Project] });
            }

            if (newTask)
            {
                SaveTasks();
                BindTasks();
                // Redo this so we have the same object references to Tasks
                appointmentTasks = MatchAppointmentsToTasks(appointments);
            }
            return appointmentTasks;
        }

        private void SaveTasks()
        {
            Save(Tasks.Where(t=> !t.IsRegExTemp).ToList(), Settings.TasksPath);
        }

        private void SaveProjects()
        {
            Save(Projects.ToList(), Settings.ProjectsPath);
        }

        private static TimeSpan GetTime(IEnumerable<AppointmentTask> appointmentTasks)
        {
            TimeSpan totalTime = new TimeSpan();
            foreach (var at in appointmentTasks)
            {
                var time = (at.Appointment.End - at.Appointment.Start);
                totalTime += time;
            }
            return totalTime;
        }

        private class AppointmentTask
        {
            public Outlook.AppointmentItem Appointment { get; set; }
            public Task Task { get; set; }
            public Project Project { get; set; }
            public override string ToString()
            {
                return $"{Task.Name} {Appointment.Start} - {Appointment.End} {Appointment.Duration / 60m}hrs";
            }
        }

        private void lstTasks_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtTaskDailyHours.Clear();
            List<AppointmentTask> tasksForProjectForDay;
            StringBuilder sb;
            double totalHours;
            TimeSpan time;
            var selectedItem = (ListBoxItem<Project>)lstProjects.SelectedItem;
            for (DateTime day = dtpStart.Value; day < dtpEnd.Value.AddDays(1); day = day.AddDays(1))
            {
                totalHours = 0.0;
                sb = new StringBuilder();

                tasksForProjectForDay = AppointmentTasks.Where(at => at.Task.Project == selectedItem.Value.Id && day <= at.Appointment.Start && day.AddDays(1) >= at.Appointment.End).ToList();
                time = GetTime(tasksForProjectForDay);
                if (time.TotalHours > 0)
                {
                    totalHours += time.TotalHours;

                    foreach (var at in tasksForProjectForDay.GroupBy(t => t.Task.Name))
                    {
                        sb.AppendLine($"\t- {at.Key} = {GetTime(at).TotalHours}");
                    }
                }
                
                if (totalHours > 0.0)
                {
                    txtTaskDailyHours.AppendText(string.Format("{0} {1} Total Hours {2}{3}{4}{3}", day.DayOfWeek, day.ToShortDateString(), Math.Round(totalHours,2), Environment.NewLine, sb));
                    txtTaskDailyHours.SelectionStart = 0;
                    txtTaskDailyHours.ScrollToCaret();
                }
            }
        }

        #region Select All Logic

        private void txtTasks_KeyDown(object sender, KeyEventArgs e) { HandleSelectAll((TextBox)sender, e); }

        private void txtTaskDailyHours_KeyDown(object sender, KeyEventArgs e) { HandleSelectAll((TextBox)sender, e); }

        private void txtDailyHours_KeyDown(object sender, KeyEventArgs e) { HandleSelectAll((TextBox)sender, e); }

        private void HandleSelectAll(TextBox txtBox, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode.ToString() == "A")
            {
                txtBox.SelectAll();
            }
        }

        #endregion // Select All Logic

        private void bsProject_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                case ListChangedType.ItemDeleted:
                case ListChangedType.ItemMoved:
                case ListChangedType.Reset:
                    if (Projects != null)
                    {
                        bsDefaultProject.DataSource = null;
                        bsDefaultProject.DataSource = Projects.OrderBy(p => p.Name);
                        SaveProjects();
                    }
                    break;
                case ListChangedType.ItemAdded:
                    var newProject = ((Project)((BindingSource)sender)[e.NewIndex]);
                    newProject.Id = Guid.NewGuid();
                    newProject.IsBillable = true;
                    break;
            }
        }

        private void bsTask_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                case ListChangedType.ItemDeleted:
                case ListChangedType.ItemMoved:
                case ListChangedType.Reset:
                    if (Tasks != null)
                    {
                        SaveTasks();
                    }
                    break;
            }
        }

        public Guid ProjectPreEditValue { get; set; }

        private void dgvTasks_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == projectDataGridViewComboBoxColumn.Index)
            {
                var projectId = (Guid)((DataGridView)sender)[e.ColumnIndex, e.RowIndex].Value;
                if (ProjectPreEditValue != projectId)
                {
                    ((DataGridView)sender)[e.ColumnIndex - 1, e.RowIndex].Value = Projects.First(p => p.Id == projectId).IsBillable;
                }
            }
        }

        private void dgvTasks_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex == projectDataGridViewComboBoxColumn.Index)
            {
                ProjectPreEditValue = (Guid)((DataGridView)sender)[e.ColumnIndex, e.RowIndex].Value;
            }
        }
    }

    public class ListBoxItem<T>
    {
        public string Text { get; set; }
        public T Value { get; set; }

        public ListBoxItem(T value, string text)
        {
            Text = text;
            Value = value;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
