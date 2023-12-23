using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.Xrm.Sdk;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace DLaB.ModelBuilderExtensions.Entity
{
    public class OptionSetPropertyGenerator : TypedServiceBase<ICustomizeCodeDomService>
    {
        public Dictionary<string, string> PropertyEnumMappings { get => DLaBSettings.PropertyEnumMappings; set => DLaBSettings.PropertyEnumMappings = value; }
        public bool SuppressINotifyPattern { get => Settings.SuppressINotifyPattern; set => Settings.SuppressINotifyPattern = value; }
        public bool ReplaceEnumPropertiesWithOptionSet { get => DLaBSettings.ReplaceEnumPropertiesWithOptionSet; set => DLaBSettings.ReplaceEnumPropertiesWithOptionSet = value; }
        public bool UseEnumForStateCodes { get => DLaBSettings.UseEnumForStateCodes; set => DLaBSettings.UseEnumForStateCodes = value; }

        #region Constructors

        public OptionSetPropertyGenerator(ICustomizeCodeDomService defaultService, IDictionary<string, string> parameters) : base(defaultService, parameters)
        {
        }

        public OptionSetPropertyGenerator(ICustomizeCodeDomService defaultService, DLaBModelBuilderSettings settings = null) : base(defaultService, settings)
        {
        }

        #endregion Constructors

        #region ICustomizeCodeDomService Members

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            SetServiceCache(services);

            foreach (var type in codeUnit.GetEntityTypes())
            {
                var entityName = type.GetEntityLogicalName();
                var enumProperties = new Dictionary<int,CodeMemberProperty>();
                var metadata = ServiceCache.EntityMetadataByLogicalName[entityName];
                foreach (var member in type.Members)
                {
                    if (!(member is CodeMemberProperty property))
                    {
                        continue;
                    }
                    var attribute = metadata.Attributes.FirstOrDefault(a => a.LogicalName == property.GetLogicalName());
                    if (SkipProperty(property, type, attribute))
                    {
                        continue;
                    }

                    enumProperties[type.Members.IndexOf(property)] = property;
                }

                foreach (var enumProp in enumProperties.Where(p => p.Value != null).OrderByDescending(p => p.Key))
                {
                    var property = enumProp.Value;
                    if (UseEnumForStateCodes && property.GetLogicalName() == "statecode")
                    {
                        continue;
                    }

                    type.Members[enumProp.Key] = GetOptionSetPropertyType(property);

                    if (!ReplaceEnumPropertiesWithOptionSet)
                    {
                        var enumType = property.Type.BaseType
                            .Replace("?", string.Empty)
                            .Replace("System.Collections.Generic.IEnumerable<", string.Empty)
                            .Replace(">", string.Empty);
                        if (!ServiceCache.MetadataForEnumsByName.ContainsKey(enumType)) { 
                            // OptionSet is not generated, skip generating Enum property
                            continue;
                        }
                        property.Name += "Enum";
                        type.Members.Insert(enumProp.Key + 1, property);
                    }
                }
            }
        }

        private bool SkipProperty(CodeMemberProperty property, CodeTypeDeclaration type, AttributeMetadata attribute)
        {
            return property == null ||
                   attribute == null ||
                   !attribute.IsOptionSet() ||
                   property.CustomAttributes.Cast<CodeAttributeDeclaration>().Any(att => att.Name == "System.ObsoleteAttribute");// ||
                   //OptionSetIsSkipped(property, entityLogicalName);
        }

        //private bool OptionSetIsSkipped(CodeMemberProperty property, string entityLogicalName)
        //{
        //    var info = GetOptionSetEnumInfo(property, entityLogicalName);
        //    return info != null; //&& !OptionSet.CodeWriterFilterService.Approver.IsAllowed(info.OptionSetType);
        //}

        #endregion

        private CodeMemberProperty GetOptionSetPropertyType(CodeMemberProperty enumProp)
        {
            var isMultiSelect = enumProp.Type.BaseType.StartsWith("System.Collections.Generic.IEnumerable");
            var property = new CodeMemberProperty
            {
                Name = enumProp.Name,
                Type = new CodeTypeReference(isMultiSelect
                    ? typeof(OptionSetValueCollection)
                    : typeof(OptionSetValue)),
                Attributes = enumProp.Attributes,
                CustomAttributes = enumProp.CustomAttributes,
                HasGet = enumProp.HasGet,
                HasSet = enumProp.HasSet,
                LinePragma = enumProp.LinePragma,
            };
            property.Comments.AddRange(enumProp.Comments);
            property.StartDirectives.AddRange(enumProp.StartDirectives);
            property.EndDirectives.AddRange(enumProp.EndDirectives);
            property.Parameters.AddRange(enumProp.Parameters);

            var logicalName = enumProp.GetLogicalName();
            AddOptionSetGet(property, logicalName);
            AddOptionSetSet(property, logicalName);
            return property;
        }

        private void AddOptionSetGet(CodeMemberProperty property, string logicalName)
        {
            // return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValueCollection>(attributeLogicalName)
            // return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("logicalName")
            var returnExpression = new CodeMethodInvokeExpression(
                new CodeMethodReferenceExpression(
                    new CodeThisReferenceExpression(),
                    "GetAttributeValue",
                    property.Type)
                , new CodePrimitiveExpression(logicalName));

            property.GetStatements.Add(new CodeMethodReturnStatement(returnExpression));
        }

        private void AddOptionSetSet(CodeMemberProperty property, string logicalName)
        {
            if (!property.HasSet)
            {
                return;
            }

            if (!SuppressINotifyPattern)
            {
                // this.OnPropertyChanging("PropName");
                property.SetStatements.Add(new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(), "OnPropertyChanging", new CodePrimitiveExpression(property.Name)
                ));
            }

            // this.SetAttributeValue("logicalName", getValueExpression);
            property.SetStatements.Add(
                new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(),
                    "SetAttributeValue",
                    new CodePrimitiveExpression(logicalName),
                    new CodePropertySetValueReferenceExpression()));

            if (!SuppressINotifyPattern)
            {
                // this.OnPropertyChanged("PropName");
                property.SetStatements.Add(new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(), "OnPropertyChanged", new CodePrimitiveExpression(property.Name)
                ));
            }
        }
    }
}
