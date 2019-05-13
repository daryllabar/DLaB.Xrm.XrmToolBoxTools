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

using System;
using System.Collections.Generic;
using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Sdk.Metadata;
using System.Linq;
using Source.DLaB.Common;

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
        private ICodeWriterFilterService EntityFilterService { get; }

        private static HashSet<string> UsedEntityGlobalOptionSets { get; set; }

        public static WhitelistBlacklistLogic Approver => new WhitelistBlacklistLogic(Config.GetHashSet("OptionSetsWhitelist", new HashSet<string>()),
                                                                                      Config.GetList("OptionSetsWhitelist", new List<string>()),
                                                                                      Config.GetHashSet("OptionSetsToSkip", new HashSet<string>()),
                                                                                      Config.GetList("OptionSetPrefixesToSkip", new List<string>()));

        public CodeWriterFilterService(ICodeWriterFilterService defaultService)
        {

            DefaultService = defaultService;
            if (string.IsNullOrWhiteSpace(OptionSetEntityFilter)
                || !GenerateOnlyReferencedOptionSets)
            {
                EntityFilterService = DefaultService;
            }
            else
            {
                var t = Type.GetType(OptionSetEntityFilter);
                if (t == null)
                {
                    throw new Exception("Unable to determine OptionSetEntityFilter Type");
                }
                EntityFilterService = (ICodeWriterFilterService)Activator.CreateInstance(t, DefaultService);
            }
            GeneratedOptionSets = new HashSet<string>();
        }

        private static readonly string OptionSetEntityFilter = Config.GetAppSettingOrDefault("OptionSetEntityFilter", "DLaB.CrmSvcUtilExtensions.Entity.CodeWriterFilterService");
        private static readonly bool GenerateOnlyReferencedOptionSets = Config.GetAppSettingOrDefault("GenerateOnlyReferencedOptionSets", false);

        /// <summary>
        /// Does not mark the OptionSet for generation if it has already been generated.  
        /// This could get called for the same Global Option Set multiple times because it's on multiple Entities
        /// </summary>
        public bool GenerateOptionSet(OptionSetMetadataBase optionSetMetadata, IServiceProvider services)
        {
            PopulateUsedEntityGlobalOptionSetOnInitialCall(services);

            // Skip the state optionsets unless the XrmClient is used 
            if (!Entity.CustomizeCodeDomService.UseXrmClient && optionSetMetadata.OptionSetType == OptionSetType.State)
            {
                return false;
            }

            if (optionSetMetadata.IsGlobal.GetValueOrDefault()
                && GenerateOnlyReferencedOptionSets
                && !UsedEntityGlobalOptionSets.Contains(optionSetMetadata.Name.ToLower()))
            {
                return false;
            }
            

            if (!Approver.IsAllowed(optionSetMetadata.Name.ToLower()))
            {
                return false;
            }

            var generate = false;

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
            if (generate && optionSetMetadata is OptionSetMetadata metadataOptionSet)
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

        private void PopulateUsedEntityGlobalOptionSetOnInitialCall(IServiceProvider services)
        {
            if (UsedEntityGlobalOptionSets != null) { return; }
            UsedEntityGlobalOptionSets = new HashSet<string>();

            if (!GenerateOnlyReferencedOptionSets) { return; }

            var metadataService = (IMetadataProviderService) services.GetService(typeof(IMetadataProviderService));
            var metadata = metadataService.LoadMetadata();
            foreach (var entity in metadata.Entities.Where(m => GenerateEntity(m, services)))
            {
                foreach (var name in entity.Attributes.Where(a => a.AttributeType == AttributeTypeCode.Picklist || a.AttributeType == AttributeTypeCode.Virtual && a is MultiSelectPicklistAttributeMetadata)
                    .Cast<EnumAttributeMetadata>()
                    .Where(a => a.OptionSet.IsGlobal.GetValueOrDefault())
                    .Select(a => a.OptionSet.Name.ToLower()))
                {
                    if (!UsedEntityGlobalOptionSets.Contains(name))
                    {
                        UsedEntityGlobalOptionSets.Add(name);
                    }
                }
            }
        }

        /// <summary>
        /// Ideally, we wouldn't generate any attributes, but we must in order to leverage
        /// the logic in CrmSvcUtil.  If the attribute for an OptionSet is not generated,
        /// then a null reference exception is thrown when attempting to create the
        /// OptionSet.  We will remove these in our ICustomizeCodeDomService implementation.
        /// </summary>
        public bool GenerateAttribute(AttributeMetadata attributeMetadata, IServiceProvider services)
        {
            return attributeMetadata.AttributeType == AttributeTypeCode.Picklist
                    || attributeMetadata.AttributeType == AttributeTypeCode.State
                    || attributeMetadata.AttributeType == AttributeTypeCode.Status;
        }

        /// <summary>
        /// Ideally, we wouldn't generate any entities, but we must in order to leverage
        /// the logic in CrmSvcUtil.  If an entity which contains a custom OptionSet
        /// attribute is not generated, then the custom OptionSet will not be generated,
        /// either.  We will remove these in our ICustomizeCodeDomService implementation.
        /// </summary>
        public bool GenerateEntity(EntityMetadata entityMetadata, IServiceProvider services)
        {
            return EntityFilterService.GenerateEntity(entityMetadata, services);
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