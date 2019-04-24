﻿// =====================================================================
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
using System.Text.RegularExpressions;

namespace DLaB.CrmSvcUtilExtensions.Action
{
    /// <summary>
    /// Create an implementation of ICustomizeCodeDomService if you want to manipulate the
    /// code dom after ICodeGenerationService has run.
    /// </summary>
    public sealed class CustomizeCodeDomService : ICustomizeCodeDomService
    {
        public static bool GenerateActionAttributeNameConsts => ConfigHelper.GetAppSettingOrDefault("GenerateActionAttributeNameConsts", false);

        public bool MakeResponseActionsEditable { get; }

        public CustomizeCodeDomService()
        {
            MakeResponseActionsEditable = ConfigHelper.GetAppSettingOrDefault("MakeResponseActionsEditable", false);
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
                    if (Skip(types[j].Name))
                    {
                        types.RemoveAt(j);
                    }
                    else
                    {
                        ProcessAction(types[j]);
                        j++;
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

        private static readonly HashSet<string> ActionsWhitelist = Config.GetHashSet("ActionsWhitelist", new HashSet<string>());
        private static readonly List<string> ActionPrefixesWhitelist = Config.GetList("ActionPrefixesWhitelist", new List<string>());

        private static readonly HashSet<string> ActionsToSkip = Config.GetHashSet("ActionsToSkip", new HashSet<string>());
        private static readonly List<string> ActionPrefixesToSkip = Config.GetList("ActionPrefixesToSkip", new List<string>());

        private bool Skip(string name)
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
            
            var prefix = Regex.Match(name, "([^_]+)_").Groups[1].Value;
            var hasPrefix = !string.IsNullOrEmpty(prefix);


            // Check for white list filters

            // If the prefix for the action in in the whitelist then allow it to be generated
            if ((ActionPrefixesWhitelist.Count > 0) && hasPrefix && ActionPrefixesWhitelist.Any(x => x.Equals(prefix, StringComparison.InvariantCultureIgnoreCase)))
            {
                return false;
            }

            // If the action is in the whitelist then allow it to be generated
            if ((ActionsWhitelist.Count > 0) && ActionsWhitelist.Any(x => x.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
            {
                return false;
            }

            // If any white list filters were specified and we didn't match any then we will skip and not process blacklist items
            if ((ActionPrefixesWhitelist.Count > 0) || (ActionsWhitelist.Count > 0))
            {
                return true;
            }


            // Check for black list filters

            // TODO: This seems fishy to me??? Why is the publisher prefix being stripped off?
            //       This doesn't allow prefixes to be skipped to properly filter since the publisher
            //       prefixes are removed, e.g., it would be impossible to filter all "msdyn" actions
            //       because the prefix is stripped off before comparing with the prefix list.
            //       I left this as is because it would introduce a breaking change.
            var index = name.IndexOf('_');
            if (index >= 0)
            {
                name = name.Substring(index + 1, name.Length - index - 1);
            }
            name = name.ToLower();
            return ActionsToSkip.Contains(name) || ActionPrefixesToSkip.Any(p => name.StartsWith(p));   
        }
    }
}