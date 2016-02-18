using System;
using System.Collections.Generic;

namespace DLaB.CrmSvcUtilExtensions.OptionSet.Transliteration
{
    public class TransliterationAlphabet
    {
        public int LanguageCode { get; private set; }

        public Dictionary<char, string> Alphabet { get; private set; }

        public TransliterationAlphabet(int langageCode, Dictionary<char, string> transliterationDict)
        {
            this.LanguageCode = langageCode;

            this.Alphabet = transliterationDict;
        }

        /// <summary>
        /// Transliterate from Latin to specified aplhabet.
        /// </summary>
        /// <param name="text">Text to transliterate.</param>
        /// <returns>Returns transliterated text.</returns>
        public string Transliterate(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            var result = "";

            var chars = text.ToCharArray();
            
            foreach (var c in chars)
            {
                if (char.IsLower(c))
                {
                    result += this.Transliterate(c);
                }
                else
                {
                    char v=char.ToLower(c);
                    result += this.Transliterate(v).ToUpper();
                }
            }

            return result;
        }

        /// <summary>
        /// Transliterate one character into string.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private string Transliterate(char c)
        {
            string result;

            if (this.Alphabet.ContainsKey(c))
            {
                result = this.Alphabet[c];
            }
            else if (DefaultAlphabet.ContainsKey(c))
            {
                result = DefaultAlphabet[c];
            }
            else
            {
                result = "_";
            }
            return result;
        }

        /// <summary>
        /// Default English alphabet.
        /// </summary>
        private static readonly Dictionary<char, string> DefaultAlphabet
            = new Dictionary<char, string>
            {
                {'a',"a"},
                {'b',"b"},
                {'c',"c"},
                {'d',"d"},
                {'e',"e"},
                {'f',"f"},
                {'g',"g"},
                {'h',"h"},
                {'i',"i"},
                {'j',"j"},
                {'k',"k"},
                {'l',"l"},
                {'m',"m"},
                {'n',"n"},
                {'o',"o"},
                {'p',"p"},
                {'q',"q"},
                {'r',"r"},
                {'s',"s"},
                {'t',"t"},
                {'u',"u"},
                {'v',"v"},
                {'w',"w"},
                {'x',"x"},
                {'y',"y"},
                {'z',"z"},
                {'1',"1"},
                {'2',"2"},
                {'3',"3"},
                {'4',"4"},
                {'5',"5"},
                {'6',"6"},
                {'7',"7"},
                {'8',"8"},
                {'9',"9"},
                {'0',"0"},
                {'-', "-"}
            };
    }
}
