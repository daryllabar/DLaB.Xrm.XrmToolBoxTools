using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace DLaB.CrmSvcUtilExtensions
{
    using OptionSet.Transliteration;
    
    public class NamingService : INamingService
    {
        public static bool UseLogicalNames => ConfigHelper.GetNonNullableAppSettingOrDefault("UseLogicalNames", false);
        public static bool CamelCaseClassNames => ConfigHelper.GetNonNullableAppSettingOrDefault("CamelCaseClassNames", false);
        public static bool CamelCaseMemberNames => ConfigHelper.GetNonNullableAppSettingOrDefault("CamelCaseMemberNames", false);
        public static int LanguageCodeOverride => ConfigHelper.GetNonNullableAppSettingOrDefault("OptionSetLanguageCodeOverride", -1);
        public Dictionary<string, string> OptionSetNames { get; set; }
        private const int English = 1033;  
        private string ValidCSharpNameRegEx { get; set; }
        private INamingService DefaultService { get; set; }
        private Dictionary<string, HashSet<string>> EntityAttributeSpecifiedNames { get; set; }
        private string InvalidCSharpNamePrefix { get; }
        private string LocalOptionSetFormat { get; }
        private bool UseDeprecatedOptionSetNaming { get; }
        private static HashSet<string> _entityNames;

        public HashSet<string> EntityNames => _entityNames ?? (_entityNames = GenerateEntityNames());

        /// <summary>
        /// This field keeps track of options with the same name and the values that have been defined.  Internal Dictionary Key is Name + "_" + Value
        /// </summary>
        private Dictionary<OptionSetMetadataBase, Dictionary<string, bool>> OptionNameValueDuplicates { get; }

        public NamingService(INamingService defaultService)
        {
            DefaultService = defaultService;
            OptionSetNames = ConfigHelper.GetDictionary("OptionSetNames", false);
            EntityAttributeSpecifiedNames = ConfigHelper.GetDictionaryHash("EntityAttributeSpecifiedNames", false);
            OptionNameValueDuplicates = new Dictionary<OptionSetMetadataBase, Dictionary<string, bool>>();
            InvalidCSharpNamePrefix = ConfigHelper.GetAppSettingOrDefault("InvalidCSharpNamePrefix", "_");
            LocalOptionSetFormat = ConfigHelper.GetAppSettingOrDefault("LocalOptionSetFormat", "{0}_{1}");
            UseDeprecatedOptionSetNaming = ConfigHelper.GetAppSettingOrDefault("UseDeprecatedOptionSetNaming", false);
            ValidCSharpNameRegEx = ConfigHelper.GetAppSettingOrDefault("ValidCSharpNameRegEx", @"[^a-zA-Z0-9_]");
        }

        private HashSet<string> GenerateEntityNames()
        {
            var metadata = BaseCustomCodeGenerationService.Metadata;
            if (metadata == null)
            {
                throw new Exception("DLaB.CrmSvcUtilExtensions.BaseCustomCodeGenerationService hasn't been called!");
            }
            return new HashSet<string>(metadata.Entities.Select(e => GetNameForEntity(e, BaseCustomCodeGenerationService.ServiceProvider)).ToArray());
        }

        /// <summary>
        /// Provide a new implementation for finding a name for an OptionSet. If the
        /// OptionSet is not global, we want the name to be the concatenation of the Entity's
        /// name and the Attribute's name.  Otherwise, we can use the default implementation.
        /// </summary>
        public string GetNameForOptionSet(EntityMetadata entityMetadata, OptionSetMetadataBase optionSetMetadata, IServiceProvider services)
        {
            var name = GetPossiblyConflictedNameForOptionSet(entityMetadata, optionSetMetadata, services);
            while (EntityNames.Contains(name))
            {
                name += "_Enum";
            }
            return OptionSetNames.TryGetValue(name.ToLower(), out var overriden) ? overriden : name;
        }

        private string GetPossiblyConflictedNameForOptionSet(EntityMetadata entityMetadata, OptionSetMetadataBase optionSetMetadata, IServiceProvider services)
        {
            var defaultName = DefaultService.GetNameForOptionSet(entityMetadata, optionSetMetadata, services);

            if (UseDeprecatedOptionSetNaming)
            {
                return defaultName;
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

        private static Dictionary<string,string> CasingByGlobalOptionSet = new Dictionary<string, string>{
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

        private static string UpdateCasingForCustomGlobalOptionSets(string name, OptionSetMetadataBase optionSetMetadata)
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
            var possiblyDuplicateName = GetPossiblyDuplicateNameForOption(optionSetMetadata, services, optionMetadata);
            return AppendValueForDuplicateOptionSetValueNames(optionSetMetadata, possiblyDuplicateName, optionMetadata.Value.GetValueOrDefault(), services);
        }

        private static string Transliterate(OptionMetadata optionMetadata, string englishName)
        {
            var localizedLabels = optionMetadata.Label.LocalizedLabels;
            if (LanguageCodeOverride < 0 || LanguageCodeOverride == English)
            {
                if (IsLabelPopulated(englishName))
                {
                    return englishName.RemoveDiacritics();
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
        /// Checks to make sure that the name begins with a valid character. If the name does not begin with a valid character, then add an underscore to the beginning of the name.
        /// </summary>
        private string GetValidCSharpName(string name)
        {
            //remove spaces and special characters
            name = Regex.Replace(name, ValidCSharpNameRegEx, string.Empty);
            if (name.Length > 0 && !char.IsLetter(name, 0))
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
            var defaultName = DefaultService.GetNameForOption(metadata, option, services);
            defaultName = Transliterate(option, defaultName);

            var name = GetValidCSharpName(defaultName);
            if (defaultName == string.Empty)
            {
                name = name + option.Value;
            }
            return name;
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
            Dictionary<string, bool> duplicateNameValues;
            if (!OptionNameValueDuplicates.TryGetValue(optionSetMetadata, out duplicateNameValues))
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
            try
            {

                var defaultName = DefaultService.GetNameForEntity(entityMetadata, services);
                return CamelCaseClassNames
                    ? CamelCaser.Case(defaultName)
                    : defaultName;
            }
            catch (System.Reflection.ReflectionTypeLoadException ex)
            {
                ex.LoaderExceptions.ToList().ForEach(e => Console.WriteLine(e.Message));
                throw ex.LoaderExceptions.First();
            }
        }

        public string GetNameForRequestField(SdkMessageRequest request, SdkMessageRequestField requestField, IServiceProvider services)
        {
            var defaultName = DefaultService.GetNameForRequestField(request, requestField, services);
            return CamelCaseMemberNames
                ? CamelCaser.Case(defaultName)
                : defaultName;
        }

        public string GetNameForResponseField(SdkMessageResponse response, SdkMessageResponseField responseField, IServiceProvider services)
        {
            var defaultName = DefaultService.GetNameForResponseField(response, responseField, services);
            return CamelCaseMemberNames
                ? CamelCaser.Case(defaultName)
                : defaultName;
        }

        public string GetNameForRelationship(EntityMetadata entityMetadata, RelationshipMetadataBase relationshipMetadata, EntityRole? reflexiveRole, IServiceProvider services)
        {
            var defaultName = DefaultService.GetNameForRelationship(entityMetadata, relationshipMetadata, reflexiveRole, services);
            return CamelCaseMemberNames
                ? CamelCaser.Case(defaultName)
                : defaultName;
        }

        #region Default INamingService Calls

        public string GetNameForServiceContext(IServiceProvider services) { return DefaultService.GetNameForServiceContext(services); }
        public string GetNameForMessagePair(SdkMessagePair messagePair, IServiceProvider services) { return DefaultService.GetNameForMessagePair(messagePair, services); }
        public string GetNameForEntitySet(EntityMetadata entityMetadata, IServiceProvider services) { return DefaultService.GetNameForEntitySet(entityMetadata, services); }

        #endregion Default INamingService Calls
    }
}
