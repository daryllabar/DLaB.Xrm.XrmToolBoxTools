namespace DLaB.CrmSvcUtilExtensions.OptionSet.Transliteration
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Crm.Services.Utility;
    using Microsoft.Xrm.Sdk.Metadata;

    /// <summary>
    /// Create an implementation of ICodeWriterFilterService if you want to provide your own
    /// logic for whether or not a given item will have code generated for it.
    /// </summary>
    public sealed class FilteringService : ICodeWriterFilterService
    {
        private Dictionary<string, bool> GeneratedOptionSets { get; set; }

        private ICodeWriterFilterService DefaultService { get; set; }

        public FilteringService(ICodeWriterFilterService defaultService)
        {
            DefaultService = defaultService;
            GeneratedOptionSets = new Dictionary<string, bool>();
        }

        /// <summary>
        /// Does not mark the OptionSet for generation if it has already been marked for
        /// generation.
        /// </summary>
        public bool GenerateOptionSet(
            OptionSetMetadataBase optionSetMetadata, IServiceProvider services)
        {
            if (optionSetMetadata.IsGlobal.HasValue && optionSetMetadata.IsGlobal.Value)
            {
                if (!GeneratedOptionSets.ContainsKey(optionSetMetadata.Name))
                {
                    GeneratedOptionSets[optionSetMetadata.Name] = true;
                    return true;
                }
            }
            else
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Ideally, we wouldn't generate any attributes, but we must in order to leverage
        /// the logic in CrmSvcUtil.  If the attribute for an OptionSet is not generated,
        /// then a null reference exception is thrown when attempting to create the
        /// OptionSet.  We will remove these in our ICustomizeCodeDomService implementation.
        /// </summary>
        public bool GenerateAttribute(
            AttributeMetadata attributeMetadata, IServiceProvider services)
        {
            return (attributeMetadata.AttributeType == AttributeTypeCode.Picklist
                || attributeMetadata.AttributeType == AttributeTypeCode.State
                || attributeMetadata.AttributeType == AttributeTypeCode.Status);
        }

        /// <summary>
        /// Ideally, we wouldn't generate any entities, but we must in order to leverage
        /// the logic in CrmSvcUtil.  If an entity which contains a custom OptionSet
        /// attribute is not generated, then the custom OptionSet will not be generated,
        /// either.  We will remove these in our ICustomizeCodeDomService implementation.
        /// </summary>
        public bool GenerateEntity(
            EntityMetadata entityMetadata, IServiceProvider services)
        {
            return DefaultService.GenerateEntity(entityMetadata, services);
        }

        /// <summary>
        /// We don't want to generate any relationships.
        /// </summary>
        public bool GenerateRelationship(
            RelationshipMetadataBase relationshipMetadata, EntityMetadata otherEntityMetadata,
            IServiceProvider services)
        {
            return false;
        }

        /// <summary>
        /// We don't want to generate any service contexts.
        /// </summary>
        public bool GenerateServiceContext(IServiceProvider services)
        {
            return false;
        }

        public bool GenerateOption(OptionMetadata optionMetadata, IServiceProvider services)
        {
            return DefaultService.GenerateOption(optionMetadata, services);
        }
    }
}
