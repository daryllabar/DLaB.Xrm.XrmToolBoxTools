using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using DLaB.CrmSvcUtilExtensions.Entity;
using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace DLaB.CrmSvcUtilExtensions
{
    public static class Extensions
    {
        private const string XrmAttributeLogicalName = "Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute";
        private const string XrmRelationshipSchemaName = "Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute";
        private static List<Tuple<CodeTypeDeclaration, EntityMetadata>> _entityTypes;


        #region CodeCompileUnit

        public static List<Tuple<CodeTypeDeclaration, EntityMetadata>> GetEntityTypes(this CodeCompileUnit codeUnit, IServiceProvider services)
        {
            if (_entityTypes != null)
            {
                return _entityTypes;
            }

            _entityTypes = new List<Tuple<CodeTypeDeclaration, EntityMetadata>>();
            var metadata = ((IMetadataProviderService) services.GetService(typeof(IMetadataProviderService))).LoadMetadata().Entities.ToDictionary(e => e.LogicalName);
            foreach (var type in codeUnit.GetTypes().Where(type => type.IsClass && !type.IsContextType()))
            {
                var logicalNameAttribute = type.CustomAttributes.Cast<CodeAttributeDeclaration>()
                            .FirstOrDefault(a => a.Name == "Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute");
                if (logicalNameAttribute == null)
                {
                    continue;
                }

                var typeEntityName = ((CodePrimitiveExpression) logicalNameAttribute.Arguments[0].Value).Value.ToString();
                if (metadata.TryGetValue(typeEntityName, out var entityMetadata))
                {
                    _entityTypes.Add(new Tuple<CodeTypeDeclaration, EntityMetadata>(type, entityMetadata));
                }
            }

            return _entityTypes;
        }

        public static IEnumerable<CodeTypeDeclaration> GetTypes(this CodeCompileUnit codeUnit)
        {
            for (var i = 0; i < codeUnit.Namespaces.Count; ++i)
            {
                var types = codeUnit.Namespaces[i].Types;
                for (var j = 0; j < types.Count; j++)
                {
                    yield return types[j];
                }
            }
        }

        public static void RemoveAssemblyAttributes(this CodeCompileUnit codeUnit)
        {
            var attributesToRemove = new List<CodeAttributeDeclaration>();
            foreach (CodeAttributeDeclaration attribute in codeUnit.AssemblyCustomAttributes)
            {
                Trace.TraceInformation("Attribute BaseType is {0}", attribute.AttributeType.BaseType);
                var baseType = attribute.AttributeType.BaseType;
                if (baseType == "Microsoft.Xrm.Sdk.Client.ProxyTypesAssemblyAttribute"
                    || baseType  == "System.CodeDom.Compiler.GeneratedCodeAttribute")
                {
                    attributesToRemove.Add(attribute);
                }
            }

            foreach (var attribute in attributesToRemove)
            {
                codeUnit.AssemblyCustomAttributes.Remove(attribute);
            }
        }

        #endregion CodeCompileUnit

        #region CodeMemberProperty

        public static string GetLogicalName(this CodeMemberProperty property)
        {
            return 
                (from CodeAttributeDeclaration att in property.CustomAttributes
                where att.AttributeType.BaseType == XrmAttributeLogicalName
                select ((CodePrimitiveExpression) att.Arguments[0].Value).Value.ToString()).FirstOrDefault();
        }

        public static string GetRelationshipLogicalName(this CodeMemberProperty property)
        {
            return 
                (from CodeAttributeDeclaration att in property.CustomAttributes
                    where att.AttributeType.BaseType == XrmRelationshipSchemaName
                    select ((CodePrimitiveExpression) att.Arguments[0].Value).Value.ToString()).FirstOrDefault();
        }

        #endregion CodeMemberProperty

        #region CodeTypeDeclaration

        public static string GetFieldInitalizedValue(this CodeTypeDeclaration type, string fieldName)
        {
            var field = type.Members.OfType<CodeMemberField>().FirstOrDefault(f => f.Name == fieldName);
            if (field != null)
            {
                return ((CodePrimitiveExpression)field.InitExpression).Value.ToString();
            }

            throw new Exception("Field " + fieldName + " was not found for type " + type.Name);
        }

        public static string GetEntityLogicalName(this CodeTypeDeclaration type)
        {
            try
            {
                return ((CodePrimitiveExpression)type.GetCustomAttribute("Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute").Arguments[0].Value).Value.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to get the EntityLogicalName for " + type.Name, ex);
            }
        }

        public static string GetRequestProxyAttribute(this CodeTypeDeclaration type)
        {
            try
            {
                return ((CodePrimitiveExpression)type.GetCustomAttribute("Microsoft.Xrm.Sdk.Client.RequestProxyAttribute")?.Arguments[0].Value)?.Value.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to get the EntityLogicalName for " + type.Name, ex);
            }
        }

        public static string GetResponseProxyAttribute(this CodeTypeDeclaration type)
        {
            try
            {
                return ((CodePrimitiveExpression)type.GetCustomAttribute("Microsoft.Xrm.Sdk.Client.ResponseProxyAttribute")?.Arguments[0].Value)?.Value.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to get the EntityLogicalName for " + type.Name, ex);
            }
        }

        public static CodeAttributeDeclaration GetCustomAttribute(this CodeTypeDeclaration type, string attributeName)
        {
            try
            {
                return type.CustomAttributes.Cast<CodeAttributeDeclaration>().FirstOrDefault(a => a.Name == attributeName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to get the Custom Attribute {attributeName} for {type.Name}", ex);
            }
        }

        /// <summary>
        /// Determines if the type inherits from one of the known Xrm OrganizationServiceContext types.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static bool IsContextType(this CodeTypeDeclaration type)
        {
            var baseType = type.BaseTypes[0].BaseType;
            return baseType == "Microsoft.Xrm.Client.CrmOrganizationServiceContext"
                   || baseType == "Microsoft.Xrm.Sdk.Client.OrganizationServiceContext";
        }

        public static bool IsBaseEntityType(this CodeTypeDeclaration type)
        {
            var name = type.Name;
            return name == EntityBaseClassGenerator.BaseEntityName
                   || name == EntityBaseClassGenerator.OrgEntityName
                   || name == EntityBaseClassGenerator.UserEntityName;
        }

        #endregion // CodeTypeDeclaration

        #region Label

        public static string GetLocalOrDefaultText(this Label label, string defaultIfNull = null)
        {
            var local = label.UserLocalizedLabel ?? label.LocalizedLabels.FirstOrDefault();

            if (local == null)
            {
                return defaultIfNull;
            }
            else
            {
                return local.Label ?? defaultIfNull;
            }
        }

        #endregion // Label

        #region OptionSetMetadataBase

        public static List<OptionMetadata> GetOptions(this OptionSetMetadataBase metadata)
        {
            List<OptionMetadata> options;
            var booleanOptionSet = metadata as BooleanOptionSetMetadata;
            var nonBooleanOptionSet = metadata as OptionSetMetadata;
            if (booleanOptionSet != null)
            {
                options = new List<OptionMetadata>
                {
                    booleanOptionSet.FalseOption,
                    booleanOptionSet.TrueOption
                };
            }
            else if (nonBooleanOptionSet != null)
            {
                options = nonBooleanOptionSet.Options.ToList();
            }
            else
            {
                throw new NotImplementedException($"OptionSetMetadataBase was of type {metadata.GetType().FullName}.  Only BooleanOptionSetMetadata or OptionSetMetadata are implemented");
            }
            return options;
        }

        #endregion OptionSetMetadataBase

        #region String

        /// <summary>
        /// Removes the diacritics.  In Unicode characters with diacritics are combination of 2 (or more) characters, 
        /// for example "ê" is compound of "e" and "^", "ü" is compound of "u" and "¨", etc. 
        /// This method allow to only keep the base character, in my examples "e" and "u"
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string RemoveDiacritics(this string text)
        {
            if (text == null)
            {
                return null;
            }
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var diacriticLess = from c in normalizedString
                                let unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c)
                                where unicodeCategory != UnicodeCategory.NonSpacingMark
                                select c;
            return new string(diacriticLess.ToArray()).Normalize(NormalizationForm.FormC);
        }

        #endregion String
    }
}
