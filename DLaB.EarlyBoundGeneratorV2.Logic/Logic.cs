using DLaB.EarlyBoundGeneratorV2.Settings;
using DLaB.Log;
using DLaB.ModelBuilderExtensions;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.Xrm.Sdk;
using System;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Text.Json;

namespace DLaB.EarlyBoundGeneratorV2
{
    /// <summary>
    /// Main Logic Class shared between XTB and API version of EBG
    /// </summary>
    public class Logic
    {
        private readonly object _speakToken = new object();
        private EarlyBoundGeneratorConfig EarlyBoundGeneratorConfig { get; }

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
            var initialLogLevel = Logger.Instance.LogLevel;
            Console.SetOut(logger);
            try
            {
                var parameterBuilder = new ArgumentBuilder(EarlyBoundGeneratorConfig.SettingsTemplatePath, EarlyBoundGeneratorConfig.RootPath, Logger.AddDetail);
                var firstFile = Directory.GetFiles(parameterBuilder.OutputPath).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(firstFile) && !AbleToMakeFileAccessible(firstFile))
                {
                    return false;
                }

                var logFilePath = ErrorLogger.GetLogPath(EarlyBoundGeneratorConfig.ExtensionConfig.XrmToolBoxPluginPath);
                if (File.Exists(logFilePath))
                {
                    File.Delete(logFilePath);
                }

                if (int.TryParse(EarlyBoundGeneratorConfig.ExtensionConfig.ModelBuilderLogLevel, out var logLevel))
                {
                    Logger.Instance.LogLevel = (LogLevel)logLevel;
                }
                var runner = new ModelBuilder(Logger.Instance);
                runner.Parameters.LoadArguments(parameterBuilder.GetArguments());
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
                Logger.Instance.LogLevel = initialLogLevel;
            }
        }

        /// <summary>
        /// Checks that this given path is valid and accessible
        /// </summary>
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
            var path = earlyBoundGeneratorConfig.SettingsTemplatePath;

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

        /// <summary>
        /// The Extensions to the Pac Model Builder will live in a different assembly.  To ensure that users can run against the PAC commandline, reset the namespace to the PAC version
        /// </summary>
        public void RemoveXrmToolBoxPluginPath()
        {
            var path = EarlyBoundGeneratorConfig.SettingsTemplatePath;

            var contents = File.ReadAllLines(path);

            File.WriteAllLines(path, contents.Where(l => !l.TrimStart().StartsWith("\"xrmToolBoxPluginPath\"")));
        }

        private void Speak(string words)
        {
            if (!EarlyBoundGeneratorConfig.AudibleCompletionNotification)
            {
                return;
            }
            var speaker = new SpeechSynthesizer();
            lock (_speakToken)
            {
                speaker.Speak(words);
            }
        }
    }
}
