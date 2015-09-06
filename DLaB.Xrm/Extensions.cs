using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using DLaB.Xrm.Messages;

namespace DLaB.Xrm
{
    /// <summary>
    /// Contains extension methods for improving developement speed and readability.
    /// </summary>
    public static partial class Extensions
    {
        #region AttributeMetadata

        public static bool IsLocalOptionSetAttribute(this AttributeMetadata attribute)
        {
            if (attribute.AttributeType == AttributeTypeCode.Picklist)
            {
                var picklist = attribute as PicklistAttributeMetadata;
                if (picklist != null)
                {
                    return picklist.OptionSet.IsGlobal.HasValue && !picklist.OptionSet.IsGlobal.Value;
                }
            }
            return false;
        } 

        #endregion // AttributeMetadata

        #region ConditionExpression

        /// <summary>
        /// Gets the SQL like statement of the ConditionExpression
        /// </summary>
        /// <param name="ce">The ConditionExpression</param>
        /// <returns></returns>
        public static string GetStatement(this ConditionExpression ce)
        {
            string condition;
            switch (ce.Operator)
            {
                case ConditionOperator.Null:
                case ConditionOperator.NotNull:
                    condition = String.Format("{0} is {1} ", ce.AttributeName, ce.Operator);
                    break;
                case ConditionOperator.In:
                case ConditionOperator.NotIn:
                    condition = String.Format("{0} {1} {{{2}}} ", ce.AttributeName, ce.Operator, String.Join(", ", ce.Values));
                    break;
                case ConditionOperator.Equal:
                    condition = String.Format("{0} = {1} ", ce.AttributeName, GetDisplayValue(ce.Values[0]));
                    break;
                default:
                    condition = String.Format("{0} {1} {2} ", ce.AttributeName, ce.Operator, GetDisplayValue(ce.Values[0]));
                    break;
            }
            return condition;
        }

        private static string GetDisplayValue(object value)
        {
            if (value == null) { return "<null>"; }
            var type = value.GetType();

            if (typeof(String).IsAssignableFrom(type))
            {
                return "\"" + value + "\"";
            }
            else if (typeof(Guid).IsAssignableFrom(type))
            {
                return "{" + value + "}";
            }
            else
            {
                return value.ToString();
            }
        }

        #endregion // ConditionExpression

        #region ColumnSet

        /// <summary>
        /// Allows for adding column names in an early bound manner:
        /// columnSet.AddColumns&lt;new_inquiry&gt;(i => new { i.new_inquiryId, i.new_zoneid, i.new_name });
        /// </summary>
        /// <typeparam name="T">An Entity Type</typeparam>
        /// <param name="columnSet">The ColumnSet</param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <returns></returns>
        public static ColumnSet AddColumns<T>(this ColumnSet columnSet, Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity
        {
            var initializer = anonymousTypeInitializer.Body as NewExpression;
            if (initializer == null || initializer.Members == null)
                throw new ArgumentException("lambda must return an object initializer");

            columnSet.AddColumns(initializer.Members.Select(member => member.Name.ToLower()).ToArray());
            return columnSet;
        }

        /// <summary>
        /// Adds all custom columns of the Entity to the column set, skipping the default CRM columns
        /// </summary>
        /// <typeparam name="T">The Type of Entity to get the properties for</typeparam>
        /// <param name="columns">The column set to add all custom columns too.</param>
        /// <returns></returns>
        public static ColumnSet AllCustomColumns<T>(this ColumnSet columns) where T : Entity
        {
            foreach (var column in typeof(T).GetProperties().Where(p =>
                        p.Name.Length >= 4 &&
                        p.Name[3] == '_' &&
                        p.PropertyType.Name != "IEnumerable`1" && // Don't return any 1:M relationships
                        !p.PropertyType.IsSubclassOf(typeof(Entity)))) // Don't return any relationships (should be EntityReference)
            {
                columns.AddColumn(column.Name.ToLower());
            }

            return columns;
        }

        #endregion // ColumnSet

        #region Entity

        /// <summary>
        /// Returns the Aliased Value for a column specified in a Linked entity
        /// </summary>
        /// <typeparam name="T">The type of the aliased attribute form the linked entity</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="attributeName">The aliased attribute from the linked entity.  Can be preappeneded with the
        /// linked entities logical name and a period. ie "Contact.LastName"</param>
        /// <returns></returns>
        public static T GetAliasedValue<T>(this Entity entity, string attributeName)
        {
            string aliasedEntityName = SplitAliasedAttributeEntityName(ref attributeName);

            AliasedValue aliased;
            foreach (var attribute in entity.Attributes.Values)
            {
                aliased = attribute as AliasedValue;
                if (entity.IsAttributeAliasedValue(attributeName, aliasedEntityName, aliased))
                {
                    try
                    {
                        return (T)aliased.Value;
                    }
                    catch (InvalidCastException)
                    {
                        throw new InvalidCastException(
                            String.Format("Unable to cast attribute {0}.{1} from type {2} to type {3}",
                                    aliased.EntityLogicalName, aliased.AttributeLogicalName,
                                    typeof(T).Name, aliased.Value.GetType().Name));
                    }
                }
            }

            throw new Exception("Aliased value with attribute " + attributeName +
                " was not found!  Only these attributes were found: " + String.Join(", ", entity.Attributes.Keys));
        }

        private static bool IsAttributeAliasedValue(this Entity entity, string attributeName, string aliasedEntityName, AliasedValue aliased)
        {
            bool value =
           (aliased != null &&
                (aliasedEntityName == null || aliasedEntityName == aliased.EntityLogicalName) &&
                aliased.AttributeLogicalName == attributeName);


            /// I believe there is a bug in CRM 2011 when dealing with aggregate values of a linked entity in FetchXML.
            /// Even though it is marked with an alias, the AliasedValue in the Attribute collection will use the 
            /// actual CRM name, rather than the aliased one, even though the AttributeCollection's key will correctly
            /// use the aliased name.  So if the aliased Attribute Logical Name doesn't match the assumed attribute name
            /// value, check to see if the entity contains an AliasedValue with that key whose attribute logical name 
            /// doesn't match the key (the assumed bug), and mark it as being the aliased attribute
            if (!value && aliased != null && entity.Contains(attributeName))
            {
                var aliasedByKey = entity[attributeName] as AliasedValue;
                if (aliasedByKey != null && aliasedByKey.AttributeLogicalName != attributeName &&
                    Object.ReferenceEquals(aliased, aliasedByKey))
                {
                    value = true;
                }
            }

            return value;
        }

        /// <summary>
        /// Gets the actual type of the entity as defined by the entity LogicalName, even if it is just an entity.
        /// ie: (new Entity(Contact.EntityLogicalName)).GetEntityType() == (new Contact()).GetType()
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static Type GetEntityType(this Entity entity)
        {
            var assembly = CrmService.GetEarlyBoundProxyAssembly();
            foreach (Type t in assembly.GetTypes())
            {
                var attribute =
                    (EntityLogicalNameAttribute)
                    t.GetCustomAttributes(typeof(EntityLogicalNameAttribute), false).FirstOrDefault();
                if (attribute != null && attribute.LogicalName == entity.LogicalName)
                {
                    return t;
                }
            }
            throw new Exception("Type " + entity.LogicalName + " Not found!");
        }

        /// <summary>
        /// Returns the name of the Attribute that contains the default name of the entity
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static String GetNameAttribute(this Entity entity)
        {
            var value = String.Empty;
            var prefix = Xrm.Common.Config.GetAppSettingOrDefault("EntityPrefix", "new");
            if (entity.Contains(prefix + "_name"))
            {
                value = prefix + "_name";
            }
            else if (entity.Contains("fullname"))
            {
                value = "fullname";
            }
            else if (entity.Contains("name"))
            {
                value = "name";
            }
            else if (entity.LogicalName.Contains('_'))
            {
                // Get prefix of entity
                prefix = entity.LogicalName.Substring(0, entity.LogicalName.IndexOf('_'));
                if (entity.Contains(prefix + "_name"))
                {
                    value = prefix + "_name";
                }
            }

            return value;
        }

        /// <summary>
        /// Returns the Name and Id of the Current Entity in this format "Name (id)"
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static String GetNameId(this Entity entity)
        {
            var value = String.Empty;
            var nameAttribute = entity.GetNameAttribute();
            if (nameAttribute != String.Empty && entity.Contains(nameAttribute))
            {
                value = entity.Attributes[nameAttribute] as String ?? "NULL";
            }

            return value + String.Format(" ({0})", entity.Id);
        }

        /// <summary>
        /// Returns the value for the attribute, or the default value for the type if it does not exist in the entity.
        /// </summary>
        /// <typeparam name="T">The attribute type</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns></returns>
        public static T GetOrDefault<T>(this Entity entity, string attributeName)
        {
            return entity.GetOrDefault(attributeName, default(T));
        }

        /// <summary>
        /// Returns the value for the attribute, or the default value for the type if it does not exist in the entity.
        /// </summary>
        /// <typeparam name="T">The attribute type</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="defaultValue">The value to use if the entity does not contain the attribute.</param>
        /// <returns></returns>
        public static T GetOrDefault<T>(this Entity entity, string attributeName, T defaultValue)
        {
            T value;
            if (entity.Contains(attributeName))
            {
                value = (T)entity[attributeName];
            }
            else
            {
                value = defaultValue;
            }
            return value;
        }

        /// <summary>
        /// Returns the Aliased Value for a column specified in a Linked entity
        /// </summary>
        /// <typeparam name="T">The type of the aliased attribute form the linked entity</typeparam>
        /// <param name="entity"></param>
        /// <param name="attributeName">The aliased attribute from the linked entity.  Can be preappeneded with the
        /// linked entities logical name and a period. ie "Contact.LastName"</param>
        /// <returns></returns>
        public static bool HasAliasedAttribute(this Entity entity, string attributeName)
        {
            string aliasedEntityName = SplitAliasedAttributeEntityName(ref attributeName);
            return entity.Attributes.Values.Any(a =>
                entity.IsAttributeAliasedValue(attributeName, aliasedEntityName, a as AliasedValue));
        }

        /// <summary>
        /// Handles spliting the attributeName if it is formated as "EntityAliasedName.AttributeName",
        /// updating the attribute name and returning the aliased EntityName
        /// </summary>
        /// <param name="attributeName"></param>
        private static string SplitAliasedAttributeEntityName(ref string attributeName)
        {
            string aliasedEntityName = null;
            if (attributeName.Contains('.'))
            {
                var split = attributeName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length != 2)
                {
                    throw new Exception("Attribute Name was specified for an Alaised Value with " + split.Length +
                    " split parts, and two were expected.  Attribute Name = " + attributeName);
                }
                aliasedEntityName = split[0];
                attributeName = split[1];
            }

            return aliasedEntityName;
        }

