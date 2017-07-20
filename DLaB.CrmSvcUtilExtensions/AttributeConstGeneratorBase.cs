using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Crm.Services.Utility;
using System.Reflection;
using System.IO;

namespace DLaB.CrmSvcUtilExtensions
{
    public abstract class AttributeConstGeneratorBase : ICustomizeCodeDomService
    {
        public static string AttributeConstsClassName => ConfigHelper.GetAppSettingOrDefault("AttributeConstsClassName", "Fields");

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)   
        {
            var types = codeUnit.Namespaces[0].Types;
            var attributes = new HashSet<string>();
            foreach (var type in types.Cast<CodeTypeDeclaration>().
                                 Where(type => type.IsClass && !type.IsContextType()))
            {
                attributes.Clear();
                var @class = new CodeTypeDeclaration {
                    Name = AttributeConstsClassName, 
                    IsClass = true,
                    TypeAttributes = TypeAttributes.Public
                };

                foreach (var member in from CodeTypeMember member in type.Members 
                                       let prop = member as CodeMemberProperty 
                                       where prop != null 
                                       select prop)
                {
                    CreateAttributeConstForProperty(@class, member, attributes);
                }

                if (attributes.Any())
                {
                    type.Members.Insert(0, GenerateTypeWithoutEmptyLines(@class));
                }
            }
        }

        /// <summary>
        /// Gets the name of the attribute logical.
        /// </summary>
        /// <returns></returns>
        protected abstract string GetAttributeLogicalName(CodeMemberProperty prop);

        private void CreateAttributeConstForProperty(CodeTypeDeclaration type, CodeMemberProperty prop, HashSet<string> attributes)
        {
            var attributeLogicalName = GetAttributeLogicalName(prop);
            if (attributes.Contains(prop.Name) || attributeLogicalName == null) return;

            attributes.Add(prop.Name);
            type.Members.Add(new CodeMemberField
            {
                // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
                Attributes = MemberAttributes.Public | MemberAttributes.Const,
                Name = prop.Name,
                Type = new CodeTypeReference(typeof (string)),
                InitExpression = new CodePrimitiveExpression(attributeLogicalName)
            });
        }

        /// <summary>
        /// Removes the blank lines spaces by generating the code as a string without BlankLinesBetweenMembers
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private static CodeSnippetTypeMember GenerateTypeWithoutEmptyLines(CodeTypeDeclaration type)
        {
            var provider = CodeDomProvider.CreateProvider("CSharp");
            using (var sourceWriter = new StringWriter())
            using (var tabbedWriter = new IndentedTextWriter(sourceWriter, "\t"))
            {
                tabbedWriter.Indent = 2;
                provider.GenerateCodeFromType(type, tabbedWriter, new CodeGeneratorOptions
                {
                    BracingStyle = "C",
                    IndentString = "\t",
                    BlankLinesBetweenMembers = false
                });
                var stringSource = sourceWriter.ToString().Replace("public class", "public static class");
                var lastNewLine = stringSource.LastIndexOf(Environment.NewLine, StringComparison.Ordinal);
                if (lastNewLine >= 0)
                {
                    stringSource = stringSource.Remove(lastNewLine);
                }
                return new CodeSnippetTypeMember("\t\t" + stringSource);
            }
        }
    }
}