using System;
using System.Collections.Generic;
using Source.DLaB.Common;

namespace DLaB.CrmSvcUtilExtensions
{
    public class ConfigHelper
    {
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

        /// <summary>
        /// Attempts to read the setting from the config file, and Parse to get the value.
        /// If the Type doesn't contain a Parse, a cast is attempted.
        /// Any failure in the Parse will throw an exception.
        /// If the config value is null or a whitespace string, then the default value will be used.
        /// </summary>
        /// <typeparam name="T">The type to attempt to cast the config setting to</typeparam>
        /// <param name="appSetting">The AppSetting Key</param>
        /// <param name="defaultValue">The Value to return if the config file does not contain an entry for the appSetting</param>
        /// <returns></returns>
        public static T GetNonNullableAppSettingOrDefault<T>(string appSetting, T defaultValue)
        {
            try
            {
                var strValue = Config.GetAppSettingOrDefault(appSetting, "");
                return string.IsNullOrWhiteSpace(strValue)
                    ? defaultValue
                    : Config.GetAppSettingOrDefault(appSetting, defaultValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception attempting to GetAppSettingOrDefault for config key: " + appSetting);
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
