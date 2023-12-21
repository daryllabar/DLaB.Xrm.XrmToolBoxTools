using DLaB.Common;
using DLaB.XrmToolBoxCommon.Editors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using XrmToolBox.Extensibility;
using CommonConfig = DLaB.Common.Config;

// ReSharper disable UnusedMember.Global
namespace DLaB.EarlyBoundGeneratorV2.Settings
{
    [TypeConverter(typeof(FilterPropertyTypeConverter))]
    public partial class SettingsMap: IGetPluginControl<EarlyBoundGeneratorPlugin>
    {
        private const string StringEditorName = @"System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

        #region Properties

        #region Debug

        [Category("7 - Debug")]
        [DisplayName("Model Builder Log Level")]
        [Description("Trace = 0 - Logs that contain the most detailed messages. These messages may contain sensitive application data.  These messages are disabled by default and should never be enabled in a production environment.\r\nDebug = 1 - Logs that are used for interactive investigation during development.  These logs should primarily contain information useful for debugging and have no long-term value.\r\nInformation = 2 - Logs that track the general flow of the application. These logs should have long-term value.\r\nWarning = 3 - Logs that highlight an abnormal or unexpected event in the application flow, but do not otherwise cause the application execution to stop.\r\nError = 4 - Logs that highlight when the current flow of execution is stopped due to a failure. These should indicate a failure in the current activity, not an application-wide failure.\r\nCritical = 5 - Logs that describe an unrecoverable application or system crash, or a catastrophic failure that requires immediate attention.\r\nNone = 6 - Not used for writing log messages. Specifies that a logging category should not write any messages.")]
        public string ModelBuilderLogLevel
        {
            get => Config.ExtensionConfig.ModelBuilderLogLevel;
            set => Config.ExtensionConfig.ModelBuilderLogLevel = byte.TryParse(value, out var result) && result <= 6 ? result.ToString() : "2";
        }

        [Category("7 - Debug")]
        [DisplayName("Read Serialized Metadata")]
        [Description("For Debugging Only!  Used to not communicate to the server for the metadata, but to use the local metadata file instead.  Should only be used for testing generation outputs.  Set Serialize Metadata to true first to connect to the server and retrieve the metadata, then set it back to false to not write it again since it's already local.")]
        public bool ReadSerializedMetadata
        {
            get => Config.ExtensionConfig.ReadSerializedMetadata;
            set => Config.ExtensionConfig.ReadSerializedMetadata = value;
        }

        [Category("7 - Debug")]
        [DisplayName("Serialize Metadata")]
        [Description("For Debugging Only!  Serializes the Metadata to a local file on disk.  By default, this writes to a file named \"metadata.xml\" in the directory of the XrmToolBox.exe file. (Generates a 200-400+ mb xml file).")]
        public bool SerializeMetadata
        {
            get => Config.ExtensionConfig.SerializeMetadata;
            set => Config.ExtensionConfig.SerializeMetadata = value;
        }

        [Category("7 - Debug")]
        [DisplayName("Update Builder Settings Json")]
        [Description("If this is set to false, then all setting changes made in the Early Bound Generator will not take affect outside of out directory since the builderSettings.json file isn't getting updated, but is helpful if custom editing of the builderSettings.json file is required.  Please note, the \"dLaB.ModelBuilder.xrmToolBoxPluginPath\" will most likely need to be set.")]
        public bool UpdateBuilderSettingsJson
        {
            get => Config.UpdateBuilderSettingsJson;
            set => Config.UpdateBuilderSettingsJson = value;
        }

        [Category("7 - Debug")]
        [DisplayName("Wait For Attached Debugger")]
        [Description("For Debugging Only!  Waits until a debugger is attached to the ModelBuilder before processing the command.")]
        public bool WaitForAttachedDebugger
        {   
            get => Config.ExtensionConfig.WaitForAttachedDebugger;
            set => Config.ExtensionConfig.WaitForAttachedDebugger = value;
        }

        #endregion Debug

        #region Entities

        [Category("2 - Entities")]
        [DisplayName("Add DebuggerNonUserCode")]
        [Description("Specifies that the DebuggerNonUserCodeAttribute should be applied to all generated properties and methods.\n\rThis directs the debugger to skip stepping into generated entity files.")]
        public bool AddDebuggerNonUserCode
        {
            get => Config.ExtensionConfig.AddDebuggerNonUserCode;
            set => Config.ExtensionConfig.AddDebuggerNonUserCode = value;
        }

        [Category("2 - Entities")]
        [DisplayName("Create One File Per Entity")]
        [Description("Specifies that each Entity class should be outputted to it's own file, rather than a single file with all entities.")]
        public bool CreateOneFilePerEntity
        {
            get => Config.ExtensionConfig.CreateOneFilePerEntity;
            set => Config.ExtensionConfig.CreateOneFilePerEntity = value;
        }

        [Category("2 - Entities")]
        [DisplayName("Emit Virtual Attributes")]
        [Description("When set, includes the Virtual Attributes of entities in the generated code.")]
        public bool EmitVirtualAttributes
        {
            get => Config.EmitVirtualAttributes;
            set => Config.EmitVirtualAttributes = value;
        }

