using System.Collections.Generic;
using System.Linq;

namespace DLaB.ModelBuilderExtensions
{
    public class TextMatcher
    {
        private readonly string[] _matchesToSearchFor;

        public TextMatcher(IEnumerable<string> matchesToSearchFor)
        {
            _matchesToSearchFor = matchesToSearchFor.Select(t => "^" + System.Text.RegularExpressions.Regex.Escape(t).Replace("\\*", ".*") + "$").ToArray();
        }

        public bool HasMatch(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            foreach(var match in _matchesToSearchFor)
            {
                if(System.Text.RegularExpressions.Regex.IsMatch(value, match, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
