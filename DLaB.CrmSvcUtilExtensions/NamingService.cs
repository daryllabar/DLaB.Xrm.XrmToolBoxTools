using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using DLaB.Common;
using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace DLaB.CrmSvcUtilExtensions
{
    using OptionSet.Transliteration;
    
    public class NamingService : INamingService
    {
        private string ValidCSharpNameRegEx { get; set; }
        private INamingService DefaultService { get; set; }
        private Dictionary<string, List<string>> EntityAttributeSpecifiedNames { get; set; }
        private string InvalidCSharpNamePrefix { get; }
        private string LocalOptionSetFormat { get; }
        private bool UseDeprecatedOptionSetNaming { get; }

        /// <summary>
        /// This field keeps track of options with the same name and the values that have been defined.  Internal Dictionary Key is Name + "_" + Value
        /// </summary>
        private Dictionary<OptionSetMetadataBase, Dictionary<string, bool>> OptionNameValueDuplicates { get; }

        public NamingService(INamingService defaultService)
        {
            DefaultService = defaultService;
            EntityAttributeSpecifiedNames = ConfigHelper.GetDictionaryList("EntityAttributeSpecifiedNames", false);
            OptionNameValueDuplicates = new Dictionary<OptionSetMetadataBase, Dictionary<string, bool>>();
            InvalidCSharpNamePrefix = ConfigHelper.GetAppSettingOrDefault("InvalidCSharpNamePrefix", "_");
            LocalOptionSetFormat = ConfigHelper.GetAppSettingOrDefault("LocalOptionSetFormat", "{0}_{1}");
            UseDeprecatedOptionSetNaming = ConfigHelper.GetAppSettingOrDefault("UseDeprecatedOptionSetNaming", false);
            ValidCSharpNameRegEx = ConfigHelper.GetAppSettingOrDefault("ValidCSharpNameRegEx", @"[^a-zA-Z0-9_]");
        }

        /// <summary>
        /// Provide a new implementation for finding a name for an OptionSet. If the
        /// OptionSet is not global, we want the name to be the concatenation of the Entity's
        /// name and the Attribute's name.  Otherwise, we can use the default implementation.
        /// </summary>
        public string GetNameForOptionSet(EntityMetadata entityMetadata, OptionSetMetadataBase optionSetMetadata, IServiceProvider services)
        {
            if (UseDeprecatedOptionSetNaming)
            {
                return DefaultService.GetNameForOptionSet(entityMetadata, optionSetMetadata, services);
            }
            // Ensure that the OptionSet is not global before using the custom implementation.
            if (optionSetMetadata.IsGlobal.HasValue && !optionSetMetadata.IsGlobal.Value)
            {
                // Find the attribute which uses the specified OptionSet.
                var attribute =
                    (from a in entityMetadata.Attributes
                     where a.AttributeType == AttributeTypeCode.Picklist &&
                        ((EnumAttributeMetadata)a).OptionSet.MetadataId == optionSetMetadata.MetadataId
                     select a).FirstOrDefault();

                // Check for null, since statuscode attributes on custom entities are not global, 
                // but their optionsets are not included in the attribute metadata of the entity, either.
                if (attribute == null)
                {
                    if (optionSetMetadata.OptionSetType.GetValueOrDefault() == OptionSetType.Status && DefaultService.GetNameForOptionSet(entityMetadata, optionSetMetadata, services).EndsWith("statuscode"))
                    {
                        return string.Format(LocalOptionSetFormat, DefaultService.GetNameForEntity(entityMetadata, services), "StatusCode");
                    }
                }
                else
                {
                    // Concatenate the name of the entity and the name of the attribute
                    // together to form the OptionSet name.
                    return string.Format(LocalOptionSetFormat, DefaultService.GetNameForEntity(entityMetadata, services),
                           DefaultService.GetNameForAttribute(entityMetadata, attribute, services));
                }
            }
            var name = DefaultService.GetNameForOptionSet(entityMetadata, optionSetMetadata, services);
            name = UpdateCasingForGlobalOptionSets(name, optionSetMetadata);
            return name;
        }

        private string UpdateCasingForGlobalOptionSets(string name, OptionSetMetadataBase optionSetMetadata)
        {
            switch (name.ToLower())
            {
                case "budgetstatus":
                    return "BudgetStatus";
                case "componentstate":
                    return "ComponentState";
                case "componenttype":
                    return "ComponentType";
                case "connectionrole_category":
                    return "ConnectionRole_Category";
                case "convertrule_channelactivity":
                    return "ConvertRule_ChannelActivity";
                case "dependencytype":
                    return "DependencyType";
                case "emailserverprofile_authenticationprotocol":
                    return "EmailServerProfile_AuthenticationProtocol";
                case "field_security_permission_type":
                    return "Field_Security_Permission_Type";
                case "goal_fiscalperiod":
                    return "Goal_FiscalPeriod";
                case "goal_fiscalyear":
                    return "Goal_FiscalYear";
                case "incident_caseorigincode":
                    return "Incident_CaseOriginCode";
                case "initialcommunication":
                    return "InitialCommunication";
                case "lead_salesstage":
                    return "Lead_SalesStage";
                case "metric_goaltype":
                    return "Metric_GoalType";
                case "need":
                    return "Need";
                case "opportunity_salesstage":
                    return "Opportunity_SalesStage";
                case "principalsyncattributemapping_syncdirection":
                    return "PrincipalSyncAttributeMapping_SyncDirection";
                case "processstage_category":
                    return "Processstage_Category";
                case "purchaseprocess":
                    return "PurchaseProcess";
                case "purchasetimeframe":
                    return "PurchaseTimeFrame";
                case "qooi_pricingerrorcode":
                    return "Qooi_PricingErrorCode";
                case "qooiproduct_producttype":
                    return "QooiProduct_ProductType";
                case "qooiproduct_propertiesconfigurationstatus":
                    return "QooiProduct_PropertiesConfigurationStatus";
                case "recurrencerule_monthofyear":
                    return "RecurrenceRule_MonthOfYear";
                case "servicestage":
                    return "ServiceStage";
                case "sharepoint_validationstatus":
                    return "SharePoint_ValidationStatus";
                case "sharepoint_validationstatusreason":
                    return "SharePoint_ValidationStatusReason";
                case "sharepointdocumentlocation_locationtype":
                    return "SharePointDocumentLocation_LocationType";
                case "socialactivity_postmessagetype":
                    return "SocialActivity_PostMessageType";
                case "socialprofile_community":
                    return "SocialProfile_Community";
                case "syncattributemapping_syncdirection":
                    return "SyncAttributeMapping_SyncDirection";
                case "workflow_runas":
                    return "Workflow_RunAs";
                case "workflow_stage":
                    return "Workflow_Stage";
                case "workflowlog_objecttypecode":
                    return "WorkflowLog_ObjectTypeCode";
                default:
                    return UpdateCasingForCustomGlobalOptionSets(name, optionSetMetadata);
            }
        }

        private static string UpdateCasingForCustomGlobalOptionSets(string name, OptionSetMetadataBase optionSetMetadata)
        {
            var displayName = optionSetMetadata.DisplayName?.GetLocalOrDefaultText() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(displayName))
            {
                return name;
            }

            displayName = displayName.RemoveDiacritics().Replace(" ", ""); // Remove spaces
            if (name.EndsWith(displayName.ToLower()))
            {
                name = name.Replace(displayName.ToLower(), displayName);
            }
            else if (name.Contains(displayName.ToLower()) && name.IndexOf(displayName.ToLower(), StringComparison.Ordinal) == name.LastIndexOf(displayName.ToLower(), StringComparison.Ordinal))
            {
                // Name only contains the display name, and only once, but also contains other characters.  Captialize the Display Name, and the next character
                // as long as more than one character exists: given HelloWorld, helloworldstatus => HelloWorldStatus but helloworlds => HelloWorlds
                // May need to check for plural instead... s/es/ies
                var index = name.IndexOf(displayName.ToLower(), StringComparison.Ordinal) + displayName.Length;
                name = name.Replace(displayName.ToLower(), displayName);
                if (index < name.Length - 1)
                {
                    name = name.Substring(0, index) + char.ToUpper(name[index]) + name.Substring(index + 1, name.Length - index - 1);
                }
            }
            return name;
        }

        public string GetNameForOption(OptionSetMetadataBase optionSetMetadata, OptionMetadata optionMetadata, IServiceProvider services)
        {
            var defaultName = DefaultService.GetNameForOption(optionSetMetadata, optionMetadata, services);

            defaultName = Transliterate(optionMetadata, defaultName);

            var newName = GetValidCSharpName(defaultName);
            newName = AppendValueForDuplicateOptionSetValueNames(optionSetMetadata, newName, optionMetadata.Value.GetValueOrDefault(), services);

            if (newName == defaultName)
            {
                Trace.TraceInformation("The name of this option is {0}", defaultName);
            }
            else
            {
                Trace.TraceInformation("The name of this option was {0} but has been changed to {1}", defaultName, newName);
            }

            return newName;
        }

        private static string Transliterate(OptionMetadata optionMetadata, string defaultName)
        {
            var defaultNameIsInEnglish = !string.IsNullOrEmpty(defaultName) && !defaultName.Contains("UnknownLabel");
            if (defaultNameIsInEnglish)
            {
                return defaultName;
            }

            var localizedLabel =
                optionMetadata.Label
                    .LocalizedLabels
                    .FirstOrDefault(x =>
                        TransliterationService.AvailableCodes.Value.Contains(x.LanguageCode));

            return localizedLabel == null ? 
                defaultName.RemoveDiacritics() : 
                TransliterationService.Transliterate(localizedLabel.LanguageCode, localizedLabel.Label);
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
        /// Checks to make sure that the name does not already exist for the OptionSetto be generated.
        /// </summary>
        private Dictionary<string, bool> GetDuplicateNameValues(OptionSetMetadataBase metadata, IServiceProvider services)
        {
            var nameValueDups = new Dictionary<string, bool>();
            // Look through all options, populating the namesAndValues Collection
            foreach (var option in metadata.GetOptions())
            {
                var defaultName = DefaultService.GetNameForOption(metadata, option, services);

                defaultName = Transliterate(option, defaultName);

                var name = GetValidCSharpName(defaultName);

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
        /// Allows for Specified Attribute Nmaes to be used to set the generated attribute name
        /// </summary>
        /// <param name="entityMetadata">The entity metadata.</param>
        /// <param name="attributeMetadata">The attribute metadata.</param>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public string GetNameForAttribute(EntityMetadata entityMetadata, AttributeMetadata attributeMetadata, IServiceProvider services)
        {
            List<string> specifiedNames;
            string attributeName;
            if (EntityAttributeSpecifiedNames.TryGetValue(entityMetadata.LogicalName.ToLower(), out specifiedNames) &&
                specifiedNames.Any(s => string.Equals(s, attributeMetadata.LogicalName, StringComparison.OrdinalIgnoreCase)))
            {
                attributeName = specifiedNames.First(s => string.Equals(s, attributeMetadata.LogicalName, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                attributeName = DefaultService.GetNameForAttribute(entityMetadata, attributeMetadata, services);
            }
            return attributeName;
        }

        #region Default INamingService Calls

        public string GetNameForEntity(EntityMetadata entityMetadata, IServiceProvider services) { return DefaultService.GetNameForEntity(entityMetadata, services); }
        public string GetNameForRelationship(EntityMetadata entityMetadata, RelationshipMetadataBase relationshipMetadata, EntityRole? reflexiveRole, IServiceProvider services) { return DefaultService.GetNameForRelationship(entityMetadata, relationshipMetadata, reflexiveRole, services); }
        public string GetNameForServiceContext(IServiceProvider services) { return DefaultService.GetNameForServiceContext(services); }
        public string GetNameForMessagePair(SdkMessagePair messagePair, IServiceProvider services) { return DefaultService.GetNameForMessagePair(messagePair, services); }
        public string GetNameForRequestField(SdkMessageRequest request, SdkMessageRequestField requestField, IServiceProvider services) { return DefaultService.GetNameForRequestField(request, requestField, services); }
        public string GetNameForResponseField(SdkMessageResponse response, SdkMessageResponseField responseField, IServiceProvider services) { return DefaultService.GetNameForResponseField(response, responseField, services); }
        public string GetNameForEntitySet(EntityMetadata entityMetadata, IServiceProvider services) { return DefaultService.GetNameForEntitySet(entityMetadata, services); }

        #endregion Default INamingService Calls
    }
}
