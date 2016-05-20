// =====================================================================
//
//  This file is part of the Microsoft Dynamics CRM SDK Code Samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  This source code is intended only as a supplement to Microsoft
//  Development Tools and/or online documentation.  See these other
//  materials for detailed information regarding Microsoft code samples.
//
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
//
// =====================================================================
//<snippetFilteringService>

// Define SKIP_STATE_OPTIONSETS if you plan on using this extension in conjunction with
// the unextended CrmSvcUtil, since it already generates state optionsets.
#define SKIP_STATE_OPTIONSETS

using System;
using System.Collections.Generic;
using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Sdk.Metadata;
using System.Linq;
using DLaB.Common;

namespace DLaB.CrmSvcUtilExtensions.OptionSet
{
    /// <summary>
    /// Specifies that OptionSets should be created.  Works in conjunction with CreateOptionSetEnums, since it also specifies other
    /// data to be created, but the CreateOptionSetEnums removes it from the DOM after it's already been added, since it needs the metadata.
    /// Prevents duplicate OptionSetGeneration
    /// </summary>
    public sealed class CodeWriterFilterService : ICodeWriterFilterService
    {
        private HashSet<string> GeneratedOptionSets { get; }

        private ICodeWriterFilterService DefaultService { get; }

        public CodeWriterFilterService(ICodeWriterFilterService defaultService)
        {
            DefaultService = defaultService;
            GeneratedOptionSets = new HashSet<string>();
        }

        private static readonly HashSet<string> OptionSetsToSkip = Config.GetHashSet("OptionSetsToSkip", new HashSet<string>());
        private static readonly List<string> OptionSetPrefixesToSkip = Config.GetList("OptionSetPrefixesToSkip", new List<string>());

        /// <summary>
        /// Does not mark the OptionSet for generation if it has already been generated.  
        /// This could get called for the same Global Option Set multiple times because it's on multiple Entites
        /// </summary>
        public bool GenerateOptionSet(OptionSetMetadataBase optionSetMetadata, IServiceProvider services)
        {

#if SKIP_STATE_OPTIONSETS
            // Only skip the state optionsets if the user of the extension wishes to.
            if (optionSetMetadata.OptionSetType == OptionSetType.State)
            {
                return false;
            }
#endif
            if (Skip(optionSetMetadata.Name))
            {
                return false;
            }

            bool generate = false;

            if (optionSetMetadata.IsGlobal.GetValueOrDefault())
            {
                if (!GeneratedOptionSets.Contains(optionSetMetadata.Name))
                {
                    GeneratedOptionSets.Add(optionSetMetadata.Name);
                    generate = true;
                }
            }
            else
            {
                generate = true;
            }

            // Remove Dups
            var metadataOptionSet = optionSetMetadata as OptionSetMetadata;
            if (generate && metadataOptionSet != null)
            {
                var namingService = new NamingService((INamingService)services.GetService(typeof(INamingService)));
                var names = new HashSet<string>();
                foreach (var option in metadataOptionSet.Options.ToList())
                {
                    var name = namingService.GetNameForOption(optionSetMetadata, option, services);
                    if (names.Contains(name))
                    {
                        metadataOptionSet.Options.Remove(option);
                    }
                    names.Add(name);
                }
            }

            return generate;
        }

        private bool Skip(string name)
        {
            name = name.ToLower();
            return OptionSetsToSkip.Contains(name) || OptionSetPrefixesToSkip.Any(p => name.StartsWith(p));
        }

        /// <summary>
        /// Ideally, we wouldn't generate any attributes, but we must in order to leverage
        /// the logic in CrmSvcUtil.  If the attribute for an OptionSet is not generated,
        /// then a null reference exception is thrown when attempting to create the
        /// OptionSet.  We will remove these in our ICustomizeCodeDomService implementation.
        /// </summary>
        public bool GenerateAttribute(AttributeMetadata attributeMetadata, IServiceProvider services)
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
        public bool GenerateEntity(EntityMetadata entityMetadata, IServiceProvider services)
        {
            return DefaultService.GenerateEntity(entityMetadata, services);
        }

        /// <summary>
        /// We don't want to generate any relationships.
        /// </summary>
        public bool GenerateRelationship(RelationshipMetadataBase relationshipMetadata, EntityMetadata otherEntityMetadata, IServiceProvider services)
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

    //</snippetFilteringService>
}