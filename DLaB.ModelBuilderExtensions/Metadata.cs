using System.Runtime.Serialization;
using DLaB.ModelBuilderExtensions.Serialization;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.Xrm.Sdk.Metadata;

namespace DLaB.ModelBuilderExtensions
{
    [DataContract(Namespace = "http://DLaB.ModelBuilderExtensions")]
    public class Metadata : IOrganizationMetadata, IExtensibleDataObject
    {
        [DataMember]
        public EntityMetadata[] Entities { get; set; }
        
        [DataMember]
        public OptionSetMetadataBase[] OptionSets { get; set; }

        private SdkMessages _messages;
        public SdkMessages Messages
        {
            get => _messages ?? (_messages = MetadataMessages);
            set
            {
                _messages = value;
                if (value != null) {
                    MetadataMessages = new MetadataMessages(value);
                }
            }
        }

        [DataMember]
        public MetadataMessages MetadataMessages { get; set; }

        public ExtensionDataObject ExtensionData { get; set; }
    }
}
