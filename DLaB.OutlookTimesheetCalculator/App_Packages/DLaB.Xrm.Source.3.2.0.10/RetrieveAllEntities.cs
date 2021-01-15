using System;
using System.Collections.Generic;
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
        private delegate EntitiesWithCookie<T> EntityRetriever(IOrganizationService service, QueryExpression qe);

        private EntityRetriever EntityRetrievingMethod { get; set; }

        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="qe">The qe.</param>
        /// <param name="maxCount">The maximum count.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="async">if set to <c>true</c> the Service will retrieve the next batch asynchrounsly after starting to return the first batch.  This means that any looping that is performed with this call, can not use the same organization service or else there will be a multi-threading issue.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllEntities(IOrganizationService service, QueryExpression qe, int? maxCount = null, int? pageSize = null, bool async = false)
        {
            return new RetrieveAllEntities<T>().GetAllEntitiesInstance(service, qe, maxCount, pageSize, async);
        }

        private IEnumerable<T> GetAllEntitiesInstance(IOrganizationService service, QueryExpression qe, int? maxCount, int? pageSize, bool async)
        {
            var page = qe.PageInfo;
            IAsyncResult asyncResult = null;
            EntityRetrievingMethod = GetEntitiesWithCookie;
            var count = 0;

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

            page.PageNumber = 1;
            page.PagingCookie = null;

            var response = GetEntitiesWithCookie(service, qe);

            while (response.MoreRecords && response.Entities != null && (maxCount == null || maxCount.Value <= count))
            {
                UpdatePageCount(page, ref count, maxCount);
                page.PageNumber++;
                page.PagingCookie = response.Cookie;

                // Perform Async call for next set, while yield returning current set 
                try
                {
                    // If async, begin request to get new entities before yielding results
                    if (async)
                    {
                        asyncResult = EntityRetrievingMethod.BeginInvoke(service, qe, null, this);
                    }
                    // Retrieve all records from the result set.
                    foreach (var entity in response.Entities)
                    {
                        yield return entity;
                    }

                    // If sync, wait until all entities have been returned, then get entities
                    if (!async)
                    {
                        response = GetEntitiesWithCookie(service, qe);
                    }
                }
                finally
                {
                    if (asyncResult != null)
                    {
                        response = EntityRetrievingMethod.EndInvoke(asyncResult);
                        asyncResult.AsyncWaitHandle.Close();
                    }
                }
            }

            if (response.Entities == null)
            {
                yield break;
            }

            foreach (var entity in response.Entities)
            {
                yield return entity;
            }
        }

        private void UpdatePageCount(PagingInfo page, ref int count, int? maxCount)
        {
            if (maxCount > 0)
            {
                if (page.Count + count > maxCount.Value)
                {
                    page.Count = maxCount.Value - count;
                }
                else
                {
                    count = count + page.Count;
                }
            }
        }

        private EntitiesWithCookie<T> GetEntitiesWithCookie(IOrganizationService service, QueryExpression qe)
        {
            var response = service.RetrieveMultiple(qe);
            var values = response.ToEntityList<T>();
            return new EntitiesWithCookie<T>
            {
                Entities = values,
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
