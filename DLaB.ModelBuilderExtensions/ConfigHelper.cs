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
                if (_settings != null && _loadedSettingsPath == settingsPath)
                {
                    return;
                }

                _loadedSettingsPath = settingsPath;
                _settings = settingsPath.ToUpper() == "DEFAULT"
                    ? new DLaBModelBuilderSettings()
                    : JsonSerializer.Deserialize<DLaBModelBuilderSettings>(File.ReadAllText(settingsPath)) ?? new DLaBModelBuilderSettings();

                _settings.ProcessCustomFlags();

                if (!string.IsNullOrWhiteSpace(_settings.OutDirectory))
                {
                    return;
                }

                var outDirectory = parameters.GetFirstKey("outdirectory", "o");
                if (string.IsNullOrWhiteSpace(outDirectory))
                {
                    outDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                }
                _settings.OutDirectory = outDirectory;
            }
            else
            {
                throw new Exception("No \"settingsTemplateFile\" found in the parameters collection!");
            }
        }

        public static void ClearCache()
        {
            _parameters = null;
            _settings = null;
            _loadedSettingsPath = null;
        }
    }
}
