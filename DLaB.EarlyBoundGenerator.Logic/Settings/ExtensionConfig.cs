using System;
using System.Xml.Serialization;

namespace DLaB.EarlyBoundGenerator.Settings
{
    /// <summary>
    /// Serializable class containing all settings that will get written to the CrmSrvUtil.exe.config
    /// </summary>
    [Serializable]
    public class ExtensionConfig
    {
        /// <summary>
        /// Pipe Delimited String containing the prefixes of Actions to not generate.
        /// </summary>
        public string ActionPrefixesToSkip { get; set; }
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
        /// Using a dictionary, attempts to correctly camelcase class/enum names.
        /// </summary>
        public bool CamelCaseClassNames { get; set; }
        /// <summary>
        /// Using a dictionary, attempts to correctly camelcase column/parameter names
        /// </summary>
        public bool CamelCaseMemberNames { get; set; }
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
        /// <summary>
        /// Pipe Delimited String containing the logical names of Entities to not generate
        /// </summary>
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
        /// Pipe delimited string containing prefixes of entities to not generate.
        /// </summary>
        public string EntityPrefixesToSkip { get; set; }
        /// <summary>
        /// Pipe delimited string containing prefixes of entities to be generated.
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
        /// By default the CrmSvcUtil generates the Entity Type Code, this is considered dangerous and not recommended since it is a system generated value, and not one defined in the solution metadata, changing from environment to environment.
        /// </summary>
        public bool GenerateEntityTypeCode { get; set; }
        /// <summary>
        /// Specifies the generation of Enum properties for option sets
        /// </summary>                                          
        public bool GenerateEnumProperties { get; set; }
        /// <summary>
        /// Specifies that only option sets that are referenced by Entities that are generated.
        /// </summary>                                          
        public bool GenerateOnlyReferencedOptionSets { get; set; }
        /// <summary>
        /// Generates an OptionSetMetadataAttribute class used to allow for storing of the metadata of OptionSetValues i.e. display order, name, description, etc.
        /// Only used if Add Option Set Metadata Attribute is true.
        /// </summary>
        public bool GenerateOptionSetMetadataAttribute { get; set; }
        /// <summary>
        /// Combines all local option sets into a single file per entity.  Only used if Create One File Per Option Set is true.
        /// </summary>
        public bool GroupLocalOptionSetsByEntity { get; set; }
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
        /// Overrides the default (English:1033) language code used for generating Option Set Value names (the value, not the option set)
        /// </summary>
        public int? OptionSetLanguageCodeOverride { get; set; }
        /// <summary>
        /// Pipe delimited string containing prefixes of entities to not generate.
        /// </summary>
        public string OptionSetPrefixesToSkip { get; set; }
        /// <summary>
        /// Pipe Delimited String containing the logical names of Option Set Names to not generate
        /// </summary>
        public string OptionSetsToSkip { get; set; }
        /// <summary>
        /// If Option Sets have identical names to Entities, it will cause naming conflicts.  By default this is overcome by post-fixing "_Enum" to the Option Set name.  This setting allows a custom mapping for option set names to be specified.
        /// </summary>
        public string OptionSetNames { get; set; }
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
        /// Used in conjunction with Camel Case Class Names and Camel Case Member Names to override any defaults.
        /// </summary>
        public string TokenCapitalizationOverrides { get; set; }
        /// <summary>
        /// Used to manually specify an OptionSetValue Property of an entity that doesn't have an enum mapping 
        /// Format: EntityName.PropertyName|
        /// </summary>
        public string UnmappedProperties { get; set; }
        /// <summary>
        /// Creates Local OptionSets Using the Deprecated Naming Convention. prefix_oobentityname_prefix_attribute
        /// </summary>
        /// <value>
        /// <c>true</c> if [use Deprecated Option Set Naming]; otherwise, <c>false</c>.
        /// </value>
        public bool UseDeprecatedOptionSetNaming { get; set; }
        /// <summary>
        /// Generate entities and properties with logical name instead of schema name.
        /// </summary>
        public bool UseLogicalNames { get; set; }
        /// <summary>
        /// Uses TFS to checkout files
        /// </summary>
        public bool UseTfsToCheckoutFiles { get; set; }
        /// <summary>
        /// Uses "internal" as the access modifier instead of "public".
        /// </summary>
        public bool UseInternalAsAccessModifier { get; set; }
        /// <summary>
        /// For Debugging Only!
        /// Waits until a debugger is attached to the CrmSvcUtil.exe before processing the command.
        /// </summary>
        public bool WaitForAttachedDebugger { get; set; }

