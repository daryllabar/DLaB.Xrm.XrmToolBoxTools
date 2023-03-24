using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;

namespace DLaB.ModelBuilderExtensions
{
    public class DLaBModelBuilder
    {
        /// <summary>
        /// No Config
        /// </summary>
        [JsonPropertyName("attributeConstsClassName")]
        public string AttributeConstsClassName { get; set; }

        [JsonPropertyName("addDebuggerNonUserCode")]
        public bool AddDebuggerNonUserCode { get; set; }

        [JsonPropertyName("addNewFilesToProject")] 
        public bool AddNewFilesToProject { get; set; }

        [JsonPropertyName("addOptionSetMetadataAttribute")]
        public bool AddOptionSetMetadataAttribute { get; set; }

        /// <summary>
        /// No Config
        /// </summary>
        [JsonPropertyName("addPrimaryAttributeConsts")]
        public bool AddPrimaryAttributeConsts { get; set; }

        [JsonPropertyName("builderSettingsJsonRelativePath")]
        public string BuilderSettingsJsonRelativePath { get; set; }

        [JsonPropertyName("camelCaseClassNames")]
        public bool CamelCaseClassNames { get; set; }

        [JsonPropertyName("camelCaseMemberNames")]
        public bool CamelCaseMemberNames { get; set; }

        /// <summary>
        /// No Config
        /// </summary>
        [JsonPropertyName("camelCaseOptionSetNames")]
        public bool CamelCaseOptionSetNames { get; set; }

