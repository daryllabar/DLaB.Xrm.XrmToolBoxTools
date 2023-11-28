using DLaB.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DLaB.ModelBuilderExtensions
{
    public class CamelCaser
    {
        private static CamelCaser _default;
        private Dictionary<int, HashSet<string>> Dictionary { get; }
        private List<string> Overrides { get; }
        private readonly int _maxWordLength;

        public CamelCaser(Dictionary<int, HashSet<string>> dictionary, List<string> overrides = null)
        {
            Dictionary = dictionary;
            Overrides = overrides ?? new List<string>();
            _maxWordLength = dictionary.Keys.Max();
        }

        public CamelCaser(params string[] words)
        {
            var dictionary = new Dictionary<int, HashSet<string>>();
            foreach (var word in words)
            {
                dictionary.AddOrAppend(word.Length, word);
            }
            Dictionary = dictionary;
            Overrides = new List<string>();
            _maxWordLength = dictionary.Keys.Max();
        }

        public CamelCaser(List<string> overrides = null, params string[] words): this(words)
        {
            Overrides = overrides ?? new List<string>();
        }

        private static Dictionary<int, HashSet<string>> LoadDictionary()
        {
            var dict = new Dictionary<int, HashSet<string>>();
            var dictPath = ConfigHelper.Settings.DLaBModelBuilder.CamelCaseNamesDictionaryPath;
            if (!File.Exists(dictPath))
            {
                throw new FileNotFoundException($"Camel Case Dictionary not found at {dictPath}!", dictPath);
            }

            foreach (var word in File.ReadLines(dictPath).Select(f => f.Trim().ToLower()))
            {
                AddWord(dict, word);
            }

            foreach (var word in ConfigHelper.Settings.DLaBModelBuilder.CamelCaseCustomWords)
            {
                AddWord(dict, word);
            }

            return dict;
        }

        private static void AddWord(Dictionary<int, HashSet<string>> dict, string word)
        {
            try
            {
                var length = word.Length;
                if (dict.TryGetValue(length, out var hash))
                {
                    hash.Add(word);
                }
                else
                {
                    dict[length] = new HashSet<string> { word };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error processing word " + word, ex);
            }
        }

        private static List<string> LoadOverrides()
        {
            return ConfigHelper.Settings.DLaBModelBuilder.TokenCapitalizationOverrides
                         .Select(t=> t.Trim())
                         .OrderByDescending(t => t.Length).ToList();
        }

        public static string Case(string value, params string[] preferredEndings)
        {
            if (_default == null)
            {
                _default = new CamelCaser(LoadDictionary(), LoadOverrides());
            }

            return _default.CaseWord(value, preferredEndings);
        }

        public static void ClearCache()
        {
            _default = null;
        }

        public string CaseWord(string value, params string[] preferredEndings)
        {
            value = value.ToLower();
            preferredEndings = preferredEndings.Length == 0
                ? new[] { "Guid", "Id" }
                : preferredEndings;

            var backward = CaseWord(value, preferredEndings, false);
            var forward = CaseWord(value, preferredEndings, true);

            return ChooseBest(backward, forward);
        }

        private string CaseWord(string value, string[] preferredEndings, bool parseForward)
        {
            foreach (var ending in preferredEndings)
            {
                if (value.EndsWith(ending.ToLower()))
                {
                    var tmp = CaseInternal(value.Substring(0, value.Length - ending.Length), parseForward);
                    value = CaseInternal(value, parseForward);
                    return tmp.Count(char.IsUpper) < value.Count(char.IsUpper)
                        ? tmp + ending
                        : value;
                }
            }

            return CaseInternal(value, parseForward);
        }

        private string ChooseBest(string option1, string option2)
        {
            var count1 = option1.Count(char.IsUpper);
            var count2 = option2.Count(char.IsUpper);

            if (count1 == count2)
            {
                return ParseForLongerWords(option1, option2);
            }

            return count1 < count2
                ? option1
                : option2;
        }

        private static readonly Dictionary<string, HashSet<string>> PreferredWords = new Dictionary<string, HashSet<string>>
        {
            { "For", new HashSet<string>(new[] { "Fort" })},
            { "In", new HashSet<string>(new[] { "Tin", "Bin", "Sin" })},
            { "Team", new HashSet<string>(new[] { "Steam" })},
            { "Sent", new HashSet<string>(new[] { "Ent" })},
            { "Set", new HashSet<string>(new[] { "Et" })},
            { "Side", new HashSet<string>(new[] { "Ide" })},
        };

        public string ParseForLongerWords(string option1, string option2)
        {
            var words1 = Regex.Matches(option1, @"([A-Z][a-z]+)").Cast<Match>().Select(m => m.Value).ToList();
            var words2 = Regex.Matches(option2, @"([A-Z][a-z]+)").Cast<Match>().Select(m => m.Value).ToList();

            // Check for single letter words
            if (words1.Count > words2.Count)
            {
                return option1;
            }

            if (words1.Count > words2.Count)
            {
                return option2;
            }

            for (var i = 0; i < words1.Count; i++)
            {
                if (PreferredWords.TryGetValue(words1[i], out var lessPreferred2) && lessPreferred2.Contains(words2[i]))
                {
                    return option1;
                }

                if (PreferredWords.TryGetValue(words2[i], out var lessPreferred1) && lessPreferred1.Contains(words1[i]))
                {
                    return option2;
                }
            }


            var allLengths1 = words1.GroupBy(x => x.Length).OrderByDescending(x => x.Key).ToList();
            var allLengths2 = words2.GroupBy(x => x.Length).OrderByDescending(x => x.Key).ToList();

            for (var i = 0; i < allLengths1.Count && i < allLengths2.Count; i++)
            {
                if (allLengths1[i].Key > allLengths2[i].Key)
                {
                    return option1;
                }
                if (allLengths1[i].Key < allLengths2[i].Key)
                {
                    return option2;
                } 
                
                if (allLengths1[i].Count() > allLengths2[i].Count())
                {
                    return option1;
                }
                
                if (allLengths2[i].Count() > allLengths1[i].Count())
                {
                    return option2;
                }
            }

            // Uncle?
            return option1;
        }

        private string CaseInternal(string value, bool parseForward)
        {
            foreach (var token in Overrides)
            {
                var index = value.IndexOf(token, StringComparison.InvariantCultureIgnoreCase);
                if (index >= 0)
                {
                    var start = index == 0
                        ? string.Empty
                        : CaseInternal(value.Substring(0, index), parseForward);
                    var end = index + token.Length == value.Length
                        ? string.Empty
                        : CaseInternal(value.Substring(index + token.Length), parseForward);
                    return start + token + end;
                }
            }

            // split by under score first
            if (value.Contains('_'))
            {
                var parts = value.Split('_');

                for (var i = 0; i < parts.Length; i++)
                {
                    parts[i] = CasePart(parts[i], parseForward);
                }

                return string.Join("_", parts);
            }

            return CasePart(value, parseForward);
        }

        private readonly Dictionary<string, string> _alreadyCasedBackward = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _alreadyCasedForward = new Dictionary<string, string>();

        private string CasePart(string part, bool parseForward)
        {
            if (parseForward && _alreadyCasedForward.TryGetValue(part, out var forward))
            {
                return forward;
            }

            if (!parseForward && _alreadyCasedBackward.TryGetValue(part, out var backward))
            {
                return backward;
            }

            var result = CasePartForCache(part, parseForward);
            (parseForward ? _alreadyCasedForward : _alreadyCasedBackward)[part] = result;
            return result;
        }

        private string CasePartForCache(string part, bool parseForward)
        {
            if (string.IsNullOrWhiteSpace(part))
            {
                return string.Empty;
            }

            if (part.Length == 1)
            {
                return Capitalize(part);
            }

            // split by word, biggest to littlest
            Fallback fallback = null;
            var currentLength = part.Length > _maxWordLength ? _maxWordLength : part.Length;
            for (var length = currentLength; length >= 1; length--)
            {
                if (Dictionary.TryGetValue(length, out var hash))
                {
                    var word = parseForward
                        ? part.Substring(0, length)
                        : part.Substring(part.Length - length);
                    if (hash.Contains(word))
                    {
                        // Check for a valid next word
                        // If not found, set as fallback value, try a smaller word
                        // If found, continue processing
                        var remaining = CaseRemaining(part, word, parseForward);
                        var index = FirstNonWordPartIndex(remaining);
                        if (index >= 0)
                        {
                            if (fallback == null
                                || index < fallback.Index)
                            {
                                fallback = new Fallback
                                {
                                    Index = index,
                                    Remaining = remaining,
                                    Word = word
                                };
                            }

                            continue;
                        }

                        return parseForward
                            ? Capitalize(word) + remaining
                            : remaining + Capitalize(word);
                    }
                }
            }

            if (fallback != null)
            {
                return parseForward
                    ? Capitalize(fallback.Word) + fallback.Remaining
                    : fallback.Remaining + Capitalize(fallback.Word);
            }

            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            if (parseForward)
            {
                return part[0].ToString().ToUpper() + CasePart(part.Substring(1), parseForward);
            }

            var nonWordIndex = part.Length - 1;
            var nonWord = part[nonWordIndex];
            return CasePart(part.Substring(0, nonWordIndex), parseForward) + nonWord.ToString().ToUpper();
            // ReSharper restore ConditionIsAlwaysTrueOrFalse
        }

        private int FirstNonWordPartIndex(string remaining)
        {
            for (var i = 0; i < remaining.Length - 1; i++)
            {
                if (char.IsUpper(remaining[i]) && char.IsUpper(remaining[i + 1]))
                {
                    return i;
                }
            }

            return -1;
        }

        private string CaseRemaining(string whole, string word, bool parseForward)
        {
            if (whole.Length == word.Length)
            {
                return string.Empty;
            }

            var remaining = parseForward
                ? whole.Substring(word.Length)
                : whole.Substring(0, whole.Length - word.Length);
            return remaining.Length < 2 
                ? Capitalize(remaining) 
                : CasePart(remaining, parseForward);
        }

        private string Capitalize(string word)
        {
            return word.Length > 1
                ? word.First().ToString().ToUpper() + word.Substring(1)
                : word.ToUpper();
        }

        private class Fallback
        {
            public int Index { get; set; }
            public string Remaining { get; set; }
            public string Word { get; set; }
        }
    }
}
