using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLaB.EarlyBoundGenerator.Settings;
using System.Speech.Synthesis;

namespace DLaB.EarlyBoundGenerator
{
    public class Logic
    {
        private readonly object _updateAppConfigToken = new Object();
        private readonly object _speakToken = new Object();
        private Config Config { get; set; }
        private Boolean _configUpdated;

        public Logic(Config config)
        {
            Config = config;
        }

        public void CreateActions()
        {
            Create(CreationType.Actions);
        }

        public void ExecuteAll()
        {
            if (Config.SupportsActions)
            {
                Parallel.Invoke(CreateActions, CreateEntities, CreateOptionSets);
            }
            else
            {
                Parallel.Invoke(CreateEntities, CreateOptionSets);
            }
        }

        public void CreateEntities()
        {
            Create(CreationType.Entities);
        }


        private void Create(CreationType creationType)
        {
            var filePath = GetOutputFilePath(Config, creationType);
            // Check for file to be editable if not using TFS and creating only one file
            if (!Config.ExtensionConfig.UseTfsToCheckoutFiles 
                && ((creationType == CreationType.Actions && !Config.ExtensionConfig.CreateOneFilePerAction) || 
                    (creationType == CreationType.Entities && !Config.ExtensionConfig.CreateOneFilePerEntity) ||
                    (creationType == CreationType.OptionSets && !Config.ExtensionConfig.CreateOneFilePerOptionSet))
                && !AbleToMakeFileAccessible(filePath))
            {
                return;
            }

            var date = File.GetLastWriteTimeUtc(filePath);
            var p = new Process
            {
                StartInfo =
                {
                    FileName = Config.CrmSvcUtilPath,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    Arguments = GetConfigArguments(Config, creationType),
                },
            };

            if (!File.Exists(p.StartInfo.FileName))
            {
                throw new FileNotFoundException("Unable to locate CrmSvcUtil at path '" + p.StartInfo.FileName +"'.  Update the CrmSvcUtilRelativePath in the DLaB.EarlyBoundGeneratorPlugin.Settings.xml file and try again.");
            }

            var args = GetSafeArgs(Config, p);
            if (Config.IncludeCommandLine)
            {
                switch (creationType)
                {
                    case CreationType.Actions:
                        Config.ExtensionConfig.ActionCommandLineText = "\"" + p.StartInfo.FileName + "\" " + args;
                        break;
                    case CreationType.All:
                        break;
                    case CreationType.Entities:
                        Config.ExtensionConfig.EntityCommandLineText = "\"" + p.StartInfo.FileName + "\" " + args;
                        break;
                    case CreationType.OptionSets:
                        Config.ExtensionConfig.OptionSetCommandLineText = "\"" + p.StartInfo.FileName + "\" " + args;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(creationType));
                }
            }
            UpdateCrmSvcUtilConfig(Config);
            UpdateStatus("Shelling out to CrmSrvUtil for creating " + creationType, "Executing \"" + p.StartInfo.FileName + "\" " + args);
            p.Start();
            var consoleOutput = new StringBuilder();
            while (!p.StandardOutput.EndOfStream)
            {
                var line = p.StandardOutput.ReadLine();
                UpdateStatus(line);
                consoleOutput.AppendLine(line);
            }

            HandleResult(filePath, date, creationType, consoleOutput.ToString());
        }

        protected bool AbleToMakeFileAccessible(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }

            if (!File.GetAttributes(filePath).HasFlag(FileAttributes.ReadOnly)) { return true; }

            try
            {
                new FileInfo(filePath) {IsReadOnly = false}.Refresh();
            }
            catch (Exception ex)
            {
                UpdateStatus("Unable to set IsReadOnly Flag to false " + filePath + Environment.NewLine + ex);
                return false;
            }

            if (!File.GetAttributes(filePath).HasFlag(FileAttributes.ReadOnly)) { return true; }

