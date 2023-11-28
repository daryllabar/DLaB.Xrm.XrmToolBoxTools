using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata.Query;
#if DLAB_UNROOT_COMMON_NAMESPACE
using DLaB.Common;
#else
using Source.DLaB.Common;
#endif
#if DLAB_UNROOT_NAMESPACE || DLAB_XRM

namespace DLaB.Xrm
#else

namespace Source.DLaB.Xrm
#endif
{
    /// <summary>
    /// Extension class for Xrm
    /// </summary>
    public static partial class Extensions
    {

        /// <summary>
        /// Uses the string value to get a deterministic day value between 1-365 to get the newest email older than that day, and updates it's currency to acquire a DB lock.
        /// If no email is found, the oldest Business Unit's Fax is updated instead.
        /// </summary>
        /// <param name="service">The Service.</param>
        /// <param name="value">The string value to ensure uniqueness of to lock on.  For example, the e-mail to ensure that a contact with a duplicate email address, will not be created.</param>
        /// <param name="tracer">The Tracing Service.</param>
        /// <param name="initialTable">The table to use to initially acquire a lock on.  It will use the value parameter to select a date within the last year to attempt to find a record in this table that it can lock, to allow multiple locks to be acquired simultaneously for different values.</param>
        /// <param name="initialTableColumn">The column to update in the initial table.</param>
        /// <param name="backupTable">The backup table to use if no record is found in the initial table.  This table has to exist, or an exception will be thrown.</param>
        /// <param name="backupTableColumn">The Column to update in the backup table.</param>
        // ReSharper disable StringLiteralTypo
        public static void AcquireLock(this IOrganizationService service, string value, ITracingService tracer = null,
            string initialTable = "email", string initialTableColumn = "transactioncurrencyid",
            string backupTable = "businessunit", string backupTableColumn = "address2_fax")
        {
            const string createdOn = "createdon";
            // ReSharper restore StringLiteralTypo
            // Use the hashcode to get a count that is deterministic, between 1-365, to get a random day in the last year
            var deterministic365DayCount = new Random(value.GetDeterministicHashCode()).Next(1, 366);
            // Query for the newest email that is older than the random day in the last year
            var qe = new QueryExpression(initialTable);
            qe.ColumnSet.AddColumns(initialTableColumn);
            qe.Criteria.AddCondition(createdOn, ConditionOperator.LessThan, DateTime.UtcNow.AddDays(-1 * deterministic365DayCount));
            qe.AddOrder(createdOn, OrderType.Descending);
            var initial = service.GetFirstOrDefault(qe);
            if (initial == null)
            {
                tracer?.Trace("No email found older than {0} days.  Get the oldest Business Unit.", deterministic365DayCount);
                qe = new QueryExpression(backupTable);
                qe.ColumnSet.AddColumns(backupTableColumn);
                qe.AddOrder(createdOn, OrderType.Descending);
                var backup = service.GetFirst(qe);
                service.Update(new Entity(backupTable)
                {
                    Id = backup.Id, 
                    [backupTableColumn] = backup.GetAttributeValue<object>(backupTableColumn)
                });
                return;
            }

            service.Update(new Entity(initialTable)
            {
                Id = initial.Id,
                [initialTableColumn] = initial.GetAttributeValue<object>(initialTableColumn)
            });
        }

        #region Assign

        /// <summary>
        /// Assigns the supplied entity to the supplied user
        /// </summary>
        /// <param name="service"></param>
        /// <param name="target"></param>
        /// <param name="newOwner"></param>
        public static void Assign(this IOrganizationService service, EntityReference target, EntityReference newOwner)
        {
            service.Update(new Entity(target.LogicalName)
            {
                Id = target.Id,
                ["ownerid"] = newOwner
            });
        }

        /// <summary>
        /// Reassigns the owner of the entity to the new owner
        /// </summary>
        /// <param name="service"></param>
        /// <param name="itemToChangeOwnershipOf">Must have Logical Name and Id Populated</param>
        /// <param name="userId"></param>
        public static void Assign(this IOrganizationService service, Entity itemToChangeOwnershipOf, Guid userId)
        {
            Assign(service, itemToChangeOwnershipOf.ToEntityReference(), new EntityReference("systemuser", userId));
        }

        /// <summary>
        /// Reassigns the owner of the entity to the new owner
        /// </summary>
        /// <param name="service"></param>
        /// <param name="itemToChangeOwnershipOf">Must have Logical Name and Id Populated</param>
        /// <param name="userId"></param>
        public static void Assign(this IOrganizationService service, EntityReference itemToChangeOwnershipOf, Guid userId)
        {
            Assign(service, itemToChangeOwnershipOf, new EntityReference("systemuser", userId));
        }


        /// <summary>
        /// Reassigns the owner of the entity to the new owner
        /// </summary>
        /// <param name="service"></param>
        /// <param name="itemToChangeOwnershipOf">Must have Logical Name and Id Populated</param>
        /// <param name="teamId"></param>
        public static void AssignTeam(this IOrganizationService service, Entity itemToChangeOwnershipOf, Guid teamId)
        {
            Assign(service, itemToChangeOwnershipOf.ToEntityReference(), new EntityReference("team", teamId));
        }

        /// <summary>
        /// Reassigns the owner of the entity to the new owner
        /// </summary>
        /// <param name="service"></param>
        /// <param name="itemToChangeOwnershipOf">Must have Logical Name and Id Populated</param>
        /// <param name="teamId"></param>
        public static void AssignTeam(this IOrganizationService service, EntityReference itemToChangeOwnershipOf, Guid teamId)
        {
            Assign(service, itemToChangeOwnershipOf, new EntityReference("team", teamId));
        }

        #endregion Assign

        #region Associate

        /// <summary>
        /// Associates one or more entities to an entity.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entity"></param>
        /// <param name="relationshipLogicalName"></param>
        /// <param name="entities"></param>
        public static void Associate(this IOrganizationService service, Entity entity, string relationshipLogicalName, params Entity[] entities)
        {
            var relationship = new Relationship(relationshipLogicalName);
            if (entity.LogicalName == entities.First().LogicalName)
            {
                relationship.PrimaryEntityRole = EntityRole.Referenced;
            }

            service.Associate(entity.LogicalName, entity.Id,
                relationship,
                new EntityReferenceCollection(entities.Select(e => e.ToEntityReference()).ToList()));
        }

        /// <summary>
        /// Associates one or more entities to an entity.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entity"></param>
        /// <param name="relationshipLogicalName"></param>
        /// <param name="entities"></param>
        public static void Associate(this IOrganizationService service, EntityReference entity, string relationshipLogicalName, params EntityReference[] entities)
        {
            var relationship = new Relationship(relationshipLogicalName);
            if (entity.LogicalName == entities.First().LogicalName)
            {
                relationship.PrimaryEntityRole = EntityRole.Referenced;
            }

            service.Associate(entity.LogicalName, entity.Id,
                relationship,
                new EntityReferenceCollection(entities.ToList()));
        }

        #endregion Associate

        #region CreateOrMinimumUpdate

        /// <summary>
        /// Compares the values in the most recent entities by id dictionary to the given value, only updating the fields that are out of date.
        /// </summary>
        /// <typeparam name="TEntity">The Entity Type</typeparam>
        /// <param name="service">The Service.</param>
        /// <param name="entity">The Entity to Create or Update.</param>
        /// <param name="mostRecentEntitiesById">The most recent entities by id.</param>
        public static void CreateOrMinimumUpdate<TEntity>(this IOrganizationService service, TEntity entity, Dictionary<Guid, TEntity> mostRecentEntitiesById) where TEntity : Entity
        {
            service.CreateOrMinimumUpdate(entity, new MinimumUpdaterDefault<TEntity>(mostRecentEntitiesById));
        }

        /// <summary>
        /// Compares the values in the most recent entities by id dictionary to the given value, only updating the fields that are out of date.
        /// </summary>
        /// <typeparam name="TEntity">The Entity Type</typeparam>
        /// <param name="service">The Service.</param>
        /// <param name="entity">The Entity to Create or Update.</param>
        /// <param name="updater">The IMinimumUpdater to use.</param>
        public static void CreateOrMinimumUpdate<TEntity>(this IOrganizationService service, TEntity entity, IMinimumUpdater<TEntity> updater = null) where TEntity: Entity
        {
            if (entity.Id == Guid.Empty)
            {
                // No Guid.  Must be for create
                updater?.PreCreate(entity);
#if PRE_KEYATTRIBUTE
                entity.Id = service.Create(entity);
#else
                entity.Id = entity.KeyAttributes?.Count > 0
                    ? service.Upsert(entity).Target.Id
                    : service.Create(entity);
#endif
                updater?.PostCreate(entity);
                return;
            }

            var image = GetCurrentValue(service, entity, updater);
            if (image == null)
            {
                // Guid exists, but no current version, update everything.
                updater?.PreUpdate(entity);
                service.Update(entity);
                return;
            }

            // Perform a minimum update
            var localEntity = entity.Clone();
            var unchangedAttributes = new List<string>();

            foreach (var keyValue in image.Attributes.Where(kvp => localEntity.Contains(kvp.Key)
                                                                   && kvp.Value.NullSafeEquals(localEntity.GetAttributeValue<object>(kvp.Key))
                                                                   && !kvp.Value.NullSafeEquals(localEntity.Id)))
            {
                unchangedAttributes.Add(keyValue.Key);
                localEntity.Attributes.Remove(keyValue.Key);
            }
            if (localEntity.Attributes.Count == 1
                && localEntity.Attributes.First().Value.Equals(localEntity.Id))
            {
                // Only attribute left is the Id Guid.  Skip!
                updater?.NoChangesToSync(entity);
                return;
            }

            updater?.PreMinimalUpdate(entity, localEntity, unchangedAttributes);
            service.Update(localEntity);

            if (updater?.ShouldUpdateCurrentVersion(image, localEntity) == true)
            {
                foreach (var keyValue in localEntity.Attributes)
                {
                    image[keyValue.Key] = keyValue.Value;
                }
            }
        }

        private static TEntity GetCurrentValue<TEntity>(IOrganizationService service, TEntity entity, IMinimumUpdater<TEntity> updater) where TEntity : Entity
        {
            if(updater != null)
            {
                return updater.GetCurrentValue(entity);
            }

            return typeof(TEntity) == typeof(Entity)
                ? (TEntity) service.GetEntityOrDefault(entity.LogicalName, entity.Id)
                : service.GetEntityOrDefault<TEntity>(entity.Id);
        }

        private static bool NullSafeEquals(this object thisValue, object value)
        {
            if (thisValue == null)
            {
                return value == null;
            }

            return thisValue.Equals(value);
        }

        #endregion CreateOrMinimumUpdate

        #region CreateWithSuppressDuplicateDetection

        /// <summary>
        /// Creates a record with SuppressDuplicateDetection Enabled to Ignore any potential Duplicates Created
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Guid CreateWithSuppressDuplicateDetection(this IOrganizationService service, Entity entity)
        {
            var response = (CreateResponse)service.Execute(new CreateRequest
            {
                Target = entity,
                ["SuppressDuplicateDetection"] = true
            });
            return response.id;
        }

        #endregion CreateWithSuppressDuplicateDetection

        #region Delete

        /// <summary>
        /// Deletes the specified entity
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entity">The entity to be deleted.</param>
        public static void Delete(this IOrganizationService service, Entity entity)
        {
            service.Delete(entity.LogicalName, entity.Id);
        }

        /// <summary>
        /// Deletes the specified entity  
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static void Delete(this IOrganizationService service, EntityReference entity)
        {
            service.Delete(entity.LogicalName, entity.Id);
        }

        #endregion Delete

        #region DeleteIfExists

        /// <summary>
        /// Attempts to delete the entity with the given id. If it doesn't exist, false is returned
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entity">The entity to delete if it exists.</param>
        public static void DeleteIfExists(this IOrganizationService service, Entity entity)
        {
            service.DeleteIfExists(entity.LogicalName, entity.Id);
        }

        /// <summary>
        /// Delete all active entities in the entity specified by the LogicalName and the Filter Expression
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="logicalName">The logical name of the entity that will be deleted.</param>
        /// <param name="fe">The filter expression to use to determine what records to delete.</param>
        /// <returns></returns>
        public static bool DeleteIfExists(this IOrganizationService service, string logicalName, FilterExpression fe)
        {
            var qe = new QueryExpression(logicalName) { Criteria = fe };
            return service.DeleteIfExists(qe);
        }

        /// <summary>
        /// Attempts to delete the entity with the given id. If it doesn't exist, false is returned
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="id">The id of the entity to search and potentially delete.</param>
        /// <returns></returns>
        public static bool DeleteIfExists(this IOrganizationService service, string entityName, Guid id)
        {
            return DeleteIfExistsWithRetry(service, entityName, id, 0);
        }

        /// <summary>
        /// Delete all entities that are returned by the Query Expression.
        /// </summary>
        /// <param name="service">The Service</param>
        /// <param name="qe">The query expression used to define the set of entities to delete</param>
        /// <returns></returns>
        public static bool DeleteIfExists(this IOrganizationService service, QueryExpression qe)
        {
            var exists = false;
            var idName = EntityHelper.GetIdAttributeName(qe.EntityName);
            qe.ColumnSet = new ColumnSet(idName);
            qe.NoLock = true;
            var entities = service.RetrieveMultiple(qe);
            if (entities.Entities.Count > 0)
            {
                exists = true;
                entities.Entities.ToList().ForEach(e => service.Delete(qe.EntityName, e.Id));
            }
            return exists;
        }

        private static bool DeleteIfExistsInternal(IOrganizationService service, string logicalName, Guid id)
        {
            var exists = false;
            var idName = EntityHelper.GetIdAttributeName(logicalName);
            var qe = new QueryExpression(logicalName) { ColumnSet = new ColumnSet(idName) };

            qe.WhereEqual(idName, id);
            qe.First();
            qe.NoLock = true;
            if (service.RetrieveMultiple(qe).Entities.Count > 0)
            {
                service.Delete(logicalName, id);
                exists = true;
            }
            return exists;
        }

        /// <summary>
        /// There have been Generic SQL errors caused with calling this while using multi-threading.  This hopefully
        /// will fix that
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="id">The id.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <returns></returns>
        private static bool DeleteIfExistsWithRetry(IOrganizationService service, string entityName, Guid id,
                                                    int retryCount)
        {
            bool exists;
            try
            {
                exists = DeleteIfExistsInternal(service, entityName, id);
            }
            catch (System.ServiceModel.FaultException<OrganizationServiceFault> ex)
            {
                if (retryCount < 10 && ex.Message.Equals("Generic SQL error.", StringComparison.CurrentCultureIgnoreCase))
                { // This is normally caused by database deadlock issue.  
                    // Attempt to reprocess once after sleeping a random number of milliseconds
                    System.Threading.Thread.Sleep(new Random(System.Threading.Thread.CurrentThread.ManagedThreadId).
                        Next(1000, 5000));
                    exists = DeleteIfExistsWithRetry(service, entityName, id, retryCount + 1);
                }
                else if (ex.Message.EndsWith(id + " Does Not Exist"))
                {
                    exists = false;
                }
                else if (ex.Message == "The object you tried to delete is associated with another object and cannot be deleted.")
                {
                    throw new Exception("Entity " + entityName + " (" + id + ") is associated with another object and cannot be deleted.");
                }
                else
                {
                    throw;
                }
            }

            return exists;
        }

        #endregion DeleteIfExists

        /// <summary>
        /// Executes a batch of requests against the CRM Web Service using the ExecuteMultipleRequest command.
        /// </summary>
        /// <remarks>
        /// ExecuteMultipleRequest allows for a maximum of 1000 messages to be processed in a single batch job.
        /// </remarks>
        /// <param name="service">Organization Service proxy for connecting to the relevant CRM instance.</param>
        /// <param name="requestCollection">Collection of organization requests to execute against the CRM Web Services.</param>
        /// <param name="returnResponses">Indicates if responses should be returned for the action taken on each entity in the bulk operation.</param>
        /// <param name="continueOnError">Indicates if the batch job should continue if an error occurs for any of the entities being processed. Default is true.</param>
        /// <returns>Returns the <see cref="ExecuteMultipleResponse"/> containing responses and faults from the operation if the returnResponses parameter is set to true; otherwise returns null. Default is true.</returns>
        public static ExecuteMultipleResponse ExecuteMultiple(this IOrganizationService service, OrganizationRequestCollection requestCollection, bool returnResponses = true, bool continueOnError = true)
        {
            // Validate required parameters.
            if (service == null)
                throw new ArgumentNullException(nameof(service), "A valid Organization Service Proxy must be specified.");
            // Validate the request collection.
            if (requestCollection == null)
                throw new ArgumentNullException(nameof(requestCollection), "The collection of requests to batch process cannot be null.");
            // Ensure the user is not attempting to pass in more than 1000 requests for the batch job, as this is the maximum number CRM allows within a single batch.
            if (requestCollection.Count > 1000)
                throw new ArgumentOutOfRangeException(nameof(requestCollection), "The Entity Collection cannot contain more than 1000 items, as that is the maximum number of messages that can be processed by the CRM web services in a single batch.");

            try
            {
                // Instantiate a new ExecuteMultipleRequest.
                var multipleRequest = new ExecuteMultipleRequest
                {
                    Settings = new ExecuteMultipleSettings { ContinueOnError = continueOnError, ReturnResponses = returnResponses },
                    Requests = requestCollection
                };

                return service.Execute(multipleRequest) as ExecuteMultipleResponse;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while executing an ExecuteMultipleRequest. See inner exception for details.", ex);
            }
        }

        #region GetAllEntities

        /// <summary>
        /// Gets all entities using the Query Expression
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="qe">Query Expression to Execute.</param>
        /// <param name="maxCount">The maximum number of entities to retrieve.  Use null for default.</param>
        /// <param name="pageSize">Number of records to return in each fetch.  Use null for default.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllEntities<T>(this IOrganizationService service, QueryExpression qe, int? maxCount = null, int? pageSize = null)
            where T : Entity
        {
            return RetrieveAllEntities<T>.GetAllEntities(service, qe, maxCount, pageSize);
        }

        /// <summary>
        /// Gets all entities using the Query Expression
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="qe">Query Expression to Execute.</param>
        /// <param name="maxCount">The maximum number of entities to retrieve.  Use null for default.</param>
        /// <param name="pageSize">Number of records to return in each fetch.  Use null for default.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllEntities<T>(this IOrganizationService service, TypedQueryExpression<T> qe, int? maxCount = null, int? pageSize = null)
            where T : Entity
        {
            return RetrieveAllEntities<T>.GetAllEntities(service, qe, maxCount, pageSize);
        }

        #endregion GetAllEntities

        /// <summary>
        /// Returns the WhoAmIResponse to determine the current user's UserId, BusinessUnitId, and OrganizationId
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static WhoAmIResponse GetCurrentlyExecutingUserInfo(this IOrganizationService service)
        {
            return (WhoAmIResponse)service.Execute(new WhoAmIRequest());
        }

        #region GetEntity

        /// <summary>
        /// Retrieves the Entity of the given type with the given Id, with all columns
        /// </summary>
        /// <typeparam name="T">An early bound Entity Type</typeparam>
        /// <param name="service">open IOrganizationService</param>
        /// <param name="id">Primary Key of Entity</param>
        /// <returns></returns>
        public static T GetEntity<T>(this IOrganizationService service, Guid id)
            where T : Entity
        {
            return service.GetEntity<T>(id, SolutionCheckerAvoider.CreateColumnSetWithAllColumns());
        }

        /// <summary>
        /// Retrieves the Entity of the given type with the given Id, with the given columns
        /// </summary>
        /// <typeparam name="T">An early bound Entity Type</typeparam>
        /// <param name="service">open IOrganizationService</param>
        /// <param name="id">Primary Key of Entity</param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <returns></returns>
        public static T GetEntity<T>(this IOrganizationService service, Guid id, Expression<Func<T, object>> anonymousTypeInitializer)
            where T : Entity
        {
            return service.GetEntity<T>(id, AddColumns(new ColumnSet(), anonymousTypeInitializer));
        }

        /// <summary>
        /// Retrieves the Entity of the given type with the given Id, with the given columns
        /// </summary>
        /// <typeparam name="T">An early bound Entity Type</typeparam>
        /// <param name="service">open IOrganizationService</param>
        /// <param name="id">Primary Key of Entity</param>
        /// <param name="columnSet">Columns to retrieve</param>
        /// <returns></returns>
        public static T GetEntity<T>(this IOrganizationService service, Guid id, ColumnSet columnSet)
            where T : Entity
        {
            return service.Retrieve(EntityHelper.GetEntityLogicalName<T>(), id, columnSet).AsEntity<T>();
        }

        #endregion GetEntity

        #region GetEntityLogicalName

        private static readonly ConcurrentDictionary<int, string> ObjectTypeToLogicalNameMapping = new ConcurrentDictionary<int, string>();
        private static readonly object ObjectTypeToLogicalNameMappingLock = new object();
        /// <summary>
        /// Gets the Entity Logical Name for the given object Type Code
        /// </summary>
        /// <param name="service"></param>
        /// <param name="objectTypeCode">The Object Type Code</param>
        /// <param name="useCache">Allows for caching the calls in a thread safe manner</param>
        /// <returns></returns>
        public static string GetEntityLogicalName(this IOrganizationService service, int objectTypeCode, bool useCache = true)
        {
            return useCache
                ? ObjectTypeToLogicalNameMapping.GetOrAddSafe(ObjectTypeToLogicalNameMappingLock, objectTypeCode, c => GetEntityLogicalNameInternal(service, c))
                : GetEntityLogicalNameInternal(service, objectTypeCode);
        }

        private static string GetEntityLogicalNameInternal(this IOrganizationService service, int objectTypeCode)
        {
            var entityFilter = new MetadataFilterExpression(LogicalOperator.And);
            entityFilter.Conditions.Add(new MetadataConditionExpression("ObjectTypeCode", MetadataConditionOperator.Equals, objectTypeCode));
            var propertyExpression = new MetadataPropertiesExpression { AllProperties = false };
            propertyExpression.PropertyNames.Add("LogicalName");

            var response = (RetrieveMetadataChangesResponse)service.Execute(new RetrieveMetadataChangesRequest
            {
                Query = new EntityQueryExpression
                {
                    Criteria = entityFilter,
                    Properties = propertyExpression
                }
            });

            return response.EntityMetadata.Count >= 1
                ? response.EntityMetadata[0].LogicalName
                : null;
        }

        #endregion GetEntityLogicalName

        #region GetEntities

        /// <summary>
        /// Returns first 5000 entities using the Query Expression
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="qb">Query to Execute.</param>
        /// <returns></returns>
        public static List<T> GetEntities<T>(this IOrganizationService service, QueryBase qb) where T : Entity
        {
            return service.RetrieveMultiple(qb).ToEntityList<T>();
        }

        /// <summary>
        /// Returns first 5000 entities using the Query Expression
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="qe">Query Expression to Execute.</param>
        /// <returns></returns>
        public static List<T> GetEntities<T>(this IOrganizationService service, TypedQueryExpression<T> qe) where T : Entity
        {
            return service.RetrieveMultiple(qe).ToEntityList<T>();
        }

        #endregion GetEntities

        #region GetFirst

        /// <summary>
        /// Gets the first entity that matches the query expression.  An exception is thrown if none are found.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="qe">The query expression.</param>
        /// <returns></returns>
        public static Entity GetFirst(this IOrganizationService service, QueryExpression qe)
        {
            var entity = service.GetFirstOrDefault(qe);
            AssertExists(entity, qe);
            return entity;
        }

        /// <summary>
        /// Gets the first entity that matches the query expression.  An exception is thrown if none are found.
        /// </summary>
        /// <typeparam name="T">The Entity Type.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="qe">The query expression.</param>
        /// <returns></returns>
        public static T GetFirst<T>(this IOrganizationService service, QueryExpression qe) where T : Entity
        {
            var entity = service.GetFirstOrDefault<T>(qe);
            AssertExists(entity, qe);
            return entity;
        }

        /// <summary>
        /// Gets the first entity that matches the query expression.  An exception is thrown if none are found.
        /// </summary>
        /// <typeparam name="T">The Entity Type.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="qe">The query expression.</param>
        /// <returns></returns>
        public static T GetFirst<T>(this IOrganizationService service, TypedQueryExpression<T> qe) where T : Entity
        {
            var entity = service.GetFirstOrDefault(qe);
            AssertExists(entity, qe);
            return entity;
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private static void AssertExists<T>(T entity, QueryExpression qe) where T : Entity
        {
            if (entity == null)
            {
                throw new InvalidOperationException("No " + EntityHelper.GetEntityLogicalName<T>() + " found for query " +
                                                    qe.GetSqlStatement());
            }
        }

        #endregion GetFirst

        #region GetFirstOrDefault

        /// <summary>
        /// Gets the first entity that matches the query expression.  Null is returned if none are found.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public static Entity GetFirstOrDefault(this IOrganizationService service, QueryBase query)
        {
            query.First();
            return service.RetrieveMultiple(query).Entities.FirstOrDefault();
        }

        /// <summary>
        /// Gets the first entity that is returned by the fetch expression.  Null is returned if none are found.
        /// </summary>
        /// <typeparam name="T">The Entity Type.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="fe">The fetch expression.</param>
        /// <returns></returns>
        public static T GetFirstOrDefault<T>(this IOrganizationService service, FetchExpression fe) where T : Entity
        {
            var entity = service.RetrieveMultiple(fe).Entities.FirstOrDefault();
            return entity?.AsEntity<T>();
        }

        /// <summary>
        /// Gets the first entity that matches the query.  Null is returned if none are found.
        /// </summary>
        /// <typeparam name="T">The Entity Type.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="qb">The query.</param>
        /// <returns></returns>
        public static T GetFirstOrDefault<T>(this IOrganizationService service, QueryBase qb) where T : Entity
        {
            qb.First();
            return service.GetEntities<T>(qb).FirstOrDefault();
        }

        /// <summary>
        /// Gets the first entity that matches the query expression.  Null is returned if none are found.
        /// </summary>
        /// <typeparam name="T">The Entity Type.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="qe">The query expression.</param>
        /// <returns></returns>
        public static T GetFirstOrDefault<T>(this IOrganizationService service, TypedQueryExpression<T> qe) where T : Entity
        {
            return service.GetFirstOrDefault<T>(qe.Query);
        }

        #endregion GetFirstOrDefault

        /// <summary>
        /// Gets the local time from the UTC time.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="userId">The id of the user to lookup the timezone code user settings</param>
        /// <param name="utcTime">The given UTC time to find the user's local time for.  Defaults to DateTime.UtcNow</param>
        /// <param name="defaultTimeZoneCode">Default TimeZoneCode if the user has no TimeZoneCode defined.  Defaults to EDT.</param>
        public static DateTime GetUserLocalTime(this IOrganizationService service, Guid userId, DateTime? utcTime, int defaultTimeZoneCode = 35)
        {
            var timeZoneCode = RetrieveUserSettingsTimeZoneCode(service, userId) ?? defaultTimeZoneCode;
            var request = new LocalTimeFromUtcTimeRequest
            {
                TimeZoneCode = timeZoneCode,
                UtcTime = utcTime ?? DateTime.UtcNow
            };

            var response = (LocalTimeFromUtcTimeResponse)service.Execute(request);

            return response.LocalTime;
        }

        /// <summary>
        /// Retrieves the current users TimeZoneCode
        /// </summary>
        private static int? RetrieveUserSettingsTimeZoneCode(IOrganizationService service, Guid userId)
        {
            // ReSharper disable StringLiteralTypo
            var setting = service.GetFirstOrDefault("usersettings", new ColumnSet("timezonecode"), "systemuserid", userId);
            return setting?.GetAttributeValue<int?>("timezonecode");
            // ReSharper restore StringLiteralTypo
        }

        #region InitializeFrom

        /// <summary>
        /// Utilizes the standard OOB Mappings from CRM to hydrate fields on child record from a parent.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="parentEntity">The Parent Entity.</param>
        /// <param name="childLogicalName">The logical name of the child</param>
        /// <param name="targetFieldType">The Target Field Type</param>
        /// <returns></returns>
        public static Entity InitializeFrom(this IOrganizationService service, EntityReference parentEntity, string childLogicalName, TargetFieldType targetFieldType = TargetFieldType.All)
        {
            var initialize = new InitializeFromRequest
            {
                TargetEntityName = childLogicalName,
                EntityMoniker = parentEntity,
                TargetFieldType = targetFieldType
            };
            var initialized = (InitializeFromResponse)service.Execute(initialize);

            return initialized.Entity;
        }

        /// <summary>
        /// Utilizes the standard OOB Mappings from CRM to hydrate fields on child record from a parent.
        /// </summary>
        /// <typeparam name="T">The Entity Type to Return</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="parentEntity">The Parent Entity.</param>
        /// <param name="targetFieldType">The Target Field Type</param>
        /// <returns></returns>
        public static T InitializeFrom<T>(this IOrganizationService service, EntityReference parentEntity, TargetFieldType targetFieldType = TargetFieldType.All) where T : Entity
        {
            var initialize = new InitializeFromRequest
            {
                TargetEntityName = EntityHelper.GetEntityLogicalName<T>(),
                EntityMoniker = parentEntity,
                TargetFieldType = targetFieldType
            };
            var initialized = (InitializeFromResponse)service.Execute(initialize);

            return initialized.Entity.AsEntity<T>();
        }

        #endregion InitializeFrom

        /// <summary>
        /// Sets the state
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entity">The entity to set the state of.</param>
        /// <param name="state">The state to change the entity to.</param>
        /// <param name="status">The status to change the entity to.</param>
        /// <returns></returns>
        public static void SetState(this IOrganizationService service, EntityReference entity, int state, int? status = null)
        {
            var info = new LateBoundActivePropertyInfo(entity.LogicalName);

            var local = new Entity(entity.LogicalName)
            {
                Id = entity.Id,
                [info.AttributeName] = info.ActiveAttribute == ActiveAttributeType.IsDisabled
                    ? (object)(state == 1)
                    : new OptionSetValue(state)
            };
            if (status.HasValue)
            {
                local["status"] = new OptionSetValue(status.Value);
            }

            service.Update(local);
        }

        /// <summary>
        /// Sets the state
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entity">The entity to set the state of.</param>
        /// <param name="state">The state to change the entity to.</param>
        /// <param name="status">The status to change the entity to.</param>
        /// <returns></returns>
        public static void SetState(this IOrganizationService service, Entity entity, int state, int? status = null)
        {
            SetState(service, entity.ToEntityReference(), state, status);
        }

        /// <summary>
        /// Currently only tested against System Users.  Not sure if it will work with other entities
        /// May need to rename this to SetSystemUserState
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="logicalName">logical name of the entity.</param>
        /// <param name="id">The id of the entity.</param>
        /// <param name="active">if set to <c>true</c> [active].</param>
        /// <returns></returns>
        public static void SetState(this IOrganizationService service, string logicalName, Guid id, bool active)
        {
            SetState(service, new EntityReference(logicalName, id), active);
        }

        /// <summary>
        /// Currently only tested against System Users.  Not sure if it will work with other entities
        /// May need to rename this to SetSystemUserState
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="active">if set to <c>true</c> [active].</param>
        /// <returns></returns>
        public static void SetState(this IOrganizationService service, EntityReference entity, bool active)
        {
            var info = new LateBoundActivePropertyInfo(entity.LogicalName);
            var state = active ?
                info.ActiveState ?? 0 :
                info.NotActiveState ?? (info.ActiveState == 1 ? 0 : 1);

            SetState(service, entity, state);
        }

        /// <summary>
        /// Attempts to delete the Entity, eating the error if it doesn't exist
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="logicalName">Logical name of the entity.</param>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static bool TryDelete(this IOrganizationService service, string logicalName, Guid id)
        {
            var exists = false;
            try
            {
                service.Delete(logicalName, id);
                exists = true;
            }
            catch (System.ServiceModel.FaultException<OrganizationServiceFault> ex)
            {
                if (!ex.Message.EndsWith(id + " Does Not Exist"))
                {
                    throw;
                }
            }

            return exists;
        }

        #region UpdateWithSupressDuplicateDetection

        /// <summary>
        /// Creates a record with SuppressDuplicateDetection Enabled to Ignore any potential Duplicates Created
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static void UpdateWithSuppressDuplicateDetection(this IOrganizationService service, Entity entity)
        {
            service.Execute(new UpdateRequest
            {
                Target = entity,
                ["SuppressDuplicateDetection"] = true
            });
        }

        #endregion CreateWithSupressDuplicateDetection

#if !PRE_KEYATTRIBUTE
        /// <summary>
        /// Updates or insert a record in CRM.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static UpsertResponse Upsert(this IOrganizationService service, Entity entity)
        {
            return (UpsertResponse) service.Execute(new UpsertRequest
            {
                Target = entity
            });
        }
#endif
    }
}
