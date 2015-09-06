using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using DLaB.Xrm;
using DLaB.Xrm.Common;
using DLaB.Xrm.Entities;
using DLaB.Xrm.Core.Entities;

namespace DLaB.Xrm.Messages
{
    /// <summary>
    /// Created based on http://msdn.microsoft.com/en-us/library/hh670605.aspx
    /// </summary>
    public class BulkDelete
    {
        const int ARBITRARY_MAX_POLLING_TIME = 60;

        /// <summary>
        /// Gets the max asynchronous request timeout from the AppSettings.MaxAsynchronousRequestTimeout or defaults to 60.
        /// </summary>
        public static int MaxAsynchronousRequestTimeout { get { return Config.GetAppSettingOrDefault("MaxAsynchronousRequestTimeout", ARBITRARY_MAX_POLLING_TIME); } }

        /// <summary>
        /// Deletes all entities returned by the set of query sets
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="querySets">The query expressiosn to use.</param>
        /// <returns></returns>
        public static BulkDeleteResult Delete(IOrganizationService service, QueryExpression[] querySets)
        {
            BulkDeleteResult result = new BulkDeleteResult();
            try
            {
                result = DeleteInternal(service, querySets);
            }
            catch (Exception ex)
            {
                result.Error = ex;
            }

            return result;
        }

        private static BulkDeleteResult DeleteInternal(IOrganizationService service, QueryExpression[] querySets)
        {
            BulkDeleteResult result = new BulkDeleteResult();

            // Create the bulk delete request
            BulkDeleteRequest bulkDeleteRequest = new BulkDeleteRequest()
            {
                // Set the request properties
                JobName = "Temp Bulk Delete " + DateTime.Now,
                QuerySet = querySets,
                ToRecipients = new Guid[0],
                CCRecipients = new Guid[0],
                RecurrencePattern = string.Empty
            };

            // Submit the bulk delete job.
            // NOTE: Because this is an asynchronous operation, the response will be immediate.
            BulkDeleteResponse response = (BulkDeleteResponse)service.Execute(bulkDeleteRequest);

            Guid asyncId = response.JobId;
            var bulkOperation = GetBulkDeleteOperation(service, asyncId);

            // Monitor the async operation through polling until it is complete or max polling time expires.
            int secondsTicker = MaxAsynchronousRequestTimeout;
            while (secondsTicker > 0)
            {
                // Make sure that the async operation was retrieved.
                if (bulkOperation != null && bulkOperation.StateCode.Value == BulkDeleteOperationState.Completed)
                {
                    result.TimedOut = false;
                    break;
                }

                // Wait a second for async operation to become active.
                System.Threading.Thread.Sleep(1000);
                secondsTicker--;

                // Retrieve the entity again
                bulkOperation = GetBulkDeleteOperation(service, asyncId);
            }

            if (result.TimedOut == null)
            {
                result.TimedOut = true;
            }

            // Validate that the operation was completed.
            if (bulkOperation == null)
            {
                result.Result = "The Bulk Operation for Async " + asyncId + " was not found.";
            }
            else
            {
                result.DeletedCount = bulkOperation.SuccessCount ?? 0;
                result.DeleteFailedCount = bulkOperation.FailureCount ?? 0;
                if (bulkOperation.StateCode.Value != BulkDeleteOperationState.Completed ||
                    bulkOperation.StatusCode.Value != (int)bulkdeleteoperation_statuscode.Succeeded)
                {
                    // This happens if it took longer than the polling time allowed 
                    // for this operation to finish.
                    result.Result = "The operation took longer than the polling time allowed for this operation to finish.";
                }
                else if (result.DeleteFailedCount > 0)
                {
                    result.Result = string.Format("The opertion had {0} failures and {1} successful deletions",
                            result.DeletedCount, result.DeleteFailedCount);
                }
                else
                {
                    result.Result = string.Format("The operation had {0} successful deletions",
                        result.DeletedCount);
                }
            }

            service.Delete(BulkDeleteOperation.EntityLogicalName, bulkOperation.Id);

            // We have to update the AsyncOperation to be in a Completed state before we can delete it.
            service.InitializeEntity<Entities.AsyncOperation>(bulkOperation.AsyncOperationId.Id,
                    a => { a.StateCode = AsyncOperationState.Completed; });

            // Not sure if the status code needs to be set...
            //a.StatusCode = new OptionSetValue((int)asyncoperation_statuscode.Succeeded) });
            service.Delete(Entities.AsyncOperation.EntityLogicalName, bulkOperation.AsyncOperationId.Id);
            return result;
        }

        private static BulkDeleteOperation GetBulkDeleteOperation(IOrganizationService service, Guid asyncId)
        {
            var settings = new QuerySettings<BulkDeleteOperation>()
            {
                ActiveOnly = false,
                Columns = new ColumnSet("failurecount", "successcount", "statecode", "statuscode",
                          "bulkdeleteoperationid", "asyncoperationid"),
                First = true
            };
            return service.RetrieveList<BulkDeleteOperation>(settings.CreateExpression("asyncoperationid", asyncId)).FirstOrDefault();
        }


    }

    /// <summary>
    /// Contains the results of a BulkDelete Opteration
    /// </summary>
    public class BulkDeleteResult
    {
        /// <summary>
        /// Gets the error of the BulkDelete Operation, if there was one.  Returns null if no error occured.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public System.Exception Error { get; internal set; }
        /// <summary>
        /// Gets the result of the BulkDelete Operation.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public string Result { get; internal set; }
        /// <summary>
        /// Gets the number of deleted entities from the BulkDelete Operation.
        /// </summary>
        /// <value>
        /// The deleted count.
        /// </value>
        public int DeletedCount { get; internal set; }
        /// <summary>
        /// Gets the number of entities that failed to be deleted by the BulkDelete Operation.
        /// </summary>
        /// <value>
        /// The delete failed count.
        /// </value>
        public int DeleteFailedCount { get; internal set; }
        /// <summary>
        /// Returns true if the request timed out.
        /// </summary>
        public bool? TimedOut { get; internal set; }
    }
}
