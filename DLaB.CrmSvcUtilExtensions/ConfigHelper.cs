using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Remoting.Channels;

namespace DLaB.CrmSvcUtilExtensions
{
    public class ConfigHelper
    {
        /// <summary>
        /// Parses the value by "|", then by "," into a Dictionary of String.  The key will always be lowercased.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="lowerCaseValues"></param>
        /// <returns></returns>
        internal static Dictionary<string, List<string>> GetDictionaryList(string value, bool lowerCaseValues)
        {
            try
            {
                var dictionaryList = new Dictionary<string, List<string>>();
                if (string.IsNullOrWhiteSpace(value))
                {
                    return dictionaryList;
                }

                value = value.Replace(" ", string.Empty);

                foreach (var entity in value.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries))
                {
                    var splitAttributes = entity.Split(',');
                    var attributes = new List<string>();

                    for (var i = 1; i < splitAttributes.Length; i++)
                    {
                        if (lowerCaseValues)
                        {
                            attributes.Add(splitAttributes[i].ToLower());
                        }
                        else
                        {
                            attributes.Add(splitAttributes[i]);
                        }
                    }

                    dictionaryList.Add(splitAttributes[0].ToLower(), attributes);
                }
                return dictionaryList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception attempting to GetDictionaryList for config key: " + value);
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        internal static HashSet<string> GetHashSet(string appSettingKey)
        {
            try
            {
                var configValues = GetAppSettingOrDefault(appSettingKey, string.Empty).
                    Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries).
                    Select(s => s.ToLower()).ToList();

                if (configValues.Any())
                {
                    return new HashSet<string>(configValues);
                }
                return new HashSet<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception attempting to GetHashSet for config key: " + appSettingKey);
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
                T value;
                var config = ConfigurationManager.AppSettings[appSetting];
                if (config == null)
                {
                    value = defaultValue;
                }
                else
                {
                    var type = typeof (T);
                    var parse = type.GetMethod("Parse", new Type[] {typeof (string) });

                    if (parse == null)
                    {
                        value = (T) Convert.ChangeType(config, type);
                    }
                    else
                    {
                        value = (T) parse.Invoke(null, new Object[] {config});
                    }
                }
                return value;
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
