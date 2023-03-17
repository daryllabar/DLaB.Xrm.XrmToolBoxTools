using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;

namespace DLaB.ModelBuilderExtensions.Serialization
{
    [DataContract(Namespace = "http://DLaB.ModelBuilderExtensions")]
    public class MetadataMessages
    {
        [DataMember]
        public Dictionary<Guid, MetadataMessage> Messages { get; set; }

        public MetadataMessages(SdkMessages messages)
        {
            Messages = new Dictionary<Guid, MetadataMessage>();
            foreach (var message in messages.MessageCollection)
            {
                Messages[message.Key] = new MetadataMessage(message.Value);
            }
        }

        public static implicit operator SdkMessages(MetadataMessages messages)
        {
            var dict = new Dictionary<Guid, SdkMessage>();
            foreach (var message in messages.Messages)
            {
                dict[message.Key] = message.Value;
            }
            return new SdkMessages(dict);
        }
    }
}
