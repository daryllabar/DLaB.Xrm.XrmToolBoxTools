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
using Source.DLaB.Common;

namespace DLaB.CrmSvcUtilExtensions.Action
{
    /// <summary>
    /// Create an implementation of ICustomizeCodeDomService if you want to manipulate the
    /// code dom after ICodeGenerationService has run.
    /// </summary>
    public sealed class CustomizeCodeDomService : ICustomizeCodeDomService
    {
        public static bool GenerateActionAttributeNameConsts => ConfigHelper.GetAppSettingOrDefault("GenerateActionAttributeNameConsts", false);
        public WhitelistBlacklistLogic Approver { get; }

        public bool MakeResponseActionsEditable { get; }

        public CustomizeCodeDomService()
        {
            MakeResponseActionsEditable = ConfigHelper.GetAppSettingOrDefault("MakeResponseActionsEditable", false);
            Approver = new WhitelistBlacklistLogic(Config.GetHashSet("ActionsWhitelist", new HashSet<string>()),
                                                   Config.GetList("ActionPrefixesWhitelist", new List<string>()),
                                                   Config.GetHashSet("ActionsToSkip", new HashSet<string>()),
                                                   Config.GetList("ActionPrefixesToSkip", new List<string>()));
        }

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

            ProcessActions(codeUnit);

            if (GenerateActionAttributeNameConsts)
            {
                new AttributeConstGenerator().CustomizeCodeDom(codeUnit, services);
            }

            Trace.TraceInformation("Exiting ICustomizeCodeDomService.CustomizeCodeDom");
        }

        private void ProcessActions(CodeCompileUnit codeUnit)
        {
            // Iterate over all of the namespaces that were generated.
            for (var i = 0; i < codeUnit.Namespaces.Count; ++i)
            {
                var types = codeUnit.Namespaces[i].Types;
                Trace.TraceInformation("Number of types in Namespace {0}: {1}", codeUnit.Namespaces[i].Name, types.Count);
                // Iterate over all of the types that were created in the namespace.
                for (var j = 0; j < types.Count; )
                {
                    // Remove the type if it is not an enum (all OptionSets are enums) or has no members.
                    if (GenerateAction(types[j].Name))
                    {
                        ProcessAction(types[j]);
                        j++;
                    }
                    else
                    {
                        types.RemoveAt(j);
                    }
                }
            }
        }

        private void ProcessAction(CodeTypeDeclaration action)
        {
            var orgResponse = new CodeTypeReference(typeof(Microsoft.Xrm.Sdk.OrganizationResponse)).BaseType;
            if (MakeResponseActionsEditable && action.BaseTypes.OfType<CodeTypeReference>().Any(r => r.BaseType == orgResponse))
            {
                foreach (var prop in from CodeTypeMember member in action.Members
                                       let propDom = member as CodeMemberProperty
                                       where propDom != null && !propDom.HasSet 
                                       select propDom)
                {
                    var thisMember = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "Results");
                    var indexOf = new CodeArrayIndexerExpression(thisMember, new CodePrimitiveExpression(prop.Name));
                    prop.SetStatements.Add(new CodeAssignStatement(indexOf, new CodePropertySetValueReferenceExpression()));
                }
            }
        }

        private bool GenerateAction(string name)
        {
            name = name.Replace(" ", string.Empty);
            // Actions are weird, don't know how to get the whole name since it's a workflow, so I'll hack this here
            if (name.EndsWith("Request"))
            {
                name = name.Remove(name.Length - "Request".Length);
            }else if (name.EndsWith("Response"))
            {
                name = name.Remove(name.Length - "Response".Length);
            }

            return Approver.IsAllowed(name.ToLower());
        }
    }
}