using System.Diagnostics;
using DLaB.CrmSvcUtilExtensions.OptionSet;
using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;

namespace DLaB.CrmSvcUtilExtensions.Action
{
    class CodeWriterFilterService  : ICodeWriterFilterService
    {
        private ICodeWriterFilterService DefaultService { get; set; }

        public CodeWriterFilterService(ICodeWriterFilterService defaultService)
        {
            DefaultService = defaultService;
        }

        #region ICodeWriterFilterService Members

        #region Default To False Implmentations

        public bool GenerateAttribute(AttributeMetadata attributeMetadata, IServiceProvider services)
        {
            return false;
        }

        public bool GenerateOption(OptionMetadata optionMetadata, IServiceProvider services)
        {
            return false;
        }

        public bool GenerateRelationship(RelationshipMetadataBase relationshipMetadata, EntityMetadata otherEntityMetadata, IServiceProvider services)
        {
            return false;
        }

        public bool GenerateServiceContext(IServiceProvider services)
        {
            return false;
        }

        public bool GenerateOptionSet(OptionSetMetadataBase optionSetMetadata, IServiceProvider services)
        {
            return false;
        }

        public bool GenerateEntity(EntityMetadata entityMetadata, IServiceProvider services)
        {
            return false;
        }

        #endregion // Default To False Implmentations
        
        #endregion
    }
}
