using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DLaB.ModelBuilderExtensions
{
    public class DLaBModelBuilder
    {
        /// <summary>
        /// No Config
        /// </summary>
        [JsonPropertyName("messageLogicalFieldName")]
        public string MessageLogicalFieldName { get; set; }

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

        [JsonPropertyName("adjustCasingForEnumOptions")]
        public bool AdjustCasingForEnumOptions { get; set; }

        /// <summary>
        /// No Config
        /// </summary>
        [JsonPropertyName("attributeConstsClassName")]
        public string AttributeConstsClassName { get; set; }

        [JsonPropertyName("builderSettingsJsonRelativePath")]
        public string BuilderSettingsJsonRelativePath { get; set; }

        [JsonPropertyName("camelCaseClassNames")]
        public bool CamelCaseClassNames { get; set; }

        [JsonPropertyName("camelCaseCustomWords")]
        public List<string> CamelCaseCustomWords { get; set; }

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
            get => _camelCaseNamesDictionaryRelativePath.RootPath(XrmToolBoxPluginPath);
            set => _camelCaseNamesDictionaryRelativePath = value;
        }

        [JsonPropertyName("cleanupCrmSvcUtilLocalOptionSets")]
        public bool CleanupCrmSvcUtilLocalOptionSets { get; set; }

        [JsonPropertyName("createOneFilePerMessage")]
        public bool CreateOneFilePerMessage { get; set; }

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

        [JsonPropertyName("entitiesFileName")]
        public string EntitiesFileName { get; set; }

        [JsonPropertyName("entityBlacklist")]
        public List<string> EntityBlacklist { get; set; }

        [JsonPropertyName("entityClassNameOverrides")]
        public Dictionary<string, string> EntityClassNameOverrides { get; set; }

        [JsonPropertyName("entityRegExBlacklist")]
        public List<string> EntityRegExBlacklist { get; set; }

        [JsonPropertyName("filePrefixText")]
        public string FilePrefixText { get; set; }

        [JsonPropertyName("generateAllOptionSetLabelMetadata")]
        public bool GenerateAllOptionSetLabelMetadata { get; set; }

        [JsonPropertyName("generateMessageAttributeNameConsts")]
        public bool GenerateMessageAttributeNameConsts { get; set; }

        [JsonPropertyName("generateAnonymousTypeConstructor")]
        public bool GenerateAnonymousTypeConstructor { get; set; }

        [JsonPropertyName("generateConstructorsSansLogicalName")]
        public bool GenerateConstructorsSansLogicalName { get; set; }

        [JsonPropertyName("generateEntityRelationships")]
        public bool GenerateEntityRelationships { get; set; }

        [JsonPropertyName("generateOptionSetProperties")]
        public bool GenerateOptionSetProperties { get; set; }

        [JsonPropertyName("generateOptionSetMetadataAttribute")]
        public bool GenerateOptionSetMetadataAttribute { get; set; }

        [JsonPropertyName("generateTypesAsInternal")]
        public bool GenerateTypesAsInternal { get; set; }

        [JsonPropertyName("groupLocalOptionSetsByEntity")]
        public bool GroupLocalOptionSetsByEntity { get; set; }

        [JsonPropertyName("groupMessageRequestWithResponse")]
        public bool GroupMessageRequestWithResponse { get; set; }

        [JsonPropertyName("includeCommandLine")]
        public bool IncludeCommandLine { get; set; }

        [JsonPropertyName("invalidCSharpNamePrefix")]
        public string InvalidCSharpNamePrefix { get; set; }
        
        /// <summary>
        /// No Config
        /// </summary>
        [JsonPropertyName("labelTextReplacement")]
        public Dictionary<string, string> LabelTextReplacement { get; set; }

        [JsonPropertyName("localOptionSetFormat")]
        public string LocalOptionSetFormat { get; set; }

        /// <summary>
        /// No Config
        /// </summary>
        [JsonPropertyName("loggingEnabled")]
        public bool LoggingEnabled { get; set; }

        [JsonPropertyName("makeAllFieldsEditable")]
        public bool MakeAllFieldsEditable { get; set; }

        [JsonPropertyName("makeReadonlyFieldsEditable")]
        public bool MakeReadonlyFieldsEditable { get; set; }

        [JsonPropertyName("makeResponseMessagesEditable")]
        public bool MakeResponseMessagesEditable { get; set; }

        [JsonPropertyName("messageBlacklist")]
        public List<string> MessageBlacklist { get; set; }

        [JsonPropertyName("messagesFileName")]
        public string MessagesFileName { get; set; }

        [JsonPropertyName("modelBuilderLogLevel")]
        public string ModelBuilderLogLevel { get; set; }

        [JsonPropertyName("optionSetsFileName")]
        public string OptionSetsFileName { get; set; }

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

        /// <summary>
        /// No Config
        /// </summary>
        [JsonPropertyName("orgEntityClassName")]
        public string OrgEntityClassName { get; set; }

        [JsonPropertyName("outputRelativeDirectory")]
        public string OutputRelativeDirectory { get; set; }

        [JsonPropertyName("projectNameForEarlyBoundFiles")]
        public string ProjectNameForEarlyBoundFiles { get; set; }

        [JsonPropertyName("propertyEnumMappings")]
        public Dictionary<string, string> PropertyEnumMappings { get; set; }

        [JsonPropertyName("readSerializedMetadata")]
        public bool ReadSerializedMetadata { get; set; }

        [JsonPropertyName("removeRuntimeVersionComment")]
        public bool RemoveRuntimeVersionComment { get; set; }
        /// <summary>
        /// No Config
        /// </summary>
        [JsonPropertyName("relationshipConstsClassName")]
        public string RelationshipConstsClassName { get; set; }

        [JsonPropertyName("replaceEnumPropertiesWithOptionSet")]
        public bool ReplaceEnumPropertiesWithOptionSet { get; set; }

        /// <summary>
        /// No Config
        /// </summary>
        [JsonPropertyName("serializedMetadataRelativeFilePath")]
        public string SerializedMetadataRelativeFilePath { get; set; }

        [JsonPropertyName("serializeMetadata")]
        public bool SerializeMetadata { get; set; }

        [JsonPropertyName("suppressAutogeneratedFileHeaderComment")]
        public bool SuppressAutogeneratedFileHeaderComment { get; set; }

        [JsonPropertyName("tokenCapitalizationOverrides")]
        public List<string> TokenCapitalizationOverrides { get; set; }

        private string _transliterationRelativePath;
        [JsonPropertyName("transliterationRelativePath")]
        public string TransliterationPath
        {
            get => _transliterationRelativePath.RootPath(XrmToolBoxPluginPath);
            set => _transliterationRelativePath = value;
        }

        /// <summary>
        /// No Config.  Fix for https://github.com/daryllabar/DLaB.Xrm.XrmToolBoxTools/issues/464
        /// </summary>
        [JsonPropertyName("updateEnumerableEntityProperties")]
        public bool UpdateEnumerableEntityProperties { get; set; }

        /// <summary>
        /// No Config
        /// </summary>
        [JsonPropertyName("updateMultiOptionSetAttributes")]
        public bool UpdateMultiOptionSetAttributes { get; set; }

        [JsonPropertyName("useCrmSvcUtilStateEnumNamingConvention")]
        public bool UseCrmSvcUtilStateEnumNamingConvention { get; set; }

        [JsonPropertyName("useDisplayNameForBpfName")]
        public bool UseDisplayNameForBpfName { get; set; }

        [JsonPropertyName("useEnumForStateCodes")]
        public bool UseEnumForStateCodes { get; set; }

        /// <summary>
        /// No Config
        /// </summary>
        [JsonPropertyName("userEntityClassName")]
        public string UserEntityClassName { get; set; }

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
            MessageLogicalFieldName = "ActionLogicalName";
            AddPrimaryAttributeConsts = true;
            AttributeConstsClassName = "Fields";
            CamelCaseCustomWords = new List<string>();
            EnableFileDataType = true;
            EntitiesFileName = "Entities.cs";
            EntityAttributeSpecifiedNames = new Dictionary<string, HashSet<string>>();
            EntityBlacklist = new List<string>();
            EntityClassNameOverrides = new Dictionary<string, string>();
            EntityRegExBlacklist = new List<string>();
            GenerateEntityRelationships = true;
            GroupMessageRequestWithResponse = true;
            InvalidCSharpNamePrefix = "_";
            LabelTextReplacement = new Dictionary<string, string>()
            {
                { "$", string.Empty },
                { "(", string.Empty }
            };
            LocalOptionSetFormat = "{0}_{1}";
            MessageBlacklist = new List<string>();
            MessagesFileName = "Messages.cs";
            SerializedMetadataRelativeFilePath = "metadata.xml";
            OptionSetNames = new Dictionary<string, string>();
            OptionSetsFileName = "OptionSet.cs";
            OrgEntityClassName = "OrganizationOwnedEntity";
            PropertyEnumMappings = new Dictionary<string, string>();
            RelationshipConstsClassName = "Relationships";
            RemoveRuntimeVersionComment = true;
            TokenCapitalizationOverrides = new List<string>();
            UpdateEnumerableEntityProperties = true;
            UserEntityClassName = "UserOwnedEntity";
            ValidCSharpNameRegEx = @"[^a-zA-Z0-9_]";
        }
    }

    public class DLaBModelBuilderSettings
    {
        [JsonPropertyName("dLaB.ModelBuilder")]
        public DLaBModelBuilder DLaBModelBuilder { get; set; } = new DLaBModelBuilder();

        #region ModelBuilder Properties
        // ReSharper disable InconsistentNaming

        [JsonPropertyName("emitEntityETC")]
        public bool EmitEntityETC { get; set; }

        [JsonPropertyName("emitFieldsClasses")]
        public bool EmitFieldsClasses { get; set; }
        
        [JsonPropertyName("entityNamesFilter")]
        public List<string> EntityNamesFilter { get; set; } = new List<string>();

        [JsonPropertyName("emitVirtualAttributes")]
        public bool EmitVirtualAttributes { get; set; }

    [JsonPropertyName("entityTypesFolder")]
        public string EntityTypesFolder { get; set; }

        [JsonPropertyName("generateActions")]
        public bool GenerateActions { get; set; }

        [JsonPropertyName("generateGlobalOptionSets")]
        public bool GenerateGlobalOptionSets { get; set; }

        [JsonPropertyName("messageNamesFilter")]
        public List<string> MessageNamesFilter { get; set; } = new List<string>();

        [JsonPropertyName("messagesTypesFolder")]
        public string MessagesTypesFolder { get; set; }

        [JsonPropertyName("namespace")]
        public string Namespace { get; set; }

        [JsonPropertyName("namingService")]
        public string NamingService { get; set; }

        [JsonPropertyName("optionSetsTypesFolder")]
        public string OptionSetsTypesFolder { get; set; }

        /// <summary>
        /// Pulled from the parameters in the ConfigHelper.Initialize
        /// </summary>
        [JsonPropertyName("outDirectory")]
        public string OutDirectory { get; set; }

        [JsonPropertyName("serviceContextName")]
        public string ServiceContextName { get; set; }

        [JsonPropertyName("suppressGeneratedCodeAttribute")]
        public bool SuppressGeneratedCodeAttribute { get; set; }

        [JsonPropertyName("suppressINotifyPattern")]
        public bool SuppressINotifyPattern { get; set; }

        // ReSharper restore InconsistentNaming
        #endregion ModelBuilder Properties
    }


}