        [Category("2 - Entities")]
        [DisplayName("Entity Relative Output Path")]
        [Description("This is realtive to the Path of the Output Relative Directory (or if not populated, the Settings File).  If \"Create One File Per Entity\" is enabled, this needs to be a file path that ends in \".cs\", else, this needs to be a path to a directory.")]
        [Editor(typeof(PathEditor), typeof(UITypeEditor))]
        [DynamicRelativePathEditor(nameof(SettingsPath), nameof(CreateOneFilePerEntity), "C# files|*.cs", "cs", false)]
        public string EntityTypesFolder
        {
            get => Config.EntityTypesFolder;
            set => Config.EntityTypesFolder = GetRelativePathToFileOrDirectory(value, CreateOneFilePerEntity, "Entities.cs");
        }

        [Category("2 - Entities")]
        [DisplayName("Entities Blacklist")]
        [Description("Contains Entities to not generate.")]
        [Editor(typeof(EntitiesHashEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionCountConverter))]
        public HashSet<string> EntitiesBlacklist { get; set; }

        [Category("2 - Entities")]
        [DisplayName("Entities Whitelist")]
        [Description("Contains only the Entities to generate.  If empty, all Entities will be included.")]
        [Editor(typeof(EntitiesHashEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionCountConverter))]
        [CollectionCount("All Entities")]
        public HashSet<string> EntitiesWhitelist { get; set; }

        [Category("2 - Entities")]
        [DisplayName("Attribute Capitalization Override")]
        [Description("Allows for the ability to specify the capitalization of an attribute on an entity.")]
        [Editor(typeof(SpecifyAttributesCaseEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionCountConverter))]
        public Dictionary<string, HashSet<string>> EntityAttributeSpecifiedNames { get; set; }

        [Category("2 - Entities")]
        [DisplayName("Entity Class Name Overrides")]
        [Description("Allows for specifying a specific name or casing for an Entity class.  For example, if the business refers to an 'Account' as a 'Family', specifying a mapping from 'account' to 'Family' will result in the name of the C# class being 'Family', even though the logical name would still be 'account'.")]
        [Editor(typeof(SpecifyEntityNameEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionCountConverter))]
        public Dictionary<string,string> EntityClassNameOverrides { get;set;}

        [Category("2 - Entities")]
        [DisplayName("Entities RegEx Blacklist")]
        [Description("RegEx used to exclude entities from being generated.  If the entity logical name matches the given regex, it will not be generated. (Reminder, a wild card is \".*\" in regex)")]
        [Editor(StringEditorName, typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionCountConverter))]
        public List<string> EntityRegExBlacklist { get; set; }

