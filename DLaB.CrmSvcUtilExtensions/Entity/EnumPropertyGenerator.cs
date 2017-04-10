using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DLaB.CrmSvcUtilExtensions.Entity
{
    class EnumPropertyGenerator : ICustomizeCodeDomService
    {
        public Dictionary<string, string> SpecifiedMappings { get; private set; }
        public Dictionary<string, HashSet<string>> UnmappedProperties { get; private set; }

        public INamingService NamingService { get; private set; }
        public IServiceProvider Services { get; private set; }

        #region ICustomizeCodeDomService Members

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            NamingService = new NamingService((INamingService)services.GetService(typeof(INamingService)));
            Services = services;
            InitializeMappings();
            var types = codeUnit.Namespaces[0].Types;
            foreach (CodeTypeDeclaration type in types)
            {
                if (!type.IsClass || type.IsContextType()) { continue; }

                var logicalName = type.GetFieldInitalizedValue("EntityLogicalName");
                var propertiesToAdd = new List<CodeMemberProperty>();
                foreach (var member in type.Members)
                {
                    var property = member as CodeMemberProperty;
                    if (SkipProperty(property, type))
                    {
                        continue;
                    }
                    propertiesToAdd.Add(GetOptionSetEnumType(property, logicalName));
                }

                foreach (var enumProp in propertiesToAdd.Where(p => p != null))
                {
                    type.Members.Add(enumProp);
                }
            }

            AddEntityOptionSetEnumDeclaration(types);
        }

        private bool SkipProperty(CodeMemberProperty property, CodeTypeDeclaration type)
        {
            HashSet<string> attributes;
            return property == null ||
                   !IsOptionSetProperty(property) ||
                   (UnmappedProperties.TryGetValue(type.Name.ToLower(), out attributes) && attributes.Contains(property.Name.ToLower())) ||
                   property.CustomAttributes.Cast<CodeAttributeDeclaration>().Any(att => att.Name == "System.ObsoleteAttribute");
        }

        private static bool IsOptionSetProperty(CodeMemberProperty property)
        {
            // By default this check will work
            return property.Type.BaseType == "Microsoft.Xrm.Sdk.OptionSetValue" || IsNullableIntPropery(property);
        }

        // If using the Xrm Client, OptionSets are converted to nullable Ints
        private static bool IsNullableIntPropery(CodeMemberProperty property)
        {
            return property.Type.BaseType == "System.Nullable`1" &&
                   property.Type.TypeArguments != null &&
                   property.Type.TypeArguments.Count == 1 &&
                   property.Type.TypeArguments[0].BaseType == "System.Int32";
        }

        #endregion

        private void InitializeMappings()
        {
            var specifedMappings = ConfigHelper.GetAppSettingOrDefault("PropertyEnumMappings", string.Empty).Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            UnmappedProperties = ConfigHelper.GetDictionaryHash("UnmappedProperties", true);
            SpecifiedMappings = new Dictionary<string, string>();

            foreach (var specifiedMapping in specifedMappings)
            {
                if (string.IsNullOrWhiteSpace(specifiedMapping))
                {
                    continue;
                }
                var parts = specifiedMapping.Split(',');
                SpecifiedMappings.Add(parts[0].Trim().ToLower(), parts[1].Trim());
            }
        }

        //private IEnumerable<CodeMemberProperty> GetProperties(CodeTypeDeclaration type){
        //    foreach(var member in type.Members){
        //        var property = member as CodeMemberProperty;
        //        if(property != null){
        //            yield return property;
        //        }
        //    }
        //    yield break;
        //}

        private CodeMemberProperty GetOptionSetEnumType(CodeMemberProperty prop, string entityLogicalName)
        {
            var data = CodeWriterFilterService.EntityMetadata[entityLogicalName];
            string propertyLogicalName = (from CodeAttributeDeclaration att in prop.CustomAttributes
                                          where att.AttributeType.BaseType == "Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute" 
                                          select ((CodePrimitiveExpression) att.Arguments[0].Value).Value.ToString()).FirstOrDefault();

            if (propertyLogicalName == null) { throw new Exception("Unable to determine property Logical Name"); }

            var attribute = data.Attributes.FirstOrDefault(a => a.LogicalName == propertyLogicalName);
            var picklist = attribute as EnumAttributeMetadata;
            string specifiedEnum;
            if (picklist == null) { return null; }

            var enumName = NamingService.GetNameForOptionSet(data, picklist.OptionSet, Services);
            if (SpecifiedMappings.TryGetValue(entityLogicalName.ToLower() + "." + prop.Name.ToLower(), out specifiedEnum))
            {
                enumName = specifiedEnum;
            }
            else if (CodeWriterFilterService.EntityMetadata.ContainsKey(enumName) && CodeWriterFilterService.EntityMetadata[enumName].SchemaName == enumName)
            {
                enumName += "Enum";
            }

            enumName += "?";

            var property = new CodeMemberProperty
            {
                Name = prop.Name + "Enum",
                Type = new CodeTypeReference(enumName),
                Attributes = System.CodeDom.MemberAttributes.Public
            };

            // [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("AttributeLogicalName")]
            property.CustomAttributes.Add(new CodeAttributeDeclaration("Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute", new CodeAttributeArgument(new CodePrimitiveExpression(propertyLogicalName))));

            property.GetStatements.Add(
                new CodeMethodReturnStatement(
                    new CodeCastExpression(
                        enumName,
                        new CodeMethodInvokeExpression(
                            new CodeTypeReferenceExpression("EntityOptionSetEnum"),
                            "GetEnum",
                            new CodeThisReferenceExpression(),
                            new CodePrimitiveExpression(propertyLogicalName)))));

            if (prop.HasSet)
            {
                var setSnippet = IsNullableIntPropery(prop) ? "(int?)value" : "value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null";

                property.SetStatements.Add(
                    new CodeAssignStatement(
                        new CodeVariableReferenceExpression(prop.Name),
                        new CodeSnippetExpression(setSnippet)));
            }
            return property;
        }


        private static void AddEntityOptionSetEnumDeclaration(CodeTypeDeclarationCollection types)
        {
            var enumClass = new CodeTypeDeclaration("EntityOptionSetEnum")
            {
                IsClass = true,
                TypeAttributes = TypeAttributes.Sealed | TypeAttributes.NotPublic,
            };

            // public static int? GetEnum(Microsoft.Xrm.Sdk.Entity entity, string attributeLogicalName)
            var get = new CodeMemberMethod
            {
                Name = "GetEnum",
                ReturnType = new CodeTypeReference(typeof(int?)),
                // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
                Attributes = System.CodeDom.MemberAttributes.Static | System.CodeDom.MemberAttributes.Public,
            };
            get.Parameters.Add(new CodeParameterDeclarationExpression(typeof(Microsoft.Xrm.Sdk.Entity), "entity"));
            get.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "attributeLogicalName"));

            // entity.Attributes.ContainsKey(attributeLogicalName)
            var entityAttributesContainsKey =
                new CodeMethodReferenceExpression(
                    new CodePropertyReferenceExpression(
                        new CodeArgumentReferenceExpression("entity"),
                        "Attributes"),
                    "ContainsKey");
            var invokeContainsKey = new CodeMethodInvokeExpression(entityAttributesContainsKey, new CodeArgumentReferenceExpression("attributeLogicalName"));

            // Microsoft.Xrm.Sdk.OptionSetValue value = entity.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>(attributeLogicalName).Value;
            var declareAndSetValue =
                new CodeVariableDeclarationStatement
                {
                    Type = new CodeTypeReference(typeof(OptionSetValue)),
                    Name = "value",
                    InitExpression = new CodeMethodInvokeExpression(
                        new CodeMethodReferenceExpression(
                            new CodeArgumentReferenceExpression("entity"), "GetAttributeValue", new CodeTypeReference(typeof(OptionSetValue))),
                            new CodeArgumentReferenceExpression("attributeLogicalName"))
                };

            // value != null
            var valueNeNull = new CodeSnippetExpression("value != null");

            // value.Value
            var invokeValueGetValue = new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("value"), "Value");

            // if(invokeContainsKey){return invokeGetAttributeValue;}else{return null}
            get.Statements.Add(new CodeConditionStatement(invokeContainsKey, declareAndSetValue, 
                new CodeConditionStatement(valueNeNull, new CodeMethodReturnStatement(invokeValueGetValue))));

            // return null;
            get.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(null)));

            enumClass.Members.Add(get);

            types.Add(enumClass);
        }
    }
}
