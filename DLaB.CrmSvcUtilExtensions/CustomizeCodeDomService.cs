using DLaB.ModelBuilderExtensions.Entity;
using DLaB.ModelBuilderExtensions.OptionSet;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Source.DLaB.Xrm;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xrm.Sdk.Metadata;

namespace DLaB.ModelBuilderExtensions
{
    public class CustomizeCodeDomService : TypedServiceSettings<ICustomizeCodeDomService>, ICustomizeCodeDomService
    {
        #region Entity Properties

        public bool AddDebuggerNonUserCode { get => DLaBSettings.AddDebuggerNonUserCode; set => DLaBSettings.AddDebuggerNonUserCode = value; }
        public bool AddPrimaryAttributeConsts { get => DLaBSettings.AddPrimaryAttributeConsts; set => DLaBSettings.AddPrimaryAttributeConsts = value; }
        public bool CreateBaseClasses { get => DLaBSettings.CreateBaseClasses; set => DLaBSettings.CreateBaseClasses = value; }
        public bool GenerateAnonymousTypeConstructor { get => DLaBSettings.GenerateAnonymousTypeConstructor; set => DLaBSettings.GenerateAnonymousTypeConstructor = value; }
        public bool GenerateAttributeNameConsts { get => DLaBSettings.GenerateAttributeNameConsts; set => DLaBSettings.GenerateAttributeNameConsts = value; }
        public bool GenerateConstructorsSansLogicalName { get => DLaBSettings.GenerateConstructorsSansLogicalName; set => DLaBSettings.GenerateConstructorsSansLogicalName = value; }
        public bool GenerateEntityTypeCode { get => DLaBSettings.GenerateEntityTypeCode; set => DLaBSettings.GenerateEntityTypeCode = value; }
        public bool GenerateEnumProperties { get => DLaBSettings.GenerateEnumProperties; set => DLaBSettings.GenerateEnumProperties = value; }
        public bool GenerateOptionSetMetadataAttribute { get => DLaBSettings.GenerateOptionSetMetadataAttribute; set => DLaBSettings.GenerateOptionSetMetadataAttribute = value; }
        public bool ReplaceOptionSetPropertiesWithEnum { get => DLaBSettings.ReplaceOptionSetPropertiesWithEnum; set => DLaBSettings.ReplaceOptionSetPropertiesWithEnum = value; }
        public bool UpdateMultiOptionSetAttributes { get => DLaBSettings.UpdateMultiOptionSetAttributes; set => DLaBSettings.UpdateMultiOptionSetAttributes = value; }
        public bool UpdateEnumerableEntityProperties { get => DLaBSettings.UpdateEnumerableEntityProperties; set => DLaBSettings.UpdateEnumerableEntityProperties = value; }

        #endregion Entity Properties

        #region Message Properties

        public WhitelistBlacklistLogic MessageApprover { get; set; }
        public bool GenerateActionAttributeNameConsts { get => DLaBSettings.GenerateActionAttributeNameConsts; set => DLaBSettings.GenerateActionAttributeNameConsts = value; }

        public bool MakeResponseActionsEditable { get => DLaBSettings.MakeResponseActionsEditable; set => DLaBSettings.MakeResponseActionsEditable = value; }

        #endregion Message Properties

        private Dictionary<string, EntityMetadata> _entities;

        public CustomizeCodeDomService(ICustomizeCodeDomService defaultService, IDictionary<string, string> parameters) : base(defaultService, parameters)
        {
            MessageApprover = new WhitelistBlacklistLogic(Settings.MessageNamesFilter?.Any() == true,
                new HashSet<string>(DLaBSettings.MessageToSkip),
                DLaBSettings.MessagePrefixesToSkip);
        }

        public CustomizeCodeDomService(ICustomizeCodeDomService defaultService, DLaBModelBuilderSettings settings) : base(defaultService, settings)
        {
            MessageApprover = new WhitelistBlacklistLogic(Settings.MessageNamesFilter?.Any() == true,
                new HashSet<string>(DLaBSettings.MessageToSkip),
                DLaBSettings.MessagePrefixesToSkip);
        }

        #region ICustomizeCodeDomService Members

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            Trace.TraceInformation("Entering DLaB.ModelBuilderExtensions.CustomizeCodeDomService.CustomizeCodeDom");

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
                new AnonymousTypeConstructorGenerator(GetEntities(services)).CustomizeCodeDom(codeUnit, services);
            }

            if (!GenerateEntityTypeCode)
            {
                new RemoveEntityTypeCodeService().CustomizeCodeDom(codeUnit, services);
            }

            if (GenerateEnumProperties)
            {
                var generator = new EnumPropertyGenerator(CreateBaseClasses, ReplaceOptionSetPropertiesWithEnum, GetEntities(services));
                generator.CustomizeCodeDom(codeUnit, services);
            }

            if (GenerateOptionSetMetadataAttribute)
            {
                new OptionSetMetadataAttributeGenerator().CustomizeCodeDom(codeUnit, services);
            }

            if (CreateBaseClasses)
            {
                new EntityBaseClassGenerator(this, Settings).CustomizeCodeDom(codeUnit, services);
            }

            if (AddDebuggerNonUserCode)
            {
                new Entity.MemberAttributes().CustomizeCodeDom(codeUnit, services);
            }


            Trace.TraceInformation("Exiting ICustomizeCodeDomService.CustomizeCodeDom");
        }

        #endregion

        private Dictionary<string, EntityMetadata> GetEntities(IServiceProvider services)
        {
            return _entities ?? (_entities = services.GetService<IMetadataProviderService>().LoadMetadata(services).Entities.ToDictionary(e => e.LogicalName));
        }

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
                    if (GenerateMessage(types[j].Name))
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

        private bool GenerateMessage(string name)
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

            return MessageApprover.IsAllowed(name.ToLower());
        }
    }
}
