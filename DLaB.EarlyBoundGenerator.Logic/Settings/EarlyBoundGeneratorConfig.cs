using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Source.DLaB.Common;
using Source.DLaB.Common.VersionControl;

namespace DLaB.EarlyBoundGenerator.Settings
{
    [Serializable]
    [XmlType("Config")]
    [XmlRoot("Config")]
    public class EarlyBoundGeneratorConfig
    {
        #region Properties

        /// <summary>
        /// Use speech synthesizer to notify of code generation completion.
        /// </summary>
        [Category("Global")]
        [DisplayName("Audible Completion Notification")]
        [Description("Use speech synthesizer to notify of code generation completion.")]
        public bool AudibleCompletionNotification { get; set; }

        /// <summary>
        /// The CrmSvcUtil relative path.
        /// </summary>
        /// <value>
        /// The CrmSvcUtil relative path.
        /// </value>
        [Category("Global")]
        [DisplayName("CrmSvcUtil Relative Path")]
        [Description("The Path to the CrmSvcUtil.exe, relative to the CrmSvcUtil Realtive Root Path.  Defaults to using the CrmSvcUtil that comes by default.")]
        public string CrmSvcUtilRelativePath { get; set; }

        /// <summary>
        /// Specifies whether to include in the early bound class, the command line used to generate it.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [include command line]; otherwise, <c>false</c>.
        /// </value>
        [Category("Global")]
        [DisplayName("Include Command Line")]
        [Description("Specifies whether to include in the early bound class, the command line used to generate it.")]
        public bool IncludeCommandLine { get; set; }

        /// <summary>
        /// Masks the password in the command line
        /// </summary>
        /// <value>
        ///   <c>true</c> if [mask password]; otherwise, <c>false</c>.
        /// </value>
        [Category("Global")]
        [DisplayName("Mask Password")]
        [Description("Masks the password in the outputted command line.")]
        public bool MaskPassword { get; set; }

        /// <summary>
        /// Gets or sets the last ran version.
        /// </summary>
        /// <value>
        /// The last ran version.
        /// </value>
        [Category("Meta")]
        [DisplayName("Settings Version")]
        [Description("The Settings File Version.")]
        [ReadOnly(true)]
        public string SettingsVersion{ get; set; }
        /// <summary>
        /// The version of the EarlyBoundGeneratorPlugin
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [Category("Meta")]
        [DisplayName("Version")]
        [Description("Version of the Early Bound Generator.")]
        [ReadOnly(true)]
        public string Version { get; set; }
        /// <summary>
        /// Settings that will get written to the CrmSrvUtil.exe.config
        /// </summary>
        /// <value>
        /// The extension configuration.
        /// </value>
        [Category("CrmSvcUtil")]
        [DisplayName("Settings")]
        [Description("Settings that will get written to the CrmSrvUtil.exe.config.")]
        public ExtensionConfig ExtensionConfig { get; set; }
        /// <summary>
        /// These are the required commandline arguments that are passed to the CrmSrvUtil to correctly wire up the extensions in DLaB.CrmSvcUtilExtensions.
        /// </summary>
        /// <value>
        /// The extension arguments.
        /// </value>
        [Category("CrmSvcUtil")]
        [DisplayName("Extension Arguments")]
        [Description("These are the required commandline arguments that are passed to the CrmSrvUtil to correctly wire up the extensions in DLaB.CrmSvcUtilExtensions.")]
        public List<Argument> ExtensionArguments { get; set; }
        /// <summary>
        /// These are the commandline arguments that are passed to the CrmSrvUtil that can have varying values, depending on the user's preference.
        /// </summary>
        /// <value>
        /// The user arguments.
        /// </value>
        [Category("CrmSvcUtil")]
        [DisplayName("User Arguments")]
        [Description("Commandline arguments that are passed to the CrmSrvUtil that can have varying values, depending on the user's preference.")]
        public List<Argument> UserArguments { get; set; }

