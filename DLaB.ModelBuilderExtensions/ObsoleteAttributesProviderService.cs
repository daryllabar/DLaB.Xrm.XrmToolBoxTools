using Microsoft.Extensions.DependencyInjection;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DLaB.ModelBuilderExtensions
{
    public class ObsoleteAttributesProviderService : CustomServiceSettings, IObsoleteAttributesProviderService
    {
        private static readonly PropertyInfo DeprecatedVersionProperty = typeof(AttributeMetadata).GetProperty(nameof(AttributeMetadata.DeprecatedVersion))
            ?? throw new NotImplementedException("No DeprecatedVersion property for type AttributeMetadata! Unable to update obsolete metadata.");
        private static readonly MethodInfo DeprecatedVersionSetter = DeprecatedVersionProperty.GetSetMethod(true)
            ?? throw new NotImplementedException("No DeprecatedVersion setter for type AttributeMetadata! Unable to update obsolete metadata.");

        public bool ObsoleteDeprecated { get => DLaBSettings.ObsoleteDeprecated; set => DLaBSettings.ObsoleteDeprecated = value; }
        public List<string> ObsoleteTokens { get => DLaBSettings.ObsoleteTokens; set => DLaBSettings.ObsoleteTokens = value; }
        private int OptionSetLanguageCodeOverride { get => DLaBSettings.OptionSetLanguageCodeOverride; set => DLaBSettings.OptionSetLanguageCodeOverride = value; }

        private HashSet<string>? _obsoleteAttributes;

        public ObsoleteAttributesProviderService(IDictionary<string, string> parameters) : base(parameters)
        {
        }

        public ObsoleteAttributesProviderService(DLaBModelBuilderSettings? settings = null) : base(settings)
        {
        }

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
            _obsoleteAttributes = GetObsoleteAttributes(entities, ObsoleteTokens, OptionSetLanguageCodeOverride);
            return _obsoleteAttributes;
        }

        internal static HashSet<string> GetObsoleteAttributes(IEnumerable<EntityMetadata> entities, IEnumerable<string>? obsoleteTokens, int optionSetLanguageCodeOverride)
        {
            var obsoleteMatches = new TextMatcher(obsoleteTokens ?? []);
            var concurrentObsoleteAttributes = new System.Collections.Concurrent.ConcurrentBag<string>();
            System.Threading.Tasks.Parallel.ForEach(entities, entity =>
            {
                foreach (var attribute in GetMatchingObsoleteAttributes(entity, obsoleteMatches, optionSetLanguageCodeOverride))
                {
                    concurrentObsoleteAttributes.Add(entity.LogicalName + "." + attribute.LogicalName);
                }
            });

            return new HashSet<string>(concurrentObsoleteAttributes);
        }

        internal static void PopulateDeprecatedVersion(IEnumerable<EntityMetadata> entities, IEnumerable<string>? obsoleteTokens, int optionSetLanguageCodeOverride)
        {
            var obsoleteMatches = new TextMatcher(obsoleteTokens ?? []);

            foreach (var entity in entities)
            {
                foreach (var attribute in GetMatchingObsoleteAttributes(entity, obsoleteMatches, optionSetLanguageCodeOverride))
                {
                    if (attribute.DeprecatedVersion == null)
                    {
                        DeprecatedVersionSetter.Invoke(attribute, [string.Empty]);
                    }
                }
            }
        }

        private static IEnumerable<AttributeMetadata> GetMatchingObsoleteAttributes(EntityMetadata entity, TextMatcher obsoleteMatches, int optionSetLanguageCodeOverride)
        {
            return (entity.Attributes ?? [])
                .Where(a => obsoleteMatches.HasMatch(a.DisplayName?.GetLocalOrDefaultText(optionSetLanguageCodeOverride) ?? string.Empty));
        }
    }

    public interface IObsoleteAttributesProviderService
    {
        HashSet<string> GetObsoleteAttributes(IServiceProvider serviceProvider);
    }
}
