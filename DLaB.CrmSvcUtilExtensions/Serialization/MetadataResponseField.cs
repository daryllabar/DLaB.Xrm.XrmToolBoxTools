using System.Runtime.Serialization;
using Microsoft.Crm.Services.Utility;

namespace DLaB.CrmSvcUtilExtensions.Serialization
{
    [DataContract(Namespace = "http://DLaB.CrmSvcUtilExtensions")]
    public class MetadataResponseField
    {
        [DataMember]
        public int Index { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        // ReSharper disable once InconsistentNaming
        public string CLRFormatter { get; set; }
        [DataMember]
        public string Value { get; set; }

        public MetadataResponseField(SdkMessageResponseField field)
        {
            Index = field.Index;
            Name = field.Name;
            CLRFormatter = field.CLRFormatter;
            Value = field.Value;
        }

        public static implicit operator SdkMessageResponseField(MetadataResponseField field)
        {
            return new SdkMessageResponseField(field.Index, field.Name, field.CLRFormatter, field.Value);
        }
    }
}
