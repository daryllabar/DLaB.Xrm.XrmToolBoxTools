using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Serialization;
using static DLaB.EarlyBoundGeneratorV2.Settings.EarlyBoundGeneratorConfig;

namespace DLaB.EarlyBoundGeneratorV2.Settings
{
    /// <summary>
    /// Serializable class containing all settings that will get written to the templateSettings.json
    /// </summary>
    [Serializable]
    public class ExtensionConfig
    {
        /// <summary>
        /// Pipe Delimited String containing the prefixes of Actions to be generated.
        /// </summary>
        public string ActionPrefixesWhitelist { get; set; }
        /// <summary>
        /// Pipe Delimited String containing the logical names of Actions to be generated.
        /// </summary>
        public string ActionsWhitelist { get; set; }
        /// <summary>
        /// Pipe Delimited String containing the logical names of Actions to not generate
        /// </summary>
        public string ActionsToSkip { get; set; }
        /// <summary>
        /// Specifies that the debugger should skip stepping into generated entity files.
        /// </summary>
        public bool AddDebuggerNonUserCode { get; set; }
        /// <summary>
        /// Adds newly created files to Project File
        /// </summary>
        public bool AddNewFilesToProject { get; set; }
        /// <summary>
        /// Adds the OptionSetMetadataAttribute to enums to be able to access enum metadata.
        /// Ensure Generate Option Set Metadata Attribute is true to generate the attribute definition, unless this has been handled in some other manner.
        /// </summary>
        public bool AddOptionSetMetadataAttribute { get; set; }
        /// <summary>
        /// By default, the Dataverse Model Builder will convert the Option labels into a C# friendly name, and not adjust the casing of enum.  This could results in a Label of "This is a very long label with some capitals (JSON / XML / CSV)" being created as ThisisaverylonglabelwithsomecapitalsJSONXMLCSV, instead of a much more readable ThisIsAVeryLongLabelWithSomeCapitalsJsonXmlCsv,
        /// </summary>
        public bool AdjustCasingForEnumOptions { get; set; }
        /// <summary>
        /// This is relative to the Path of the Settings File.  This should end with the a ".json" extension.
        /// </summary>
        public string BuilderSettingsJsonRelativePath { get; set; }
        /// <summary>
        /// Using a dictionary, attempts to correctly camelcase class/enum names.
        /// </summary>
        public bool CamelCaseClassNames { get; set; }
        /// <summary>
        /// Custom words to add to the standard dictionary file.  Allows for more control of the camel case naming convention, without having to check the entire dictionary file in.
        /// </summary>
        public string CamelCaseCustomWords { get; set; }
        /// <summary>
        /// Using a dictionary, attempts to correctly camelcase column/parameter names
        /// </summary>
        public bool CamelCaseMemberNames { get; set; }
        /// <summary>
        /// The Camel Case Dictionary Relative Path.
        /// </summary>
        public string CamelCaseNamesDictionaryRelativePath { get; set; }
        /// <summary>
        /// The PAC ModelBuilder generates local option sets in the same files as the entity class, but the CrmSvcUtil created these option sets separately.  This setting will look for these files and delete them from the disk.  After the upgrade, this should be disabled.  It is recommended to enable "1 - Global - Add New Files To Project" as well to cleanup the files from the project.
        /// </summary>
        public bool CleanupCrmSvcUtilLocalOptionSets { get; set; }
        /// <summary>
        /// Specifies that each Action class should be outputted to it's own file
        /// </summary>
        public bool CreateOneFilePerAction { get; set; }
        /// <summary>
        /// Specifies that each Entity class should be outputted to it's own file
        /// </summary>
        public bool CreateOneFilePerEntity { get; set; }
        /// <summary>
        /// Specifies that each OptionSet class should be outputted to it's own file
        /// </summary>
        public bool CreateOneFilePerOptionSet { get; set; }
        /// <summary>
        /// Clears all .cs files from output folders prior to file generation.
        /// This helps to remove files that are no longer being generated.
        /// Only used if Create One File Per Action/Entity/OptionSet is true.
        /// </summary>
        public bool DeleteFilesFromOutputFolders { get; set; }
        public string EntitiesToSkip { get; set; }
        /// <summary>
        /// Pipe Delimited String containing the logical names of Entities to generate.  If empty, all Entities will be included.
        /// </summary>
        public string EntitiesWhitelist { get; set; }
        /// <summary>
        /// Formatted as a single string in the format of "EntityName1:firstAttributeName, ... ,lastAttributeName|EntityName2:firstAttributeName, ... ,lastAttributeName|..."
        /// Basically split each entity by pipe, use then split by colon, then comma, with the first value being the entityName
        /// Allows for the ability to specify the capitalization of an attribute on an entity
        /// </summary>
        public string EntityAttributeSpecifiedNames { get; set; }
        /// <summary>
        /// Allows for specifying a specific name or casing for an Entity class.  For example, if the business refers to an 'Account' as a 'Family', specifying a mapping from 'account' to 'Family' will result in the name of the C# class being 'Family', even though the logical name would still be 'account'.
        /// </summary>
        public string EntityClassNameOverrides { get; set; }
        /// <summary>
        /// Pipe delimited string containing prefixes of entities to not generate.
        /// </summary>
        public string EntityPrefixesToSkip { get; set; }
        /// <summary>
        /// Pipe delimited string containing wildcard capable values of entities to be generated.
        /// </summary>
        public string EntityPrefixesWhitelist { get; set; }
        /// <summary>
        /// Text that appears at the top of every  file.  Could be used to include Copyright information or #Paragma warning disable statements
        /// </summary>
        public string FilePrefixText { get; set; }
        /// <summary>
        /// Specifies the generation of an Attributes Struct containing logical names for all attributes for the Action
        /// </summary>
        public bool GenerateActionAttributeNameConsts { get; set; }
        /// <summary>
        /// Generates all labels by language code in the Option Set Metadata Attributes
        /// </summary>
        public bool GenerateAllOptionSetLabelMetadata { get; set; }
        /// <summary>
        /// Specifies the generation of an Attributes Struct containing logical names for all attributes for the Entity
        /// </summary>
        public bool GenerateAttributeNameConsts { get; set; }
        /// <summary>
        /// Specifies the generation of AnonymousType Constructors for entities
        /// </summary>
        public bool GenerateAnonymousTypeConstructor { get; set; }
        /// <summary>
        /// Adds Constructors to each early bound entity class to use constructors available to Microsoft.Xrm.Sdk.Entity
        /// </summary>
        public bool GenerateConstructorsSansLogicalName { get; set; }
        /// <summary>
        /// Specifies the generation of Relationships properties for Entities
        /// </summary>
        public bool GenerateEntityRelationships { get; set; }
        /// <summary>
        /// Specifies the generation of Enum properties for option sets
        /// </summary>                                          
        public bool GenerateEnumProperties { get; set; }
        /// <summary>
        /// Generate all Global OptionSets, note: if an entity contains a reference to a global optionset, it will be emitted even if this switch is not present.
        /// </summary>
        public bool GenerateGlobalOptionSets { get; set; }
        /// <summary>
        /// Specifies that Entity class should implement the INotifyPropertyChanging and INotifyPropertyChanged interfaces, calling OnPropertyChanging and OnPropertyChanged for each set of an attributes
        /// </summary>
        public bool GenerateINotifyPattern { get; set; }
        /// <summary>
        /// Generates an OptionSetMetadataAttribute class used to allow for storing of the metadata of OptionSetValues i.e. display order, name, description, etc.
        /// Only used if Add Option Set Metadata Attribute is true.
        /// </summary>
        public bool GenerateOptionSetMetadataAttribute { get; set; }
        /// <summary>
        /// All generated types are marked as "internal" instead of "public".
        /// </summary>
        public bool GenerateTypesAsInternal { get; set; }
        /// <summary>
        /// Combines all local option sets into a single file per entity.  Only used if Create One File Per Option Set is true.
        /// </summary>
        public bool GroupLocalOptionSetsByEntity { get; set; }
        /// <summary>
        /// The CrmSvcUtil version of the Early Bound Generator created seperate files for each message request/response class, but the PAC ModelBuilder combines each request/response pair into a single file.  This allows for turning this off to maintain backwards compability.
        /// </summary>
        public bool GroupMessageRequestWithResponse { get; set; }
        /// <summary>
        /// Specifies the Prefix to be used for OptionSets that would normally start with an invalid first character ie "1st"
        /// </summary>
        public string InvalidCSharpNamePrefix { get; set; }
        /// <summary>
        /// Defines that Entities should be created with all attributes as editable.
        /// Helpful for writing linq statements where those attributes are wanting to be returned in the select.
        /// </summary>
        public bool MakeAllFieldsEditable { get; set; }
        /// <summary>
        /// Defines that Entities should be created with editable createdby, createdon, modifiedby, modifiedon, owningbusinessunit, owningteam, and owninguser properties.
        /// Helpful for writing linq statements where those attributes are wanting to be returned in the select
        /// </summary>
        public bool MakeReadonlyFieldsEditable { get; set; }
        /// <summary>
        /// Specifies that the properties of Response Actions should be editable.
        /// </summary>
        public bool MakeResponseActionsEditable { get; set; }
        /// <summary>
        /// Defines the format of Local Option Sets where {0} is the Entity Schema Name, and {1} is the Attribute Schema Name.  
        /// The format Specified in the SDK is {0}{1}, but the default is {0}_{1}, but used to be prefix_{0}_{1}(all lower case)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  
        /// </summary>
        public string LocalOptionSetFormat { get; set; }
        /// <summary>
        /// Trace = 0 - Logs that contain the most detailed messages. These messages may contain sensitive application data.  These messages are disabled by default and should never be enabled in a production environment.
        /// Debug = 1 - Logs that are used for interactive investigation during development.  These logs should primarily contain information useful for debugging and have no long-term value.
        /// Information = 2 - Logs that track the general flow of the application. These logs should have long-term value.
        /// Warning = 3 - Logs that highlight an abnormal or unexpected event in the application flow, but do not otherwise cause the application execution to stop.
        /// Error = 4 - Logs that highlight when the current flow of execution is stopped due to a failure. These should indicate a failure in the current activity, not an application-wide failure.
        /// Critical = 5 - Logs that describe an unrecoverable application or system crash, or a catastrophic failure that requires immediate attention.
        /// None = 6 - Not used for writing log messages. Specifies that a logging category should not write any messages.
        /// </summary>
        public string ModelBuilderLogLevel { get; set; }
        /// <summary>
        /// Overrides the default (English:1033) language code used for generating Option Set Value names (the value, not the option set)
        /// </summary>
        public int? OptionSetLanguageCodeOverride { get; set; }
        /// <summary>
        /// If Option Sets have identical names to Entities, it will cause naming conflicts.  By default this is overcome by post-fixing "_Enum" to the Option Set name.  This setting allows a custom mapping for option set names to be specified.
        /// </summary>
        public string OptionSetNames { get; set; }
        /// <summary>
        /// By default, this is the directory of the Settings file.  But if populated, this directory will be used instead, and all other paths will be relative to this one.
        /// </summary>
        public string OutputRelativeDirectory { get; set; }
        /// <summary>
        /// The name of the project to add newly created files to. If not value is provided, the first one found will be used.
        /// </summary>
        public string ProjectNameForEarlyBoundFiles { get; set; }
        /// <summary>
        /// Used to manually specify an enum mapping for an OptionSetValue Property on an entity 
        /// Format: EntityName.PropertyName,EnumName|
        /// </summary>
        public string PropertyEnumMappings { get; set; }
        /// <summary>
        /// For Debugging Only!
        /// Used to not communicate to the server for the metadata, but to use the local metadata file instead.  Should only be used for testing generation outputs.
        /// Set Serialize Metadata to true first to connect to the server and retrieve the metadata, then set it back to false to not write it again since it's already local.
        /// </summary>
        public bool ReadSerializedMetadata { get; set; }
        /// <summary>
        /// Remove the Runtime Version in the header comment
        /// </summary>
        public bool RemoveRuntimeVersionComment { get; set; }
        /// <summary>
        /// Used in Conjunction with GenerateEnumProperties.  Allows for replacing the OptionSet properties, rather than duplicating them
        /// </summary>
        public bool ReplaceOptionSetPropertiesWithEnum { get; set; }
        /// <summary>
        /// For Debugging Only!
        /// Serializes the Metadata to a local file on disk.  (Generates a 200-400+ mb xml file).
        /// </summary>
        public bool SerializeMetadata { get; set; }
        /// <summary>
        /// Prevents generation of the <auto-generated> header comments.
        /// </summary>
        public bool SuppressAutogeneratedFileHeaderComment { get; set; }
        /// <summary>
        /// Used in conjunction with Camel Case Class Names and Camel Case Member Names to override any defaults.
        /// </summary>
        public string TokenCapitalizationOverrides { get; set; }
        /// <summary>
        /// The path relative, to the XrmToolBox Plugins directory, to a folder containing the language code json files to be used for transliteration.
        /// </summary>
        public string TransliterationRelativePath { get; set; }
        /// <summary>
        /// The CrmSvcUtil names entity statecode enums as "{EntityName}State", but the PAC ModelBuilder names them the same way all other enums are generated "{EntityName}_StateCode".  This allows for maintaining backwards compability.
        /// </summary>
        public bool UseCrmSvcUtilStateEnumNamingConvention { get; set; }
        /// <summary>
        /// By default, Business Process Flows entities are named with the format {PublisherPrefix}_bpf_GUID. This will instead generate the bpf clases with the format {PublisherPrefix}_bpf_{DisplayName} and entity relationships with similarly, guid replaced by display name, formats.
        /// </summary>
        public bool UseDisplayNameForBpfName { get; set; }
        /// <summary>
        /// The CrmSvcUtil generates state codes as enums properties.  This allows for generating just this property as an enum.  Only valid when Replace Option Set Properties with Enum is false.
        /// </summary>
        public bool UseEnumForStateCodes { get; set; }
        /// <summary>
        /// Creates Local OptionSets Using the Deprecated Naming Convention. prefix_oobentityname_prefix_attribute
        /// </summary>
        /// <value>
        /// <c>true</c> if [use Deprecated Option Set Naming]; otherwise, <c>false</c>.
        /// </value>
        /// <summary>
        /// Generate entities and properties with logical name instead of schema name.
        /// </summary>
        public bool UseLogicalNames { get; set; }
        /// <summary>
        /// Uses TFS to checkout files
        /// </summary>
        public bool UseTfsToCheckoutFiles { get; set; }
        /// <summary>
        /// For Debugging Only!
        /// Waits until a debugger is attached to the ModelBuilder before processing the command.
        /// </summary>
        public bool WaitForAttachedDebugger { get; set; }

