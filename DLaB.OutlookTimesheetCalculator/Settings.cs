using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XrmToolBox.Extensibility;

namespace DLaB.OutlookTimesheetCalculator
{
    /// <summary>
    /// This class can help you to store settings for your plugin
    /// </summary>
    /// <remarks>
    /// This class must be XML serializable
    /// </remarks>
    [Serializable]
    public class Settings
    {
        public string OptionSettingsPath { get; set; }
        public string ProjectsPath { get; set; }
        public string TasksPath { get; set; }

        public Settings()
        {
            OptionSettingsPath = Path.Combine(Paths.SettingsPath, "DLaB.OutlookTimesheetCalulator", "Options.xml");
            ProjectsPath = Path.Combine(Paths.SettingsPath, "DLaB.OutlookTimesheetCalulator", "Projects.xml");
            TasksPath = Path.Combine(Paths.SettingsPath, "DLaB.OutlookTimesheetCalulator", "Tasks.xml");
        }
    }
}