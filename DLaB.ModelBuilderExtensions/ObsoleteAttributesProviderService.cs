using Microsoft.Extensions.DependencyInjection;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DLaB.ModelBuilderExtensions
{
    public class ObsoleteAttributesProviderService : CustomServiceSettings, IObsoleteAttributesProviderService
    {
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

            var obsoleteMatches = new TextMatcher(ObsoleteTokens);
            var concurrentObsoleteAttributes = new System.Collections.Concurrent.ConcurrentBag<string>();
            var entities = serviceProvider.GetRequiredService<IMetadataProviderService>().LoadMetadata(serviceProvider).Entities;
            System.Threading.Tasks.Parallel.ForEach(entities, entity =>
            {
                foreach (var attribute in entity.Attributes.Where(a => obsoleteMatches.HasMatch(a.DisplayName!.GetLocalOrDefaultText(OptionSetLanguageCodeOverride) ?? string.Empty)))
                {
                    concurrentObsoleteAttributes.Add(entity.LogicalName + "." + attribute.LogicalName);
                }
            });

            _obsoleteAttributes = new HashSet<string>(concurrentObsoleteAttributes);
            return _obsoleteAttributes;
        }
    }

    public interface IObsoleteAttributesProviderService
    {
        HashSet<string> GetObsoleteAttributes(IServiceProvider serviceProvider);
    }
}
