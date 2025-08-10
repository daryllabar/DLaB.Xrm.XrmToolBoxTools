using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DLaB.Xrm;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;

namespace DLaB.ModelBuilderExtensions.Entity
{
    /// <summary>
    /// Adds the System.Diagnostics.AddDebuggerNonUserCode Attribute
    /// </summary>
    public class MemberAttributes : TypedServiceSettings<ICustomizeCodeDomService>, ICustomizeCodeDomService
    {
        public bool AddDebuggerNonUserCode { get => DLaBSettings.AddDebuggerNonUserCode; set => DLaBSettings.AddDebuggerNonUserCode = value; }
        public bool ObsoleteDeprecated { get => DLaBSettings.ObsoleteDeprecated; set => DLaBSettings.ObsoleteDeprecated = value; }
        public List<string> ObsoleteTokens { get => DLaBSettings.ObsoleteTokens; set => DLaBSettings.ObsoleteTokens = value; }

        public MemberAttributes(ICustomizeCodeDomService defaultService, DLaBModelBuilderSettings settings = null) : base(defaultService, settings)
        {
        }

        public MemberAttributes(ICustomizeCodeDomService defaultService, IDictionary<string, string> parameters) : base(defaultService, parameters)
        {
        }

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            if (!AddDebuggerNonUserCode && !ObsoleteDeprecated)
            {
                return;
            }

            var obsoleteAttributes = GetObsoleteAttributes(services);
            var provider = CodeDomProvider.CreateProvider("CSharp");
            var options = new CodeGeneratorOptions()
            {
                BracingStyle = "C",
                IndentString = "\t",
            };

            if (ObsoleteDeprecated)
            {
                foreach (var code in from CodeTypeDeclaration type in codeUnit.Namespaces[0].Types
                         where type.IsClass
                         from dynamic member in type.Members
                         select new { EntityLogicalName = type.GetEntityLogicalName(), Member = member })
                {
                    if (code.Member is CodeMemberProperty property
                        && obsoleteAttributes.Contains(code.EntityLogicalName + "." + property.GetLogicalName())) {
                        property.CustomAttributes.Add(new CodeAttributeDeclaration("System.Obsolete", new CodeAttributeArgument(new CodePrimitiveExpression("This attribute is deprecated."))));
                    }
                }
            }

            if (AddDebuggerNonUserCode)
            {
                using (var sourceWriter = new StringWriter())
                {
                    foreach (CodeTypeDeclaration type in codeUnit.Namespaces[0].Types)
                    {
                        if (!type.IsClass)
                        {
                            continue;
                        }
                        var items = new List<CodeTypeMember>();
                        foreach (CodeTypeMember codeMember in type.Members)
                        {
                            var property = codeMember as CodeMemberProperty;
                            if (property == null)
                            {
                                items.Add(codeMember);
                                continue;
                            }
                            items.Add(AddPropertyAttributes(provider, options, sourceWriter, property));
                        }
                        type.Members.Clear();
                        type.Members.AddRange(items.ToArray());
                    }
                }
            }

            foreach (var member in from CodeTypeDeclaration type in codeUnit.Namespaces[0].Types
                                    where type.IsClass
                                    from dynamic member in type.Members
                                    select member )
            {
                AddCodeAttributeDeclaration(member);
            }
        }

        private HashSet<string> GetObsoleteAttributes(IServiceProvider services)
        {
            var metadata = services.GetService<IMetadataProviderService>().LoadMetadata(services);
            if (!ObsoleteDeprecated)
            {
                return new HashSet<string>();
            }

            var obsoleteMatches = new TextMatcher(ObsoleteTokens);
            var concurrentObsoleteAttributes = new System.Collections.Concurrent.ConcurrentBag<string>();

            System.Threading.Tasks.Parallel.ForEach(metadata.Entities, entity =>
            {
                foreach (var attribute in entity.Attributes.Where(a => obsoleteMatches.HasMatch(a.DisplayName.GetLocalOrDefaultText())))
                {
                    concurrentObsoleteAttributes.Add(entity.LogicalName + "." + attribute.LogicalName);
                }
            });

            return new HashSet<string>(concurrentObsoleteAttributes);
        }

        private CodeTypeMember AddPropertyAttributes(CodeDomProvider provider, CodeGeneratorOptions options, StringWriter sourceWriter, CodeMemberProperty property)
        {
            provider.GenerateCodeFromMember(property, sourceWriter, options);
            var code = sourceWriter.ToString();
            sourceWriter.GetStringBuilder().Clear(); // Clear String Builder
            var lines = code.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
            lines.RemoveAt(0);
            lines.RemoveAt(lines.Count - 1);
            for (var i = lines.Count - 1; i >= 0; i--)
            {
                var line = lines[i];
                lines[i] = "\t\t" + line;
                if (line.TrimStart() == "get" || line.TrimStart() == "set")
                {
                    //Insert attribute above
                    if (AddDebuggerNonUserCode)
                    { 
                        lines.Insert(i, "\t\t\t[System.Diagnostics.DebuggerNonUserCode()]");
                    }
                }
            }

            return new CodeSnippetTypeMember(string.Join(Environment.NewLine, lines.ToArray()));
        }

        // ReSharper disable once UnusedParameter.Local
        private void AddCodeAttributeDeclaration(CodeTypeMember member)
        {
            // Default is do nothing
        }

        private void AddCodeAttributeDeclaration(CodeMemberProperty member)
        {
            member.CustomAttributes.Add(new CodeAttributeDeclaration("System.Diagnostics.DebuggerNonUserCode"));
        }

        private void AddCodeAttributeDeclaration(CodeMemberMethod member)
        {
            member.CustomAttributes.Add(new CodeAttributeDeclaration("System.Diagnostics.DebuggerNonUserCode"));
        }
    }
}
