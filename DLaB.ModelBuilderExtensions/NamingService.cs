using DLaB.ModelBuilderExtensions.OptionSet.Transliteration;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace DLaB.ModelBuilderExtensions
{
    public class NamingService : TypedServiceBase<INamingService>, INamingService
    {
        public const int English = 1033;

        public bool AdjustCasingForEnumOptions { get => DLaBSettings.AdjustCasingForEnumOptions; set => DLaBSettings.AdjustCasingForEnumOptions = value; }
        public bool CamelCaseClassNames { get => DLaBSettings.CamelCaseClassNames; set => DLaBSettings.CamelCaseClassNames = value; }
        public bool CamelCaseMemberNames { get => DLaBSettings.CamelCaseMemberNames; set => DLaBSettings.CamelCaseMemberNames = value; }
        public bool CamelCaseOptionSetNames { get => DLaBSettings.CamelCaseOptionSetNames; set => DLaBSettings.CamelCaseOptionSetNames = value; }
        public Dictionary<string, HashSet<string>> EntityAttributeSpecifiedNames { get => DLaBSettings.EntityAttributeSpecifiedNames; set => DLaBSettings.EntityAttributeSpecifiedNames = value; }
        public Dictionary<string, string> EntityClassNameOverrides { get => DLaBSettings.EntityClassNameOverrides; set => DLaBSettings.EntityClassNameOverrides = value; }
        public string InvalidCSharpNamePrefix { get => DLaBSettings.InvalidCSharpNamePrefix; set => DLaBSettings.InvalidCSharpNamePrefix = value; }
        public Dictionary<string, string> LabelTextReplacement { get => DLaBSettings.LabelTextReplacement; set => DLaBSettings.LabelTextReplacement = value; }
        public int LanguageCodeOverride { get => DLaBSettings.OptionSetLanguageCodeOverride; set => DLaBSettings.OptionSetLanguageCodeOverride = value; }
        public string LocalOptionSetFormat { get => DLaBSettings.LocalOptionSetFormat; set => DLaBSettings.LocalOptionSetFormat = value; }
        public Dictionary<string, string> OptionSetNames { get => DLaBSettings.OptionSetNames; set => DLaBSettings.OptionSetNames = value; }
        public bool UseCrmSvcUtilStateEnumNamingConvention { get => DLaBSettings.UseCrmSvcUtilStateEnumNamingConvention; set => DLaBSettings.UseCrmSvcUtilStateEnumNamingConvention = value; }
        public bool UseDisplayNameForBpfName { get => DLaBSettings.UseDisplayNameForBpfName; set => DLaBSettings.UseDisplayNameForBpfName = value; }
        public bool UseLogicalNames { get => DLaBSettings.UseLogicalNames; set => DLaBSettings.UseLogicalNames = value; }
        public string ValidCSharpNameRegEx { get => DLaBSettings.ValidCSharpNameRegEx; set => DLaBSettings.ValidCSharpNameRegEx = value; }

        private readonly int _languageCode;
        private HashSet<string> _entityNames;
        private readonly Dictionary<string,string> _generatedBpfLogicalNamesByClassName = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _generatedNames = new Dictionary<string, string>();

        private TransliterationService TransliterationService { get; set; }

        /// <summary>
        /// This field keeps track of options with the same name and the values that have been defined.  Internal Dictionary Key is Name + "_" + Value
        /// </summary>
        private Dictionary<OptionSetMetadataBase, Dictionary<string, bool>> OptionNameValueDuplicates { get; set; } = new Dictionary<OptionSetMetadataBase, Dictionary<string, bool>>();

        #region Constructors

        public NamingService(INamingService defaultService, IDictionary<string, string> parameters) : base(defaultService, parameters)
        {
            TransliterationService = new TransliterationService(DLaBSettings);
            _languageCode = GetLanguageCode(defaultService);
        }

        public NamingService(INamingService defaultService, DLaBModelBuilderSettings settings = null) : base(defaultService, settings)
        {
            TransliterationService = new TransliterationService(DLaBSettings);
            _languageCode = GetLanguageCode(defaultService);
        }

        #endregion Constructors   

        /// <summary>
        /// Used to determine the language code used by the default service, but allow for the override
        /// </summary>
        /// <param name="defaultService"></param>
        /// <returns></returns>
        private int GetLanguageCode(INamingService defaultService)
        {
            var languageCode = LanguageCodeOverride <= 0
                ? English
                : LanguageCodeOverride;
            if (defaultService == null)
            {
                // Should only happen in testing.
                return languageCode;
            }
            var parametersField = defaultService.GetType().GetField("_parameters", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (!(parametersField?.GetValue(defaultService) is ModelBuilderInvokeParameters modelParameters)) {
                return languageCode;
            }

            if (LanguageCodeOverride > 0)
            {
                modelParameters.SystemDefaultLanguageId = LanguageCodeOverride;
            }
            else
            {
                languageCode = modelParameters.SystemDefaultLanguageId ?? languageCode;
            }

            return languageCode;
        }

        public HashSet<string> GetEntityNames(IServiceProvider services)
        {
            if (_entityNames != null)
            {
                return _entityNames;
            }
            _entityNames = new HashSet<string>(ServiceCache.GetDefault(services).EntityMetadataByLogicalName.Values.Select(e => GetNameForEntity(e, services)).ToArray());
            return _entityNames;
        }

        /// <summary>
        /// Provide a new implementation for finding a name for an OptionSet. If the
        /// OptionSet is not global, we want the name to be the concatenation of the Entity's
        /// name and the Attribute's name.  Otherwise, we can use the default implementation.
        /// </summary>
        public string GetNameForOptionSet(EntityMetadata entityMetadata, OptionSetMetadataBase optionSetMetadata, IServiceProvider services)
        {
            var generatedKey = $"OptionSet:{entityMetadata?.LogicalName}|!|{optionSetMetadata?.Name}";
            if (_generatedNames.TryGetValue(generatedKey, out var generatedName))
            {
                return generatedName;
            }
            SetServiceCache(services);
            var name = GetPossiblyConflictedNameForOptionSet(entityMetadata, optionSetMetadata, services);
            var entityNames = GetEntityNames(services);
            while (entityNames.Contains(name))
            {
                name += "_Enum";
            }

            if(UseCrmSvcUtilStateEnumNamingConvention
                && optionSetMetadata.IsGlobal != true
                && name.ToLower().EndsWith("_statecode")
                && entityMetadata.Attributes.FirstOrDefault(a => a.AttributeType == AttributeTypeCode.State) != null)
            {
                name = GetNameForEntity(entityMetadata, services) + "State";
            }

            if(OptionSetNames.TryGetValue(name.ToLower(), out var overriden))
            {
                _generatedNames[generatedKey] = overriden;
                return overriden;
            }

            _generatedNames[generatedKey] = name;
            return name;
        }

        private string GetPossiblyConflictedNameForOptionSet(EntityMetadata entityMetadata, OptionSetMetadataBase optionSetMetadata, IServiceProvider services)
        {
            var defaultName = DefaultService.GetNameForOptionSet(entityMetadata, optionSetMetadata, services);

            if (UseDisplayNameForBpfName && defaultName.ToLower().EndsWith("_statecode"))
            {
                var end = "_" + defaultName.Split('_').Last();
                var name = defaultName.Substring(0, defaultName.Length - end.Length);
                defaultName = GetBpfNameToDisplay(name, entityMetadata) + end;
            }

            // Ensure that the OptionSet is not global before using the custom implementation.
            if (optionSetMetadata.IsGlobal.HasValue && !optionSetMetadata.IsGlobal.Value)
            {
                // Find the attribute which uses the specified OptionSet.
                var attribute =
                    (from a in entityMetadata.Attributes
                        where a.AttributeType == AttributeTypeCode.Picklist &&
                              ((EnumAttributeMetadata) a).OptionSet.MetadataId == optionSetMetadata.MetadataId
                        select a).FirstOrDefault();

                // Check for null, since statuscode attributes on custom entities are not global, 
                // but their optionsets are not included in the attribute metadata of the entity, either.
                if (attribute == null)
                {
                    if (optionSetMetadata.OptionSetType.GetValueOrDefault() == OptionSetType.Status && defaultName.EndsWith("statuscode"))
                    {
                        defaultName = string.Format(LocalOptionSetFormat, GetNameForEntity(entityMetadata, services), "StatusCode");
                    }
                }
                else
                {
                    // Concatenate the name of the entity and the name of the attribute
                    // together to form the OptionSet name.
                    return string.Format(LocalOptionSetFormat, GetNameForEntity(entityMetadata, services),
                        GetNameForAttribute(entityMetadata, attribute, services, CamelCaseClassNames, UseLogicalNames));
                }
            }

            return UpdateCasingForGlobalOptionSets(defaultName, optionSetMetadata);
        }

        private static readonly Dictionary<string,string> CasingByGlobalOptionSet = new Dictionary<string, string>{
            { "budgetstatus", "BudgetStatus" },
            { "componentstate", "ComponentState" },
            { "componenttype", "ComponentType" },
            { "connectionrole_category", "ConnectionRole_Category" },
            { "convertrule_channelactivity", "ConvertRule_ChannelActivity" },
            { "dependencytype", "DependencyType" },
            { "emailserverprofile_authenticationprotocol", "EmailServerProfile_AuthenticationProtocol" },
            { "field_security_permission_type", "Field_Security_Permission_Type" },
            { "goal_fiscalperiod", "Goal_FiscalPeriod" },
            { "goal_fiscalyear", "Goal_FiscalYear" },
            { "incident_caseorigincode", "Incident_CaseOriginCode" },
            { "initialcommunication", "InitialCommunication" },
            { "lead_salesstage", "Lead_SalesStage" },
            { "metric_goaltype", "Metric_GoalType" },
            { "need", "Need" },
            { "opportunity_salesstage", "Opportunity_SalesStage" },
            { "principalsyncattributemapping_syncdirection", "PrincipalSyncAttributeMapping_SyncDirection" },
            { "processstage_category", "Processstage_Category" },
            { "purchaseprocess", "PurchaseProcess" },
            { "purchasetimeframe", "PurchaseTimeFrame" },
            { "qooi_pricingerrorcode", "Qooi_PricingErrorCode" },
            { "qooiproduct_producttype", "QooiProduct_ProductType" },
            { "qooiproduct_propertiesconfigurationstatus", "QooiProduct_PropertiesConfigurationStatus" },
            { "recurrencerule_monthofyear", "RecurrenceRule_MonthOfYear" },
            { "servicestage", "ServiceStage" },
            { "sharepoint_validationstatus", "SharePoint_ValidationStatus" },
            { "sharepoint_validationstatusreason", "SharePoint_ValidationStatusReason" },
            { "sharepointdocumentlocation_locationtype", "SharePointDocumentLocation_LocationType" },
            { "socialactivity_postmessagetype", "SocialActivity_PostMessageType" },
            { "socialprofile_community", "SocialProfile_Community" },
            { "syncattributemapping_syncdirection", "SyncAttributeMapping_SyncDirection" },
            { "workflow_runas", "Workflow_RunAs" },
            { "workflow_stage", "Workflow_Stage" },
            { "workflowlog_objecttypecode", "WorkflowLog_ObjectTypeCode" }
        };

        private string UpdateCasingForGlobalOptionSets(string name, OptionSetMetadataBase optionSetMetadata)
        {
            return CasingByGlobalOptionSet.TryGetValue(name, out var casing)
                ? casing
                : UpdateCasingForCustomGlobalOptionSets(name, optionSetMetadata);
        }

        private string UpdateCasingForCustomGlobalOptionSets(string name, OptionSetMetadataBase optionSetMetadata)
        {
            var preferredEndings = new [] {"StateCode", "Status", "State"};
            var displayName = optionSetMetadata.DisplayName?.GetLocalOrDefaultText() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(displayName))
            {
                return CamelCaseClassNames
                    ? CamelCaser.Case(name, preferredEndings)
                    : name;
            }

            displayName = displayName.RemoveDiacritics().Replace(" ", ""); // Remove spaces
            if (name.EndsWith(displayName.ToLower()))
            {
                name = name.Replace(displayName.ToLower(), displayName);
            }
            else if (name.Contains(displayName.ToLower()) && name.IndexOf(displayName.ToLower(), StringComparison.Ordinal) == name.LastIndexOf(displayName.ToLower(), StringComparison.Ordinal))
            {
                // Name only contains the display name, and only once, but also contains other characters.  Capitalize the Display Name, and the next character
                // as long as more than one character exists: given HelloWorld, helloworldstatus => HelloWorldStatus but helloworlds => HelloWorlds
                // May need to check for plural instead... s/es/ies
                var index = name.IndexOf(displayName.ToLower(), StringComparison.Ordinal) + displayName.Length;
                name = name.Replace(displayName.ToLower(), displayName);
                if (index < name.Length - 1)
                {
                    name = name.Substring(0, index) + char.ToUpper(name[index]) + name.Substring(index + 1, name.Length - index - 1);
                }
            }
            return CamelCaseClassNames
                ? CamelCaser.Case(name, preferredEndings)
                : name;
        }

        public string GetNameForOption(OptionSetMetadataBase optionSetMetadata, OptionMetadata optionMetadata, IServiceProvider services)
        {
            SetServiceCache(services);
            var possiblyDuplicateName = GetPossiblyDuplicateNameForOption(optionSetMetadata, services, optionMetadata);
            return AppendValueForDuplicateOptionSetValueNames(optionSetMetadata, possiblyDuplicateName, optionMetadata.Value.GetValueOrDefault(), services);
        }

        private string Transliterate(OptionMetadata optionMetadata, string defaultName)
        {
            var localizedLabels = optionMetadata.Label.LocalizedLabels;
            if (LanguageCodeOverride <= 0)
            {
                if (IsLabelPopulated(defaultName))
                {
                    return defaultName.RemoveDiacritics();
                }
            }
            else
            {
                var overrideLabel = localizedLabels.FirstOrDefault(l => l.LanguageCode == LanguageCodeOverride && IsLabelPopulated(l.Label));
                if (overrideLabel != null)
                {
                    return TransliterationService.HasCode(LanguageCodeOverride)
                        ? TransliterationService.Transliterate(overrideLabel)
                        : overrideLabel.Label.RemoveDiacritics();
                }
            }

            var localizedLabel = localizedLabels.FirstOrDefault(x => TransliterationService.HasCode(x.LanguageCode) && IsLabelPopulated(x.Label));

            return localizedLabel == null ?
                localizedLabels.FirstOrDefault(x => IsLabelPopulated(x.Label))?.Label?.RemoveDiacritics() ?? string.Empty :
                TransliterationService.Transliterate(localizedLabel);
        }

        /// <summary>
        /// Determines whether the name is null, or empty, or "UnknownLabel"
        /// </summary>
        /// <param name="label">The name.</param>
        /// <returns></returns>
        private static bool IsLabelPopulated(string label)
        {
            return !string.IsNullOrEmpty(label) && !label.Contains("UnknownLabel");
        }

        /// <summary>
        /// Checks to make sure that the name begins with a valid character. If the name does not begin with a valid character, then add the InvalidCSharpNamePrefix to the beginning of the name.
        /// </summary>
        private string GetValidCSharpName(string name)
        {
            //remove spaces and special characters
            name = Regex.Replace(name, ValidCSharpNameRegEx, string.Empty);
            if (name.Length > 0 && !char.IsLetter(name, 0) && name[0] != '_')
            {
                name = InvalidCSharpNamePrefix + name;
            }
            else if (name.Length == 0)
            {
                name = InvalidCSharpNamePrefix;
            }

            return name;
        }

        /// <summary>
        /// Checks to make sure that the name does not already exist for the OptionSet to be generated.
        /// </summary>
        private Dictionary<string, bool> GetDuplicateNameValues(OptionSetMetadataBase metadata, IServiceProvider services)
        {
            var nameValueDups = new Dictionary<string, bool>();
            // Look through all options, populating the namesAndValues Collection
            foreach (var option in metadata.GetOptions())
            {
                var name = GetPossiblyDuplicateNameForOption(metadata, services, option);

                nameValueDups[name] = nameValueDups.ContainsKey(name);

                if (metadata.OptionSetType == OptionSetType.Status)
                {
                    // For Statuses, also do State
                    name = AppendState(option, name);
                    nameValueDups[name] = nameValueDups.ContainsKey(name);
                }
            }
            return nameValueDups;
        }

        private string GetPossiblyDuplicateNameForOption(OptionSetMetadataBase metadata, IServiceProvider services, OptionMetadata option)
        {
            var defaultName = GetDefaultNameForOptionWithLabelReplacements(metadata, services, option);
            defaultName = Transliterate(option, defaultName);

            var name = GetValidCSharpName(defaultName);
            if (defaultName == string.Empty)
            {
                name += option.Value;
            }

            return CamelCaseOptionSetNames
                ? CamelCaser.Case(name)
                : name;
        }

        private string GetDefaultNameForOptionWithLabelReplacements(OptionSetMetadataBase metadata, IServiceProvider services, OptionMetadata option)
        {
            var updatedLabels = new Dictionary<Guid, string>();
            var labels = new List<LocalizedLabel>();
            labels.AddRange(option.Label.LocalizedLabels);
            labels.Add(option.Label.UserLocalizedLabel);
            foreach (var replacement in LabelTextReplacement)
            {
                foreach (var label in labels.Where(l => l.Label.Contains(replacement.Key)))
                {
                    label.MetadataId = label.MetadataId ?? Guid.NewGuid();
                    updatedLabels[label.MetadataId.Value] = label.Label;
                    label.Label = label.Label.Replace(replacement.Key, replacement.Value);
                }
            }

            var defaultName = AdjustCasingForEnumOptions
                ? GetNameForOption_Hack(metadata, option, services)
                : DefaultService.GetNameForOption(metadata, option, services);
            foreach (var updatedLabel in updatedLabels)
            {
                foreach (var label in labels.Where(l => l.MetadataId == updatedLabel.Key)){
                    label.Label = updatedLabel.Value;
                }
            }

            return defaultName;
        }

        /// <summary>
        /// Copied and tweaked from the Microsoft.PowerPlatform.Dataverse.ModelBuilderLib.NamingService
        /// </summary>
        /// <param name="optionSetMetadata"></param>
        /// <param name="optionMetadata"></param>
        /// <param name="services"></param>
        /// <returns></returns>
        private string GetNameForOption_Hack(OptionSetMetadataBase optionSetMetadata, OptionMetadata optionMetadata, IServiceProvider services)
        {
            var generatedKey = $"Option:{optionSetMetadata?.Name}|!|{optionMetadata?.Value.GetValueOrDefault()}";
            if (_generatedNames.TryGetValue(generatedKey, out var generatedName))
            {
                return generatedName;
            }

            string name = string.Empty;
            StateOptionMetadata stateOption = optionMetadata as StateOptionMetadata;
            if (stateOption != null)
            {
                name = stateOption.InvariantName;
            }

            if (string.IsNullOrEmpty(name)) // Name still null try this area. 
            {
                if (optionMetadata.Label != null &&
                    optionMetadata.Label.LocalizedLabels.Any()) // Counter check for localization miss ( no 1033 label ) 
                {
                    // Need to add get for current system default language. 
                    var lblToUse = optionMetadata.Label.LocalizedLabels.FirstOrDefault(f => f.LanguageCode == _languageCode)
                                    ?? optionMetadata.Label.LocalizedLabels.FirstOrDefault(f => f.LanguageCode == English);
                    if (lblToUse != null && !string.IsNullOrEmpty(lblToUse.Label))
                        name = lblToUse.Label;
                }

                if (string.IsNullOrEmpty(name)
                    && optionMetadata.Label.UserLocalizedLabel != null)
                {
                    name = optionMetadata.Label.UserLocalizedLabel.Label;
                }

                // Fail over check. 
                if (string.IsNullOrEmpty(name) &&
                    optionMetadata.Label != null &&
                    optionMetadata.Label.LocalizedLabels.Any())
                {
                    // For whatever reason, the system default language did not return a name, try to get the first label available. 
                    LocalizedLabel lblToUse = optionMetadata.Label.LocalizedLabels.FirstOrDefault();
                    if (lblToUse != null && !string.IsNullOrEmpty(lblToUse.Label))
                        name = lblToUse.Label;
                }
            }

            //name = CreateValidName(name);
            name = GetNameFromLabel(name);

            if (string.IsNullOrEmpty(name))
                name = string.Format(CultureInfo.InvariantCulture, "UnknownLabel{0}", optionMetadata.Value.Value);

            _generatedNames[generatedKey] = name;
            return name;
        }

        public string GetNameFromLabel(string label)
        {
            var underScoredName = Regex.Replace(label, ValidCSharpNameRegEx, "_");
            var words = underScoredName.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < words.Length; i++)
            {
                var word = words[i];
                if (string.IsNullOrWhiteSpace(word))
                {
                    continue;
                }
                words[i] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word.ToLower());
            }
            return string.Join("", words);
        }

        private static string AppendState(OptionMetadata option, string name)
        {
            var statusOption = (StatusOptionMetadata) option;
            name += "_" + (statusOption.State == 0 ? "Active" : "Inactive");
            return name;
        }

        /// <summary>
        /// Appends the value of the optionset for options with duplicatenames.
        /// </summary>
        /// <param name="optionSetMetadata">The option set metadata.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        private string AppendValueForDuplicateOptionSetValueNames(OptionSetMetadataBase optionSetMetadata, string name, int? value, IServiceProvider services)
        {
            if (!OptionNameValueDuplicates.TryGetValue(optionSetMetadata, out var duplicateNameValues))
            {
                // Only do the work of determining all of the names and their values once.
                duplicateNameValues = GetDuplicateNameValues(optionSetMetadata, services);
                OptionNameValueDuplicates[optionSetMetadata] = duplicateNameValues;
            }

            if (!duplicateNameValues[name])
            {
                // No Dup, return
                return name;
            }

            if (optionSetMetadata.OptionSetType == OptionSetType.Status)
            {
                name = AppendState(optionSetMetadata.GetOptions().Single(o => o.Value == value), name);
                if (!duplicateNameValues[name])
                {
                    // Appended with State, No Dup, return
                    return name;
                }
            }

            // Postfix name with numerical value
            if (value < 0)
            {
                // Handle Negativives
                {
                    name += "_neg" + Math.Abs(value.GetValueOrDefault());
                    return name;
                }
            }
            name += "_" + value;
            return name;
        }

        /// <summary>
        /// Allows for Specified Attribute Names to be used to set the generated attribute name
        /// </summary>
        /// <param name="entityMetadata">The entity metadata.</param>
        /// <param name="attributeMetadata">The attribute metadata.</param>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public string GetNameForAttribute(EntityMetadata entityMetadata, AttributeMetadata attributeMetadata, IServiceProvider services)
        {
            SetServiceCache(services);
            return GetNameForAttribute(entityMetadata, attributeMetadata, services, CamelCaseMemberNames, UseLogicalNames);
        }

        private string GetNameForAttribute(EntityMetadata entityMetadata, AttributeMetadata attributeMetadata, IServiceProvider services, bool camelCase, bool useLogicalNames)
        {
            string attributeName;

            if (EntityAttributeSpecifiedNames.TryGetValue(entityMetadata.LogicalName.ToLower(), out var specifiedNames) &&
                specifiedNames.Any(s => string.Equals(s, attributeMetadata.LogicalName, StringComparison.OrdinalIgnoreCase)))
            {
                attributeName = specifiedNames.First(s => string.Equals(s, attributeMetadata.LogicalName, StringComparison.OrdinalIgnoreCase));
            }
            else if (useLogicalNames)
            {
                attributeName = attributeMetadata.LogicalName;
            }
            else
            {
                attributeName = DefaultService.GetNameForAttribute(entityMetadata, attributeMetadata, services);
                attributeName = camelCase
                    ? CamelCaser.Case(attributeName)
                    : attributeName;
            }

            return attributeName;
        }

        public string GetNameForEntity(EntityMetadata entityMetadata, IServiceProvider services)
        {
            SetServiceCache(services);
            var defaultName = DefaultService.GetNameForEntity(entityMetadata, services);
            
            if (UseDisplayNameForBpfName)
            {
                defaultName = GetBpfNameToDisplay(defaultName, entityMetadata);
            }

            if(EntityClassNameOverrides.TryGetValue(entityMetadata.LogicalName, out var overriden))
            {
                defaultName = overriden;
            }

            return CamelCaseClassNames
                ? CamelCaser.Case(defaultName)
                : defaultName;
        }

        private string GetBpfNameToDisplay(string name, EntityMetadata entityMetadata)
        {
            return GetBpfNameToDisplay(name, entityMetadata, null);
        }

        private string GetBpfNameToDisplay(string name, string entityLogicalName)
        {
            return GetBpfNameToDisplay(name, null, entityLogicalName);
        }

        private string GetBpfNameToDisplay(string name, EntityMetadata entityMetadata, string entityLogicalName)
        {
            var bpf = BpfInfo.Parse(name);
            if (!bpf.IsBpfName)
            {
                return name;
            }

            entityMetadata = entityMetadata ?? ServiceCache.EntityMetadataByLogicalName[entityLogicalName];
            name = bpf.Prefix + GetValidCSharpName(entityMetadata.DisplayName.GetLocalOrDefaultText()) + bpf.Postfix;
            name = MakeNameUniqueIfAlreadyUsedByDifferentLogicalName(name, entityMetadata, bpf);

            return name;
        }

        private string MakeNameUniqueIfAlreadyUsedByDifferentLogicalName(string name, EntityMetadata entityMetadata, BpfInfo bpf)
        {
            if (_generatedBpfLogicalNamesByClassName.TryGetValue(name, out var logicalName) && logicalName != entityMetadata.LogicalName)
            {
                name += bpf.Id;
            }
            else
            {
                _generatedBpfLogicalNamesByClassName[name] = entityMetadata.LogicalName;
            }

            return name;
        }

        public string GetNameForMessagePair(SdkMessagePair messagePair, IServiceProvider services)
        {
            SetServiceCache(services);
            var defaultName = DefaultService.GetNameForMessagePair(messagePair, services);
            return CamelCaseMemberNames
                ? CamelCaser.Case(defaultName)
                : defaultName;
        }

        public string GetNameForRequestField(SdkMessageRequest request, SdkMessageRequestField requestField, IServiceProvider services)
        {
            SetServiceCache(services);
            var defaultName = DefaultService.GetNameForRequestField(request, requestField, services);
            return CamelCaseMemberNames
                ? CamelCaser.Case(defaultName)
                : defaultName;
        }

        public string GetNameForResponseField(SdkMessageResponse response, SdkMessageResponseField responseField, IServiceProvider services)
        {
            SetServiceCache(services);
            var defaultName = DefaultService.GetNameForResponseField(response, responseField, services);
            return CamelCaseMemberNames
                ? CamelCaser.Case(defaultName)
                : defaultName;
        }

        public string GetNameForRelationship(EntityMetadata entityMetadata, RelationshipMetadataBase relationshipMetadata, EntityRole? reflexiveRole, IServiceProvider services)
        {
            SetServiceCache(services);
            var defaultName = DefaultService.GetNameForRelationship(entityMetadata, relationshipMetadata, reflexiveRole, services);

            if (UseDisplayNameForBpfName
                && relationshipMetadata is OneToManyRelationshipMetadata oneToMany)
            {
                var logicalName = oneToMany.ReferencedEntityNavigationPropertyName == defaultName
                                  && oneToMany.ReferencingEntity != "processsession"
                    ? oneToMany.ReferencingEntity
                    : oneToMany.ReferencedEntity;
                defaultName = GetBpfNameToDisplay(defaultName, logicalName);
            }

            return CamelCaseMemberNames
                ? CamelCaser.Case(defaultName)
                : defaultName;
        }

        #region Default INamingService Calls

        public string GetNameForServiceContext(IServiceProvider services)
        {
            SetServiceCache(services); 
            return DefaultService.GetNameForServiceContext(services);
        }

        public string GetNameForEntitySet(EntityMetadata entityMetadata, IServiceProvider services)
        {
            SetServiceCache(services); 
            return DefaultService.GetNameForEntitySet(entityMetadata, services);
        }

        #endregion Default INamingService Calls
    }
}
