using System;
using System.IO;
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
            OptionSettingsPath = Path.Combine(Paths.SettingsPath, "DLaB.OutlookTimesheetCalculator", "Options.xml");
            ProjectsPath = Path.Combine(Paths.SettingsPath, "DLaB.OutlookTimesheetCalculator", "Projects.xml");
            TasksPath = Path.Combine(Paths.SettingsPath, "DLaB.OutlookTimesheetCalculator", "Tasks.xml");
        }
    }
}