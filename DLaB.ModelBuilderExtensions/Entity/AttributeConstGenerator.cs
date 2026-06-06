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
                var obsoleteProperties = new HashSet<string>(
                    entity.GetMembers<CodeMemberProperty>()
                        .Where(p => p.CustomAttributes.Cast<CodeAttributeDeclaration>().Any(a => IsObsoleteAttribute(a.Name)))
                        .Select(p => p.Name));

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
                    .Replace($"\" +{Environment.NewLine}\t\t\"","")
                    .Split(new [] {
                    Environment.NewLine
                }, StringSplitOptions.None).ToList();
               
                var start = lines.FindIndex(l => l.StartsWith("\t\t\tpublic const string "));
                var end = lines.FindIndex(l => l.StartsWith("\t\t}"));
                var attributes = lines.Skip(start).Take(end - start).ToList();
                lines.RemoveRange(start, end - start);
                lines.InsertRange(start, attributes.OrderBy(a => a.Split(' ').Last()));
                if (checkForMemberReplacement)
                {
                    var collisionIndex = lines.FindIndex(l => l.Contains(collisionString));
                    lines[collisionIndex] = lines[collisionIndex].Replace(collisionString, " = \"");
                }

                AddObsoleteAttributesToFieldConstants(lines, obsoleteProperties);
                constsClass.Text = string.Join(Environment.NewLine, lines);
            }
        }

        private static bool IsObsoleteAttribute(string name)
        {
            return string.Equals(name, "System.Obsolete", StringComparison.Ordinal)
                   || string.Equals(name, "System.ObsoleteAttribute", StringComparison.Ordinal)
                   || string.Equals(name, "Obsolete", StringComparison.Ordinal)
                   || string.Equals(name, "ObsoleteAttribute", StringComparison.Ordinal);
        }

        private static void AddObsoleteAttributesToFieldConstants(List<string> lines, HashSet<string> obsoleteProperties)
        {
            if (!obsoleteProperties.Any())
            {
                return;
            }

            const string constPrefix = "\t\t\tpublic const string ";
            for (var i = 0; i < lines.Count; i++)
            {
                if (!lines[i].StartsWith(constPrefix, StringComparison.Ordinal))
                {
                    continue;
                }

                var end = lines[i].IndexOf(" = ", constPrefix.Length, StringComparison.Ordinal);
                if (end < 0)
                {
                    continue;
                }

                var fieldName = lines[i].Substring(constPrefix.Length, end - constPrefix.Length);
                if (!obsoleteProperties.Contains(fieldName))
                {
                    continue;
                }

                if (i > 0 && lines[i - 1].IndexOf("[System.Obsolete", StringComparison.Ordinal) >= 0)
                {
                    continue;
                }

                lines.Insert(i, "\t\t\t[System.Obsolete(\"This attribute is deprecated.\")]");
                i++;
            }
        }
    }
}