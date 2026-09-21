using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using System;
using System.CodeDom;
using System.Reflection;
using CodeDomMemberAttributes = System.CodeDom.MemberAttributes;

namespace DLaB.ModelBuilderExtensions.Entity
{
    /// <summary>
    /// Generates the DateTimeMetadataAttribute, it's enums, and it's extension class, used to define the Format and
    /// TimeZone Adjustment (behavior) of DateTime properties.
    /// TODO: This really should be changed so the code generator service generates this code into a separate file, rather than adding it to the namespace via the CustomizeCodeDomService
    /// </summary>
    public class DateTimeMetadataAttributeGenerator : ICustomizeCodeDomService
    {
        public const string AttributeClassName = "DateTimeMetadataAttribute";
        public const string ExtensionClassName = "DateTimeMetadataAttributeExtensions";
        public const string FormatEnumName = "DateTimeFormat";
        public const string TimeZoneEnumName = "DateTimeTimeZoneAdjustment";

        private readonly bool _makeReferenceTypesNullable;

        public DateTimeMetadataAttributeGenerator(bool makeReferenceTypesNullable = false)
        {
            _makeReferenceTypesNullable = makeReferenceTypesNullable;
        }

        #region ICustomizeCodeDomService Members

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            var types = codeUnit.Namespaces[0].Types;
            types.Add(CreateFormatEnum());
            types.Add(CreateTimeZoneEnum());
            types.Add(CreateDateTimeMetadataAttributeClass());
            types.Add(CreateExtensionClass());
        }

        #endregion

        #region Enum Generation

        /*
        /// <summary>
        /// The format of the DateTime column.
        /// </summary>
        public enum DateTimeFormat
        {
            DateAndTime = 0,
            DateOnly = 1,
        }
        */
        private static CodeTypeDeclaration CreateFormatEnum()
        {
            var formatEnum = new CodeTypeDeclaration(FormatEnumName)
            {
                IsEnum = true,
                TypeAttributes = TypeAttributes.Public
            };
            formatEnum.Comments.AddRange(CreateComments("The format of the DateTime column as defined in Dataverse."));
            formatEnum.Members.Add(CreateEnumValue("DateAndTime", 0, "The column contains both a date and a time."));
            formatEnum.Members.Add(CreateEnumValue("DateOnly", 1, "The column contains only a date."));
            return formatEnum;
        }

        /*
        /// <summary>
        /// The time zone adjustment (behavior) of the DateTime column.
        /// </summary>
        public enum DateTimeTimeZoneAdjustment
        {
            UserLocal = 0,
            TimeZoneIndependent = 1,
            DateOnly = 2,
        }
        */
        private static CodeTypeDeclaration CreateTimeZoneEnum()
        {
            var timeZoneEnum = new CodeTypeDeclaration(TimeZoneEnumName)
            {
                IsEnum = true,
                TypeAttributes = TypeAttributes.Public
            };
            timeZoneEnum.Comments.AddRange(CreateComments("The time zone adjustment (behavior) of the DateTime column as defined in Dataverse."));
            timeZoneEnum.Members.Add(CreateEnumValue("UserLocal", 0, "The value is stored in UTC and displayed in the time zone of the user."));
            timeZoneEnum.Members.Add(CreateEnumValue("TimeZoneIndependent", 1, "The value is stored and displayed the same, independent of the time zone of the user."));
            timeZoneEnum.Members.Add(CreateEnumValue("DateOnly", 2, "The time portion of the value is not stored, and the date is displayed the same, independent of the time zone of the user."));
            return timeZoneEnum;
        }

        private static CodeMemberField CreateEnumValue(string name, int value, string comment)
        {
            var field = new CodeMemberField(typeof(int), name)
            {
                InitExpression = new CodePrimitiveExpression(value)
            };
            field.Comments.AddRange(CreateComments(comment));
            return field;
        }

        #endregion Enum Generation

