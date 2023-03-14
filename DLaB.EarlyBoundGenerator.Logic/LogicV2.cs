using DLaB.EarlyBoundGenerator.Settings;
using DLaB.Log;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Text.Json;

namespace DLaB.EarlyBoundGenerator
{
    /// <summary>
    /// Main Logic Class shared between XTB and API version of EBG
    /// </summary>
    public class LogicV2
    {
        private readonly object _speakToken = new object();
        private EarlyBoundGeneratorConfig EarlyBoundGeneratorConfig { get; }
        private bool _useInteractiveMode;
        private static readonly HashSet<string> ModelBuilderSwitches = new HashSet<string>(new[]
        {
            "emitfieldsclasses",
            "generateActions",
            "generateGlobalOptionSets",
            "emitfieldsclasses",
            "interactivelogin",
            "help",
            "legacyMode",
            "nologo",
            "splitfiles",
            "suppressGeneratedCodeAttribute",
            "suppressINotifyPattern",
            "writesettingsTemplateFile"
        });

        private static readonly HashSet<string> ModelBuilderParametersToSkip = new HashSet<string>(new[]
        {
            // Parameters that are in the template file
            "entitytypesfolder",
            "language",
            "messagestypesfolder",
            "optionsetstypesfolder",
            // Not Valid?
            "includeMessages"
        });

        /// <summary>
        /// Initializes a new Logic class.
        /// </summary>
        /// <param name="earlyBoundGeneratorConfig"></param>
        public LogicV2(EarlyBoundGeneratorConfig earlyBoundGeneratorConfig)
        {
            EarlyBoundGeneratorConfig = earlyBoundGeneratorConfig;
        }

        /// <summary>
        /// Creates the entities, and optionally global Option Sets and Messages.
        /// </summary>
        [Obsolete("Not Implemented", true)]
        public void Create()
        {
            
        }

        public void Create(IOrganizationService service)
        {
            var currentOut = Console.Out;
            var logger = new LoggerTextWriter();
            Console.SetOut(logger);
            var runner = new ProcessModelInvoker(GetParameters(GetParameters()));
            var result = runner.Invoke(service);
            logger.FlushLogger();
            Console.SetOut(currentOut);
            if (result != 0)
            {
                Logger.Show("An error occurred!", " An error when calling ProcessModelInvoker.Invoke.  Result: "  + result);
            }

            //var filePath = Path.Combine(EarlyBoundGeneratorConfig.RootPath, EarlyBoundGeneratorConfig.EntityOutPath,
            //    EarlyBoundGeneratorConfig.ServiceContextName + ".cs");
            //
            //// Check for file to be editable if not using TFS and creating only one file
            //if (!EarlyBoundGeneratorConfig.ExtensionConfig.UseTfsToCheckoutFiles 
            //    && !EarlyBoundGeneratorConfig.ExtensionConfig.CreateOneFilePerEntity
            //    && !AbleToMakeFileAccessible(filePath))
            //{
            //    return;
            //}
            //
            //var date = File.GetLastWriteTimeUtc(filePath);
            //
            //
            //
            //var args = GetSafeArgs(EarlyBoundGeneratorConfig, p);
            //if (EarlyBoundGeneratorConfig.IncludeCommandLine)
            //{
            //    switch (creationType)
            //    {
            //        case CreationType.Actions:
            //            EarlyBoundGeneratorConfig.ExtensionConfig.ActionCommandLineText = "\"" + p.StartInfo.FileName + "\" " + args;
            //            break;
            //        case CreationType.All:
            //            break;
            //        case CreationType.Entities:
            //            EarlyBoundGeneratorConfig.ExtensionConfig.EntityCommandLineText = "\"" + p.StartInfo.FileName + "\" " + args;
            //            break;
            //        case CreationType.OptionSets:
            //            EarlyBoundGeneratorConfig.ExtensionConfig.OptionSetCommandLineText = "\"" + p.StartInfo.FileName + "\" " + args;
            //            break;
            //        default:
            //            throw new ArgumentOutOfRangeException(nameof(creationType));
            //    }
            //}
            //UpdateBuilderSettingsJson(EarlyBoundGeneratorConfig);
            //Logger.Show("Shelling out to CrmSrvUtil for creating " + creationType, "Executing \"" + p.StartInfo.FileName + "\" " + args);
            //p.Start();
            //var consoleOutput = new StringBuilder();
            //while (!p.StandardOutput.EndOfStream)
            //{
            //    var line = p.StandardOutput.ReadLine();
            //    if (!string.IsNullOrWhiteSpace(line) && line.Contains("[****") && line.Contains("****]"))
            //    {
            //        line = line.SubstringByString("[****", "****]");
            //        Logger.Show(line);
            //    }
            //    else
            //    {
            //        Logger.AddDetail(line);
            //    }
            //    consoleOutput.AppendLine(line);
            //}
            //
            //HandleResult(filePath, date, creationType, consoleOutput.ToString(), EarlyBoundGeneratorConfig.AudibleCompletionNotification);
        }

