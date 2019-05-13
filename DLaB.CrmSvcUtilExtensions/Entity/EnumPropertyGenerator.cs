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
        public bool CreateBaseClasses { get; }
        public bool MultiSelectEnumCreated { get; private set; }
        public Dictionary<string, string> SpecifiedMappings { get; private set; }
        public Dictionary<string, HashSet<string>> UnmappedProperties { get; private set; }

        public INamingService NamingService { get; private set; }
        public IServiceProvider Services { get; private set; }

        public EnumPropertyGenerator(bool createBaseClasses)
        {
            CreateBaseClasses = createBaseClasses;
            MultiSelectEnumCreated = false;
        }

        #region ICustomizeCodeDomService Members

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            NamingService = new NamingService((INamingService)services.GetService(typeof(INamingService)));
            Services = services;
            InitializeMappings();
            var types = codeUnit.Namespaces[0].Types;
            foreach (CodeTypeDeclaration type in types)
            {
                if (!type.IsClass || type.IsContextType() || type.IsBaseEntityType()) { continue; }

                var logicalName = type.GetFieldInitalizedValue("EntityLogicalName");
                var propertiesToAdd = new List<CodeMemberProperty>();
                foreach (var member in type.Members)
                {
                    var property = member as CodeMemberProperty;
                    if (SkipProperty(property, type, logicalName))
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

            if (!CreateBaseClasses)
            {
                // If creating Base Classes, this will be included in the base class
                types.Add(GetEntityOptionSetEnumDeclaration());
            }
        }

        private bool SkipProperty(CodeMemberProperty property, CodeTypeDeclaration type, string entityLogicalName)
        {
            HashSet<string> attributes;
            return property == null ||
                   !IsOptionSetProperty(property) ||
                   (UnmappedProperties.TryGetValue(type.Name.ToLower(), out attributes) && attributes.Contains(property.Name.ToLower())) ||
                   property.CustomAttributes.Cast<CodeAttributeDeclaration>().Any(att => att.Name == "System.ObsoleteAttribute") ||
                   OptionSetIsSkipped(property, entityLogicalName);
        }

        private static bool IsOptionSetProperty(CodeMemberProperty property)
        {
            // By default this check will work
            return property.Type.BaseType == "Microsoft.Xrm.Sdk.OptionSetValue" 
                   || property.Type.BaseType == "Microsoft.Xrm.Sdk.OptionSetValueCollection"
                   || IsNullableIntProperty(property);
        }

        private bool OptionSetIsSkipped(CodeMemberProperty property, string entityLogicalName)
        {
            var info = GetOptionSetEnumInfo(property, entityLogicalName);
            return info != null && !OptionSet.CodeWriterFilterService.Approver.IsAllowed(info.OptionSetType);
        }

        // If using the Xrm Client, OptionSets are converted to nullable Ints
        private static bool IsNullableIntProperty(CodeMemberProperty property)
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

        private class EnumPropertyInfo
        {
            public string OptionSetType { get; set; }
            public string EnumType => IsMultSelect
                ? "System.Collections.Generic.IEnumerable<" + OptionSetType + ">"
                : OptionSetType + "?";   
            public string PropertyName { get; set; }
            public string LogicalName { get; set; }
            public bool IsMultSelect { get; set; }
        }

        private CodeMemberProperty GetOptionSetEnumType(CodeMemberProperty prop, string entityLogicalName)
        {
            var info = GetOptionSetEnumInfo(prop, entityLogicalName);
            if (info == null)
            {
                return null;                
            }

            var property = new CodeMemberProperty
            {
                Name = info.PropertyName,
                Type = new CodeTypeReference(info.EnumType),
                Attributes = System.CodeDom.MemberAttributes.Public
            };

            // [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("AttributeLogicalName")]
            property.CustomAttributes.Add(new CodeAttributeDeclaration("Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute", new CodeAttributeArgument(new CodePrimitiveExpression(info.LogicalName))));
            AddEnumGet(info, property);
            AddEnumSet(prop, info, property);
            return property;
        }

        private void AddEnumGet(EnumPropertyInfo info, CodeMemberProperty property)
        {
            CodeExpression returnExpression;
            if (info.IsMultSelect)
            {
                MultiSelectEnumCreated = true;
                // return EntityOptionSetEnum.GetMultiEnum<info.OptionSetType>(this, info.LogicalName);
                returnExpression =
                    new CodeMethodInvokeExpression(
                        new CodeMethodReferenceExpression(
                            CreateBaseClasses
                                ? new CodeTypeReferenceExpression(EntityBaseClassGenerator.BaseEntityName)
                                : new CodeTypeReferenceExpression("EntityOptionSetEnum"),
                            "GetMultiEnum",
                            new CodeTypeReference(info.OptionSetType)),
                        new CodeThisReferenceExpression(),
                        new CodePrimitiveExpression(info.LogicalName));
            }
            else
            {
                returnExpression =
                    new CodeCastExpression(
                        info.EnumType,
                        new CodeMethodInvokeExpression(
                            CreateBaseClasses
                                ? new CodeTypeReferenceExpression(EntityBaseClassGenerator.BaseEntityName)
                                : new CodeTypeReferenceExpression("EntityOptionSetEnum"),
                            "GetEnum",
                            new CodeThisReferenceExpression(),
                            new CodePrimitiveExpression(info.LogicalName)));
            }

            property.GetStatements.Add(new CodeMethodReturnStatement(returnExpression));
        }

        private void AddEnumSet(CodeMemberProperty prop, EnumPropertyInfo info, CodeMemberProperty property)
        {
            if (prop.HasSet)
            {
                CodeExpression setExpression;
                if (info.IsMultSelect)
                {
                    //EntityOptionSetEnum.GetMultiEnum(this, info.LogicalName, value)
                    setExpression =
                        new CodeMethodInvokeExpression(
                            CreateBaseClasses
                                ? new CodeTypeReferenceExpression(EntityBaseClassGenerator.BaseEntityName)
                                : new CodeTypeReferenceExpression("EntityOptionSetEnum"),
                            "GetMultiEnum",
                            new CodeThisReferenceExpression(),
                            new CodePrimitiveExpression(info.LogicalName),
                            new CodePropertySetValueReferenceExpression());
                }
                else
                {
                    setExpression = new CodeSnippetExpression(
                        IsNullableIntProperty(prop)
                            ? "(int?)value"
                            : "value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null");
                }

                property.SetStatements.Add(
                    new CodeAssignStatement(
                        new CodeVariableReferenceExpression(prop.Name),
                        setExpression));
            }
        }

        private EnumPropertyInfo GetOptionSetEnumInfo(CodeMemberProperty prop, string entityLogicalName)
        {
            var propertyLogicalName = prop.GetLogicalName();
            if (propertyLogicalName == null) { throw new Exception("Unable to determine property Logical Name"); }

            var data = CodeWriterFilterService.EntityMetadata[entityLogicalName];
            var attribute = data.Attributes.FirstOrDefault(a => a.LogicalName == propertyLogicalName);
            var picklist = attribute as EnumAttributeMetadata;
            if (picklist == null) { return null; }

            var enumName = NamingService.GetNameForOptionSet(data, picklist.OptionSet, Services);
            if (SpecifiedMappings.TryGetValue(entityLogicalName.ToLower() + "." + prop.Name.ToLower(), out var specifiedEnum))
            {
                enumName = specifiedEnum;
            }
            else if (CodeWriterFilterService.EntityMetadata.ContainsKey(enumName) && CodeWriterFilterService.EntityMetadata[enumName].SchemaName == enumName)
            {
                enumName += "Enum";
            }

            return new EnumPropertyInfo
            {
                OptionSetType = enumName,
                IsMultSelect = picklist is MultiSelectPicklistAttributeMetadata,
                PropertyName = prop.Name + "Enum",
                LogicalName = propertyLogicalName
            };
        }

        private CodeTypeDeclaration GetEntityOptionSetEnumDeclaration()
        {
            var enumClass = new CodeTypeDeclaration("EntityOptionSetEnum")
            {
                IsClass = true,
                TypeAttributes = TypeAttributes.Sealed | TypeAttributes.NotPublic,
            };

            enumClass.Members.AddRange(CreateGetEnumMethods(MultiSelectEnumCreated));

            return enumClass;
        }

        public static CodeTypeMember[] CreateGetEnumMethods(bool multiSelectCreated)
        {
            var members = new List<CodeTypeMember>
            {
                CreateGetEnumMethod()
            };
            if (multiSelectCreated)
            {
                members.Add(CreateGetMultiEnum());
                members.Add(CreateGetMultiEnumSet());
            }

            return members.ToArray();
        }

        private static CodeMemberMethod CreateGetEnumMethod()
        {
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
                            new CodeArgumentReferenceExpression("entity"),
                            "GetAttributeValue",
                            new CodeTypeReference(typeof(OptionSetValue))),
                        new CodeArgumentReferenceExpression("attributeLogicalName"))
                };

            // value != null
            var valueNeNull = new CodeSnippetExpression("value != null");

            // value.Value
            var invokeValueGetValue = new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("value"), "Value");

            // if(invokeContainsKey){return invokeGetAttributeValue;}else{return null}
            get.Statements.Add(new CodeConditionStatement(invokeContainsKey,
                declareAndSetValue,
                new CodeConditionStatement(valueNeNull, new CodeMethodReturnStatement(invokeValueGetValue))));

            // return null;
            get.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(null)));
            return get;
        }

        private static CodeMemberMethod CreateGetMultiEnum()
        {
            var returnType = new CodeTypeReference(typeof(IEnumerable<>));
            returnType.TypeArguments.Add(new CodeTypeReference("T"));
            // public static IEnumerable<T> GetMultiEnum<T>(Entity entity, string attributeLogicalName)
            var get = new CodeMemberMethod
            {
                Name = "GetMultiEnum",
                ReturnType = returnType,
                // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
                Attributes = System.CodeDom.MemberAttributes.Static | System.CodeDom.MemberAttributes.Public,
            };
            get.TypeParameters.Add(new CodeTypeParameter("T"));
            get.Parameters.Add(new CodeParameterDeclarationExpression(typeof(Microsoft.Xrm.Sdk.Entity), "entity"));
            get.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "attributeLogicalName"));

            // OptionSetValueCollection value = entity.GetAttributeValue<OptionSetValueCollection>(attributeLogicalName)
            get.Statements.Add(new CodeVariableDeclarationStatement(
                typeof(OptionSetValueCollection),
                "value",
                new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(
                        new CodeArgumentReferenceExpression("entity"),
                        "GetAttributeValue",
                        new CodeTypeReference(typeof(OptionSetValueCollection))),
                    new CodeArgumentReferenceExpression("attributeLogicalName")
                )));

            // var list = new System.Collections.Generic.List<T>();
            var listType = new CodeTypeReference(typeof(List<>));
            listType.TypeArguments.Add(new CodeTypeReference("T"));
            get.Statements.Add(new CodeVariableDeclarationStatement(
                listType,
                "list",
                new CodeObjectCreateExpression(listType)
                ));

            //list.AddRange(Enumerable.Select(value, v => (T)(object)v.Value));
            get.Statements.Add(new CodeMethodInvokeExpression(
                new CodeArgumentReferenceExpression("list"),
                "AddRange",
                new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(typeof(Enumerable)), 
                    "Select", 
                    new CodeArgumentReferenceExpression("value"), 
                    new CodeSnippetExpression("v => (T)(object)v.Value")
                )));

            //return list;
            get.Statements.Add(new CodeMethodReturnStatement(new CodeArgumentReferenceExpression("list")));

            return get;
        }

        private static CodeMemberMethod CreateGetMultiEnumSet()
        {
            var optionSetValueCollection = typeof(OptionSetValueCollection);

            // public static Microsoft.Xrm.Sdk.OptionSetValueCollection GetMultiEnum<T>(Microsoft.Xrm.Sdk.Entity entity, string attributeLogicalName, System.Collections.Generic.IEnumerable<T> values)
            var get = new CodeMemberMethod
            {
                Name = "GetMultiEnum",
                ReturnType = new CodeTypeReference(optionSetValueCollection),
                // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
                Attributes = System.CodeDom.MemberAttributes.Static | System.CodeDom.MemberAttributes.Public,
            };
            get.TypeParameters.Add(new CodeTypeParameter("T"));
            get.Parameters.Add(new CodeParameterDeclarationExpression(typeof(Microsoft.Xrm.Sdk.Entity), "entity"));
            get.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "attributeLogicalName"));

            var valuesType = new CodeTypeReference(typeof(IEnumerable<>));
            valuesType.TypeArguments.Add(new CodeTypeReference("T"));
            get.Parameters.Add(new CodeParameterDeclarationExpression(valuesType, "values"));

            //Microsoft.Xrm.Sdk.OptionSetValueCollection collection = new Microsoft.Xrm.Sdk.OptionSetValueCollection();
            get.Statements.Add(new CodeVariableDeclarationStatement(
                optionSetValueCollection,
                "collection",
                new CodeObjectCreateExpression(optionSetValueCollection)
            ));

            //collection.AddRange(System.Enumerable.Linq.Select(values, v => new Microsoft.Xrm.Sdk.OptionSetValue((int)(object)v));
            get.Statements.Add(new CodeMethodInvokeExpression(
                new CodeArgumentReferenceExpression("collection"),
                "AddRange",
                new CodeMethodInvokeExpression(
                    new CodeTypeReferenceExpression(typeof(Enumerable)), 
                    "Select", 
                    new CodeArgumentReferenceExpression("values"), 
                    new CodeSnippetExpression("v => new Microsoft.Xrm.Sdk.OptionSetValue((int)(object)v)")
                )));

            //return collection;
            get.Statements.Add(new CodeMethodReturnStatement(new CodeArgumentReferenceExpression("collection")));

            return get;
        }
    }
}
