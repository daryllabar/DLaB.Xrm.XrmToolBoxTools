using System.Collections.Generic;
using System.Runtime.Serialization;
// ReSharper disable InconsistentNaming

namespace DLaB.ModelBuilderExtensions.OptionSet.Transliteration
{
    public class AlphabetPoco
    {
        public int code { get; set; }
        public string language { get; set; }
        public List<string[]> alphabet { get; set; }
    }

}
