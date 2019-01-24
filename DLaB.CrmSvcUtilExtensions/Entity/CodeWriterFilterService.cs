using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using Source.DLaB.Common;

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
        public HashSet<string> EntitiesWhitelist { get; set; }
        public List<string> EntityPrefixesToSkip { get; set; } 

        public bool GenerateEntityRelationships { get; set; }

        static CodeWriterFilterService()
        {
            EntityMetadata = new Dictionary<string, EntityMetadata>();
        }

        public CodeWriterFilterService(ICodeWriterFilterService defaultService)
        {
            DefaultService = defaultService;
            EntitiesToSkip = Config.GetHashSet("EntitiesToSkip", new HashSet<string>());
            EntitiesWhitelist = Config.GetHashSet("EntitiesWhitelist", new HashSet<string>());
            EntityPrefixesToSkip = Config.GetList("EntityPrefixesToSkip", new List<string>());
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

            // If Whitelist is populated, Skip if not in Whitelist.
            if (EntitiesWhitelist.Count > 0 && !EntitiesWhitelist.Contains(entityMetadata.LogicalName))
            {
                return false;
            }

            return !EntitiesToSkip.Contains(entityMetadata.LogicalName) && !EntityPrefixesToSkip.Any(p => entityMetadata.LogicalName.StartsWith(p));
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
            //if (optionSetMetadata.OptionSetType.Value == OptionSetType.State
            //        || optionSetMetadata.OptionSetType.Value == OptionSetType.Status
            //        || optionSetMetadata.OptionSetType.Value == OptionSetType.Picklist && optionSetMetadata.IsGlobal == false)
            //{
            //    return true;
            //}
            //else
            //{
                return DefaultService.GenerateOptionSet(optionSetMetadata, services);
            // }

        }

        #endregion
    }
}