        #region NonSerialized Properties
        [XmlIgnore]
        [Browsable(false)]
        public bool UseCrmOnline { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public bool UseConnectionString { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public string ConnectionString { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public string Domain { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public string UserName { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public string Password { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public string RootPath { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public bool SupportsActions { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public string Url { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public IEnumerable<Argument> CommandLineArguments => UserArguments.Union(ExtensionArguments);

        [XmlIgnore]
        [Browsable(false)]
        public string CrmSvcUtilPath => 
            Directory.Exists(CrmSvcUtilRelativePath)
                ? CrmSvcUtilRelativePath
                : Path.Combine(CrmSvcUtilRealtiveRootPath ?? Directory.GetCurrentDirectory(), CrmSvcUtilRelativePath);

        [XmlIgnore]
        [Browsable(false)]
        public string CrmSvcUtilRealtiveRootPath { get; set; }

        #region UserArguments Helpers

        [XmlIgnore]
        [Browsable(false)]
        public string ActionOutPath
        {
            get { return GetUserArgument(CreationType.Actions, UserArgumentNames.Out).Value; }
            set { SetUserArgument(CreationType.Actions, UserArgumentNames.Out, value); }
        }

        [XmlIgnore]
        [Browsable(false)]
        public string EntityOutPath
        {
            get { return GetUserArgument(CreationType.Entities, UserArgumentNames.Out).Value; }
            set { SetUserArgument(CreationType.Entities, UserArgumentNames.Out, value); }
        }

        [XmlIgnore]
        [Browsable(false)]
        public string Namespace
        {
            get { return GetUserArgument(CreationType.All, UserArgumentNames.Namespace).Value; }
            set { SetUserArgument(CreationType.All, UserArgumentNames.Namespace, value); }
        }

        [XmlIgnore]
        [Browsable(false)]
        public string OptionSetOutPath
        {
            get { return GetUserArgument(CreationType.OptionSets, UserArgumentNames.Out).Value; }
            set { SetUserArgument(CreationType.OptionSets, UserArgumentNames.Out, value); }
        }

        [XmlIgnore]
        [Browsable(false)]
        public string ServiceContextName
        {
            get { return GetUserArgument(CreationType.Entities, UserArgumentNames.ServiceContextName).Value; }
            set { SetUserArgument(CreationType.Entities, UserArgumentNames.ServiceContextName, value); }
        }

        #endregion // UserArguments Helpers

        #endregion // NonSerialized Properties

        #endregion // Properties       

        private EarlyBoundGeneratorConfig()
        {
            CrmSvcUtilRelativePath = Config.GetAppSettingOrDefault("CrmSvcUtilRelativePath", @"DLaB.EarlyBoundGenerator\crmsvcutil.exe");
            UseConnectionString = Config.GetAppSettingOrDefault("UseConnectionString", false);
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        #region Add Missing Default settings

        /// <summary>
        /// Initializes a new instance of the <see cref="EarlyBoundGeneratorConfig"/> class.
        /// </summary>
        /// <param name="poco">The poco.</param>
        /// <param name="filePath">The file path.</param>
        private EarlyBoundGeneratorConfig(POCO.Config poco, string filePath)
        {
            var @default = GetDefault();
            var defaultConfig = @default.ExtensionConfig;
            var pocoConfig = poco.ExtensionConfig;

            CrmSvcUtilRelativePath = poco.CrmSvcUtilRelativePath ?? @default.CrmSvcUtilRelativePath;
            RemoveObsoleteValues(poco, @default);

            AudibleCompletionNotification = poco.AudibleCompletionNotification.GetValueOrDefault(@default.AudibleCompletionNotification);
            IncludeCommandLine = poco.IncludeCommandLine.GetValueOrDefault(@default.IncludeCommandLine);
            MaskPassword = poco.MaskPassword.GetValueOrDefault(@default.MaskPassword);


            UpdateObsoleteSettings(poco, pocoConfig, @default);

            ExtensionConfig = new ExtensionConfig
            {
                ActionPrefixesToSkip = AddPipeDelimitedMissingDefaultValues(pocoConfig.ActionPrefixesToSkip, defaultConfig.ActionPrefixesToSkip),
                ActionPrefixesWhitelist = AddPipeDelimitedMissingDefaultValues(pocoConfig.ActionPrefixesWhitelist, defaultConfig.ActionPrefixesWhitelist),
                ActionsWhitelist = AddPipeDelimitedMissingDefaultValues(pocoConfig.ActionsWhitelist, defaultConfig.ActionsWhitelist),
                ActionsToSkip = AddPipeDelimitedMissingDefaultValues(pocoConfig.ActionsToSkip, defaultConfig.ActionsToSkip),
                AddDebuggerNonUserCode = pocoConfig.AddDebuggerNonUserCode.GetValueOrDefault(defaultConfig.AddDebuggerNonUserCode),
                AddNewFilesToProject = pocoConfig.AddNewFilesToProject.GetValueOrDefault(defaultConfig.AddNewFilesToProject),
                CreateOneFilePerAction = pocoConfig.CreateOneFilePerAction.GetValueOrDefault(defaultConfig.CreateOneFilePerAction),
                CreateOneFilePerEntity = pocoConfig.CreateOneFilePerEntity.GetValueOrDefault(defaultConfig.CreateOneFilePerEntity),
                CreateOneFilePerOptionSet = pocoConfig.CreateOneFilePerOptionSet.GetValueOrDefault(defaultConfig.CreateOneFilePerOptionSet),
                EntitiesToSkip = AddPipeDelimitedMissingDefaultValues(pocoConfig.EntitiesToSkip, defaultConfig.EntitiesToSkip),
                EntitiesWhitelist = AddPipeDelimitedMissingDefaultValues(pocoConfig.EntitiesWhitelist, defaultConfig.EntitiesWhitelist),
                EntityAttributeSpecifiedNames = AddMissingDictionaryHashDefaultValues(pocoConfig.EntityAttributeSpecifiedNames, defaultConfig.EntityAttributeSpecifiedNames),
                EntityPrefixesToSkip = AddPipeDelimitedMissingDefaultValues(pocoConfig.EntityPrefixesToSkip, defaultConfig.EntityPrefixesToSkip),
                EntityPrefixesWhitelist = AddPipeDelimitedMissingDefaultValues(pocoConfig.EntityPrefixesWhitelist, defaultConfig.EntityPrefixesWhitelist),
                GenerateActionAttributeNameConsts = pocoConfig.GenerateActionAttributeNameConsts.GetValueOrDefault(defaultConfig.GenerateActionAttributeNameConsts),
                GenerateAttributeNameConsts = pocoConfig.GenerateAttributeNameConsts.GetValueOrDefault(defaultConfig.GenerateAttributeNameConsts),
                GenerateAnonymousTypeConstructor = pocoConfig.GenerateAnonymousTypeConstructor.GetValueOrDefault(defaultConfig.GenerateAnonymousTypeConstructor),
                GenerateEntityRelationships = pocoConfig.GenerateEntityRelationships.GetValueOrDefault(defaultConfig.GenerateEntityRelationships),
                GenerateEnumProperties = pocoConfig.GenerateEnumProperties.GetValueOrDefault(defaultConfig.GenerateEnumProperties),
                GenerateOnlyReferencedOptionSets = pocoConfig.GenerateOnlyReferencedOptionSets.GetValueOrDefault(defaultConfig.GenerateOnlyReferencedOptionSets),
                InvalidCSharpNamePrefix = pocoConfig.InvalidCSharpNamePrefix ?? defaultConfig.InvalidCSharpNamePrefix,
                MakeAllFieldsEditable = pocoConfig.MakeAllFieldsEditable ?? defaultConfig.MakeAllFieldsEditable,
                MakeReadonlyFieldsEditable = pocoConfig.MakeReadonlyFieldsEditable ?? defaultConfig.MakeReadonlyFieldsEditable,
                MakeResponseActionsEditable = pocoConfig.MakeResponseActionsEditable ?? defaultConfig.MakeResponseActionsEditable,
                LocalOptionSetFormat = pocoConfig.LocalOptionSetFormat ?? defaultConfig.LocalOptionSetFormat,
                OptionSetLanguageCodeOverride = pocoConfig.OptionSetLanguageCodeOverride ?? defaultConfig.OptionSetLanguageCodeOverride,
                OptionSetPrefixesToSkip = AddPipeDelimitedMissingDefaultValues(pocoConfig.OptionSetPrefixesToSkip, defaultConfig.OptionSetPrefixesToSkip),
                OptionSetsToSkip = AddPipeDelimitedMissingDefaultValues(pocoConfig.OptionSetsToSkip, defaultConfig.OptionSetsToSkip),
                PropertyEnumMappings = AddPipeDelimitedMissingDefaultValues(pocoConfig.PropertyEnumMappings, defaultConfig.PropertyEnumMappings),
                RemoveRuntimeVersionComment = pocoConfig.RemoveRuntimeVersionComment.GetValueOrDefault(defaultConfig.RemoveRuntimeVersionComment),
                UnmappedProperties = AddMissingDictionaryHashDefaultValues(pocoConfig.UnmappedProperties, defaultConfig.UnmappedProperties),
                UseDeprecatedOptionSetNaming = pocoConfig.UseDeprecatedOptionSetNaming.GetValueOrDefault(defaultConfig.UseDeprecatedOptionSetNaming),
                UseTfsToCheckoutFiles = pocoConfig.UseTfsToCheckoutFiles.GetValueOrDefault(defaultConfig.UseTfsToCheckoutFiles),
                UseXrmClient = pocoConfig.UseXrmClient.GetValueOrDefault(defaultConfig.UseXrmClient)
            };

            ExtensionArguments = AddMissingArguments(poco.ExtensionArguments, @default.ExtensionArguments);
            UserArguments = AddMissingArguments(poco.UserArguments, @default.UserArguments);
            SettingsVersion = string.IsNullOrWhiteSpace(poco.Version) ? "0.0.0.0" : poco.Version;
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private static void UpdateObsoleteSettings(POCO.Config poco, POCO.ExtensionConfig pocoConfig, EarlyBoundGeneratorConfig @default)
        {
            if (new Version(poco.Version) < new Version("1.2016.6.1"))
            {
                // Storing of UnmappedProperties and EntityAttributeSpecified Names switched from Key,Value1,Value2|Key,Value1,Value2 to Key:Value1,Value2|Key:Value1,Value2
                // Also convert from a List to a HashSet
                pocoConfig.EntityAttributeSpecifiedNames = ConvertNonColonDelimitedDictionaryListToDictionaryHash(pocoConfig.EntityAttributeSpecifiedNames);
                pocoConfig.UnmappedProperties = ConvertNonColonDelimitedDictionaryListToDictionaryHash(pocoConfig.UnmappedProperties);
            }

            if (new Version(poco.Version) < new Version("1.2018.9.12"))
            {
                // Update the OptionSet codecustomization Argument Setting to use the new Generic Code Customziation Service
                var oldValue = poco.ExtensionArguments.FirstOrDefault(
                    a => a.SettingType == CreationType.OptionSets
                         && a.Name == "codecustomization");
                var newValue = @default.ExtensionArguments.FirstOrDefault(
                    a => a.SettingType == CreationType.OptionSets
                         && a.Name == "codecustomization");
                if (oldValue != null
                    && newValue != null)
                {
                    poco.ExtensionArguments.Remove(oldValue);
                    poco.ExtensionArguments.Add(newValue);
                }
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

        private void RemoveObsoleteValues(POCO.Config poco, EarlyBoundGeneratorConfig @default)
        {
            if (CrmSvcUtilRelativePath == @"Plugins\CrmSvcUtil Ref\crmsvcutil.exe" 
                || CrmSvcUtilRelativePath == @"CrmSvcUtil Ref\crmsvcutil.exe")
            {
                // 12.15.2016 XTB changed to use use the User directory, no plugin folder needed now
                // 3.12.2019 Nuget Stopped liking spaces in the CrmSvcUtilFolder.  Updated to be the plugin specific DLaB.EarlyBoundGenerator
                CrmSvcUtilRelativePath = @default.CrmSvcUtilRelativePath;
            }
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

        private string AddPipeDelimitedMissingDefaultValues(string value, string @default)
        {
            try
            {
                if (value == null || @default == null)
                {
                    return @default ?? value;
                }

                var splitValues = value.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
                var hash = new HashSet<string>(splitValues);
                splitValues.AddRange(@default.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries).
                                              Where(key => !hash.Contains(key)));

                return Config.ToString(splitValues);
            }
            catch (Exception ex)
            {
                throw new Exception("Error Processing config value: " + value, ex);
            }
        }

        private string AddMissingDictionaryHashDefaultValues(string value, string @default)
        {
            try
            {
                if (value == null || @default == null)
                {
                    return @default ?? value;
                }
                var values = Config.GetDictionaryHash<string, string>(Guid.NewGuid().ToString(), value);
                var defaultValues = Config.GetDictionaryHash<string, string>(Guid.NewGuid().ToString(), @default);

                foreach (var entry in defaultValues)
                {
                    HashSet<string> hash;
                    if (!values.TryGetValue(entry.Key, out hash))
                    {
                        hash = new HashSet<string>();
                        values[entry.Key] = hash;
                    }

                    foreach (var item in entry.Value.Where(v => !hash.Contains(v)))
                    {
                        hash.Add(item);
                    }
                }

                // All missing values have been added.  Join back Values
                return Config.ToString(values);
            }
            catch (Exception ex)
            {
                throw new Exception("Error Processing config value: " + value, ex);
            }
        }

        private List<Argument> AddMissingArguments(List<Argument> value, List<Argument> @default)
        {
            if (value == null || @default == null)
            {
                return value ?? @default ?? new List<Argument>();
            }
            value.AddRange(@default.Where(arg => !value.Any(a => a.SettingType == arg.SettingType && a.Name == arg.Name)));

            return value;
        }

        public static EarlyBoundGeneratorConfig GetDefault()
        {

            var @default = new EarlyBoundGeneratorConfig
            {
                AudibleCompletionNotification = true,
                IncludeCommandLine = true,
                MaskPassword = true,    
                ExtensionArguments = new List<Argument>(new [] {
                    // Actions
                    new Argument(CreationType.Actions, CrmSrvUtilService.CodeCustomization, "DLaB.CrmSvcUtilExtensions.Action.CustomizeCodeDomService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.Actions, CrmSrvUtilService.CodeGenerationService, "DLaB.CrmSvcUtilExtensions.Action.CustomCodeGenerationService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.Actions, CrmSrvUtilService.CodeWriterFilter, "DLaB.CrmSvcUtilExtensions.Action.CodeWriterFilterService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.Actions, CrmSrvUtilService.MetadataProviderService, "DLaB.CrmSvcUtilExtensions.BaseMetadataProviderService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.Entities, CrmSrvUtilService.CodeCustomization, "DLaB.CrmSvcUtilExtensions.Entity.CustomizeCodeDomService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.Entities, CrmSrvUtilService.CodeGenerationService, "DLaB.CrmSvcUtilExtensions.Entity.CustomCodeGenerationService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.Entities, CrmSrvUtilService.CodeWriterFilter, "DLaB.CrmSvcUtilExtensions.Entity.CodeWriterFilterService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.Entities, CrmSrvUtilService.NamingService, "DLaB.CrmSvcUtilExtensions.NamingService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.Entities, CrmSrvUtilService.MetadataProviderService, "DLaB.CrmSvcUtilExtensions.Entity.MetadataProviderService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.OptionSets, CrmSrvUtilService.CodeCustomization, "DLaB.CrmSvcUtilExtensions.OptionSet.CustomizeCodeDomService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.OptionSets, CrmSrvUtilService.CodeGenerationService, "DLaB.CrmSvcUtilExtensions.OptionSet.CustomCodeGenerationService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.OptionSets, CrmSrvUtilService.CodeWriterFilter, "DLaB.CrmSvcUtilExtensions.OptionSet.CodeWriterFilterService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.OptionSets, CrmSrvUtilService.NamingService, "DLaB.CrmSvcUtilExtensions.NamingService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.OptionSets, CrmSrvUtilService.MetadataProviderService, "DLaB.CrmSvcUtilExtensions.BaseMetadataProviderService,DLaB.CrmSvcUtilExtensions")
                }),
                ExtensionConfig = ExtensionConfig.GetDefault(),
                UserArguments = new List<Argument>(new [] {
                    new Argument(CreationType.Actions, "generateActions", null), 
                    new Argument(CreationType.Actions, "out",  @"EBG\Actions.cs"),
                    new Argument(CreationType.All, "namespace", "CrmEarlyBound"),
                    new Argument(CreationType.Entities, "out", @"EBG\Entities.cs"),
                    new Argument(CreationType.Entities, "servicecontextname", "CrmServiceContext"),
                    new Argument(CreationType.OptionSets, "out",  @"EBG\OptionSets.cs")
                }), 
            };
            @default.SettingsVersion = @default.Version;
            return @default;
        }

        #endregion // Add Missing Default settings

        public static EarlyBoundGeneratorConfig Load(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    var config = GetDefault();
                    return config;
                }

                var serializer = new XmlSerializer(typeof (POCO.Config));
                POCO.Config poco;
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    poco = (POCO.Config) serializer.Deserialize(fs);
                    fs.Close();
                }
                var settings = new EarlyBoundGeneratorConfig(poco, filePath);
                return settings;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured attempting to load Xml configuration: " + filePath, ex);
            }
        }

        public void Save(string filePath)
        {
            var undoCheckoutIfUnchanged = FileRequiresUndoCheckout(filePath);

            var serializer = new XmlSerializer(typeof (EarlyBoundGeneratorConfig));
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

            attributes = attributes & ~FileAttributes.ReadOnly;
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

        public string GetSettingValue(CreationType creationType, string setting)
        {
            var value = CommandLineArguments.FirstOrDefault(s => string.Equals(s.Name, setting, StringComparison.InvariantCultureIgnoreCase)
                                                                 && (s.SettingType == creationType || s.SettingType == CreationType.All));

            if (value == null)
            {
                throw new KeyNotFoundException("Unable to find setting for " + creationType + " " + setting);
            }

            return value.Value;
        }

        public Argument GetExtensionArgument(CreationType creationType, CrmSrvUtilService service)
        {
            return GetExtensionArgument(creationType, service.ToString().ToLower());
        }

        public Argument GetExtensionArgument(CreationType creationType, string setting)
        {
            return ExtensionArguments.FirstOrDefault(a => a.SettingType == creationType && 
                                                          string.Equals(a.Name, setting, StringComparison.InvariantCultureIgnoreCase)) ?? 
                new Argument(creationType, setting, string.Empty);
        }

        public void SetExtensionArgument(CreationType creationType, CrmSrvUtilService service, string value)
        {
            SetExtensionArgument(creationType, service.ToString().ToLower(), value);
        }

        public void SetExtensionArgument(CreationType creationType, string setting, string value)
        {
            var argument = GetExtensionArgument(creationType, setting);

            if (argument == null)
            {
                if (value != null)
                {
                    ExtensionArguments.Add(new Argument { Name = setting, SettingType = creationType, Value = value });
                }
            }
            else if (value == null)
            {
                ExtensionArguments.Remove(argument);
            }
            else
            {
                argument.Value = value;
            }
        }

        private Argument GetUserArgument(CreationType creationType, string setting)
        {
            var argument = UserArguments.FirstOrDefault(s =>
                string.Equals(s.Name, setting, StringComparison.InvariantCultureIgnoreCase)
                && s.SettingType == creationType);

            return argument ?? new Argument(creationType, setting, string.Empty);
        }

        private void SetUserArgument(CreationType creationType, string setting, string value)
        {
            var argument = GetUserArgument(creationType, setting);

            if (argument == null)
            {
                if (value != null)
                {
                    UserArguments.Add(new Argument {Name = setting, SettingType = creationType, Value = value});
                }
            }
            else if (value == null)
            {
                UserArguments.Remove(argument);
            }
            else
            {
                argument.Value = value;
            }
        }

        internal struct UserArgumentNames
        {
            public const string Namespace = "namespace";
            public const string Out = "out";
            public const string ServiceContextName = "servicecontextname";
        }
    }
}

namespace DLaB.EarlyBoundGenerator.Settings.POCO
{
    /// <summary>
    /// Serializable Class with Nullable types to be able to tell if they are populated or not
    /// </summary>
    public class Config
    {
        public bool? AudibleCompletionNotification { get; set; }
        public string CrmSvcUtilRelativePath { get; set; }
        public bool? IncludeCommandLine { get; set; }
        public bool? MaskPassword { get; set; }
        public ExtensionConfig ExtensionConfig { get; set; }
        public List<Argument> ExtensionArguments { get; set; }
        public List<Argument> UserArguments { get; set; }
        public string Version { get; set; }
    }
}