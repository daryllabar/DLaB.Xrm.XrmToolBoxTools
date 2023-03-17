using DLaB.ModelBuilderExtensions.OptionSet;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Source.DLaB.Common;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DLaB.ModelBuilderExtensions.Entity;

namespace DLaB.ModelBuilderExtensions
{
    public class CustomizeCodeDomService : ICustomizeCodeDomService
    {
        public ICustomizeCodeDomService DefaultService { get; }
        #region Entity Properties

        public static bool AddDebuggerNonUserCode => ConfigHelper.GetAppSettingOrDefault("AddDebuggerNonUserCode", true);
        public static bool AddPrimaryAttributeConsts => ConfigHelper.GetAppSettingOrDefault("AddPrimaryAttributeConsts", true);
        public static bool CreateBaseClasses => ConfigHelper.GetAppSettingOrDefault("CreateBaseClasses", false);
        public static bool GenerateAnonymousTypeConstructor => ConfigHelper.GetAppSettingOrDefault("GenerateAnonymousTypeConstructor", true);
        public static bool GenerateAttributeNameConsts => ConfigHelper.GetAppSettingOrDefault("GenerateAttributeNameConsts", false);
        public static bool GenerateConstructorsSansLogicalName => ConfigHelper.GetAppSettingOrDefault("GenerateConstructorsSansLogicalName", false);
        public static bool GenerateEntityTypeCode => ConfigHelper.GetAppSettingOrDefault("GenerateEntityTypeCode", false);
        public static bool GenerateEnumProperties => ConfigHelper.GetAppSettingOrDefault("GenerateEnumProperties", true);
        public static bool GenerateOptionSetMetadataAttribute => ConfigHelper.GetAppSettingOrDefault("GenerateOptionSetMetadataAttribute", false);
        public static bool ReplaceOptionSetPropertiesWithEnum => ConfigHelper.GetAppSettingOrDefault("ReplaceOptionSetPropertiesWithEnum", true);
        public static bool UpdateMultiOptionSetAttributes => ConfigHelper.GetAppSettingOrDefault("UpdateMultiOptionSetAttributes", true);
        public static bool UpdateEnumerableEntityProperties => ConfigHelper.GetAppSettingOrDefault("UpdateEnumerableEntityProperties", true);

        #endregion Entity Properties

        #region Message Properties

        public static bool GenerateActionAttributeNameConsts => ConfigHelper.GetAppSettingOrDefault("GenerateActionAttributeNameConsts", false);
        public WhitelistBlacklistLogic Approver { get; }

        public bool MakeResponseActionsEditable { get; }

        #endregion Message Properties

        public CustomizeCodeDomService(ICustomizeCodeDomService defaultService, IDictionary<string, string> parameters)
        {
            DefaultService = defaultService;
            ConfigHelper.Initialize(parameters);

            MakeResponseActionsEditable = ConfigHelper.GetAppSettingOrDefault("MakeResponseActionsEditable", false);
            Approver = new WhitelistBlacklistLogic(Config.GetHashSet("ActionsWhitelist", new HashSet<string>()),
                Config.GetList("ActionPrefixesWhitelist", new List<string>()),
                Config.GetHashSet("ActionsToSkip", new HashSet<string>()),
                Config.GetList("ActionPrefixesToSkip", new List<string>()));
        }

        #region ICustomizeCodeDomService Members

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            Trace.TraceInformation("Entering ICustomizeCodeDomService.CustomizeCodeDom");
            Trace.TraceInformation("Number of Namespaces generated: {0}", codeUnit.Namespaces.Count);


            ProcessActions(codeUnit, services);
            ProcessOptionSets(codeUnit, services);

            if (UpdateMultiOptionSetAttributes)
            {
                new MultiOptionSetAttributeUpdater().CustomizeCodeDom(codeUnit, services);
            }
            if (UpdateEnumerableEntityProperties)
            {
                new EnumerableEntityPropertyUpdater().CustomizeCodeDom(codeUnit, services);
            }
            if (AddPrimaryAttributeConsts)
            {
                new PrimaryAttributeGenerator().CustomizeCodeDom(codeUnit, services);
            }
            if (GenerateConstructorsSansLogicalName)
            {
                new EntityConstructorsGenerator().CustomizeCodeDom(codeUnit, services);
            }
            if (GenerateAttributeNameConsts)
            {
                new RelationshipConstGenerator().CustomizeCodeDom(codeUnit, services);
                new AttributeConstGenerator().CustomizeCodeDom(codeUnit, services);
            }
            if (GenerateAnonymousTypeConstructor)
            {
                new AnonymousTypeConstructorGenerator().CustomizeCodeDom(codeUnit, services);
            }
            if (!GenerateEntityTypeCode)
            {
                new RemoveEntityTypeCodeService().CustomizeCodeDom(codeUnit, services);
            }

            var multiSelectCreated = false;
            if (GenerateEnumProperties)
            {
                var generator = new EnumPropertyGenerator(CreateBaseClasses, ReplaceOptionSetPropertiesWithEnum);
                generator.CustomizeCodeDom(codeUnit, services);
                multiSelectCreated = generator.MultiSelectEnumCreated;
            }
        
            if (GenerateOptionSetMetadataAttribute)
            {
                new OptionSetMetadataAttributeGenerator().CustomizeCodeDom(codeUnit, services);
            }

            if (CreateBaseClasses)
            {
                new EntityBaseClassGenerator(multiSelectCreated).CustomizeCodeDom(codeUnit, services);
            }
            if (AddDebuggerNonUserCode)
            {
                new Entity.MemberAttributes().CustomizeCodeDom(codeUnit, services);
            }


            Trace.TraceInformation("Exiting ICustomizeCodeDomService.CustomizeCodeDom");
        }

        #endregion


        private void ProcessActions(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            // Iterate over all of the namespaces that were generated.
            for (var i = 0; i < codeUnit.Namespaces.Count; ++i)
            {
                var types = codeUnit.Namespaces[i].Types;
                Trace.TraceInformation("Number of types in Namespace {0}: {1}", codeUnit.Namespaces[i].Name, types.Count);
                // Iterate over all of the types that were created in the namespace.
                for (var j = 0; j < types.Count;)
                {
                    // Remove the type if it is not to be generated.
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

            if (GenerateActionAttributeNameConsts)
            {
                new AttributeConstGenerator().CustomizeCodeDom(codeUnit, services);
            }
        }

        private void ProcessOptionSets(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            new CreateOptionSetEnums().CustomizeCodeDom(codeUnit, services);
            new LocalMultiOptionSetGenerator().CustomizeCodeDom(codeUnit, services);
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
            }
            else if (name.EndsWith("Response"))
            {
                name = name.Remove(name.Length - "Response".Length);
            }

            return Approver.IsAllowed(name.ToLower());
        }
    }
}
