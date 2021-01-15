using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm
#else
namespace Source.DLaB.Xrm
#endif
	
{
    /// <summary>
    /// Determines the Active Attribute for the Latebound Entity
    /// </summary>
    public class LateBoundActivePropertyInfo : ActivePropertyInfo<Entity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LateBoundActivePropertyInfo"/> class.
        /// </summary>
        /// <param name="logicalName">Name of the logical.</param>
        public LateBoundActivePropertyInfo(string logicalName) : base(logicalName) { }

        /// <summary>
        /// Determines whether the specified entity is active.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static bool? IsActive(IOrganizationService service, Entity entity)
        {
            return IsActive(service, entity.LogicalName, entity.Id);
        }

        /// <summary>
        /// Determines whether the specified entity is active.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="logicalName">Name of the logical.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <returns></returns>
        public static bool? IsActive(IOrganizationService service, string logicalName, Guid entityId)
        {
            var info = new LateBoundActivePropertyInfo(logicalName);
            var entity = service.Retrieve(logicalName, entityId, new ColumnSet(info.AttributeName));
            return IsActive(info, entity);
        }

        /// <summary>
        /// Determines whether the specified service is active.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException">IsActive(IOrganizationService,Guid) signature is not supported for LateBound</exception>
        [Obsolete("IsActive(IOrganizationService,Guid) signature is not supported for LateBound", true)]
        public new static bool? IsActive(IOrganizationService service, Guid entityId)
        {
            throw new NotSupportedException("IsActive(IOrganizationService,Guid) signature is not supported for LateBound");
        }
    }
}
