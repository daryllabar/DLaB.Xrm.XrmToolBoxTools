using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace DLaB.Xrm
{
    internal class RetrieveAllEntities<T> where T : Microsoft.Xrm.Sdk.Entity
    {
        private const int DEFAULT_PAGE_SIZE = 5000;
        private delegate EntitiesWithCookie<T> EntityRetriever(IOrganizationService service, QueryExpression qe);

        private EntityRetriever EntityRetrievingMethod { get; set; }

        public static IEnumerable<T> GetAllEntities(IOrganizationService service, QueryExpression qe, int? maxCount = null, int? pageSize = null)
        {
            return new RetrieveAllEntities<T>().GetAllEntitiesAsync(service, qe, maxCount, pageSize);
        }

        private IEnumerable<T> GetAllEntitiesAsync(IOrganizationService service, QueryExpression qe, int? maxCount, int? pageSize)
        {
            var page = qe.PageInfo;
            IAsyncResult asyncResult = null;
            EntityRetrievingMethod = GetEntitiesWithCookie;
            int count = 0;

            if (maxCount != null && pageSize == null && maxCount < DEFAULT_PAGE_SIZE)
            {
                // Updte page Size to Max Count to limit the number of records retrieved
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
                    asyncResult = EntityRetrievingMethod.BeginInvoke(service, qe, null, this);

                    // Retrieve all records from the result set.
                    foreach (T entity in response.Entities)
                    {
                        yield return entity;
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
            else
            {
                foreach (T entity in response.Entities)
                {
                    yield return entity;
                }
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
            EntityCollection response = service.RetrieveMultiple(qe);
            var values = response.ToEntityList<T>();
            return new EntitiesWithCookie<T>()
            {
                Entities = values,
                Cookie = response.PagingCookie,
                MoreRecords = response.MoreRecords
            };
        }

        private class EntitiesWithCookie<E> where E : Microsoft.Xrm.Sdk.Entity
        {
            public IEnumerable<E> Entities { get; set; }
            public string Cookie { get; set; }
            public bool MoreRecords { get; set; }
        }
    }
}
