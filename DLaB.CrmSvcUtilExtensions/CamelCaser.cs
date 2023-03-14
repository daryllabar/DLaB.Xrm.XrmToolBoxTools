using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Crm.Sdk.Messages;
using Source.DLaB.Common;

namespace DLaB.ModelBuilderExtensions
{
    public static class CamelCaser
    {
        public static Lazy<Dictionary<int,HashSet<string>>> Dictionary = new Lazy<Dictionary<int, HashSet<string>>>(LoadDictionary);
        public static Lazy<List<string>> Overrides = new Lazy<List<string>>(LoadOverrides);
        private static readonly string Directory = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName ?? "C:\\") ?? "C:\\");
        private static int _maxWordLength = int.MaxValue;

        private static Dictionary<int, HashSet<string>> LoadDictionary()
        {
            var dict = new Dictionary<int, HashSet<string>>();
            foreach (var word in File.ReadLines(Path.Combine(Directory, "DLaB.Dictionary.txt")).Select(f => f.Trim().ToLower()))
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

            _maxWordLength = dict.Keys.Max();
            return dict;
        }

        private static List<string> LoadOverrides()
        {
            return Config.GetList("TokenCapitalizationOverrides", new List<string>())
                         .Select(t=>  t.Trim())
                         .OrderByDescending(t => t.Length).ToList();
        }

        public static string Case(string value, params string[] preferredEndings)
        {
            value = value.ToLower();
            preferredEndings = preferredEndings.Length == 0
                ? new[] {"Id"}
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

        private static string CaseInternal(string value)
        {
            foreach (var token in Overrides.Value)
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

        private static string CasePart(string part)
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
                if (Dictionary.Value.TryGetValue(length, out var hash))
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

        private static string CaseRemaining(string whole, string word)
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

        private static string Capitalize(string word)
        {
            return word.Length > 1
                ? word.First().ToString().ToUpper() + word.Substring(1)
                : word.ToUpper();
        }
    }
}
