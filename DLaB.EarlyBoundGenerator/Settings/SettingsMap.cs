using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using DLaB.XrmToolBoxCommon.Editors;
using CommonConfig = Source.DLaB.Common.Config;
using Source.DLaB.Common;
using XrmToolBox.Extensibility;
using DLaB.XrmToolBoxCommon.Forms;

// ReSharper disable UnusedMember.Global

namespace DLaB.EarlyBoundGenerator.Settings
{
    public partial class SettingsMap: IGetPluginControl<EarlyBoundGeneratorPlugin>
    {
        private const string StringEditorName = @"System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

        #region Properties

        #region Actions

        [Category("Actions")]
        [DisplayName("Action Relative Output Path")]
        [Description("This is realtive to the Path of the Settings File.  If \"Create One File Per Action\" is enabled, this needs to be a file path that ends in \".cs\", else, this needs to be a path to a directory.")]
        [Editor(typeof(PathEditor), typeof(UITypeEditor))]
        [DynamicRelativePathEditor(nameof(SettingsPath), nameof(CreateOneFilePerAction), "C# files|*.cs", "cs", false)]
        public string ActionOutPath
        {
            get => Config.ActionOutPath;
            set => Config.ActionOutPath = GetRelativePathToFileOrDirectory(value, CreateOneFilePerAction, "Actions.cs");
        }

        [Category("Actions")]
        [DisplayName("Action Prefixes Whitelist")]
        [Description("Allows for the ability to specify Action prefixes that will be included in generation.")]
        [Editor(StringEditorName, typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionCountConverter))]
        public List<string> ActionPrefixesWhitelist { get; set; }

        [Category("Actions")]
        [DisplayName("Actions Whitelist")]
        [Description("Allows for the ability to specify Actions that will be included in generation.")]
        [Editor(typeof(ActionsHashEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionCountConverter))]
        public HashSet<string> ActionsWhitelist { get; set; }

        [Category("Actions")]
        [DisplayName("Actions Blacklist")]
        [Description("Allows for the ability to specify Actions to not generate.")]
        [Editor(typeof(ActionsHashEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionCountConverter))]
        public HashSet<string> ActionsToSkip { get; set; }

        [Category("Actions")]
        [DisplayName("Create One File Per Action")]
        [Description("Specifies that each Action class should be outputted to it's own file rather than a single file with all actions.")]
        public bool CreateOneFilePerAction
        {
            get => Config.ExtensionConfig.CreateOneFilePerAction;
            set => Config.ExtensionConfig.CreateOneFilePerAction = value;
        }

        [Category("Actions")]
        [DisplayName("Generate Action Attribute Name Constants")]
        [Description("Adds a Static Class to each Action class that contains the Logical Names of all properties for the Action.")]
        public bool GenerateActionAttributeNameConsts
        {
            get => Config.ExtensionConfig.GenerateActionAttributeNameConsts;
            set => Config.ExtensionConfig.GenerateActionAttributeNameConsts = value;
        }

        [Category("Actions")]
        [DisplayName("Make Response Actions Editable")]
        [Description("Specifies that the properties of Response Actions should be editable.")]
        public bool MakeResponseActionsEditable
        {
            get => Config.ExtensionConfig.MakeResponseActionsEditable;
            set => Config.ExtensionConfig.MakeResponseActionsEditable = value;
        }

