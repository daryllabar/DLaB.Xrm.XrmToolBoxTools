#if DLAB_UNROOT_NAMESPACE
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
        /// The Default Entry Seperator
        /// </summary>
        public const char EntrySeperator = '|';

        /// <summary>
        /// Gets or sets the Entry seperators.
        /// </summary>
        /// <value>
        /// The Entry seperators.
        /// </value>
        public char[] EntrySeperators { get; set; }

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
            EntrySeperators = new[] { EntrySeperator };
            ConvertValuesToLower = false;
        }

        internal T ParseValue<T>(string value)
        {
            return value == null ? default(T) : (ConvertValuesToLower ? value.ToLower() : value).ParseOrConvertString<T>();
        }
    }
}
