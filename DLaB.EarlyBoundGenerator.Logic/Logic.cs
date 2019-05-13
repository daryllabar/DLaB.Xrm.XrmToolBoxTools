using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLaB.EarlyBoundGenerator.Settings;
using System.Speech.Synthesis;
using DLaB.Log;
using Source.DLaB.Common;

namespace DLaB.EarlyBoundGenerator
{
    public class Logic
    {
        private readonly object _updateAppConfigToken = new object();
        private readonly object _speakToken = new object();
        private EarlyBoundGeneratorConfig EarlyBoundGeneratorConfig { get; }
        private bool _configUpdated;
        private bool _useInteractiveMode;

        public Logic(EarlyBoundGeneratorConfig earlyBoundGeneratorConfig)
        {
            EarlyBoundGeneratorConfig = earlyBoundGeneratorConfig;
        }

        public void CreateActions()
        {
            Create(CreationType.Actions);
        }

        public void ExecuteAll()
        {
            if (EarlyBoundGeneratorConfig.SupportsActions)
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
            var filePath = GetOutputFilePath(EarlyBoundGeneratorConfig, creationType);
            // Check for file to be editable if not using TFS and creating only one file
            if (!EarlyBoundGeneratorConfig.ExtensionConfig.UseTfsToCheckoutFiles 
                && ((creationType == CreationType.Actions && !EarlyBoundGeneratorConfig.ExtensionConfig.CreateOneFilePerAction) || 
                    (creationType == CreationType.Entities && !EarlyBoundGeneratorConfig.ExtensionConfig.CreateOneFilePerEntity) ||
                    (creationType == CreationType.OptionSets && !EarlyBoundGeneratorConfig.ExtensionConfig.CreateOneFilePerOptionSet))
                && !AbleToMakeFileAccessible(filePath))
            {
                return;
            }

            var date = File.GetLastWriteTimeUtc(filePath);
            var p = new Process
            {
                StartInfo =
                {
                    FileName = Path.GetFullPath(EarlyBoundGeneratorConfig.CrmSvcUtilPath),
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    Arguments = GetConfigArguments(EarlyBoundGeneratorConfig, creationType),
                },
            };

            if (!File.Exists(p.StartInfo.FileName))
            {
                throw new FileNotFoundException("Unable to locate CrmSvcUtil at path '" + p.StartInfo.FileName +"'.  Update the CrmSvcUtilRelativePath in the DLaB.EarlyBoundGeneratorPlugin.Settings.xml file and try again.");
            }

            var args = GetSafeArgs(EarlyBoundGeneratorConfig, p);
            if (EarlyBoundGeneratorConfig.IncludeCommandLine)
            {
                switch (creationType)
                {
                    case CreationType.Actions:
                        EarlyBoundGeneratorConfig.ExtensionConfig.ActionCommandLineText = "\"" + p.StartInfo.FileName + "\" " + args;
                        break;
                    case CreationType.All:
                        break;
                    case CreationType.Entities:
                        EarlyBoundGeneratorConfig.ExtensionConfig.EntityCommandLineText = "\"" + p.StartInfo.FileName + "\" " + args;
                        break;
                    case CreationType.OptionSets:
                        EarlyBoundGeneratorConfig.ExtensionConfig.OptionSetCommandLineText = "\"" + p.StartInfo.FileName + "\" " + args;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(creationType));
                }
            }
            UpdateCrmSvcUtilConfig(EarlyBoundGeneratorConfig);
            Logger.Show("Shelling out to CrmSrvUtil for creating " + creationType, "Executing \"" + p.StartInfo.FileName + "\" " + args);
            p.Start();
            var consoleOutput = new StringBuilder();
            while (!p.StandardOutput.EndOfStream)
            {
                var line = p.StandardOutput.ReadLine();
                if (!string.IsNullOrWhiteSpace(line) && line.Contains("[****") && line.Contains("****]"))
                {
                    line = line.SubstringByString("[****", "****]");
                    Logger.Show(line);
                }
                else
                {
                    Logger.AddDetail(line);
                }
                consoleOutput.AppendLine(line);
            }

            HandleResult(filePath, date, creationType, consoleOutput.ToString(), EarlyBoundGeneratorConfig.AudibleCompletionNotification);
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
                Logger.AddDetail("Unable to set IsReadOnly Flag to false " + filePath + Environment.NewLine + ex);
                return false;
            }

            if (!File.GetAttributes(filePath).HasFlag(FileAttributes.ReadOnly)) { return true; }

