using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm
#else
namespace Source.DLaB.Xrm
#endif
	
{
    public static partial class Extensions
    {
        #region IOrganizationService

        #region GetEntityOrDefault

        /// <summary>
        /// Gets the entity by id. Null is returned if it isn't found.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="logicalName">Logical name of the entity.</param>
        /// <param name="id">Id of the entity to search for.</param>
        /// <param name="columnSet">Columns to retrieve.</param>
        /// <returns></returns>
        public static Entity GetEntityOrDefault(this IOrganizationService service, string logicalName, Guid id, ColumnSet columnSet = null)
        {
            var idName = EntityHelper.GetIdAttributeName(logicalName);
            return columnSet == null 
                ? service.GetFirstOrDefault(logicalName, idName, id)
                : service.GetFirstOrDefault(logicalName, columnSet, idName, id);
        }

        /// <summary>
        /// Gets the entity with the given id.  Null is returned if it isn't found.
        /// </summary>
        /// <typeparam name="T">The Entity Type.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="id">Id of the entity to search for.</param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add.</param>
        /// <returns></returns>
        public static T GetEntityOrDefault<T>(this IOrganizationService service, Guid id, Expression<Func<T, object>> anonymousTypeInitializer = null) where T : Entity
        {
            var idName = EntityHelper.GetIdAttributeName<T>();
            return anonymousTypeInitializer == null 
                ? service.GetFirstOrDefault<T>(idName, id)
                : service.GetFirstOrDefault(anonymousTypeInitializer, idName, id);
        }

#if !PRE_KEYATTRIBUTE

        /// <summary>
        /// Gets the entity with the given Key Value Pair collection.  Null is returned if it isn't found.
        /// </summary>
        /// <param name="service">open IOrganizationService</param>
        /// <param name="logicalName">The logical name</param>
        /// <param name="kvps">the KeyAttributes Collection</param>
        /// <param name="columnSet">Columns to retrieve.</param>
        /// <returns></returns>
        public static Entity GetEntityOrDefault(this IOrganizationService service, string logicalName, KeyAttributeCollection kvps, ColumnSet columnSet = null)
        {
            if(kvps.Count == 0)
            {
                throw new ArgumentException(nameof(kvps) + " must contain at least one kvp!", nameof(kvps));
            }

            var qe = columnSet == null
                ? QueryExpressionFactory.Create(logicalName)
                : QueryExpressionFactory.Create(logicalName, columnSet);
            foreach (var kvp in kvps)
            {
                switch (kvp.Value)
                {
                    case null:
                        qe.WhereEqual(new ConditionExpression(kvp.Key, ConditionOperator.Null));
                        break;
                    case EntityReference ef:
                        qe.WhereEqual(kvp.Key, ef.Id);
                        break;
                    default:
                        qe.WhereEqual(kvp.Key, kvp.Value);
                        break;
                }
            }

            return service.GetFirstOrDefault(qe);
        }

        /// <summary>
        /// Gets the entity with the given Key Value Pair collection.  Null is returned if it isn't found.
        /// </summary>
        /// <param name="service">open IOrganizationService</param>
        /// <param name="kvps">the KeyAttributes Collection</param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous</param>
        /// <returns></returns>
        public static T GetEntityOrDefault<T>(this IOrganizationService service, KeyAttributeCollection kvps, Expression<Func<T, object>> anonymousTypeInitializer = null) where T : Entity
        {
            var logicalName = EntityHelper.GetEntityLogicalName<T>();
            var cs = anonymousTypeInitializer == null
                ? null
                : new ColumnSet().AddColumns(anonymousTypeInitializer);
            return GetEntityOrDefault(service, logicalName, kvps, cs)?.ToEntity<T>();
        }
#endif

        #endregion GetEntityOrDefault

        #region GetEntitiesById

        /// <summary>
        /// Gets the first 5000 active entities with the given ids.
        /// </summary>
        /// <param name="service">The IOrganizationService.</param>
        /// <param name="logicalName">Logical name of the entity.</param>
        /// <param name="ids">Ids of the entity to search for.</param>
        /// <returns></returns>
        public static List<Entity> GetEntitiesById(this IOrganizationService service,
                string logicalName, IEnumerable<Guid> ids)
        {
            return service.GetEntitiesIn(logicalName, EntityHelper.GetIdAttributeName(logicalName), ids);
        }

        /// <summary>
        /// Gets the first 5000 active entities with the given ids.
        /// </summary>
        /// <param name="service">The IOrganizationService.</param>
        /// <param name="logicalName">Logical name of the entity.</param>
        /// <param name="ids">Ids of the entity to search for.</param>
        /// <returns></returns>
        public static List<Entity> GetEntitiesById(this IOrganizationService service,
                string logicalName, params Guid[] ids)
        {
            return service.GetEntitiesIn(logicalName, EntityHelper.GetIdAttributeName(logicalName), ids);
        }

        /// <summary>
        /// Gets the first 5000 active entities (with the given subset of columns only) with the given ids.
        /// </summary>
        /// <param name="service">The IOrganizationService.</param>
        /// <param name="columnSet">Columns to retrieve.</param>
        /// <param name="logicalName">Logical name of the entity.</param>
        /// <param name="ids">Ids of the entity to search for.</param>
        /// <returns></returns>
        public static List<Entity> GetEntitiesById(this IOrganizationService service, string logicalName,
                ColumnSet columnSet, IEnumerable<Guid> ids)
        {
            return service.GetEntitiesIn(logicalName, columnSet, EntityHelper.GetIdAttributeName(logicalName), ids);
        }

        /// <summary>
        /// Gets the first 5000 active entities (with the given subset of columns only) with the given ids.
        /// </summary>
        /// <param name="service">The IOrganizationService.</param>
        /// <param name="columnSet">Columns to retrieve.</param>
        /// <param name="logicalName">Logical name of the entity.</param>
        /// <param name="ids">Ids of the entity to search for.</param>
        /// <returns></returns>
        public static List<Entity> GetEntitiesById(this IOrganizationService service, string logicalName,
                 ColumnSet columnSet, params Guid[] ids)
        {
            return service.GetEntitiesIn(logicalName, columnSet, EntityHelper.GetIdAttributeName(logicalName), ids);
        }

        #endregion GetEntitiesById

        #region GetEntitiesById<T>

        /// <summary>
        /// Gets the first 5000 active entities with the given ids.
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return</typeparam>
        /// <param name="service">The IOrganizationService.</param>
        /// <param name="ids">Ids of the entity to search for.</param>
        /// <returns></returns>
        public static List<T> GetEntitiesById<T>(this IOrganizationService service, IEnumerable<Guid> ids) where T : Entity
        {
            return service.GetEntitiesIn<T>(EntityHelper.GetIdAttributeName<T>(), ids);
        }

        /// <summary>
        /// Gets the first 5000 active entities with the given ids.
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return.</typeparam>
        /// <param name="service">The IOrganizationService.</param>
        /// <param name="ids">Ids of the entity to search for.</param>
        /// <returns></returns>
        public static List<T> GetEntitiesById<T>(this IOrganizationService service, params Guid[] ids) where T : Entity
        {
            return service.GetEntitiesIn<T>(EntityHelper.GetIdAttributeName<T>(), ids);
        }

        /// <summary>
        /// Gets the first 5000 active entities (with the given subset of columns only) with the given ids.
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return.</typeparam>
        /// <param name="service">The IOrganizationService.</param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add.</param>
        /// <param name="ids">Ids of the entity to search for.</param>
        /// <returns></returns>
        public static List<T> GetEntitiesById<T>(this IOrganizationService service, Expression<Func<T, object>> anonymousTypeInitializer,
                IEnumerable<Guid> ids) where T : Entity
        {
            return service.GetEntitiesIn(anonymousTypeInitializer, EntityHelper.GetIdAttributeName<T>(), ids);
        }

        /// <summary>
        /// Gets the first 5000 active entities (with the given subset of columns only) with the given ids.
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return.</typeparam>
        /// <param name="service">The IOrganizationService.</param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add.</param>
        /// <param name="ids">Ids of the entity to search for.</param>
        /// <returns></returns>
        public static List<T> GetEntitiesById<T>(this IOrganizationService service, Expression<Func<T, object>> anonymousTypeInitializer,
                params Guid[] ids) where T : Entity
        {
            return service.GetEntitiesIn(anonymousTypeInitializer, EntityHelper.GetIdAttributeName<T>(), ids);
        }

        /// <summary>
        /// Gets the first 5000 active entities (with the given subset of columns only) with the given ids.
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return.</typeparam>
        /// <param name="service">The IOrganizationService.</param>
        /// <param name="columnSet">Columns to retrieve</param>
        /// <param name="ids">Ids of the entity to search for.</param>
        /// <returns></returns>
        public static List<T> GetEntitiesById<T>(this IOrganizationService service, ColumnSet columnSet, IEnumerable<Guid> ids) where T : Entity
        {
            return service.GetEntitiesIn<T>(columnSet, EntityHelper.GetIdAttributeName<T>(), ids);
        }

        /// <summary>
        /// Gets first 5000 Active Entities (with the given subset of columns only) 
        /// where the columnNameAndValue Pairs match.
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return.</typeparam>
        /// <param name="service">The IOrganizationService.</param>
        /// <param name="columnSet">Columns to retrieve.</param>
        /// <param name="ids">Ids of the entity to search for.</param>
        /// <returns></returns>
        public static List<T> GetEntitiesById<T>(this IOrganizationService service, ColumnSet columnSet, params Guid[] ids) where T : Entity
        {
            return service.GetEntitiesIn<T>(columnSet, EntityHelper.GetIdAttributeName<T>(), ids);
        }

        #endregion GetEntitiesById<T>

        #endregion IOrganizationServic
    }
}
