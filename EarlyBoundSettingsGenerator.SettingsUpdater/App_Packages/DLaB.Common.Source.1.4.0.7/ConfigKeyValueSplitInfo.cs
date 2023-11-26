#if DLAB_UNROOT_COMMON_NAMESPACE
namespace DLaB.Common
#else
namespace Source.DLaB.Common
#endif
{
    /// <summary>
    /// Settings used to control how a Key/Value config setting is parsed.  
    /// </summary>
#if DLAB_PUBLIC
    public class ConfigKeyValueSplitInfo : ConfigValuesSplitInfo
#else
    internal class ConfigKeyValueSplitInfo : ConfigValuesSplitInfo
#endif
    {
        internal new static ConfigKeyValueSplitInfo Default { get; } = new ConfigKeyValueSplitInfo();

        /// <summary>
        /// The Default Key/Value Seperator
        /// </summary>
        public const char KeyValueSeperator = ':';

        /// <summary>
        /// Gets or sets the key value seperators.
        /// </summary>
        /// <value>
        /// The key value seperators.
        /// </value>
        public char[] KeyValueSeperators { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [convert keys to lower].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [convert keys to lower]; otherwise, <c>false</c>.
        /// </value>
        public bool ConvertKeysToLower { get; set; }

        /// <summary>
        /// Defaults to splitting entries by "|", and Key/values by ":", lower casing the keys.<para />
        /// For Example:<para />
        /// - Entry1Key:Entry1Value|Entry2Key:Entry2Value|Entry3Key:Entry3Value|Entry4Key:Entry4Value
        /// </summary>
        public ConfigKeyValueSplitInfo()
        {
            KeyValueSeperators = new[] { KeyValueSeperator };
            ConvertKeysToLower = true;
        }

        internal T ParseKey<T>(string key)
        {
            return key == null ? default(T) : (ConvertKeysToLower ? key.ToLower() : key).ParseOrConvertString<T>();
        }
    }
}
