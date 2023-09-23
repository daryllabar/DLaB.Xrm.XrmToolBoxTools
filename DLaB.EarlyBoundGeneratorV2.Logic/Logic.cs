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
using DLaB.ModelBuilderExtensions;
using Microsoft.Extensions.Logging;

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

                var logFilePath = ErrorLogger.GetLogPath(EarlyBoundGeneratorConfig.ExtensionConfig.XrmToolBoxPluginPath);
                if (File.Exists(logFilePath))
                {
                    File.Delete(logFilePath);
                }

                Logger.Instance.LogLevel = LogLevel.Information;
                var runner = new ModelBuilder(Logger.Instance);
                runner.Parameters.LoadArguments(GetParameters(parameters));
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

                    if (File.Exists(logFilePath))
                    {
                        Logger.AddDetail("Log File:");
                        Logger.AddDetail(File.ReadAllText(logFilePath));
                    }

                    return false;
                }
            }
            finally
            {
                logger.FlushLogger();
                Console.SetOut(currentOut);
            }
        }

        private ModelBuilderInvokeParameters GetParameters()
        {
            return new ModelBuilderInvokeParameters(new ModeBuilderLoggerService("DateModelBuilderTests"))
            {
                SettingsTemplateFile = EarlyBoundGeneratorConfig.SettingsTemplatePath,
                SplitFilesByObject = true,
                OutDirectory = EarlyBoundGeneratorConfig.RootPath
            };
        }

        public string[] GetParameters(ModelBuilderInvokeParameters parameters)
        {
            var lines = new List<string>();
            var commandLine = new List<string>();
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
                        var flag = "--" + kvp.Key;
                        commandLine.Add(flag);
                        Logger.AddDetail(flag);
                        lines.Add($"/{kvp.Key}");
                    }
                }
                else
                {
                    var kvpParameter = $"--{kvp.Key} {kvp.Value}";
                    commandLine.Add(kvpParameter);
                    Logger.AddDetail(kvpParameter);
                    lines.Add($"/{kvp.Key}:{kvp.Value}");
                }
            }

            Logger.AddDetail("Finished Generating ProcessModelInvoker Parameters.");
            var values = lines.OrderBy(v => v).ToArray();

            Logger.AddDetail("Command line for Cloud generation:");
            Logger.AddDetail($"PAC modelbuilder build {string.Join(" ", commandLine.Where(v => !v.Contains("splitfiles")))}");

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
                        earlyBoundGeneratorConfig.ExtensionConfig.PopulateBuilderProperties(properties, earlyBoundGeneratorConfig.GenerateMessages);


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
        public void RemoveXrmToolBoxPluginPath()
        {
            var path = GetJsonConfigPath(EarlyBoundGeneratorConfig);

            var contents = File.ReadAllLines(path);

            File.WriteAllLines(path, contents.Where(l => !l.TrimStart().StartsWith("\"xrmToolBoxPluginPath\"")));
        }

        private void Speak(string words)
        {
            var speaker = new SpeechSynthesizer();
            lock (_speakToken)
            {
                speaker.Speak(words);
            }
        }
    }
}
