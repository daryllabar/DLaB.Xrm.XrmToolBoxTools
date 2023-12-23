using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;

namespace DLaB.ModelBuilderExtensions
{
    public class CodeWriterFilterService : TypedServiceSettings<ICodeWriterFilterService>, ICodeWriterFilterService
    {
        public BlacklistLogic EntityApprover { get; set; }

        private bool EnableFileDataType { get => DLaBSettings.EnableFileDataType; set => DLaBSettings.EnableFileDataType = value; }
        private bool EmitEntityETC { get => Settings.EmitEntityETC; set => Settings.EmitEntityETC = value; }

        public bool GenerateEntityRelationships { get => DLaBSettings.GenerateEntityRelationships; set => DLaBSettings.GenerateEntityRelationships = value; }

        public CodeWriterFilterService(ICodeWriterFilterService defaultService, IDictionary<string, string> parameters) : base(defaultService, parameters)
        {
            EntityApprover = new BlacklistLogic(new HashSet<string>(DLaBSettings.EntityBlacklist), DLaBSettings.EntityRegExBlacklist);
        }

        public CodeWriterFilterService(ICodeWriterFilterService defaultService, DLaBModelBuilderSettings settings = null) : base(defaultService, settings)
        {
            EntityApprover = new BlacklistLogic(new HashSet<string>(DLaBSettings.EntityBlacklist), DLaBSettings.EntityRegExBlacklist);
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

        public bool GenerateEntity(EntityMetadata entityMetadata, IServiceProvider services)
        {
            // Some entities are not normally created (attachment for example) not sure why.  Allowing Whitelist to Override here.
            // Commented out on switch to ModelBuilder. Not sure if this is valid.
            //if (!Approver.IsExplicitlyAllowed(entityMetadata.LogicalName)
            //    && !DefaultService.GenerateEntity(entityMetadata, services)) { return false; }

            //if (!EntityMetadata.ContainsKey(entityMetadata.LogicalName))
            //{
            //    EntityMetadata.Add(entityMetadata.LogicalName, entityMetadata);
            //}

            return EntityApprover.IsAllowed(entityMetadata.LogicalName);
        }

        public bool GenerateRelationship(RelationshipMetadataBase relationshipMetadata, EntityMetadata otherEntityMetadata, IServiceProvider services)
        {
            return GenerateEntityRelationships && DefaultService.GenerateRelationship(relationshipMetadata, otherEntityMetadata, services);
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
            return (EmitEntityETC
                    || (optionSetMetadata.Name != "connection_record1objecttypecode"
                        && optionSetMetadata.Name != "connection_record2objecttypecode"))
                   &&
                   DefaultService.GenerateOptionSet(optionSetMetadata, services);
            // }
        }

        #endregion

        private static bool IsFileDataTypeAttribute(AttributeMetadata metadata)
        {
            return metadata.LogicalName?.EndsWith("_name") == true
                   && metadata.AttributeOf != null
                   && metadata.IsRenameable != null
                   && metadata.IsRenameable.Value == false
                   && metadata is StringAttributeMetadata;
        }
    }
}
