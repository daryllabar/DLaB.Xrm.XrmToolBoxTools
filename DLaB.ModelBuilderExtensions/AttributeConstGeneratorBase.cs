using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DLaB.ModelBuilderExtensions
{
    public abstract class AttributeConstGeneratorBase : TypedServiceSettings<ICustomizeCodeDomService>, ICustomizeCodeDomService
    {
        protected const string RemovalString = "9C8F3879-309D-4DB2-B138-3F2E3A462A1C";
        protected const string OobConstsClassName = "Fields";

        public virtual string AttributeConstsClassName { get => DLaBSettings.AttributeConstsClassName; set => DLaBSettings.AttributeConstsClassName = value; }
        public abstract bool GenerateAttributeNameConsts { get; set; }
        public virtual int InsertIndex { get; }

        protected AttributeConstGeneratorBase(ICustomizeCodeDomService defaultService, IDictionary<string, string> parameters) : base(defaultService, parameters)
        {

        }

        protected AttributeConstGeneratorBase(ICustomizeCodeDomService defaultService, DLaBModelBuilderSettings settings = null) : base(defaultService, settings)
        {

        }

        public virtual void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            if (!GenerateAttributeNameConsts)
            {
                return;
            }

            var attributes = new HashSet<string>();
            foreach (var type in codeUnit.GetMessageTypes())
            {
                attributes.Clear();
                var @class = new CodeTypeDeclaration
                {
                    Name = AttributeConstsClassName,
                    IsClass = true,
                    TypeAttributes = TypeAttributes.Public
                };

                AddNonPropertyValues(@class, type, attributes);
                foreach (var member in from CodeTypeMember member in type.Members
                                       let prop = member as CodeMemberProperty
                                       where prop != null
                                       select prop)
                {
                    CreateAttributeConstForProperty(@class, member, attributes);
                }

                if (attributes.Any())
                {
                    type.Members.Insert(InsertIndex, GenerateTypeWithoutEmptyLines(@class));
                }
            }
        }

        protected virtual void AddNonPropertyValues(CodeTypeDeclaration constantsClass, CodeTypeDeclaration type, HashSet<string> attributes)
        {
            // None
        }

        protected virtual string GetAttributeLogicalName(CodeMemberProperty prop)
        {
	        foreach (CodeStatement statement in prop.SetStatements)
	        {
		        if (statement is CodeAssignStatement assignStatement
			        && assignStatement.Left is CodeIndexerExpression indexerExpression)
		        {
			        foreach (CodeExpression index in indexerExpression.Indices)
			        {
				        if (index is CodePrimitiveExpression primitiveExpression)
				        {
					        return primitiveExpression.Value?.ToString();
				        }
			        }
		        }
	        }

	        // Fallback to property name if structure doesn't match expected pattern
	        return prop.Name;
        }
        private void CreateAttributeConstForProperty(CodeTypeDeclaration type, CodeMemberProperty prop, HashSet<string> attributes)
        {
            AddAttributeConstIfNotExists(type, prop.Name, GetAttributeLogicalName(prop), attributes);
        }

        protected int AddAttributeConstIfNotExists(CodeTypeDeclaration type, string name, string attributeLogicalName, HashSet<string> attributes)
        {
            if (attributeLogicalName == null)
            {
                return -1;
            }

            // Handle Removal of characters as specified by the attribute logical name (used for N:N relationships
            if (attributeLogicalName.Contains(RemovalString))
            {
                var parts = attributeLogicalName.Split(new[] { RemovalString }, StringSplitOptions.None);
                attributeLogicalName = parts[1];
                name = name.Substring(parts[0].Length);
            }

            if (attributes.Contains(name))
                return -1;

            attributes.Add(name);
            return type.Members.Add(new CodeMemberField
            {
                // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
                Attributes = MemberAttributes.Public | MemberAttributes.Const,
                Name = name,
                Type = new CodeTypeReference(typeof(string)),
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