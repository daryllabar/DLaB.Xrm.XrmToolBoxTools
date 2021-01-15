#if !PRE_KEYATTRIBUTE
using System;
using System.Diagnostics;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm
#else
namespace Source.DLaB.Xrm
#endif
	
{
    /// <summary>
    /// Extensions for handling Optimistic Concurrency that was first used in CRM 2015.1
    /// </summary>
#if !DLAB_XRM_DEBUG
    [DebuggerNonUserCode]
#endif
    public static class OptimisticConcurrencyExtensions {
        /// <summary>
        /// Preforms an Optimistic Update.  If the entity's RowVersion doesn't match, the exception will be caught, and a reconciliation will be attempted before re-updating, indefinitely.
        /// Return null from reconcileEntity to skip cancel update
        /// Returns the entity whose Update Succeeded or null if the reconcileEntity function returned a null.
        /// </summary>
        /// <typeparam name="T">The entity type</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="reconcileEntity">Function that accepts the latest version of the entity from the server as a property, and returns what the updated version should now be.</param>
        /// <param name="retrieveEntity">Function used to get the entity if the RowVersion doesn't match.  Defaults to getting the entity with all columns returned.</param>
        /// <exception cref="Exception">No row version is set!  Unable to preform OptimisticUpdate</exception>
        public static T OptimisticUpdate<T>(this IOrganizationService service, T entity, Func<T, T> reconcileEntity, Func<IOrganizationService, T> retrieveEntity = null) where T: Entity
        {
            if (string.IsNullOrWhiteSpace(entity.RowVersion))
            {
                throw new Exception("No row version is set!  Unable to preform OptimisticUpdate");
            }
            
            var request = new UpdateRequest
            {
                ConcurrencyBehavior = ConcurrencyBehavior.IfRowVersionMatches,
                Target = entity
            };

            entity = null;

            while (true)
            {
                try
                {
                    service.Execute(request);
                    return entity;
                }
                catch (FaultException<OrganizationServiceFault> ex) 
                {
                    if (ex.Detail == null || ex.Detail.ErrorCode != CrmSdk.ErrorCodes.ConcurrencyVersionMismatch)
                    {
                        throw;
                    }
                    entity = retrieveEntity == null ? service.GetEntity<T>(request.Target.Id) : retrieveEntity(service);
                    request.Target = reconcileEntity(entity);
                    if (request.Target == null)
                    {
                        // Reconciling code must have given up on reconciling
                        return null;
                    }
                    request.Target.RowVersion = entity.RowVersion;
                    if (string.IsNullOrWhiteSpace(entity.RowVersion))
                    {
                        throw new Exception("No row version is set!  Unable to preform OptimisticUpdate");
                    }
                }
            }
        }
    }
}
#endif