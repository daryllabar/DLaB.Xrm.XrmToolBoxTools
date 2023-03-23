using System;
using System.Collections.Generic;
using System.Linq;

namespace DLaB.ModelBuilderExtensions
{
    public class WhitelistBlacklistLogic
    {
        public HashSet<string> BlackList { get; }
        public List<string> BlacklistPrefixes { get; }
        public bool WhiteListEnabled { get; }

        public WhitelistBlacklistLogic(bool whiteListEnabled, HashSet<string> blackList, List<string> blacklistPrefixes)
        {
            WhiteListEnabled = whiteListEnabled;
            BlackList = blackList;
            BlacklistPrefixes = blacklistPrefixes;
        }

        public bool IsAllowed(string value)
        {
            return WhiteListEnabled
                   || !IsBlacklisted(value);
        }

        private bool IsBlacklisted(string value)
        {
            return BlackList.Contains(value)
                   || BlacklistPrefixes.Any(preFix => value.StartsWith(preFix, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
