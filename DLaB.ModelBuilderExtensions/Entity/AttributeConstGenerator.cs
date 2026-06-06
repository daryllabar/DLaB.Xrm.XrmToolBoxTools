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
                var obsoleteProperties = (from p in entity.GetMembers<CodeMemberProperty>()
                                          let obsoleteAttribute = p.CustomAttributes.OfType<CodeAttributeDeclaration>()
                                              .FirstOrDefault(a => IsObsoleteAttribute(a.Name))
                                          where obsoleteAttribute != null
                                          select new
                                          {
                                              p.Name,
                                              Message = GetObsoleteMessage(obsoleteAttribute)
                                          })
                    .ToDictionary(p => p.Name, p => p.Message);

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

        private static string GetObsoleteMessage(CodeAttributeDeclaration obsoleteAttribute)
        {
            const string defaultMessage = "This attribute is deprecated.";
            if (obsoleteAttribute.Arguments.Count == 0 || !(obsoleteAttribute.Arguments[0].Value is CodePrimitiveExpression expression))
            {
                return defaultMessage;
            }

            return expression.Value as string ?? defaultMessage;
        }

        private static bool IsObsoleteAttribute(string name)
        {
            return string.Equals(name, "System.Obsolete", StringComparison.Ordinal)
                   || string.Equals(name, "System.ObsoleteAttribute", StringComparison.Ordinal)
                   || string.Equals(name, "Obsolete", StringComparison.Ordinal)
                   || string.Equals(name, "ObsoleteAttribute", StringComparison.Ordinal);
        }

        private static void AddObsoleteAttributesToFieldConstants(List<string> lines, IDictionary<string, string> obsoleteProperties)
        {
            if (!obsoleteProperties.Any())
            {
                return;
            }

            const string constPrefix = "public const string ";
            for (var i = 0; i < lines.Count; i++)
            {
                var trimmedLine = lines[i].TrimStart();
                if (!trimmedLine.StartsWith(constPrefix, StringComparison.Ordinal))
                {
                    continue;
                }

                var end = trimmedLine.IndexOf(" = ", constPrefix.Length, StringComparison.Ordinal);
                if (end < 0)
                {
                    continue;
                }

                var fieldName = trimmedLine.Substring(constPrefix.Length, end - constPrefix.Length);
                if (!obsoleteProperties.TryGetValue(fieldName, out var message))
                {
                    continue;
                }

                var previousLine = i > 0 ? lines[i - 1].Trim() : string.Empty;
                if (string.IsNullOrWhiteSpace(previousLine) && i > 1)
                {
                    previousLine = lines[i - 2].Trim();
                }
                if (previousLine.StartsWith("[System.Obsolete", StringComparison.Ordinal))
                {
                    continue;
                }

                var indent = lines[i].Substring(0, lines[i].Length - trimmedLine.Length);
                lines.Insert(i, $"{indent}[System.Obsolete(\"{EscapeObsoleteMessage(message)}\")]");
                i++;
            }
        }

        private static string EscapeObsoleteMessage(string message)
        {
            return message
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n")
                .Replace("\t", "\\t");
        }
    }
}