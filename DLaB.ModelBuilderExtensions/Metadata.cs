using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DLaB.ModelBuilderExtensions.Serialization;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.Xrm.Sdk.Metadata;

namespace DLaB.ModelBuilderExtensions
{
    [DataContract(Namespace = "http://DLaB.ModelBuilderExtensions")]
    public class Metadata : IOrganizationMetadata2, IExtensibleDataObject
    {
        private List<OptionSetMetadataBase> _optionSetList = new List<OptionSetMetadataBase>();
        private OptionSetMetadataBase[] _optionSets = Array.Empty<OptionSetMetadataBase>();

        [DataMember]
        public EntityMetadata[] Entities { get; set; }
        
        [DataMember]
        public OptionSetMetadataBase[] OptionSets {
            get
            {
                if (_optionSets.Length != _optionSetList.Count)
                {
                    _optionSets = _optionSetList.ToArray();
                }

                return _optionSets;
            }
            set
            {
                _optionSets = value;
                _optionSetList = _optionSets.ToList();
            }
        }

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

        public void AddOptionSetInfo(OptionSetMetadata optionSet)
        {
            if (_optionSetList.Any(a => a.Name.Equals(optionSet.Name)))
                return; // skipping. 
            _optionSetList.Add(optionSet);
        }
    }
}
