using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;

namespace DLaB.ModelBuilderExtensions.Entity
{
    public class AttributeConstGenerator : AttributeConstGeneratorBase
    {
        private const string Typo = "Available fields, a the time of codegen, for the ";
        private const string Corrected = "Available fields, at the time of codegen, for the ";

        public override bool GenerateAttributeNameConsts { get => Settings.EmitFieldsClasses; set => Settings.EmitFieldsClasses = value; }
        public bool ObsoleteDeprecated { get => DLaBSettings.ObsoleteDeprecated; set => DLaBSettings.ObsoleteDeprecated = value; }

        public AttributeConstGenerator(ICustomizeCodeDomService defaultService, IDictionary<string, string> parameters) : base(defaultService, parameters)
        {
        }

        public AttributeConstGenerator(ICustomizeCodeDomService defaultService, DLaBModelBuilderSettings settings = null) : base(defaultService, settings)
        {
        }

        public override void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            var obsoleteAttributes = ObsoleteDeprecated
                ? services.GetServiceOrLoadDefault<IObsoleteAttributesProviderService>(() => new ObsoleteAttributesProviderService(Settings)).GetObsoleteAttributes(services)
                : null;
            foreach (var entity in codeUnit.GetEntityTypes())
            {
                var constsClass = entity.GetMembers<CodeSnippetTypeMember>().FirstOrDefault(s => s.Text.Contains($"public partial class {OobConstsClassName}"));
                if (constsClass == null)
                {
                    continue;
                }

                var collisionString = NamingService.PropertyCollisionPostFix + " = \"";
                var checkForMemberReplacement = constsClass.Text.Contains(collisionString);
                var lines = constsClass.Text
                    .Replace(Typo, Corrected)
                    .Replace($"public static class {OobConstsClassName}", $"public static partial class {AttributeConstsClassName}")
                    .Replace($"\" +{Environment.NewLine}\t\t\"", "")
                    .Split(new[] {
                    Environment.NewLine
                }, StringSplitOptions.None).ToList();

                var start = lines.FindIndex(l => l.StartsWith("\t\t\tpublic const string "));
                var end = lines.FindIndex(l => l.StartsWith("\t\t}"));
                var attributes = lines.Skip(start).Take(end - start).ToList();
                if (obsoleteAttributes?.Any() == true)
                {
                    attributes = attributes.Select(a => UpdateObsoleteAttribute(entity.GetEntityLogicalName(), a, obsoleteAttributes)).ToList();
                }
                lines.RemoveRange(start, end - start);
                lines.InsertRange(start, attributes.OrderBy(a => a.Split(' ').Last()));
                if (checkForMemberReplacement)
                {
                    var collisionIndex = lines.FindIndex(l => l.Contains(collisionString));
                    lines[collisionIndex] = lines[collisionIndex].Replace(collisionString, " = \"");
                }
                constsClass.Text = string.Join(Environment.NewLine, lines);
            }
        }

        private string UpdateObsoleteAttribute(string entityLogicalName, string attribute, HashSet<string> obsoleteAttributes)
        {
            var parts = attribute.Split('"');
            if (parts.Length < 2)
            {
                return attribute;
            }
            var logicalName = parts[1];
            return obsoleteAttributes.Contains($"{entityLogicalName}.{logicalName}")
                ? $"\t\t\t[System.Obsolete(\"This attribute is deprecated.\")]{Environment.NewLine}{attribute}"
                : attribute;
        }
    }
}