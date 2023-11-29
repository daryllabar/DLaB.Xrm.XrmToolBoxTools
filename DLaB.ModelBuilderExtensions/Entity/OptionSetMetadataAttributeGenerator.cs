using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeDomMemberAttributes = System.CodeDom.MemberAttributes;

namespace DLaB.ModelBuilderExtensions.Entity
{
    /// <summary>
    /// TODO: This really should be changed so the code generator service generates this code into a separate file, rather than adding it to the namespace via the CustomizeCodeDomService
    /// </summary>
    public class OptionSetMetadataAttributeGenerator : ICustomizeCodeDomService
    {
        #region ICustomizeCodeDomService Members

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            var types = codeUnit.Namespaces[0].Types;
            types.Add(CreateOptionSetMetadataAttributeClass());
            types.Add(CreateOptionSetExtensionClass());
        }

        #endregion

        #region OptionSetMetadataAttribute Class Generation

        /*
        [System.AttributeUsage(System.AttributeTargets.Field)]
        public class OptionSetMetadataAttribute : System.Attribute
        {
	        public string Color { get; set; }
	        public string Description { get; set; }
            public System.Collections.Generic.Dictionary<int, string> Namess { get; set; }
	        public int ExternalValue { get; set; }
	        public int Index { get; set; }
	        public string Name { get; set; }

	        public OptionSetMetadataAttribute(string name, int displayIndex, string color = null, string description = null, string externalValue = null, System.Collections.Generic.Dictionary<int, string> names = null)
	        {
		        this.Color = color;
                this.Description = description;
                this._nameObjects = names
                this.ExternalValue = externalValue;
                this.DisplayIndex = displayIndex;
                this.Name = name;
	        }
        }

        */
        private static CodeTypeDeclaration CreateOptionSetMetadataAttributeClass()
        {
            var attributeClass = new CodeTypeDeclaration("OptionSetMetadataAttribute")
            {
                CustomAttributes = CreateOptionSetMetdataCustomAttributes(),
                IsClass = true,
                TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed
            };
            attributeClass.Comments.AddRange(CreateOptionSetMetadataClassComments());
            attributeClass.BaseTypes.Add(new CodeTypeReference(typeof(Attribute))); // : System.Attribute

            attributeClass.Members.Add(new CodeMemberField(typeof(object[]), "_nameObjects"));
            attributeClass.Members.Add(new CodeMemberField(typeof(Dictionary<int,string>), "_names"));
            attributeClass.Members.AddRange(CreateOptionSetMetadataProperties());
            //attributeClass.Members.Add(CreateOptionSetMetadataConstructor(attributeClass.Name, true));
            attributeClass.Members.Add(CreateOptionSetMetadataConstructor(attributeClass.Name, false));
            attributeClass.Members.Add(CreateNamesMethod());

            return attributeClass;
        }

        #region Class Declaration

