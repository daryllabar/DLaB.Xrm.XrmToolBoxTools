using DLaB.Common;
using DLaB.Common.VersionControl;
using DLaB.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

namespace DLaB.EarlyBoundGeneratorV2.Settings
{
    /// <summary>
    /// POCO for EBG Settings
    /// </summary>
    [Serializable]
    [XmlType("Config")]
    [XmlRoot("Config")]
    public class EarlyBoundGeneratorConfig
    {
        private const string EarlyBoundGeneratorV2Name= "DLaB.EarlyBoundGeneratorV2";

        #region Properties

        /// <summary>
        /// Use speech synthesizer to notify of code generation completion.
        /// </summary>
        public bool AudibleCompletionNotification { get; set; }

        /// <summary>
        /// Called after the CodeDOM generation has been completed, assuming the default instance of ICodeGenerationService. It is useful for generating additional classes, such as the constants in picklists.  This really shouldn't be changed unless there is something custom that is required and is not, and will not, be added to the Early Bound Generator. 
        /// </summary>
        public string CodeCustomizationService { get; set; }

        /// <summary>
        /// Called after MetadataProviderService returns the metadata.  Controls calling all other services.  This really shouldn't be changed unless there is something custom that is required and is not, and will not, be added to the Early Bound Generator. 
        /// </summary>
        public string CodeGenerationService { get; set; }

        /// <summary>
        /// Called during the CodeDOM generation to determine if Option Sets, Options, Entities, Attributes Relationships, or Service Contexts are generated.  This really shouldn't be changed unless there is something custom that is required and is not, and will not, be added to the Early Bound Generator.
        /// </summary>
        public string CodeWriterFilterService { get; set; }

        /// <summary>
        /// Called during the CodeDOM generation to determine if the Message is generated.  This really shouldn't be changed unless there is something custom that is required and is not, and will not, be added to the Early Bound Generator.
        /// </summary>
        public string CodeWriterMessageFilterService { get; set; }

        /// <summary>
        /// When set, includes the entity ETC ( entity type code ) in the generated code.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public bool EmitEntityETC { get; set; }
        
        /// <summary>
        /// When set, includes the Virtual Attributes of entities in the generated code.
        /// </summary>
        public bool EmitVirtualAttributes { get; set; }

        /// <summary>
        /// Entity output path
        /// </summary>
        public string EntityTypesFolder { get; set; }

        /// <summary>
        /// Settings that will get written to the builderSettings.json
        /// </summary>
        /// <value>
        /// The extension configuration.
        /// </value>
        public ExtensionConfig ExtensionConfig { get; set; }

        /// <summary>
        /// Generates classes for messages (Actions/Custom APIs/Microsoft Messages) as part of code generation.
        /// </summary>
        public bool GenerateMessages { get; set; }

        /// <summary>
        /// Specifies whether to include in the early bound class, the command line used to generate it.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [include command line]; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeCommandLine { get; set; }

        /// <summary>
        /// Folder name that will contain messages
        /// </summary>
        public string MessageTypesFolder { get; set; }

        /// <summary>
        /// Used to retrieve the metadata from the server.  Using the 7 - Debug settings, this can be used cache the metadata for testing purposes.  This really shouldn't be changed unless there is something custom that is required and is not, and will not, be added to the Early Bound Generator."
        /// </summary>
        public string MetadataProviderService { get; set; }

        /// <summary>
        /// Used to determine the query to retrieve the metadata from the server.  This really shouldn't be changed unless there is something custom that is required and is not, and will not, be added to the Early Bound Generator.
        /// </summary>
        public string MetadataQueryProviderService { get; set; }

        /// <summary>
        /// Called during the CodeDOM generation to determine the name for objects.  This really shouldn't be changed unless there is something custom that is required and is not, and will not, be added to the Early Bound Generator. 
        /// </summary>
        public string NamingService { get; set; }

        /// <summary>
        /// Option set output path
        /// </summary>
        public string OptionSetsTypesFolder { get; set; }

        /// <summary>
        /// Namespace
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Name of the Service Context
        /// </summary>
        public string ServiceContextName { get; set; }

