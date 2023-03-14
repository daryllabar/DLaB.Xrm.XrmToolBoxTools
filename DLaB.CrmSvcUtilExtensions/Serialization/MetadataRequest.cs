using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Crm.Services.Utility;

namespace DLaB.ModelBuilderExtensions.Serialization
{
    [DataContract(Namespace = "http://DLaB.ModelBuilderExtensions")]
    public class MetadataRequest
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public Dictionary<int, MetadataRequestField> RequestFields { get; set; }

        public MetadataRequest(SdkMessageRequest request)
        {
            Id = request.Id;
            Name = request.Name;
            RequestFields = new Dictionary<int, MetadataRequestField>();
            foreach (var field in request.RequestFields)
            {
                RequestFields[field.Key] = new MetadataRequestField(field.Value);
            }
        }

        public SdkMessageRequest ToSdk(SdkMessagePair sdk)
        {
            var request = new SdkMessageRequest(sdk, Id, Name);
            foreach (var field in RequestFields)
            {
                request.RequestFields[field.Key] = field.Value.ToSdk(request);
            }

            return request;
        }
    }
}
