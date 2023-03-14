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
//<snippetCodeCustomizationService>

// Define REMOVE_PROXY_TYPE_ASSEMBLY_ATTRIBUTE if you plan on compiling the output from
// this CrmSvcUtil extension with the output from the unextended CrmSvcUtil in the same
// assembly (this assembly attribute can only be defined once in the assembly).

using System;

using Microsoft.Crm.Services.Utility;
using System.Diagnostics;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk.Metadata;

namespace DLaB.ModelBuilderExtensions.OptionSet
{
    /// <summary>
    /// Create an implementation of ICustomizeCodeDomService if you want to manipulate the
    /// code dom after ICodeGenerationService has run.
    /// </summary>
    public sealed class CreateOptionSetEnums : ICustomizeCodeDomService
    {
        public static bool AddOptionSetMetadataAttribute => ConfigHelper.GetAppSettingOrDefault("AddOptionSetMetadataAttribute", false);

        /// <summary>
        /// Remove the unnecessary classes that we generated for entities. 
        /// </summary>
        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            Trace.TraceInformation("Entering ICustomizeCodeDomService.CustomizeCodeDom");
            Trace.TraceInformation("Number of Namespaces generated: {0}", codeUnit.Namespaces.Count);

            codeUnit.RemoveAssemblyAttributes();            
            RemoveNonOptionSetDefinitions(codeUnit);
            AddMetadataAttributes(codeUnit, services);
            SortOptionSets(codeUnit);
            Trace.TraceInformation("Exiting ICustomizeCodeDomService.CustomizeCodeDom");
        }

        private void RemoveNonOptionSetDefinitions(CodeCompileUnit codeUnit)
        {
            // Iterate over all of the namespaces that were generated.
            for (var i = 0; i < codeUnit.Namespaces.Count; ++i)
            {
                var types = codeUnit.Namespaces[i].Types;
                Trace.TraceInformation("Number of types in Namespace {0}: {1}", codeUnit.Namespaces[i].Name, types.Count);
                // Iterate over all of the types that were created in the namespace.
                for (var j = 0; j < types.Count;)
                {
                    // Remove the type if it is not an enum (all OptionSets are enums) or has been defined to be skipped.
                    if (!types[j].IsEnum)
                    {
                        types.RemoveAt(j);
                    }
                    else
                    {
                        j += 1;
                    }
                }
            }
        }

        private static void AddMetadataAttributes(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            if (!AddOptionSetMetadataAttribute)
            {
                return;
            }

            
            var metadataByName = services.GetMetadataForEnumsByName();
            foreach (var type in codeUnit.GetTypes())
            {
                if (metadataByName.TryGetValue(type.Name, out var osMetadata))
                {
                    AddMetadataAttributesForSet(type, osMetadata);
                }
                else
                {
                    Trace.TraceInformation("Unable to find metadata for {0}", type.Name);
                }
            }
        }

        public static void AddMetadataAttributesForSet(CodeTypeDeclaration type, OptionSetMetadataBase osMetadata)
        {
            if (!AddOptionSetMetadataAttribute)
            {
                return;
            }

            Trace.TraceInformation("Adding MetadataAttributes for {0}", type.Name);
            var options = osMetadata.GetOptions();
            var metadataByValue = options.ToDictionary(k => k.Value);
            var orderIndexByValue = new Dictionary<int, int>();
            for (var i = 0; i < options.Count; i++)
            {
                if (options[i].Value is int intValue)
                {
                    orderIndexByValue.Add(intValue, i);
                }
                else
                {
                    Trace.TraceInformation("Unable to find orderIndexByValue for {0}", type.Name);
                }
            }

            for (var i = 0; i < type.Members.Count; i++)
            {
                var value = type.Members[i] as CodeMemberField;
                if (value != null 
                    && value.InitExpression is CodePrimitiveExpression primitive 
                    && primitive.Value is int intValue 
                    && metadataByValue.TryGetValue(intValue, out var metadata))
                {
                    var attribute = new CodeAttributeDeclaration("OptionSetMetadataAttribute", 
                        new CodeAttributeArgument(new CodePrimitiveExpression(metadata.Label.GetLocalOrDefaultText())),
                        new CodeAttributeArgument(new CodePrimitiveExpression(orderIndexByValue[intValue]))
                    );
                    var optionalArs = new Stack<string>(new []
                    {
                        metadata.Color,
                        metadata.Description.GetLocalOrDefaultText(),
                        metadata.ExternalValue
                    });

                    while (optionalArs.Count > 0 && string.IsNullOrWhiteSpace(optionalArs.Peek()))
                    {
                        optionalArs.Pop();
                    }

                    if (optionalArs.Count > 0)
                    {
                        attribute.Arguments.AddRange(
                            optionalArs.Select(v => new CodeAttributeArgument(new CodePrimitiveExpression(v)))
                                       .Reverse().ToArray()
                        );
                    }

                    value.CustomAttributes.Add(attribute);
                }
                else
                {
                    Trace.TraceInformation("Unable to determine OptionSetMetadataAttribute for {0}", value?.Name);
                }
            }
        }

        private static void SortOptionSets(CodeCompileUnit codeUnit)
        {
            // Attempt to order by name by copying to temp collection, removing all from real collection, then adding back ordered by the name
            var temp = new CodeNamespace[codeUnit.Namespaces.Count];
            codeUnit.Namespaces.CopyTo(temp, 0);

            for (int i = 0; i < codeUnit.Namespaces.Count; i++)
            {
                codeUnit.Namespaces.RemoveAt(i);
            }
            codeUnit.Namespaces.AddRange(temp.OrderBy(n => n.Name).ToArray());

            for (int i = codeUnit.Namespaces.Count - 1; i >= 0; i--)
            {
                var nameSpace = codeUnit.Namespaces[i];

                // Attempt to order by name by copying to temp collection, removing all from real collection, then adding back ordered by the name
                var tmpType = new CodeTypeDeclaration[nameSpace.Types.Count];
                nameSpace.Types.CopyTo(tmpType, 0);

                for (int j = nameSpace.Types.Count - 1; j >= 0; j--)
                {
                    var type = nameSpace.Types[j];
                    nameSpace.Types.RemoveAt(j);

                    var tmpMember = new CodeTypeMember[type.Members.Count];
                    type.Members.CopyTo(tmpMember, 0);

                    for (int k = type.Members.Count - 1; k >= 0; k--)
                    {
                        type.Members.RemoveAt(k);
                    }
                    type.Members.AddRange(tmpMember.OrderBy(m => m.Name).ToArray());
                }
                nameSpace.Types.AddRange(tmpType.OrderBy(n => n.Name).ToArray());
            }
        }
    }
}