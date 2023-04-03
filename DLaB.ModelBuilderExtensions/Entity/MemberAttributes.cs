using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;

namespace DLaB.ModelBuilderExtensions.Entity
{
    /// <summary>
    /// Adds the System.Diagnostics.AddDebuggerNonUserCode Attribute
    /// </summary>
    public class MemberAttributes : ICustomizeCodeDomService
    {
        #region ICustomizeCodeDomService Members

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            var provider = CodeDomProvider.CreateProvider("CSharp");
            var options = new CodeGeneratorOptions()
            {
                BracingStyle = "C",
                IndentString = "\t",
            };
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
                        items.Add(new CodeSnippetTypeMember(GetPropertyTextWithGetSetLevelDebuggerNonUserCodeAttribute(provider, options, sourceWriter, property)));
                    }
                    type.Members.Clear();
                    type.Members.AddRange(items.ToArray());
                }
            }

            foreach (var member in from CodeTypeDeclaration type in codeUnit.Namespaces[0].Types 
                                   where type.IsClass 
                                    from dynamic member in type.Members 
                                    select member)
            {
                AddCodeAttributeDeclaration(member);
            }
        }

        private static string GetPropertyTextWithGetSetLevelDebuggerNonUserCodeAttribute(CodeDomProvider provider, CodeGeneratorOptions options, StringWriter sourceWriter, CodeMemberProperty property)
        {
            provider.GenerateCodeFromMember(property, sourceWriter, options);
            var code = sourceWriter.ToString();
            sourceWriter.GetStringBuilder().Clear(); // Clear String Builder
            var lines = code.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
            lines.RemoveAt(0);
            lines.RemoveAt(lines.Count - 1);
            for (var i = lines.Count() - 1; i >= 0; i--)
            {
                var line = lines[i];
                lines[i] = "\t\t" + line;
                if (line.TrimStart() == "get" || line.TrimStart() == "set")
                {
                    //Insert attribute above
                    lines.Insert(i, "\t\t\t[System.Diagnostics.DebuggerNonUserCode()]");
                }
            }

            return string.Join(Environment.NewLine, lines.ToArray());
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

        #endregion
    }
}