        #endregion // Entity

        #region EntityCollection

        /// <summary>
        /// Converts the entity collection into a list, casting each entity.
        /// </summary>
        /// <typeparam name="T">The type of Entity</typeparam>
        /// <param name="col">The collection to convert</param>
        /// <returns></returns>
        public static List<T> ToEntityList<T>(this EntityCollection col) where T : Entity
        {
            List<T> values = new List<T>();
            foreach (Entity entity in col.Entities)
            {
                values.Add(entity.ToEntity<T>());
            }
            return values;
        }

        #endregion // EntityCollection

        #region EntityMetadata

        public static string GetDisplayNameWithLogical(this EntityMetadata entity)
        {
            return entity.DisplayName.GetLocalOrDefaultText(entity.SchemaName) + " (" + entity.LogicalName + ")";
        }

        #endregion // EntityMetadata

        #region Entity Reference

        /// <summary>
        /// Returns the Name and Id of an entity reference in this format "Name (id)"
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static String GetNameId(this EntityReference entity)
        {
            return String.Format("{0} ({1})", entity.Name, entity.Id);
        }

        #endregion // Entity Reference

        #region FilterExpression

        /// <summary>
        /// Depending on the Type of T, adds the correct is active criteria Statment
        /// Note: Use AddLink&lt;T&gt; for Linked Entities
        /// </summary>
        /// <typeparam name="T">The type of Entity use in adding an active only condition</typeparam>
        /// <param name="fe">The filter expression.</param>
        /// <returns></returns>
        public static FilterExpression ActiveOnly<T>(this FilterExpression fe) where T : Entity
        {
            var type = typeof(T);
            if (type.GetProperty("isdisabled") != null || type.GetProperty("IsDisabled") != null)
            {
                var ce = new ConditionExpression("isdisabled", ConditionOperator.Equal, false);
                fe.AddCondtionEnforceAndFilterOperator(ce);
            }
            else if (type.GetProperty("statecode") != null || type.GetProperty("StateCode") != null)
            {
                switch (type.Name)
                {
                    // Entities with a Canceled State
                    case "ActivityPointer":
                    case "Appointment":
                    case "BulkOperation":
                    case "CampaignActivity":
                    case "ContractDetail":
                    case "Email":
                    case "Fax":
                    case "Letter":
                    case "OrderClose":
                    case "PhoneCall":
                    case "QuoteClose":
                    case "RecurringAppointmentMaster":
                    case "ServiceAppointment":
                    case "TaskState":
                        fe.AddCondtionEnforceAndFilterOperator(new ConditionExpression("statecode", ConditionOperator.NotEqual, 2));
                        break;

                    case "DuplicateRule": // don't ask me why, but this one is flipped
                        fe.StateIs(1);
                        break;

                    // Entities with states that can't be grouped into seperate all incluse active and inactive states
                    // No constraint will be added for these entities
                    case "AsyncOperation":
                    case "BulkDeleteOperation":
                    case "Contract":
                    case "Lead":
                    case "Opportunity":
                    case "ProcessSession":
                    case "Quote":
                    case "SdkMessageProcessingStep":
                    case "Workflow":
                        // Do Nothing
                        break;

                    default:
                        fe.StateIs(0);
                        break;
                }
            }
            return fe;
        }

