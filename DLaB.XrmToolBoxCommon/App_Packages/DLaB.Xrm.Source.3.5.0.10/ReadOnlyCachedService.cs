using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.Runtime.Caching;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm
#else
namespace Source.DLaB.Xrm
#endif
{
    /// <summary>
    /// An IOrganizationService that caches the results of queries
    /// </summary>
#if !DLAB_XRM_DEBUG
    [System.Diagnostics.DebuggerNonUserCode]
#endif
    public class ReadOnlyCachedService: IOrganizationService
    {

        private readonly IOrganizationService _service;

        /// <summary>
        /// Default Prefix for all keys used in the Cache
        /// </summary>
        protected const string DefaultKeyPrefix = "{7041D6DF-13AE-4101-934C-BBB5D9409851}-";
        /// <summary>
        /// Separator for key and value of request Parameter
        /// </summary>
        protected const char DefaultRequestParamKeySeparator = '~';
        /// <summary>
        /// Separator for kvps of request parameters
        /// </summary>
        protected const string DefaultRequestParamKvpSeparator = "|";
        
        /// <summary>
        /// Prefix for all keys used in the Cache.  Defaults to the DefaultKeyPrefix
        /// </summary>
        protected virtual string KeyPrefix => DefaultKeyPrefix;
        /// <summary>
        /// Separator for key and value of request Parameter
        /// </summary>
        protected virtual char ParamKeySeparator => DefaultRequestParamKeySeparator;
        /// <summary>
        /// Separator for kvps of request parameters
        /// </summary>
        protected virtual string ParamKvpSeparator => DefaultRequestParamKvpSeparator;

        private MemoryCache _cache;
        /// <summary>
        /// Cache that is used to store the results of requests
        /// </summary>
        public MemoryCache Cache => _cache ?? (_cache = GetCache());

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">The Service</param>
        public ReadOnlyCachedService(IOrganizationService service)
        {
            _service = service;
        }

        /// <summary>
        /// The Cache to be used by the service.  Defaults to the DLaBXrmConfig.CacheConfig.GetCache().
        /// </summary>
        protected virtual MemoryCache GetCache()
        {
            return DLaBXrmConfig.CacheConfig.GetCache();
        }

        /// <summary>
        /// The ExpirationTime to use.  Defaults to two hours into the future
        /// </summary>
        /// <typeparam name="T">The Value Type</typeparam>
        /// <param name="key">The Key</param>
        /// <param name="value">The value</param>
        /// <returns></returns>
        protected virtual DateTime GetExpirationTime<T>(string key, T value)
        {
            return DateTime.UtcNow.AddHours(2);
        }

        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <param name="entity">The Entity</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual Guid Create(Entity entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the entity from the cache or the system if not in the cache
        /// </summary>
        /// <param name="entityName">The Entity Name</param>
        /// <param name="id">The Id</param>
        /// <param name="columnSet">The ColumnSet</param>
        /// <returns></returns>
        public virtual Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            return Cache.GetOrAdd(KeyPrefix + entityName + "|" + id,
                k => _service.Retrieve(entityName, id, new ColumnSet(true)),
                GetExpirationTime).Clone(true);
        }

        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <param name="entity">The Entity</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void Update(Entity entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <param name="entityName">The EntityName</param>
        /// <param name="id">The Id</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void Delete(string entityName, Guid id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implements the Cache of Requests
        /// </summary>
        /// <param name="request">The Request</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual OrganizationResponse Execute(OrganizationRequest request)
        {
            switch (request)
            {
                case RetrieveRequest retrieve:
                {
                        var response = new RetrieveResponse();
                        response[nameof(response.Entity)] = Retrieve(retrieve.Target.LogicalName, retrieve.Target.Id, retrieve.ColumnSet);
                        return response;
                }
                case RetrieveMultipleRequest retrieveMultiple:
                {
                        var response = new RetrieveMultipleResponse();
                        response[nameof(response.EntityCollection)] = RetrieveMultiple(retrieveMultiple.Query);
                        return response;
                }
                default:
                    if (!request.RequestName.StartsWith("Retrieve"))
                    {
                        throw new NotImplementedException("ReadOnlyCachedService by default only supports messages that \"Retrieve\" message requests.");
                    }

                    var key = GetKey(request);
                    return Cache.GetOrAdd(KeyPrefix + key,
                        k => _service.Execute(request),
                        GetExpirationTime);
            }
        }

        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <param name="entityName">The Entity Name</param>
        /// <param name="entityId">The Entity Id</param>
        /// <param name="relationship">The Relationship</param>
        /// <param name="relatedEntities">The Related Entities</param>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <param name="entityName">The Entity Name</param>
        /// <param name="entityId">The Entity Id</param>
        /// <param name="relationship">The Relationship</param>
        /// <param name="relatedEntities">The Related Entities</param>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the entities from the cache or the system if not in the cache
        /// </summary>
        /// <param name="query">The Query</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual EntityCollection RetrieveMultiple(QueryBase query)
        {
            var key = GetKey(query);
            return Cache.GetOrAdd(KeyPrefix + key,
                k => _service.RetrieveMultiple(query),
                GetExpirationTime).Clone();
        }

        /// <summary>
        /// Retrieves the entities from the cache or the system if not in the cache
        /// </summary>
        /// <param name="request">The Request</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected virtual RetrieveEntityResponse RetrieveEntity(RetrieveEntityRequest request)
        {
            var key = GetKey(request);
            return (RetrieveEntityResponse) Cache.GetOrAdd(KeyPrefix + key,
                k => _service.Execute(request),
                GetExpirationTime);
        }

        /// <summary>
        /// Retrieves the attribute from the cache or the system if not in the cache
        /// </summary>
        /// <param name="request">The Request</param>
        /// <returns></returns>
        protected virtual RetrieveAttributeResponse RetrieveAttribute(RetrieveAttributeRequest request)
        {
            var key = GetKey(request);
            return (RetrieveAttributeResponse)Cache.GetOrAdd(KeyPrefix + key,
                               k => _service.Execute(request),
                                              GetExpirationTime);
        }

        /// <summary>
        /// Gets a cache key for the specified query.
        /// </summary>
        /// <param name="query">The query to get a cache key for.</param>
        /// <returns>A cache key for the specified query.</returns>
        protected virtual string GetKey(QueryBase query)
        {
            switch (query)
            {
                case QueryExpression qe:
                    qe.ColumnSet = new ColumnSet(true);
                    return qe.GetSqlStatement();
                case FetchExpression fe:
                    // TODO: Select * for Fetch?
                    return fe.Query;
                case QueryByAttribute qba:
                    qba.ColumnSet = new ColumnSet(true);
                    return $"{qba.EntityName}|Atts({string.Join("|", qba.Attributes)})|Values({string.Join("|", qba.Values.Select(v => v.ObjectToStringDebug()))})";
                default:
                    throw new NotImplementedException($"Unknown QueryBase {query.GetType().FullName}!");
            }
        }

        /// <summary>
        /// Generates a key from the input <paramref name="request"/> by replacing separators in each parameter key value pair and joining them into a string.
        /// </summary>
        /// <param name="request">The organization request whose parameters will be used to generate a key.</param>
        /// <returns>A string representing the generated key.</returns>
        protected virtual string GetKey(OrganizationRequest request)
        {
            var parts = request.Parameters.Select(kvp => ReplaceSeparators(kvp.Key) + ParamKeySeparator + ReplaceSeparators(kvp.Value?.ToString()));
            return string.Join(ParamKvpSeparator, parts);

            string ReplaceSeparators(string value)
            {
                return string.IsNullOrWhiteSpace(value)
                    ? string.Empty 
                    : value.Replace(ParamKeySeparator, ' ').Replace(ParamKvpSeparator, " ");
            }
        }
    }
}
