using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using DLaB.Xrm.Messages;
using System.Linq.Expressions;

namespace DLaB.Xrm
{
    public static partial class Extensions
    {
        #region FilterExpression

        /// <summary>
        /// Adds the column name and value pairs as conditions
        /// </summary>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name", "John Doe" </param>
        ///// <param name="logicalName">The logical name of the QueryExpression.EntityName or FilterExpression.EntityName.
        ///// Used to determine if the Id of the entity is being equal compared to.  If so, remove any inactive condition
        ///// checks</param>
        private static FilterExpression WhereEqual(this FilterExpression filterExpression, string entityName,
            params object[] columnNameAndValuePairs)
        {
            int length = columnNameAndValuePairs.Length;
            if (length == 0)
            {
                return filterExpression;
            }

            // Get Indexes of all or's and ConditionExpressions
            var indexes = Enumerable.Range(0, length);
            var ors = indexes.Where(
                    i => columnNameAndValuePairs.ItemAtIndexIsType(i, typeof(LogicalOperator))).
                    OrderBy(i => i).ToArray();
            var conditions = indexes.Where(
                    i => columnNameAndValuePairs.ItemAtIndexIsType(i, typeof(ConditionExpression))).
                    OrderBy(i => i).ToArray();

            bool hasOr = ors.Length > 0;
            if ((length - ors.Length - conditions.Length) % 2 != 0)
            {
                throw new ArgumentException("Each Column Name must have a value after it.  Invalid " +
                    "columnNameAndValuePairs length");
            }

            int orIndex = 0;
            int conditionIndex = 0;
            FilterExpression orFilter = GetOrFilter(filterExpression, ors);
            FilterExpression currentFilter = hasOr ? orFilter.AddFilter(LogicalOperator.And) : filterExpression;

            for (int i = 0; i < length; i += 2)
            {
                if (hasOr && ors.Length > orIndex && ors[orIndex] == i)
                {
                    i++; // Skip LogicalOperator
                    orIndex++;
                    currentFilter = orFilter.AddFilter(LogicalOperator.And); // Create new And filter, adding to Or
                }

                if (conditions.Length > conditionIndex && conditions[conditionIndex] == i)
                {
                    conditionIndex++;
                    currentFilter.AddCondition(columnNameAndValuePairs[i] as ConditionExpression);
                    i--; // There is no second parameter, so subtract 1 to look at the next set of pairs
                }
                else
                {
                    // Non Condition Expression
                    AddNameValuePairCondition(currentFilter,
                        (string)columnNameAndValuePairs[i], columnNameAndValuePairs[i + 1], entityName);
                }
            }

            return filterExpression;
        }

        private static void AddNameValuePairCondition(FilterExpression filter, string attributeName, object value, string entityName)
        {
            if (value == null)
            {
                filter.AddCondition(attributeName, ConditionOperator.Null);
            }
            else if (value is EntityReference)
            {
                throw new ArgumentException("Value type EntityReference is not allowed as a parameter.  Consider " +
                "appending .Id to the value for parameter '" + attributeName + "'");
            }
            else if (value is OptionSetValue)
            {
                throw new ArgumentException("Value type OptionSetValue is not allowed as a parameter.  Consider " +
                "appending .Value to the value for parameter '" + attributeName + "'");
            }
            else
            {
                filter.AddCondition(attributeName, ConditionOperator.Equal, value);

                // Code to remove the active only clause of the entity if the id of the entity is being searched for.
                //if (filter.FilterOperator == LogicalOperator.And && entityName != null && 
                //    (Utilities.EntityHelper.GetIrregularIdAttributeName(entityName) ?? entityName + "id") == attributeName)
                //{
                //    // Remove any Inactive conditions.  If the attribute id of the link or query is being passed in,
                //    // probably don't care if it's active or not.
                //    var isActiveCondition = filter.Conditions.FirstOrDefault(c => c.AttributeName.In("isdisabled", "statecode"));
                //    if (isActiveCondition != null)
                //    {
                //        filter.Conditions.Remove(isActiveCondition);
                //    }
                //}
            }
        }

        private static bool ItemAtIndexIsType(this Object[] objCol, int i, Type type)
        {
            return objCol[i] != null && objCol[i].GetType() == type;
        }