        /// <summary>
        /// Returns a SQL-ish Representation of the filter
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public static string GetStatement(this FilterExpression filter)
        {
            var conditions = new List<String>();
            foreach (var condition in filter.Conditions)
            {
                conditions.Add(GetStatement(condition));
            }
            if (filter.Conditions.Count > 0 && filter.Filters.Count > 0)
            {
                conditions[conditions.Count - 1] += Environment.NewLine;
            }
            foreach (var child in filter.Filters)
            {
                conditions.Add(GetStatement(child) + Environment.NewLine);
            }
            string join = filter.FilterOperator.ToString().PadLeft(3, ' ') + " ";
            string statement = String.Join(join, conditions.ToArray());
            if (String.IsNullOrWhiteSpace(statement))
            {
                return string.Empty;
            }
            else
            {
                return "( " + statement + ") ";
            }
        }

        /// <summary>
        /// Adds a Condition expression to the filter expression to force the statecode to be a specfic value.
        /// </summary>
        /// <param name="fe">The Filter Expression.</param>
        /// <param name="entityStateEnum">The entity state enum value.</param>
        /// <returns>The Filter expression with the conditionexpression added</returns>
        public static FilterExpression StateIs(this FilterExpression fe, object entityStateEnum)
        {
            fe.AddCondtionEnforceAndFilterOperator(new ConditionExpression("statecode", ConditionOperator.Equal, (int)entityStateEnum));
            return fe;
        }

        #endregion // FilterExpression


        #region IEnumerable<EntityMetadata>

        public static object[] ToObjectCollectionArray(this IEnumerable<EntityMetadata> entities)
        {
            return entities.
                Select(e => new ObjectCollectionItem<EntityMetadata>(e.GetDisplayNameWithLogical(), e)).
                OrderBy(r => r.DisplayName).Cast<object>().ToArray();
        }

        #endregion // IEnumerable<EntityMetadata>

        #region IExtensibleDataObject

        /// <summary>
        /// Serializes the specified obj, returning it's xml serialized value.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static string Serialize(this IExtensibleDataObject obj)
        {
            var serializer = new NetDataContractSerializer();
            var writer = new StringWriter(CultureInfo.InvariantCulture);
            serializer.WriteObject(new System.Xml.XmlTextWriter(writer), obj);
            return writer.ToString();
        }

        #endregion // IExtensibleDataObject

        #region IOrganizationService

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
        /// Performs an Asynchronous delete using the query expression.
        /// Use the querySet overload if performing multiple deletes
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="qe">Query Expression used to determine what entities to delete.</param>
        /// <returns></returns>
        public static BulkDeleteResult Delete(this IOrganizationService service, QueryExpression qe)
        {
            return BulkDelete.Delete(service, new QueryExpression[] { qe });
        }

        /// <summary>
        /// Performs an Asynchronous delete using the queryset.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="querySet">Array of Query Expression to delete</param>
        /// <returns></returns>
        public static BulkDeleteResult Delete(this IOrganizationService service, QueryExpression[] querySet)
        {
            return BulkDelete.Delete(service, querySet);
        }

        private static void DeleteIfExists(CrmService settings, string entityName, Guid id)
        {
            using (var service = CrmService.CreateOrgProxy(settings))
            {
                DeleteIfExists(service, entityName, id);
            }
        }

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
        /// There have been Generic SQL errors casued with calling this while using multi-threading.  This hopefully
        /// will fix that
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="id">The id.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <returns></returns>
        private static bool DeleteIfExistsWithRetry(IOrganizationService service, string entityName, Guid id, int retryCount)
        {
            bool exists = false;
            try
            {
                exists = DeleteIfExistsInternal(service, entityName, id);
            }
            catch (System.ServiceModel.FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                if (retryCount < 10 && ex.Message.Equals("Generic SQL error.", StringComparison.CurrentCultureIgnoreCase))
                { // This is normally caused by database deadlock issue.  
                    // Attempt to reprocess once after sleeping a random number of milliseconds
                    System.Threading.Thread.Sleep(new Random(System.Threading.Thread.CurrentThread.ManagedThreadId).Next(1000, 5000));
                    exists = DeleteIfExistsWithRetry(service, entityName, id, retryCount + 1);
                }
                else if (!ex.Message.EndsWith(id.ToString() + " Does Not Exist"))
                {
                    exists = false;
                }
                else
                {
                    throw;
                }
            }

            return exists;
        }

        private static bool DeleteIfExistsInternal(IOrganizationService service, string logicalName, Guid id)
        {
            bool exists = false;
            var qe = new QueryExpression(logicalName);
            qe.ColumnSet = new ColumnSet(logicalName + "id");

            qe.WhereEqual(logicalName + "id", id);
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
        /// Gets all active entities using the Query Expression
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="qe">Query Expression to Execute.  Validating that the StateCode is 0 or isdisabled is false,
        /// is added to the Query expression</param>
        /// <param name="maxCount">The maximum number of entities to retrieve.  Use null for default.</param>
        /// <param name="pageSize">Number of records to return in each fetch.  Use null for default.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllEntities<T>(this IOrganizationService service, QueryExpression qe, int? maxCount = null, int? pageSize = null)
            where T : Entity
        {
            qe.ActiveOnly<T>();
            return RetrieveAllEntities<T>.GetAllEntities(service, qe, maxCount, pageSize);
        }

        /// <summary>
        /// Gets all entities (DOES NOT ensure active only) using the Query Expression
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="qe">Query Expression to Execute.</param>
        /// <returns></returns>
        public static List<T> GetEntities<T>(this IOrganizationService service, QueryExpression qe)
            where T : Entity
        {
            return service.RetrieveList<T>(qe);
        }

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
            return service.GetEntity<T>(id, new ColumnSet(true));
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
            var columnSet = AddColumns<T>(new ColumnSet(), anonymousTypeInitializer);
            return service.GetEntity<T>(id, columnSet);
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
            return service.Retrieve(GetEntityLogicalName<T>(), id, columnSet).ToEntity<T>();
        }

        internal static string GetEntityLogicalName<T>()
        {
            var field = typeof(T).GetField("EntityLogicalName");
            if (field == null)
            {
                if (typeof(T) == typeof(Entity))
                {
                    return "entity";
                }
                else
                {
                    throw new Exception("Type " + typeof(T).FullName + " does not contain an EntityLogicalName Field");
                }
            }
            return (string)field.GetValue(null);
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
            AssertExists<T>(entity, qe);
            return entity;
        }

