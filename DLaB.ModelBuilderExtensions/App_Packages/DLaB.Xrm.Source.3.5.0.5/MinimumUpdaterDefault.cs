using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xrm.Sdk;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm
#else
namespace Source.DLaB.Xrm
#endif
{

    /// <summary>
    /// Default Implementation
    /// </summary>
#if !DLAB_XRM_DEBUG
    [DebuggerNonUserCode]
#endif
    public class MinimumUpdaterDefault<TEntity> : IMinimumUpdater<TEntity>  where TEntity : Entity
    {
        /// <summary>
        /// Dictionary of Current Values
        /// </summary>
        protected Dictionary<Guid, TEntity> CurrentValuesById { get; }
        /// <summary>
        /// Default Construction
        /// </summary>
        /// <param name="currentValuesById"></param>
        public MinimumUpdaterDefault(Dictionary<Guid, TEntity> currentValuesById = null)
        {
            CurrentValuesById = currentValuesById ?? new Dictionary<Guid, TEntity>();
        }

        /// <summary>
        /// Called prior to create.
        /// </summary>
        /// <param name="entity"></param>
        public virtual void PreCreate(TEntity entity)
        {
        }

        /// <summary>
        /// Called post create.  Will contain the new Id.
        /// </summary>
        /// <param name="entity">The entity created.</param>
        public virtual void PostCreate(TEntity entity)
        {
        }

        /// <summary>
        /// Should get the current in memory version of the entity for the given entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public virtual TEntity GetCurrentValue(TEntity entity)
        {
            return CurrentValuesById.TryGetValue(entity.Id, out var current) ? current : null;
        }

        /// <summary>
        /// Called prior to a full update.
        /// </summary>
        /// <param name="entity">The Entity to update.</param>
        public virtual void PreUpdate(TEntity entity)
        {
         
        }

        /// <summary>
        /// Called when it has been determined no changes are to be made.
        /// </summary>
        /// <param name="entity"></param>
        public virtual void NoChangesToSync(TEntity entity)
        {
         
        }

        /// <summary>
        /// Called when a minimal update is to be made. 
        /// </summary>
        /// <param name="entity">The passed in entity.</param>
        /// <param name="minimalChangesEntity">The minimal required values to update.</param>
        /// <param name="unchangedAttributes">The list of unchanged attributes that were removed from the local entity.</param>
        public virtual void PreMinimalUpdate(TEntity entity, TEntity minimalChangesEntity, List<string> unchangedAttributes)
        {
            
        }

        /// <summary>
        /// Return true to update the current in memory version.
        /// </summary>
        /// <param name="currentVersion">The current version.</param>
        /// <param name="updatedEntity">The </param>
        /// <returns>true if the current version should be updated.</returns>
        public virtual bool ShouldUpdateCurrentVersion(TEntity currentVersion, TEntity updatedEntity)
        {
            return true;
        }
    }
}
