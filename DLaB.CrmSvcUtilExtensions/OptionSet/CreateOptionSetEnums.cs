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
using System.Linq;

namespace DLaB.CrmSvcUtilExtensions.OptionSet
{
    /// <summary>
    /// Create an implementation of ICustomizeCodeDomService if you want to manipulate the
    /// code dom after ICodeGenerationService has run.
    /// </summary>
    public sealed class CreateOptionSetEnums : ICustomizeCodeDomService
    {
        /// <summary>
        /// Remove the unnecessary classes that we generated for entities. 
        /// </summary>
        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            Trace.TraceInformation("Entering ICustomizeCodeDomService.CustomizeCodeDom");
            Trace.TraceInformation("Number of Namespaces generated: {0}", codeUnit.Namespaces.Count);

            //#if REMOVE_PROXY_TYPE_ASSEMBLY_ATTRIBUTE

            foreach (CodeAttributeDeclaration attribute in codeUnit.AssemblyCustomAttributes)
            {
                Trace.TraceInformation("Attribute BaseType is {0}", attribute.AttributeType.BaseType);
                if (attribute.AttributeType.BaseType == "Microsoft.Xrm.Sdk.Client.ProxyTypesAssemblyAttribute")
                {
                    codeUnit.AssemblyCustomAttributes.Remove(attribute);
                    break;
                }
            }

            //#endif

            RemoveNonOptionSetDefinitions(codeUnit);
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