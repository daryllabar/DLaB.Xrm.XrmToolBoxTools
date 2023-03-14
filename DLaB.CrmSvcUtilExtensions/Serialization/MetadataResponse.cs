using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Crm.Services.Utility;

namespace DLaB.ModelBuilderExtensions.Serialization
{
    [DataContract(Namespace = "http://DLaB.ModelBuilderExtensions")]
    public class MetadataResponse
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public Dictionary<int, MetadataResponseField> ResponseFields { get; set; }

        public MetadataResponse(SdkMessageResponse request)
        {
            Id = request.Id;
            ResponseFields = new Dictionary<int, MetadataResponseField>();
            foreach (var field in request.ResponseFields)
            {
                ResponseFields[field.Key] = new MetadataResponseField(field.Value);
            }
        }

        internal SdkMessageResponse ToSdk(SdkMessage sdk)
        {
            var response = new SdkMessageResponse(Id);
            foreach (var field in ResponseFields)
            {
                response.ResponseFields[field.Key] = field.Value;
            }
            return response;
        }
    }
}
