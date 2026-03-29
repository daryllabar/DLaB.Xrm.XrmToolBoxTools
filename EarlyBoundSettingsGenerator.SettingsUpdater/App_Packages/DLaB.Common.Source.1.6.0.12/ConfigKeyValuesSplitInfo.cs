using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if DLAB_UNROOT_COMMON_NAMESPACE
namespace DLaB.Common
#else
namespace Source.DLaB.Common
#endif
{
    /// <summary>
    /// Settings used to control how a Key/List&lt;Value&gt; config setting is parsed.  
    /// </summary>
#if DLAB_PUBLIC
    public class ConfigKeyValuesSplitInfo : ConfigKeyValueSplitInfo
#else
    internal class ConfigKeyValuesSplitInfo : ConfigKeyValueSplitInfo
#endif
    {
        internal new static ConfigKeyValuesSplitInfo Default { get; } = new ConfigKeyValuesSplitInfo();

        /// <summary>
        /// The Default Entry Values Separator
        /// </summary>
        public const char EntryValuesSeparator = ',';
        /// <summary>
        /// Gets or sets the entry values separators.
        /// </summary>
        /// <value>
        /// The entry values separators.
        /// </value>
        public char[] EntryValuesSeparators { get; set; }

        /// <summary>
        /// Defaults to splitting entries by "|", Key/values by ":", and values by ",", lower casing the keys.<para />
        /// For Example:<para />
        /// - Entry1Key:Entry1Value|Entry2Key:Entry2Value|Entry3Key:Entry3Value|Entry4Key:Entry4Value
        /// </summary>
        public ConfigKeyValuesSplitInfo()
        {
            EntryValuesSeparators = new[] { EntryValuesSeparator };
        }
    }
}