        [Category("Actions")]
        [DisplayName("Workflowless Actions")]
        [Description("Some actions are being created by MS that are not workflows, and don't show up in the list of actions for adding to whitelist/blacklist, but are getting genereated and causing errors.  This setting is used to manually add action names to the selected lists.")]
        [Editor(StringEditorName, typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionCountConverter))]
        public List<string> WorkflowlessActions { get; set; }

        #endregion Actions

        #region Debug

        [Category("Debug")]
        [DisplayName("Read Serialized Metadata")]
        [Description("For Debugging Only!  Used to not communicate to the server for the metadata, but to use the local metadata file instead.  Should only be used for testing generation outputs.  Set Serialize Metadata to true first to connect to the server and retrieve the metadata, then set it back to false to not write it again since it's already local.")]
        public bool ReadSerializedMetadata
        {
            get => Config.ExtensionConfig.ReadSerializedMetadata;
            set => Config.ExtensionConfig.ReadSerializedMetadata = value;
        }

        [Category("Debug")]
        [DisplayName("Serialize Metadata")]
        [Description("For Debugging Only!  Serializes the Metadata to a local file on disk.  (Generates a 200-400+ mb xml file).")]
        public bool SerializeMetadata
        {
            get => Config.ExtensionConfig.SerializeMetadata;
            set => Config.ExtensionConfig.SerializeMetadata = value;
        }

        [Category("Debug")]
        [DisplayName("Wait For Attached Debugger")]
        [Description("For Debugging Only!  Waits until a debugger is attached to the CrmSvcUtil.exe before processing the command.")]
        public bool WaitForAttachedDebugger
        {
            get => Config.ExtensionConfig.WaitForAttachedDebugger;
            set => Config.ExtensionConfig.WaitForAttachedDebugger = value;
        }

        #endregion Debug

        #region Entities

        [Category("Entities")]
        [DisplayName("Add DebuggerNonUserCode")]
        [Description("Specifies that the DebuggerNonUserCodeAttribute should be applied to all generated properties and methods.\n\rThis directs the debugger to skip stepping into generated entity files.")]
        public bool AddDebuggerNonUserCode
        {
            get => Config.ExtensionConfig.AddDebuggerNonUserCode;
            set => Config.ExtensionConfig.AddDebuggerNonUserCode = value;
        }

        [Category("Entities")]
        [DisplayName("Create One File Per Entity")]
        [Description("Specifies that each Entity class should be outputted to it's own file, rather than a single file with all entities.")]
        public bool CreateOneFilePerEntity
        {
            get => Config.ExtensionConfig.CreateOneFilePerEntity;
            set => Config.ExtensionConfig.CreateOneFilePerEntity = value;
        }

        [Category("Entities")]
        [DisplayName("Entity Relative Output Path")]
        [Description("This is realtive to the Path of the Settings File.  If \"Create One File Per Entity\" is enabled, this needs to be a file path that ends in \".cs\", else, this needs to be a path to a directory.")]
        [Editor(typeof(PathEditor), typeof(UITypeEditor))]
        [DynamicRelativePathEditor(nameof(SettingsPath), nameof(CreateOneFilePerEntity), "C# files|*.cs", "cs", false)]
        public string EntityOutPath
        {
            get => Config.EntityOutPath;
            set => Config.EntityOutPath = GetRelativePathToFileOrDirectory(value, CreateOneFilePerEntity, "Entities.cs");
        }

        [Category("Entities")]
        [DisplayName("Entities Blacklist")]
        [Description("Contains Entities to not generate.")]
        [Editor(typeof(EntitiesHashEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionCountConverter))]
        public HashSet<string> EntitiesToSkip { get; set; }

        [Category("Entities")]
        [DisplayName("Entities Whitelist")]
        [Description("Contains only the Entities to generate.  If empty, all Entities will be included.")]
        [Editor(typeof(EntitiesHashEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionCountConverter))]
        [CollectionCount("All Entities")]
        public HashSet<string> EntitiesWhitelist { get; set; }

        [Category("Entities")]
        [DisplayName("Attribute Capitalization Override")]
        [Description("Allows for the ability to specify the capitalization of an attribute on an entity.")]
        [Editor(typeof(SpecifyAttributesCaseEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionCountConverter))]
        public Dictionary<string, HashSet<string>> EntityAttributeSpecifiedNames { get; set; }

        [Category("Entities")]
        [DisplayName("Entities Prefix Blacklist")]
        [Description("Contains list of prefixes to not generate.  If the Entity starts with the given prefix, it will not be generated.")]
        [Editor(StringEditorName, typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionCountConverter))]
        public List<string> EntityPrefixesToSkip { get; set; }

        [Category("Entities")]
        [DisplayName("Entities Prefix Whitelist")]
        [Description("Contains list of prefixes to generate.  If the Entity starts with the given prefix, it will be generated.")]
        [Editor(StringEditorName, typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionCountConverter))]
        public List<string> EntityPrefixesWhitelist { get; set; }

        [Category("Entities")]
        [DisplayName("Generate Entity Attribute Name Constants")]
        [Description("Adds a Static Class to each Entity class that contains the Logical Names of all attributes for the Entity.")]
        public bool GenerateAttributeNameConsts
        {
            get => Config.ExtensionConfig.GenerateAttributeNameConsts;
            set => Config.ExtensionConfig.GenerateAttributeNameConsts = value;
        }

        [Category("Entities")]
        [DisplayName("Generate Anonymous Type Constructor")]
        [Description("Adds an Object Constructor to each early bound entity class to simplify LINQ projections (http://stackoverflow.com/questions/27623542).")]
        public bool GenerateAnonymousTypeConstructor
        {
            get => Config.ExtensionConfig.GenerateAnonymousTypeConstructor;
            set => Config.ExtensionConfig.GenerateAnonymousTypeConstructor = value;
        }

        [Category("Entities")]
        [DisplayName("Generate Constructor to match Microsoft.Xrm.Sdk.Entity Constructors")]
        [Description("Adds Constructors to each early bound entity class to use constructors available to Microsoft.Xrm.Sdk.Entity")]
        public bool GenerateConstructorsSansLogicalName
        {
            get => Config.ExtensionConfig.GenerateConstructorsSansLogicalName;
            set => Config.ExtensionConfig.GenerateConstructorsSansLogicalName = value;
        }

        [Category("Entities")]
        [DisplayName("Generate Entity Relationships")]
        [Description("Specifies if 1:N, N:1, and N:N relationships properties are generated for entities.")]
        public bool GenerateEntityRelationships
        {
            get => Config.ExtensionConfig.GenerateEntityRelationships;
            set => Config.ExtensionConfig.GenerateEntityRelationships = value;
        }

        [Category("Entities")]
        [DisplayName("Generate Entity Type Code")]
        [Description("By default the CrmSvcUtil generates the Entity Type Code, this is considered dangerous to use and is not recommended since it is a system generated value, and not one defined in the solution metadata, changing from environment to environment.")]
        public bool GenerateEntityTypeCode
        {
            get => Config.ExtensionConfig.GenerateEntityTypeCode;
            set => Config.ExtensionConfig.GenerateEntityTypeCode = value;
        }

        [Category("Entities")]
        [DisplayName("Generate Enum Properties")]
        [Description("Adds an additional property to each early bound entity class for each optionset property it normally contains, typed to the appropriate Enum, rather than OptionSet, with Enum postfixed to the existing optionset name.")]
        public bool GenerateEnumProperties
        {
            get => Config.ExtensionConfig.GenerateEnumProperties;
            set => Config.ExtensionConfig.GenerateEnumProperties = value;
        }

        [Category("Entities")]
        [DisplayName("Generate Option Set Metadata Attribute")]
        [Description("Generates an OptionSetMetadataAttribute class used to allow for storing of the metadata of OptionSetValues i.e. display order, name, description, etc.  Only used if Add Option Set Metadata Attribute is true.")]
        public bool GenerateOptionSetMetadataAttribute
        {
            get => Config.ExtensionConfig.GenerateOptionSetMetadataAttribute;
            set => Config.ExtensionConfig.GenerateOptionSetMetadataAttribute = value;
        }

        [Category("Entities")]
        [DisplayName("Make All Fields Editable")]
        [Description("Defines that Entities should be created with all attributes as editable.  This may be confusing for some developers because attempts to update FullName on the contact will silently fail.")]
        public bool MakeAllFieldsEditable
        {
            get => Config.ExtensionConfig.MakeAllFieldsEditable;
            set => Config.ExtensionConfig.MakeAllFieldsEditable = value;
        }

        [Category("Entities")]
        [DisplayName("Make Readonly Fields Editable")]
        [Description("Defines that Entities should be created with editable createdby, createdon, modifiedby, modifiedon, owningbusinessunit, owningteam, and owninguser properties.\n\rHelpful for writing linq statements where those attributes are wanting to be returned in the select.")]
        public bool MakeReadonlyFieldsEditable
        {
            get => Config.ExtensionConfig.MakeReadonlyFieldsEditable;
            set => Config.ExtensionConfig.MakeReadonlyFieldsEditable = value;
        }

        [Category("Entities")]
        [DisplayName("Property Enum Mapping")]
        [Description("Manually specifies an enum mapping for an OptionSetValue Property on an entity.\n\rThis is useful if you have multiple local options that really are the same value.  This then allows easier comparision since the enums don't have to be converted.")]
        [Editor(typeof(AttributesToEnumMapperEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionCountConverter))]
        public List<string> PropertyEnumMappings { get; set; }

        [Category("Entities")]
        [DisplayName("Replace Option Set Properties with Enum")]
        [Description("Only used when Generate Enum Properties is true.  When true, replaces OptionSet properties with Enum properties.  When false, each OptionSet properties is duplicated with Enum postfixed to the existing optionset property name.")]
        public bool ReplaceOptionSetPropertiesWithEnum
        {
            get => Config.ExtensionConfig.ReplaceOptionSetPropertiesWithEnum;
            set => Config.ExtensionConfig.ReplaceOptionSetPropertiesWithEnum = value;
        }

        [Category("Entities")]
        [DisplayName("Service Context Name")]
        [Description("Specifies the name of the generated CRM Context.")]
        public string ServiceContextName
        {
            get => Config.ServiceContextName;
            set => Config.ServiceContextName = value;
        }

        [Category("Entities")]
        [DisplayName("Unmapped Properties")]
        [Description("Allows for the ability to specify an OptionSetValue Property of an entity that doesn't have an enum mapping.")]
        [Editor(typeof(SpecifyAttributesEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionCountConverter))]
        public Dictionary<string, HashSet<string>> UnmappedProperties { get; set; }

        #endregion Entities

        #region Global

        [Category("Global")]
        [DisplayName("Add New Files To Project")]
        [Description("Allows adding any newly generated files that don't already exist, to the first project file found in the hierarchy of the output path.")]
        public bool AddNewFilesToProject
        {
            get => Config.ExtensionConfig.AddNewFilesToProject;
            set => Config.ExtensionConfig.AddNewFilesToProject = value;
        }

        [Category("Global")]
        [DisplayName("Audible Completion Notification")]
        [Description("Use speech synthesizer to notify of code generation completion.  May not work on VMs or machines without sound cards.")]
        public bool AudibleCompletionNotification
        {
            get => Config.AudibleCompletionNotification;
            set => Config.AudibleCompletionNotification = value;
        }

        [Category("Entities")]
        [DisplayName("Use Logical Names for Properties")]
        [Description("Generate properties of entities with logical name instead of schema name. May result in breaking code if a field has the same logical name as the entity. Attribute namings can be overridden using \"Attribute Capitalization Override\" setting.")]
        public bool UseLogicalNames
        {
            get => Config.ExtensionConfig.UseLogicalNames;
            set => Config.ExtensionConfig.UseLogicalNames = value;
        }

        [Category("Global")]
        [DisplayName("Camel Case Class Names")]
        [Description("Using a dictionary, attempts to correctly camelcase class/enum names.")]
        public bool CamelCaseClassNames
        {
            get => Config.ExtensionConfig.CamelCaseClassNames;
            set => Config.ExtensionConfig.CamelCaseClassNames = value;
        }

        [Category("Global")]
        [DisplayName("Camel Case Member Names")]
        [Description("Using a dictionary, attempts to correctly camelcase column/parameter names")]
        public bool CamelCaseMemberNames
        {
            get => Config.ExtensionConfig.CamelCaseMemberNames;
            set => Config.ExtensionConfig.CamelCaseMemberNames = value;
        }

        [Category("Global")]
        [DisplayName("CrmSvcUtil Relative Path")]
        [Description("The Path to the CrmSvcUtil.exe, relative to the CrmSvcUtil Realtive Root Path.  Defaults to using the CrmSvcUtil that comes by default.")]
        public string CrmSvcUtilRelativePath
        {
            get => Config.CrmSvcUtilRelativePath;
            set => Config.CrmSvcUtilRelativePath = value;
        }

        [Category("Global")]
        [DisplayName("Delete Files From Output Folders")]
        [Description("Clears all .cs files from output folders prior to file generation.  This helps to remove files that are no longer being generated.  Only used if Create One File Per Action/Entity/OptionSet is true.")]
        public bool DeleteFilesFromOutputFolders
        {
            get => Config.ExtensionConfig.DeleteFilesFromOutputFolders;
            set => Config.ExtensionConfig.DeleteFilesFromOutputFolders = value;
        }

        [Category("Global")]
        [DisplayName("File Prefix Text")]
        [Description("Text that appears at the top of every file.  Could be used to include Copyright information or #Paragma warning disable statements.  This is a string format text, with {0} being the file name with extension.")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string FilePrefixText
        {
            get => Config.ExtensionConfig.FilePrefixText;
            set => Config.ExtensionConfig.FilePrefixText = value;
        }

        [Category("Global")]
        [DisplayName("Suppress Generated Code Attribute")]
        [Description("Enables SuppressGeneratedCodeAttribute command line parameters.")]
        public bool SuppressGeneratedCodeAttribute
        {
            get => Config.SuppressGeneratedCodeAttribute;
            set => Config.SuppressGeneratedCodeAttribute = value;
        }

        [Category("Global")]
        [DisplayName("Include Command Line")]
        [Description("Specifies whether to include in the early bound class the command line used to generate it.")]
        public bool IncludeCommandLine
        {
            get => Config.IncludeCommandLine;
            set => Config.IncludeCommandLine = value;
        }

        [Category("Global")]
        [DisplayName("Mask Password")]
        [Description("Masks the password in the outputted command line.")]
        public bool MaskPassword
        {
            get => Config.MaskPassword;
            set => Config.MaskPassword = value;
        }

        [Category("Global")]
        [DisplayName("Namespace")]
        [Description("The Namespace generated code will be placed in.")]
        public string Namespace
        {
            get => Config.Namespace;
            set => Config.Namespace = value;
        }

        [Category("Global")]
        [DisplayName("Project Name For Early Bound Files")]
        [Description("(Optional) Defines the actual project name to search for when searching for a project file. If no value is provided, the first project file found will be used.")]
        public string ProjectNameForEarlyBoundFiles
        {
            get => Config.ExtensionConfig.ProjectNameForEarlyBoundFiles;
            set => Config.ExtensionConfig.ProjectNameForEarlyBoundFiles = value;
        }

        [Category("Global")]
        [DisplayName("Remove Runtime Version Comment")]
        [Description(@"Removes the ""//   Runtime Version:X.X.X.X"" comment from the header of generated files.
This helps to alleviate unnecessary differences that pop up when the classes are generated from machines with different .Net Framework updates installed.")]
        public bool RemoveRuntimeVersionComment
        {
            get => Config.ExtensionConfig.RemoveRuntimeVersionComment;
            set => Config.ExtensionConfig.RemoveRuntimeVersionComment = value;
        }

        [Category("Global")]
        [DisplayName("Token Capitalization Overrides")]
        [Description("Used in conjunction with Camel Case Class Names and Camel Case Member Names to override any defaults.")]
        [Editor(StringEditorName, typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionCountConverter))]
        public List<string> TokenCapitalizationOverrides { get; set; }

        [Category("Global")]
        [DisplayName("Use Tfs")]
        [Description("Will use TFS to attempt to check out the early bound classes.  Not needed for Git based repositories.")]
        public bool UseTfsToCheckoutFiles
        {
            get => Config.ExtensionConfig.UseTfsToCheckoutFiles;
            set => Config.ExtensionConfig.UseTfsToCheckoutFiles = value;
        }

        [Category("Global")]
        [DisplayName("Use 'internal' as the access modifier for the generated types")]
        [Description("Start marking the early bound classes as 'internal' instead of 'public'")]
        public bool UseInternalAsAccessModifier
        {
            get => Config.ExtensionConfig.UseInternalAsAccessModifier;
            set => Config.ExtensionConfig.UseInternalAsAccessModifier = value;
        }

        #endregion Global Properties

        #region Meta

        [Category("Meta")]
        [DisplayName("Settings Version")]
        [Description("The Settings File Version.")]
        public string SettingsVersion => Config.SettingsVersion;

        [Category("Meta")]
        [DisplayName("Early Bound Generator Version")]
        [Description("Version of the Early Bound Generator.")]
        public string Version => Config.Version;

        #endregion Meta

        #region Option Sets

        [Category("Option Sets")]
        [DisplayName("Add Option Set Metadata Attribute")]
        [Description("Adds the OptionSetMetadataAttribute to enums to be able to access enum metadata.  Ensure Generate Option Set Metadata Attribute is true to generate the attribute definition, unless this has been handled in some other manner.")]
        public bool AddOptionSetMetadataAttribute
        {
            get => Config.ExtensionConfig.AddOptionSetMetadataAttribute;
            set => Config.ExtensionConfig.AddOptionSetMetadataAttribute = value;
        }

        [Category("Option Sets")]
        [DisplayName("Create One File Per Option Set")]
        [Description("Specifies that each Option Set Enum should be outputted to it's own file, rather than a single file with all Enums.")]
        public bool CreateOneFilePerOptionSet
        {
            get => Config.ExtensionConfig.CreateOneFilePerOptionSet;
            set => Config.ExtensionConfig.CreateOneFilePerOptionSet = value;
        }

        [Category("Option Sets")]
        [DisplayName("Generate Only Referenced Option Sets")]
        [Description("Uses the defined entity filtering to only generate Option Set Enums that are actually referenced in the entities.  This include both Global and Local Option Sets. ")]
        public bool GenerateOnlyReferencedOptionSets
        {
            get => Config.ExtensionConfig.GenerateOnlyReferencedOptionSets;
            set => Config.ExtensionConfig.GenerateOnlyReferencedOptionSets = value;
        }

        [Category("Option Sets")]
        [DisplayName("Group Local Option Sets By Entity")]
        [Description("Combines all local option sets into a single file per entity.  Only used if Create One File Per Option Set is true.")]
        public bool GroupLocalOptionSetsByEntity
        {
            get => Config.ExtensionConfig.GroupLocalOptionSetsByEntity;
            set => Config.ExtensionConfig.GroupLocalOptionSetsByEntity = value;
        }

        [Category("Option Sets")]
        [DisplayName("Invalid C# Name Prefix")]
        [Description("Specifies the Prefix to be used for OptionSets that would normally start with an invalid first character ie \"1st\"")]
        public string InvalidCSharpNamePrefix
        {
            get => Config.ExtensionConfig.InvalidCSharpNamePrefix;
            set => Config.ExtensionConfig.InvalidCSharpNamePrefix = value;
        }

        [Category("Option Sets")]
        [DisplayName("Local Option Set Format")]
        [Description("Defines the format of Local Option Sets where {0} is the Entity Schema Name, and {1} is the Attribute Schema Name.   The format Specified in the SDK is {0}{1}, but the default is {0}_{1}, but used to be prefix_{0}_{1}(all lower case).")]
        public string LocalOptionSetFormat
        {
            get => Config.ExtensionConfig.LocalOptionSetFormat;
            set => Config.ExtensionConfig.LocalOptionSetFormat = value;
        }

        [Category("Option Sets")]
        [DisplayName("Option Set Output Relative Path")]
        [Description("This is realtive to the Path of the Settings File.  If \"Create One File Per Option Set\" is enabled, this needs to be a file path that ends in \".cs\", else, this needs to be a path to a directory.")]
        [Editor(typeof(PathEditor), typeof(UITypeEditor))]
        [DynamicRelativePathEditor(nameof(SettingsPath), nameof(CreateOneFilePerOptionSet), "C# files|*.cs", "cs", false)]
        public string OptionSetOutPath
        {
            get => Config.OptionSetOutPath;
            set => Config.OptionSetOutPath = GetRelativePathToFileOrDirectory(value, CreateOneFilePerOptionSet, "OptionSets.cs");
        }

        [Category("Option Sets")]
        [DisplayName("Option Set Prefix Blacklist")]
        [Description("Contains list of prefixes to not generate.  If the Option Set starts with the given prefix, it will not be generated.")]
        [Editor(StringEditorName, typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionCountConverter))]
        public List<string> OptionSetPrefixesToSkip { get; set; }

        [Category("Option Sets")]
        [DisplayName("Option Sets Blacklist")]
        [Description("Allows for the ability to specify OptionSets to not generate.")]
        [Editor(typeof(OptionSetsHashEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionCountConverter))]
        public HashSet<string> OptionSetsToSkip { get; set; }

        [Category("Option Sets")]
        [DisplayName("Option Set Value Language Code Override")]
        [Description("Overrides the default (English:1033) language code used for generating Option Set Value names (the value, not the option set).")]
        public int? OptionSetLanguageCodeOverride
        {
            get => Config.ExtensionConfig.OptionSetLanguageCodeOverride;
            set => Config.ExtensionConfig.OptionSetLanguageCodeOverride = value;
        }

        [Category("Option Sets")]
        [DisplayName("Option Set Names")]
        [Description("If Option Sets have idential names to Entities, it will cause naming conflicts.  By default this is overcome by postfixing \"_Enum\" to the Option Set name.  This setting allows a custom mapping for option set names to be specified.")]
        [Editor(typeof(DictionaryEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionCountConverter))]
        public Dictionary<string, string> OptionSetNames { get; set; }

        [Category("Option Sets")]
        [DisplayName("Use Deprecated Option Set Naming")]
        [Description("Creates Local OptionSets Using the Deprecated Naming Convention. prefix_oobentityname_prefix_attribute.")]
        public bool UseDeprecatedOptionSetNaming
        {
            get => Config.ExtensionConfig.UseDeprecatedOptionSetNaming;
            set => Config.ExtensionConfig.UseDeprecatedOptionSetNaming = value;
        }

        #endregion Option Sets

        [Browsable(false)]
        public string SettingsPath { get; set; }

        /// <summary>
        /// True if at least one Action/Entity/OptionSet will create one file per item, else False.
        /// </summary>
        [Browsable(false)]
        private bool AtLeastOneCreateFilePerSelected =>
            CreateOneFilePerAction
            || CreateOneFilePerEntity
            || CreateOneFilePerOptionSet;

        private EarlyBoundGeneratorPlugin Plugin { get; }

        private EarlyBoundGeneratorConfig Config { get; }

        #endregion Properties

        public SettingsMap(EarlyBoundGeneratorPlugin plugin, EarlyBoundGeneratorConfig config)
        {
            Plugin = plugin;
            Config = config;

            var propertyToParse = nameof(ActionPrefixesWhitelist);
            var propertyValue = string.Empty;
            string RemoveWhiteSpace(string propertyName, string value)
            {
                propertyToParse = propertyName;
                propertyValue = value;
                return value?.Replace(" ", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty);
            }

            try
            {

                var info = new ConfigKeyValueSplitInfo {ConvertKeysToLower = false};
                ActionPrefixesWhitelist = RemoveWhiteSpace(nameof(ActionPrefixesWhitelist), config.ExtensionConfig.ActionPrefixesWhitelist).GetList<string>();
                ActionsWhitelist = RemoveWhiteSpace(nameof(ActionsWhitelist), config.ExtensionConfig.ActionsWhitelist).GetHashSet<string>();
                ActionsToSkip = RemoveWhiteSpace(nameof(ActionsToSkip), config.ExtensionConfig.ActionsToSkip).GetHashSet<string>(info);
                EntitiesToSkip = RemoveWhiteSpace(nameof(EntitiesToSkip), config.ExtensionConfig.EntitiesToSkip).GetHashSet<string>();
                EntitiesWhitelist = RemoveWhiteSpace(nameof(EntitiesWhitelist), config.ExtensionConfig.EntitiesWhitelist).GetHashSet<string>();
                EntityAttributeSpecifiedNames = RemoveWhiteSpace(nameof(EntityAttributeSpecifiedNames), config.ExtensionConfig.EntityAttributeSpecifiedNames).GetDictionaryHash<string, string>();
                EntityPrefixesToSkip = RemoveWhiteSpace(nameof(EntityPrefixesToSkip), config.ExtensionConfig.EntityPrefixesToSkip).GetList<string>();
                EntityPrefixesWhitelist = RemoveWhiteSpace(nameof(EntityPrefixesWhitelist), config.ExtensionConfig.EntityPrefixesWhitelist).GetList<string>();
                PropertyEnumMappings = RemoveWhiteSpace(nameof(PropertyEnumMappings), config.ExtensionConfig.PropertyEnumMappings).GetList<string>();
                OptionSetNames = RemoveWhiteSpace(nameof(OptionSetNames), Config.ExtensionConfig.OptionSetNames).GetDictionary<string,string>();
                OptionSetPrefixesToSkip = RemoveWhiteSpace(nameof(OptionSetPrefixesToSkip), config.ExtensionConfig.OptionSetPrefixesToSkip).GetList<string>();
                OptionSetsToSkip = RemoveWhiteSpace(nameof(OptionSetsToSkip), config.ExtensionConfig.OptionSetsToSkip).GetHashSet<string>();
                TokenCapitalizationOverrides = RemoveWhiteSpace(nameof(TokenCapitalizationOverrides), config.ExtensionConfig.TokenCapitalizationOverrides).GetList<string>();
                UnmappedProperties = RemoveWhiteSpace(nameof(UnmappedProperties), config.ExtensionConfig.UnmappedProperties).GetDictionaryHash<string, string>();
                WorkflowlessActions = RemoveWhiteSpace(nameof(WorkflowlessActions), config.WorkflowlessActions).GetList<string>();
            }
            catch (Exception ex)
            {
                throw new FormatException("Unable parsing property " + propertyToParse + Environment.NewLine + "Value: " + propertyValue + Environment.NewLine, ex);
            }

            SetupCustomTypeDescriptor();
            OnChangeMap = GetOnChangeHandlers();
            ProcessDynamicallyVisibleProperties();
        }

        /// <summary>
        /// The display values are different than the serialized versions and have to be updated.
        /// </summary>
        public void PushChanges()
        {
            var info = new ConfigKeyValueSplitInfo{ ConvertKeysToLower = false};
            Config.ExtensionConfig.ActionPrefixesWhitelist = CommonConfig.ToStringSorted(ActionPrefixesWhitelist, info);
            Config.ExtensionConfig.ActionsWhitelist = CommonConfig.ToStringSorted(ActionsWhitelist, info);
            Config.ExtensionConfig.ActionsToSkip = CommonConfig.ToStringSorted(ActionsToSkip, info);
            Config.ExtensionConfig.EntitiesToSkip = CommonConfig.ToStringSorted(EntitiesToSkip);
            Config.ExtensionConfig.EntitiesWhitelist = CommonConfig.ToStringSorted(EntitiesWhitelist);
            Config.ExtensionConfig.EntityAttributeSpecifiedNames = CommonConfig.ToStringSorted(EntityAttributeSpecifiedNames);
            Config.ExtensionConfig.EntityPrefixesToSkip = CommonConfig.ToStringSorted(EntityPrefixesToSkip);
            Config.ExtensionConfig.EntityPrefixesWhitelist = CommonConfig.ToStringSorted(EntityPrefixesWhitelist);
            Config.ExtensionConfig.PropertyEnumMappings = CommonConfig.ToStringSorted(PropertyEnumMappings);
            Config.ExtensionConfig.OptionSetNames = CommonConfig.ToStringSorted(OptionSetNames);
            Config.ExtensionConfig.OptionSetPrefixesToSkip = CommonConfig.ToStringSorted(OptionSetPrefixesToSkip);
            Config.ExtensionConfig.OptionSetsToSkip = CommonConfig.ToStringSorted(OptionSetsToSkip);
            Config.ExtensionConfig.TokenCapitalizationOverrides = CommonConfig.ToStringSorted(TokenCapitalizationOverrides);
            Config.ExtensionConfig.UnmappedProperties = CommonConfig.ToStringSorted(UnmappedProperties);
            Config.WorkflowlessActions = CommonConfig.ToStringSorted(WorkflowlessActions, info);
        }

        public EarlyBoundGeneratorPlugin GetPluginControl()
        {
            return Plugin;
        }

        PluginControlBase IGetPluginControl.GetPluginControl()
        {
            return GetPluginControl();
        }

        private string GetRelativePathToFileOrDirectory(string value, bool requireFile, string defaultFileName)
        {
            var isCsFile = value.ToLower().EndsWith(".cs");
            if (requireFile)
            {
                if (isCsFile)
                {
                    value = System.IO.Path.GetDirectoryName(value) + @"\";
                }
            }
            else if (!isCsFile)
            {
                value = System.IO.Path.Combine(value, defaultFileName);
            }

            return value;
        }
    }
}
