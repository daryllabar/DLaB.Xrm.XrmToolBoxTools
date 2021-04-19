using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using Source.DLaB.Common;

namespace DLaB.CrmSvcUtilExtensions.Entity
{
    public class CodeWriterFilterService  : ICodeWriterFilterService
    {
        private static bool EnableFileDataType => ConfigHelper.GetAppSettingOrDefault("EnableFileDataType", true);
        private ICodeWriterFilterService DefaultService { get; }
        /// <summary>
        /// Contains Meta Data for entities, key'd by logical name
        /// </summary>
        public static Dictionary<string, EntityMetadata> EntityMetadata { get; set; }
        public WhitelistBlacklistLogic Approver { get; }

        public bool GenerateEntityRelationships { get; set; }

        static CodeWriterFilterService()
        {
            EntityMetadata = new Dictionary<string, EntityMetadata>();
        }

        public CodeWriterFilterService(ICodeWriterFilterService defaultService)
        { 
            DefaultService = defaultService;
            Approver = new WhitelistBlacklistLogic(Config.GetHashSet("EntitiesWhitelist", new HashSet<string>()),
                                                   Config.GetList("EntityPrefixesWhitelist", new List<string>()),
                                                   Config.GetHashSet("EntitiesToSkip", new HashSet<string>()),
                                                   Config.GetList("EntityPrefixesToSkip", new List<string>()));
            GenerateEntityRelationships = ConfigHelper.GetAppSettingOrDefault("GenerateEntityRelationships", true);
        }

        #region ICodeWriterFilterService Members

        #region Pass Through Implementations

        public bool GenerateOption(OptionMetadata optionMetadata, IServiceProvider services)
        {
            return DefaultService.GenerateOption(optionMetadata, services);
        }

        public bool GenerateServiceContext(IServiceProvider services)
        {
            return DefaultService.GenerateServiceContext(services);
        }

        #endregion // Pass Through Implementations

        public bool GenerateAttribute(AttributeMetadata metadata, IServiceProvider services)
        {
            return EnableFileDataType && IsFileDataTypeAttribute(metadata)
                   || DefaultService.GenerateAttribute(metadata, services);
        }

        private static bool IsFileDataTypeAttribute(AttributeMetadata metadata)
        {
            return metadata.LogicalName?.EndsWith("_name") == true
                   && metadata.AttributeOf != null
                   && metadata.IsRenameable != null
                   && metadata.IsRenameable.Value == false
                   && metadata is StringAttributeMetadata;
        }


        public bool GenerateEntity(EntityMetadata entityMetadata, IServiceProvider services)
        {
            // Some entities are not normally create (attachment for example) not sure why.  Allowing Whitelist to Override here.
            if (!Approver.IsExplicitlyAllowed(entityMetadata.LogicalName)
                && !DefaultService.GenerateEntity(entityMetadata, services)) { return false; }

            if (!EntityMetadata.ContainsKey(entityMetadata.LogicalName))
            {
                EntityMetadata.Add(entityMetadata.LogicalName, entityMetadata);
            }

            return Approver.IsAllowed(entityMetadata.LogicalName);
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
