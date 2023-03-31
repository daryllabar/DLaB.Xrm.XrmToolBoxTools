using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DLaB.ModelBuilderExtensions
{
    public class BlacklistLogic
    {
        public HashSet<string> BlackList { get; }
        public List<string> BlacklistPrefixes { get; }

        public BlacklistLogic(HashSet<string> blackList, List<string> blacklistPrefixes)
        {
            BlackList = blackList;
            BlacklistPrefixes = blacklistPrefixes;
        }

        public bool IsAllowed(string value)
        {
            return !IsBlacklisted(value);
        }

        private bool IsBlacklisted(string value)
        {
            return BlackList.Contains(value)
                   || BlacklistPrefixes.Any(pattern => Regex.Match(value, pattern).Success);
        }
    }
}