        /// <summary>
        /// Suppress all generated objects being tagged with the code generation engine and version
        /// </summary>
        public bool SuppressGeneratedCodeAttribute { get; set; }

        /// <summary>
        /// Gets or sets the last ran version.
        /// </summary>
        /// <value>
        /// The last ran version.
        /// </value>
        public string SettingsVersion { get; set; }

        /// <summary>
        /// If this is set to false, then all setting changes made in the Early Bound Generator will not take affect outside of out directory since the builderSettings.json file isn't getting updated, but is helpful if custom editing of the builderSettings.json file is required.
        /// </summary>
        public bool UpdateBuilderSettingsJson { get; set; }

        /// <summary>
        /// The version of the EarlyBoundGeneratorPlugin
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public string Version { get; set; }

        #region NonSerialized Properties

        /// <summary>
        /// Set during Execution
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public bool UseCrmOnline { get; set; }
        /// <summary>
        /// Set during Execution
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string RootPath { get; set; }
        /// <summary>
        /// Set during Execution
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public bool SupportsActions { get; set; }

        /// <summary>
        /// Path of the Model Builder Template file, relative to the root path, if not fully rooted
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string SettingsTemplatePath =>
            Path.IsPathRooted(ExtensionConfig.BuilderSettingsJsonRelativePath)
                ? ExtensionConfig.BuilderSettingsJsonRelativePath
                : Path.Combine(RootPath, ExtensionConfig.BuilderSettingsJsonRelativePath);

        #endregion // NonSerialized Properties

        #endregion // Properties       

        private static readonly string[] PreV2TokenCapitalizations = {
            "AccessTeam",
            "ActiveState",
            "BusinessAs",
            "CardUci",
            "DefaultOnCase",
            "EmailAnd",
            "FeatureSet",
            "IsMsTeams",
            "IsPaiEnabled",
            "IsSopIntegration",
            "MsDyUsd",
            "O365Admin",
            "OnHold",
            "OwnerOnAssign",
            "ParticipatesIn",
            "PartiesOnEmail",
            "PauseStates",
            "SentOn",
            "SlaId",
            "SlaKpi",
            "SyncOptIn",
            "Timeout",
            "UserPuid",
            "VoiceMail",
            "Weblink"
        };

        private EarlyBoundGeneratorConfig()
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        #region Add Missing Default settings