        private ModelBuilderInvokeParameters GetParameters()
        {
            var parameters = new ModelBuilderInvokeParameters
            {
                SettingsTemplateFile = EarlyBoundGeneratorConfig.SettingsTemplatePath,
                //CodeCustomizationService = "DLaB.CrmSvcUtilExtensions.Entity.CustomizeCodeDomService,DLaB.CrmSvcUtilExtensions",
                //CodeGenerationService = "DLaB.CrmSvcUtilExtensions.Entity.CustomCodeGenerationService,DLaB.CrmSvcUtilExtensions",
                //CodeWriterFilterService = "DLaB.CrmSvcUtilExtensions.Entity.CodeWriterFilterService,DLaB.CrmSvcUtilExtensions",
                //MetadataProviderService = "DLaB.CrmSvcUtilExtensions.Entity.MetadataProviderService,DLaB.CrmSvcUtilExtensions",
                SplitFilesByObject = EarlyBoundGeneratorConfig.ExtensionConfig.GenerateSeparateFiles
            };

            if (parameters.SplitFilesByObject)
            {
                parameters.OutDirectory = EarlyBoundGeneratorConfig.RootPath;
            }
            else
            {
                parameters.OutputFile = Path.Combine(EarlyBoundGeneratorConfig.RootPath, EarlyBoundGeneratorConfig.ServiceContextName + ".cs");
            }

            return parameters;
        }