        #region NonSerialized Properties

        /// <summary>
        /// Path of the Camel Case Names Dictionary file, relative to the Pac Or XrmToolBoxPluginPath, if not fully rooted
        /// </summary>
        [XmlIgnore]
        public string CamelCaseNamesDictionaryPath =>
            Directory.Exists(CamelCaseNamesDictionaryRelativePath)
                ? CamelCaseNamesDictionaryRelativePath
                : Path.Combine(XrmToolBoxPluginPath ?? Directory.GetCurrentDirectory(), CamelCaseNamesDictionaryRelativePath);

        /// <summary>
        /// Set during Execution
        /// </summary>
        [XmlIgnore]
        public string XrmToolBoxPluginPath { get; set; }

        #endregion // NonSerialized Properties

        public static ExtensionConfig GetDefault()
        {
            return new ExtensionConfig
            {
                ActionPrefixesWhitelist = null,
                ActionsToSkip = null,
                ActionsWhitelist = "analyze",
                AddDebuggerNonUserCode = true,
                AddNewFilesToProject = true,
                AddOptionSetMetadataAttribute = true,
                AdjustCasingForEnumOptions = true,
                BuilderSettingsJsonRelativePath = "builderSettings.json",
                CamelCaseClassNames = true,
                CamelCaseCustomWords = null,
                CamelCaseNamesDictionaryRelativePath = @"DLaB.EarlyBoundGeneratorV2\DLaB.Dictionary.txt",
                CamelCaseMemberNames = true,
                CleanupCrmSvcUtilLocalOptionSets = false,
                CreateOneFilePerAction = true,
                CreateOneFilePerEntity = true,
                CreateOneFilePerOptionSet = true,
                DeleteFilesFromOutputFolders = false,
                EntitiesToSkip = null,
                EntitiesWhitelist = "activityparty|activitypointer|businessunit|contact|email|systemuser|transactioncurrency",
                EntityAttributeSpecifiedNames = null,
                EntityClassNameOverrides = null,
                EntityPrefixesToSkip = null,
                EntityPrefixesWhitelist = null,
                FilePrefixText = null,
                GenerateActionAttributeNameConsts = true,
                GenerateAllOptionSetLabelMetadata = false,
                GenerateAnonymousTypeConstructor = true,
                GenerateAttributeNameConsts = true,
                GenerateConstructorsSansLogicalName = true,
                GenerateEntityRelationships = true,
                GenerateEnumProperties = true,
                GenerateGlobalOptionSets = false,
                GenerateINotifyPattern = false,
                GenerateOptionSetMetadataAttribute = true,
                GenerateTypesAsInternal = false,
                GroupLocalOptionSetsByEntity = false,
                GroupMessageRequestWithResponse = true,
                InvalidCSharpNamePrefix = "_",
                LocalOptionSetFormat = "{0}_{1}",
                MakeAllFieldsEditable = false,
                MakeReadonlyFieldsEditable = false,
                MakeResponseActionsEditable = true,
                ModelBuilderLogLevel = "2",
                OptionSetLanguageCodeOverride = null,
                OptionSetNames = null,
                OutputRelativeDirectory = null,
                ProjectNameForEarlyBoundFiles = string.Empty,
                PropertyEnumMappings = string.Empty,
                ReadSerializedMetadata = false,
                RemoveRuntimeVersionComment = true,
                ReplaceOptionSetPropertiesWithEnum = true,
                SerializeMetadata = false,
                SuppressAutogeneratedFileHeaderComment = false,
                TokenCapitalizationOverrides = "AccessTeam|ActiveState|AssignedTo|BusinessAs|CardUci|DefaultOnCase|EmailAnd|EmailSend|EmailSender|FeatureSet|FedEx|ForAn|Geronimo|IsMsTeams|IsPaiEnabled|IsSopIntegration|MsDynCe_|MsDynMkt_|MsDyUsd|O365Admin|OcSkillIdentMlModel|OnHold|OrderId|OwnerOnAssign|PauseStates|PredictiveAddress|PartiesOnEmail|ParticipatesIn|SentOn|SettingsAndSummary|SlaId|SlaKpi|SyncOptIn|Timeout|TradeShow|UserPuid|VoiceMail",
                TransliterationRelativePath = @"DLaB.EarlyBoundGeneratorV2\alphabets",
                UseCrmSvcUtilStateEnumNamingConvention = false,
                UseDisplayNameForBpfName = true,
                UseEnumForStateCodes = false,
                UseLogicalNames = false,
                UseTfsToCheckoutFiles = false,
                WaitForAttachedDebugger = false,
            };
        }