        /// <summary>
        /// Initializes a new instance of the <see cref="EarlyBoundGeneratorConfig"/> class.
        /// </summary>
        private EarlyBoundGeneratorConfig(POCO.Config poco)
        {
            var @default = GetDefault();
            RemoveObsoleteValues(poco);
            UpdateObsoleteSettings(poco, poco.ExtensionConfig, @default.ExtensionConfig);

            AudibleCompletionNotification  =  poco.AudibleCompletionNotification  ?? @default.AudibleCompletionNotification;
            CodeCustomizationService       =  poco.CodeCustomizationService       ?? @default.CodeCustomizationService;
            CodeGenerationService          =  poco.CodeGenerationService          ?? @default.CodeGenerationService;
            CodeWriterFilterService        =  poco.CodeWriterFilterService        ?? @default.CodeWriterFilterService;
            CodeWriterMessageFilterService =  poco.CodeWriterMessageFilterService ?? @default.CodeWriterMessageFilterService;
            EmitEntityETC                  =  poco.EmitEntityETC                  ?? @default.EmitEntityETC;
            EmitVirtualAttributes          =  poco.EmitVirtualAttributes          ?? @default.EmitVirtualAttributes;
            EntityTypesFolder              =  poco.EntityTypesFolder              ?? @default.EntityTypesFolder;
            IncludeCommandLine             =  poco.IncludeCommandLine             ?? @default.IncludeCommandLine;
            GenerateMessages               =  poco.GenerateMessages               ?? @default.GenerateMessages;
            MetadataProviderService        =  poco.MetadataProviderService        ?? @default.MetadataProviderService;
            MetadataQueryProviderService   =  poco.MetadataQueryProviderService   ?? @default.MetadataQueryProviderService;
            Namespace                      =  poco.Namespace                      ?? @default.Namespace;
            NamingService                  =  poco.NamingService                  ?? @default.NamingService;
            MessageTypesFolder             =  poco.MessageTypesFolder             ?? @default.MessageTypesFolder;
            OptionSetsTypesFolder          =  poco.OptionSetsTypesFolder          ?? @default.OptionSetsTypesFolder;
            ServiceContextName             =  poco.ServiceContextName             ?? @default.ServiceContextName;
            SuppressGeneratedCodeAttribute =  poco.SuppressGeneratedCodeAttribute ?? @default.SuppressGeneratedCodeAttribute;
            UpdateBuilderSettingsJson      =  poco.UpdateBuilderSettingsJson      ?? @default.UpdateBuilderSettingsJson;

            ExtensionConfig = @default.ExtensionConfig;
            ExtensionConfig.SetPopulatedValues(poco.ExtensionConfig);

            SettingsVersion = string.IsNullOrWhiteSpace(poco.Version) ? "0.0.0.0" : poco.Version;
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void UpdateObsoleteSettings(POCO.Config poco, POCO.ExtensionConfig pocoConfig, ExtensionConfig defaultConfig)
        {
            var pocoVersion = new Version(poco.Version);
            if (pocoVersion < new Version("1.2016.6.1"))
            {
                Logger.AddDetail("Updating config to 1.2016.6.1 settings.");
                // Storing of UnmappedProperties and EntityAttributeSpecified Names switched from Key,Value1,Value2|Key,Value1,Value2 to Key:Value1,Value2|Key:Value1,Value2
                // Also convert from a List to a HashSet
                pocoConfig.EntityAttributeSpecifiedNames = ConvertNonColonDelimitedDictionaryListToDictionaryHash(pocoConfig.EntityAttributeSpecifiedNames);
                // Unmapped Properties were removed, don't need to worry about upgrade path
                //pocoConfig.UnmappedProperties = ConvertNonColonDelimitedDictionaryListToDictionaryHash(pocoConfig.UnmappedProperties);
            }

            if (pocoVersion < new Version("1.2020.3.23"))
            {
                Logger.AddDetail("Updating config to 1.2020.3.23 settings.");
                // Added new option to overwrite Option Set properties.  This is the desired default now, but don't break old generation settings.
                if (poco.ExtensionConfig.ReplaceOptionSetPropertiesWithEnum == null)
                {
                    poco.ExtensionConfig.ReplaceOptionSetPropertiesWithEnum = false;
                }
            }

            if (pocoVersion < new Version("1.2020.10.1"))
            {
                Logger.AddDetail("Updating config to 1.2020.10.1 settings.");
                // Issue #254 add invalid actions to blacklist.
                var invalidBlacklistItems = "RetrieveAppSettingList|RetrieveAppSetting|SaveAppSetting|msdyn_GetSIFeatureConfiguration".ToLower();
                if (string.IsNullOrWhiteSpace(poco.ExtensionConfig.ActionsToSkip))
                {
                    poco.ExtensionConfig.ActionsToSkip = invalidBlacklistItems;
                }
                else 
                {
                    poco.ExtensionConfig.ActionsToSkip += "|" + invalidBlacklistItems;
                }
            }

            if (pocoVersion < new Version("1.2020.12.18"))
            {
                Logger.AddDetail("Updating config to 1.2020.12.18 settings.");
                // 12.18.2020 introduced Valueless parameters, but GenerateActions existed before as a null, need a boolean value to determine if it should be included
                var generateActions = poco.UserArguments.FirstOrDefault(a => a.Name == "generateActions" && a.Value == null);
                if (generateActions != null)
                {
                    generateActions.Value = "true";
                    generateActions.Valueless = true;
                }
            }

            if (pocoVersion < new Version("2.2023.4.3"))
            {
                if (pocoConfig.AddNewFilesToProject != true)
                {
                    Logger.Show("You are upgrading from a CrmSvcUtil based Early Bound Generator to PAC ModelBuilder one, but don't have \"1 - Global - Add New Files To Project\" enabled.  It is suggested that this is enabled before generating the new types to simplify the transition!");
                }
                
                Logger.AddDetail("Updating config to 2.2023.4.3 settings.");
                // 3.2.2023 Switch to Model Builder.  Pull User Arguments into typed settings
                ObsoleteUserArgument(poco, a => a.Name == "generateActions", (p, arg) => poco.GenerateMessages = bool.Parse(arg.Value));
                ObsoleteUserArgument(poco, a => a.Name == "namespace", (p, arg) => poco.Namespace = arg.Value);
                ObsoleteUserArgument(poco, a => a.Name == "out" && a.SettingType == CreationType.Actions, (p, arg) => poco.MessageTypesFolder = arg.Value);
                ObsoleteUserArgument(poco, a => a.Name == "out" && a.SettingType == CreationType.Entities, (p, arg) => poco.EntityTypesFolder = arg.Value);
                ObsoleteUserArgument(poco, a => a.Name == "out" && a.SettingType == CreationType.OptionSets, (p, arg) => poco.OptionSetsTypesFolder = arg.Value);
                ObsoleteUserArgument(poco, a => a.Name == "servicecontextname", (p, arg) => poco.ServiceContextName = arg.Value);
                ObsoleteUserArgument(poco, a => a.Name == "SuppressGeneratedCodeAttribute", (p, arg) => poco.SuppressGeneratedCodeAttribute = bool.Parse(arg.Value));

                if (!string.IsNullOrWhiteSpace(pocoConfig.CamelCaseNamesDictionaryRelativePath) && pocoConfig.CamelCaseNamesDictionaryRelativePath.Contains(@"DLaB.EarlyBoundGenerator\"))
                {
                    pocoConfig.CamelCaseNamesDictionaryRelativePath = pocoConfig.CamelCaseNamesDictionaryRelativePath.Replace(@"DLaB.EarlyBoundGenerator\", $@"{EarlyBoundGeneratorV2Name}\");
                }
                pocoConfig.CleanupCrmSvcUtilLocalOptionSets = true;
                pocoConfig.GenerateINotifyPattern = true;
                if (pocoConfig.GenerateOnlyReferencedOptionSets == true)
                {
                    pocoConfig.GenerateGlobalOptionSets = false;
                }

                if (pocoConfig.CreateOneFilePerAction == false)
                {
                    pocoConfig.GroupMessageRequestWithResponse = false;
                }
                pocoConfig.UseCrmSvcUtilStateEnumNamingConvention = true;

                if (!string.IsNullOrWhiteSpace(pocoConfig.EntityPrefixesToSkip))
                {
                    pocoConfig.EntityPrefixesToSkip = InjectMissingWildcards(pocoConfig.EntityPrefixesToSkip);
                }

                AddNewTokens(pocoConfig, defaultConfig, PreV2TokenCapitalizations);

                Logger.AddDetail("Finished Updating Config to 2.2023.4.3 settings!");
                Logger.AddDetail("Check out the update documentation!");
                Logger.AddDetail("https://github.com/daryllabar/DLaB.Xrm.XrmToolBoxTools/wiki/EBG-%E2%80%90-Version-2.2023.4.3-Upgrade-To-PAC-ModelBuilder");
            }

            if (pocoVersion < new Version("2.2023.9.20"))
            {
#pragma warning disable CS0618 // Type or member is obsolete
                Logger.AddDetail("Updating config to 2.2023.9.21 settings.");
                // Entity Type Codes are not handled by the Model Builder
                if (pocoConfig.GenerateEntityTypeCode == true)
                {
                    poco.EmitEntityETC = true;
                }
#pragma warning restore CS0618 // Type or member is obsolete
            }
            if (pocoVersion < new Version("2.2023.12.21"))
            {
                Logger.AddDetail("Updating config to 2.2023.12.21 settings.");
                Logger.AddDetail(" - Setting AdjustCasingForEnumOptions to false.  Consider updating to true to increase readability of enum values.");
                pocoConfig.AdjustCasingForEnumOptions = false;
            }
        }

        private static string InjectMissingWildcards(string value)
        {
            return string.Join("|", value.Split('|').Select(v => v.Contains("*") ? v.Replace("*", ".*") : v + ".*").ToArray());
        }

        private static void AddNewTokens(POCO.ExtensionConfig pocoConfig, ExtensionConfig defaultConfig, string[] previousTokens)
        {
            var previousExisting = new HashSet<string>(pocoConfig.TokenCapitalizationOverrides.ToLower().Split('|'));
            var existing = new HashSet<string>(pocoConfig.TokenCapitalizationOverrides.Split('|').Select(v => v.Trim()));
            var previousOob = new HashSet<string>(previousTokens.Select(v => v.ToLower()));
            foreach (var value in defaultConfig.TokenCapitalizationOverrides.Split('|'))
            {
                if (previousOob.Contains(value.ToLower()) || previousExisting.Contains(value.ToLower()))
                {
                    continue;
                }
                Logger.AddDetail("Adding " + value + " to TokenCapitalizationOverrides.");
                existing.Add(value);
            }

            pocoConfig.TokenCapitalizationOverrides = string.Join("|" + Environment.NewLine, existing);
        }

        private static void ObsoleteUserArgument(POCO.Config poco, Func<Argument, bool> firstArg, Action<POCO.Config, Argument> updateAction)
        {
            var arg = poco.UserArguments.FirstOrDefault(firstArg);
            if (arg != null)
            {
                if (!string.IsNullOrWhiteSpace(arg.Value))
                {
                    updateAction(poco, arg);
                }
                poco.UserArguments.Remove(arg);
            }
        }

        private static string ConvertNonColonDelimitedDictionaryListToDictionaryHash(string oldValue)
        {
            if (oldValue == null)
            {
                return null;
            }
            var oldValues = Config.GetList<string>(Guid.NewGuid().ToString(), oldValue);
            var newValues = new Dictionary<string, HashSet<string>>();
            foreach (var entry in oldValues)
            {
                var hash = new HashSet<string>();
                var values = entry.Split(',');
                newValues.Add(values.First(), hash);
                foreach (var value in values.Skip(1).Where(v => !hash.Contains(v)))
                {
                    hash.Add(value);
                }
            }
            return Config.ToString(newValues);
        }

        private void RemoveObsoleteValues(POCO.Config poco)
        {
            foreach (var value in poco.ExtensionArguments.Where(a => string.Equals(a.Value, "DLaB.CrmSvcUtilExtensions.Entity.OverridePropertyNames,DLaB.CrmSvcUtilExtensions", StringComparison.InvariantCultureIgnoreCase)).ToList())
            {
                // Pre 2.13.2016, this was the default value.  Replaced with a single naming service that both Entities and OptionSets can use
                poco.ExtensionArguments.Remove(value);
            }

            // Pre 2.13.2016, this was the default value.  Not Needed Anymore
            var old = "OpportunityProduct.OpportunityStateCode,opportunity_statuscode|" +
                      "OpportunityProduct.PricingErrorCode,qooi_pricingerrorcode|" +
                      "ResourceGroup.GroupTypeCode,constraintbasedgroup_grouptypecode";
            if (string.Equals(poco.ExtensionConfig.PropertyEnumMappings, old, StringComparison.InvariantCultureIgnoreCase) || string.Equals(poco.ExtensionConfig.PropertyEnumMappings, old + "|", StringComparison.InvariantCultureIgnoreCase))
            {
                poco.ExtensionConfig.PropertyEnumMappings = string.Empty;
            }
        }

        /// <summary>
        /// Gets the default config
        /// </summary>
        /// <returns></returns>
        public static EarlyBoundGeneratorConfig GetDefault()
        {
            var @default = new EarlyBoundGeneratorConfig
            {
                AudibleCompletionNotification = true,
                CodeCustomizationService = "DLaB.ModelBuilderExtensions.CustomizeCodeDomService,DLaB.ModelBuilderExtensions",
                CodeGenerationService = "DLaB.ModelBuilderExtensions.CodeGenerationService,DLaB.ModelBuilderExtensions",
                CodeWriterFilterService = "DLaB.ModelBuilderExtensions.CodeWriterFilterService,DLaB.ModelBuilderExtensions",
                CodeWriterMessageFilterService = "DLaB.ModelBuilderExtensions.CodeWriterMessageFilterService,DLaB.ModelBuilderExtensions",
                EmitEntityETC = false,
                EmitVirtualAttributes = true,
                EntityTypesFolder = "Entities",
                ExtensionConfig = ExtensionConfig.GetDefault(),
                GenerateMessages = true,
                IncludeCommandLine = false,
                //ExtensionArguments = new List<Argument>(new[] {
                    // Actions
                    //new Argument(CreationType.Actions, CrmSrvUtilService.CodeGenerationService, "DLaB.ModelBuilderExtensions.Action.CustomCodeGenerationService,DLaB.ModelBuilderExtensions"),
                    //new Argument(CreationType.Actions, CrmSrvUtilService.CodeWriterFilter, "DLaB.ModelBuilderExtensions.Action.CodeWriterFilterService,DLaB.ModelBuilderExtensions"),
                    //new Argument(CreationType.Actions, CrmSrvUtilService.MetadataProviderService, "DLaB.ModelBuilderExtensions.BaseMetadataProviderService,DLaB.ModelBuilderExtensions"),
                    //new Argument(CreationType.Entities, CrmSrvUtilService.CodeGenerationService, "DLaB.ModelBuilderExtensions.Entity.CustomCodeGenerationService,DLaB.ModelBuilderExtensions"),
                    //new Argument(CreationType.Entities, CrmSrvUtilService.CodeWriterFilter, "DLaB.ModelBuilderExtensions.Entity.CodeWriterFilterService,DLaB.ModelBuilderExtensions"),
                    //new Argument(CreationType.Entities, CrmSrvUtilService.MetadataProviderService, "DLaB.ModelBuilderExtensions.Entity.MetadataProviderService,DLaB.ModelBuilderExtensions"),
                    //new Argument(CreationType.OptionSets, CrmSrvUtilService.CodeGenerationService, "DLaB.ModelBuilderExtensions.OptionSet.CustomCodeGenerationService,DLaB.ModelBuilderExtensions"),
                    //new Argument(CreationType.OptionSets, CrmSrvUtilService.CodeWriterFilter, "DLaB.ModelBuilderExtensions.OptionSet.CodeWriterFilterService,DLaB.ModelBuilderExtensions"),
                    //new Argument(CreationType.OptionSets, CrmSrvUtilService.MetadataProviderService, "DLaB.ModelBuilderExtensions.BaseMetadataProviderService,DLaB.ModelBuilderExtensions")
                //}),
                MetadataProviderService = "DLaB.ModelBuilderExtensions.MetadataProviderService,DLaB.ModelBuilderExtensions",
                MetadataQueryProviderService = "DLaB.ModelBuilderExtensions.MetadataQueryProviderService,DLaB.ModelBuilderExtensions",
                MessageTypesFolder = "Messages",
                Namespace = "DataverseModel",
                NamingService = "DLaB.ModelBuilderExtensions.NamingService,DLaB.ModelBuilderExtensions",
                OptionSetsTypesFolder = "OptionSets",
                ServiceContextName = "DataverseContext", 
                SuppressGeneratedCodeAttribute = true,
                UpdateBuilderSettingsJson = true
            };
            @default.SettingsVersion = @default.Version;
            return @default;
        }

        #endregion // Add Missing Default settings

        /// <summary>
        /// Loads the Config from the given path.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static EarlyBoundGeneratorConfig Load(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    var config = GetDefault();
                    return config;
                }

                var serializer = new XmlSerializer(typeof(POCO.Config));
                POCO.Config poco;
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    poco = (POCO.Config)serializer.Deserialize(fs);
                    fs.Close();
                }
                var settings = new EarlyBoundGeneratorConfig(poco);
                return settings;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured attempting to load Xml configuration: " + filePath, ex);
            }
        }

