using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
#if DLAB_UNROOT_COMMON_NAMESPACE
using DLaB.Common.Exceptions;
#else
using Source.DLaB.Common.Exceptions;
#endif

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm
#else
namespace Source.DLaB.Xrm
#endif
{
    /// <summary>
    /// Defines settings for a MinimumUpdater logic.
    /// </summary>
    public interface IMinimumUpdater<TEntity> where TEntity : Entity
    {
        /// <summary>
        /// Called prior to create.
        /// </summary>
        /// <param name="entity"></param>
        void PreCreate(TEntity entity);
        /// <summary>
        /// Called post create.  Will contain the new Id.
        /// </summary>
        /// <param name="entity">The entity created.</param>
        void PostCreate(TEntity entity);
        /// <summary>
        /// Should get the current in memory version of the entity for the given entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        TEntity GetCurrentValue(TEntity entity);
        /// <summary>
        /// Called prior to a full update.
        /// </summary>
        /// <param name="entity">The Entity to update.</param>
        void PreUpdate(TEntity entity);
        /// <summary>
        /// Called when it has been determined no changes are to be made.
        /// </summary>
        /// <param name="entity"></param>
        void NoChangesToSync(TEntity entity);
        /// <summary>
        /// Called when a minimal update is to be made. 
        /// </summary>
        /// <param name="entity">The passed in entity.</param>
        /// <param name="minimalChangesEntity">The minimal required values to update.</param>
        /// <param name="unchangedAttributes">The list of unchanged attributes that were removed from the local entity.</param>
        void PreMinimalUpdate(TEntity entity, TEntity minimalChangesEntity, List<string> unchangedAttributes);
        /// <summary>
        /// Return true to update the current in memory version.
        /// </summary>
        /// <param name="currentVersion">The current version.</param>
        /// <param name="updatedEntity">The </param>
        /// <returns>true if the current version should be updated.</returns>
        bool ShouldUpdateCurrentVersion(TEntity currentVersion, TEntity updatedEntity);
    }
}
