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

        public string CaseWord(string value, params string[] preferredEndings)
        {
            value = value.ToLower();
            preferredEndings = preferredEndings.Length == 0
                ? new[] { "Id" }
                : preferredEndings;
            foreach (var ending in preferredEndings)
            {
                if (value.EndsWith(ending.ToLower()))
                {
                    var tmp = CaseInternal(value.Substring(0, value.Length - ending.Length));
                    value = CaseInternal(value);
                    return tmp.Count(char.IsUpper) < value.Count(char.IsUpper)
                        ? tmp + ending
                        : value;
                }
            }
            return CaseInternal(value);
        }

        public static string Case(string value, params string[] preferredEndings)
        {
            if (_default == null)
            {
                _default = new CamelCaser(LazyDictionary.Value, LazyOverrides.Value);
            }

            return _default.CaseWord(value, preferredEndings);
        }

        private string CaseInternal(string value)
        {
            foreach (var token in Overrides)
            {
                var index = value.IndexOf(token, StringComparison.InvariantCultureIgnoreCase);
                if (index >= 0)
                {
                    return CaseInternal(value.Substring(0, index)) + token + CaseInternal(value.Substring(index + token.Length));
                }
            }

            // split by under score first
            if (value.Contains('_'))
            {
                var parts = value.Split('_');

                for (var i = 0; i < parts.Length; i++)
                {
                    parts[i] = CasePart(parts[i]);
                }

                return string.Join("_", parts);
            }

            return CasePart(value);
        }

        private string CasePart(string part)
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
                    var word = part.Substring(part.Length-length);
                    if (hash.Contains(word))
                    {
                        return CaseRemaining(part, word) + Capitalize(word);
                    }
                }
            }

            var nonWord = part[part.Length-1];
            return CasePart(part.Substring(0, part.Length-1)) + nonWord.ToString().ToUpper();
        }

        private string CaseRemaining(string whole, string word)
        {
            if (whole.Length == word.Length)
            {
                return string.Empty;
            }

            var remaining = whole.Substring(0, whole.Length - word.Length);
            return remaining.Length < 2 
                ? Capitalize(remaining) 
                : CasePart(remaining);
        }

        private string Capitalize(string word)
        {
            return word.Length > 1
                ? word.First().ToString().ToUpper() + word.Substring(1)
                : word.ToUpper();
        }
    }
}
