using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DLaB.ModelBuilderExtensions
{
    public class BlacklistLogic
    {
        public HashSet<string> Blacklist { get; }
        public List<string> BlacklistPrefixes { get; }

        public BlacklistLogic(HashSet<string> blacklist, List<string> blacklistPrefixes)
        {
            Blacklist = blacklist;
            BlacklistPrefixes = blacklistPrefixes;
        }

        public bool IsAllowed(string value)
        {
            return !IsBlacklisted(value);
        }

        private bool IsBlacklisted(string value)
        {
            return Blacklist.Contains(value)
                   || BlacklistPrefixes.Any(pattern => Regex.Match(value, pattern).Success);
        }
    }
}