        private static CodeAttributeDeclarationCollection CreateOptionSetMetdataCustomAttributes()
        {
            return new CodeAttributeDeclarationCollection // [System.AttributeUsage(System.AttributeTargets.Field)]
            {
                new CodeAttributeDeclaration(new CodeTypeReference(typeof(AttributeUsageAttribute)), new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(AttributeTargets)), "Field")))
            };
        }

        private static CodeCommentStatement[] CreateOptionSetMetadataClassComments()
        {
            return new[] {
                new CodeCommentStatement(@"<summary>", true),
                new CodeCommentStatement(@"Attribute to handle storing the OptionSet's Metadata.", true),
                new CodeCommentStatement(@"</summary>", true)};
        }

        private static CodeTypeMember[] CreateOptionSetMetadataProperties()
        {
            return Properties.OrderBy(p => p.PropertyName)
                             .Select(p =>
                                 new CodeSnippetTypeMember(p.GetPropertySnippet())
                                 {
                                     Comments =
                                     {
                                         new CodeCommentStatement("<summary>", true),
                                         new CodeCommentStatement($"{p.CommentName} of the OptionSetValue.", true),
                                         new CodeCommentStatement(@"</summary>", true),
                                     }
                                 }
                             ).Cast<CodeTypeMember>().ToArray();
        }

        #endregion Class Declaration

        #region Constructors

        private static CodeConstructor CreateOptionSetMetadataConstructor(string className, bool basicConstructor)
        {
            // basicConstructor == false
            // public OptionSetMetadataAttribute(string name, string description = null, string color = null): this(name, int.MinValue, int.MinValue, color, description) { }

            // basicConstructor == true
            // public OptionSetMetadataAttribute(string name, int index, int externalValue, string color = null, string description = null, params object[] names)
            // {
            //     Color = color;
            //     Description = description;
            //     ExternalValue = externalValue;
            //     Index = index;
            //     Name = name;
            //     _nameObjects = names;
            // }
            var constructor = new CodeConstructor
            {
                Attributes = CodeDomMemberAttributes.Public,
                Name = className
            };
            constructor.Comments.AddRange(GetConstructorComments(className, basicConstructor));
            constructor.Parameters.AddRange(GetConstructorParameters(basicConstructor));
            constructor.ChainedConstructorArgs.AddRange(GetConstructorChainedArgs(basicConstructor));
            constructor.Statements.AddRange(GetConstructorStatements(basicConstructor));

            return constructor;
        }

        private static CodeCommentStatement[] GetConstructorComments(string className, bool basicConstructor)
        {
            var paramComments = new List<CodeCommentStatement>
            {
                new CodeCommentStatement(@"<summary>", true),
                new CodeCommentStatement($@"Initializes a new instance of the <see cref=""{className}""/> class.", true),
                new CodeCommentStatement(@"</summary>", true)
            };
            paramComments.AddRange(
                Properties.Where(p => p.IsValidForConstructor(basicConstructor))
                          .OrderBy(p => p.ParameterIndex(basicConstructor))
                          .Select(p => new CodeCommentStatement($@"<param name=""{p.VariableName}"">{p.CommentName} of the value.</param>", true)));

            return paramComments.ToArray();
        }

        private static CodeParameterDeclarationExpression[] GetConstructorParameters(bool basicConstructor)
        {
            return Properties.Where(p => p.IsValidForConstructor(basicConstructor))
                             .OrderBy(p => p.ParameterIndex(basicConstructor))
                             .Select(p => p.GetParameterDeclaration()).ToArray();
        }

        private static CodeExpression[] GetConstructorChainedArgs(bool basicConstructor)
        {
            if (!basicConstructor)
            {
                return Array.Empty<CodeExpression>();
            }

            return Properties.Where(p => p.IsValidForConstructor(false))
                             .OrderBy(p => p.FullConstructorIndex)
                             .Select(p => p.Type == typeof(int) && p.BasicConstructorIndex < 0
                                 ? (CodeExpression) new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(int)), "MinValue")
                                 : new CodeVariableReferenceExpression(p.VariableName)).ToArray();
        }
                                 

        private static CodeStatement[] GetConstructorStatements(bool basicConstructor)
        {
            if (basicConstructor)
            {
                return Array.Empty<CodeStatement>();
            }

            return Properties.Select(p => p.GetConstructorAssignment()).ToArray();
        }

        #endregion Constructors

        private static CodeMemberMethod CreateNamesMethod()
        {
            var method = new CodeMemberMethod
            {
                Name = "CreateNames",
                ReturnType = new CodeTypeReference(typeof(Dictionary<int, string>))
            };

            var namesVariable = new CodeVariableDeclarationStatement(typeof(Dictionary<int, string>), "names")
                {
                    InitExpression = new CodeObjectCreateExpression(typeof(Dictionary<int, string>))
                };

            var loop = new CodeIterationStatement
            {
                InitStatement = new CodeVariableDeclarationStatement(typeof(int), "i", new CodePrimitiveExpression(0)),
                TestExpression = new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"), CodeBinaryOperatorType.LessThan, new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("_nameObjects"), "Length")),
                IncrementStatement = new CodeAssignStatement(new CodeVariableReferenceExpression("i"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(2)))
            };

            var addStatement = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("names"), "Add");
            addStatement.Parameters.Add(new CodeCastExpression(typeof(int), new CodeArrayIndexerExpression(new CodeVariableReferenceExpression("_nameObjects"), new CodeVariableReferenceExpression("i"))));
            addStatement.Parameters.Add(new CodeCastExpression(typeof(string), new CodeArrayIndexerExpression(new CodeVariableReferenceExpression("_nameObjects"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1)))));

            loop.Statements.Add(addStatement);

            method.Statements.Add(namesVariable);
            method.Statements.Add(loop);
            method.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("names")));

            return method;
        }

        private static readonly List<PropertyInfo> Properties = new List<PropertyInfo>
        {
            new PropertyInfo(2, 2, typeof(string), "Color"),
            new PropertyInfo(3, 3, typeof(string), "Description"),
            new PropertyInfo(-1, 5, typeof(object[]), "Names")
            {
                CustomConstructorAssignmentName = "_nameObjects",
                CustomPropertySnippet = @"		public System.Collections.Generic.Dictionary<int, string> Names
		{
			get
			{
				return _names ?? (_names = CreateNames());
			} 
			set
			{
				_names = value;
				if (value == null)
				{
				    _nameObjects = new object[0];
				}
				else
				{
				    _nameObjects = null;
				}
			}
		}",
                IsParamsParameter = true
            },
            new PropertyInfo(-1, 4, typeof(string), "ExternalValue", "External value"),
            new PropertyInfo(1, 1, typeof(int), "DisplayIndex", "Display order index"),
            new PropertyInfo(0, 0, typeof(string), "Name"),
        };

        private class PropertyInfo
        {
            // ReSharper disable MemberCanBePrivate.Local
            // ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
            public string PropertyName { get; set; }
            public bool IsParamsParameter { get; set; }
            public string VariableName { get; private set; }
            public string VariableNameDeclaration { get {
                if (PropertyName == "Name"
                    || PropertyName == "Names"
                    || Type == typeof(int) )
                {
                    return VariableName;
                }

                return VariableName + " = null";
            }}
            public Type Type { get; set; }
            public string CommentName { get; set; }
            public int BasicConstructorIndex { get; set; }
            public int FullConstructorIndex { get; set; }
            public string CustomPropertySnippet { get; set; }
            public string CustomConstructorAssignmentName { get; set; }

            public string TypeDisplayName => GetTypeDisplayName(Type);
            // ReSharper restore AutoPropertyCanBeMadeGetOnly.Local
            // ReSharper restore MemberCanBePrivate.Local

            public PropertyInfo(int basicConstructorIndex, int fullConstructorIndex, Type type, string propertyName, string commentName = null)
            {
                BasicConstructorIndex = basicConstructorIndex;
                FullConstructorIndex = fullConstructorIndex;
                Type = type;
                PropertyName = propertyName;
                VariableName = PropertyName[0].ToString().ToLower() + PropertyName.Substring(1);
                CommentName = commentName;
                if (string.IsNullOrWhiteSpace(CommentName))
                {
                    CommentName = PropertyName;
                }
            }

            public CodeParameterDeclarationExpression GetParameterDeclaration()
            {
                var declaration = new CodeParameterDeclarationExpression(Type, VariableNameDeclaration);
                if (IsParamsParameter)
                {
                    declaration.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(ParamArrayAttribute))));
                }
                return declaration;
            }

            public string GetPropertySnippet()
            {
                return string.IsNullOrWhiteSpace(CustomPropertySnippet) 
                    ? $@"{"\t\t"}public {TypeDisplayName} {PropertyName} {{ get; set; }}"
                    : CustomPropertySnippet;
            }

            public bool IsValidForConstructor(bool basicConstructor)
            {
                return !basicConstructor || BasicConstructorIndex >= 0;
            }

            public int ParameterIndex(bool basicConstructor)
            {
                return basicConstructor 
                       ? BasicConstructorIndex
                       : FullConstructorIndex;
            }

            private string GetTypeDisplayName(Type type)
            {
                var nonGenericTypeName = type == typeof(int)
                    ? "int"
                    : type.Name.ToLower();
                return type.IsGenericType
                    ? $"{type.Namespace}.{type.Name.Split('`')[0]}<{string.Join(", ", type.GenericTypeArguments.Select(GetTypeDisplayName))}>"
                    : nonGenericTypeName;
            }

            public CodeStatement GetConstructorAssignment()
            {
                var name = string.IsNullOrWhiteSpace(CustomConstructorAssignmentName)
                    ? PropertyName
                    : CustomConstructorAssignmentName;

                return new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), name), new CodeVariableReferenceExpression(VariableName));
            }
        }

        #endregion OptionSetMetadataAttribute Class Generation

        #region OptionSetExtension Class Generation

        /*
        public static class OptionSetExtension
        {
            public static OptionSetMetadataAttribute GetMetadata<T>(this T value) where T : struct, System.IConvertible
            {
                System.Type enumType = typeof(T);
                if (!typeof(T).IsEnum)
                {
                    throw new System.ArgumentException("T must be an enum!");
                }

                System.Reflection.MemberInfo[] members = enumType.GetMember(value.ToString());

		        for (var i=0; i < members.Length; i++)
                {
                    var attribute = System.Reflection.CustomAttributeExtensions.GetCustomAttribute(enumType, typeof(OptionSetMetadataAttribute));
                    if (attribute != null)
                    {
                        return (OptionSetMetadataAttribute)attribute;
                    }
                }
                throw new System.ArgumentException("T must be an enum adorned with an OptionSetMetadataAttribute!");
            }
        }
         */

        private static CodeTypeDeclaration CreateOptionSetExtensionClass()
        {
            var extClass = new CodeTypeDeclaration("OptionSetExtension")
            {
                IsClass = true,
                Attributes = CodeDomMemberAttributes.Public
            };
            extClass.Comments.AddRange(CreateOptionSetExtensionClassComments());
            extClass.Members.Add(CreateOptionSetExtensionGetMetadataMethod());

            return extClass;
        }

        private static CodeCommentStatement[] CreateOptionSetExtensionClassComments()
        {
            return new[] {
                new CodeCommentStatement(@"<summary>", true),
                new CodeCommentStatement(@"Extension class to handle retrieving of OptionSetMetadataAttribute.", true),
                new CodeCommentStatement(@"</summary>", true)};
        }

        public static CodeMemberMethod CreateOptionSetExtensionGetMetadataMethod()
        {
            // public static int? GetEnum(Microsoft.Xrm.Sdk.Entity entity, string attributeLogicalName)
            var method = new CodeMemberMethod
            {
                Name = "GetMetadata",
                ReturnType = new CodeTypeReference("OptionSetMetadataAttribute"),
                // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
                Attributes = CodeDomMemberAttributes.Public | CodeDomMemberAttributes.Static,
            };
            method.Comments.AddRange(CreateGetMetadataMethodComments());
            method.TypeParameters.Add(new CodeTypeParameter("T")
            {
                Constraints =
                {
                    " struct",
                    new CodeTypeReference(typeof(IConvertible))
                }
            });
            method.Parameters.Add(new CodeParameterDeclarationExpression("this T", "value"));
            method.Statements.AddRange(new CodeStatementCollection{ 
                GetEnumTypeInitializer(),
                GetThrowIfNotEnumCheck(),
                GetMemberAttributeInitializer(),
                GetReturnAttributeLoop(),
                GetFinalThrowStatement()
            });

            return method;
        }

        private static CodeCommentStatement[] CreateGetMetadataMethodComments()
        {
            return new[]
            {
                new CodeCommentStatement(@"<summary>", true),
                new CodeCommentStatement(@"Returns the OptionSetMetadataAttribute for the given enum value", true),
                new CodeCommentStatement(@"</summary>", true),
                new CodeCommentStatement(@"<typeparam name=""T"">OptionSet Enum Type</typeparam>", true),
                new CodeCommentStatement(@"<param name=""value"">Enum Value with OptionSetMetadataAttribute</param>", true)
            };
        }


        private static CodeVariableDeclarationStatement GetEnumTypeInitializer()
        {
            // System.Type enumType = typeof(T);
            return new CodeVariableDeclarationStatement
            {
                Type = new CodeTypeReference(typeof(Type)),
                Name = "enumType",
                InitExpression = new CodeTypeOfExpression(new CodeTypeReference("T"))
            };
        }

        private static CodeConditionStatement GetThrowIfNotEnumCheck()
        {
            // if (!enumType.IsEnum)
            // {
            //     throw new System.ArgumentException("T must be an enum!");
            // }
            return new CodeConditionStatement(new CodeSnippetExpression("!enumType.IsEnum"),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new CodeTypeReference(typeof(ArgumentException)), new CodePrimitiveExpression("T must be an enum!"))));
        }

        private static CodeVariableDeclarationStatement GetMemberAttributeInitializer()
        {
            // System.Reflection.MemberInfo[] members = enumType.GetMember(value.ToString());
            var valueToStringCall = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("value"), "ToString");
            return new CodeVariableDeclarationStatement
            {
                Type = new CodeTypeReference(typeof(MemberInfo[])),
                Name = "members",
                InitExpression = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("enumType"), "GetMember", valueToStringCall)
            };
        }

        private static CodeIterationStatement GetReturnAttributeLoop()
        {
            /*
            for (var i=0; i < members.Length; i++)
            {
                var attribute = System.Reflection.CustomAttributeExtensions.GetCustomAttribute(enumType, typeof(OptionSetMetadataAttribute));
                if (attribute != null)struct
                {
                    return (OptionSetMetadataAttribute)attribute;
                }
            }
             */
            var loopInit = new CodeVariableDeclarationStatement
            {
                Type = new CodeTypeReference(typeof(int)),
                Name = "i",
                InitExpression = new CodePrimitiveExpression(0)
            };

            var loopTest = new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"),
                CodeBinaryOperatorType.LessThan, new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("members"), "Length"));
            var loop = new CodeIterationStatement(loopInit, loopTest, new CodeSnippetStatement("i++"),
                new CodeVariableDeclarationStatement
                {
                    Type = new CodeTypeReference(typeof(Attribute)),
                    Name = "attribute",
                    InitExpression = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(CustomAttributeExtensions)), "GetCustomAttribute", new CodeVariableReferenceExpression("members[i]"), new CodeTypeOfExpression(new CodeTypeReference("OptionSetMetadataAttribute")))
                },
                new CodeConditionStatement(new CodeSnippetExpression("attribute != null"),
                    new CodeMethodReturnStatement(new CodeCastExpression(new CodeTypeReference("OptionSetMetadataAttribute"), new CodeVariableReferenceExpression("attribute"))))
            );
            return loop;
        }

        private static CodeThrowExceptionStatement GetFinalThrowStatement()
        {
            // throw new System.ArgumentException("T must be an enum adorned with an OptionSetMetadataAttribute!");
            return new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new CodeTypeReference(typeof(ArgumentException)), new CodePrimitiveExpression("T must be an enum adorned with an OptionSetMetadataAttribute!")));
        }

        #endregion OptionSetExtension Class Generation
    }
}
