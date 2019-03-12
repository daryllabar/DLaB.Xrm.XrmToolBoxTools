using System.Collections.Generic;
using System.ComponentModel;
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
        [DynamicRelativePathEditor(nameof(SettingsPath), "C# files|*.cs", "cs", false)]
        public string ActionOutPath
        {
            get => Config.ActionOutPath;
            set => Config.ActionOutPath = GetRelativePathToFileOrDirectory(value, CreateOneFilePerAction, "Actions.cs");
        }

        [Category("Actions")]
        [DisplayName("Actions Blacklist")]
        [Description("Allows for the ability to specify Actions to not generate.")]
        [Editor(typeof(ActionsHashEditor), typeof(UITypeEditor))]
        [TypeConverter(CollectionCountConverter.Name)]
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

        #endregion Actions

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
        [DynamicRelativePathEditor(nameof(SettingsPath), "C# files|*.cs", "cs", false)]
        public string EntityOutPath
        {
            get => Config.EntityOutPath;
            set => Config.EntityOutPath = GetRelativePathToFileOrDirectory(value, CreateOneFilePerEntity, "Entities.cs");
        }

        [Category("Entities")]
        [DisplayName("Entities Blacklist")]
        [Description("Contains Entities to not generate.")]
        [Editor(typeof(EntitiesHashEditor), typeof(UITypeEditor))]
        [TypeConverter(CollectionCountConverter.Name)]
        public HashSet<string> EntitiesToSkip { get; set; }

        [Category("Entities")]
        [DisplayName("Entities Whitelist")]
        [Description("Contains only the Entities to generate.  If empty, all Entities will be included.")]
        [Editor(typeof(EntitiesHashEditor), typeof(UITypeEditor))]
        [TypeConverter(CollectionCountConverter.Name)]
        [CollectionCount("All Entities")]
        public HashSet<string> EntitiesWhitelist { get; set; }

        [Category("Entities")]
        [DisplayName("Attribute Capitalization Override")]
        [Description("Allows for the ability to specify the capitalization of an attribute on an entity.")]
        [Editor(typeof(SpecifyAttributesCaseEditor), typeof(UITypeEditor))]
        [TypeConverter(CollectionCountConverter.Name)]
        public Dictionary<string, HashSet<string>> EntityAttributeSpecifiedNames { get; set; }

        [Category("Entities")]
        [DisplayName("Entities Prefix Blacklist")]
        [Description("Contains list of prefixes to not generate.  If the Entity starts with the given prefix, it will not be generated.")]
        [Editor(StringEditorName, typeof(UITypeEditor))]
        [TypeConverter(CollectionCountConverter.Name)]
        public List<string> EntityPrefixesToSkip { get; set; }

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
        [DisplayName("Generate Entity Relationships")]
        [Description("Specifies if 1:N, N:1, and N:N relationships properties are generated for entities.")]
        public bool GenerateEntityRelationships
        {
            get => Config.ExtensionConfig.GenerateEntityRelationships;
            set => Config.ExtensionConfig.GenerateEntityRelationships = value;
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
        [TypeConverter(CollectionCountConverter.Name)]
        public List<string> PropertyEnumMappings { get; set; }

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
        [TypeConverter(CollectionCountConverter.Name)]
        public Dictionary<string, HashSet<string>> UnmappedProperties { get; set; }

        [Category("Entities")]
        [DisplayName("Use Xrm Client")]
        [Description("(Not Recommended) Specifies the Service Context should inherit from CrmOrganizationServiceContext, and conversly, Entities from Xrm.Client.Entity.\r\nThis results in a dependence on Microsoft.Xrm.Client.dll that must be accounted for during plugins and workflows since it has been deprecated and isn't included with CRM by default:\r\nhttps://community.dynamics.com/crm/b/develop1/archive/2013/08/12/microsoft-xrm-client-part-1-crmorganizationservicecontext-and-when-should-i-use-it.")]
        public bool UseXrmClient
        {
            get => Config.ExtensionConfig.UseXrmClient;
            set => Config.ExtensionConfig.UseXrmClient = value;
        }

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

        [Category("Global")]
        [DisplayName("CrmSvcUtil Relative Path")]
        [Description("The Path to the CrmSvcUtil.exe, relative to the CrmSvcUtil Realtive Root Path.  Defaults to using the CrmSvcUtil that comes by default.")]
        public string CrmSvcUtilRelativePath
        {
            get => Config.CrmSvcUtilRelativePath;
            set => Config.CrmSvcUtilRelativePath = value;
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
        [DisplayName("Remove Runtime Version Comment")]
        [Description(@"Removes the ""//   Runtime Version:X.X.X.X"" comment from the header of generated files.
This helps to alleviate unnecessary differences that pop up when the classes are generated from machines with different .Net Framework updates installed.")]
        public bool RemoveRuntimeVersionComment
        {
            get => Config.ExtensionConfig.RemoveRuntimeVersionComment;
            set => Config.ExtensionConfig.RemoveRuntimeVersionComment = value;
        }

        [Category("Global")]
        [DisplayName("Use Tfs")]
        [Description("Will use TFS to attempt to check out the early bound classes.  Not needed for Git based repositories.")]
        public bool UseTfsToCheckoutFiles
        {
            get => Config.ExtensionConfig.UseTfsToCheckoutFiles;
            set => Config.ExtensionConfig.UseTfsToCheckoutFiles = value;
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
        [DynamicRelativePathEditor(nameof(SettingsPath), "C# files|*.cs", "cs", false)]
        public string OptionSetOutPath
        {
            get => Config.OptionSetOutPath;
            set => Config.OptionSetOutPath = GetRelativePathToFileOrDirectory(value, CreateOneFilePerOptionSet, "OptionSets.cs");
        }

        [Category("Option Sets")]
        [DisplayName("Option Set Prefix Blacklist")]
        [Description("Contains list of prefixes to not generate.  If the Option Set starts with the given prefix, it will not be generated.")]
        [Editor(StringEditorName, typeof(UITypeEditor))]
        [TypeConverter(CollectionCountConverter.Name)]
        public List<string> OptionSetPrefixesToSkip { get; set; }

        [Category("Option Sets")]
        [DisplayName("Option Sets Blacklist")]
        [Description("Allows for the ability to specify OptionSets to not generate.")]
        [Editor(typeof(OptionSetsHashEditor), typeof(UITypeEditor))]
        [TypeConverter(CollectionCountConverter.Name)]
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

        private EarlyBoundGeneratorPlugin Plugin { get; }

        private EarlyBoundGeneratorConfig Config { get; }

        #endregion Properties

        public SettingsMap(EarlyBoundGeneratorPlugin plugin, EarlyBoundGeneratorConfig config)
        {
            Plugin = plugin;
            Config = config;

            string RemoveWhiteSpace(string value)
            {
                return value?.Replace(" ", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty);
            }

            var info = new ConfigKeyValueSplitInfo { ConvertKeysToLower = false };
            ActionsToSkip = RemoveWhiteSpace(config.ExtensionConfig.ActionsToSkip).GetHashSet<string>(info);
            EntitiesToSkip = RemoveWhiteSpace(config.ExtensionConfig.EntitiesToSkip).GetHashSet<string>();
            EntitiesWhitelist = RemoveWhiteSpace(config.ExtensionConfig.EntitiesWhitelist).GetHashSet<string>();
            EntityAttributeSpecifiedNames = RemoveWhiteSpace(config.ExtensionConfig.EntityAttributeSpecifiedNames).GetDictionaryHash<string, string>();
            EntityPrefixesToSkip = RemoveWhiteSpace(config.ExtensionConfig.EntityPrefixesToSkip).GetList<string>();
            PropertyEnumMappings = RemoveWhiteSpace(config.ExtensionConfig.PropertyEnumMappings).GetList<string>();
            OptionSetPrefixesToSkip = RemoveWhiteSpace(config.ExtensionConfig.OptionSetPrefixesToSkip).GetList<string>();
            OptionSetsToSkip = RemoveWhiteSpace(config.ExtensionConfig.OptionSetsToSkip).GetHashSet<string>();
            UnmappedProperties = RemoveWhiteSpace(Config.ExtensionConfig.UnmappedProperties).GetDictionaryHash<string, string>();

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
            Config.ExtensionConfig.ActionsToSkip = CommonConfig.ToString(ActionsToSkip, info);
            Config.ExtensionConfig.EntitiesToSkip = CommonConfig.ToString(EntitiesToSkip);
            Config.ExtensionConfig.EntitiesWhitelist = CommonConfig.ToString(EntitiesWhitelist);
            Config.ExtensionConfig.EntityAttributeSpecifiedNames = CommonConfig.ToString(EntityAttributeSpecifiedNames);
            Config.ExtensionConfig.EntityPrefixesToSkip = CommonConfig.ToString(EntityPrefixesToSkip);
            Config.ExtensionConfig.PropertyEnumMappings = CommonConfig.ToString(PropertyEnumMappings);
            Config.ExtensionConfig.OptionSetPrefixesToSkip = CommonConfig.ToString(OptionSetPrefixesToSkip);
            Config.ExtensionConfig.OptionSetsToSkip = CommonConfig.ToString(OptionSetsToSkip);
            Config.ExtensionConfig.UnmappedProperties = CommonConfig.ToString(UnmappedProperties);
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