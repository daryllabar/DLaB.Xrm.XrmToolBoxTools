using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using Microsoft.Xrm.Sdk;

namespace DLaB.ModelBuilderExtensions.OptionSet.Transliteration
{
    public class TransliterationService
    {
        private DLaBModelBuilder DLaBSettings { get; set; }
        public string TransliterationPath { get => DLaBSettings.TransliterationPath; set => DLaBSettings.TransliterationPath = value; }

        private readonly List<TransliterationAlphabet> Alphabets = new List<TransliterationAlphabet>();

        private HashSet<int> _availableCodes;
        public HashSet<int> AvailableCodes => _availableCodes ?? (_availableCodes = GetAvailableCodes());

        public TransliterationService(DLaBModelBuilder settings)
        {
            DLaBSettings = settings;
        }

        private HashSet<int> GetAvailableCodes()
        {
            if (!Directory.Exists(TransliterationPath))
            {
                return new HashSet<int>();
            }

            return new HashSet<int>(
                Directory.GetFiles(TransliterationPath)
                    .Select(Path.GetFileName)
                    .Select(x => x.Split('.')[0])
                    .Select(int.Parse));
        }

        public string Transliterate(LocalizedLabel label) { return Transliterate(label.LanguageCode, label.Label); }

        public string Transliterate(int languageCode, string label)
        {
            var alphabet = 
                Alphabets.SingleOrDefault(x => x.LanguageCode == languageCode) ?? LoadAlphabet(languageCode);

            return alphabet.Transliterate(label);
        }

        public bool HasCode(int languageCode) { return AvailableCodes.Contains(languageCode); }

        private TransliterationAlphabet LoadAlphabet(int languageCode)
        {
            var serializer = new DataContractJsonSerializer(typeof(AlphabetPoco));
            var path = Path.Combine(TransliterationPath, languageCode + ".json");
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

        private Stream GenerateStreamFromString(string s)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(s);
                    writer.Flush();
                    stream.Position = 0;
                    return stream;
                }
            }
        }
    }
}
