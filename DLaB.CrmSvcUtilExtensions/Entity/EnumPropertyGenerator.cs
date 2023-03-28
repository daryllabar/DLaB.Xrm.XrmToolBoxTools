using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xrm.Sdk.Extensions;

//using System.Reflection;

namespace DLaB.ModelBuilderExtensions.Entity
{
    public class EnumPropertyGenerator : TypedServiceBase<ICustomizeCodeDomService>
    {
        public string BaseEntityName { get => DLaBSettings.BaseEntityClassName; set => DLaBSettings.BaseEntityClassName = value; }
        public bool CreateBaseClasses { get => DLaBSettings.CreateBaseClasses; set => DLaBSettings.CreateBaseClasses = value; }
        public bool MultiSelectEnumCreated { get; private set; }
        public Dictionary<string, string> PropertyEnumMappings { get => DLaBSettings.PropertyEnumMappings; set => DLaBSettings.PropertyEnumMappings = value; }
        public bool ReplaceOptionSetPropertiesWithEnum { get => DLaBSettings.ReplaceOptionSetPropertiesWithEnum; set => DLaBSettings.ReplaceOptionSetPropertiesWithEnum = value; }
        public Dictionary<string, HashSet<string>> UnmappedProperties { get => DLaBSettings.UnmappedProperties; set => DLaBSettings.UnmappedProperties = value; }


        public EnumPropertyGenerator(ICustomizeCodeDomService defaultService, IDictionary<string, string> parameters) : base(defaultService, parameters)
        {
            MultiSelectEnumCreated = false;
        }

        public EnumPropertyGenerator(ICustomizeCodeDomService defaultService, DLaBModelBuilderSettings settings = null) : base(defaultService, settings)
        {
            MultiSelectEnumCreated = false;
        }

        #region ICustomizeCodeDomService Members

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            SetServiceCache(services);

            foreach (var type in codeUnit.GetEntityTypes())
            {
                var logicalName = type.GetEntityLogicalName();
                var propertiesToReplace = new Dictionary<int,CodeMemberProperty>();
                foreach (var member in type.Members)
                {
                    var property = member as CodeMemberProperty;
                    if (SkipProperty(property, type, logicalName))
                    {
                        continue;
                    }

                    // ReSharper disable once AssignNullToNotNullAttribute
                    propertiesToReplace[type.Members.IndexOf(property)] = GetOptionSetEnumType(property, logicalName);
                }

                foreach (var enumProp in propertiesToReplace.Where(p => p.Value != null).OrderByDescending(p => p.Key))
                {
                    if (!ReplaceOptionSetPropertiesWithEnum)
                    {
                        type.Members.Insert(enumProp.Key + 1, enumProp.Value);
                    }
                }
            }

            //if (!CreateBaseClasses)
            //{
            //    // If creating Base Classes, this will be included in the base class
            //    types.Add(GetEntityOptionSetEnumDeclaration());
            //}
        }

        private bool SkipProperty(CodeMemberProperty property, CodeTypeDeclaration type, string entityLogicalName)
        {
            HashSet<string> attributes;
            return property == null ||
                   !IsOptionSetProperty(property) ||
                   (UnmappedProperties.TryGetValue(type.Name.ToLower(), out attributes) && attributes.Contains(property.Name.ToLower())) ||
                   property.CustomAttributes.Cast<CodeAttributeDeclaration>().Any(att => att.Name == "System.ObsoleteAttribute");// ||
                   //OptionSetIsSkipped(property, entityLogicalName);
        }

        private static bool IsOptionSetProperty(CodeMemberProperty property)
        {
            // By default this check will work
            return property.Type.BaseType == "Microsoft.Xrm.Sdk.OptionSetValue" 
                   || property.Type.BaseType == "Microsoft.Xrm.Sdk.OptionSetValueCollection"
                   || IsNullableIntProperty(property);
        }

        //private bool OptionSetIsSkipped(CodeMemberProperty property, string entityLogicalName)
        //{
        //    var info = GetOptionSetEnumInfo(property, entityLogicalName);
        //    return info != null; //&& !OptionSet.CodeWriterFilterService.Approver.IsAllowed(info.OptionSetType);
        //}

