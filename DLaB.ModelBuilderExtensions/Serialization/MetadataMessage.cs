using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;

namespace DLaB.ModelBuilderExtensions.Serialization
{
    [DataContract(Namespace = "http://DLaB.ModelBuilderExtensions")]
    public class MetadataMessage
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public bool IsPrivate { get; set; }
        [DataMember]
        public bool IsCustomAction { get; set; }
        [DataMember]
        public Dictionary<Guid, MetadataPair> MetadataPairs { get; set; }
        [DataMember]
        public Dictionary<Guid, MetadataFilter> MetadataFilters { get; set; }

        public MetadataMessage(SdkMessage message)
        {
            Name = message.Name;
            Id = message.Id;
            IsPrivate = message.IsPrivate;
            IsCustomAction = message.IsCustomAction;
            MetadataPairs = new Dictionary<Guid, MetadataPair>();
            foreach (var pair in message.SdkMessagePairs)
            {
                MetadataPairs[pair.Key] = new MetadataPair(pair.Value);
            }
            MetadataFilters = new Dictionary<Guid, MetadataFilter>();
            foreach (var filter in message.SdkMessageFilters)
            {
                MetadataFilters[filter.Key] = new MetadataFilter(filter.Value);
            }
        }

        public static implicit operator SdkMessage(MetadataMessage message)
        {
            var sdk = new SdkMessage(message.Id, message.Name, message.IsPrivate);
            var isCustomField = typeof(SdkMessage).GetField("_isCustomAction", BindingFlags.Instance | BindingFlags.NonPublic);
            isCustomField?.SetValue(sdk, message.IsCustomAction);

            foreach (var filter in message.MetadataFilters)
            {
                sdk.SdkMessageFilters[filter.Key] = filter.Value;
            }
            foreach (var pair in message.MetadataPairs)
            {
                sdk.SdkMessagePairs[pair.Key] = pair.Value.ToSdk(sdk);
            }

            return sdk;
        }
    }

}
