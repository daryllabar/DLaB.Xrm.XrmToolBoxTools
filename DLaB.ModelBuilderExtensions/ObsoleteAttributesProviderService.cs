using Microsoft.Extensions.DependencyInjection;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DLaB.ModelBuilderExtensions
{
    /// <summary>
    /// Defines a service for retrieving and populating obsolete/deprecated attribute information
    /// from Dynamics 365/Dataverse metadata.
    /// </summary>
    public class ObsoleteAttributesProviderService : CustomServiceSettings, IObsoleteAttributesProviderService
    {
        private static readonly PropertyInfo DeprecatedVersionProperty = typeof(AttributeMetadata).GetProperty(nameof(AttributeMetadata.DeprecatedVersion))
            ?? throw new NotImplementedException("No DeprecatedVersion property for type AttributeMetadata! Unable to update obsolete metadata.");
        private static readonly MethodInfo DeprecatedVersionSetter = DeprecatedVersionProperty.GetSetMethod(true)
            ?? throw new NotImplementedException("No DeprecatedVersion setter for type AttributeMetadata! Unable to update obsolete metadata.");

        public bool ObsoleteDeprecated { get => DLaBSettings.ObsoleteDeprecated; set => DLaBSettings.ObsoleteDeprecated = value; }
        public List<string> ObsoleteTokens { get => DLaBSettings.ObsoleteTokens; set => DLaBSettings.ObsoleteTokens = value; }
        private int OptionSetLanguageCodeOverride { get => DLaBSettings.OptionSetLanguageCodeOverride; set => DLaBSettings.OptionSetLanguageCodeOverride = value; }

        private bool _deprecatedVersionsPopulated;
        private HashSet<string>? _obsoleteAttributes;

        public ObsoleteAttributesProviderService(IDictionary<string, string> parameters) : base(parameters)
        {
        }

        public ObsoleteAttributesProviderService(DLaBModelBuilderSettings? settings = null) : base(settings)
        {
        }

        /// <summary>
        /// Retrieves the set of logical names for attributes that are considered obsolete.
        /// </summary>
        /// <param name="serviceProvider">The service provider used to access metadata and other services.</param>
        /// <returns>A <see cref="HashSet{T}"/> of logical attribute names that are obsolete.</returns>
        public HashSet<string> GetObsoleteAttributes(IServiceProvider serviceProvider)
        {
            if (!ObsoleteDeprecated)
            {
                return [];
            }

            if (_obsoleteAttributes != null)
            {
                return _obsoleteAttributes;
            }

            var entities = serviceProvider.GetRequiredService<IMetadataProviderService>().LoadMetadata(serviceProvider).Entities;
            if (!_deprecatedVersionsPopulated)
            {
                PopulateDeprecatedVersion(entities);
            }

            _obsoleteAttributes = GetObsoleteAttributes(entities);
            return _obsoleteAttributes;
        }

        private HashSet<string> GetObsoleteAttributes(IEnumerable<EntityMetadata> entities)
        {
            return new HashSet<string>(
                from entity in entities
                from attribute in entity.Attributes ?? []
                where attribute.DeprecatedVersion != null
                select entity.LogicalName + "." + attribute.LogicalName);
        }

        /// <summary>
        /// Populates the deprecated version information on attributes within the provided entity metadata.
        /// </summary>
        /// <param name="entities">The collection of <see cref="EntityMetadata"/> whose attributes will be updated with deprecation version info.</param>
        public void PopulateDeprecatedVersion(IEnumerable<EntityMetadata> entities)
        {
            if (!ObsoleteDeprecated)
            {
                return;
            }

            var obsoleteMatches = new TextMatcher(ObsoleteTokens);

            foreach (var entity in entities)
            {
                foreach (var attribute in (entity.Attributes ?? [])
                         .Where(a => obsoleteMatches.HasMatch(a.DisplayName?.GetLocalOrDefaultText(OptionSetLanguageCodeOverride) ?? string.Empty)))
                {
                    if (attribute.DeprecatedVersion == null)
                    {
                        DeprecatedVersionSetter.Invoke(attribute, [string.Empty]);
                    }
                }
            }

            _deprecatedVersionsPopulated = true;
        }
    }

    /// <summary>
    /// Defines a service for retrieving and populating obsolete/deprecated attribute information
    /// from Dynamics 365/Dataverse metadata.
    /// </summary>
    public interface IObsoleteAttributesProviderService
    {
        /// <summary>
        /// Retrieves the set of logical names for attributes that are considered obsolete.
        /// </summary>
        /// <param name="serviceProvider">The service provider used to access metadata and other services.</param>
        /// <returns>A <see cref="HashSet{T}"/> of logical attribute names that are obsolete.</returns>
        HashSet<string> GetObsoleteAttributes(IServiceProvider serviceProvider);

        /// <summary>
        /// Populates the deprecated version information on attributes within the provided entity metadata.
        /// </summary>
        /// <param name="entities">The collection of <see cref="EntityMetadata"/> whose attributes will be updated with deprecation version info.</param>
        void PopulateDeprecatedVersion(IEnumerable<EntityMetadata> entities);
    }
}
