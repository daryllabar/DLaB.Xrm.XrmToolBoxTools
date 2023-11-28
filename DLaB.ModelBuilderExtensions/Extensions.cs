using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace DLaB.ModelBuilderExtensions
{
    public static class Extensions
    {
        private const string XrmAttributeLogicalName = "Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute";
        private const string XrmRelationshipSchemaName = "Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute";

        #region AttributeMetadata

        public static bool IsOptionSet(this AttributeMetadata attribute)
        {
            var type = attribute.AttributeType;
            return type == AttributeTypeCode.State
                   || type == AttributeTypeCode.Status
                   || type == AttributeTypeCode.Picklist
                   || type == AttributeTypeCode.Virtual && attribute is EnumAttributeMetadata;
        }

        #endregion AttributeMetadata

        #region CodeCompileUnit

        public static List<Tuple<CodeTypeDeclaration, EntityMetadata>> GetEntityTypes(this CodeCompileUnit codeUnit, Dictionary<string, EntityMetadata> entityTypesByLogicalName)
        {
            var entityTypes = new List<Tuple<CodeTypeDeclaration, EntityMetadata>>();
            foreach (var type in codeUnit.GetEntityTypes())
            {
                var typeEntityName = GetEntityLogicalName(type);
                if (entityTypesByLogicalName.TryGetValue(typeEntityName, out var entityMetadata))
                {
                    entityTypes.Add(new Tuple<CodeTypeDeclaration, EntityMetadata>(type, entityMetadata));
                }
            }

            return entityTypes;
        }

        public static IEnumerable<CodeTypeDeclaration> GetEntityTypes(this CodeCompileUnit codeUnit)
        {
            return codeUnit.GetTypes().Where(type => type.IsClass
                                                     && type.IsEntityType());
        }

        public static IEnumerable<CodeTypeDeclaration> GetMessageTypes(this CodeCompileUnit codeUnit)
        {
            return codeUnit.GetTypes().Where(type => type.IsClass
                                                     && type.IsMessageType());
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

        #region CodeNamespaceCollection

        public static IEnumerable<CodeTypeDeclaration> GetTypes(this CodeNamespace codeNamespace)
        {
            for (var j = 0; j < codeNamespace.Types.Count; j++)
            {
                yield return codeNamespace.Types[j];
            }
        }

        #endregion CodeNamespaceCollection

        #region CodeTypeDeclaration

        /// <summary>
        /// System.CodeDom.CodeMemberEvent
        /// System.CodeDom.CodeMemberField
        /// System.CodeDom.CodeMemberMethod
        /// System.CodeDom.CodeMemberProperty
        /// System.CodeDom.CodeSnippetTypeMember
        /// System.CodeDom.CodeTypeDeclaration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetMembers<T>(this CodeTypeDeclaration type) where T: CodeTypeMember
        {
            for (var i = 0; i < type.Members.Count; ++i)
            {
                if (type.Members[i] is T @event)
                {
                    yield return @event;
                }
            }
        }

        #endregion CodeTypeDeclaration

        public static IEnumerable<CodeTypeReference> GetTypes(this CodeTypeReferenceCollection collection)
        {
            for (var i = 0; i < collection.Count; ++i)
            {
                yield return collection[i];
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

        #region CodeNamespace

        public static CodeNamespace CloneShallow(this CodeNamespace code)
        {
            var clone = new CodeNamespace(code.Name);
            clone.Comments.AddRange(code.Comments);
            var imports = new CodeNamespaceImport[code.Imports.Count];
            for (var i = 0; i < imports.Length; i++)
            {
                imports[i] = code.Imports[i];
            }

            clone.Imports.AddRange(imports);
            clone.Types.AddRange(code.Types);
            return clone;
        }

        public static CodeNamespace OrderTypesByName(this CodeNamespace code)
        {
            var types = code.GetTypes().OrderBy(t => t.Name).ToArray();
            code.Types.Clear();
            code.Types.AddRange(types);
            return code;
        }

        #endregion CodeNamespace

        #region CodeTypeDeclaration

        public static string GetFieldInitializedValue(this CodeTypeDeclaration type, string fieldName)
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
                return ((CodePrimitiveExpression)type.GetCustomAttribute("Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute")?.Arguments[0].Value)?.Value.ToString();
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
            if (type.BaseTypes.Count == 0)
            {
                return false;
            }

            var baseType = type.BaseTypes[0].BaseType;
            return baseType == "Microsoft.Xrm.Sdk.Client.OrganizationServiceContext";
        }

        /// <summary>
        /// Determines if the type inherits from one of the known Message types.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static bool IsMessageType(this CodeTypeDeclaration type)
        {
            if (type.BaseTypes.Count == 0)
            {
                return false;
            }

            var baseType = type.BaseTypes[0].BaseType;
            return baseType == "Microsoft.Xrm.Sdk.OrganizationRequest"
                   || baseType == "Microsoft.Xrm.Sdk.OrganizationResponse";
        }

        public static bool IsEntityType(this CodeTypeDeclaration type)
        {
            return type.GetCustomAttribute("Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute") != null;
        }

        #endregion // CodeTypeDeclaration

        #region IDictionary<string,string>

        public static string GetFirstKey(this IDictionary<string, string> dict, params string[] keys)
        {
            foreach (var key in keys)
            {
                if (dict.TryGetValue(key, out var value))
                {
                    return value;
                }
            }

            return null;
        }

        #endregion Dictionary<string,string>

        #region IServiceProvider

        public static void UpdateService<T>(this IServiceProvider services, T service)
        {
            var dictionaryField = services.GetType().GetField("_services", BindingFlags.NonPublic | BindingFlags.Instance);
            if (dictionaryField == null)
            {
                throw new NotImplementedException("Unable to determine the dictionary field for the IServiceProvider in UpdateService!");
            }
            var dictionary = (Dictionary<Type, object>)dictionaryField.GetValue(services);
            dictionary[typeof(T)] = service;
        }

        #endregion IServiceProvider

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
            if (metadata is BooleanOptionSetMetadata booleanOptionSet)
            {
                options = new List<OptionMetadata>
                {
                    booleanOptionSet.FalseOption,
                    booleanOptionSet.TrueOption
                };
            }
            else if (metadata is OptionSetMetadata nonBooleanOptionSet)
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

        /// <summary>
        /// If the path is not already rooted, uses the defaultRootDirectory to root it.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="defaultRootDirectory">Defaults to the AppDomain.CurrentDomain.BaseDirectory if not provided</param>
        /// <returns></returns>
        public static string RootPath(this string filePath, string defaultRootDirectory = null)
        {
            return Path.IsPathRooted(filePath)
                ? filePath
                : Path.GetFullPath(Path.Combine(defaultRootDirectory ?? AppDomain.CurrentDomain.BaseDirectory, filePath));
        }

        #endregion String
    }
}