        /// <summary>
        /// Saves the Config to the given path
        /// </summary>
        /// <param name="filePath"></param>
        public void Save(string filePath)
        {
            var undoCheckoutIfUnchanged = FileRequiresUndoCheckout(filePath);

            var serializer = new XmlSerializer(typeof(EarlyBoundGeneratorConfig));
            var xmlWriterSettings = new XmlWriterSettings
            {
                Indent = true,
                NewLineOnAttributes = true,
            };
            using (var xmlWriter = XmlWriter.Create(filePath, xmlWriterSettings))
            {
                serializer.Serialize(xmlWriter, this);
                xmlWriter.Close();
            }

            // Put pipe delimited values on new lines to make it easier to see changes in source control
            var xml = File.ReadAllText(filePath);
            xml = xml.Replace("|", "|" + Environment.NewLine);
            File.WriteAllText(filePath, xml);

            if (undoCheckoutIfUnchanged)
            {
                var tfs = new VsTfsSourceControlProvider();
                tfs.UndoCheckoutIfUnchanged(filePath);
            }
        }

        private bool FileRequiresUndoCheckout(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }
            var attributes = File.GetAttributes(filePath);

            var undoCheckoutIfUnchanged = false;
            if (!attributes.HasFlag(FileAttributes.ReadOnly))
            {
                return false;
            }

