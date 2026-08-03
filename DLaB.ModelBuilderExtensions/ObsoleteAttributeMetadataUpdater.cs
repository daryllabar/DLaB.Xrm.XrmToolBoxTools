using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DLaB.ModelBuilderExtensions
{
    internal static class ObsoleteAttributeMetadataUpdater
    {
        private static readonly PropertyInfo DeprecatedVersionProperty = typeof(AttributeMetadata).GetProperty(nameof(AttributeMetadata.DeprecatedVersion))
            ?? throw new NotImplementedException("No DeprecatedVersion property for type AttributeMetadata! Unable to update obsolete metadata.");
        private static readonly MethodInfo DeprecatedVersionSetter = DeprecatedVersionProperty.GetSetMethod(true)
            ?? throw new NotImplementedException("No DeprecatedVersion setter for type AttributeMetadata! Unable to update obsolete metadata.");

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
}
