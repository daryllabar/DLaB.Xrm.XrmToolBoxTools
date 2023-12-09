using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DLaB.AttributeManager
{
    public class RetryOrgService : IOrganizationService
    {
        private readonly IOrganizationService _actual;
        private readonly Action<string> _trace;
        private readonly List<int> _retryTimeouts;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="trace"></param>
        /// <param name="retryTimeouts">Retry Timeouts in ms.  Defaults to [1, 1,000, 20,000]</param>
        public RetryOrgService(IOrganizationService actual, Action<string> trace, IEnumerable<int> retryTimeouts = null)
        {
            _actual = actual;
            _trace = trace;
            var temp = (retryTimeouts ?? new[] { 1, 1000, 20000 }).ToList();
            temp.Add(int.MinValue);
            _retryTimeouts = temp;
        }

        public Guid Create(Entity entity)
        {
            return ExecuteRequestWithRetries(nameof(Create), () => _actual.Create(entity));
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            return ExecuteRequestWithRetries(nameof(Retrieve), () => _actual.Retrieve(entityName, id, columnSet));
        }

        public void Update(Entity entity)
        {
            ExecuteRequestWithRetries(nameof(Update), () => _actual.Update(entity));
        }

        public void Delete(string entityName, Guid id)
        {
            ExecuteRequestWithRetries(nameof(Delete), () => _actual.Delete(entityName, id));
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            return ExecuteRequestWithRetries(nameof(Execute), () => _actual.Execute(request));
        }

        public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            ExecuteRequestWithRetries(nameof(Associate), () => _actual.Associate(entityName, entityId, relationship, relatedEntities));
        }

        public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            ExecuteRequestWithRetries(nameof(Disassociate), () => _actual.Disassociate(entityName, entityId, relationship, relatedEntities));
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            return ExecuteRequestWithRetries(nameof(RetrieveMultiple), () => _actual.RetrieveMultiple(query));
        }


        private T ExecuteRequestWithRetries<T>(string request, Func<T> requestAction)
        {
            foreach (var timeout in _retryTimeouts)
            {
                try
                {
                    return requestAction();
                }
                catch (Exception ex)
                {
                    if (timeout == int.MinValue)
                    {
                        throw;
                    }

                    _trace($"{request} Failed.  Retrying in {timeout}ms.  Exception: {ex.Message}");
                    System.Threading.Thread.Sleep(timeout);
                }
            }

            throw new Exception("No retries defined!");
        }

        private void ExecuteRequestWithRetries(string request, Action requestAction)
        {
            foreach (var timeout in _retryTimeouts)
            {
                try
                {
                    requestAction();
                    return;
                }
                catch (Exception ex)
                {
                    if (timeout == int.MinValue)
                    {
                        throw;
                    }

                    _trace($"{request} Failed.  Retrying in {timeout}ms.  Exception: {ex.Message}");
                    System.Threading.Thread.Sleep(timeout);
                }
            }

            throw new Exception("No retries defined!");
        }
    }
}
