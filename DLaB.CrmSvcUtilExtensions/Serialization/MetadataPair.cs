using System;
using System.Runtime.Serialization;
using Microsoft.Crm.Services.Utility;

namespace DLaB.CrmSvcUtilExtensions.Serialization
{
    [DataContract(Namespace = "http://DLaB.CrmSvcUtilExtensions")]
    public class MetadataPair
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string MessageNamespace { get; set; }
        [DataMember]
        public MetadataRequest Request { get; set; }
        [DataMember]
        public MetadataResponse Response { get; set; }

        public MetadataPair(SdkMessagePair pair)
        {
            Id = pair.Id;
            MessageNamespace = pair.MessageNamespace;
            Request = pair.Request == null ? null : new MetadataRequest(pair.Request);
            Response = pair.Response == null ? null : new MetadataResponse(pair.Response);
        }

        public SdkMessagePair ToSdk(SdkMessage sdk)
        {
            var pair = new SdkMessagePair(sdk, Id, MessageNamespace);
            pair.Request = Request.ToSdk(pair);
            if (Response != null)
            {
                pair.Response = Response.ToSdk(sdk);
            }
            return pair;
        }
    }
}