        #region DateTimeMetadataAttribute Class Generation

        /*
        /// <summary>
        /// Attribute to handle storing the DateTime column's Metadata.
        /// </summary>
        [System.AttributeUsage(System.AttributeTargets.Property)]
        public sealed class DateTimeMetadataAttribute : System.Attribute
        {
            public DateTimeFormat Format { get; set; }
            public DateTimeTimeZoneAdjustment TimeZone { get; set; }

            public DateTimeMetadataAttribute(DateTimeFormat format, DateTimeTimeZoneAdjustment timeZone)
            {
                this.Format = format;
                this.TimeZone = timeZone;
            }
        }
        */
        private static CodeTypeDeclaration CreateDateTimeMetadataAttributeClass()
        {
            var attributeClass = new CodeTypeDeclaration(AttributeClassName)
            {
                CustomAttributes = new CodeAttributeDeclarationCollection // [System.AttributeUsage(System.AttributeTargets.Property)]
                {
                    new CodeAttributeDeclaration(new CodeTypeReference(typeof(AttributeUsageAttribute)), new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(AttributeTargets)), "Property")))
                },
                IsClass = true,
                TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed
            };
            attributeClass.Comments.AddRange(CreateComments("Attribute to handle storing the DateTime column's Metadata."));
            attributeClass.BaseTypes.Add(new CodeTypeReference(typeof(Attribute))); // : System.Attribute

            attributeClass.Members.Add(CreateAutoProperty(FormatEnumName, "Format", "Format of the DateTime column."));
            attributeClass.Members.Add(CreateAutoProperty(TimeZoneEnumName, "TimeZone", "Time zone adjustment (behavior) of the DateTime column."));
            attributeClass.Members.Add(CreateConstructor());

            return attributeClass;
        }

        private static CodeSnippetTypeMember CreateAutoProperty(string type, string name, string comment)
        {
            var property = new CodeSnippetTypeMember($"\t\tpublic {type} {name} {{ get; set; }}");
            property.Comments.AddRange(CreateComments(comment));
            return property;
        }

        private static CodeConstructor CreateConstructor()
        {
            var constructor = new CodeConstructor
            {
                Attributes = CodeDomMemberAttributes.Public,
                Name = AttributeClassName
            };
            constructor.Comments.AddRange(CreateComments($@"Initializes a new instance of the <see cref=""{AttributeClassName}""/> class."));
            constructor.Comments.Add(new CodeCommentStatement(@"<param name=""format"">Format of the DateTime column.</param>", true));
            constructor.Comments.Add(new CodeCommentStatement(@"<param name=""timeZone"">Time zone adjustment (behavior) of the DateTime column.</param>", true));
            constructor.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(FormatEnumName), "format"));
            constructor.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(TimeZoneEnumName), "timeZone"));
            constructor.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "Format"), new CodeVariableReferenceExpression("format")));
            constructor.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "TimeZone"), new CodeVariableReferenceExpression("timeZone")));

            return constructor;
        }

        #endregion DateTimeMetadataAttribute Class Generation

        #region DateTimeMetadataAttributeExtensions Class Generation

        /*
        /// <summary>
        /// Extension class to handle retrieving of the DateTimeMetadataAttribute.
        /// </summary>
        public static class DateTimeMetadataAttributeExtensions
        {
            public static DateTimeMetadataAttribute GetMetadata<T>(this T entity, string propertyName) where T : Microsoft.Xrm.Sdk.Entity
            {
                System.Reflection.PropertyInfo property = typeof(T).GetProperty(propertyName);
                if (property == null)
                {
                    throw new System.ArgumentException("No property named " + propertyName + " was found!");
                }
                System.Attribute attribute = System.Reflection.CustomAttributeExtensions.GetCustomAttribute(property, typeof(DateTimeMetadataAttribute));
                if (attribute == null)
                {
                    throw new System.ArgumentException("Property " + propertyName + " is not adorned with a DateTimeMetadataAttribute!");
                }
                return (DateTimeMetadataAttribute)attribute;
            }
        }
        */
        private CodeTypeDeclaration CreateExtensionClass()
        {
            var extClass = new CodeTypeDeclaration(ExtensionClassName)
            {
                IsClass = true,
                Attributes = CodeDomMemberAttributes.Public
            };
            extClass.Comments.AddRange(CreateComments("Extension class to handle retrieving of the DateTimeMetadataAttribute."));
            extClass.Members.Add(CreateGetMetadataMethod());

            return extClass;
        }

        private CodeMemberMethod CreateGetMetadataMethod()
        {
            var method = new CodeMemberMethod
            {
                Name = "GetMetadata",
                ReturnType = new CodeTypeReference(AttributeClassName),
                // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
                Attributes = CodeDomMemberAttributes.Public | CodeDomMemberAttributes.Static
            };
            method.Comments.AddRange(CreateComments("Returns the DateTimeMetadataAttribute of the given DateTime property of the entity."));
            method.Comments.Add(new CodeCommentStatement(@"<typeparam name=""T"">Entity Type</typeparam>", true));
            method.Comments.Add(new CodeCommentStatement(@"<param name=""entity"">Entity containing the DateTime property</param>", true));
            method.Comments.Add(new CodeCommentStatement(@"<param name=""propertyName"">Name of the DateTime property adorned with a DateTimeMetadataAttribute</param>", true));
            method.TypeParameters.Add(new CodeTypeParameter("T")
            {
                Constraints =
                {
                    new CodeTypeReference(typeof(Microsoft.Xrm.Sdk.Entity))
                }
            });
            method.Parameters.Add(new CodeParameterDeclarationExpression("this T", "entity"));
            method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "propertyName"));

            method.Statements.Add(new CodeVariableDeclarationStatement
            {
                Type = _makeReferenceTypesNullable
                    ? new CodeTypeReference("System.Reflection.PropertyInfo?")
                    : new CodeTypeReference(typeof(PropertyInfo)),
                Name = "property",
                InitExpression = new CodeMethodInvokeExpression(new CodeTypeOfExpression(new CodeTypeReference("T")), "GetProperty", new CodeVariableReferenceExpression("propertyName"))
            });
            method.Statements.Add(new CodeConditionStatement(new CodeSnippetExpression("property == null"),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new CodeTypeReference(typeof(ArgumentException)),
                    new CodeSnippetExpression(@"""No property named "" + propertyName + "" was found on type "" + typeof(T).FullName + ""!""")))));
            method.Statements.Add(new CodeVariableDeclarationStatement
            {
                Type = _makeReferenceTypesNullable
                    ? new CodeTypeReference("System.Attribute?")
                    : new CodeTypeReference(typeof(Attribute)),
                Name = "attribute",
                InitExpression = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(CustomAttributeExtensions)), "GetCustomAttribute", new CodeVariableReferenceExpression("property"), new CodeTypeOfExpression(new CodeTypeReference(AttributeClassName)))
            });
            method.Statements.Add(new CodeConditionStatement(new CodeSnippetExpression("attribute == null"),
                new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new CodeTypeReference(typeof(ArgumentException)),
                    new CodeSnippetExpression($@"""Property "" + propertyName + "" of type "" + typeof(T).FullName + "" is not adorned with a {AttributeClassName}!""")))));
            method.Statements.Add(new CodeMethodReturnStatement(new CodeCastExpression(new CodeTypeReference(AttributeClassName), new CodeVariableReferenceExpression("attribute"))));

            return method;
        }

        #endregion DateTimeMetadataAttributeExtensions Class Generation

        private static CodeCommentStatement[] CreateComments(string summary)
        {
            return new[]
            {
                new CodeCommentStatement(@"<summary>", true),
                new CodeCommentStatement(summary, true),
                new CodeCommentStatement(@"</summary>", true)
            };
        }
    }
}