            attributes &= ~FileAttributes.ReadOnly;
            if (ExtensionConfig.UseTfsToCheckoutFiles)
            {
                try
                {
                    var tfs = new VsTfsSourceControlProvider();
                    tfs.Checkout(filePath);
                    if (File.GetAttributes(filePath).HasFlag(FileAttributes.ReadOnly))
                    {
                        // something failed, just make it editable.
                        File.SetAttributes(filePath, attributes);
                    }
                    else
                    {
                        undoCheckoutIfUnchanged = true;
                    }
                }
                catch
                {
                    // eat it and just make it editable.
                    File.SetAttributes(filePath, attributes);
                }
            }
            else
            {
                File.SetAttributes(filePath, attributes);
            }

            return undoCheckoutIfUnchanged;
        }

        internal struct UserArgumentNames
        {
            public const string Out = "out";
            public const string OutDirectory = "outDirectory";
            public const string SettingsTemplateFile = "settingsTemplateFile";
        }

        internal struct BuilderSettingsJsonNames
        {
            public const string CodeCustomizationService = "codeCustomizationService";
            public const string CodeGenerationService = "codeGenerationService";
            public const string CodeWriterFilterService = "codeWriterFilterService";
            public const string CodeWriterMessageFilterService = "codeWriterMessageFilterService";
            public const string EmitFieldsClasses = "emitFieldsClasses";
            // ReSharper disable once InconsistentNaming
            public const string EmitEntityETC = "emitEntityETC";
            public const string EmitVirtualAttributes = "emitVirtualAttributes";
            public const string EntityNamesFilter = "entityNamesFilter";
            public const string EntityTypesFolder = "entityTypesFolder";
            public const string GenerateActions = "generateActions";
            public const string GenerateGlobalOptionSets = "generateGlobalOptionSets";
            public const string Language = "language";
            public const string Namespace = "namespace";
            public const string NamingService = "namingService";
            public const string MetadataProviderService = "metadataProviderService";
            public const string MetadataQueryProviderService = "metadataQueryProviderService";
            public const string MessageNamesFilter = "messageNamesFilter";
            public const string MessagesTypesFolder = "messagesTypesFolder";
            public const string OptionSetsTypesFolder = "optionSetsTypesFolder";
            public const string ServiceContextName = "serviceContextName";
            public const string SuppressGeneratedCodeAttribute = "suppressGeneratedCodeAttribute";
            public const string SuppressINotifyPattern = "suppressINotifyPattern";
        }