        #region NonSerialized Properties

        /// <summary>
        /// If populated, used as the Command Line text to insert into generated header for Actions
        /// </summary>
        [XmlIgnore]
        public string ActionCommandLineText { get; set; }

        /// <summary>
        /// If populated, used as the Command Line text to insert into generated header for Entities
        /// </summary>
        [XmlIgnore]
        public string EntityCommandLineText { get; set; }

        /// <summary>
        /// If populated, used as the Command Line text to insert into generated header for OptionSets
        /// </summary>
        [XmlIgnore]
        public string OptionSetCommandLineText { get; set; }

        #endregion // NonSerialized Properties

        public static ExtensionConfig GetDefault()
        {
            return new ExtensionConfig
            {
                AddDebuggerNonUserCode = true,
                AddNewFilesToProject = true,
                CreateOneFilePerAction = false,
                CreateOneFilePerEntity = false,
                CreateOneFilePerOptionSet = false,
                DeleteFilesFromOutputFolders = false,
                GenerateActionAttributeNameConsts = false,
                GenerateAttributeNameConsts = false,
                GenerateAnonymousTypeConstructor = true,
                GenerateConstructorsSansLogicalName = false,
                GenerateEntityRelationships = true,
                GenerateEntityTypeCode = false,
                GenerateEnumProperties = true,
                GenerateOnlyReferencedOptionSets = true,
                ActionPrefixesToSkip = null,
                ActionPrefixesWhitelist = null,
                ActionsWhitelist = null,
                ActionsToSkip = null,
                EntitiesToSkip = null,
                EntitiesWhitelist = null,
                EntityAttributeSpecifiedNames = null,
                EntityPrefixesToSkip = null,
                EntityPrefixesWhitelist = null,
                InvalidCSharpNamePrefix = "_",
                MakeAllFieldsEditable = false,
                MakeReadonlyFieldsEditable = false,
                MakeResponseActionsEditable = false,
                LocalOptionSetFormat = "{0}_{1}",
                OptionSetNames = null,
                OptionSetPrefixesToSkip = null,
                OptionSetsToSkip = null,
                OptionSetLanguageCodeOverride = null,
                ProjectNameForEarlyBoundFiles = string.Empty,
                PropertyEnumMappings = string.Empty,
                RemoveRuntimeVersionComment = true,
                ReplaceOptionSetPropertiesWithEnum = true,
                UnmappedProperties =
                    "DuplicateRule:BaseEntityTypeCode,MatchingEntityTypeCode|" +
                    "InvoiceDetail:InvoiceStateCode|" +
                    "LeadAddress:AddressTypeCode,ShippingMethodCode|" +
                    "Organization:CurrencyFormatCode,DateFormatCode,TimeFormatCode,WeekStartDayCode|" +
                    "Quote:StatusCode|" +
                    "QuoteDetail:QuoteStateCode|" +
                    "SalesOrderDetail:SalesOrderStateCode|",
                AddOptionSetMetadataAttribute = true,
                CamelCaseClassNames = false,
                CamelCaseMemberNames = false,
                FilePrefixText = null,
                GenerateOptionSetMetadataAttribute = true,
                GroupLocalOptionSetsByEntity = false,
                ReadSerializedMetadata = false,
                SerializeMetadata = false,
                TokenCapitalizationOverrides = "ActiveState|AccessTeam|BusinessAs|CardUci|DefaultOnCase|EmailAnd|FeatureSet|IsMsTeams|IsPaiEnabled|IsSopIntegration|MsDyUsd|O365Admin|OnHold|OwnerOnAssign|PauseStates|PartiesOnEmail|ParticipatesIn|SentOn|SlaKpi|SlaId|SyncOptIn|Timeout|UserPuid|VoiceMail|Weblink",
                UseDeprecatedOptionSetNaming = false,
                UseInternalAsAccessModifier = false,
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
            ActionPrefixesToSkip = GetValueOrDefault(poco.ActionPrefixesToSkip, ActionPrefixesToSkip);
            ActionPrefixesWhitelist = GetValueOrDefault(poco.ActionPrefixesWhitelist, ActionPrefixesWhitelist);
            ActionsWhitelist = GetValueOrDefault(poco.ActionsWhitelist, ActionsWhitelist);
            ActionsToSkip = GetValueOrDefault(poco.ActionsToSkip, ActionsToSkip);
            AddDebuggerNonUserCode = poco.AddDebuggerNonUserCode ?? AddDebuggerNonUserCode;
            AddNewFilesToProject = poco.AddNewFilesToProject ?? AddNewFilesToProject;
            AddOptionSetMetadataAttribute = poco.AddOptionSetMetadataAttribute ?? AddOptionSetMetadataAttribute;
            CamelCaseClassNames = poco.CamelCaseClassNames ?? CamelCaseClassNames;
            CamelCaseMemberNames = poco.CamelCaseMemberNames ?? CamelCaseMemberNames;
            CreateOneFilePerAction = poco.CreateOneFilePerAction ?? CreateOneFilePerAction;
            CreateOneFilePerEntity = poco.CreateOneFilePerEntity ?? CreateOneFilePerEntity;
            CreateOneFilePerOptionSet = poco.CreateOneFilePerOptionSet ?? CreateOneFilePerOptionSet;
            DeleteFilesFromOutputFolders = poco.DeleteFilesFromOutputFolders ?? DeleteFilesFromOutputFolders;
            EntitiesToSkip = GetValueOrDefault(poco.EntitiesToSkip, EntitiesToSkip);
            EntitiesWhitelist = GetValueOrDefault(poco.EntitiesWhitelist, EntitiesWhitelist);
            EntityAttributeSpecifiedNames = GetValueOrDefault(poco.EntityAttributeSpecifiedNames, EntityAttributeSpecifiedNames);
            EntityPrefixesToSkip = GetValueOrDefault(poco.EntityPrefixesToSkip, EntityPrefixesToSkip);
            EntityPrefixesWhitelist = GetValueOrDefault(poco.EntityPrefixesWhitelist, EntityPrefixesWhitelist);
            FilePrefixText = GetValueOrDefault(poco.FilePrefixText, FilePrefixText);
            GenerateActionAttributeNameConsts = poco.GenerateActionAttributeNameConsts ?? GenerateActionAttributeNameConsts;
            GenerateAttributeNameConsts = poco.GenerateAttributeNameConsts ?? GenerateAttributeNameConsts;
            GenerateAnonymousTypeConstructor = poco.GenerateAnonymousTypeConstructor ?? GenerateAnonymousTypeConstructor;
            GenerateConstructorsSansLogicalName = poco.GenerateConstructorsSansLogicalName ?? GenerateConstructorsSansLogicalName;
            GenerateEntityRelationships = poco.GenerateEntityRelationships ?? GenerateEntityRelationships;
            GenerateEntityTypeCode = poco.GenerateEntityTypeCode ?? GenerateEntityTypeCode;
            GenerateEnumProperties = poco.GenerateEnumProperties ?? GenerateEnumProperties;
            GenerateOnlyReferencedOptionSets = poco.GenerateOnlyReferencedOptionSets ?? GenerateOnlyReferencedOptionSets;
            GenerateOptionSetMetadataAttribute = poco.GenerateOptionSetMetadataAttribute ?? GenerateOptionSetMetadataAttribute;
            GroupLocalOptionSetsByEntity = poco.GroupLocalOptionSetsByEntity ?? GroupLocalOptionSetsByEntity;
            InvalidCSharpNamePrefix = poco.InvalidCSharpNamePrefix ?? InvalidCSharpNamePrefix;
            MakeAllFieldsEditable = poco.MakeAllFieldsEditable ?? MakeAllFieldsEditable;
            MakeReadonlyFieldsEditable = poco.MakeReadonlyFieldsEditable ?? MakeReadonlyFieldsEditable;
            MakeResponseActionsEditable = poco.MakeResponseActionsEditable ?? MakeResponseActionsEditable;
            LocalOptionSetFormat = poco.LocalOptionSetFormat ?? LocalOptionSetFormat;
            OptionSetLanguageCodeOverride = poco.OptionSetLanguageCodeOverride ?? OptionSetLanguageCodeOverride;
            OptionSetNames = GetValueOrDefault(poco.OptionSetNames, OptionSetNames);
            OptionSetPrefixesToSkip = GetValueOrDefault(poco.OptionSetPrefixesToSkip, OptionSetPrefixesToSkip);
            OptionSetsToSkip = GetValueOrDefault(poco.OptionSetsToSkip, OptionSetsToSkip);
            ProjectNameForEarlyBoundFiles = poco.ProjectNameForEarlyBoundFiles ?? ProjectNameForEarlyBoundFiles;
            PropertyEnumMappings = GetValueOrDefault(poco.PropertyEnumMappings, PropertyEnumMappings);
            ReadSerializedMetadata = poco.ReadSerializedMetadata ?? ReadSerializedMetadata;
            RemoveRuntimeVersionComment = poco.RemoveRuntimeVersionComment ?? RemoveRuntimeVersionComment;
            ReplaceOptionSetPropertiesWithEnum = poco.ReplaceOptionSetPropertiesWithEnum ?? ReplaceOptionSetPropertiesWithEnum;
            SerializeMetadata = poco.SerializeMetadata ?? SerializeMetadata;
             TokenCapitalizationOverrides = GetValueOrDefault(poco.TokenCapitalizationOverrides, TokenCapitalizationOverrides);
            UnmappedProperties = GetValueOrDefault(poco.UnmappedProperties, UnmappedProperties);
            UseDeprecatedOptionSetNaming = poco.UseDeprecatedOptionSetNaming ?? UseDeprecatedOptionSetNaming;
            UseInternalAsAccessModifier = poco.UseInternalAsAccessModifier ?? UseInternalAsAccessModifier;
            UseLogicalNames = poco.UseLogicalNames ?? UseLogicalNames;
            UseTfsToCheckoutFiles = poco.UseTfsToCheckoutFiles ?? UseTfsToCheckoutFiles;
            WaitForAttachedDebugger = poco.WaitForAttachedDebugger ?? WaitForAttachedDebugger;

            string GetValueOrDefault(string value, string defaultValue)
            {
                return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
            }
        }
    }
}

#pragma warning disable 1591
namespace DLaB.EarlyBoundGenerator.Settings.POCO
{
    /// <summary>
    /// Serializable Class with Nullable types to be able to tell if they are populated or not
    /// </summary>
    public class ExtensionConfig
    {
        public bool? CreateOneFilePerEntity { get; set; }
        public bool? CreateOneFilePerOptionSet { get; set; }
        public bool? DeleteFilesFromOutputFolders { get; set; }
        public bool? GenerateActionAttributeNameConsts { get; set; }
        public bool? GenerateAttributeNameConsts { get; set; }
        public bool? GenerateAnonymousTypeConstructor { get; set; }
        public bool? GenerateConstructorsSansLogicalName { get; set; }
        public bool? GenerateEntityRelationships { get; set; }
        public bool? GenerateEntityTypeCode { get; set; }
        public bool? GenerateEnumProperties { get; set; }
        public bool? AddDebuggerNonUserCode { get; set; }
        public bool? CreateOneFilePerAction { get; set; }
        public bool? ReplaceOptionSetPropertiesWithEnum { get; set; }
        public string ActionPrefixesToSkip { get; set; }
        public string ActionPrefixesWhitelist { get; set; }
        public string ActionsToSkip { get; set; }
        public string ActionsWhitelist { get; set; }
        public string EntitiesToSkip { get; set; }
        public string EntitiesWhitelist { get; set; }
        public string EntityAttributeSpecifiedNames { get; set; }
        public string EntityPrefixesToSkip { get; set; }
        public string EntityPrefixesWhitelist { get; set; }
        public bool? GenerateOnlyReferencedOptionSets { get; set; }
        public string InvalidCSharpNamePrefix { get; set; }
        public bool? MakeAllFieldsEditable { get; set; }
        public bool? MakeReadonlyFieldsEditable { get; set; }
        public bool? MakeResponseActionsEditable { get; set; }
        public string LocalOptionSetFormat { get; set; }
        public string OptionSetPrefixesToSkip { get; set; }
        public string OptionSetsToSkip { get; set; }
        public int? OptionSetLanguageCodeOverride { get; set; }
        public string PropertyEnumMappings { get; set; }
        public string UnmappedProperties { get; set; }
        public bool? AddNewFilesToProject { get; set; }
        public bool? AddOptionSetMetadataAttribute { get; set; }
        public bool? CamelCaseClassNames { get; set; }
        public bool? CamelCaseMemberNames { get; set; }
        public string FilePrefixText { get; set; }
        public bool? GenerateOptionSetMetadataAttribute { get; set; }
        public bool? GroupLocalOptionSetsByEntity { get; set; }
        public string OptionSetNames { get; set; }
        public string ProjectNameForEarlyBoundFiles { get; set; }
        public bool? ReadSerializedMetadata { get; set; }
        public bool? RemoveRuntimeVersionComment { get; set; }
        public bool? SerializeMetadata { get; set; }
        public string TokenCapitalizationOverrides { get; set; }
        public bool? UseDeprecatedOptionSetNaming { get; set; }
        public bool? UseInternalAsAccessModifier { get; set; }
        public bool? UseLogicalNames { get; set; }
        public bool? UseTfsToCheckoutFiles { get; set; }
        public bool? WaitForAttachedDebugger { get; set; }
    }
}
