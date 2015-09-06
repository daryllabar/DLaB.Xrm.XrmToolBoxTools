using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Crm.Services.Utility;
using System.Reflection;
using System.IO;

namespace DLaB.CrmSvcUtilExtensions.Entity
{
    public class AttributeConstGenerator : ICustomizeCodeDomService
    {
        public static string AttributeConstsStructName { get { return ConfigHelper.GetAppSettingOrDefault("AttributeConstsStructName", "Fields"); } }

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            var types = codeUnit.Namespaces[0].Types;
            var attributes = new HashSet<string>();
            foreach (var type in types.Cast<CodeTypeDeclaration>().
                                 Where(type => type.IsClass && !type.IsContextType()))
            {
                attributes.Clear();
                var @struct = new CodeTypeDeclaration {
                    Name = AttributeConstsStructName, 
                    IsStruct = true, 
                    TypeAttributes = TypeAttributes.Public
                };

                foreach (var member in from CodeTypeMember member in type.Members 
                                       let prop = member as CodeMemberProperty 
                                       where prop != null 
                                       select prop)
                {
                    CreateAttributeConstForProperty(@struct, member, attributes);
                }

                if (attributes.Any())
                {
                    type.Members.Insert(0, GenerateTypeWithoutEmptyLines(@struct));
                }
            }
        }

        private static void CreateAttributeConstForProperty(CodeTypeDeclaration type, CodeMemberProperty prop, HashSet<string> attributes)
        {
            var attributeLogicalName = (from CodeAttributeDeclaration att in prop.CustomAttributes
                where att.AttributeType.BaseType == "Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute"
                select ((CodePrimitiveExpression) att.Arguments[0].Value).Value.ToString()).FirstOrDefault();

            if (attributes.Contains(prop.Name) || attributeLogicalName == null) return;

            attributes.Add(prop.Name);
            type.Members.Add(new CodeMemberField
            {
                // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
                Attributes = System.CodeDom.MemberAttributes.Public | System.CodeDom.MemberAttributes.Const,
                Name = prop.Name,
                Type = new CodeTypeReference(typeof (String)),
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
                provider.GenerateCodeFromType(type, tabbedWriter, new CodeGeneratorOptions()
                {
                    BracingStyle = "C",
                    IndentString = "\t",
                    BlankLinesBetweenMembers = false
                });
                return new CodeSnippetTypeMember("\t\t" + sourceWriter);
            }
        }
    }
}