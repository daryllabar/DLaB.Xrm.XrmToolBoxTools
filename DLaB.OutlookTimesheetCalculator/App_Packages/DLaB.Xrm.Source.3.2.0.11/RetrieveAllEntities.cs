using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm
#else
namespace Source.DLaB.Xrm
#endif

{
    internal class RetrieveAllEntities<T> where T : Entity
    {
        private const int DefaultPageSize = 5000;
        private int _totalRetrievedCount;

        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="qe">The qe.</param>
        /// <param name="maxCount">The maximum count.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="async">if set to <c>true</c> the Service will retrieve the next batch asynchronously after starting to return the first batch.  This means that any looping that is performed with this call, can not use the same organization service or else there will be a multi-threading issue.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllEntities(IOrganizationService service, QueryExpression qe, int? maxCount = null, int? pageSize = null, bool async = false)
        {
            return new RetrieveAllEntities<T>().GetAllEntitiesInstance(service, qe, maxCount, pageSize, async);
        }

        private IEnumerable<T> GetAllEntitiesInstance(IOrganizationService service, QueryExpression qe, int? maxCount, int? pageSize, bool async)
        {
            var page = qe.PageInfo;
            ConditionallySetPageCount(maxCount, pageSize, page);
            page.PageNumber = 1;
            page.PagingCookie = null;

            var response = GetEntitiesWithCookie(service, qe);

            while (response.MoreRecords
                   && response.Entities != null
                   && (maxCount == null || maxCount.Value <= _totalRetrievedCount))
            {
                UpdatePageCount(page, maxCount);
                page.PageNumber++;
                page.PagingCookie = response.Cookie;

                if (async)
                {
                    var task = new Task<EntitiesWithCookie<T>>(() => GetEntitiesWithCookie(service, qe));
                    task.Start();

                    foreach (var entity in response.Entities)
                    {
                        yield return entity;
                    }

                    response = task.Result;
                }
                else
                {
                    foreach (var entity in response.Entities)
                    {
                        yield return entity;
                    }

                    response = GetEntitiesWithCookie(service, qe);
                }
            }

            // No more records on server, return whatever has been received
            if (response.Entities == null)
            {
                yield break;
            }

            foreach (var entity in response.Entities)
            {
                yield return entity;
            }
        }

        private static void ConditionallySetPageCount(int? maxCount, int? pageSize, PagingInfo page)
        {
            if (maxCount != null && pageSize == null && maxCount < DefaultPageSize)
            {
                // Update page Size to Max Count to limit the number of records retrieved
                pageSize = maxCount;
            }

            // Check for page Size / Max Count Settings
            if (maxCount < pageSize)
            {
                pageSize = maxCount;
            }

            if (pageSize != null && pageSize > 0)
            {
                page.Count = pageSize.Value;
            }
        }


        private void UpdatePageCount(PagingInfo page, int? maxCount)
        {
            if (maxCount <= 0)
            {
                return;
            }

            if (page.Count + _totalRetrievedCount > maxCount)
            {
                page.Count = maxCount.Value - _totalRetrievedCount;
            }
            else
            {
                _totalRetrievedCount += page.Count;
            }
        }

        private EntitiesWithCookie<T> GetEntitiesWithCookie(IOrganizationService service, QueryExpression qe)
        {
            var response = service.RetrieveMultiple(qe);
            return new EntitiesWithCookie<T>
            {
                Entities = response.Entities.Select(e => e.AsEntity<T>()),
                Cookie = response.PagingCookie,
                MoreRecords = response.MoreRecords
            };
        }

        private class EntitiesWithCookie<TEntity> where TEntity : Entity
        {
            public IEnumerable<TEntity> Entities { get; set; }
            public string Cookie { get; set; }
            public bool MoreRecords { get; set; }
        }
    }
}