        public string[] GetParameters(ModelBuilderInvokeParameters parameters)
        {
            var lines = new List<string>();
            Logger.AddDetail("Generating ProcessModelInvoker Parameters:");
            foreach (var kvp in parameters.ToDictionary().Where(kvp => !string.IsNullOrWhiteSpace(kvp.Value)))
            {
                if (ModelBuilderParametersToSkip.Contains(kvp.Key))
                {
                    // skip...
                }
                else if (ModelBuilderSwitches.Contains(kvp.Key))
                {
                    if (bool.TryParse(kvp.Value, out var boolVal) && boolVal)
                    {
                        var @switch = $"/{kvp.Key}";
                        Logger.AddDetail("  " + @switch);
                        lines.Add(@switch);
                    }
                }
                else
                {
                    var kvpParameter = $"/{kvp.Key}:{kvp.Value}";
                    Logger.AddDetail("  " + kvpParameter);
                    lines.Add(kvpParameter);
                }
            }

            Logger.AddDetail("Finished Generating ProcessModelInvoker Parameters.");
            return lines.OrderBy(v => v)
                .ToArray();
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

        private static string GetSafeArgs(EarlyBoundGeneratorConfig earlyBoundGeneratorConfig, Process p)
        {
            var args = p.StartInfo.Arguments;
            if (earlyBoundGeneratorConfig.MaskPassword && !string.IsNullOrWhiteSpace(earlyBoundGeneratorConfig.Password))
            {
                args = p.StartInfo.Arguments.Replace(earlyBoundGeneratorConfig.Password, new string('*', earlyBoundGeneratorConfig.Password.Length));
            }
            if (earlyBoundGeneratorConfig.MaskPassword && !string.IsNullOrWhiteSpace(earlyBoundGeneratorConfig.ConnectionString))
            {
                var tmp = earlyBoundGeneratorConfig.ConnectionString;
                var start = tmp.IndexOf("Password", StringComparison.InvariantCultureIgnoreCase);
                if(start == -1)
                {
                    start = tmp.IndexOf("ClientSecret=", StringComparison.InvariantCultureIgnoreCase);
                }
                if (start == -1)
                {
                    // No Password present
                    return args;
                }
                var end = tmp.IndexOf("=", start, StringComparison.InvariantCultureIgnoreCase);
                start += end - start + 1;
                if (tmp.Length <= start + 2)
                {
                    return args;
                }

                end = tmp.IndexOf(tmp[start + 1] == '\'' ? '\'' : ';', start + 1);
                if (end == -1)
                {
                    end = tmp.Length;
                }
                args = args.Replace(tmp, tmp.Substring(0, start) + new string('*', end - start) + tmp.Substring(end));

            }
            return args;
        }

        public void UpdateBuilderSettingsJson()
        {
            UpdateBuilderSettingsJson(EarlyBoundGeneratorConfig, true);
        }

        private static void UpdateBuilderSettingsJson(EarlyBoundGeneratorConfig earlyBoundGeneratorConfig, bool allowRetry)
        {
            var path = Path.IsPathRooted(earlyBoundGeneratorConfig.ExtensionConfig.BuilderSettingsJsonRelativePath)
                ? earlyBoundGeneratorConfig.ExtensionConfig.BuilderSettingsJsonRelativePath
                : Path.Combine(earlyBoundGeneratorConfig.RootPath, earlyBoundGeneratorConfig.ExtensionConfig.BuilderSettingsJsonRelativePath);

            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                var dir = Path.GetDirectoryName(path);
                if (dir != null)
                {
                    Directory.CreateDirectory(dir);
                }
                // else is it rooted?  When does GetDirectoryName return null?
            }

            if (!File.Exists(path))
            {
                File.WriteAllText(path, @"{}");
            }

            try
            {
                using (var stream = File.Open(path, FileMode.Open, FileAccess.ReadWrite))
                using (var document = JsonDocument.Parse(stream))
                {
                    stream.SetLength(0);
                    using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
                           {
                               Indented = true
                           }))
                    {
                        var properties = document.RootElement.EnumerateObject().ToDictionary(k => k.Name);
                        properties["dLaB.ModelBuilder"] = new JsonProperty();
                        earlyBoundGeneratorConfig.PopulateBuilderProperties(properties);
                        earlyBoundGeneratorConfig.ExtensionConfig.PopulateBuilderProperties(properties);


                        writer.WriteStartObject();
                        foreach (var kvp in properties.OrderBy(p => p.Key))
                        {
                            if (kvp.Key == "dLaB.ModelBuilder")
                            {
                                writer.WritePropertyName(kvp.Key);
                                writer.WriteStartObject();
                                earlyBoundGeneratorConfig.ExtensionConfig.WriteDLaBModelBuilderProperties(writer);
                                writer.WriteEndObject();
                            }
                            else
                            {
                                kvp.Value.WriteTo(writer);
                            }
                        }

                        writer.WriteEndObject();
                    }
                }
            }
            catch (Exception ex)
            {
                if (!allowRetry || ex.Source != "System.Text.Json")
                {
                    throw new Exception($"Unable to update BuilderSettings Json at {path}!", ex);
                }

                File.Delete(path);
                File.WriteAllText(path, "{}");
                UpdateBuilderSettingsJson(earlyBoundGeneratorConfig, false);
            }
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
                    value = EarlyBoundGeneratorConfig.RootPath;
                }
                if (argument.Valueless)
                {
                    if (argument.Value == "true")
                    {
                        sb.AppendFormat("/{0} ", argument.Name);

                    }
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

        private void HandleResult(string filePath, DateTime date, CreationType creationType, string consoleOutput, bool speakResult)
        {
            try
            {
                if (consoleOutput.Contains("Unable to Login to Dynamics CRM"))
                {
                    Logger.Show("Unable to login.  Attempting to login using interactive mode.");
                    _useInteractiveMode = true;
                    //Create();
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