        private string _camelCaseNamesDictionaryRelativePath;
        [JsonPropertyName("camelCaseNamesDictionaryRelativePath")]
        public string CamelCaseNamesDictionaryPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_camelCaseNamesDictionaryRelativePath))
                {
                    return _camelCaseNamesDictionaryRelativePath;
                }

                return Path.IsPathRooted(_camelCaseNamesDictionaryRelativePath)
                    ? _camelCaseNamesDictionaryRelativePath
                    : Path.Combine(XrmToolBoxPluginPath ?? Directory.GetCurrentDirectory(), _camelCaseNamesDictionaryRelativePath);
            }
            set => _camelCaseNamesDictionaryRelativePath = value;
        }

        /// <summary>
        /// No Config
        /// </summary>
        [JsonPropertyName("createBaseClasses")]
        public bool CreateBaseClasses { get; set; }

        [JsonPropertyName("createOneFilePerAction")]
        public bool CreateOneFilePerAction { get; set; }

        [JsonPropertyName("createOneFilePerEntity")]
        public bool CreateOneFilePerEntity { get; set; }

        [JsonPropertyName("createOneFilePerOptionSet")]
        public bool CreateOneFilePerOptionSet { get; set; }

        [JsonPropertyName("deleteFilesFromOutputFolders")]
        public bool DeleteFilesFromOutputFolders { get; set; }

        /// <summary>
        /// No Config
        /// </summary>
        [JsonPropertyName("enableFileDataType")]
        public bool EnableFileDataType { get; set; }

        [JsonPropertyName("entityAttributeSpecifiedNames")]
        public Dictionary<string, HashSet<string>> EntityAttributeSpecifiedNames { get; set; }

        [JsonPropertyName("entityPrefixesToSkip")]
        public List<string> EntityPrefixesToSkip { get; set; }

        [JsonPropertyName("entitiesToSkip")]
        public List<string> EntitiesToSkip { get; set; }

        [JsonPropertyName("generateActionAttributeNameConsts")]
        public bool GenerateActionAttributeNameConsts { get; set; }

        [JsonPropertyName("generateAttributeNameConsts")]
        public bool GenerateAttributeNameConsts { get; set; }

        [JsonPropertyName("generateAnonymousTypeConstructor")]
        public bool GenerateAnonymousTypeConstructor { get; set; }

        [JsonPropertyName("generateConstructorsSansLogicalName")]
        public bool GenerateConstructorsSansLogicalName { get; set; }

        [JsonPropertyName("generateEntityRelationships")]
        public bool GenerateEntityRelationships { get; set; }

        [JsonPropertyName("generateEntityTypeCode")]
        public bool GenerateEntityTypeCode { get; set; }

        [JsonPropertyName("generateEnumProperties")]
        public bool GenerateEnumProperties { get; set; }

        [JsonPropertyName("generateOnlyReferencedOptionSets")]
        public bool GenerateOnlyReferencedOptionSets { get; set; }

        [JsonPropertyName("generateOptionSetMetadataAttribute")]
        public bool GenerateOptionSetMetadataAttribute { get; set; }

        [JsonPropertyName("generateSeparateFiles")]
        public bool GenerateSeparateFiles { get; set; }

        [JsonPropertyName("groupLocalOptionSetsByEntity")]
        public bool GroupLocalOptionSetsByEntity { get; set; }

        [JsonPropertyName("invalidCSharpNamePrefix")]
        public string InvalidCSharpNamePrefix { get; set; }

        [JsonPropertyName("localOptionSetFormat")]
        public string LocalOptionSetFormat { get; set; }

        [JsonPropertyName("makeAllFieldsEditable")]
        public bool MakeAllFieldsEditable { get; set; }

        [JsonPropertyName("makeReadonlyFieldsEditable")]
        public bool MakeReadonlyFieldsEditable { get; set; }

        [JsonPropertyName("makeResponseActionsEditable")]
        public bool MakeResponseActionsEditable { get; set; }

        [JsonPropertyName("messagePrefixesToSkip")]
        public List<string> MessagePrefixesToSkip { get; set; }

        [JsonPropertyName("messageToSkip")]
        public List<string> MessageToSkip { get; set; }

        private string _optionSetLanguageCodeOverride;
        [JsonPropertyName("optionSetLanguageCodeOverride")]
        public string OptionSetLanguageCodeOverrideString {
            get => _optionSetLanguageCodeOverride;
            set
            {
                _optionSetLanguageCodeOverride = value;
                OptionSetLanguageCodeOverride = string.IsNullOrWhiteSpace(value)
                    ? int.MinValue
                    : int.Parse(value);
            }
        }

        [JsonPropertyName("optionSetNames")]
        public Dictionary<string, string> OptionSetNames { get; set; }

        [JsonIgnore]
        public int OptionSetLanguageCodeOverride { get; set; }

        [JsonPropertyName("readSerializedMetadata")]
        public bool ReadSerializedMetadata { get; set; }

        [JsonPropertyName("removeRuntimeVersionComment")]
        public bool RemoveRuntimeVersionComment { get; set; }
        /// <summary>
        /// No Config
        /// </summary>
        [JsonPropertyName("relationshipConstsClassName")]
        public string RelationshipConstsClassName { get; set; }

        [JsonPropertyName("replaceOptionSetPropertiesWithEnum")]
        public bool ReplaceOptionSetPropertiesWithEnum { get; set; }

        [JsonPropertyName("serializeMetadata")]
        public bool SerializeMetadata { get; set; }

        [JsonPropertyName("tokenCapitalizationOverrides")]
        public List<string> TokenCapitalizationOverrides { get; set; }

        /// <summary>
        /// No Config
        /// </summary>
        [JsonPropertyName("updateEnumerableEntityProperties")]
        public bool UpdateEnumerableEntityProperties { get; set; }

        /// <summary>
        /// No Config
        /// </summary>
        [JsonPropertyName("updateMultiOptionSetAttributes")]
        public bool UpdateMultiOptionSetAttributes { get; set; }

        [JsonPropertyName("useLogicalNames")]
        public bool UseLogicalNames { get; set; }

        [JsonPropertyName("useTfsToCheckoutFiles")]
        public bool UseTfsToCheckoutFiles { get; set; }

        [JsonPropertyName("validCSharpNameRegEx")]
        public string ValidCSharpNameRegEx { get; set; }

        [JsonPropertyName("waitForAttachedDebugger")]
        public bool WaitForAttachedDebugger { get; set; }

        [JsonPropertyName("xrmToolBoxPluginPath")]
        public string XrmToolBoxPluginPath { get; set; }

        public DLaBModelBuilder()
        {
            AttributeConstsClassName = "Fields";
            EnableFileDataType = true;
            EntitiesToSkip = new List<string>();
            EntityAttributeSpecifiedNames = new Dictionary<string, HashSet<string>>();
            EntityPrefixesToSkip = new List<string>();
            GenerateEntityRelationships = true;
            InvalidCSharpNamePrefix = "_";
            LocalOptionSetFormat = "{0}_{1}";
            MessagePrefixesToSkip = new List<string>();
            MessageToSkip = new List<string>();
            OptionSetNames = new Dictionary<string, string>();
            RelationshipConstsClassName = "Relationships";
            TokenCapitalizationOverrides = new List<string>();
            ValidCSharpNameRegEx = @"[^a-zA-Z0-9_]";
        }
    }

    public class DLaBModelBuilderSettings
    {
        [JsonPropertyName("dLaB.ModelBuilder")]
        public DLaBModelBuilder DLaBModelBuilder { get; set; }

        [JsonPropertyName("emitFieldsClasses")]
        public bool EmitFieldsClasses { get; set; }

        [JsonPropertyName("entityNamesFilter")]
        public List<string> EntityNamesFilter { get; set; }

        [JsonPropertyName("entityTypesFolder")]
        public string EntityTypesFolder { get; set; }

        [JsonPropertyName("generateGlobalOptionSets")]
        public bool GenerateGlobalOptionSets { get; set; }

        [JsonPropertyName("messageNamesFilter")]
        public List<string> MessageNamesFilter { get; set; }

        [JsonPropertyName("messagesTypesFolder")]
        public string MessagesTypesFolder { get; set; }

        [JsonPropertyName("namespace")]
        public string Namespace { get; set; }

        [JsonPropertyName("namingService")]
        public string NamingService { get; set; }

        [JsonPropertyName("optionSetsTypesFolder")]
        public string OptionSetsTypesFolder { get; set; }

        [JsonPropertyName("serviceContextName")]
        public string ServiceContextName { get; set; }

        [JsonPropertyName("suppressGeneratedCodeAttribute")]
        public bool SuppressGeneratedCodeAttribute { get; set; }

        [JsonPropertyName("suppressINotifyPattern")]
        public bool SuppressINotifyPattern { get; set; }

        public DLaBModelBuilderSettings()
        {
            DLaBModelBuilder = new DLaBModelBuilder();
            EntityNamesFilter = new List<string>();
            MessageNamesFilter = new List<string>();
        }
    }


}
