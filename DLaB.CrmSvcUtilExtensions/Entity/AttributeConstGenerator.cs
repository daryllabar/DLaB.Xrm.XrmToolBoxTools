using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;

namespace DLaB.ModelBuilderExtensions.Entity
{
    public class AttributeConstGenerator : AttributeConstGeneratorBase
    {
        public override bool GenerateAttributeNameConsts { get => DLaBSettings.GenerateAttributeNameConsts; set => DLaBSettings.GenerateAttributeNameConsts = value; }

        public AttributeConstGenerator(ICustomizeCodeDomService defaultService, IDictionary<string, string> parameters) : base(defaultService, parameters)
        {
        }

        public AttributeConstGenerator(ICustomizeCodeDomService defaultService, DLaBModelBuilderSettings settings = null) : base(defaultService, settings)
        {
        }

        public override void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            if (GenerateAttributeNameConsts)
            {
                CreateConstsClass(codeUnit);
            }
            else
            {
                RemoveMemberType(codeUnit);
            }
        }

        protected virtual void CreateConstsClass(CodeCompileUnit codeUnit)
        {
            if (AttributeConstsClassName != OobConstsClassName)
            {
                var type = codeUnit.GetTypes().First(t => t.IsClass && t.Name == OobConstsClassName);
                type.Name = AttributeConstsClassName;
            }
        }

        private static void RemoveMemberType(CodeCompileUnit codeUnit)
        {
            foreach (var type in codeUnit.GetTypes().Where(t => t.IsClass))
            {
                foreach (var member in type.GetMembers<CodeTypeDeclaration>().Where(t => t.Name == OobConstsClassName).ToList())
                {
                    type.Members.Remove(member);
                }
            }
        }
    }
}