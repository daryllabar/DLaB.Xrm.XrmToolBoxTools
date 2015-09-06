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
using System.Text.RegularExpressions;
using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System.Linq;


namespace DLaB.CrmSvcUtilExtensions.OptionSet
{
    /// <summary>
    /// Specifies that OptionSets should be created.  Works in conjunction with CreateOptionSetEnums, since it also specifies other
    /// data to be created, but the CreateOptionSetEnums removes it from the DOM after it's already been added, since it needs the metadata.
    /// Also fixes any Invalid C# naming conventions
    /// Also fixes duplicate OptionSetValueNames
    /// </summary>
    public sealed class FilterOptionSetEnums : ICodeWriterFilterService
    {
        private HashSet<String> GeneratedOptionSets { get; set; }

        private ICodeWriterFilterService DefaultService { get; set; }

        public FilterOptionSetEnums(ICodeWriterFilterService defaultService)
        {
            DefaultService = defaultService;
            GeneratedOptionSets = new HashSet<String>();
        }

        /// <summary>
        /// Does not mark the OptionSet for generation if it has already been marked for
        /// generation.  This could get called for the same Global Option Set multiple times because it's on multiple Entites
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

            if (generate && optionSetMetadata.Name != null)
            {
                string name = optionSetMetadata.Name;
                if (name.ToLower().EndsWith("set"))
                {
                    optionSetMetadata.Name = name + "Enum";
                }
            }

            if (generate)
            {
                HandleDuplicateNames(optionSetMetadata);
            }

            return generate;
        }

        private static void HandleDuplicateNames(OptionSetMetadataBase optionSetMetadata)
        {
            var nonBooleanOptionSet = optionSetMetadata as OptionSetMetadata;
            if (nonBooleanOptionSet == null) { return; }

            foreach (var option in nonBooleanOptionSet.Options.ToList())
            {
                bool addValue = false;
                foreach (var otherOption in nonBooleanOptionSet.Options.Where(o =>
                    option != o &&
                    GetValidCSharpName(o) == GetValidCSharpName(option)).ToList())
                {
                    // options have identical text values, Remove if the int values are the same, add int to name if they are different
                    if (option.Value == otherOption.Value)
                    {
                        nonBooleanOptionSet.Options.Remove(otherOption);
                    }
                    else
                    {
                        otherOption.Label.UserLocalizedLabel.Label = string.Format("{0}_{1}", otherOption.Label.GetLocalOrDefaultText(), otherOption.Value);
                        addValue = true;
                    }
                }

                if (addValue)
                {
                    option.Label.UserLocalizedLabel.Label = string.Format("{0}_{1}", option.Label.GetLocalOrDefaultText(), option.Value);
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
            HandleInvalidCSharpName(optionMetadata);

            return DefaultService.GenerateOption(optionMetadata, services);
        }

        /// <summary>
        /// Fix to handle invalid C# naming conventions for optionSets
        /// </summary>
        /// <param name="optionMetadata"></param>
        private static void HandleInvalidCSharpName(OptionMetadata optionMetadata)
        {
            optionMetadata.Label = new Label(GetValidCSharpName(optionMetadata), 1033);
        }

        private static string GetValidCSharpName(OptionMetadata optionMetadata)
        {
            string label = optionMetadata.Label.GetLocalOrDefaultText();
            //remove spaces and special characters
            label = Regex.Replace(label, @"[^a-zA-Z0-9_]", string.Empty);
            if (label.Length > 0 && !char.IsLetter(label, 0))
            {
                label = "_" + label;
            }
            return label;
        }
    }

    //</snippetFilteringService>
}