using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;
using DLaB.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DLaB.ModelBuilderExtensions
{
    public class MetadataQueryProviderService : TypedServiceBase<IMetadataProviderQueryService>, IMetadataProviderQueryService
    {
        public List<string> EntityNamesFilter { get => Settings.EntityNamesFilter; set => Settings.EntityNamesFilter = value; }

        public MetadataQueryProviderService(IMetadataProviderQueryService defaultService, IDictionary<string, string> parameters) : base(defaultService, parameters)
        {
            
        }

        public MetadataQueryProviderService(IMetadataProviderQueryService defaultService, DLaBModelBuilderSettings settings = null) : base(defaultService, settings)
        {

        }

        public EntityMetadata[] RetrieveEntities(IOrganizationService service)
        {
            return EntityNamesFilter.Any(l => l.Contains('*'))
                ? RetrieveEntitiesWithByEntityNamesCombinedWithWildCardSearchResults(service)
                  // No Prefix * required, use default
                : DefaultService.RetrieveEntities(service);
        }

        private EntityMetadata[] RetrieveEntitiesWithByEntityNamesCombinedWithWildCardSearchResults(IOrganizationService service)
        {
            var entityNames = EntityNamesFilter.Where(l => !l.Contains('*') && !l.Contains('%')).ToList();
            entityNames = AddAndOrderEntitiesFromWildCardSearch(service, entityNames);

            var filterExpression = new MetadataFilterExpression(LogicalOperator.Or);
            foreach (var nameBatch in entityNames.Batch(50))
            {
                filterExpression.Conditions.Add(new MetadataConditionExpression("logicalname", MetadataConditionOperator.In, nameBatch.ToArray()));
            }

            return ((RetrieveMetadataChangesResponse)service.Execute(new RetrieveMetadataChangesRequest
            {
                Query = new EntityQueryExpression
                {
                    Criteria = filterExpression,
                    Properties = new MetadataPropertiesExpression
                    {
                        AllProperties = true
                    }
                }
            })).EntityMetadata.ToArray();
        }

        private List<string> AddAndOrderEntitiesFromWildCardSearch(IOrganizationService service, List<string> entityNames)
        {
            var condition = new ConditionExpression("logicalname", ConditionOperator.Like);
            var searchQe = new QueryExpression("entity")
            {
                ColumnSet = new ColumnSet("logicalname"),
                Criteria = new FilterExpression()
            };
            searchQe.Criteria.Conditions.Add(condition);

            foreach (var wildCardName in EntityNamesFilter.Where(l => l.Contains("*") || l.Contains('%')))
            {
                condition.Values.Clear();
                condition.Values.Add(wildCardName.Replace('*', '%'));
                entityNames.AddRange(service.RetrieveMultiple(searchQe).Entities.Select(e => e.GetAttributeValue<string>("logicalname")));
            }

            return entityNames.OrderBy(e => e).Distinct().ToList();
        }
        public OptionSetMetadataBase[] RetrieveOptionSets(IOrganizationService service)
        {
            return DefaultService.RetrieveOptionSets(service);
        }

        // MS is including UpsertMultiple by default.  Not sure why.  Removing it if it's not in the filter.
        private static readonly List<string> _msForcedMessages = new List<string>
        {
            "CreateMultiple",
            "DeleteMultiple",
            "UpdateMultiple",
            "UpsertMultiple",
        };

        public SdkMessages RetrieveSdkRequests(IOrganizationService service)
        {
            var messages = DefaultService.RetrieveSdkRequests(service);

            foreach (var message in _msForcedMessages)
            {
                var forcedMessage = messages.MessageCollection.Values.FirstOrDefault(v => v.Name == message);
                if (forcedMessage != null
                    && (
                        !Settings.MessageNamesFilter.Any(WildcardMatches)
                        || DLaBSettings.MessageBlacklist.Any(WildcardMatches)
                    ))
                {
                    messages.MessageCollection.Remove(forcedMessage.Id);
                }
            }


            return messages;

            bool WildcardMatches(string f)
            {
                return Regex.IsMatch("UpsertMultiple", f.Replace('%', '*'), RegexOptions.IgnoreCase);
            }
        }
    }
}