        /// <summary>
        /// Updates properties, only if actually populated in the poco.
        /// </summary>
        /// <param name="poco"></param>
        public void SetPopulatedValues(POCO.ExtensionConfig poco)
        {
            ActionPrefixesWhitelist = GetValueOrDefault(poco.ActionPrefixesWhitelist, ActionPrefixesWhitelist);
            ActionsWhitelist = poco.ActionsWhitelist ?? ActionsWhitelist;
            ActionsToSkip = GetValueOrDefault(poco.ActionsToSkip, ActionsToSkip);
            AddDebuggerNonUserCode = poco.AddDebuggerNonUserCode ?? AddDebuggerNonUserCode;
            AddNewFilesToProject = poco.AddNewFilesToProject ?? AddNewFilesToProject;
            AddOptionSetMetadataAttribute = poco.AddOptionSetMetadataAttribute ?? AddOptionSetMetadataAttribute;
            AdjustCasingForEnumOptions = poco.AdjustCasingForEnumOptions ?? AdjustCasingForEnumOptions;
            BuilderSettingsJsonRelativePath = GetValueOrDefault(poco.BuilderSettingsJsonRelativePath, BuilderSettingsJsonRelativePath);
            CamelCaseClassNames = poco.CamelCaseClassNames ?? CamelCaseClassNames;
            CamelCaseCustomWords = GetValueOrDefault(poco.CamelCaseCustomWords, CamelCaseCustomWords);
            CamelCaseNamesDictionaryRelativePath = poco.CamelCaseNamesDictionaryRelativePath ?? CamelCaseNamesDictionaryRelativePath;
            CamelCaseMemberNames = poco.CamelCaseMemberNames ?? CamelCaseMemberNames;
            CleanupCrmSvcUtilLocalOptionSets = poco.CleanupCrmSvcUtilLocalOptionSets ?? CleanupCrmSvcUtilLocalOptionSets;
            CreateOneFilePerAction = poco.CreateOneFilePerAction ?? CreateOneFilePerAction;
            CreateOneFilePerEntity = poco.CreateOneFilePerEntity ?? CreateOneFilePerEntity;
            CreateOneFilePerOptionSet = poco.CreateOneFilePerOptionSet ?? CreateOneFilePerOptionSet;
            DeleteFilesFromOutputFolders = poco.DeleteFilesFromOutputFolders ?? DeleteFilesFromOutputFolders;
            EntitiesToSkip = GetValueOrDefault(poco.EntitiesToSkip, EntitiesToSkip);
            EntitiesWhitelist = poco.EntitiesWhitelist ?? EntitiesWhitelist;
            EntityAttributeSpecifiedNames = GetValueOrDefault(poco.EntityAttributeSpecifiedNames, EntityAttributeSpecifiedNames);
            EntityClassNameOverrides = GetValueOrDefault(poco.EntityClassNameOverrides, EntityClassNameOverrides);
            EntityPrefixesToSkip = GetValueOrDefault(poco.EntityPrefixesToSkip, EntityPrefixesToSkip);
            EntityPrefixesWhitelist = GetValueOrDefault(poco.EntityPrefixesWhitelist, EntityPrefixesWhitelist);
            FilePrefixText = GetValueOrDefault(poco.FilePrefixText, FilePrefixText);
            GenerateActionAttributeNameConsts = poco.GenerateActionAttributeNameConsts ?? GenerateActionAttributeNameConsts;
            GenerateAllOptionSetLabelMetadata = poco.GenerateAllOptionSetLabelMetadata ?? GenerateAllOptionSetLabelMetadata;
            GenerateAttributeNameConsts = poco.GenerateAttributeNameConsts ?? GenerateAttributeNameConsts;
            GenerateAnonymousTypeConstructor = poco.GenerateAnonymousTypeConstructor ?? GenerateAnonymousTypeConstructor;
            GenerateConstructorsSansLogicalName = poco.GenerateConstructorsSansLogicalName ?? GenerateConstructorsSansLogicalName;
            GenerateEntityRelationships = poco.GenerateEntityRelationships ?? GenerateEntityRelationships;
            GenerateEnumProperties = poco.GenerateEnumProperties ?? GenerateEnumProperties;
            GenerateGlobalOptionSets = poco.GenerateGlobalOptionSets ?? GenerateGlobalOptionSets;
            GenerateINotifyPattern = poco.GenerateINotifyPattern ?? GenerateINotifyPattern;
            GenerateOptionSetMetadataAttribute = poco.GenerateOptionSetMetadataAttribute ?? GenerateOptionSetMetadataAttribute;
            GenerateTypesAsInternal = poco.GenerateTypesAsInternal ?? GenerateTypesAsInternal;
            GroupLocalOptionSetsByEntity = poco.GroupLocalOptionSetsByEntity ?? GroupLocalOptionSetsByEntity;
            GroupMessageRequestWithResponse = poco.GroupMessageRequestWithResponse ?? GroupMessageRequestWithResponse;
            InvalidCSharpNamePrefix = poco.InvalidCSharpNamePrefix ?? InvalidCSharpNamePrefix;
            MakeAllFieldsEditable = poco.MakeAllFieldsEditable ?? MakeAllFieldsEditable;
            MakeReadonlyFieldsEditable = poco.MakeReadonlyFieldsEditable ?? MakeReadonlyFieldsEditable;
            MakeResponseActionsEditable = poco.MakeResponseActionsEditable ?? MakeResponseActionsEditable;
            LocalOptionSetFormat = poco.LocalOptionSetFormat ?? LocalOptionSetFormat;
            ModelBuilderLogLevel = GetValueOrDefault(poco.ModelBuilderLogLevel, ModelBuilderLogLevel);
            OptionSetLanguageCodeOverride = poco.OptionSetLanguageCodeOverride ?? OptionSetLanguageCodeOverride;
            OptionSetNames = GetValueOrDefault(poco.OptionSetNames, OptionSetNames);
            OutputRelativeDirectory = GetValueOrDefault(poco.OutputRelativeDirectory, OutputRelativeDirectory);
            ProjectNameForEarlyBoundFiles = poco.ProjectNameForEarlyBoundFiles ?? ProjectNameForEarlyBoundFiles;
            PropertyEnumMappings = GetValueOrDefault(poco.PropertyEnumMappings, PropertyEnumMappings);
            ReadSerializedMetadata = poco.ReadSerializedMetadata ?? ReadSerializedMetadata;
            RemoveRuntimeVersionComment = poco.RemoveRuntimeVersionComment ?? RemoveRuntimeVersionComment;
            ReplaceOptionSetPropertiesWithEnum = poco.ReplaceOptionSetPropertiesWithEnum ?? ReplaceOptionSetPropertiesWithEnum;
            SerializeMetadata = poco.SerializeMetadata ?? SerializeMetadata;
            SuppressAutogeneratedFileHeaderComment = poco.SuppressAutogeneratedFileHeaderComment ?? SuppressAutogeneratedFileHeaderComment;
            TokenCapitalizationOverrides = GetValueOrDefault(poco.TokenCapitalizationOverrides, TokenCapitalizationOverrides);
            TransliterationRelativePath = GetValueOrDefault(poco.TransliterationRelativePath, TransliterationRelativePath);
            UseCrmSvcUtilStateEnumNamingConvention = poco.UseCrmSvcUtilStateEnumNamingConvention ?? UseCrmSvcUtilStateEnumNamingConvention;
            UseDisplayNameForBpfName = poco.UseDisplayNameForBpfName ?? UseDisplayNameForBpfName;
            UseEnumForStateCodes = poco.UseEnumForStateCodes ?? UseEnumForStateCodes;
            UseLogicalNames = poco.UseLogicalNames ?? UseLogicalNames;
            UseTfsToCheckoutFiles = poco.UseTfsToCheckoutFiles ?? UseTfsToCheckoutFiles;
            WaitForAttachedDebugger = poco.WaitForAttachedDebugger ?? WaitForAttachedDebugger;

            string GetValueOrDefault(string value, string defaultValue)
            {
                return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
            }
        }

