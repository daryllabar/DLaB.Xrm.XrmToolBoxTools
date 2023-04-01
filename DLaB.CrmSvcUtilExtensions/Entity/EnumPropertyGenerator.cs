using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.Xrm.Sdk.Extensions;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

//using System.Reflection;

namespace DLaB.ModelBuilderExtensions.Entity
{
    public class EnumPropertyGenerator : TypedServiceBase<ICustomizeCodeDomService>
    {
        public Dictionary<string, string> PropertyEnumMappings { get => DLaBSettings.PropertyEnumMappings; set => DLaBSettings.PropertyEnumMappings = value; }
        public bool ReplaceOptionSetPropertiesWithEnum { get => DLaBSettings.ReplaceOptionSetPropertiesWithEnum; set => DLaBSettings.ReplaceOptionSetPropertiesWithEnum = value; }
        public Dictionary<string, HashSet<string>> UnmappedProperties { get => DLaBSettings.UnmappedProperties; set => DLaBSettings.UnmappedProperties = value; }


        public EnumPropertyGenerator(ICustomizeCodeDomService defaultService, IDictionary<string, string> parameters) : base(defaultService, parameters)
        {
        }

        public EnumPropertyGenerator(ICustomizeCodeDomService defaultService, DLaBModelBuilderSettings settings = null) : base(defaultService, settings)
        {
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
                    if (SkipProperty(property, type))
                    {
                        continue;
                    }

                    // ReSharper disable once AssignNullToNotNullAttribute
                    propertiesToReplace[type.Members.IndexOf(property)] = property;;
                }

                foreach (var enumProp in propertiesToReplace.Where(p => p.Value != null).OrderByDescending(p => p.Key))
                {
                    if (!ReplaceOptionSetPropertiesWithEnum)
                    {
                        type.Members.Insert(enumProp.Key + 1, enumProp.Value);
                    }
                }
            }
        }

        private bool SkipProperty(CodeMemberProperty property, CodeTypeDeclaration type)
        {
            return property == null ||
                   !IsOptionSetProperty(property) ||
                   (UnmappedProperties.TryGetValue(type.Name.ToLower(), out var attributes) && attributes.Contains(property.Name.ToLower())) ||
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
                // return EntityOptionSetEnum.GetMultiEnum<info.OptionSetType>(this, info.LogicalName);
                returnExpression =
                    new CodeMethodInvokeExpression(
                        new CodeMethodReferenceExpression(
                            new CodeTypeReferenceExpression("EntityOptionSetEnum"),
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
                             new CodeTypeReferenceExpression("EntityOptionSetEnum"),
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
                        new CodeTypeReferenceExpression("EntityOptionSetEnum"),
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
    }
}
