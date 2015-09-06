using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk;
using System.Collections.Concurrent;
using Microsoft.Xrm.Sdk.Messages;
using DLaB.Xrm.Common;

namespace DLaB.Xrm
{
    public static partial class Extensions
    {
        private static ConcurrentDictionary<string, OptionMetadataCollection> _optionSets = new ConcurrentDictionary<string, OptionMetadataCollection>();

        /// <summary>
        /// Returns the OptionMetadataCollection for an entity attribute option set
        /// </summary>
        /// <param name="settings">The CrmService settings used to create a connection to the server if the information isn't already cached.</param>
        /// <param name="entityLogicalName">Name of the entity logical.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns></returns>
        public static OptionMetadataCollection GetEntityOptionSet(this CrmService settings, string entityLogicalName, string attributeName)
        {
            string key = GetEntityOptionSetKey(GetServiceKey(settings), entityLogicalName, attributeName);

            OptionMetadataCollection options;

            if (_optionSets.TryGetValue(key, out options))
            {
                return options;
            }
            else
            {
                using (var service = CrmService.CreateOrgProxy(settings))
                {
                    return GetEntityOptionSet(service, entityLogicalName, attributeName);
                }
            }
        }

        /// <summary>
        /// Returns the OptionMetadataCollection for an entity attribute option set.  Unless you already have an IOrganizationService, use the CrmService's GetEntityOptionSet method since it won't create a new connection to the server if the value is already cached.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityLogicalName">Logical name of the entity.</param>
        /// <param name="attributeName">Attribute name of the entity.</param>
        /// <returns></returns>
        public static OptionMetadataCollection GetEntityOptionSet(this IOrganizationService service, string entityLogicalName, string attributeName)
        {
            string key = GetEntityOptionSetKey(service.GetOrganizationKey(), entityLogicalName, attributeName);

            OptionMetadataCollection options;

            if (!_optionSets.TryGetValue(key, out options))
            {
                RetrieveAttributeRequest attributeRequest = new RetrieveAttributeRequest
                {
                    EntityLogicalName = entityLogicalName,
                    LogicalName = attributeName,
                    RetrieveAsIfPublished = true
                };

                var response = (RetrieveAttributeResponse)service.Execute(attributeRequest);
                options = ((EnumAttributeMetadata)response.AttributeMetadata).OptionSet.Options;

                _optionSets.AddOrUpdate(key, options, (k, o) => o); // If the value already exists, just use the old value...
            }

            return options;
        }

        #region Key Helpers
        
        private static string GetEntityOptionSetKey(string serviceKey, string entityLogicalName, string attributeName)
        {
            return string.Format("{0}EntityOptionSet|{1}|{2}", serviceKey, entityLogicalName, attributeName);
        }

        private static string GetOptionSetKey(string serviceKey, string optionSetName)
        {
            return serviceKey + "GlobalOptionSet|" + optionSetName;
        }

        private static string GetServiceKey(CrmService settings)
        {
            int start = settings.CrmServerUrl.IndexOf(@"//") + 2; // @"//".Length
            int end = settings.CrmServerUrl.LastIndexOf('/');

            return String.Format("{0}|{1}|",
                settings.CrmServerUrl.Substring(start, end - start),
                settings.CrmOrganization);
        }

        #endregion // Key Helpers

        /// <summary>
        /// Returns the OptionMetadataCollection for a global option set.  Unless you already have an IOrganizationService, use the CrmService's GetOptionSet method since it won't create a new connection to the server if the value is already cached.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="optionSetName">Name of the option set.</param>
        /// <returns></returns>
        public static OptionMetadataCollection GetOptionSet(this IOrganizationService service, string optionSetName)
        {
            string key = GetOptionSetKey(service.GetOrganizationKey(), optionSetName);
            OptionMetadataCollection options;

            if (!_optionSets.TryGetValue(key, out options))
            {
                var response = (RetrieveOptionSetResponse)service.Execute(new RetrieveOptionSetRequest { Name = optionSetName });
                options = ((OptionSetMetadata)response.OptionSetMetadata).Options;

                _optionSets.AddOrUpdate(key, options, (k, o) => o); // If the value already exists, just use the old value...
            }

            return options;
        }

        /// <summary>
        /// Returns the OptionMetadataCollection for a global option set
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="optionSetName">Name of the option set.</param>
        /// <returns></returns>
        public static OptionMetadataCollection GetOptionSet(this CrmService settings, string optionSetName)
        {
            string key = GetOptionSetKey(GetServiceKey(settings), optionSetName);
            OptionMetadataCollection options;

            if (_optionSets.TryGetValue(key, out options))
            {
                return options;
            }
            else
            {
                using (var service = CrmService.CreateOrgProxy(settings))
                {
                    return GetOptionSet(service, optionSetName);
                }
            }
        }

