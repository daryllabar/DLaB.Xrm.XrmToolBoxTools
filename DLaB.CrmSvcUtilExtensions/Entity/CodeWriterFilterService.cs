using DLaB.CrmSvcUtilExtensions.OptionSet;
using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;

namespace DLaB.CrmSvcUtilExtensions.Entity
{
    class CodeWriterFilterService  : ICodeWriterFilterService
    {
        private ICodeWriterFilterService DefaultService { get; }
        /// <summary>
        /// Contains Meta Data for entities, key'd by logical name
        /// </summary>
        public static Dictionary<string, EntityMetadata> EntityMetadata { get; set; }
        public HashSet<string> EntitiesToSkip { get; set; }

        public bool GenerateEntityRelationships { get; set; }

        static CodeWriterFilterService()
        {
            EntityMetadata = new Dictionary<string, EntityMetadata>();
        }

        public CodeWriterFilterService(ICodeWriterFilterService defaultService)
        {
            DefaultService = defaultService;
            EntitiesToSkip = ConfigHelper.GetHashSet("EntitiesToSkip");
            GenerateEntityRelationships = ConfigHelper.GetAppSettingOrDefault("GenerateEntityRelationships", true);
        }

        #region ICodeWriterFilterService Members

        #region Pass Through Implementations

        public bool GenerateAttribute(AttributeMetadata attributeMetadata, IServiceProvider services)
        {
            return DefaultService.GenerateAttribute(attributeMetadata, services);
        }
         
        public bool GenerateOption(OptionMetadata optionMetadata, IServiceProvider services)
        {
            return DefaultService.GenerateOption(optionMetadata, services);
        }

        public bool GenerateServiceContext(IServiceProvider services)
        {
            return DefaultService.GenerateServiceContext(services);
        }

        #endregion // Pass Through Implementations
        
        public bool GenerateEntity(EntityMetadata entityMetadata, IServiceProvider services)
        {
            if (!DefaultService.GenerateEntity(entityMetadata, services)) { return false; }

            if (!EntityMetadata.ContainsKey(entityMetadata.LogicalName))
            {
                EntityMetadata.Add(entityMetadata.LogicalName, entityMetadata);
            }
            return !EntitiesToSkip.Contains(entityMetadata.LogicalName);
        }

        public bool GenerateRelationship(RelationshipMetadataBase relationshipMetadata, EntityMetadata otherEntityMetadata, IServiceProvider services)
        {
            if (!GenerateEntityRelationships)
            {
                return false;
            }
            return DefaultService.GenerateRelationship(relationshipMetadata, otherEntityMetadata, services);
        }

        public bool GenerateOptionSet(OptionSetMetadataBase optionSetMetadata, IServiceProvider services)
        {
            return DefaultService.GenerateOptionSet(optionSetMetadata, services);
        }

        #endregion
    }
}