            UpdateStatus("File \"" + filePath + "\" is read only, please checkout the file before running");
            return false;
        }

        private string GetOutputFilePath(Config config, CreationType creationType)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            var filePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), Config.GetSettingValue(creationType, Config.UserArgumentNames.Out));

            if (creationType == CreationType.Actions && config.ExtensionConfig.CreateOneFilePerAction)
            {
                return Path.Combine(filePath, "Actions.cs");
            }
            else if (creationType == CreationType.Entities && config.ExtensionConfig.CreateOneFilePerEntity)
            {
                return Path.Combine(filePath, config.ServiceContextName + ".cs");
            }
            else if (creationType == CreationType.OptionSets && config.ExtensionConfig.CreateOneFilePerOptionSet)
            {
                return Path.Combine(filePath, "OptionSets.cs");
            }
            else
            {
                return filePath;
            }
        }

        private static string GetSafeArgs(Config config, Process p)
        {
            var args = p.StartInfo.Arguments;
            if (config.MaskPassword && !string.IsNullOrWhiteSpace(config.Password))
            {
                args = p.StartInfo.Arguments.Replace(config.Password, new string('*', config.Password.Length));
            }
            return args;
        }

        private void UpdateCrmSvcUtilConfig(Config config)
        {
            lock (_updateAppConfigToken)
            {
                if (_configUpdated) { return; }
                //load custom config file
                Configuration file;

                string filePath = Path.GetFullPath(config.CrmSvcUtilPath) + ".config";
                var map = new ExeConfigurationFileMap { ExeConfigFilename = filePath };
                try
                {
                    file = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
                }
                catch (ConfigurationErrorsException ex)
                {
                    if (ex.BareMessage == "Root element is missing.")
                    {
                        File.Delete(filePath);
                        UpdateCrmSvcUtilConfig(config);
                        return;
                    }
                    throw;
                }

                if (UpdateConfigAppSetting(file, "ActionCommandLineText", config.ExtensionConfig.ActionCommandLineText, true) |
                    UpdateConfigAppSetting(file, "ActionsToSkip", config.ExtensionConfig.ActionsToSkip) |
                    UpdateConfigAppSetting(file, "AddDebuggerNonUserCode", config.ExtensionConfig.AddDebuggerNonUserCode.ToString()) |
                    UpdateConfigAppSetting(file, "AddNewFilesToProject", config.ExtensionConfig.AddNewFilesToProject.ToString()) |
                    UpdateConfigAppSetting(file, "CreateOneFilePerAction", config.ExtensionConfig.CreateOneFilePerAction.ToString()) |
                    UpdateConfigAppSetting(file, "CreateOneFilePerEntity", config.ExtensionConfig.CreateOneFilePerEntity.ToString()) |
                    UpdateConfigAppSetting(file, "CreateOneFilePerOptionSet", config.ExtensionConfig.CreateOneFilePerOptionSet.ToString()) |
                    UpdateConfigAppSetting(file, "EntityAttributeSpecifiedNames", config.ExtensionConfig.EntityAttributeSpecifiedNames) |
                    UpdateConfigAppSetting(file, "EntityCommandLineText", config.ExtensionConfig.EntityCommandLineText, true) |
                    UpdateConfigAppSetting(file, "EntitiesToSkip", config.ExtensionConfig.EntitiesToSkip) |
                    UpdateConfigAppSetting(file, "GenerateAttributeNameConsts", config.ExtensionConfig.GenerateAttributeNameConsts.ToString()) |
                    UpdateConfigAppSetting(file, "GenerateAnonymousTypeConstructor", config.ExtensionConfig.GenerateAnonymousTypeConstructor.ToString()) |
                    UpdateConfigAppSetting(file, "GenerateEnumProperties", config.ExtensionConfig.GenerateEnumProperties.ToString()) |
                    UpdateConfigAppSetting(file, "OptionSetsToSkip", config.ExtensionConfig.OptionSetsToSkip) |
                    UpdateConfigAppSetting(file, "OptionSetCommandLineText", config.ExtensionConfig.OptionSetCommandLineText, true) |
                    UpdateConfigAppSetting(file, "PropertyEnumMappings", config.ExtensionConfig.PropertyEnumMappings) |
                    UpdateConfigAppSetting(file, "RemoveRuntimeVersionComment", config.ExtensionConfig.RemoveRuntimeVersionComment.ToString()) |
                    UpdateConfigAppSetting(file, "UnmappedProperties", config.ExtensionConfig.UnmappedProperties) |
                    UpdateConfigAppSetting(file, "UseTfsToCheckoutFiles", config.ExtensionConfig.UseTfsToCheckoutFiles.ToString()) |
                    UpdateConfigAppSetting(file, "UseXrmClient", config.ExtensionConfig.UseXrmClient.ToString()))

                {
                    file.Save(ConfigurationSaveMode.Minimal);
                }
                _configUpdated = true;
            }
        }

        private static bool UpdateConfigAppSetting(Configuration file, string key, string configValue, bool keepWhiteSpace = false)
        {
            configValue = configValue ?? string.Empty;
            if (!keepWhiteSpace)
            {
                configValue = configValue.Replace(" ", "");
                configValue = configValue.Replace("\n", "");
            }
            bool update = false;
            var value = file.AppSettings.Settings[key];
            if (value == null)
            {
                update = true;
                file.AppSettings.Settings.Add(key, configValue);
            }
            else if (value.Value != configValue)
            {
                update = true;
                value.Value = configValue;
            }
            return update;
        }

        private string GetConfigArguments(Config config, CreationType type)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("/url:\"{0}\" ", config.Url);

            foreach (var argument in config.CommandLineArguments.Where(a => a.SettingType == CreationType.All || a.SettingType == type))
            {
                var value = argument.Value;
                if (argument.Name == "out")
                {
                    value = GetOutputFilePath(config, type);
                }
                if (argument.Value == null)
                {
                    sb.AppendFormat("/{0} ", argument.Name);
                }
                else
                {
                    sb.AppendFormat("/{0}:\"{1}\" ", argument.Name, value);
                }
            }

            if (!string.IsNullOrWhiteSpace(config.Password))
            {
                sb.AppendFormat("/username:\"{0}\" ", config.UserName);
                sb.AppendFormat("/password:\"{0}\" ", config.Password);

                // Add Login Info
                if (!config.UseCrmOnline && !string.IsNullOrWhiteSpace(config.Domain))
                {
                    sb.AppendFormat("/domain:\"{0}\" ", config.Domain);
                }
            }

            return sb.ToString();
        }

        public void CreateOptionSets()
        {
            Create(CreationType.OptionSets);
        }

        private void HandleResult(string filePath, DateTime date, CreationType creationType, string consoleOutput)
        {
            var speaker = new SpeechSynthesizer();
            try
            {
                //if (creationType == CreationType.Actions && Config.ExtensionConfig.CreateOneFilePerAction)
                //{
                //    var tempPath = filePath;
                //    filePath = "Actions.cs";
                //    if (!File.Exists(tempPath))
                //    {
                //        lock (_speakToken)
                //        {
                //            speaker.Speak("Actions.cs Completed Successfully");
                //        }
                //        return;
                //    }
                //}
                //else if (creationType == CreationType.OptionSets && Config.ExtensionConfig.CreateOneFilePerOptionSet)
                //{
                //    var tempPath = filePath;
                //    filePath = "OptionSet.cs";
                //    if (!File.Exists(tempPath))
                //    {
                //        lock (_speakToken)
                //        {
                //            speaker.Speak("OptionSet.cs Completed Successfully");
                //        }
                //        return;
                //    }
                //}
                //else 
                if (date != File.GetLastWriteTimeUtc(filePath) || consoleOutput.Contains(filePath + " was unchanged."))
                {
                    lock (_speakToken)
                    {
                        speaker.Speak(creationType + " Completed Successfully");
                    }
                    return;
                }
            }
            catch(Exception ex)
            {
                UpdateStatus("Error", ex.ToString());
                lock (_speakToken)
                {
                    speaker.Speak(creationType + " Errored");
                }
            }

            //int result;
            lock (_speakToken)
            {
                UpdateStatus("Error", "Output file was not updated or not found!  "  + filePath);
                speaker.Speak(creationType + " Errored");
            }
        }

        #region UpdateStatus

        public delegate void LogHandler(LogMessageInfo info);
        public event LogHandler OnLog;

        private void UpdateStatus(string message)
        {
            UpdateStatus(new LogMessageInfo(message));
        }

        // ReSharper disable once UnusedMember.Local
        private void UpdateStatus(string messageFormat, params object[] args)
        {
            UpdateStatus(new LogMessageInfo(messageFormat, args));
        }

        private void UpdateStatus(string summary, string detail)
        {
            UpdateStatus(new LogMessageInfo(summary, detail));
        }

        private void UpdateStatus(LogMessageInfo info)
        {
            if (OnLog != null)
            {
                OnLog(info);
            }
        }

        public class LogMessageInfo
        {
            public string Summary { get; set; }
            public string Detail { get; set; }

            public LogMessageInfo(string message) : this(message, message) { }
            public LogMessageInfo(string messageFormat, params object[] args) : this(string.Format(messageFormat, args)) { }

            public LogMessageInfo(string summary, string detail)
            {
                // Since the format and the string, string constructors have identical looking signatures, check to ensure that an "object[] args" wasn't intended
                var conditionalFormat = ConditionallyFormat(summary, detail);
                if (conditionalFormat != null)
                {
                    summary = conditionalFormat;
                    detail = conditionalFormat;
                }
                Summary = summary;
                Detail = detail;
            }

            private string ConditionallyFormat(string format, string value)
            {
                if (format.Contains("{0}"))
                {
                    return string.Format(format, value);
                }

                return null;
            }
        }

        #endregion // UpdateStatus
    }
}