        public void PopulateBuilderProperties(Dictionary<string, JsonProperty> properties)
        {
            var defaultSettings = GetDefault();

            properties.SetJsonPropertyIfPopulated(BuilderSettingsJsonNames.CodeCustomizationService, CodeCustomizationService);
            properties.SetJsonPropertyIfPopulated(BuilderSettingsJsonNames.CodeGenerationService, CodeGenerationService);
            properties.SetJsonPropertyIfPopulated(BuilderSettingsJsonNames.CodeWriterFilterService, CodeWriterFilterService);
            properties.SetJsonPropertyIfPopulated(BuilderSettingsJsonNames.CodeWriterMessageFilterService, CodeWriterMessageFilterService);
            properties.SetJsonPropertyIfPopulated(BuilderSettingsJsonNames.MetadataProviderService, MetadataProviderService);
            properties.SetJsonPropertyIfPopulated(BuilderSettingsJsonNames.MetadataQueryProviderService, MetadataQueryProviderService);

            properties.SetJsonProperty(BuilderSettingsJsonNames.EmitEntityETC, EmitEntityETC);
            properties.SetJsonProperty(BuilderSettingsJsonNames.EmitVirtualAttributes, EmitVirtualAttributes);
            SetOutputFolderProperty(BuilderSettingsJsonNames.EntityTypesFolder, EntityTypesFolder, defaultSettings.EntityTypesFolder);
            properties.SetJsonProperty(BuilderSettingsJsonNames.GenerateActions, GenerateMessages);
            SetOutputFolderProperty(BuilderSettingsJsonNames.MessagesTypesFolder, MessageTypesFolder, defaultSettings.MessageTypesFolder, !GenerateMessages);
            properties.SetJsonProperty(BuilderSettingsJsonNames.Namespace, Namespace);
            properties.SetJsonPropertyIfPopulated(BuilderSettingsJsonNames.NamingService, NamingService);
            SetOutputFolderProperty(BuilderSettingsJsonNames.OptionSetsTypesFolder, OptionSetsTypesFolder, defaultSettings.OptionSetsTypesFolder);
            properties.SetJsonProperty(BuilderSettingsJsonNames.ServiceContextName, ServiceContextName);
            properties.SetJsonProperty(BuilderSettingsJsonNames.SuppressGeneratedCodeAttribute, SuppressGeneratedCodeAttribute);

            return;

            void SetOutputFolderProperty(string propertyName, string typeName, string @default, bool remove = false)
            {
                if (remove)
                {
                    propertyName = propertyName.LowerFirstChar();
                    if(properties.ContainsKey(propertyName))
                    {
                        properties.Remove(propertyName);
                    }
                    return;
                }

                properties.SetJsonPropertyIfPopulated(propertyName, typeName.Contains(".cs") ? @default : typeName);
            }
        }
    }
}