        /// <summary>
        /// Gets the UserLocalizedLabel Of the Option
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string GetText(this OptionMetadata value)
        {
            return value.Label.UserLocalizedLabel.Label;
        }

        /// <summary>
        /// Gets the entity option meta data for a single OptionSetValue, for the specified entity's OptionSet, based on the text value of the OptionSetValue.  Unless you already have an IOrganizationService, use the CrmService's GetEntityOption method since it won't create a new connection to the server if the value is already cached.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityName">Logical name of the entity.</param>
        /// <param name="attributeName">Name of the attribute that stores the option set.</param>
        /// <param name="optionTextValue">The option set value's text value.</param>
        /// <returns></returns>
        public static OptionMetadata GetEntityOption(this IOrganizationService service, string entityName, string attributeName, string optionTextValue)
        {
            var options = GetEntityOptionSet(service, entityName, attributeName);
            return options.FirstOrDefault(v => String.Equals(v.GetText(), optionTextValue, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// Gets the entity option meta data for a single OptionSetValue, for the specified entity's OptionSet, based on the text value of the OptionSetValue.
        /// </summary>
        /// <param name="settings">The CrmService settings.</param>
        /// <param name="entityName">Logical name of the entity.</param>
        /// <param name="attributeName">Name of the attribute that stores the option set.</param>
        /// <param name="optionTextValue">The option set value's text value.</param>
        /// <returns></returns>
        public static OptionMetadata GetEntityOption(this CrmService settings, string entityName, string attributeName, string optionTextValue)
        {
            var options = GetEntityOptionSet(settings, entityName, attributeName);
            return options.FirstOrDefault(v => String.Equals(v.GetText(), optionTextValue, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// Gets the entity option meta data for a single OptionSetValue, for the specified entity's OptionSet, based on the int value of the OptionSetValue.  Unless you already have an IOrganizationService, use the CrmService's GetEntityOption method since it won't create a new connection to the server if the value is already cached.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityName">Logical name of the entity.</param>
        /// <param name="attributeName">Name of the attribute that stores the option set.</param>
        /// <param name="value">The option set value's int value.</param>
        /// <returns></returns>
        public static OptionMetadata GetEntityOption(this IOrganizationService service, string entityName, string attributeName, int value)
        {
            var options = GetEntityOptionSet(service, entityName, attributeName);
            return options.FirstOrDefault(v => v.Value == value);
        }

        /// <summary>
        /// Gets the entity option meta data for a single OptionSetValue, for the specified entity's OptionSet, based on the int value of the OptionSetValue.
        /// </summary>
        /// <param name="settings">The CrmService settings.</param>
        /// <param name="entityName">Logical name of the entity.</param>
        /// <param name="attributeName">Name of the attribute that stores the option set.</param>
        /// <param name="value">The option set value's int value.</param>
        /// <returns></returns>
        public static OptionMetadata GetEntityOption(this CrmService settings, string entityName, string attributeName, int value)
        {
            var options = GetEntityOptionSet(settings, entityName, attributeName);
            return options.FirstOrDefault(v => v.Value == value);
        }

        /// <summary>
        /// Gets the entity's option set value's text value based on its int value.  Unless you already have an IOrganizationService, use the CrmService's GetEntityOptionText method since it won't create a new connection to the server if the value is already cached.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityName">Logical name of the entity.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="value">The int value of the OptionSetValue.</param>
        /// <returns></returns>
        public static string GetEntityOptionText(this IOrganizationService service, string entityName, string attributeName, int value)
        {
            return service.GetEntityOption(entityName, attributeName, value).GetText();
        }

        /// <summary>
        /// Gets the entity's option set value's text value based on its int value.
        /// </summary>
        /// <param name="settings">The CrmService settings.</param>
        /// <param name="entityName">Logical name of the entity.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="value">The int value of the OptionSetValue.</param>
        /// <returns></returns>
        public static string GetEntityOptionText(this CrmService settings, string entityName, string attributeName, int value)
        {
            return settings.GetEntityOption(entityName, attributeName, value).GetText();
        }

        /// <summary>
        /// Gets the entity's option set value's int value based on its text value.  Unless you already have an IOrganizationService, use the CrmService's GetEntityOptionValue method since it won't create a new connection to the server if the value is already cached.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityName">Logical name of the entity.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="optionTextValue">The text value of the OptionSetValue.</param>
        /// <returns></returns>
        public static int GetEntityOptionValue(this IOrganizationService service, string entityName, string attributeName, string optionTextValue)
        {
            return service.GetEntityOption(entityName, attributeName, optionTextValue).Value.GetValueOrDefault();
        }

        /// <summary>
        /// Gets the entity's option set value's int value based on its text value.
        /// </summary>
        /// <param name="settings">The CrmService settings.</param>
        /// <param name="entityName">Logical name of the entity.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="optionTextValue">The text value of the OptionSetValue.</param>
        /// <returns></returns>
        public static int GetEntityOptionValue(this CrmService settings, string entityName, string attributeName, string optionTextValue)
        {
            return settings.GetEntityOption(entityName, attributeName, optionTextValue).Value.GetValueOrDefault();
        }

        /// <summary>
        /// Gets an option set meta data value based on its text value.  Unless you already have an IOrganizationService, use the CrmService's GetOption method since it won't create a new connection to the server if the value is already cached.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="optionSetName">Name of the option set.</param>
        /// <param name="optionTextValue">The text value of the OptionSetValue.</param>
        /// <returns></returns>
        public static OptionMetadata GetOption(this IOrganizationService service, string optionSetName, string optionTextValue)
        {
            var options = GetOptionSet(service, optionSetName);
            return options.FirstOrDefault(v => String.Equals(v.GetText(), optionTextValue, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// Gets an option set meta data value based on its text value.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="optionSetName">Name of the option set.</param>
        /// <param name="optionTextValue">The text value of the OptionSetValue.</param>
        /// <returns></returns>
        public static OptionMetadata GetOption(this CrmService settings, string optionSetName, string optionTextValue)
        {
            var options = GetOptionSet(settings, optionSetName);
            return options.FirstOrDefault(v => String.Equals(v.GetText(), optionTextValue, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// Gets an option set meta data value based on its int value.    Unless you already have an IOrganizationService, use the CrmService's GetOption method since it won't create a new connection to the server if the value is already cached.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="optionSetName">Name of the option set.</param>
        /// <param name="value">The int value of the OptionSetValue.</param>
        /// <returns></returns>
        public static OptionMetadata GetOption(this IOrganizationService service, string optionSetName, int value)
        {
            var options = GetOptionSet(service, optionSetName);
            return options.FirstOrDefault(v => v.Value == value);
        }

        /// <summary>
        /// Gets an option set meta data value based on its int value.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="optionSetName">Name of the option set.</param>
        /// <param name="value">The int value of the OptionSetValue.</param>
        /// <returns></returns>
        public static OptionMetadata GetOption(this CrmService settings, string optionSetName, int value)
        {
            var options = GetOptionSet(settings, optionSetName);
            return options.FirstOrDefault(v => v.Value == value);
        }

        /// <summary>
        /// Gets an option set value's text value based on its int value.  Unless you already have an IOrganizationService, use the CrmService's GetOptionText method since it won't create a new connection to the server if the value is already cached.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="optionSetName">Name of the option set.</param>
        /// <param name="value">The int value of the OptionSetValue.</param>
        /// <returns></returns>
        public static string GetOptionText(this IOrganizationService service, string optionSetName, int value)
        {
            return service.GetOption(optionSetName, value).GetText();
        }

        /// <summary>
        /// Gets an option set value's text value based on its int value.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="optionSetName">Name of the option set.</param>
        /// <param name="value">The int value of the OptionSetValue.</param>
        /// <returns></returns>
        public static string GetOptionText(this CrmService settings, string optionSetName, int value)
        {
            return settings.GetOption(optionSetName, value).GetText();
        }

        /// <summary>
        /// Gets an option set value's int value based on its text value.  Unless you already have an IOrganizationService, use the CrmService's GetOptionValue method since it won't create a new connection to the server if the value is already cached.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="optionSetName">Name of the option set.</param>
        /// <param name="optionTextValue">The text value of the OptionSetValue.</param>
        /// <returns></returns>
        public static int GetOptionValue(this IOrganizationService service, string optionSetName, string optionTextValue)
        {
            return service.GetOption(optionSetName, optionTextValue).Value.GetValueOrDefault();
        }

        /// <summary>
        /// Gets an option set value's int value based on its text value.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="optionSetName">Name of the option set.</param>
        /// <param name="optionTextValue">The text value of the OptionSetValue.</param>
        /// <returns></returns>
        public static int GetOptionValue(this CrmService settings, string optionSetName, string optionTextValue)
        {
            return settings.GetOption(optionSetName, optionTextValue).Value.GetValueOrDefault();
        }
    }
}