        public void WriteDLaBModelBuilderProperties(Utf8JsonWriter writer, EarlyBoundGeneratorConfig settings)
        {
            var generateOptionSetProperties = !GenerateEnumProperties || (GenerateEnumProperties && !ReplaceOptionSetPropertiesWithEnum);
            // Write first since it will be cleaned up after the file is processed, and don't want to mess with the line before ending in a comma
            writer.AddProperty(nameof(XrmToolBoxPluginPath), XrmToolBoxPluginPath);

            writer.AddProperty(nameof(AddDebuggerNonUserCode), AddDebuggerNonUserCode);
            writer.AddProperty(nameof(AddNewFilesToProject), AddNewFilesToProject);
            writer.AddProperty(nameof(AddOptionSetMetadataAttribute), AddOptionSetMetadataAttribute);
            writer.AddProperty(nameof(AdjustCasingForEnumOptions), AdjustCasingForEnumOptions);
            writer.AddProperty(nameof(BuilderSettingsJsonRelativePath), BuilderSettingsJsonRelativePath);
            writer.AddProperty(nameof(CamelCaseClassNames), CamelCaseClassNames);
            writer.AddPropertyArray(nameof(CamelCaseCustomWords), CamelCaseCustomWords);
            writer.AddProperty(nameof(CamelCaseMemberNames), CamelCaseMemberNames);
            writer.AddProperty(nameof(CamelCaseNamesDictionaryRelativePath), CamelCaseNamesDictionaryRelativePath);
            writer.AddProperty(nameof(CleanupCrmSvcUtilLocalOptionSets), CleanupCrmSvcUtilLocalOptionSets);
            writer.AddProperty(AsMessage(nameof(CreateOneFilePerAction)), CreateOneFilePerAction);
            writer.AddProperty(nameof(CreateOneFilePerEntity), CreateOneFilePerEntity);
            writer.AddProperty(nameof(CreateOneFilePerOptionSet), CreateOneFilePerOptionSet);
            writer.AddProperty(nameof(DeleteFilesFromOutputFolders), DeleteFilesFromOutputFolders);
            writer.AddPropertyDictionaryStringHashString(nameof(EntityAttributeSpecifiedNames), EntityAttributeSpecifiedNames, false);
            writer.AddPropertyDictionaryStringString(nameof(EntityClassNameOverrides), EntityClassNameOverrides);
            AddOptionalProperty("EntitiesFileName", settings.EntityTypesFolder, !CreateOneFilePerEntity);
            writer.AddPropertyArray("EntityBlacklist", EntitiesToSkip);
            writer.AddPropertyArray("EntityRegExBlacklist", EntityPrefixesToSkip);
            writer.AddProperty(nameof(FilePrefixText), FilePrefixText, true);
            writer.AddProperty(AsMessage(nameof(GenerateActionAttributeNameConsts)), GenerateActionAttributeNameConsts);
            AddOptionalBoolProperty(nameof(GenerateAllOptionSetLabelMetadata), GenerateAllOptionSetLabelMetadata, AddOptionSetMetadataAttribute);
            writer.AddProperty(nameof(GenerateAttributeNameConsts), GenerateAttributeNameConsts);
            writer.AddProperty(nameof(GenerateAnonymousTypeConstructor), GenerateAnonymousTypeConstructor);
            writer.AddProperty(nameof(GenerateConstructorsSansLogicalName), GenerateConstructorsSansLogicalName);
            writer.AddProperty(nameof(GenerateEntityRelationships), GenerateEntityRelationships);
            writer.AddProperty("GenerateOptionSetProperties", generateOptionSetProperties);
            writer.AddProperty(nameof(GenerateOptionSetMetadataAttribute), GenerateOptionSetMetadataAttribute);
            writer.AddProperty(nameof(GenerateTypesAsInternal), GenerateTypesAsInternal);
            writer.AddProperty(nameof(GroupLocalOptionSetsByEntity), GroupLocalOptionSetsByEntity);
            writer.AddProperty(nameof(GroupMessageRequestWithResponse), GroupMessageRequestWithResponse);
            writer.AddProperty(nameof(settings.IncludeCommandLine), settings.IncludeCommandLine);
            writer.AddProperty(nameof(InvalidCSharpNamePrefix), InvalidCSharpNamePrefix);
            writer.AddProperty(nameof(LocalOptionSetFormat), LocalOptionSetFormat);
            writer.AddProperty(nameof(MakeAllFieldsEditable), MakeAllFieldsEditable);
            writer.AddProperty(nameof(MakeReadonlyFieldsEditable), MakeReadonlyFieldsEditable);
            writer.AddProperty(AsMessage(nameof(MakeResponseActionsEditable)), MakeResponseActionsEditable);
            writer.AddPropertyArray("MessageBlacklist", ActionsToSkip?.Replace("-", ""));
            AddOptionalProperty("MessagesFileName", settings.MessageTypesFolder, !CreateOneFilePerAction);
            writer.AddProperty(nameof(ModelBuilderLogLevel), ModelBuilderLogLevel);
            AddOptionalProperty("OptionSetsFileName", settings.OptionSetsTypesFolder, !CreateOneFilePerOptionSet);
            writer.AddProperty(nameof(OptionSetLanguageCodeOverride), OptionSetLanguageCodeOverride?.ToString());
            writer.AddPropertyDictionaryStringString(nameof(OptionSetNames), OptionSetNames);
            writer.AddProperty(nameof(ProjectNameForEarlyBoundFiles), ProjectNameForEarlyBoundFiles ?? string.Empty);
            writer.AddPropertyDictionaryStringString(nameof(PropertyEnumMappings), PropertyEnumMappings, false);
            writer.AddProperty(nameof(ReadSerializedMetadata), ReadSerializedMetadata);
            writer.AddProperty(nameof(RemoveRuntimeVersionComment), RemoveRuntimeVersionComment);
            AddOptionalBoolProperty("ReplaceEnumPropertiesWithOptionSet", !GenerateEnumProperties, generateOptionSetProperties);
            writer.AddProperty(nameof(OutputRelativeDirectory), OutputRelativeDirectory);
            writer.AddProperty(nameof(SerializeMetadata), SerializeMetadata);
            writer.AddProperty(nameof(SuppressAutogeneratedFileHeaderComment), SuppressAutogeneratedFileHeaderComment);
            writer.AddPropertyArray(nameof(TokenCapitalizationOverrides), TokenCapitalizationOverrides);
            writer.AddProperty(nameof(TransliterationRelativePath), TransliterationRelativePath);
            writer.AddProperty(nameof(UseLogicalNames), UseLogicalNames);
            writer.AddProperty(nameof(UseCrmSvcUtilStateEnumNamingConvention), UseCrmSvcUtilStateEnumNamingConvention);
            writer.AddProperty(nameof(UseDisplayNameForBpfName), UseDisplayNameForBpfName);
            AddOptionalBoolProperty(nameof(UseEnumForStateCodes), UseEnumForStateCodes, generateOptionSetProperties && !ReplaceOptionSetPropertiesWithEnum);
            writer.AddProperty(nameof(UseTfsToCheckoutFiles), UseTfsToCheckoutFiles);
            writer.AddProperty(nameof(WaitForAttachedDebugger), WaitForAttachedDebugger);

            void AddOptionalProperty(string fileNameKey, string fileNameValue, bool createProperty)
            {
                if (createProperty)
                {
                    writer.AddProperty(fileNameKey, fileNameValue);
                }
            }

            void AddOptionalBoolProperty(string fileNameKey, bool value, bool createProperty)
            {
                if (createProperty)
                {
                    writer.AddProperty(fileNameKey, value);
                }
            }

            string AsMessage(string actionName)
            {
                return actionName.Replace("Action", "Message");
            }
        }

