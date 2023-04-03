using DLaB.Log;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Text.Json;
using DLaB.EarlyBoundGeneratorV2.Settings;

namespace DLaB.EarlyBoundGeneratorV2
{
    /// <summary>
    /// Main Logic Class shared between XTB and API version of EBG
    /// </summary>
    public class Logic
    {
        private readonly object _speakToken = new object();
        private EarlyBoundGeneratorConfig EarlyBoundGeneratorConfig { get; }
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
        public Logic(EarlyBoundGeneratorConfig earlyBoundGeneratorConfig)
        {
            EarlyBoundGeneratorConfig = earlyBoundGeneratorConfig;
        }

        /// <summary>
        /// Creates the entities, Option Sets and Messages.
        /// </summary>
        public bool Create(IOrganizationService service)
        {
            var currentOut = Console.Out;
            var logger = new LoggerTextWriter();
            Console.SetOut(logger);
            try
            {
                var parameters = GetParameters();
                var firstFile = Directory.GetFiles(parameters.OutDirectory).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(firstFile) && !AbleToMakeFileAccessible(firstFile))
                {
                    return false;
                }
                var runner = new ProcessModelInvoker(GetParameters(parameters));
                var result = runner.Invoke(service);
                if (result == 0)
                {
                    Speak("Early Bound Generation Completed Successfully");
                    return true;
                }
                else
                {
                    Logger.Show("An error occurred!", " An error when calling ProcessModelInvoker.Invoke.  Result: " + result);
                    Speak("Early Bound Generation Errored");
                    return false;
                }
            }
            finally
            {
                logger.FlushLogger();
                Console.SetOut(currentOut);
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
            return new ModelBuilderInvokeParameters
            {
                SettingsTemplateFile = EarlyBoundGeneratorConfig.SettingsTemplatePath,
                //CodeCustomizationService = "DLaB.ModelBuilderExtensions.Entity.CustomizeCodeDomService,DLaB.ModelBuilderExtensions",
                //CodeGenerationService = "DLaB.ModelBuilderExtensions.Entity.CustomCodeGenerationService,DLaB.ModelBuilderExtensions",
                //CodeWriterFilterService = "DLaB.ModelBuilderExtensions.Entity.CodeWriterFilterService,DLaB.ModelBuilderExtensions",
                //MetadataProviderService = "DLaB.ModelBuilderExtensions.Entity.MetadataProviderService,DLaB.ModelBuilderExtensions",
                SplitFilesByObject = true,
                OutDirectory = EarlyBoundGeneratorConfig.RootPath
            };
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
                        Logger.AddDetail("--" + kvp.Key);
                        lines.Add($"/{kvp.Key}");
                    }
                }
                else
                {
                    var kvpParameter = $"/{kvp.Key}:{kvp.Value}";
                    Logger.AddDetail($"--{kvp.Key} {kvp.Value}");
                    lines.Add(kvpParameter);
                }
            }

            Logger.AddDetail("Finished Generating ProcessModelInvoker Parameters.");
            var values = lines.OrderBy(v => v)
                .ToArray();

            Logger.AddDetail("Command line for Cloud generation:");
            Logger.AddDetail($"PAC modelbuilder build {string.Join(" ", values.Where(v => !v.Contains("splitfiles")).Select(l => l.Replace(" /", " --")))}");

            return values;
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


        public void UpdateBuilderSettingsJson()
        {
            UpdateBuilderSettingsJson(EarlyBoundGeneratorConfig, true);
        }

        private static void UpdateBuilderSettingsJson(EarlyBoundGeneratorConfig earlyBoundGeneratorConfig, bool allowRetry)
        {
            var path = GetJsonConfigPath(earlyBoundGeneratorConfig);

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
                                earlyBoundGeneratorConfig.ExtensionConfig.WriteDLaBModelBuilderProperties(writer, earlyBoundGeneratorConfig);
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
                File.WriteAllText(path, @"{}");
                UpdateBuilderSettingsJson(earlyBoundGeneratorConfig, false);
            }
        }

        private static string GetJsonConfigPath(EarlyBoundGeneratorConfig earlyBoundGeneratorConfig)
        {
            var path = Path.IsPathRooted(earlyBoundGeneratorConfig.ExtensionConfig.BuilderSettingsJsonRelativePath)
                ? earlyBoundGeneratorConfig.ExtensionConfig.BuilderSettingsJsonRelativePath
                : Path.Combine(earlyBoundGeneratorConfig.RootPath, earlyBoundGeneratorConfig.ExtensionConfig.BuilderSettingsJsonRelativePath);
            return path;
        }

        /// <summary>
        /// The Extensions to the Pac Model Builder will live in a different assembly.  To ensure that users can run against the PAC commandline, reset the namespace to the PAC version
        /// </summary>
        public void RevertEarlyBoundGeneratorAssemblyNaming()
        {
            var path = GetJsonConfigPath(EarlyBoundGeneratorConfig);

            var contents = EarlyBoundGeneratorConfig.ReplaceEarlyBoundAssemblyName(File.ReadAllText(path));

            File.WriteAllText(path, contents);
            
        }

        private void Speak(string words)
        {
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