        /// <summary>
        /// Helper function to get the or filter based on the filterexpression.  If it already is or, just return it.
        /// Else, return a new child or filter
        /// </summary>
        /// <param name="filterExpression"></param>
        /// <param name="ors"></param>
        /// <returns></returns>
        private static FilterExpression GetOrFilter(FilterExpression filterExpression, int[] ors)
        {
            FilterExpression orFilter = null;
            if (ors.Length > 0)
            {
                if (ors[0] == 0)
                {
                    throw new ArgumentException("WhereEqual statement can not start with LogicalOperator value");
                }

                for (int i = 0; i < ors.Length - 1; i++)
                {
                    // Check to ensure the index of the current plus 1 doesn't equal the index of the next 
                    if (ors[i] + 1 == ors[i + 1])
                    {
                        throw new ArgumentException("All LogicalOperator values must have at least one column value seperating them");
                    }
                }

                if (filterExpression.FilterOperator == LogicalOperator.Or)
                {
                    orFilter = filterExpression;
                }
                else
                {
                    orFilter = new FilterExpression(LogicalOperator.Or);
                    filterExpression.AddFilter(orFilter);
                }
            }
            return orFilter;
        }

        #endregion // FilterExpression

        #region IOrganizationService

        /// <summary>
        /// Performs an Asynchronous delete, deleting entities that match the given set of name value pairs
        /// Use the querySet overload if performing multiple deletes
        /// </summary>
        /// <typeparam name="T">The type of Entity</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="columnNameAndValuePairs">The column name and value pairs.</param>
        /// <returns></returns>
        public static BulkDeleteResult Delete<T>(this IOrganizationService service, params object[] columnNameAndValuePairs)
            where T : Entity
        {
            return BulkDelete.Delete(service, new QueryExpression[] { QueryExpressionFactory.Create<T>(columnNameAndValuePairs) });
        }


        /// <summary>
        /// Gets all Entities where the columnNameAndValue Pairs match
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name", "John Doe"</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllEntities<T>(this IOrganizationService service, params object[] columnNameAndValuePairs) where T : Entity
        {
            return service.RetrieveAllList<T>(QueryExpressionFactory.Create<T>(columnNameAndValuePairs));
        }

        /// <summary>
        /// Gets all Entities where the columnNameAndValue Pairs match
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return</typeparam>
        /// <param name="service"></param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name", "John Doe" </param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllEntities<T>(this IOrganizationService service,
                Expression<Func<T, object>> anonymousTypeInitializer, params object[] columnNameAndValuePairs)
            where T : Entity
        {
            var columnSet = AddColumns<T>(new ColumnSet(), anonymousTypeInitializer);
            return service.GetAllEntities<T>(columnSet, columnNameAndValuePairs);
        }

