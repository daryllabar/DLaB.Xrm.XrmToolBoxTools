using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace DLaB.EarlyBoundGenerator.Settings
{
    [Serializable]
    public class Config
    {
        #region Properties

        /// <summary>
        /// The CRM Service utility relative path.
        /// </summary>
        /// <value>
        /// The CRM SVC utility relative path.
        /// </value>
        public string CrmSvcUtilRelativePath { get; set; }
        /// <summary>
        /// Specifies whether to include the command line in the early bound class used to generate it
        /// </summary>
        /// <value>
        ///   <c>true</c> if [include command line]; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeCommandLine { get; set; }
        /// <summary>
        /// Masks the password in the command line
        /// </summary>
        /// <value>
        ///   <c>true</c> if [mask password]; otherwise, <c>false</c>.
        /// </value>
        public Boolean MaskPassword { get; set; }
        /// <summary>
        /// The version of the EarlyBoundGeneratorPlugin
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public string Version { get; set; }
        /// <summary>
        /// Settings that will get written to the CrmSrvUtil.exe.config
        /// </summary>
        /// <value>
        /// The extension configuration.
        /// </value>
        public ExtensionConfig ExtensionConfig { get; set; }
        /// <summary>
        /// These are the required commandline arguments that are passed to the CrmSrvUtil to correctly wire up the extensions in DLaB.CrmSvcUtilExtensions.
        /// </summary>
        /// <value>
        /// The extension arguments.
        /// </value>
        public List<Argument> ExtensionArguments { get; set; }
        /// <summary>
        /// These are the commandline arguments that are passed to the CrmSrvUtil that can have varying values, depending on the user's preference.
        /// </summary>
        /// <value>
        /// The user arguments.
        /// </value>
        public List<Argument> UserArguments { get; set; }

        #region NonSerialized Properties

        [XmlIgnore]
        public bool UseCrmOnline { get; set; }

        [XmlIgnore]
        public string Domain { get; set; }

        [XmlIgnore]
        public string UserName { get; set; }

        [XmlIgnore]
        public string Password { get; set; }

        [XmlIgnore]
        public bool SupportsActions { get; set; }

        [XmlIgnore]
        public string Url { get; set; }

        [NonSerialized] private string _filePath;

        [XmlIgnore]
        public IEnumerable<Argument> CommandLineArguments
        {
            get { return UserArguments.Union(ExtensionArguments); }
        }

        [XmlIgnore]
        public string CrmSvcUtilPath
        {
            get
            {
                // If the path exists, use it.  If not, combing relative path
                return Directory.Exists(CrmSvcUtilRelativePath)
                    ? CrmSvcUtilRelativePath
                    // ReSharper disable once AssignNullToNotNullAttribute
                    : Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), CrmSvcUtilRelativePath);
            }
        }

        #region UserArguments Helpers

        [XmlIgnore]
        public string ActionOutPath
        {
            get { return GetUserArgument(CreationType.Actions, UserArgumentNames.Out); }
            set { SetUserArgument(CreationType.Actions, UserArgumentNames.Out, value); }
        }

        [XmlIgnore]
        public string EntityOutPath
        {
            get { return GetUserArgument(CreationType.Entities, UserArgumentNames.Out); }
            set { SetUserArgument(CreationType.Entities, UserArgumentNames.Out, value); }
        }

        [XmlIgnore]
        public string Namespace
        {
            get { return GetUserArgument(CreationType.All, UserArgumentNames.Namespace); }
            set { SetUserArgument(CreationType.All, UserArgumentNames.Namespace, value); }
        }

        [XmlIgnore]
        public string OptionSetOutPath
        {
            get { return GetUserArgument(CreationType.OptionSets, UserArgumentNames.Out); }
            set { SetUserArgument(CreationType.OptionSets, UserArgumentNames.Out, value); }
        }

        [XmlIgnore]
        public string ServiceContextName
        {
            get { return GetUserArgument(CreationType.Entities, UserArgumentNames.ServiceContextName); }
            set { SetUserArgument(CreationType.Entities, UserArgumentNames.ServiceContextName, value); }
        }

        #endregion // UserArguments Helpers

        #endregion // NonSerialized Properties

        #endregion // Properties       

        private Config()
        {
            CrmSvcUtilRelativePath = @"Plugins\CrmSvcUtil Ref\crmsvcutil.exe";
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        #region Add Missing Default settings

        /// <summary>
        /// Initializes a new instance of the <see cref="Config"/> class.
        /// </summary>
        /// <param name="poco">The poco.</param>
        /// <param name="filePath">The file path.</param>
        private Config(POCO.Config poco, string filePath)
        {
            var @default = GetDefault();
            var defaultConfig = @default.ExtensionConfig;
            var pocoConfig = poco.ExtensionConfig;

            CrmSvcUtilRelativePath = poco.CrmSvcUtilRelativePath ?? @default.CrmSvcUtilRelativePath;
            if (CrmSvcUtilRelativePath == @"CrmSvcUtil Ref\crmsvcutil.exe")
            {
                // 5.14.2015 XTB changed to use the Plugins Directory, but then MEF changed Paths to be realtive to Dll. 
                CrmSvcUtilRelativePath = @default.CrmSvcUtilRelativePath;
            }
            IncludeCommandLine = poco.IncludeCommandLine.GetValueOrDefault(@default.IncludeCommandLine);
            MaskPassword = poco.MaskPassword.GetValueOrDefault(@default.MaskPassword);
            ExtensionConfig = new ExtensionConfig
            {
                ActionsToSkip = AddPipeDelimitedMissingDefaultValues(pocoConfig.ActionsToSkip, defaultConfig.ActionsToSkip),
                AddDebuggerNonUserCode = pocoConfig.AddDebuggerNonUserCode.GetValueOrDefault(defaultConfig.AddDebuggerNonUserCode),
                AddNewFilesToProject = pocoConfig.AddNewFilesToProject.GetValueOrDefault(defaultConfig.AddNewFilesToProject),
                CreateOneFilePerAction = pocoConfig.CreateOneFilePerAction.GetValueOrDefault(defaultConfig.CreateOneFilePerAction),
                CreateOneFilePerEntity = pocoConfig.CreateOneFilePerEntity.GetValueOrDefault(defaultConfig.CreateOneFilePerEntity),
                CreateOneFilePerOptionSet = pocoConfig.CreateOneFilePerOptionSet.GetValueOrDefault(defaultConfig.CreateOneFilePerOptionSet),
                EntitiesToSkip = AddPipeDelimitedMissingDefaultValues(pocoConfig.EntitiesToSkip, defaultConfig.EntitiesToSkip),
                EntityAttributeSpecifiedNames = AddPipeThenCommaDelimitedMissingDefaultValues(pocoConfig.EntityAttributeSpecifiedNames, defaultConfig.EntityAttributeSpecifiedNames),
                GenerateAttributeNameConsts = pocoConfig.GenerateAttributeNameConsts.GetValueOrDefault(defaultConfig.GenerateAttributeNameConsts),
                GenerateAnonymousTypeConstructor = pocoConfig.GenerateAnonymousTypeConstructor.GetValueOrDefault(defaultConfig.GenerateAnonymousTypeConstructor),
                GenerateEnumProperties = pocoConfig.GenerateEnumProperties.GetValueOrDefault(defaultConfig.GenerateEnumProperties),
                InvalidCSharpNamePrefix = pocoConfig.InvalidCSharpNamePrefix ?? defaultConfig.InvalidCSharpNamePrefix,
                MakeReadonlyFieldsEditable = pocoConfig.MakeReadonlyFieldsEditable ?? defaultConfig.MakeReadonlyFieldsEditable,
                OptionSetsToSkip = AddPipeDelimitedMissingDefaultValues(pocoConfig.OptionSetsToSkip, defaultConfig.OptionSetsToSkip),
                PropertyEnumMappings = AddPipeDelimitedMissingDefaultValues(pocoConfig.PropertyEnumMappings, defaultConfig.PropertyEnumMappings),
                RemoveRuntimeVersionComment = pocoConfig.RemoveRuntimeVersionComment.GetValueOrDefault(defaultConfig.RemoveRuntimeVersionComment),
                UnmappedProperties = AddPipeThenCommaDelimitedMissingDefaultValues(pocoConfig.UnmappedProperties, defaultConfig.UnmappedProperties),
                UseTfsToCheckoutFiles = pocoConfig.UseTfsToCheckoutFiles.GetValueOrDefault(defaultConfig.UseTfsToCheckoutFiles),
                UseXrmClient = pocoConfig.UseXrmClient.GetValueOrDefault(defaultConfig.UseXrmClient)
            };

            ExtensionArguments = AddMissingArguments(poco.ExtensionArguments, @default.ExtensionArguments);
            UserArguments = AddMissingArguments(poco.UserArguments, @default.UserArguments);
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            _filePath = filePath;
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

                return string.Join("|", splitValues);
            }
            catch (Exception ex)
            {
                throw new Exception("Error Processing config value: " + value, ex);
            }
        }

        private string AddPipeThenCommaDelimitedMissingDefaultValues(string value, string @default)
        {
            try
            {
                if (value == null || @default == null)
                {
                    return @default ?? value;
                }

                var splitValues = value.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
                // Split by Dictionary of hashset
                var splitSplitValues = splitValues.ToDictionary(k => k.Split(new[] {','}).First().Trim(), v => new HashSet<string>(v.Split(new[] {','}).Skip(1).Select(s => s.Trim())));
                foreach (var entry in @default.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()))
                {
                    var values = entry.Split(new[] {','}).Select(s => s.Trim()).ToList();
                    HashSet<string> hash;
                    if (splitSplitValues.TryGetValue(values.First(), out hash))
                    {
                        foreach (var commaValue in values.Skip(1).
                            Where(commaValue => !hash.Contains(commaValue)))
                        {
                            hash.Add(commaValue);
                        }
                    }
                    else
                    {
                        splitSplitValues.Add(values.First(), new HashSet<string>(values.Skip(1)));
                    }
                }

                // All missing values have been added.  Join back Values
                return string.Join("|", splitSplitValues.Select(entry => entry.Key + "," + string.Join(",", entry.Value)));
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

        public static Config GetDefault()
        {
            return new Config
            {
                IncludeCommandLine = true,
                MaskPassword = true,    
                ExtensionArguments = new List<Argument>(new [] {
                    // Actions
                    new Argument(CreationType.Actions, "codecustomization", "DLaB.CrmSvcUtilExtensions.Action.CustomizeCodeDomService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.Actions, "codegenerationservice", "DLaB.CrmSvcUtilExtensions.Action.CustomCodeGenerationService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.Actions, "codewriterfilter", "DLaB.CrmSvcUtilExtensions.Action.CodeWriterFilterService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.Entities, "codecustomization", "DLaB.CrmSvcUtilExtensions.Entity.CustomizeCodeDomService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.Entities, "codegenerationservice", "DLaB.CrmSvcUtilExtensions.Entity.CustomCodeGenerationService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.Entities, "codewriterfilter", "DLaB.CrmSvcUtilExtensions.Entity.CodeWriterFilterService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.Entities, "namingservice", "DLaB.CrmSvcUtilExtensions.Entity.OverridePropertyNames,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.Entities, "metadataproviderservice", "DLaB.CrmSvcUtilExtensions.Entity.MetadataProviderService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.OptionSets, "codecustomization", "DLaB.CrmSvcUtilExtensions.OptionSet.CreateOptionSetEnums,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.OptionSets, "codegenerationservice", "DLaB.CrmSvcUtilExtensions.OptionSet.CustomCodeGenerationService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.OptionSets, "codewriterfilter", "DLaB.CrmSvcUtilExtensions.OptionSet.FilterOptionSetEnums,DLaB.CrmSvcUtilExtensions")
                }),
                ExtensionConfig = ExtensionConfig.GetDefault(),
                UserArguments = new List<Argument>(new [] {
                    new Argument(CreationType.Actions, "generateActions", null), 
                    new Argument(CreationType.Actions, "out", @"Plugins\CrmSvcUtil Ref\Actions.cs"),
                    new Argument(CreationType.All, "namespace", "CrmEarlyBound"),
                    new Argument(CreationType.Entities, "out", @"Plugins\CrmSvcUtil Ref\Entities.cs"),
                    new Argument(CreationType.Entities, "servicecontextname", "CrmServiceContext"),
                    new Argument(CreationType.OptionSets, "out", @"Plugins\CrmSvcUtil Ref\OptionSets.cs")
                }),            
            };
        }


        #endregion // Add Missing Default settings

        public static Config Load(string filePath)
        {
            filePath = Path.Combine(filePath, "DLaB.EarlyBoundGenerator.Settings.xml");
            if (!File.Exists(filePath))
            {
                var config = GetDefault();
                config._filePath = filePath;
                return config;
            }

            var serializer = new XmlSerializer(typeof(POCO.Config));
            POCO.Config poco;
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                poco = (POCO.Config)serializer.Deserialize(fs);
                fs.Close();
            }
            var settings = new Config(poco, filePath);
            return settings;
        }

        public void Save()
        {
            var serializer = new XmlSerializer(typeof (Config));
            var settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineOnAttributes = true,
            };
            using (var xmlWriter = XmlWriter.Create(_filePath, settings))
            {
                serializer.Serialize(xmlWriter, this);
                xmlWriter.Close();
            }
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

        private string GetUserArgument(CreationType creationType, string setting)
        {
            var argument = UserArguments.FirstOrDefault(s =>
                string.Equals(s.Name, setting, StringComparison.InvariantCultureIgnoreCase)
                && s.SettingType == creationType);

            return argument == null ? string.Empty : argument.Value;
        }

        private void SetUserArgument(CreationType creationType, string setting, string value)
        {
            var argument = UserArguments.FirstOrDefault(s =>
                string.Equals(s.Name, setting, StringComparison.InvariantCultureIgnoreCase)
                && s.SettingType == creationType);

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
        public string CrmSvcUtilRelativePath { get; set; }
        public bool? IncludeCommandLine { get; set; }
        public bool? MaskPassword { get; set; }
        public ExtensionConfig ExtensionConfig { get; set; }
        public List<Argument> ExtensionArguments { get; set; }
        public List<Argument> UserArguments { get; set; }
        public string Version { get; set; }
    }
}