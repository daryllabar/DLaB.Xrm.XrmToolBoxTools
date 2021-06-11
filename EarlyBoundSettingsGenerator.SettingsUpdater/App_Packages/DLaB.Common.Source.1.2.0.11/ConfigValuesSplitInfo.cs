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
    /// Settings used to control how a values config setting is parsed.  
    /// </summary>
#if DLAB_PUBLIC
    public class ConfigValuesSplitInfo
#else
    internal class ConfigValuesSplitInfo
# endif
    {
        internal static ConfigValuesSplitInfo Default { get; } = new ConfigValuesSplitInfo();

        /// <summary>
        /// The Default Entry Separator
        /// </summary>
        public const char EntrySeparator = '|';

        /// <summary>
        /// Gets or sets the Entry separators.
        /// </summary>
        /// <value>
        /// The Entry separators.
        /// </value>
        public char[] EntrySeparators { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [convert values to lower].
        /// </summary>
        /// <value>
        /// <c>true</c> if [convert values to lower]; otherwise, <c>false</c>.
        /// </value>
        public bool ConvertValuesToLower { get; set; }

        /// <summary>
        /// Defaults to splitting entries by "|".<para />
        /// For Example:<para />
        /// - Value1Key:Value1Value|Value2Key:Value2Value|Value3Key:Value3Value|Value4Key:Value4Value
        /// </summary>
        public ConfigValuesSplitInfo()
        {
            EntrySeparators = new[] { EntrySeparator };
            ConvertValuesToLower = false;
        }

        internal T ParseValue<T>(string value)
        {
            return value == null ? default(T) : (ConvertValuesToLower ? value.ToLower() : value).ParseOrConvertString<T>();
        }
    }

    /// <summary>
    /// Extension Methods to parse Strings
    /// </summary>
#if DLAB_PUBLIC
    public static class ConfigValuesStringExtensions
#else
    internal static class ConfigValuesStringExtensions
#endif
    {
        /// <summary>
        /// Parses a string into a List of the given type.  Defaults to | as the separator
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static List<T> GetList<T>(this string value, ConfigValuesSplitInfo info = null)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return new List<T>();
            }
            info = info ?? ConfigValuesSplitInfo.Default;
            return new List<T>(value.Split(info.EntrySeparators, StringSplitOptions.RemoveEmptyEntries).Select(v => info.ParseValue<T>(v)));
        }

        /// <summary>
        /// Parses a string into a Dictionary of the given type.  Defaults to | as the separator and : as the key value separator
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="config"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> GetDictionary<TKey, TValue>(this string config, ConfigKeyValueSplitInfo info = null)
        {
            if (string.IsNullOrWhiteSpace(config))
            {
                return new Dictionary<TKey, TValue>();
            }
            info = info ?? ConfigKeyValueSplitInfo.Default;

            return config.Split(info.EntrySeparators, StringSplitOptions.RemoveEmptyEntries).
                Select(entry => entry.Split(info.KeyValueSeperators, StringSplitOptions.RemoveEmptyEntries)).
                ToDictionary(values => info.ParseKey<TKey>(values[0]),
                    values => info.ParseValue<TValue>(values.Length > 1 ? values[1] : null));
        }

        /// <summary>
        /// Parses a string into a Dictionary of the given type.  Defaults to | as the separator and : as the key value separator and , as the value separator
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="config"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static Dictionary<TKey, List<TValue>> GetDictionaryList<TKey, TValue>(this string config, ConfigKeyValuesSplitInfo info = null)
        {
            if (string.IsNullOrWhiteSpace(config))
            {
                return new Dictionary<TKey, List<TValue>>();
            }

            info = info ?? ConfigKeyValuesSplitInfo.Default;
            var dict = new Dictionary<TKey, List<TValue>>();
            foreach (var entry in config.Split(info.EntrySeparators, StringSplitOptions.RemoveEmptyEntries))
            {
                var entryValues = entry.Split(info.KeyValueSeperators, StringSplitOptions.RemoveEmptyEntries);
                var value = entryValues.Length > 1
                    ? entryValues[1].Split(info.EntryValuesSeparators, StringSplitOptions.RemoveEmptyEntries).Select(info.ParseValue<TValue>).ToList()
                    : new List<TValue>();
                dict.Add(info.ParseKey<TKey>(entryValues[0]), value);
            }

            return dict;
        }

        /// <summary>
        /// Parses a string into a Dictionary of the given type.  Defaults to | as the separator and : as the key value separator and , as the value separator
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="config"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static Dictionary<TKey, HashSet<TValue>> GetDictionaryHash<TKey, TValue>(this string config, ConfigKeyValuesSplitInfo info = null)
        {
            if (string.IsNullOrWhiteSpace(config))
            {
                return new Dictionary<TKey, HashSet<TValue>>();
            }

            info = info ?? ConfigKeyValuesSplitInfo.Default;
            var dict = new Dictionary<TKey, HashSet<TValue>>();
            foreach (var entry in config.Split(info.EntrySeparators, StringSplitOptions.RemoveEmptyEntries))
            {
                var entryValues = entry.Split(info.KeyValueSeperators, StringSplitOptions.RemoveEmptyEntries);
                var value = entryValues.Length > 1
                    ? new HashSet<TValue>(entryValues[1].Split(info.EntryValuesSeparators, StringSplitOptions.RemoveEmptyEntries).Select(info.ParseValue<TValue>))
                    : new HashSet<TValue>();
                dict.Add(info.ParseKey<TKey>(entryValues[0]), value);
            }

            return dict;
        }

        /// <summary>
        /// Parses a string into a HashSet of the given type.  Defaults to | as the separator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static HashSet<T> GetHashSet<T>(this string value, ConfigValuesSplitInfo info = null)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return new HashSet<T>();
            }
            if (info == null)
            {
                info = new ConfigValuesSplitInfo
                {
                    ConvertValuesToLower = true
                };
            }
            return new HashSet<T>(value.Split(info.EntrySeparators).Select(v => info.ParseValue<T>(v)));
        }
    }
}
