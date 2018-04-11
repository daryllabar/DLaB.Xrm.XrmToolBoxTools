using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using Microsoft.Xrm.Sdk;

namespace DLaB.CrmSvcUtilExtensions.OptionSet.Transliteration
{
    public static class TransliterationService
    {
        private static readonly string Path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "alphabets");

        private static readonly List<TransliterationAlphabet> Alphabets = new List<TransliterationAlphabet>();
        private static readonly Lazy<HashSet<int>> LazyAvailableCodes =
            new Lazy<HashSet<int>>(() => new HashSet<int>(
            Directory.GetFiles(Path)
            .Select(System.IO.Path.GetFileName)
            .Select(x => x.Split('.')[0])
            .Select(int.Parse)));

        public static HashSet<int> AvailableCodes { get; } = LazyAvailableCodes.Value;

        public static string Transliterate(LocalizedLabel label) { return Transliterate(label.LanguageCode, label.Label); }

        public static string Transliterate(int languageCode, string label)
        {
            var alphabet = 
                Alphabets.SingleOrDefault(x => x.LanguageCode == languageCode) ?? LoadAlphabet(languageCode);

            return alphabet.Transliterate(label);
        }

        public static bool HasCode(int languageCode) { return AvailableCodes.Contains(languageCode); }

        private static TransliterationAlphabet LoadAlphabet(int languageCode)
        {
            var serializer = new DataContractJsonSerializer(typeof(AlphabetPoco));
            var path = System.IO.Path.Combine(Path, languageCode + ".json");
            AlphabetPoco alphabetJson;
            using (var stream = GenerateStreamFromString(File.ReadAllText(path)))
            {
                alphabetJson = (AlphabetPoco)serializer.ReadObject(stream);
            }

            if (alphabetJson.alphabet.Any(a =>
                a == null ||
                a.Length < 2 ||
                string.IsNullOrWhiteSpace(a[0]) ||
                a[1] == null))
            {
                throw new Exception($"Error in format of Transliteration file {path}");    
            }

            var dictionary =
                alphabetJson.alphabet
                .ToDictionary(
                    x => x[0][0],
                    x => x[1]);

            var alphabet = new TransliterationAlphabet(languageCode, dictionary);

            Alphabets.Add(alphabet);

            return alphabet;
        }

        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