        public void PopulateBuilderProperties(Dictionary<string, JsonProperty> properties, bool generateMessages)
        {
            properties.SetJsonProperty(BuilderSettingsJsonNames.EmitFieldsClasses, GenerateAttributeNameConsts);
            properties.SetJsonArrayProperty(BuilderSettingsJsonNames.EntityNamesFilter, JoinWhiteLists(EntitiesWhitelist, EntityPrefixesWhitelist));
            properties.SetJsonProperty(BuilderSettingsJsonNames.GenerateGlobalOptionSets, GenerateGlobalOptionSets);
            properties.SetJsonArrayProperty(BuilderSettingsJsonNames.MessageNamesFilter, generateMessages ? JoinWhiteLists(ActionsWhitelist, ActionPrefixesWhitelist) : null);
            properties.SetJsonProperty(BuilderSettingsJsonNames.SuppressINotifyPattern, !GenerateINotifyPattern);
        }

        private string JoinWhiteLists(string entities, string prefixes)
        {
            if (string.IsNullOrWhiteSpace(prefixes))
            {
                return entities;
            }

            prefixes = string.Join("|", prefixes.Split('|').Select(v => v.Contains('*') ? v : v + "*"));

            return string.IsNullOrWhiteSpace(entities)
                ? prefixes
                : string.Join("|", entities, prefixes);
        }
    }
}

