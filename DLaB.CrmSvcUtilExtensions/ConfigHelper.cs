using Source.DLaB.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace DLaB.ModelBuilderExtensions
{
    public class ConfigHelper
    {
        private static DLaBModelBuilderSettings _settings;

        public static DLaBModelBuilderSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    throw new Exception("ConfigHelper.Initialize has not been called!");
                }
                return _settings;
            }
        }

        private static IDictionary<string, string> _parameters;
        public static IDictionary<string, string> Parameters
        {
            get
            {
                if (_parameters == null)
                {
                    throw new Exception("ConfigHelper.Initialize has not been called!");
                }
                return _parameters;
            }
        }

        private static string _loadedSettingsPath;
        /// <summary>
        /// Required call to setup static Configuration.
        /// </summary>
        /// <param name="parameters">The parameter collection, containing a settingsTemplateFile key with a value to the Settings Template File</param>
        public static void Initialize(IDictionary<string, string> parameters)
        {
            _parameters = parameters;
            if (parameters.TryGetValue("settingsTemplateFile", out var settingsPath))
            {
                if (_settings == null || _loadedSettingsPath != settingsPath)
                {
                    _loadedSettingsPath = settingsPath;
                    _settings = settingsPath.ToUpper() == "DEFAULT"
                        ? new DLaBModelBuilderSettings()
                        : JsonSerializer.Deserialize<DLaBModelBuilderSettings>(File.ReadAllText(settingsPath)) ?? new DLaBModelBuilderSettings();

                    if (string.IsNullOrWhiteSpace(_settings.OutDirectory))
                    {
                        var outDirectory = GetParameter("outdirectory", "o");
                        if (string.IsNullOrWhiteSpace(outDirectory))
                        {
                            outDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                        }
                        _settings.OutDirectory = outDirectory;
                    }
                }
            }
            else
            {
                throw new Exception("No \"settingsTemplateFile\" found in the parameters collection!");
            }
        }

        /// <summary>
        /// Looks up the appSetting, parses the value by "|", then by "," into a Dictionary of String.  The key will always be lowercased.
        /// </summary>
        /// <param name="appSetting"></param>
        /// <param name="lowerCaseValues"></param>
        /// <returns></returns>
        internal static Dictionary<string, HashSet<string>> GetDictionaryHash(string appSetting, bool lowerCaseValues)
        {
            try
            {
                return Config.GetDictionaryHash<string, string>(appSetting, string.Empty,
                    new ConfigKeyValuesSplitInfo
                    {
                        ConvertValuesToLower = lowerCaseValues
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception attempting to GetDictionaryHash for config key: " + appSetting);
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Attempts to read the setting from the config file, and Parse to get the value.
        /// If the Type doesn't contain a Parse, a cast is attempted.
        /// Any failure in the Parse will throw an exception.
        /// If the config value is null, then the default value will be used.
        /// </summary>
        /// <typeparam name="T">The type to attempt to cast the config setting to</typeparam>
        /// <param name="appSetting">The AppSetting Key</param>
        /// <param name="defaultValue">The Value to return if the config file does not contain an entry for the appSetting</param>
        /// <returns></returns>
        public static T GetAppSettingOrDefault<T>(string appSetting, T defaultValue)
        {
            try
            {
                return Config.GetAppSettingOrDefault(appSetting, defaultValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception attempting to GetAppSettingOrDefault for config key: " + appSetting);
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public static string GetParameter(params string[] possibleNames)
        {
            foreach (var param in possibleNames)
            {
                if (Parameters.TryGetValue(param, out var value))
                {
                    return value;
                }
            }

            return null;
        }
    }
}
