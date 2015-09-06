using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk;

namespace DLaB.CrmSvcUtilExtensions.Entity
{
    class OverridePropertyNames : INamingService
    {
        private INamingService DefaultService { get; set; }
        private Dictionary<string, List<string>> EntityAttributeSpecifiedNames { get; set; }

        public OverridePropertyNames(INamingService defaultService)
        {
            DefaultService = defaultService;
            EntityAttributeSpecifiedNames = ConfigHelper.GetDictionaryList(ConfigurationManager.AppSettings["EntityAttributeSpecifiedNames"], false);
        }

        #region INamingService Members

        public string GetNameForAttribute(EntityMetadata entityMetadata, AttributeMetadata attributeMetadata, IServiceProvider services)
        {
            List<string> specifiedNames;
            String attributeName;
            if (EntityAttributeSpecifiedNames.TryGetValue(entityMetadata.LogicalName.ToLower(), out specifiedNames) &&
                specifiedNames.Any(s => String.Equals(s, attributeMetadata.LogicalName, StringComparison.OrdinalIgnoreCase)))
            {
                attributeName = specifiedNames.First(s => String.Equals(s, attributeMetadata.LogicalName, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                attributeName = DefaultService.GetNameForAttribute(entityMetadata, attributeMetadata, services);
            }
            return attributeName;
        }

        public string GetNameForEntity(EntityMetadata entityMetadata, IServiceProvider services)
        {
            return DefaultService.GetNameForEntity(entityMetadata, services);
        }

        public string GetNameForEntitySet(EntityMetadata entityMetadata, IServiceProvider services)
        {
            return DefaultService.GetNameForEntitySet(entityMetadata, services);
        }

        public string GetNameForMessagePair(SdkMessagePair messagePair, IServiceProvider services)
        {
            return DefaultService.GetNameForMessagePair(messagePair, services);
        }

        public string GetNameForOption(OptionSetMetadataBase optionSetMetadata, OptionMetadata optionMetadata, IServiceProvider services)
        {
            return DefaultService.GetNameForOption(optionSetMetadata, optionMetadata, services);
        }

        public string GetNameForOptionSet(EntityMetadata entityMetadata, OptionSetMetadataBase optionSetMetadata, IServiceProvider services)
        {
            return DefaultService.GetNameForOptionSet(entityMetadata, optionSetMetadata, services);
        }

        public string GetNameForRelationship(EntityMetadata entityMetadata, RelationshipMetadataBase relationshipMetadata, EntityRole? reflexiveRole, IServiceProvider services)
        {
            return DefaultService.GetNameForRelationship(entityMetadata, relationshipMetadata, reflexiveRole, services);
        }

        public string GetNameForRequestField(SdkMessageRequest request, SdkMessageRequestField requestField, IServiceProvider services)
        {
            return DefaultService.GetNameForRequestField(request, requestField, services);
        }

        public string GetNameForResponseField(SdkMessageResponse response, SdkMessageResponseField responseField, IServiceProvider services)
        {
            return DefaultService.GetNameForResponseField(response, responseField, services);
        }

        public string GetNameForServiceContext(IServiceProvider services)
        {
            return DefaultService.GetNameForServiceContext(services);
        }

        #endregion
    }
}
