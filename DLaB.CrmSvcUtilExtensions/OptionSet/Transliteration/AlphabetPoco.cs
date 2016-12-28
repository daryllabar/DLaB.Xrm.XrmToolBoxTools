using System.Collections.Generic;
using System.Runtime.Serialization;
// ReSharper disable InconsistentNaming

namespace DLaB.CrmSvcUtilExtensions.OptionSet.Transliteration
{
    [DataContract]
    public class AlphabetPoco
    {
        [DataMember]
        public int code { get; set; }
        [DataMember]
        public string language { get; set; }
        [DataMember]
        public List<string[]> alphabet { get; set; }
    }

}
