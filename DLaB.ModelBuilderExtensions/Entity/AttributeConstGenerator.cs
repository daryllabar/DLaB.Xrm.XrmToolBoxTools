using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;

namespace DLaB.ModelBuilderExtensions.Entity
{
    public class AttributeConstGenerator : AttributeConstGeneratorBase
    {
        public override bool GenerateAttributeNameConsts { get => Settings.EmitFieldsClasses; set => Settings.EmitFieldsClasses = value; }

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
                var constsClass = entity.GetMembers<CodeSnippetTypeMember>().FirstOrDefault(s => s.Text.Contains($"public partial class {OobConstsClassName}"));
                if (constsClass == null)
                {
                    continue;
                }

                var lines = constsClass.Text.Replace($"public static class {OobConstsClassName}", $"public static partial class {AttributeConstsClassName}")
                    .Replace($"\" +{Environment.NewLine}\t\t\"","")
                    .Split(new [] {
                    Environment.NewLine
                }, StringSplitOptions.None).ToList();
                
                var start = lines.FindIndex(l => l.StartsWith("\t\t\tpublic const string "));
                var end = lines.FindIndex(l => l.StartsWith("\t\t}"));
                var attributes = lines.Skip(start).Take(end - start).ToList();
                lines.RemoveRange(start, end - start);
                lines.InsertRange(start, attributes.OrderBy(a => a.Split(' ').Last()));
                constsClass.Text = string.Join(Environment.NewLine, lines);
            }
        }
    }
}