        private static void AssertExists<T>(T entity, QueryExpression qe) where T : Entity
        {
            if (entity == null)
            {
                throw new InvalidOperationException("No " + GetEntityLogicalName<T>() + " found for query " +
                    qe.GetSQLStatement());
            }
        }

        /// <summary>
        /// Gets the first entity that matches the query expression.  Null is returned if none are found.
        /// </summary>
        /// <typeparam name="T">The Entity Type.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="qe">The query expression.</param>
        /// <returns></returns>
        public static T GetFirstOrDefault<T>(this IOrganizationService service, QueryExpression qe) where T : Entity
        {
            qe.First();
            return service.RetrieveList<T>(qe).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the entity from CRM by id and if it doesn't exist, it creates it
        /// </summary>
        /// <typeparam name="T">The type of Entity</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="id">Id of entity</param>
        /// <returns></returns>
        public static T GetOrCreateEntity<T>(this IOrganizationService service, Guid id)
                where T : Entity, new()
        {
            return service.InitializeEntity<T>(id, e => { });
        }

        /// <summary>
        /// Retrieves the entity from CRM by id and if it doesn't exist, it creates it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <param name="id">Id of entity</param>
        /// <returns></returns>
        public static T GetOrCreateEntity<T>(this IOrganizationService service, Expression<Func<T, object>> anonymousTypeInitializer, Guid id)
            where T : Entity, new()
        {
            var columnSet = AddColumns<T>(new ColumnSet(), anonymousTypeInitializer);
            return service.GetOrCreateEntity<T>(columnSet, id);
        }

        /// <summary>
        /// Retrieves the entity from CRM by id and if it doesn't exist, it creates it
        /// </summary>
        /// <typeparam name="T">The type of Entity</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="columnSet">The column set.</param>
        /// <param name="id">Id of the entity</param>
        /// <returns></returns>
        public static T GetOrCreateEntity<T>(this IOrganizationService service, ColumnSet columnSet, Guid id)
                where T : Entity, new()
        {
            return service.InitializeEntity<T>(columnSet, id, e => { });
        }

        /// <summary>
        /// Returns a unqiue string key for the given IOrganizationService
        /// If the service is remote, the uri will be used, including the org Name
        /// If the service is on the server (ie plugin), the org id will be used
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static string GetOrganizationKey(this IOrganizationService service)
        {
            var uri = service.GetServiceUri();
            if (uri == null)
            {
                // Already on the server, grab organization guid 
                // Service should be of type Microsoft.Crm.Extensibility.InProcessServiceProxy which is an internal class.  Use reflection to grab the private field Org Id;
                var field = service.GetType().GetField("_organizationId", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (field == null)
                {
                    return String.Empty;
                }
                else
                {
                    var orgId = field.GetValue(service).ToString();
                    return "localOrgId" + orgId;
                }
            }
            else
            {
                return uri.Host + "|" + uri.Segments[1].Replace(@"/", "|");
            }
        }

        /// <summary>
        /// Assumes that this service is of type ServiceProxy&gt;IOrganizationService&lt;
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        public static Uri GetServiceUri(this IOrganizationService service)
        {
            var clientService = service as Microsoft.Xrm.Client.Services.OrganizationService;
            if (clientService != null)
            {
                service = clientService.InnerService;
            }

            var proxy = (ServiceProxy<IOrganizationService>)service;
            if (proxy.ServiceConfiguration == null)
            {
                return null;
            }

            return proxy.ServiceConfiguration.CurrentServiceEndpoint.Address.Uri;
        }

        /// <summary>
        /// Retrieves the entity from CRM by id (if it doesn't exist, it creates it), then initializes it, and saves
        /// it back to CRM
        /// </summary>
        /// <typeparam name="T">The type of Entity</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="id">Id of entity</param>
        /// <param name="initialize">function to execute on the entity to initialize it</param>
        /// <returns></returns>
        public static T InitializeEntity<T>(this IOrganizationService service, Guid id, Action<T> initialize)
            where T : Entity, new()
        {
            return service.InitializeEntity(id, initialize, (e, a) => { });
        }

        /// <summary>
        /// Retrieves the entity from CRM by id (if it doesn't exist, it creates it), then initializes it, and saves
        /// it back to CRM
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <param name="id">Id of entity</param>
        /// <param name="initialize">function to execute on the entity to initialize it</param>
        /// <returns></returns>
        public static T InitializeEntity<T>(this IOrganizationService service, Expression<Func<T, object>> anonymousTypeInitializer, Guid id, Action<T> initialize)
            where T : Entity, new()
        {
            var columnSet = AddColumns<T>(new ColumnSet(), anonymousTypeInitializer);
            return service.InitializeEntity<T>(columnSet, id, initialize);
        }

        /// <summary>
        /// Retrieves the entity from CRM by id (if it doesn't exist, it creates it), then initializes it, and saves
        /// it back to CRM
        /// </summary>
        /// <typeparam name="T">The type of Entity</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="columnSet">The column set.</param>
        /// <param name="id">Id of the entity</param>
        /// <param name="initialize">function to execute on the entity to initialize it</param>
        /// <returns></returns>
        public static T InitializeEntity<T>(this IOrganizationService service, ColumnSet columnSet, Guid id, Action<T> initialize)
            where T : Entity, new()
        {
            return service.InitializeEntity(columnSet, id, initialize, (e, a) => { });
        }

        /// <summary>
        /// Retrieves the entity from CRM by id (if it doesn't exist, it creates it), then initializes it, and saves
        /// it back to CRM
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        /// <param name="anonymousTypeInitializer">Function that initializes the Type </param>
        /// <param name="id">Id of entity</param>
        /// <param name="initialize">function to execute on the entity to initialize it</param>
        /// <param name="postInitialize">function to execute after the entity has been created or updated</param>
        /// <returns></returns>
        public static T InitializeEntity<T>(this IOrganizationService service, Expression<Func<T, object>> anonymousTypeInitializer, Guid id, Action<T> initialize, Action<T, InitializeAction> postInitialize) where T : Entity, new()
        {
            var columnSet = AddColumns<T>(new ColumnSet(), anonymousTypeInitializer);
            return service.InitializeEntity<T>(columnSet, id, initialize, postInitialize);
        }

        /// <summary>
        /// Retrieves the entity from CRM by id (if it doesn't exist, it creates it), then initializes it, and saves
        /// it back to CRM
        /// </summary>
        /// <typeparam name="T">The type of Entity</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="columnSet">The column set.</param>
        /// <param name="id">Id of entity</param>
        /// <param name="initialize">function to execute on the entity to initialize it</param>
        /// <param name="postInitialize">function to execute after the entity has been created or updated</param>
        /// <returns></returns>
        public static T InitializeEntity<T>(this IOrganizationService service, ColumnSet columnSet, Guid id,
            Action<T> initialize, Action<T, InitializeAction> postInitialize) where T : Entity, new()
        {
            string preImage = null;
            T entity = new T();
            var settings = new QuerySettings<T>()
            {
                ActiveOnly = false,
                Columns = columnSet,
                First = true
            };
            entity = service.RetrieveList<T>(settings.CreateExpression(entity.LogicalName + "id", id)).FirstOrDefault();
            if (entity == null)
            {
                entity = new T() { Id = id };
            }
            else
            {
                preImage = entity.Serialize();
            }

            initialize(entity);

            if (preImage == null)
            {
                service.Create(entity);
                postInitialize(entity, InitializeAction.Create);
            }
            else if (preImage != entity.Serialize())
            {
                service.Update(entity);
                postInitialize(entity, InitializeAction.Update);
            }
            else
            {
                postInitialize(entity, InitializeAction.NoDatabaseUpdate);
            }
            return entity;
        }

        /// <summary>
        /// Retrieves the entity from CRM by id (if it doesn't exist, it creates it), then initializes it, and saves
        /// it back to CRM
        /// </summary>
        /// <typeparam name="T">The type of Entity</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="id">Id of entity</param>
        /// <param name="initialize">function to execute on the entity to initialize it</param>
        /// <param name="postInitialize">function to execute after the entity has been created or updated</param>
        /// <returns></returns>
        public static T InitializeEntity<T>(this IOrganizationService service, Guid id, Action<T> initialize,
            Action<T, InitializeAction> postInitialize) where T : Entity, new()
        {
            return service.InitializeEntity<T>(new ColumnSet(true), id, initialize, postInitialize);
        }

        /// <summary>
        /// Describes what action was taken when attempting to intialize an entity
        /// </summary>
        public enum InitializeAction
        {
            /// <summary>
            /// The entity didn't exist, and so it was created.
            /// </summary>
            Create,
            /// <summary>
            /// The entity already existed but it's values were different, so it was updated.
            /// </summary>
            Update,
            /// <summary>
            /// The entity already existed but it's values were the same, so no database update was made
            /// </summary>
            NoDatabaseUpdate
        }

        /// <summary>
        /// Returns all entities, Short hand for service.RetrieveMultiple(qe).ToEntityList&lt;T&gt;()  Use GetAllEntities&lt;T&gt;(qe) if it needs to ensure that the entities are active
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="qe">Query Expression to Execute</param>
        /// <param name="maxCount">The maximum number of entities to retrieve.  Use null for default.</param>
        /// <param name="pageSize">Number of records to return in each fetch.  Use null for default.</param>
        /// <returns></returns>
        public static IEnumerable<T> RetrieveAllList<T>(this IOrganizationService service, QueryExpression qe, int? maxCount = null, int? pageSize = null) where T : Entity
        {
            return RetrieveAllEntities<T>.GetAllEntities(service, qe, maxCount, pageSize);
        }

        /// <summary>
        /// Returns first 5000 entities, Short hand for service.RetrieveMultiple(qe).ToEntityList&lt;T&gt;()  Use GetEntities&lt;T&gt;(qe) if it needs to ensure that the entities are active
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="qe">QueryExpression</param>
        /// <returns></returns>
        public static List<T> RetrieveList<T>(this IOrganizationService service, QueryExpression qe) where T : Entity
        {
            return service.RetrieveMultiple(qe).ToEntityList<T>();
        }

        /// <summary>
        /// If the id of the entity is empty, then the entity will be created, if it is not, then the entity will be
        /// updated
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static Guid Save(this IOrganizationService service, Entity entity)
        {
            Guid id = entity.Id;
            if (id == Guid.Empty)
            {
                id = service.Create(entity);
            }
            else
            {
                service.Update(entity);
            }
            return id;
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
        public static Microsoft.Crm.Sdk.Messages.SetStateResponse SetState(this IOrganizationService service, string logicalName, Guid id, bool active)
        {
            var setStateReq = new Microsoft.Crm.Sdk.Messages.SetStateRequest();
            setStateReq.EntityMoniker = new EntityReference(logicalName, id);
            setStateReq.State = new OptionSetValue(active ? 0 : 1);
            setStateReq.Status = new OptionSetValue(-1);

            return (Microsoft.Crm.Sdk.Messages.SetStateResponse)service.Execute(setStateReq);
        }

        /// <summary>
        /// Currently only tested against System Users.  Not sure if it will work with other entities
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entity">The entity to set the state of.</param>
        /// <param name="state">The state to change the entity to.</param>
        /// <param name="status">The status to change the entity to.</param>
        /// <returns></returns>
        public static Microsoft.Crm.Sdk.Messages.SetStateResponse SetState(this IOrganizationService service,
            Entity entity, int state, int? status)
        {
            var setStateReq = new Microsoft.Crm.Sdk.Messages.SetStateRequest();
            setStateReq.EntityMoniker = entity.ToEntityReference();
            setStateReq.State = new OptionSetValue(state);
            setStateReq.Status = new OptionSetValue(status ?? -1);

            return (Microsoft.Crm.Sdk.Messages.SetStateResponse)service.Execute(setStateReq);
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
            bool exists = false;
            try
            {
                service.Delete(logicalName, id);
                exists = true;
            }
            catch (System.ServiceModel.FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                if (!ex.Message.EndsWith(id.ToString() + " Does Not Exist"))
                {
                    throw;
                }
            }

            return exists;
        }

        #endregion // IOrganizationService

        #region Label

        public static string GetLocalOrDefaultText(this Label label, string defaultIfNull = null)
        {
            var local = label.UserLocalizedLabel;
            if (local == null)
            {
                local = label.LocalizedLabels.FirstOrDefault();
            }

            if (local == null)
            {
                return defaultIfNull;
            }
            else
            {
                return local.Label ?? defaultIfNull;
            }
        }

        #endregion // Label

        #region LinkEntity

        /// <summary>
        /// Adds a Condition expression to the link criteria of the link entity to force the statecode to be a specfic value.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="entityStateEnum">The entity state enum value.</param>
        /// <returns>The link entity with the condition expression added</returns>
        public static LinkEntity StateIs(this LinkEntity link, object entityStateEnum)
        {
            link.LinkCriteria.StateIs(entityStateEnum);
            return link;
        }

        #region AddChildLink

        /// <summary>
        /// Created do to possible bug (http://stackoverflow.com/questions/10722307/why-does-linkentity-addlink-initialize-the-linkfromentityname-with-its-own-lin)
        /// Adds the new LinkEntity as a child to this LinkEnity, rather than this LinkEntity's LinkFrom Entity
        /// Assumes that the linkFromAttributeName and the linkToAttributeName are the same
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="linkToEntityName">Name of the link to entity.</param>
        /// <param name="linkAttributesName">Name of the link from and link to attribute.</param>
        /// <returns></returns>
        public static LinkEntity AddChildLink(this LinkEntity link, string linkToEntityName, string linkAttributesName)
        {
            return link.AddChildLink(linkToEntityName, linkAttributesName, linkAttributesName);
        }

        /// <summary>
        /// Created do to possible bug (http://stackoverflow.com/questions/10722307/why-does-linkentity-addlink-initialize-the-linkfromentityname-with-its-own-lin)
        /// Adds the new LinkEntity as a child to this LinkEnity, rather than this LinkEntity's LinkFrom Entity
        /// Assumes that the linkFromAttributeName and the linkToAttributeName are the same
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="linkToEntityName">Name of the link to entity.</param>
        /// <param name="linkAttributesName">Name of the link from and link to attribute.</param>
        /// <param name="joinType">Type of the join to perform.</param>
        /// <returns></returns>
        public static LinkEntity AddChildLink(this LinkEntity link, string linkToEntityName, string linkAttributesName, JoinOperator joinType)
        {
            return link.AddChildLink(linkToEntityName, linkAttributesName, linkAttributesName, joinType);
        }

        /// <summary>
        /// Created do to possible bug (http://stackoverflow.com/questions/10722307/why-does-linkentity-addlink-initialize-the-linkfromentityname-with-its-own-lin)
        /// Adds the new LinkEntity as a child to this LinkEnity, rather than this LinkEntity's LinkFrom Entity
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="linkToEntityName">Name of the link to entity.</param>
        /// <param name="linkFromAttributeName">Name of the link from attribute.</param>
        /// <param name="linkToAttributeName">Name of the link to attribute.</param>
        /// <returns></returns>
        public static LinkEntity AddChildLink(this LinkEntity link, string linkToEntityName,
            string linkFromAttributeName, string linkToAttributeName)
        {
            return link.AddChildLink(linkToEntityName, linkFromAttributeName, linkToAttributeName, JoinOperator.Inner);
        }

        /// <summary>
        /// Created due to possible bug (http://stackoverflow.com/questions/10722307/why-does-linkentity-addlink-initialize-the-linkfromentityname-with-its-own-lin)
        /// Adds the new LinkEntity as a child to this LinkEnity, rather than this LinkEntity's LinkFrom Entity
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="linkToEntityName">Name of the link to entity.</param>
        /// <param name="linkFromAttributeName">Name of the link from attribute.</param>
        /// <param name="linkToAttributeName">Name of the link to attribute.</param>
        /// <param name="joinType">Type of the join to perform.</param>
        /// <returns></returns>
        public static LinkEntity AddChildLink(this LinkEntity link, string linkToEntityName,
            string linkFromAttributeName, string linkToAttributeName, JoinOperator joinType)
        {
            var child = new LinkEntity(
                link.LinkToEntityName, linkToEntityName,
                linkFromAttributeName, linkToAttributeName, joinType);
            link.LinkEntities.Add(child);
            return child;
        }

        #endregion // AddChildLink

        /// <summary>
        /// Assumes that the linkFromAttributeName and the linkToAttributeName are the same
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="linkToEntityName">Name of the link to entity.</param>
        /// <param name="linkAttributesName">Name of the link from and link to attribute.</param>
        /// <returns></returns>
        public static LinkEntity AddLink(this LinkEntity link, string linkToEntityName, string linkAttributesName)
        {
            return link.AddLink(linkToEntityName, linkAttributesName, linkAttributesName);
        }

        /// <summary>
        /// Assumes that the linkFromAttributeName and the linkToAttributeName are the same
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="linkToEntityName">Name of the link to entity.</param>
        /// <param name="linkAttributesName">Name of the link from and link to attribute.</param>
        /// <param name="joinType">Type of the join to perform.</param>
        /// <returns></returns>
        public static LinkEntity AddLink(this LinkEntity link, string linkToEntityName, string linkAttributesName, JoinOperator joinType)
        {
            return link.AddLink(linkToEntityName, linkAttributesName, linkAttributesName, joinType);
        }

        /// <summary>
        /// Adds the type T as a child linked entity to the LinkEntity, additionally ensuring it is active.
        /// Assumes both entities have the same attribute name to link on.
        /// </summary>
        /// <typeparam name="T">The type of the entity that is to be linked</typeparam>
        /// <param name="link">The link.</param>
        /// <param name="linkAttributesName">Name of the link from and link to attribute.</param>
        /// <returns></returns>
        public static LinkEntity AddLink<T>(this LinkEntity link, string linkAttributesName) where T : Entity
        {
            var childLink = link.AddChildLink(GetEntityLogicalName<T>(), linkAttributesName, linkAttributesName);
            childLink.LinkCriteria.ActiveOnly<T>();
            return childLink;
        }

        /// <summary>
        /// Adds the type T as a child linked entity, additionally ensuring it is active.
        /// Assumes both entities have the same attribute name to link on.
        /// </summary>
        /// <typeparam name="T">The type of the entity that is to me linked</typeparam>
        /// <param name="link">The link.</param>
        /// <param name="linkAttributesName">Name of the link from and link to attribute.</param>
        /// <param name="joinType">Type of the join to perform.</param>
        /// <returns></returns>
        public static LinkEntity AddLink<T>(this LinkEntity link, string linkAttributesName, JoinOperator joinType) where T : Entity
        {
            var childLink = link.AddChildLink(GetEntityLogicalName<T>(), linkAttributesName, linkAttributesName, joinType);
            childLink.LinkCriteria.ActiveOnly<T>();
            return childLink;
        }

        /// <summary>
        /// Adds the type T as a child linked entity, additionally ensuring it is active.
        /// </summary>
        /// <typeparam name="T">The type of the entity that is to me linked</typeparam>
        /// <param name="link">The link.</param>
        /// <param name="linkFromAttributeName">Name of the link from attribute.</param>
        /// <param name="linkToAttributeName">Name of the link to attribute.</param>
        /// <returns></returns>
        public static LinkEntity AddLink<T>(this LinkEntity link, string linkFromAttributeName, string linkToAttributeName) where T : Entity
        {
            var childLink = link.AddChildLink(GetEntityLogicalName<T>(), linkFromAttributeName, linkToAttributeName);
            childLink.LinkCriteria.ActiveOnly<T>();
            return childLink;
        }

        /// <summary>
        /// Adds the type T as a child linked entity, additionally ensuring it is active.
        /// </summary>
        /// <typeparam name="T">The type of Entity</typeparam>
        /// <param name="link">The link.</param>
        /// <param name="linkFromAttributeName">Name of the link from attribute.</param>
        /// <param name="linkToAttributeName">Name of the link to attribute.</param>
        /// <param name="joinType">Type of the join.</param>
        /// <returns></returns>
        public static LinkEntity AddLink<T>(this LinkEntity link, string linkFromAttributeName, string linkToAttributeName, JoinOperator joinType) where T : Entity
        {
            var childLink = link.AddChildLink(GetEntityLogicalName<T>(), linkFromAttributeName, linkToAttributeName, joinType);
            childLink.LinkCriteria.ActiveOnly<T>();
            return childLink;
        }

        #endregion // LinkEntity

        #region OptionSetValue

        /// <summary>
        /// Returns the value of the OptionSetValue, or int.MinValue if it is null
        /// </summary>
        /// <param name="osv"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetValueOrDefault(this OptionSetValue osv)
        {
            return GetValueOrDefault(osv, int.MinValue);
        }

        /// <summary>
        /// Returns the value of the OptionSetValue, or int.MinValue if it is null
        /// </summary>
        /// <param name="osv"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetValueOrDefault(this OptionSetValue osv, int defaultValue)
        {
            if (osv == null)
            {
                return defaultValue;
            }
            else
            {
                return osv.Value;
            }
        }

        /// <summary>
        /// Allows for Null safe Equals Comparison for more concise code.  ie.
        /// if(contact.GenderCode.NullSafeEquals(1))
        /// vs.
        /// if(contact.GenderCode != null &amp;&amp; contact.gendercode.Value == 1)
        /// </summary>
        /// <param name="osv"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool NullSafeEquals(this OptionSetValue osv, int value)
        {
            if (osv == null)
            {
                return false;
            }
            else
            {
                return osv.Value == value;
            }
        }

        /// <summary>
        /// Allows for Null safe Equals Comparison for more concise code.  ie.
        /// if(contact.GenderCode.NullSafeEquals(new OptionSet(1)))
        /// vs.
        /// if(contact.GenderCode != null && contact.gendercode.Value == new OptionSet(1))
        /// </summary>
        /// <param name="osv"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool NullSafeEquals(this OptionSetValue osv, OptionSetValue value)
        {
            if (osv == null)
            {
                return osv == value;
            }
            else
            {
                return osv.Equals(value);
            }
        }

        #endregion // OptionSetValue

        #region QueryExpression

        /// <summary>
        /// Depending on the Type of T, adds the correct is active criteria Statment
        /// Note: Does not work for Linked Entities
        /// </summary>
        /// <typeparam name="T">The type of Entity to add an active only criteria for</typeparam>
        /// <param name="qe">The queryexpression.</param>
        /// <returns></returns>
        public static QueryExpression ActiveOnly<T>(this QueryExpression qe) where T : Entity
        {
            qe.Criteria.ActiveOnly<T>();
            return qe;
        }

        /// <summary>
        /// Adds the condition to the Filter Expression, and it the current filter expression's logical operator
        /// is an Or, it will move all of the current conditions into a child filter and
        /// </summary>
        /// <param name="criteria">The filter expression.</param>
        /// <param name="ce">The condition expression.</param>
        private static void AddCondtionEnforceAndFilterOperator(this FilterExpression criteria, ConditionExpression ce)
        {
            if (criteria.FilterOperator == LogicalOperator.Or)
            {
                // Move the current filter criteria down and add the is active as an and filter join
                var fe = new FilterExpression(LogicalOperator.Or);

                // Move Conditions
                foreach (var condition in criteria.Conditions.ToList())
                {
                    fe.AddCondition(condition);
                    criteria.Conditions.Remove(condition);
                }

                // Move Filters
                foreach (var filter in criteria.Filters.ToList())
                {
                    fe.AddFilter(filter);
                    criteria.Filters.Remove(filter);
                }

                criteria.FilterOperator = LogicalOperator.And;
                criteria.AddCondition(ce);
                criteria.Filters.Add(fe);
            }
            else
            {
                criteria.AddCondition(ce);
            }
        }

        /// <summary>
        /// Adds all custom columns to the the ColumnSet of the QueryExpression.
        /// </summary>
        /// <typeparam name="T">The type of Entity</typeparam>
        /// <param name="query">The query expression.</param>
        public static void AllCustomColumns<T>(this QueryExpression query) where T : Entity
        {
            if (query.ColumnSet == null)
            {
                query.ColumnSet = new ColumnSet(false);
            }
            query.ColumnSet.AllCustomColumns<T>();
        }

        /// <summary>
        /// Updates the Query Expression to only return only the first entity that matches the query expression expression criteria.
        /// Shortcut for setting the Query's PageInfo.Count and PageInfo.PageNumber to 1.
        /// </summary>
        /// <param name="query">The query.</param>
        public static QueryExpression First(this QueryExpression query)
        {
            query.PageInfo.Count = 1;
            query.PageInfo.PageNumber = 1;
            return query;
        }

        /// <summary>
        /// Returns a SQL-ish representation of the QueryExpression's Criteria
        /// </summary>
        /// <param name="qe">The query expression.</param>
        /// <returns></returns>
        public static string GetSQLStatement(this QueryExpression qe)
        {
            StringBuilder sb = new StringBuilder();

            string top = string.Empty;
            if (qe.PageInfo != null && qe.PageInfo.Count > 0)
            {
                top = "TOP " + qe.PageInfo.Count + " ";
            }

            var allLinkedEntities = WalkAllLinkedEntities(qe);
            var cols = new List<string>();
            foreach (var link in allLinkedEntities.Where(l => l.Columns.Columns.Any()))
            {
                string linkName = link.EntityAlias ?? link.LinkToEntityName ?? "unspecified";
                linkName += ".";
                cols.AddRange(link.Columns.Columns.Select(c => linkName + c));
            }

            // Select Statement
            sb.Append("SELECT " + top);
            if (qe.ColumnSet != null)
            {
                if (qe.ColumnSet.AllColumns)
                {
                    sb.Append(cols.Count > 0 ? "*, " : "* ");
                }
                else
                {
                    cols.AddRange(qe.ColumnSet.Columns);

                }
            }
            sb.AppendLine(String.Join(", ", cols));


            // From Statement
            sb.AppendLine("FROM " + qe.EntityName);
            foreach (var link in allLinkedEntities)
            {
                sb.AppendFormat("{0} JOIN {1} on {2}.{3} = {4}.{5}{6}",
                        link.JoinOperator.ToString().ToUpper(),
                        link.LinkToEntityName,
                        link.LinkFromEntityName, link.LinkFromAttributeName,
                        link.LinkToEntityName, link.LinkToAttributeName, Environment.NewLine);
                if (link.LinkCriteria != null)
                {
                    var statement = link.LinkCriteria.GetStatement();
                    if (!String.IsNullOrWhiteSpace(statement))
                    {

                        sb.AppendLine("    AND " + statement);
                    }
                }
            }

            return sb.ToString() + "WHERE" + Environment.NewLine + GetStatement(qe.Criteria);
        }

        private static IEnumerable<LinkEntity> WalkAllLinkedEntities(QueryExpression qe)
        {
            foreach (var link in qe.LinkEntities)
            {
                yield return link;
                foreach (var child in WalkAllLinkedEntities(link))
                {
                    yield return child;
                }
            }
        }

        private static IEnumerable<LinkEntity> WalkAllLinkedEntities(LinkEntity link)
        {
            foreach (var child in link.LinkEntities)
            {
                yield return child;
                foreach (var grandchild in WalkAllLinkedEntities(child))
                {
                    yield return grandchild;
                }
            }
        }

        /// <summary>
        /// Adds a condition expression to the criteria of the query expression to force the statecode to be a specfic value.
        /// </summary>
        /// <param name="qe">The query expression.</param>
        /// <param name="entityStateEnum">The entity state enum value.</param>
        /// <returns>The link entity with the condition expression added</returns>
        public static QueryExpression StateIs(this QueryExpression qe, object entityStateEnum)
        {
            qe.Criteria.StateIs(entityStateEnum);
            return qe;
        }

        /// <summary>
        /// Updates the PageInfo.Count and PageInfo.PageNumber of the query expression to return the first X number of items
        /// </summary>
        /// <param name="qe">The query expression.</param>
        /// <param name="count">The number of entities to return.</param>
        /// <returns></returns>
        public static QueryExpression Take(this QueryExpression qe, int count)
        {
            qe.PageInfo.Count = count;
            qe.PageInfo.PageNumber = 1;

            return qe;
        }

        /// <summary>
        /// Adds the link entity to the query expression.  Assumes that the linkFromAttributeName and the linkToAttributeName are the same.
        /// </summary>
        /// <param name="qe">The query expression.</param>
        /// <param name="linkToEntityName">Name of the link to entity.</param>
        /// <param name="linkAttributesName">Name of the link from and link to attribute.</param>
        /// <returns></returns>
        public static LinkEntity AddLink(this QueryExpression qe, string linkToEntityName, string linkAttributesName)
        {
            return qe.AddLink(linkToEntityName, linkAttributesName, linkAttributesName);
        }

        /// <summary>
        /// Adds the link entity to the query expression.  Assumes that the linkFromAttributeName and the linkToAttributeName are the same.
        /// </summary>
        /// <param name="qe">The query expression.</param>
        /// <param name="linkToEntityName">Name of the link to entity.</param>
        /// <param name="linkAttributesName">Name of the link from and link to attribute.</param>
        /// <param name="joinType">Type of the join to perform.</param>
        /// <returns></returns>
        public static LinkEntity AddLink(this QueryExpression qe, string linkToEntityName, string linkAttributesName, JoinOperator joinType)
        {
            return qe.AddLink(linkToEntityName, linkAttributesName, linkAttributesName, joinType);
        }

        /// <summary>
        /// Adds the type T as a linked entity to the query expression, additionally ensuring it is active.
        /// Assumes both entities have the same attribute name to link on.
        /// </summary>
        /// <typeparam name="T">The type of the entity that is to be linked and asserted active.</typeparam>
        /// <returns></returns>
        public static LinkEntity AddLink<T>(this QueryExpression qe, string linkAttributesName) where T : Entity
        {
            var link = qe.AddLink(GetEntityLogicalName<T>(), linkAttributesName, linkAttributesName);
            link.LinkCriteria.ActiveOnly<T>();
            return link;
        }

        /// <summary>
        /// Adds the type T as a linked entity, additionally ensuring it is active.
        /// Assumes both entities have the same attribute name to link on.
        /// </summary>
        /// <typeparam name="T">The type of the entity that is to be linked and asserted active.</typeparam>
        /// <param name="qe">The query expression.</param>
        /// <param name="linkAttributesName">Name of the link attributes.</param>
        /// <param name="joinType">Type of the join to perform.</param>
        /// <returns></returns>
        public static LinkEntity AddLink<T>(this QueryExpression qe, string linkAttributesName, JoinOperator joinType) where T : Entity
        {
            var link = qe.AddLink(GetEntityLogicalName<T>(), linkAttributesName, linkAttributesName, joinType);
            link.LinkCriteria.ActiveOnly<T>();
            return link;
        }

        /// <summary>
        /// Adds the type T as a linked entity to the query expression, additionally ensuring it is active.
        /// </summary>
        /// <typeparam name="T">The type of the entity that is to be linked and asserted active.</typeparam>
        /// <param name="qe">The query expression.</param>
        /// <param name="linkFromAttributeName">Name of the link from attribute.</param>
        /// <param name="linkToAttributeName">Name of the link to attribute.</param>
        /// <returns></returns>
        public static LinkEntity AddLink<T>(this QueryExpression qe, string linkFromAttributeName, string linkToAttributeName) where T : Entity
        {
            var link = qe.AddLink(GetEntityLogicalName<T>(), linkFromAttributeName, linkToAttributeName);
            link.LinkCriteria.ActiveOnly<T>();
            return link;
        }

        /// <summary>
        /// Adds the type T as a linked entity, additionally ensuring it is active.
        /// </summary>
        /// <typeparam name="T">The type of the entity that is to be linked and asserted active.</typeparam>
        /// <param name="qe">The query expression.</param>
        /// <param name="linkFromAttributeName">Name of the link from attribute.</param>
        /// <param name="linkToAttributeName">Name of the link to attribute.</param>
        /// <param name="joinType">Type of the join to perform.</param>
        /// <returns></returns>
        public static LinkEntity AddLink<T>(this QueryExpression qe, string linkFromAttributeName, string linkToAttributeName, JoinOperator joinType) where T : Entity
        {
            var link = qe.AddLink(GetEntityLogicalName<T>(), linkFromAttributeName, linkToAttributeName, joinType);
            link.LinkCriteria.ActiveOnly<T>();
            return link;
        }

        #endregion // QueryExpression

        #region String

        /// <summary>
        /// Deserializes the entity from a string xml value to a specific entity type.
        /// </summary>
        /// <typeparam name="T">The type of entity to deserialize the xml to.</typeparam>
        /// <param name="xml">The xml to deserialize.</param>
        /// <returns></returns>
        public static T DeserializeEntity<T>(string xml) where T : Entity
        {
            return ((Entity)xml.DeserializeDataObject()).ToEntity<T>();
        }

        /// <summary>
        /// Deserializes the entity from a string xml value to an IExtensibleDataObject
        /// </summary>
        /// <param name="xml">The xml to deserialize.</param>
        /// <returns></returns>
        public static IExtensibleDataObject DeserializeDataObject(this string xml)
        {
            var serializer = new NetDataContractSerializer();
            var entity = ((IExtensibleDataObject)(serializer.ReadObject(new MemoryStream(UnicodeEncoding.UTF8.GetBytes(xml)))));
            return entity;
        }

        #endregion // String
    }
}
