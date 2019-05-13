using System;
using System.Collections.Generic;
using System.Linq;

namespace DLaB.CrmSvcUtilExtensions
{
    public class WhitelistBlacklistLogic
    {
        public HashSet<string> WhiteList { get; }
        public List<string> WhitelistPrefixes { get; }
        public HashSet<string> BlackList { get; }
        public List<string> BlacklistPrefixes { get; }
        public bool WhitelistEnabled => WhiteList.Any() || WhitelistPrefixes.Any();

        public WhitelistBlacklistLogic(HashSet<string> whiteList, List<string> whitelistPrefixes, HashSet<string> blackList, List<string> blacklistPrefixes)
        {
            WhiteList = whiteList;
            WhitelistPrefixes = whitelistPrefixes;
            BlackList = blackList;
            BlacklistPrefixes = blacklistPrefixes;
        }

        public bool IsAllowed(string value)
        {
            return IsWhiteListed(value)
                   || !WhitelistEnabled
                   && !IsBlacklisted(value);
        }

        private bool IsWhiteListed(string value)
        {
            return WhiteList.Contains(value)
                   || WhitelistPrefixes.Any(preFix => value.StartsWith(preFix, StringComparison.InvariantCultureIgnoreCase));
        }

        private bool IsBlacklisted(string value)
        {
            return BlackList.Contains(value)
                   || BlacklistPrefixes.Any(preFix => value.StartsWith(preFix, StringComparison.InvariantCultureIgnoreCase));
        }

    }
}
