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
            foreach (var entity in codeUnit.GetEntityTypes())
            {
                var constsClass = entity.GetMembers<CodeSnippetTypeMember>().FirstOrDefault(s => s.Text.Contains($"public static class {OobConstsClassName}"));
                if (constsClass == null)
                {
                    continue;
                }

                if (GenerateAttributeNameConsts)
                {
                    constsClass.Text = constsClass.Text.Replace($"public static class {OobConstsClassName}", $"public static partial class {AttributeConstsClassName}");
                }
                else
                {
                    entity.Members.Remove(constsClass);
                }
            }
        }
    }
}