namespace DLaB.EarlyBoundGeneratorV2.Settings.POCO
{
    /// <summary>
    /// Serializable Class with Nullable types to be able to tell if they are populated or not
    /// </summary>
    public class ExtensionConfig
    {
        public string ActionPrefixesWhitelist { get; set; }
        public string ActionsToSkip { get; set; }
        public string ActionsWhitelist { get; set; }
        public bool? AddDebuggerNonUserCode { get; set; }
        public bool? AddNewFilesToProject { get; set; }
        public bool? AddOptionSetMetadataAttribute { get; set; }
        public bool? AdjustCasingForEnumOptions { get; set; }
        public string BuilderSettingsJsonRelativePath { get; set; }
        public bool? CamelCaseClassNames { get; set; }
        public string CamelCaseCustomWords { get; set; }
        public bool? CamelCaseMemberNames { get; set; }
        public string CamelCaseNamesDictionaryRelativePath { get; set; }
        public bool? CleanupCrmSvcUtilLocalOptionSets { get; set; }
        public bool? CreateOneFilePerAction { get; set; }
        public bool? CreateOneFilePerEntity { get; set; }
        public bool? CreateOneFilePerOptionSet { get; set; }
        public string CustomizeCodeDomService { get; set; }
        public bool? DeleteFilesFromOutputFolders { get; set; }
        public string EntitiesToSkip { get; set; }
        public string EntitiesWhitelist { get; set; }
        public string EntityAttributeSpecifiedNames { get; set; }
        public string EntityClassNameOverrides { get; set; }
        public string EntityPrefixesToSkip { get; set; }
        public string EntityPrefixesWhitelist { get; set; }
        public string FilePrefixText { get; set; }
        public bool? GenerateActionAttributeNameConsts { get; set; }
        public bool? GenerateAllOptionSetLabelMetadata { get; set; }
        public bool? GenerateAttributeNameConsts { get; set; }
        public bool? GenerateAnonymousTypeConstructor { get; set; }
        public bool? GenerateConstructorsSansLogicalName { get; set; }
        public bool? GenerateEntityRelationships { get; set; }
        /// <summary>
        /// Now moved to the Dataverse Model Builder as EmitEntityETC
        /// </summary>
        [Obsolete("Now moved to the Dataverse Model Builder as EmitEntityETC")]
        public bool? GenerateEntityTypeCode { get; set; }
        public bool? GenerateEnumProperties { get; set; }
        public bool? GenerateGlobalOptionSets { get; set; }
        public bool? GenerateINotifyPattern { get; set; }
        /// <summary>
        /// Legacy for CrmSvcUtil
        /// </summary>
        public bool? GenerateOnlyReferencedOptionSets { get; set; }
        public bool? GenerateOptionSetMetadataAttribute { get; set; }
        public bool? GenerateTypesAsInternal { get; set; }
        public bool? GroupLocalOptionSetsByEntity { get; set; }
        public bool? GroupMessageRequestWithResponse { get; set; }
        public bool? ReplaceOptionSetPropertiesWithEnum { get; set; }
        public string InvalidCSharpNamePrefix { get; set; }
        public string LocalOptionSetFormat { get; set; }
        public bool? MakeAllFieldsEditable { get; set; }
        public bool? MakeReadonlyFieldsEditable { get; set; }
        public bool? MakeResponseActionsEditable { get; set; }
        public string ModelBuilderLogLevel { get; set; }
        public string OptionSetNames { get; set; }
        public int? OptionSetLanguageCodeOverride { get; set; }
        public string OutputRelativeDirectory { get; set; }
        public string PropertyEnumMappings { get; set; }
        public string ProjectNameForEarlyBoundFiles { get; set; }
        public bool? ReadSerializedMetadata { get; set; }
        public bool? RemoveRuntimeVersionComment { get; set; }
        public bool? SerializeMetadata { get; set; }
        public bool? SuppressAutogeneratedFileHeaderComment { get; set; }
        public string TokenCapitalizationOverrides { get; set; }
        public string TransliterationRelativePath { get; set; }
        public bool? UseCrmSvcUtilStateEnumNamingConvention { get; set; }
        public bool? UseDisplayNameForBpfName { get; set; }
        public bool? UseEnumForStateCodes { get; set; }
        public bool? UseLogicalNames { get; set; }
        public bool? UseTfsToCheckoutFiles { get; set; }
        public bool? WaitForAttachedDebugger { get; set; }
    }
}