        [Category("2 - Entities")]
        [DisplayName("Entities Wildcard Whitelist")]
        [Description("Entity wildcard \"*\" to generate.  If the Entity matches the wildcard search value it will be included.")]
        [Editor(StringEditorName, typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionCountConverter))]
        public List<string> EntityWildcardWhitelist { get; set; }

        [Category("2 - Entities")]
        [DisplayName("Generate Entity Attribute Name Constants")]
        [Description("Adds a Static Class to each Entity class that contains the Logical Names of all attributes for the Entity.")]
        public bool GenerateAttributeNameConsts
        {
            get => Config.ExtensionConfig.GenerateAttributeNameConsts;
            set => Config.ExtensionConfig.GenerateAttributeNameConsts = value;
        }

        [Category("2 - Entities")]
        [DisplayName("Generate Anonymous Type Constructor")]
        [Description("Adds an Object Constructor to each early bound entity class to simplify LINQ projections (http://stackoverflow.com/questions/27623542).")]
        public bool GenerateAnonymousTypeConstructor
        {
            get => Config.ExtensionConfig.GenerateAnonymousTypeConstructor;
            set => Config.ExtensionConfig.GenerateAnonymousTypeConstructor = value;
        }

        [Category("2 - Entities")]
        [DisplayName("Generate Constructor to match Microsoft.Xrm.Sdk.Entity Constructors")]
        [Description("Adds Constructors to each early bound entity class to use constructors available to Microsoft.Xrm.Sdk.Entity")]
        public bool GenerateConstructorsSansLogicalName
        {
            get => Config.ExtensionConfig.GenerateConstructorsSansLogicalName;
            set => Config.ExtensionConfig.GenerateConstructorsSansLogicalName = value;
        }

        [Category("2 - Entities")]
        [DisplayName("Generate Entity Relationships")]
        [Description("Specifies if 1:N, N:1, and N:N relationships properties are generated for entities.")]
        public bool GenerateEntityRelationships
        {
            get => Config.ExtensionConfig.GenerateEntityRelationships;
            set => Config.ExtensionConfig.GenerateEntityRelationships = value;
        }

        [Category("2 - Entities")]
        [DisplayName("Generate Entity Type Code")]
        [Description("By default the ModelBuilder generates the Entity Type Code, this is considered dangerous to use and is not recommended since it is a system generated value, and not one defined in the solution metadata, changing from environment to environment.")]
        public bool GenerateEntityTypeCode
        {
            get => Config.EmitEntityETC;
            set => Config.EmitEntityETC = value;
        }

        [Category("2 - Entities")]
        [DisplayName("Generate Enum Properties")]
        [Description("OptionSet attributes are generated as enums rather than Option Sets.  Default is to replace the OptionSet property with a typed enum version, but they can also be generated as an additional property using the \"2 - Entities - Replace Option Set Properties with Enum\" setting.")]
        public bool GenerateEnumProperties
        {
            get => Config.ExtensionConfig.GenerateEnumProperties;
            set => Config.ExtensionConfig.GenerateEnumProperties = value;
        }

        [Category("2 - Entities")]
        [DisplayName("Generate INotify Pattern")]
        [Description("Specifies that Entity class should implement the INotifyPropertyChanging and INotifyPropertyChanged interfaces, calling OnPropertyChanging and OnPropertyChanged for each set of an attributes")]
        public bool GenerateINotifyPattern
        {
            get => Config.ExtensionConfig.GenerateINotifyPattern;
            set => Config.ExtensionConfig.GenerateINotifyPattern = value;
        }

        [Category("2 - Entities")]
        [DisplayName("Make All Fields Editable")]
        [Description("Defines that Entities should be created with all attributes as editable.  This may be confusing for some developers because attempts to update FullName on the contact will silently fail.")]
        public bool MakeAllFieldsEditable
        {
            get => Config.ExtensionConfig.MakeAllFieldsEditable;
            set => Config.ExtensionConfig.MakeAllFieldsEditable = value;
        }

        [Category("2 - Entities")]
        [DisplayName("Make Readonly Fields Editable")]
        [Description("Defines that Entities should be created with editable createdby, createdon, modifiedby, modifiedon, owningbusinessunit, owningteam, and owninguser properties.\n\rHelpful for writing linq statements where those attributes are wanting to be returned in the select.")]
        public bool MakeReadonlyFieldsEditable
        {
            get => Config.ExtensionConfig.MakeReadonlyFieldsEditable;
            set => Config.ExtensionConfig.MakeReadonlyFieldsEditable = value;
        }

        [Category("2 - Entities")]
        [DisplayName("Property Enum Mapping")]
        [Description("Manually specifies an enum mapping for an OptionSetValue Property on an entity.\n\rThis is useful if you have multiple local options that really are the same value.  This then allows easier comparision since the enums don't have to be converted.")]
        [Editor(typeof(AttributesToEnumMapperEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionCountConverter))]
        public List<string> PropertyEnumMappings { get; set; }

        [Category("2 - Entities")]
        [DisplayName("Replace Option Set Properties with Enum")]
        [Description("Only used when Generate Enum Properties is true.  When true, replaces OptionSet properties with Enum properties.  When false, each OptionSet properties is duplicated with Enum postfixed to the existing optionset property name.")]
        public bool ReplaceOptionSetPropertiesWithEnum
        {
            get => Config.ExtensionConfig.ReplaceOptionSetPropertiesWithEnum;
            set => Config.ExtensionConfig.ReplaceOptionSetPropertiesWithEnum = value;
        }

        [Category("2 - Entities")]
        [DisplayName("Service Context Name")]
        [Description("Specifies the name of the generated CRM Context.")]
        public string ServiceContextName
        {
            get => Config.ServiceContextName;
            set => Config.ServiceContextName = value;
        }

        [Category("2 - Entities")]
        [DisplayName("Use Enum For State Codes")]
        [Description("The PAC ModelBuilder generates state codes as enums properties.  This allows for generating just this property as an enum.  Only valid when Replace Option Set Properties with Enum is false.")]
        public bool UseEnumForStateCodes
        {
            get => Config.ExtensionConfig.UseEnumForStateCodes;
            set => Config.ExtensionConfig.UseEnumForStateCodes = value;
        }

        #endregion Entities

        #region Global

        [Category("1 - Global")]
        [DisplayName("Add New Files To Project")]
        [Description("Allows adding any newly generated files that don't already exist, to the first project file found in the hierarchy of the output path.  ** NOTE ** Visual Studio tends to cache shared project files in the projects that are shared by it, which causes the file to not get loaded correctly.  It is recommended to either unload the shared project or shut down Visual Studio before generating entities in order for this to work correctly.")]
        public bool AddNewFilesToProject
        {
            get => Config.ExtensionConfig.AddNewFilesToProject;
            set => Config.ExtensionConfig.AddNewFilesToProject = value;
        }

        [Category("1 - Global")]
        [DisplayName("Audible Completion Notification")]
        [Description("Use speech synthesizer to notify of code generation completion.  May not work on VMs or machines without sound cards.")]
        public bool AudibleCompletionNotification
        {
            get => Config.AudibleCompletionNotification;
            set => Config.AudibleCompletionNotification = value;
        }

        [Category("1 - Global")]
        [DisplayName("Builder Settings Json Relative Path")]
        [Description("This is relative to the Path of the Output Relative Directory (or if not populated, the Settings File).  This should end with the a \".json\" extension.")]
        public string BuilderSettingsJsonRelativePath
        {
            get => Config.ExtensionConfig.BuilderSettingsJsonRelativePath;
            set => Config.ExtensionConfig.BuilderSettingsJsonRelativePath = value;
        }

        [Category("2 - Entities")]
        [DisplayName("Use Logical Names for Properties")]
        [Description("Generate properties of entities with logical name instead of schema name. May result in breaking code if a field has the same logical name as the entity. Attribute namings can be overridden using \"Attribute Capitalization Override\" setting.")]
        public bool UseLogicalNames
        {
            get => Config.ExtensionConfig.UseLogicalNames;
            set => Config.ExtensionConfig.UseLogicalNames = value;
        }

        [Category("1 - Global")]
        [DisplayName("Camel Case Class Names")]
        [Description("Using a dictionary, attempts to correctly camelcase class/enum names.")]
        public bool CamelCaseClassNames
        {
            get => Config.ExtensionConfig.CamelCaseClassNames;
            set => Config.ExtensionConfig.CamelCaseClassNames = value;
        }

        [Category("1 - Global")]
        [DisplayName("Camel Case Custom Words")]
        [Description("Custom words to add to the standard dictionary file.  Allows for more control of the camel case naming convention, without having to check the entire dictionary file in.")]
        [Editor(StringEditorName, typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionCountConverter))]
        public List<string> CamelCaseCustomWords { get; set; }

        [Category("1 - Global")]
        [DisplayName("Camel Case Dictionary Relative Path")]
        [Description("The path relative to the XrmToolBox Plugins directory to a Dictionary file containing a single word on each line.  This is used when auto camel casing names when either the \"Global: Camel Case Class Names\" or \"Global: Camel Case Member Names\" is set to true.")]
        public string CamelCaseNamesDictionaryRelativePath
        {
            get => Config.ExtensionConfig.CamelCaseNamesDictionaryRelativePath;
            set => Config.ExtensionConfig.CamelCaseNamesDictionaryRelativePath = value;
        }

        [Category("1 - Global")]
        [DisplayName("Camel Case Member Names")]
        [Description("Using a dictionary, attempts to correctly camelcase column/parameter names")]
        public bool CamelCaseMemberNames
        {
            get => Config.ExtensionConfig.CamelCaseMemberNames;
            set => Config.ExtensionConfig.CamelCaseMemberNames = value;
        }

        [Category("1 - Global")]
        [DisplayName("Delete Files From Output Folders")]
        [Description("Clears all .cs files from output folders prior to file generation.  This helps to remove files that are no longer being generated.  Only used if Create One File Per Entity/Message/OptionSet is true.")]
        public bool DeleteFilesFromOutputFolders
        {
            get => Config.ExtensionConfig.DeleteFilesFromOutputFolders;
            set => Config.ExtensionConfig.DeleteFilesFromOutputFolders = value;
        }

        [Category("1 - Global")]
        [DisplayName("File Prefix Text")]
        [Description("Text that appears at the top of every file.  Could be used to include Copyright information or #Paragma warning disable statements.  This is a string format text, with {0} being the file name with extension.")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string FilePrefixText
        {
            get => Config.ExtensionConfig.FilePrefixText;
            set => Config.ExtensionConfig.FilePrefixText = value;
        }


        [Category("1 - Global")]
        [DisplayName("Generate Types As Internal")]
        [Description("All generated types are marked as \"internal\" instead of \"public\".")]
        public bool GenerateTypesAsInternal
        {
            get => Config.ExtensionConfig.GenerateTypesAsInternal;
            set => Config.ExtensionConfig.GenerateTypesAsInternal = value;
        }

        [Category("1 - Global")]
        [DisplayName("Suppress Generated Code Attribute")]
        [Description("Suppress all generated objects being tagged with Genereated code Attribute, containing the code generation engine and version.")]
        public bool SuppressGeneratedCodeAttribute
        {
            get => Config.SuppressGeneratedCodeAttribute;
            set => Config.SuppressGeneratedCodeAttribute = value;
        }

        [Category("1 - Global")]
        [DisplayName("Include Command Line")]
        [Description("Specifies whether to include in the service context class the command line used to generate the classes.")]
        public bool IncludeCommandLine
        {
            get => Config.IncludeCommandLine;
            set => Config.IncludeCommandLine = value;
        }

        [Category("1 - Global")]
        [DisplayName("Namespace")]
        [Description("The Namespace generated code will be placed in.")]
        public string Namespace
        {
            get => Config.Namespace;
            set => Config.Namespace = value;
        }

        [Category("1 - Global")]
        [DisplayName("Output Relative Directory")]
        [Description("By default, this is the directory of the Settings file.  But if populated, this directory will be used instead, and all other paths will be relative to this one.")]
        [Editor(typeof(PathEditor), typeof(UITypeEditor))]
        [DynamicRelativePathEditor(nameof(SettingsPath), true, checkFileExists: false)]
        public string OutputRelativeDirectory
        {
            get => Config.ExtensionConfig.OutputRelativeDirectory;
            set => Config.ExtensionConfig.OutputRelativeDirectory = value;
        }

        [Category("1 - Global")]
        [DisplayName("Project Name For Early Bound Files")]
        [Description("(Optional) Defines the actual project name to search for when searching for a project file. If no value is provided, the first project file found will be used.")]
        public string ProjectNameForEarlyBoundFiles
        {
            get => Config.ExtensionConfig.ProjectNameForEarlyBoundFiles;
            set => Config.ExtensionConfig.ProjectNameForEarlyBoundFiles = value;
        }

        [Category("1 - Global")]
        [DisplayName("Remove Runtime Version Comment")]
        [Description(@"Removes the ""//   Runtime Version:X.X.X.X"" comment from the header of generated files.
This helps to alleviate unnecessary differences that pop up when the classes are generated from machines with different .Net Framework updates installed.")]
        public bool RemoveRuntimeVersionComment
        {
            get => Config.ExtensionConfig.RemoveRuntimeVersionComment;
            set => Config.ExtensionConfig.RemoveRuntimeVersionComment = value;
        }

        [Category("1 - Global")]
        [DisplayName("Suppress Autogenerated File Header Comment")]
        [Description("Prevents generation of the <auto-generated> header comments.")]
        public bool SuppressAutogeneratedFileHeaderComment
        {
            get => Config.ExtensionConfig.SuppressAutogeneratedFileHeaderComment;
            set => Config.ExtensionConfig.SuppressAutogeneratedFileHeaderComment = value;
        }

        [Category("1 - Global")]
        [DisplayName("Token Capitalization Overrides")]
        [Description("Used in conjunction with Camel Case Class Names and Camel Case Member Names to override any defaults.")]
        [Editor(StringEditorName, typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionCountConverter))]
        public List<string> TokenCapitalizationOverrides { get; set; }

        [Category("1 - Global")]
        [DisplayName("Use Display Name For BPF Name")]
        [Description("By default, Business Process Flows entities are named with the format {PublisherPrefix}_bpf_GUID. This will instead generate the bpf clases with the format {PublisherPrefix}_bpf_{DisplayName} and entity relationships with similarly, guid replaced by display name, formats.")]
        public bool UseDisplayNameForBpfName
        {
            get => Config.ExtensionConfig.UseDisplayNameForBpfName;
            set => Config.ExtensionConfig.UseDisplayNameForBpfName = value;
        }

        [Category("1 - Global")]
        [DisplayName("Use Tfs")]
        [Description("Will use TFS to attempt to check out the early bound classes.  Not needed for Git based repositories.")]
        [Browsable(false)]
        public bool UseTfsToCheckoutFiles
        {
            get => Config.ExtensionConfig.UseTfsToCheckoutFiles;
            set => Config.ExtensionConfig.UseTfsToCheckoutFiles = value;
        }

        #endregion Global Properties

        #region Messages

        [Category("4 - Messages")]
        [DisplayName("Message Relative Output Path")]
        [Description("This is realtive to the Path of the Output Relative Directory (or if not populated, the Settings File).  If \"Create One File Per Message\" is enabled, this needs to be a file path that ends in \".cs\", else, this needs to be a path to a directory.")]
        [Editor(typeof(PathEditor), typeof(UITypeEditor))]
        [DynamicRelativePathEditor(nameof(SettingsPath), nameof(CreateOneFilePerMessage), "C# files|*.cs", "cs", false)]
        public string MessageTypesFolder
        {
            get => Config.MessageTypesFolder;
            set => Config.MessageTypesFolder = GetRelativePathToFileOrDirectory(value, CreateOneFilePerMessage, "Messages.cs");
        }

        [Category("4 - Messages")]
        [DisplayName("Message Wildcard Whitelist")]
        [Description("Message wildcard \"*\" to generate.  If the Message matches the wildcard search value it will be included.")]
        [Editor(StringEditorName, typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionCountConverter))]
        public List<string> MessageWildcardWhitelist { get; set; }

        [Category("4 - Messages")]
        [DisplayName("Messages Whitelist")]
        [Description("Allows for the ability to specify Messages that will be included in generation.  \"*\" wildcards are valid. ")]
        [Editor(typeof(ActionsHashEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionCountConverter))]
        [CollectionCount("All Messages")]
        public HashSet<string> MessageWhitelist { get; set; }

        [Category("4 - Messages")]
        [DisplayName("Messages Blacklist")]
        [Description("Allows for the ability to specify Messages to not generate.")]
        [Editor(typeof(ActionsHashEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionCountConverter))]
        public HashSet<string> MessageBlacklist { get; set; }

        [Category("4 - Messages")]
        [DisplayName("Create One File Per Message")]
        [Description("Specifies that each Message class should be outputted to it's own file rather than a single file with all messages.")]
        public bool CreateOneFilePerMessage
        {
            get => Config.ExtensionConfig.CreateOneFilePerAction;
            set => Config.ExtensionConfig.CreateOneFilePerAction = value;
        }

        [Category("4 - Messages")]
        [DisplayName("Generate Message Attribute Name Constants")]
        [Description("Adds a Static Class to each Message class that contains the Logical Names of all properties for the Message.")]
        public bool GenerateMessageAttributeNameConsts
        {
            get => Config.ExtensionConfig.GenerateActionAttributeNameConsts;
            set => Config.ExtensionConfig.GenerateActionAttributeNameConsts = value;
        }

        [Category("4 - Messages")]
        [DisplayName("Generate Messages")]
        [Description("Generates classes for messages (Actions/Custom APIs/Microsoft Messages) as part of code generation.    ")]
        public bool GenerateMessages
        {
            get => Config.GenerateMessages;
            set => Config.GenerateMessages = value;
        }

        [Category("4 - Messages")]
        [DisplayName("Group Message Request With Response")]
        [Description("The CrmSvcUtil version of the Early Bound Generator created seperate files for each message request/response class, but the PAC ModelBuilder combines each request/response pair into a single file.  This allows for turning this off to maintain backwards compability.")]
        public bool GroupMessageRequestWithResponse
        {
            get => Config.ExtensionConfig.GroupMessageRequestWithResponse;
            set => Config.ExtensionConfig.GroupMessageRequestWithResponse = value;
        }

        [Category("4 - Messages")]
        [DisplayName("Make Response Messages Editable")]
        [Description("Specifies that the properties of Response Messages should be editable.")]
        public bool MakeResponseMessagesEditable
        {
            get => Config.ExtensionConfig.MakeResponseActionsEditable;
            set => Config.ExtensionConfig.MakeResponseActionsEditable = value;
        }

        #endregion Messages

        #region Meta

        [Category("5 - Meta")]
        [DisplayName("Settings Version")]
        [Description("The Settings File Version.")]
        public string SettingsVersion => Config.SettingsVersion;

        [Category("5 - Meta")]
        [DisplayName("Early Bound Generator Version")]
        [Description("Version of the Early Bound Generator.")]
        public string Version => Config.Version;

        #endregion Meta

        #region Option Sets

        [Category("3 - Option Sets")]
        [DisplayName("Add Option Set Metadata Attribute")]
        [Description("Adds the OptionSetMetadataAttribute to enums to be able to access enum metadata.  Ensure Generate Option Set Metadata Attribute Class is true to generate the attribute definition, unless this has been handled in some other manner.")]
        public bool AddOptionSetMetadataAttribute
        {
            get => Config.ExtensionConfig.AddOptionSetMetadataAttribute;
            set => Config.ExtensionConfig.AddOptionSetMetadataAttribute = value;
        }

        [Category("3 - Option Sets")]
        [DisplayName("Adjust Casing For Enum Options")]
        [Description("By default, the Dataverse Model Builder will convert the Option labels into a C# friendly name, and not adjust the casing of enum.  This could results in a Label of \"This is a very long label with some capitals (JSON / XML / CSV)\" being created as ThisisaverylonglabelwithsomecapitalsJSONXMLCSV, instead of a much more readable ThisIsAVeryLongLabelWithSomeCapitalsJsonXmlCsv,")]
        public bool AdjustCasingForEnumOptions
        {
            get => Config.ExtensionConfig.AdjustCasingForEnumOptions;
            set => Config.ExtensionConfig.AdjustCasingForEnumOptions = value;
        }

        [Category("3 - Option Sets")]
        [DisplayName("Cleanup CrmSvcUtil Local Option Sets")]
        [Description("The PAC ModelBuilder generates local option sets in the same files as the entity class, but the CrmSvcUtil created these option sets separately.  This setting will look for these files and delete them from the disk.  After the upgrade, this should be disabled.  It is recommended to enable \"1 - Global - Add New Files To Project\" as well to cleanup the files from the project.")]
        public bool CleanupCrmSvcUtilLocalOptionSets
        {
            get => Config.ExtensionConfig.CleanupCrmSvcUtilLocalOptionSets;
            set => Config.ExtensionConfig.CleanupCrmSvcUtilLocalOptionSets = value;
        }

        [Category("3 - Option Sets")]
        [DisplayName("Create One File Per Option Set")]
        [Description("Specifies that each Option Set Enum should be outputted to it's own file, rather than a single file with all Enums.")]
        public bool CreateOneFilePerOptionSet
        {
            get => Config.ExtensionConfig.CreateOneFilePerOptionSet;
            set => Config.ExtensionConfig.CreateOneFilePerOptionSet = value;
        }

        [Category("3 - Option Sets")]
        [DisplayName("Generate All Labels")]
        [Description("Generates all labels by language code in the Option Set Metadata Attributes.  Only used if Add Option Set Metadata Attribute is true.")]
        public bool GenerateAllOptionSetLabelMetadata
        {
            get => Config.ExtensionConfig.GenerateAllOptionSetLabelMetadata;
            set => Config.ExtensionConfig.GenerateAllOptionSetLabelMetadata = value;
        }

        [Category("3 - Option Sets")]
        [DisplayName("Generate Global Option Sets")]
        [Description("Generate all Global OptionSets, note: if an entity contains a reference to a global optionset, it will be emitted even if this switch is not present.")]
        public bool GenerateGlobalOptionSets
        {
            get => Config.ExtensionConfig.GenerateGlobalOptionSets;
            set => Config.ExtensionConfig.GenerateGlobalOptionSets = value;
        }

        [Category("3 - Option Sets")]
        [DisplayName("Generate Option Set Metadata Attribute Class")]
        [Description("Generates an OptionSetMetadataAttribute class used to allow for storing of the metadata of OptionSetValues i.e. display order, name, description, etc.  Only used if Add Option Set Metadata Attribute is true.")]
        public bool GenerateOptionSetMetadataAttribute
        {
            get => Config.ExtensionConfig.GenerateOptionSetMetadataAttribute;
            set => Config.ExtensionConfig.GenerateOptionSetMetadataAttribute = value;
        }

        [Category("3 - Option Sets")]
        [DisplayName("Group Local Option Sets By Entity")]
        [Description("Combines all local option sets into a single file per entity.  Only used if Create One File Per Option Set is true.")]
        public bool GroupLocalOptionSetsByEntity
        {
            get => Config.ExtensionConfig.GroupLocalOptionSetsByEntity;
            set => Config.ExtensionConfig.GroupLocalOptionSetsByEntity = value;
        }

        [Category("3 - Option Sets")]
        [DisplayName("Invalid C# Name Prefix")]
        [Description("Specifies the Prefix to be used for OptionSets that would normally start with an invalid first character ie \"1st\"")]
        public string InvalidCSharpNamePrefix
        {
            get => Config.ExtensionConfig.InvalidCSharpNamePrefix;
            set => Config.ExtensionConfig.InvalidCSharpNamePrefix = value;
        }

        [Category("3 - Option Sets")]
        [DisplayName("Local Option Set Format")]
        [Description("Defines the format of Local Option Sets where {0} is the Entity Schema Name, and {1} is the Attribute Schema Name.   The format Specified in the SDK is {0}{1}, but the default is {0}_{1}, but used to be prefix_{0}_{1}(all lower case).")]
        public string LocalOptionSetFormat
        {
            get => Config.ExtensionConfig.LocalOptionSetFormat;
            set => Config.ExtensionConfig.LocalOptionSetFormat = value;
        }

        [Category("3 - Option Sets")]
        [DisplayName("Option Set Output Relative Path")]
        [Description("This is realtive to the Path of the Output Relative Directory (or if not populated, the Settings File).  If \"Create One File Per Option Set\" is enabled, this needs to be a file path that ends in \".cs\", else, this needs to be a path to a directory.")]
        [Editor(typeof(PathEditor), typeof(UITypeEditor))]
        [DynamicRelativePathEditor(nameof(SettingsPath), nameof(CreateOneFilePerOptionSet), "C# files|*.cs", "cs", false)]
        public string OptionSetsTypesFolder
        {
            get => Config.OptionSetsTypesFolder;
            set => Config.OptionSetsTypesFolder = GetRelativePathToFileOrDirectory(value, CreateOneFilePerOptionSet, "OptionSets.cs");
        }

        [Category("3 - Option Sets")]
        [DisplayName("Option Set Value Language Code Override")]
        [Description("Overrides the default (English:1033) language code used for generating Option Set Value names (the value, not the option set).")]
        public int? OptionSetLanguageCodeOverride
        {
            get => Config.ExtensionConfig.OptionSetLanguageCodeOverride;
            set => Config.ExtensionConfig.OptionSetLanguageCodeOverride = value;
        }

        [Category("3 - Option Sets")]
        [DisplayName("Transliteration Relative Path")]
        [Description("The path relative, to the XrmToolBox Plugins directory, to a folder containing the language code json files to be used for transliteration.")]
        public string TransliterationRelativePath
        {
            get => Config.ExtensionConfig.TransliterationRelativePath;
            set => Config.ExtensionConfig.TransliterationRelativePath = value;
        }

        [Category("3 - Option Sets")]
        [DisplayName("Use CrmSvcUtil State Enum Naming Convention")]
        [Description("The CrmSvcUtil names entity statecode enums as \"{EntityName}State\", but the PAC ModelBuilder names them the same way all other enums are generated \"{EntityName}_StateCode\".  This allows for maintaining backwards compability.")]
        public bool UseCrmSvcUtilStateEnumNamingConvention
        {
            get => Config.ExtensionConfig.UseCrmSvcUtilStateEnumNamingConvention;
            set => Config.ExtensionConfig.UseCrmSvcUtilStateEnumNamingConvention = value;
        }

        [Category("3 - Option Sets")]
        [DisplayName("Option Set Names")]
        [Description("If Option Sets have idential names to Entities, it will cause naming conflicts.  By default this is overcome by postfixing \"_Enum\" to the Option Set name.  This setting allows a custom mapping for option set names to be specified.")]
        [Editor(typeof(DictionaryEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(CollectionCountConverter))]
        public Dictionary<string, string> OptionSetNames { get; set; }

        #endregion Option Sets

        #region Service Generation Extensions

        [Category("6 - Service Generation Extensions")]
        [DisplayName("Code Customization Service")]
        [Description("Called after the CodeDOM generation has been completed, assuming the default instance of ICodeGenerationService. It is useful for generating additional classes, such as the constants in picklists.  This really shouldn't be changed unless there is something custom that is required and is not, and will not, be added to the Early Bound Generator.")]
        public string CodeCustomizationService
        {
            get => Config.CodeCustomizationService;
            set => Config.CodeCustomizationService = value;
        }

        [Category("6 - Service Generation Extensions")]
        [DisplayName("Code Generation Service")]
        [Description("Called after MetadataProviderService returns the metadata.  Controls calling all other services.  This really shouldn't be changed unless there is something custom that is required and is not, and will not, be added to the Early Bound Generator.")]
        public string CodeGenerationService
        {
            get => Config.CodeGenerationService;
            set => Config.CodeGenerationService = value;
        }

        [Category("6 - Service Generation Extensions")]
        [DisplayName("Code Writer Filter Service")]
        [Description("Called during the CodeDOM generation to determine if Option Sets, Options, Entities, Attributes Relationships, or Service Contexts are generated.  This really shouldn't be changed unless there is something custom that is required and is not, and will not, be added to the Early Bound Generator.")]
        public string CodeWriterFilterService
        {
            get => Config.CodeWriterFilterService;
            set => Config.CodeWriterFilterService = value;
        }

        [Category("6 - Service Generation Extensions")]
        [DisplayName("Code Writer Message Filter Service")]
        [Description("Called during the CodeDOM generation to determine if the Message is generated.  This really shouldn't be changed unless there is something custom that is required and is not, and will not, be added to the Early Bound Generator.")]
        public string CodeWriterMessageFilterService
        {
            get => Config.CodeWriterMessageFilterService;
            set => Config.CodeWriterMessageFilterService = value;
        }

        [Category("6 - Service Generation Extensions")]
        [DisplayName("Metadata Provider Service")]
        [Description("Used to retrieve the metadata from the server.  Using the 7 - Debug settings, this can be used cache the metadata for testing purposes.  This really shouldn't be changed unless there is something custom that is required and is not, and will not, be added to the Early Bound Generator.")]
        public string MetadataProviderService
        {
            get => Config.MetadataProviderService;
            set => Config.MetadataProviderService = value;
        }

        [Category("6 - Service Generation Extensions")]
        [DisplayName("Metadata Query Provider Service")]
        [Description("Used to determine the query to retrieve the metadata from the server.  This really shouldn't be changed unless there is something custom that is required and is not, and will not, be added to the Early Bound Generator.")]
        public string MetadataQueryProviderService
        {
            get => Config.MetadataQueryProviderService;
            set => Config.MetadataQueryProviderService = value;
        }

        [Category("6 - Service Generation Extensions")]
        [DisplayName("Naming Service")]
        [Description("Called during the CodeDOM generation to determine the name for objects.  This really shouldn't be changed unless there is something custom that is required and is not, and will not, be added to the Early Bound Generator.")]
        public string NamingService
        {
            get => Config.NamingService;
            set => Config.NamingService = value;
        }

        #endregion Service Generation Extensions

        [Browsable(false)]
        public string SettingsPath { get; set; }

        /// <summary>
        /// True if at least one Action/Entity/OptionSet will create one file per item, else False.
        /// </summary>
        [Browsable(false)]
        private bool AtLeastOneCreateFilePerSelected =>
            CreateOneFilePerMessage
            || CreateOneFilePerEntity
            || CreateOneFilePerOptionSet;

        private EarlyBoundGeneratorPlugin Plugin { get; }

        private EarlyBoundGeneratorConfig Config { get; }

        #endregion Properties

        public SettingsMap(EarlyBoundGeneratorPlugin plugin, EarlyBoundGeneratorConfig config)
        {
            Plugin = plugin;
            Config = config;

            var propertyToParse = nameof(MessageWildcardWhitelist);
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
                CamelCaseCustomWords = RemoveWhiteSpace(nameof(CamelCaseCustomWords), config.ExtensionConfig.CamelCaseCustomWords?.ToLower()).GetList<string>();
                EntitiesBlacklist = RemoveWhiteSpace(nameof(EntitiesBlacklist), config.ExtensionConfig.EntitiesToSkip).GetHashSet<string>();
                EntitiesWhitelist = RemoveWhiteSpace(nameof(EntitiesWhitelist), config.ExtensionConfig.EntitiesWhitelist).GetHashSet<string>();
                EntityAttributeSpecifiedNames = RemoveWhiteSpace(nameof(EntityAttributeSpecifiedNames), config.ExtensionConfig.EntityAttributeSpecifiedNames).GetDictionaryHash<string, string>();
                EntityClassNameOverrides = RemoveWhiteSpace(nameof(EntityClassNameOverrides), config.ExtensionConfig.EntityClassNameOverrides).GetDictionary<string, string>();
                EntityRegExBlacklist = RemoveWhiteSpace(nameof(EntityRegExBlacklist), config.ExtensionConfig.EntityPrefixesToSkip).GetList<string>();
                EntityWildcardWhitelist = RemoveWhiteSpace(nameof(EntityWildcardWhitelist), config.ExtensionConfig.EntityPrefixesWhitelist).GetList<string>();
                MessageBlacklist = RemoveWhiteSpace(nameof(MessageBlacklist), config.ExtensionConfig.ActionsToSkip).GetHashSet<string>(info);
                MessageWhitelist = RemoveWhiteSpace(nameof(MessageWhitelist), config.ExtensionConfig.ActionsWhitelist).GetHashSet<string>();
                MessageWildcardWhitelist = RemoveWhiteSpace(nameof(MessageWildcardWhitelist), config.ExtensionConfig.ActionPrefixesWhitelist).GetList<string>();
                PropertyEnumMappings = RemoveWhiteSpace(nameof(PropertyEnumMappings), config.ExtensionConfig.PropertyEnumMappings).GetList<string>();
                OptionSetNames = RemoveWhiteSpace(nameof(OptionSetNames), Config.ExtensionConfig.OptionSetNames).GetDictionary<string,string>();
                TokenCapitalizationOverrides = RemoveWhiteSpace(nameof(TokenCapitalizationOverrides), config.ExtensionConfig.TokenCapitalizationOverrides).GetList<string>();
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
            Config.ExtensionConfig.ActionPrefixesWhitelist = CommonConfig.ToStringSorted(MessageWildcardWhitelist, info);
            Config.ExtensionConfig.ActionsWhitelist = CommonConfig.ToStringSorted(MessageWhitelist, info);
            Config.ExtensionConfig.ActionsToSkip = CommonConfig.ToStringSorted(MessageBlacklist, info);
            Config.ExtensionConfig.CamelCaseCustomWords = CommonConfig.ToStringSorted(CamelCaseCustomWords);
            Config.ExtensionConfig.EntitiesToSkip = CommonConfig.ToStringSorted(EntitiesBlacklist);
            Config.ExtensionConfig.EntitiesWhitelist = CommonConfig.ToStringSorted(EntitiesWhitelist);
            Config.ExtensionConfig.EntityAttributeSpecifiedNames = CommonConfig.ToStringSorted(EntityAttributeSpecifiedNames);
            Config.ExtensionConfig.EntityClassNameOverrides = CommonConfig.ToStringSorted(EntityClassNameOverrides);
            Config.ExtensionConfig.EntityPrefixesToSkip = CommonConfig.ToStringSorted(EntityRegExBlacklist);
            Config.ExtensionConfig.EntityPrefixesWhitelist = CommonConfig.ToStringSorted(EntityWildcardWhitelist);
            Config.ExtensionConfig.PropertyEnumMappings = CommonConfig.ToStringSorted(PropertyEnumMappings);
            Config.ExtensionConfig.OptionSetNames = CommonConfig.ToStringSorted(OptionSetNames);
            Config.ExtensionConfig.TokenCapitalizationOverrides = CommonConfig.ToStringSorted(TokenCapitalizationOverrides);
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
                    value = System.IO.Path.GetDirectoryName(value);
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
