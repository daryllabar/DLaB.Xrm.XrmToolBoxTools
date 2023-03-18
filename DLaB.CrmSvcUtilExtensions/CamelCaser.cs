using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DLaB.ModelBuilderExtensions
{
    public class CamelCaser
    {
        private static CamelCaser _default;
        private Dictionary<int, HashSet<string>> Dictionary { get; }
        private List<string> Overrides { get; }
        private static Lazy<Dictionary<int,HashSet<string>>> LazyDictionary = new Lazy<Dictionary<int, HashSet<string>>>(LoadDictionary);
        private static Lazy<List<string>> LazyOverrides = new Lazy<List<string>>(LoadOverrides);
        private int _maxWordLength = int.MaxValue;

        public CamelCaser(Dictionary<int, HashSet<string>> dictionary, List<string> overrides = null)
        {
            Dictionary = dictionary;
            Overrides = overrides ?? new List<string>();
            _maxWordLength = dictionary.Keys.Max();
        }

        private static Dictionary<int, HashSet<string>> LoadDictionary()
        {
            var dict = new Dictionary<int, HashSet<string>>();
            var dictPath = ConfigHelper.Settings.DLaBModelBuilder.CamelCaseNamesDictionaryPath;
            if (!File.Exists(dictPath))
            {
                throw new FileNotFoundException("Camel Case Dictionary not found!", dictPath);
            }

            foreach (var word in File.ReadLines(dictPath).Select(f => f.Trim().ToLower()))
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
                        dict[length] = new HashSet<string>{ word };
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error processing word " + word, ex);
                }
            }

            return dict;
        }

        private static List<string> LoadOverrides()
        {
            return ConfigHelper.Settings.DLaBModelBuilder.TokenCapitalizationOverrides
                         .Select(t=>  t.Trim())
                         .OrderByDescending(t => t.Length).ToList();
        }

        public static string Case(string value, params string[] preferredEndings)
        {
            if (_default == null)
            {
                _default = new CamelCaser(LazyDictionary.Value, LazyOverrides.Value);
            }

            return _default.CaseWord(value, preferredEndings);
        }

        public string CaseWord(string value, params string[] preferredEndings)
        {
            value = value.ToLower();
            preferredEndings = preferredEndings.Length == 0
                ? new[] { "Id" }
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
            return option1.Count(char.IsUpper) < option2.Count(char.IsUpper)
                ? option1
                : option2;
        }

        private string CaseInternal(string value, bool parseForward)
        {
            foreach (var token in Overrides)
            {
                var index = value.IndexOf(token, StringComparison.InvariantCultureIgnoreCase);
                if (index >= 0)
                {
                    return CaseInternal(value.Substring(0, index), parseForward) + token + CaseInternal(value.Substring(index + token.Length), parseForward);
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

        private string CasePart(string part, bool parseForward)
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
            var currentLength = part.Length > _maxWordLength ? _maxWordLength : part.Length;
            for (var length = currentLength; length > 1; length--)
            {
                if (Dictionary.TryGetValue(length, out var hash))
                {
                    var word = parseForward
                        ? part.Substring(0, length)
                        : part.Substring(part.Length-length);
                    if (hash.Contains(word))
                    {
                        return parseForward
                            ? Capitalize(word) + CaseRemaining(part, word, parseForward)
                            : CaseRemaining(part, word, parseForward) + Capitalize(word);
                    }
                }
            }

            var nonWord = part[part.Length-1];
            return CasePart(part.Substring(0, part.Length-1), parseForward) + nonWord.ToString().ToUpper();
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
    }
}