namespace DLaB.EarlyBoundGeneratorV2.Settings.POCO
{
    /// <summary>
    /// Serializable Class with Nullable types to be able to tell if they are populated or not
    /// </summary>
    public class Config
    {
        public bool? AudibleCompletionNotification { get; set; }
        public string CodeCustomizationService { get; set; }
        public string CodeGenerationService { get; set; }
        public string CodeWriterFilterService { get; set; }
        public string CodeWriterMessageFilterService { get; set; }
        // ReSharper disable once InconsistentNaming
        public bool? EmitEntityETC { get; set; }
        public bool? EmitVirtualAttributes { get; set; }
        public string EntityTypesFolder { get; set; }
        public ExtensionConfig ExtensionConfig { get; set; }
        public List<Argument> ExtensionArguments { get; set; }
        public bool? IncludeCommandLine { get; set; }
        public bool? GenerateMessages { get; set; }
        public string Namespace { get; set; }
        public string NamingService { get; set; }
        public string MetadataProviderService { get; set; }
        public string MetadataQueryProviderService { get; set; }
        public string MessageTypesFolder { get; set; }
        public string OptionSetsTypesFolder { get; set; }
        public string ServiceContextName { get; set; }
        public bool? SuppressGeneratedCodeAttribute { get; set; }
        public bool? UpdateBuilderSettingsJson { get; set; }
        public List<Argument> UserArguments { get; set; }
        public string Version { get; set; }
    }
}