        /// <summary>
        /// Gets all Entities where the columnNameAndValue Pairs match
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="columnSet">Columns to retrieve</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name", "John Doe"</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllEntities<T>(this IOrganizationService service, ColumnSet columnSet,
                params object[] columnNameAndValuePairs) where T : Entity
        {
            return service.RetrieveAllList<T>(QueryExpressionFactory.Create<T>(columnSet, columnNameAndValuePairs));
        }

        /// <summary>
        /// Gets first 5000 active entities where the columnNameAndValue Pairs match
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name", "John Doe"</param>
        /// <returns></returns>
        public static List<T> GetEntities<T>(this IOrganizationService service, params object[] columnNameAndValuePairs) where T : Entity
        {
            return service.RetrieveList<T>(QueryExpressionFactory.Create<T>(columnNameAndValuePairs));
        }

        /// <summary>
        /// Gets first 5000 Active Entities (with the given subset of columns only) 
        /// where the columnNameAndValue Pairs match
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return</typeparam>
        /// <param name="service"></param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name", "John Doe" </param>
        /// <returns></returns>
        public static List<T> GetEntities<T>(this IOrganizationService service, Expression<Func<T, object>> anonymousTypeInitializer,
                params object[] columnNameAndValuePairs) where T : Entity
        {
            var columnSet = AddColumns<T>(new ColumnSet(), anonymousTypeInitializer);
            return service.GetEntities<T>(columnSet, columnNameAndValuePairs);
        }

        /// <summary>
        /// Gets first 5000 active entities (with the given subset of columns only)
        /// where the columnNameAndValue Pairs match
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="columnSet">Columns to retrieve</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name", "John Doe"</param>
        /// <returns></returns>
        public static List<T> GetEntities<T>(this IOrganizationService service, ColumnSet columnSet,
                params object[] columnNameAndValuePairs) where T : Entity
        {
            return service.RetrieveList<T>(QueryExpressionFactory.Create<T>(columnSet, columnNameAndValuePairs));
        }

        /// <summary>
        /// Retreives the first active entity where the columnNameAndValue Pairs match
        /// </summary>
        /// <typeparam name="T">An early bound Entity Type</typeparam>
        /// <param name="service">open IOrganizationService</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name","John Doe" goes to entity.name = "John Doe"
        /// </param>
        /// <returns></returns>
        public static T GetFirst<T>(this IOrganizationService service, params object[] columnNameAndValuePairs) where T : Entity
        {
            var entity = service.GetFirstOrDefault<T>(columnNameAndValuePairs);
            AssertExistsWhere<T>(entity, columnNameAndValuePairs);
            return entity;
        }

        /// <summary>
        /// Retreives the first Active entity (with the given subset of columns only) 
        /// where the columnNameAndValue Pairs match
        /// </summary>
        /// <typeparam name="T">An early bound Entity Type</typeparam>
        /// <param name="service"></param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name","John Doe" goes to entity.name = "John Doe"
        /// </param>
        /// <returns></returns>
        public static T GetFirst<T>(this IOrganizationService service, Expression<Func<T, object>> anonymousTypeInitializer,
                params object[] columnNameAndValuePairs) where T : Entity
        {
            var columnSet = AddColumns<T>(new ColumnSet(), anonymousTypeInitializer);
            return service.GetFirst<T>(columnSet, columnNameAndValuePairs);
        }

        /// <summary>
        /// Retreives the first active entity (with the given subset of columns only)
        /// where the columnNameAndValue Pairs match
        /// </summary>
        /// <typeparam name="T">An early bound Entity Type</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="columnSet">Columns to retrieve</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name","John Doe" goes to entity.name = "John Doe"</param>
        /// <returns></returns>
        public static T GetFirst<T>(this IOrganizationService service, ColumnSet columnSet, params object[] columnNameAndValuePairs) where T : Entity
        {
            var entity = service.GetFirstOrDefault<T>(columnSet, columnNameAndValuePairs);
            AssertExistsWhere<T>(entity, columnNameAndValuePairs);
            return entity;
        }

        private static void AssertExistsWhere<T>(T entity, object[] columnNameAndValuePairs) where T : Entity
        {
            if (entity == null)
            {
                throw new InvalidOperationException("No " + GetEntityLogicalName<T>() + " found where " +
                    QueryExpressionFactory.Create<T>((ColumnSet)null, true, columnNameAndValuePairs).GetSQLStatement());
            }
        }

        /// <summary>
        /// Retreives the first active entity where the columnNameAndValue Pairs match
        /// </summary>
        /// <typeparam name="T">An early bound Entity Type</typeparam>
        /// <param name="service">open IOrganizationService</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name","John Doe" goes to entity.name = "John Doe"
        /// </param>
        /// <returns></returns>
        public static T GetFirstOrDefault<T>(this IOrganizationService service, params object[] columnNameAndValuePairs) where T : Entity
        {
            var settings = new QuerySettings<T>() { First = true };
            return service.RetrieveList<T>(settings.CreateExpression(columnNameAndValuePairs)).FirstOrDefault();
        }

        /// <summary>
        /// Retreives the first Active entity (with the given subset of columns only) 
        /// where the columnNameAndValue Pairs match
        /// </summary>
        /// <typeparam name="T">An early bound Entity Type</typeparam>
        /// <param name="service"></param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name","John Doe" goes to entity.name = "John Doe"
        /// </param>
        /// <returns></returns>
        public static T GetFirstOrDefault<T>(this IOrganizationService service, Expression<Func<T, object>> anonymousTypeInitializer, params object[] columnNameAndValuePairs)
            where T : Entity
        {
            var columnSet = AddColumns<T>(new ColumnSet(), anonymousTypeInitializer);
            return service.GetFirstOrDefault<T>(columnSet, columnNameAndValuePairs);
        }

        /// <summary>
        /// Retreives the first active entity (with the given subset of columns only)
        /// where the columnNameAndValue Pairs match
        /// </summary>
        /// <typeparam name="T">An early bound Entity Type</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="columnSet">Columns to retrieve</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name","John Doe" goes to entity.name = "John Doe"</param>
        /// <returns></returns>
        public static T GetFirstOrDefault<T>(this IOrganizationService service, ColumnSet columnSet, params object[] columnNameAndValuePairs) 
            where T : Entity
        {
            var settings = new QuerySettings<T>
            {
                Columns = columnSet,
                First = true
            };
            return service.RetrieveList<T>(settings.CreateExpression(columnNameAndValuePairs)).FirstOrDefault();
        }

        #region Equal Only Extensions

        // These Extension Methods only makes sense for equality, since they set the values if not found...

        /// <summary>
        /// Retrieves the entity from CRM where the columnNameAndValuePairs match.  If it doesn't exist, it creates it
        /// populating the entity with the columnNameAndValuePairs
        /// </summary>
        /// <typeparam name="T">The type of Entity</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name","John Doe" goes to entity.name = "John Doe"</param>
        /// <returns></returns>
        public static T GetOrCreateEntity<T>(this IOrganizationService service, params object[] columnNameAndValuePairs)
            where T : Entity, new()
        {
            return service.GetOrCreateEntity<T>(new ColumnSet(true), columnNameAndValuePairs);
        }

        /// <summary>
        /// Retrieves the entity from CRM where the columnNameAndValuePairs match.  If it doesn't exist, it creates it
        /// populating the entity with the columnNameAndValuePairs
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name","John Doe" goes to entity.name = "John Doe"
        /// </param>
        /// <returns></returns>
        public static T GetOrCreateEntity<T>(this IOrganizationService service, Expression<Func<T, object>> anonymousTypeInitializer, params object[] columnNameAndValuePairs)
            where T : Entity, new()
        {
            var columnSet = AddColumns<T>(new ColumnSet(), anonymousTypeInitializer);
            return service.GetOrCreateEntity<T>(columnSet, columnNameAndValuePairs);
        }

        /// <summary>
        /// Retrieves the entity from CRM where the columnNameAndValuePairs match.  If it doesn't exist, it creates it
        /// populating the entity with the columnNameAndValuePairs
        /// </summary>
        /// <typeparam name="T">The type of Entity</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="columnSet">Columns to retrieve</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name","John Doe" goes to entity.name = "John Doe"</param>
        /// <returns></returns>
        public static T GetOrCreateEntity<T>(this IOrganizationService service, ColumnSet columnSet, params object[] columnNameAndValuePairs) 
            where T : Entity, new()
        {
            int length = columnNameAndValuePairs.Length;
            if (length % 2 != 0)
            {
                throw new ArgumentException("Each Column Name must have a value after it.  Invalid " +
                    "columnNameAndValuePairs length");
            }

            T entity = new T();
            T result;
            Type valueType;
            for (int i = 0; i < length; i += 2)
            {
                valueType = columnNameAndValuePairs[i + 1].GetType();
                entity.Attributes[columnNameAndValuePairs[i] as string] = columnNameAndValuePairs[i + 1];

                if (valueType == typeof(OptionSetValue))
                {
                    // The Get Methods need OptionSetValues to be ints, but the create method needs option set 
                    columnNameAndValuePairs[i + 1] = ((OptionSetValue)columnNameAndValuePairs[i + 1]).Value;
                }

                if (valueType == typeof(EntityReference))
                {
                    // The Get Methods need EntityReference to be Guids, but the create method needs EntityReference 
                    columnNameAndValuePairs[i + 1] = ((EntityReference)columnNameAndValuePairs[i + 1]).Id;
                }
            }

            result = service.GetFirstOrDefault<T>(columnSet, columnNameAndValuePairs);
            if (result == null)
            {
                entity.Id = service.Create(entity);
                result = entity;
            }
            return result;
        }

        #endregion // Equal Only Extensions

        #endregion // IOrganizationService

        #region LinkEntity

        /// <summary>
        /// Adds the column name and value pairs to the linkCriteria of the given LinkEntity
        /// </summary>
        /// <param name="linkEntity">The link entity.</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name", "John Doe"</param>
        /// <returns></returns>
        public static LinkEntity WhereEqual(this LinkEntity linkEntity, params object[] columnNameAndValuePairs)
        {
            linkEntity.LinkCriteria.WhereEqual(linkEntity.LinkToEntityName, columnNameAndValuePairs);
            return linkEntity;
        }

        #endregion // LinkEntity

        #region QueryExpression

        /// <summary>
        /// Adds the column name and value pairs to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name", "John Doe"</param>
        /// <returns></returns>
        public static QueryExpression WhereEqual(this QueryExpression query, params object[] columnNameAndValuePairs)
        {
            // Removing the active only condition is rather dangerous commented out for now, may change in the future
            // query.Criteria.WhereEqual(query.EntityName, columnNameAndValuePairs);
            query.Criteria.WhereEqual(null, columnNameAndValuePairs);
            return query;
        }

        #endregion // QueryExpression
    }
}
