using System;
using System.Collections.Generic;
using System.Linq;

#if DLAB_UNROOT_COMMON_NAMESPACE
namespace DLaB.Common
#else
namespace Source.DLaB.Common
#endif
{
    /// <summary>
    /// Config Helper Class
    /// </summary>
#if DLAB_PUBLIC
    public class Config
#else
    internal class Config
#endif
    {
        #region Single Value GetAppSettings

        /// <summary>
        /// Attempts to read the setting from the config file, and Parse to get the value.
        /// If the Type doesn't contain a Parse, a cast is attempted.
        /// Any failure in the Parse will throw an exception.
        /// If the config value is null, then the default value will be used
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="appSetting"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetAppSettingOrDefault<T>(string appSetting, T defaultValue)
        {
            try
            {
                var config = ConfigProvider.Instance[appSetting];
                return config == null ? defaultValue : config.ParseOrConvertString<T>();
            }
            catch (Exception ex)
            {
                throw new FormatException($"Error occurred processing app setting \"{appSetting}\" with default value \"{defaultValue}\"", ex);
            }
        }

        /// <summary>
        /// Attempts to read the setting from the config file, and Parse to get the value.
        /// If the Type doesn't contain a Parse, a cast is attempted.
        /// Any failure in the Parse will throw an exception.
        /// If the config value is null, then the getDefault function will be used to retrieve the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="appSetting"></param>
        /// <param name="getDefault"></param>
        /// <returns></returns>
        public static T GetAppSettingOrDefault<T>(string appSetting, Func<T> getDefault)
        {
            try
            {
                var config = ConfigProvider.Instance[appSetting];
                return config == null ? getDefault() : config.ParseOrConvertString<T>();
            }
            catch (Exception ex)
            {
                throw new FormatException("Error occurred processing app setting " + appSetting, ex);
            }
        }

        /// <summary>
        /// Attempts to read the string setting from the config file, and convert it from a fraction to a decimal.
        /// Any failure in the Parse will throw an exception.
        /// If the config value is null, then the default value will be used
        /// </summary>
        /// <param name="appSetting"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal GetAppSettingFractionOrDefault(string appSetting, decimal defaultValue)
        {
            try
            {
                var config = ConfigProvider.Instance[appSetting];

                if (config == null)
                {
                    return defaultValue;
                }

                if (config.Contains('/'))
                {
                    var fraction = config.Split('/');
                    if (fraction.Length != 2)
                    {
                        throw new FormatException($"\"{config}\" was not in the expected format Numerator//Denominator");
                    }


                    if (int.TryParse(fraction[0], out var numerator) && int.TryParse(fraction[1], out var denominator))
                    {
                        if (denominator == 0)
                        {
                            throw new InvalidOperationException("Divide by 0 occurred");
                        }
                        return (decimal)numerator / denominator;
                    }
                }
                return defaultValue;
            }
            catch (Exception ex)
            {
                throw new FormatException($"Error occurred processing app setting \"{appSetting}\" with default value \"{defaultValue}\"", ex);
            }
        }

        /// <summary>
        /// Attempts to read the setting from the config file, and Parse to get the value.
        /// If the Type doesn't contain a Parse, a cast is attempted.
        /// Any failure in the Parse will throw an exception.
        /// If the config value is null, then the default value will be used.
        /// The setting should be in the format {Key}:{Value}|{Key}:{Value}
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="appSetting">The application setting.</param>
        /// <param name="key">AppSetting Key attribute value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="info">The settings by which to split the config value.</param>
        /// <returns></returns>
        public static T GetAppSettingOrDefaultByKey<T>(string appSetting, string key, T defaultValue, ConfigKeyValueSplitInfo info = null)
        {
            try
            {
                var config = ConfigProvider.Instance[appSetting];
                if (config == null)
                {
                    return defaultValue;
                }

                info = info ?? ConfigKeyValueSplitInfo.Default;

                // Find the Key:Value where the Key Matches
                config = config.Split(info.EntrySeparators).
                                FirstOrDefault(v => v.Split(info.KeyValueSeperators).
                                                      Select(k => info.ParseKey<string>(k)).
                                                      FirstOrDefault() == info.ParseKey<string>(key));
                return config == null ? defaultValue : SubstringByString(config, info.KeyValueSeperators.ToString()).ParseOrConvertString<T>();
            }
            catch (Exception ex)
            {
                throw new FormatException($"Error occurred processing app setting \"{appSetting}\" for key \"{key}\" with default value \"{defaultValue}\"", ex);
            }
        }

        /// <summary>
        /// Returns a the substring after the index of the first occurence of the startstring.
        /// Example: "012345678910".SubstringByString("2"); returns "345678910"
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="startString">The string that marks the start of the substring to be returned.</param>
        /// <returns></returns>
        private static string SubstringByString(string value, string startString)
        {
            var start = value.IndexOf(startString, StringComparison.Ordinal);
            return start < 0 ? null : value.Substring(start + startString.Length);
        }

        #endregion Single Value GetAppSettings

        #region GetList

        /// <summary>
        /// Attempts to read the setting from the config file and Parse to get the value.
        /// The value from the config if first split by the seperator
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="appSetting"></param>
        /// <param name="defaultValue"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static List<T> GetList<T>(string appSetting, List<T> defaultValue, ConfigValuesSplitInfo info = null)
        {
            try
            {
                var config = ConfigProvider.Instance[appSetting];
                return config == null ? defaultValue : config.GetList<T>(info);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Error occurred parsing list for app setting \"{appSetting}\"", ex);
            }
        }

        /// <summary>
        /// Attempts to read the setting from the config file and Parse to get the value.
        /// The value from the config if first split by the seperator
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="appSetting"></param>
        /// <param name="defaultValue"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static List<T> GetList<T>(string appSetting, string defaultValue, ConfigValuesSplitInfo info = null)
        {
            try
            {
                var config = ConfigProvider.Instance[appSetting] ?? defaultValue;
                return config.GetList<T>(info);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Error occurred parsing list for app setting \"{appSetting}\"", ex);
            }
        }

        /// <summary>
        /// Attempts to read the setting from the config file and Parse to get the value.
        /// The value from the config if first split by the seperator
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="appSetting"></param>
        /// <param name="getDefaultValue"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static List<T> GetList<T>(string appSetting, Func<List<T>> getDefaultValue, ConfigValuesSplitInfo info = null)
        {
            try
            {
                var config = ConfigProvider.Instance[appSetting];
                return config == null ? getDefaultValue() : config.GetList<T>(info);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Error occurred parsing list for app setting \"{appSetting}\"", ex);
            }
        }

        #endregion GetList

        #region GetDictionary

        /// <summary>
        /// Attempts to read the setting from the config file, and Parse into a Dictionary.
        /// If the Type doesn't contain a Parse, a cast is attempted.
        /// Any failure in the Parse will throw an exception.
        /// If the config value is null, then the default value will be used.
        /// The default setting should be in the format {Key}:{Value}|{Key}:{Value}
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="appSetting">The application setting.</param>
        /// <param name="getDefault">Function to get the default value.</param>
        /// <param name="info">The settings by which to split the config value.</param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> GetDictionary<TKey, TValue>(string appSetting,
                                                                           Func<Dictionary<TKey, TValue>> getDefault,
                                                                           ConfigKeyValueSplitInfo info = null)
        {
            try
            {
                var config = ConfigProvider.Instance[appSetting];
                return config == null ? getDefault() : config.GetDictionary<TKey, TValue>(info);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Error occurred parsing dictionary for app setting \"{appSetting}\"", ex);
            }
        }

        /// <summary>
        /// Attempts to read the setting from the config file, and Parse into a Dictionary.
        /// If the Type doesn't contain a Parse, a cast is attempted.
        /// Any failure in the Parse will throw an exception.
        /// If the config value is null, then the default value will be used.
        /// The default setting should be in the format {Key}:{Value}|{Key}:{Value}
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="appSetting">The application setting.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="info">The settings by which to split the config value.</param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> GetDictionary<TKey, TValue>(string appSetting, 
                                                                           Dictionary<TKey, TValue> defaultValue,
                                                                           ConfigKeyValueSplitInfo info = null)
        {
            try
            {
                var config = ConfigProvider.Instance[appSetting];
                return config == null ? defaultValue : config.GetDictionary<TKey, TValue>(info);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Error occurred parsing dictionary for app setting \"{appSetting}\"", ex);
            }
        }

        /// <summary>
        /// Attempts to read the setting from the config file, and Parse into a Dictionary.
        /// If the Type doesn't contain a Parse, a cast is attempted.
        /// Any failure in the Parse will throw an exception.
        /// If the config value is null, then the default value will be used.
        /// The default setting should be in the format {Key}:{Value}|{Key}:{Value}
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="appSetting">The application setting.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="info">The settings by which to split the config value.</param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> GetDictionary<TKey, TValue>(string appSetting,
                                                                           string defaultValue,
                                                                           ConfigKeyValueSplitInfo info = null)
        {
            try
            {
                var config = ConfigProvider.Instance[appSetting] ?? defaultValue;
                return config.GetDictionary<TKey, TValue>(info);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Error occurred parsing dictionary for app setting \"{appSetting}\", with defaultValue, \"{defaultValue}\"", ex);
            }
        }

        #endregion GetDictionary

        #region GetDictionaryList

        /// <summary>
        /// Attempts to read the setting from the config file, and Parse into a Dictionary.
        /// If the Type doesn't contain a Parse, a cast is attempted.
        /// Any failure in the Parse will throw an exception.
        /// If the config value is null, then the default value will be used.
        /// The default setting should be in the format {Key}:{Value1},{value2}|{Key}:{Value1}
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="appSetting">The application setting.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="info">The settings by which to split the config value.</param>
        /// <returns></returns>
        public static Dictionary<TKey, List<TValue>> GetDictionaryList<TKey, TValue>(string appSetting,
                                                                                     Dictionary<TKey, List<TValue>> defaultValue,
                                                                                     ConfigKeyValuesSplitInfo info = null)
        {
            try
            {
                var config = ConfigProvider.Instance[appSetting];
                return config == null ? defaultValue : config.GetDictionaryList<TKey, TValue>(info);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Error occurred parsing dictionary list for app setting \"{appSetting}\"", ex);
            }
        }

        /// <summary>
        /// Attempts to read the setting from the config file, and Parse into a Dictionary.
        /// If the Type doesn't contain a Parse, a cast is attempted.
        /// Any failure in the Parse will throw an exception.
        /// If the config value is null, then the default value will be used.
        /// The default setting should be in the format {Key}:{Value1},{value2}|{Key}:{Value1}
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="appSetting">The application setting.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="info">The settings by which to split the config value.</param>
        /// <returns></returns>
        public static Dictionary<TKey, List<TValue>> GetDictionaryList<TKey, TValue>(string appSetting,
                                                                                     string defaultValue,
                                                                                     ConfigKeyValuesSplitInfo info = null)
        {
            try
            {
                var config = ConfigProvider.Instance[appSetting] ?? defaultValue;
                return config.GetDictionaryList<TKey, TValue>(info);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Error occurred parsing dictionary list for app setting \"{appSetting}\", with default value \"{defaultValue}\"", ex);
            }
        }

        /// <summary>
        /// Attempts to read the setting from the config file, and Parse into a Dictionary.
        /// If the Type doesn't contain a Parse, a cast is attempted.
        /// Any failure in the Parse will throw an exception.
        /// If the config value is null, then the default value will be used.
        /// The default setting should be in the format {Key}:{Value1},{value2}|{Key}:{Value1}
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="appSetting">The application setting.</param>
        /// <param name="getDefault">Function to get the default value.</param>
        /// <param name="info">The settings by which to split the config value.</param>
        /// <returns></returns>
        public static Dictionary<TKey, List<TValue>> GetDictionaryList<TKey, TValue>(string appSetting,
                                                                                     Func<Dictionary<TKey, List<TValue>>> getDefault,
                                                                                     ConfigKeyValuesSplitInfo info = null)
        {
            try
            {
                var config = ConfigProvider.Instance[appSetting];
                return config == null ? getDefault() : config.GetDictionaryList<TKey, TValue>(info);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Error occurred parsing dictionary list for app setting \"{appSetting}\"", ex);
            }
        }

        #endregion GetDictionaryList

        #region GetDictionaryHash

        /// <summary>
        /// Attempts to read the setting from the config file, and Parse into a Dictionary.
        /// If the Type doesn't contain a Parse, a cast is attempted.
        /// Any failure in the Parse will throw an exception.
        /// If the config value is null, then the default value will be used.
        /// The default setting should be in the format {Key}:{Value1},{value2}|{Key}:{Value1}
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="appSetting">The application setting.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="info">The settings by which to split the config value.</param>
        /// <returns></returns>
        public static Dictionary<TKey, HashSet<TValue>> GetDictionaryHash<TKey, TValue>(string appSetting,
                                                                                     Dictionary<TKey, HashSet<TValue>> defaultValue,
                                                                                     ConfigKeyValuesSplitInfo info = null)
        {
            try
            {
                var config = ConfigProvider.Instance[appSetting];
                return config == null ? defaultValue : config.GetDictionaryHash<TKey, TValue>(info);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Error occurred parsing dictionary Hash for app setting \"{appSetting}\"", ex);
            }
        }

        /// <summary>
        /// Attempts to read the setting from the config file, and Parse into a Dictionary.
        /// If the Type doesn't contain a Parse, a cast is attempted.
        /// Any failure in the Parse will throw an exception.
        /// If the config value is null, then the default value will be used.
        /// The default setting should be in the format {Key}:{Value1},{value2}|{Key}:{Value1}
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="appSetting">The application setting.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="info">The settings by which to split the config value.</param>
        /// <returns></returns>
        public static Dictionary<TKey, HashSet<TValue>> GetDictionaryHash<TKey, TValue>(string appSetting,
                                                                                        string defaultValue,
                                                                                        ConfigKeyValuesSplitInfo info = null)
        {
            try
            {
                var config = ConfigProvider.Instance[appSetting] ?? defaultValue;
                return config.GetDictionaryHash<TKey, TValue>(info);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Error occurred parsing dictionary Hash for app setting \"{appSetting}\", with default value \"{defaultValue}\"", ex);
            }
        }

        /// <summary>
        /// Attempts to read the setting from the config file, and Parse into a Dictionary.
        /// If the Type doesn't contain a Parse, a cast is attempted.
        /// Any failure in the Parse will throw an exception.
        /// If the config value is null, then the default value will be used.
        /// The default setting should be in the format {Key}:{Value1},{value2}|{Key}:{Value1}
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="appSetting">The application setting.</param>
        /// <param name="getDefault">Function to get the default value.</param>
        /// <param name="info">The settings by which to split the config value.</param>
        /// <returns></returns>
        public static Dictionary<TKey, HashSet<TValue>> GetDictionaryHash<TKey, TValue>(string appSetting,
                                                                                        Func<Dictionary<TKey, HashSet<TValue>>> getDefault,
                                                                                        ConfigKeyValuesSplitInfo info = null)
        {
            try
            {
                var config = ConfigProvider.Instance[appSetting];
                return config == null ? getDefault() : config.GetDictionaryHash<TKey, TValue>(info);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Error occurred parsing dictionary Hash for app setting \"{appSetting}\"", ex);
            }
        }

        #endregion GetDictionaryHash

        #region GetHashSet

        /// <summary>
        /// Config Value must be in the format "Value|Value|Value" by Default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="appSetting">The application setting.</param>
        /// <param name="getDefault">The get default.</param>
        /// <param name="info">The information.</param>
        /// <returns></returns>
        /// <exception cref="System.FormatException"></exception>
        public static HashSet<T> GetHashSet<T>(string appSetting, Func<HashSet<T>> getDefault, ConfigValuesSplitInfo info = null)
        {
            try
            {
                var config = ConfigProvider.Instance[appSetting];
                return config != null ? config.GetHashSet<T>(info) : getDefault();
            }
            catch (Exception ex)
            {
                throw new FormatException($"Error occurred parsing hash for app setting \"{appSetting}\"", ex);
            }
        }

        /// <summary>
        /// Config Value must be in the format "Value|Value|Value" by Default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="appSetting"></param>
        /// <param name="defaultValue"></param>
        /// <param name="info">The settings by which to split the config value.</param>
        /// <returns></returns>
        public static HashSet<T> GetHashSet<T>(string appSetting, HashSet<T> defaultValue, ConfigValuesSplitInfo info = null)
        {
            try
            {
                var config = ConfigProvider.Instance[appSetting];
                return config == null ? defaultValue : config.GetHashSet<T>(info);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Error occurred parsing hash for app setting \"{appSetting}\"", ex);
            }
        }

        /// <summary>
        /// Config Value must be in the format "Value|Value|Value" by Default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="appSetting"></param>
        /// <param name="defaultValue"></param>
        /// <param name="info">The settings by which to split the config value.</param>
        /// <returns></returns>
        public static HashSet<T> GetHashSet<T>(string appSetting, string defaultValue, ConfigValuesSplitInfo info = null)
        {
            try
            {
                var config = ConfigProvider.Instance[appSetting] ?? defaultValue;
                return config.GetHashSet<T>(info);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Error occurred parsing hash for app setting \"{appSetting}\"", ex);
            }
        }

        #endregion GetHashSet

        #region ToString

        /// <summary>
        /// Returns a <see cref="string" /> that represents this given list to be stored as a string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="info">The information.</param>
        /// <param name="getString">The get string.</param>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public static string ToString<T>(IEnumerable<T> list, ConfigKeyValueSplitInfo info = null, Func<T, string> getString = null)
        {
            info = info ?? ConfigKeyValuesSplitInfo.Default;
            return string.Join(info.EntrySeparators.First().ToString(), ToStringIEnumerable(list, info, getString));
        }

        private static IEnumerable<string> ToStringIEnumerable<T>(IEnumerable<T> list, ConfigKeyValueSplitInfo info, Func<T, string> getString)
        {
            getString = getString ?? ToStringDefault;
            return list.Select(s => getString(s)).Select(s => info.ConvertValuesToLower ? s.ToLower() : s);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the given dictionary object to be stored as a string
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="info">The information.</param>
        /// <param name="getKeyString">The get key string.</param>
        /// <param name="getValueString">The get value string.</param>
        /// <returns>
        /// A <see cref="string" /> that represents the given dictionary object to be stored as a string
        /// </returns>
        public static string ToString<TKey, TValue>(Dictionary<TKey, TValue> dictionary, 
                                                    ConfigKeyValueSplitInfo info = null, 
                                                    Func<TKey,string> getKeyString = null,
                                                    Func<TValue, string> getValueString = null)
        {
            info = info ?? ConfigKeyValueSplitInfo.Default;
            return string.Join(info.EntrySeparators.First().ToString(), ToStringDictionary(dictionary, info, getKeyString, getValueString));
        }

        private static IEnumerable<string> ToStringDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary, ConfigKeyValueSplitInfo info, Func<TKey, string> getKeyString, Func<TValue, string> getValueString)
        {
            getKeyString = getKeyString ?? ToStringDefault;
            getValueString = getValueString ?? ToStringDefault;

            var values = from kvp in dictionary
                let key = getKeyString(kvp.Key)
                let value = getValueString(kvp.Value)
                select $"{(info.ConvertKeysToLower ? key.ToLower() : key)}{info.KeyValueSeperators.First()}{(info.ConvertValuesToLower ? value.ToLower() : value)}";
            return values;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the given dictionary object to be stored as a string
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="info">The information.</param>
        /// <param name="getKeyString">The get key string.</param>
        /// <param name="getValueString">The get value string.</param>
        /// <returns>
        /// A <see cref="string" /> that represents the given dictionary object to be stored as a string
        /// </returns>
        public static string ToString<TKey, TValue>(Dictionary<TKey, List<TValue>> dictionary,
                                            ConfigKeyValuesSplitInfo info = null,
                                            Func<TKey, string> getKeyString = null,
                                            Func<TValue, string> getValueString = null)
        {
            info = info ?? ConfigKeyValuesSplitInfo.Default;
            return string.Join(info.EntrySeparators.First().ToString(), ToStringDictionaryList(dictionary, info, getKeyString, getValueString));
        }

        private static List<string> ToStringDictionaryList<TKey, TValue>(Dictionary<TKey, List<TValue>> dictionary, ConfigKeyValuesSplitInfo info, Func<TKey, string> getKeyString, Func<TValue, string> getValueString)
        {
            getKeyString = getKeyString ?? ToStringDefault;
            getValueString = getValueString ?? ToStringDefault;

            var values = (from kvp in dictionary
                let key = getKeyString(kvp.Key)
                let prefix = (info.ConvertKeysToLower ? key.ToLower() : key) + info.KeyValueSeperators.First()
                let items = (kvp.Value ?? new List<TValue>()).Select(v => getValueString(v)).Select(v => info.ConvertValuesToLower ? v.ToLower() : v)
                select prefix + string.Join(info.EntryValuesSeparators.First().ToString(), items)).ToList();
            return values;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the given dictionary object to be stored as a string
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="info">The information.</param>
        /// <param name="getKeyString">The get key string.</param>
        /// <param name="getValueString">The get value string.</param>
        /// <returns>
        /// A <see cref="string" /> that represents the given dictionary object to be stored as a string
        /// </returns>
        public static string ToString<TKey, TValue>(Dictionary<TKey, HashSet<TValue>> dictionary,
                                            ConfigKeyValuesSplitInfo info = null,
                                            Func<TKey, string> getKeyString = null,
                                            Func<TValue, string> getValueString = null)
        {
            info = info ?? ConfigKeyValuesSplitInfo.Default;
            return string.Join(info.EntrySeparators.First().ToString(), ToStringDictionaryHashSet(dictionary, info, getKeyString, getValueString));
        }

        private static List<string> ToStringDictionaryHashSet<TKey, TValue>(Dictionary<TKey, HashSet<TValue>> dictionary, ConfigKeyValuesSplitInfo info, Func<TKey, string> getKeyString, Func<TValue, string> getValueString)
        {
            getKeyString = getKeyString ?? ToStringDefault;
            getValueString = getValueString ?? ToStringDefault;

            var values = (from kvp in dictionary
                let key = getKeyString(kvp.Key)
                let prefix = (info.ConvertKeysToLower ? key.ToLower() : key) + info.KeyValueSeperators.First()
                let items = kvp.Value.Select(v => getValueString(v)).Select(v => info.ConvertValuesToLower ? v.ToLower() : v)
                select prefix + string.Join(info.EntryValuesSeparators.First().ToString(), items)).ToList();
            return values;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this given hashset to be stored as a string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashset">The hashset.</param>
        /// <param name="info">The information.</param>
        /// <param name="getString">The get string.</param>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public static string ToString<T>(HashSet<T> hashset, ConfigKeyValueSplitInfo info = null, Func<T, string> getString = null)
        {
            info = info ?? ConfigKeyValuesSplitInfo.Default;
            return string.Join(info.EntrySeparators.First().ToString(), ToStringHashSet(hashset, info, getString));
        }

        private static IEnumerable<string> ToStringHashSet<T>(HashSet<T> hashset, ConfigKeyValueSplitInfo info, Func<T, string> getString)
        {
            getString = getString ?? ToStringDefault;
            var values = hashset.Select(s => getString(s)).Select(s => info.ConvertValuesToLower ? s.ToLower() : s);
            return values;
        }

        private static string ToStringDefault<T>(T value)
        {
            if (value != null)
            {
                return value.ToString();
            }
            var result = default(T);
            return result?.ToString();
        }

        #endregion ToString

        #region ToStringSorted

        /// <summary>
        /// Returns a <see cref="string" /> that represents this given list to be stored as a string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="info">The information.</param>
        /// <param name="getString">The get string.</param>
        /// <param name="sort">The sort function to apply</param>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public static string ToStringSorted<T>(IEnumerable<T> list, ConfigKeyValueSplitInfo info = null, Func<T, string> getString = null, Func<IEnumerable<string>, IOrderedEnumerable<string>> sort = null)
        {
            info = info ?? ConfigKeyValuesSplitInfo.Default;
            sort = sort ?? DefaultSort();
            return string.Join(info.EntrySeparators.First().ToString(), sort(ToStringIEnumerable(list, info, getString)));
        }

        private static Func<IEnumerable<string>, IOrderedEnumerable<string>> DefaultSort()
        {
            return l => l.OrderBy(v => v);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the given dictionary object to be stored as a string
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="info">The information.</param>
        /// <param name="getKeyString">The get key string.</param>
        /// <param name="getValueString">The get value string.</param>
        /// <param name="sort">The sort function to apply</param>
        /// <returns>
        /// A <see cref="string" /> that represents the given dictionary object to be stored as a string
        /// </returns>
        public static string ToStringSorted<TKey, TValue>(Dictionary<TKey, TValue> dictionary,
                                                    ConfigKeyValueSplitInfo info = null,
                                                    Func<TKey, string> getKeyString = null,
                                                    Func<TValue, string> getValueString = null,
                                                    Func<IEnumerable<string>, IOrderedEnumerable<string>> sort = null)
        {
            info = info ?? ConfigKeyValueSplitInfo.Default;
            sort = sort ?? DefaultSort();
            return string.Join(info.EntrySeparators.First().ToString(), sort(ToStringDictionary(dictionary, info, getKeyString, getValueString)));
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the given dictionary object to be stored as a string
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="info">The information.</param>
        /// <param name="getKeyString">The get key string.</param>
        /// <param name="getValueString">The get value string.</param>
        /// <param name="sort">The sort function to apply</param>
        /// <returns>
        /// A <see cref="string" /> that represents the given dictionary object to be stored as a string
        /// </returns>
        public static string ToStringSorted<TKey, TValue>(Dictionary<TKey, List<TValue>> dictionary,
                                            ConfigKeyValuesSplitInfo info = null,
                                            Func<TKey, string> getKeyString = null,
                                            Func<TValue, string> getValueString = null,
                                            Func<IEnumerable<string>, IOrderedEnumerable<string>> sort = null)
        {
            info = info ?? ConfigKeyValuesSplitInfo.Default;
            sort = sort ?? DefaultSort();
            return string.Join(info.EntrySeparators.First().ToString(), sort(ToStringDictionaryList(dictionary, info, getKeyString, getValueString)));
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the given dictionary object to be stored as a string
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="info">The information.</param>
        /// <param name="getKeyString">The get key string.</param>
        /// <param name="getValueString">The get value string.</param>
        /// <param name="sort">The sort function to apply</param>
        /// <returns>
        /// A <see cref="string" /> that represents the given dictionary object to be stored as a string
        /// </returns>
        public static string ToStringSorted<TKey, TValue>(Dictionary<TKey, HashSet<TValue>> dictionary,
                                            ConfigKeyValuesSplitInfo info = null,
                                            Func<TKey, string> getKeyString = null,
                                            Func<TValue, string> getValueString = null,
                                            Func<IEnumerable<string>, IOrderedEnumerable<string>> sort = null)
        {
            info = info ?? ConfigKeyValuesSplitInfo.Default;
            sort = sort ?? DefaultSort();
            return string.Join(info.EntrySeparators.First().ToString(), sort(ToStringDictionaryHashSet(dictionary, info, getKeyString, getValueString)));
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this given hashset to be stored as a string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashset">The hashset.</param>
        /// <param name="info">The information.</param>
        /// <param name="getString">The get string.</param>
        /// <param name="sort">The sort function to apply</param>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public static string ToStringSorted<T>(HashSet<T> hashset, ConfigKeyValueSplitInfo info = null, Func<T, string> getString = null, Func<IEnumerable<string>, IOrderedEnumerable<string>> sort = null)
        {
            info = info ?? ConfigKeyValuesSplitInfo.Default;
            sort = sort ?? DefaultSort();
            return string.Join(info.EntrySeparators.First().ToString(), sort(ToStringHashSet(hashset, info, getString)));
        }

        #endregion ToStringSorted
    }
}