        // If using the Xrm Client, OptionSets are converted to nullable Ints
        private static bool IsNullableIntProperty(CodeMemberProperty property)
        {
            return property.Type.BaseType == "System.Nullable`1" &&
                   property.Type.TypeArguments != null &&
                   property.Type.TypeArguments.Count == 1 &&
                   property.Type.TypeArguments[0].BaseType == "System.Int32";
        }

        #endregion

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
            property.Comments.AddRange(prop.Comments);
        
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
                                ? new CodeTypeReferenceExpression(BaseEntityName)
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
                                ? new CodeTypeReferenceExpression(BaseEntityName)
                                : new CodeTypeReferenceExpression("EntityOptionSetEnum"),
                            "GetEnum",
                            new CodeThisReferenceExpression(),
                            new CodePrimitiveExpression(info.LogicalName)));
            }
        
            property.GetStatements.Add(new CodeMethodReturnStatement(returnExpression));
        }
        
        private void AddEnumSet(CodeMemberProperty prop, EnumPropertyInfo info, CodeMemberProperty property)
        {
            if (!prop.HasSet)
            {
                return;
            }
        
            // this.OnPropertyChanging("PropName");
            property.SetStatements.Add(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(), "OnPropertyChanging", new CodePrimitiveExpression(prop.Name)
            ));
        
            CodeExpression getValueToSetExpression;
            if (info.IsMultSelect)
            {
                //EntityOptionSetEnum.GetMultiEnum(this, info.LogicalName, value)
                getValueToSetExpression =
                    new CodeMethodInvokeExpression(
                        CreateBaseClasses
                            ? new CodeTypeReferenceExpression(BaseEntityName)
                            : new CodeTypeReferenceExpression("EntityOptionSetEnum"),
                        "GetMultiEnum",
                        new CodeThisReferenceExpression(),
                        new CodePrimitiveExpression(info.LogicalName),
                        new CodePropertySetValueReferenceExpression());
            }
            else
            {
                getValueToSetExpression = new CodeSnippetExpression(
                    IsNullableIntProperty(prop)
                        ? "(int?)value"
                        : "value.HasValue ? new Microsoft.Xrm.Sdk.OptionSetValue((int)value) : null");
            }
        
            // this.SetAttributeValue("logicalName", getValueExpression);
            property.SetStatements.Add(
                new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    "SetAttributeValue",
                    new CodePrimitiveExpression(prop.GetLogicalName()),
                    getValueToSetExpression));
        
            // this.OnPropertyChanged("PropName");
            property.SetStatements.Add(new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(), "OnPropertyChanged", new CodePrimitiveExpression(prop.Name)
            ));
        }
        
        private EnumPropertyInfo GetOptionSetEnumInfo(CodeMemberProperty prop, string entityLogicalName)
        {
            var propertyLogicalName = prop.GetLogicalName();
            if (propertyLogicalName == null) { throw new Exception("Unable to determine property Logical Name"); }
        
            var data = ServiceCache.EntityMetadataByLogicalName[entityLogicalName];
            var attribute = data.Attributes.FirstOrDefault(a => a.LogicalName == propertyLogicalName);
            if (!(attribute is EnumAttributeMetadata picklist))
            {
                return null;
            }
        
            var enumName = ServiceProvider.Get<INamingService>().GetNameForOptionSet(data, picklist.OptionSet, ServiceCache.ServiceProvider);
            if (PropertyEnumMappings.TryGetValue(entityLogicalName.ToLower() + "." + prop.Name.ToLower(), out var specifiedEnum))
            {
                enumName = specifiedEnum;
            }
            else if (ServiceCache.EntityMetadataByLogicalName.ContainsKey(enumName) && ServiceCache.EntityMetadataByLogicalName[enumName].SchemaName == enumName)
            {
                enumName += "Enum";
            }
        
            return new EnumPropertyInfo
            {
                OptionSetType = enumName,
                IsMultSelect = picklist is MultiSelectPicklistAttributeMetadata,
                PropertyName = prop.Name + (ReplaceOptionSetPropertiesWithEnum
                    ? string.Empty
                    : "Enum" ),
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

            // if(value == null){ return list; }
            get.Statements.Add(new CodeConditionStatement(new CodeSnippetExpression("value == null"), new CodeMethodReturnStatement(new CodeVariableReferenceExpression("list"))));

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

            // if(values == null){ return null; }
            get.Statements.Add(new CodeConditionStatement(new CodeSnippetExpression("values == null"), new CodeMethodReturnStatement(new CodePrimitiveExpression(null))));

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