            Logger.AddDetail("File \"" + filePath + "\" is read only, please checkout the file before running");
            return false;
        }

        private string GetOutputFilePath(EarlyBoundGeneratorConfig earlyBoundGeneratorConfig, CreationType creationType)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            var filePath = Path.Combine(EarlyBoundGeneratorConfig.RootPath, EarlyBoundGeneratorConfig.GetSettingValue(creationType, EarlyBoundGeneratorConfig.UserArgumentNames.Out));

            if (creationType == CreationType.Actions && earlyBoundGeneratorConfig.ExtensionConfig.CreateOneFilePerAction)
            {
                filePath = Path.Combine(filePath, "Actions.cs");
            }
            else if (creationType == CreationType.Entities && earlyBoundGeneratorConfig.ExtensionConfig.CreateOneFilePerEntity)
            {
                var entities = earlyBoundGeneratorConfig.ServiceContextName;

                if (string.IsNullOrWhiteSpace(entities))
                {
                    entities = "Entities";
                }

                filePath = Path.Combine(filePath, entities + ".cs");
            }
            else if (creationType == CreationType.OptionSets && earlyBoundGeneratorConfig.ExtensionConfig.CreateOneFilePerOptionSet)
            {
                filePath = Path.Combine(filePath, "OptionSets.cs");
            }

            return Path.GetFullPath(filePath);
        }

        private static string GetSafeArgs(EarlyBoundGeneratorConfig earlyBoundGeneratorConfig, Process p)
        {
            var args = p.StartInfo.Arguments;
            if (earlyBoundGeneratorConfig.MaskPassword && !string.IsNullOrWhiteSpace(earlyBoundGeneratorConfig.Password))
            {
                args = p.StartInfo.Arguments.Replace(earlyBoundGeneratorConfig.Password, new string('*', earlyBoundGeneratorConfig.Password.Length));
            }
            return args;
        }

        private void UpdateCrmSvcUtilConfig(EarlyBoundGeneratorConfig earlyBoundGeneratorConfig)
        {
            lock (_updateAppConfigToken)
            {
                if (_configUpdated) { return; }
                //load custom config file
                Configuration file;

                string filePath = Path.GetFullPath(earlyBoundGeneratorConfig.CrmSvcUtilPath) + ".config";
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
                        UpdateCrmSvcUtilConfig(earlyBoundGeneratorConfig);
                        return;
                    }
                    throw;
                }

                var extensions = earlyBoundGeneratorConfig.ExtensionConfig;
                if (UpdateConfigAppSetting(file, "ActionCommandLineText", extensions.ActionCommandLineText, true) |
                    UpdateConfigAppSetting(file, "ActionPrefixesWhitelist", extensions.ActionPrefixesWhitelist) |
                    UpdateConfigAppSetting(file, "ActionPrefixesToSkip", extensions.ActionPrefixesToSkip) |
                    UpdateConfigAppSetting(file, "ActionsWhitelist", extensions.ActionsWhitelist) |
                    UpdateConfigAppSetting(file, "ActionsToSkip", extensions.ActionsToSkip?.Replace("-","")) |
                    UpdateConfigAppSetting(file, "AddDebuggerNonUserCode", extensions.AddDebuggerNonUserCode.ToString()) |
                    UpdateConfigAppSetting(file, "AddNewFilesToProject", extensions.AddNewFilesToProject.ToString()) |
                    UpdateConfigAppSetting(file, "CreateOneFilePerAction", extensions.CreateOneFilePerAction.ToString()) |
                    UpdateConfigAppSetting(file, "CreateOneFilePerEntity", extensions.CreateOneFilePerEntity.ToString()) |
                    UpdateConfigAppSetting(file, "CreateOneFilePerOptionSet", extensions.CreateOneFilePerOptionSet.ToString()) |
                    UpdateConfigAppSetting(file, "EntityAttributeSpecifiedNames", extensions.EntityAttributeSpecifiedNames) |
                    UpdateConfigAppSetting(file, "EntityCommandLineText", extensions.EntityCommandLineText, true) |
                    UpdateConfigAppSetting(file, "EntitiesToSkip", extensions.EntitiesToSkip) |
                    UpdateConfigAppSetting(file, "EntitiesWhitelist", extensions.EntitiesWhitelist) |
                    UpdateConfigAppSetting(file, "EntityPrefixesToSkip", extensions.EntityPrefixesToSkip) |
                    UpdateConfigAppSetting(file, "EntityPrefixesWhitelist", extensions.EntityPrefixesWhitelist) |
                    UpdateConfigAppSetting(file, "GenerateActionAttributeNameConsts", extensions.GenerateActionAttributeNameConsts.ToString()) |
                    UpdateConfigAppSetting(file, "GenerateAttributeNameConsts", extensions.GenerateAttributeNameConsts.ToString()) |
                    UpdateConfigAppSetting(file, "GenerateAnonymousTypeConstructor", extensions.GenerateAnonymousTypeConstructor.ToString()) |
                    UpdateConfigAppSetting(file, "GenerateEntityRelationships", extensions.GenerateEntityRelationships.ToString()) |
                    UpdateConfigAppSetting(file, "GenerateEnumProperties", extensions.GenerateEnumProperties.ToString()) |
                    UpdateConfigAppSetting(file, "GenerateOnlyReferencedOptionSets", extensions.GenerateOnlyReferencedOptionSets.ToString()) |
                    UpdateConfigAppSetting(file, "InvalidCSharpNamePrefix", extensions.InvalidCSharpNamePrefix) |
                    UpdateConfigAppSetting(file, "MakeAllFieldsEditable", extensions.MakeAllFieldsEditable.ToString()) |
                    UpdateConfigAppSetting(file, "MakeReadonlyFieldsEditable", extensions.MakeReadonlyFieldsEditable.ToString()) |
                    UpdateConfigAppSetting(file, "MakeResponseActionsEditable", extensions.MakeResponseActionsEditable.ToString()) |
                    UpdateConfigAppSetting(file, "LocalOptionSetFormat", extensions.LocalOptionSetFormat) |
                    UpdateConfigAppSetting(file, "OptionSetPrefixesToSkip", extensions.OptionSetPrefixesToSkip) |
                    UpdateConfigAppSetting(file, "OptionSetsToSkip", extensions.OptionSetsToSkip) |
                    UpdateConfigAppSetting(file, "OptionSetCommandLineText", extensions.OptionSetCommandLineText, true) |
                    UpdateConfigAppSetting(file, "OptionSetLanguageCodeOverride", extensions.OptionSetLanguageCodeOverride?.ToString()) |
                    UpdateConfigAppSetting(file, "PropertyEnumMappings", extensions.PropertyEnumMappings) |
                    UpdateConfigAppSetting(file, "RemoveRuntimeVersionComment", extensions.RemoveRuntimeVersionComment.ToString()) |
                    UpdateConfigAppSetting(file, "UseDeprecatedOptionSetNaming", extensions.UseDeprecatedOptionSetNaming.ToString()) |
                    UpdateConfigAppSetting(file, "UnmappedProperties", extensions.UnmappedProperties) |
                    UpdateConfigAppSetting(file, "UseTfsToCheckoutFiles", extensions.UseTfsToCheckoutFiles.ToString()) |
                    UpdateConfigAppSetting(file, "UseXrmClient", extensions.UseXrmClient.ToString()))

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

        private string GetConfigArguments(EarlyBoundGeneratorConfig earlyBoundGeneratorConfig, CreationType type)
        {
            var sb = new StringBuilder();
            if (!earlyBoundGeneratorConfig.UseConnectionString)
            {
                sb.AppendFormat("/url:\"{0}\" ", earlyBoundGeneratorConfig.Url);
            }

            foreach (var argument in earlyBoundGeneratorConfig.CommandLineArguments.Where(a => a.SettingType == CreationType.All || a.SettingType == type))
            {
                var value = argument.Value;
                if (argument.Name == "out")
                {
                    value = GetOutputFilePath(earlyBoundGeneratorConfig, type);
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

            if (!string.IsNullOrWhiteSpace(earlyBoundGeneratorConfig.ConnectionString))
            {
                // If a connection string was specified ignore all other connection settings
                sb.AppendFormat("/connectionstring:\"{0}\" ", earlyBoundGeneratorConfig.ConnectionString.Replace("\"", "\"\""));
            }
            else if (!string.IsNullOrWhiteSpace(earlyBoundGeneratorConfig.Password))
            {
                if (earlyBoundGeneratorConfig.UseConnectionString)
                {
                    // Fix for https://github.com/daryllabar/DLaB.Xrm.XrmToolBoxTools/issues/14 - Problem with CRM 2016 on premises with ADFS
                    // CrmSvcUtil.exe /out:entities.cs / connectionstring:"Url=https://serverName.domain.com:444/orgName;Domain=myDomain;UserName=username;Password=*****"
                    // And this command doesn't work :
                    // CrmSvcUtil.exe /out:entitie.cs /url:"https://serverName.domain.com:444/orgName" / domain:"myDomain" / username:"username" / password:"*****"

                    var domain = string.Empty;
                    if (!string.IsNullOrWhiteSpace(earlyBoundGeneratorConfig.Domain))
                    {
                        domain = "Domain=" +earlyBoundGeneratorConfig.Domain + ";";
                    }
                    //var password = earlyBoundGeneratorConfig.Password.Replace("^", "^^").Replace("\"", "^\"").Replace("&", "^&");  // Handle Double Quotes and &s???
                    //To handle special characters, enclose in single quotes. If password contains single quotes, they must be doubled.
                    //https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlconnection.connectionstring.aspx
                    var password = $"'{earlyBoundGeneratorConfig.Password.Replace("'", "''")}'"; 
                    var builder = new System.Data.Common.DbConnectionStringBuilder
                    {
                        {"A", $"Url={earlyBoundGeneratorConfig.Url};{domain}UserName={earlyBoundGeneratorConfig.UserName};Password={password}"}
                    };
                    
                    sb.AppendFormat("/connectionstring:{0} ", builder.ConnectionString.Substring(2)); // Replace "A=" with "/connectionstring:"
                }
                else
                {
                    sb.AppendFormat("/username:\"{0}\" ", earlyBoundGeneratorConfig.UserName);
                    sb.AppendFormat("/password:\"{0}\" ", earlyBoundGeneratorConfig.Password);

                    // Add Login Info
                    if (!earlyBoundGeneratorConfig.UseCrmOnline && !string.IsNullOrWhiteSpace(earlyBoundGeneratorConfig.Domain))
                    {
                        sb.AppendFormat("/domain:\"{0}\" ", earlyBoundGeneratorConfig.Domain);
                    }
                }
            }

            if (_useInteractiveMode)
            {
                sb.Append("/interactivelogin:true ");
            }

            return sb.ToString();
        }

        public void CreateOptionSets()
        {
            Create(CreationType.OptionSets);
        }

        private void HandleResult(string filePath, DateTime date, CreationType creationType, string consoleOutput, bool speakResult)
        {
            try
            {
                if (consoleOutput.Contains("Unable to Login to Dynamics CRM"))
                {
                    Logger.Show("Unable to login.  Attempting to login using interactive mode.");
                    _useInteractiveMode = true;
                    Create(creationType);
                    return;
                }
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
                    Speak(creationType + " Completed Successfully", speakResult);
                    return;
                }
            }
            catch(Exception ex)
            {
                Logger.Show("Error", ex.ToString());
                Speak(creationType + " Errored", speakResult);
            }

            Logger.Show("Error", "Output file was not updated or not found!  " + filePath);

        }

        private void Speak(string words, bool speakResult)
        {
            if (!speakResult) { return; }
            var speaker = new SpeechSynthesizer();
            lock (_speakToken)
            {
                speaker.Speak(words);
            }
        }

        #region UpdateStatus

        //public delegate void LogHandler(LogMessageInfo info);
        //public event LogHandler OnLog;

        //private void UpdateDetail(string message)
        //{
        //    Update(new LogMessageInfo(null, message));
        //}

        //private void Update(string summary)
        //{
        //    Update(new LogMessageInfo(summary, summary));
        //}

        //private void Update(string summary, string detail)
        //{
        //    Update(new LogMessageInfo(summary, detail));
        //}

        //private void Update(LogMessageInfo info)
        //{
        //    OnLog?.Invoke(info);
        //}

        //public class LogMessageInfo
        //{
        //    public string ModalMessage { get; set; }
        //    public string Detail { get; set; }

        //    public LogMessageInfo(string modalMessage, string detail)
        //    {
        //        // Since the format and the string, string constructors have identical looking signatures, check to ensure that an "object[] args" wasn't intended
        //        var conditionalFormat = ConditionallyFormat(modalMessage, detail);
        //        if (conditionalFormat != null)
        //        {
        //            modalMessage = null;
        //            detail = conditionalFormat;
        //        }
        //        ModalMessage = modalMessage;
        //        Detail = detail;
        //    }

        //    private string ConditionallyFormat(string format, string value)
        //    {
        //        if (!string.IsNullOrWhiteSpace(format) && format.Contains("{0}"))
        //        {
        //            return string.Format(format, value);
        //        }

        //        return null;
        //    }
        //}

        //public class LogMessageInfoFormat : LogMessageInfo
        //{
        //    public LogMessageInfoFormat(string messageFormat, params object[] args) : base(null, string.Format(messageFormat, args)) { }
        //}

        #endregion // UpdateStatus
    }
}
