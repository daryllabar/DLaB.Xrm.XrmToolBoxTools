using System.Runtime.Serialization;
using Microsoft.Crm.Services.Utility;

namespace DLaB.CrmSvcUtilExtensions.Serialization
{
    [DataContract(Namespace = "http://DLaB.CrmSvcUtilExtensions")]
    public class MetadataRequestField
    {
        [DataMember]
        public int Index { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        // ReSharper disable once InconsistentNaming
        public string CLRFormatter { get; set; }
        [DataMember]
        public bool IsOptional { get; set; }

        public MetadataRequestField(SdkMessageRequestField field)
        {
            Index = field.Index;
            Name = field.Name;
            CLRFormatter = field.CLRFormatter;
            IsOptional = field.IsOptional;
        }

        internal SdkMessageRequestField ToSdk(SdkMessageRequest sdk)
        {
            return new SdkMessageRequestField(sdk, Index, Name, CLRFormatter, IsOptional);
        }
    }
}
