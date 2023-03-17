using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System.Linq.Expressions;
using System.Collections;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm
#else
namespace Source.DLaB.Xrm
#endif
	
{
    public static partial class Extensions
    {
        #region FilterExpression

        #region Where

        /// <summary>
        /// Adds the column name and condition operator as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="filterExpression">The FilterExpression</param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        /// <param name="values">list of values to be compared with</param>
        public static FilterExpression Where(this FilterExpression filterExpression, string attributeName, ConditionOperator conditionOperator, params object[] values)
        {
            return filterExpression.WhereEqual(new ConditionExpression(attributeName, conditionOperator, values));
        }

        /// <summary>
        /// Adds the column name and condition operator as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="filterExpression">The FilterExpression</param>
        /// <param name="entityName">Name of the entity of attribute on which condition is to be defined on</param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        /// <param name="values">list of values to be compared with</param>
        public static FilterExpression Where(this FilterExpression filterExpression, string entityName, string attributeName, ConditionOperator conditionOperator, params object[] values)
        {
            return filterExpression.WhereEqual(new ConditionExpression(entityName, attributeName, conditionOperator, values));
        }

#if !XRM_2013 && !XRM_2015 && !XRM_2016
        /// <summary>
        /// Adds the column name and condition operator as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="filterExpression">The FilterExpression</param>
        /// <param name="entityName">Name of the entity of attribute on which condition is to be defined on</param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        /// <param name="compareColumns">Boolean flag to define condition on attributes instead of condition on constant value(s)</param>
        /// <param name="values">list of values or attributes(if compareColumns is true) to be compared with</param>
        public static FilterExpression Where(this FilterExpression filterExpression, string entityName, string attributeName, ConditionOperator conditionOperator, bool compareColumns, object[] values)
        {
            return filterExpression.WhereEqual(new ConditionExpression(entityName, attributeName, conditionOperator, compareColumns, values));
        }

        /// <summary>
        /// Adds the column name and condition operator as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="filterExpression">The FilterExpression</param>
        /// <param name="entityName">Name of the entity of attribute on which condition is to be defined on</param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        /// <param name="compareColumns">Boolean flag to define condition on attributes instead of condition on constant value(s)</param>
        /// <param name="value">value or attributes(if compareColumns is true) to be compared with</param>
        public static FilterExpression Where(this FilterExpression filterExpression, string entityName, string attributeName, ConditionOperator conditionOperator, bool compareColumns, object value)
        {
            return filterExpression.WhereEqual(new ConditionExpression(entityName, attributeName, conditionOperator, compareColumns, value));
        }

        /// <summary>
        /// Adds the column name and condition operator as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="filterExpression">The FilterExpression</param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        /// <param name="compareColumns">Boolean flag to define condition on attributes instead of condition on constant value(s)</param>
        /// <param name="value">value or attributes(if compareColumns is true) to be compared with</param>
        public static FilterExpression Where(this FilterExpression filterExpression, string attributeName, ConditionOperator conditionOperator, bool compareColumns, object value)
        {
            return filterExpression.WhereEqual(new ConditionExpression(attributeName, conditionOperator, compareColumns, value));
        }

        /// <summary>
        /// Adds the column name and condition operator as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="filterExpression">The FilterExpression</param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        /// <param name="compareColumns">Boolean flag to define condition on attributes instead of condition on constant value(s)</param>
        /// <param name="values">list of values or attributes(if compareColumns is true) to be compared with</param>
        public static FilterExpression Where(this FilterExpression filterExpression, string attributeName, ConditionOperator conditionOperator, bool compareColumns, object[] values)
        {
            return filterExpression.WhereEqual(new ConditionExpression(attributeName, conditionOperator, compareColumns, values));
        }
#endif

        /// <summary>
        /// Adds the column name, condition operator and value, as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="filterExpression">The FilterExpression</param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        /// <param name="value">value to be compared with</param>
        public static FilterExpression Where(this FilterExpression filterExpression, string attributeName, ConditionOperator conditionOperator, object value)
        {
            return filterExpression.WhereEqual(new ConditionExpression(attributeName, conditionOperator, value));
        }

        /// <summary>
        /// Adds the column name, condition operator and value, as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="filterExpression">The FilterExpression</param>
        /// <param name="entityName">Name of the entity of attribute on which condition is to be defined on</param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        /// <param name="value">value to be compared with</param>)
        public static FilterExpression Where(this FilterExpression filterExpression, string entityName,
            string attributeName,
            ConditionOperator conditionOperator,
            object value)
        {
            return filterExpression.WhereEqual(new ConditionExpression(entityName, attributeName, conditionOperator, value));
        }

        /// <summary>
        /// Adds the column name and condition operator as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="filterExpression">The FilterExpression</param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        public static FilterExpression Where(this FilterExpression filterExpression, string attributeName, ConditionOperator conditionOperator)
        {
            return filterExpression.WhereEqual(new ConditionExpression(attributeName, conditionOperator));
        }

        /// <summary>
        /// Adds the column name and condition operator as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="filterExpression">The FilterExpression</param>
        /// <param name="entityName">Name of the entity of attribute on which condition is to be defined on</param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        public static FilterExpression Where(this FilterExpression filterExpression, string entityName,
            string attributeName,
            ConditionOperator conditionOperator)
        {
            return filterExpression.WhereEqual(new ConditionExpression(entityName, attributeName, conditionOperator));
        }

        /// <summary>
        /// Adds the column name and condition operator as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="filterExpression">The FilterExpression</param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        /// <param name="values">list of values to be compared with</param>
        /// <remarks>Need to handle collections differently. esp. Guid arrays.</remarks>
        public static FilterExpression Where(this FilterExpression filterExpression, string attributeName,
            ConditionOperator conditionOperator,
            ICollection values)
        {
            return filterExpression.WhereEqual(new ConditionExpression(attributeName, conditionOperator, values));
        }

        #endregion Where

        /// <summary>
        /// Adds the column name and value pairs as conditions.  The columnNameAndValuePairs can contain a paired list of attribute names and their values,
        /// -or- ConditionExpressions
        /// </summary>
        /// <param name="filterExpression"></param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name", "John Doe" </param>
        public static FilterExpression WhereEqual(this FilterExpression filterExpression, params object[] columnNameAndValuePairs)
        {
            if (columnNameAndValuePairs == null) { throw new ArgumentNullException(nameof(columnNameAndValuePairs)); }
            int length = columnNameAndValuePairs.Length;
            if (length == 0)
            {
                return filterExpression;
            }

            // Get Indexes of all ors and ConditionExpressions
            var indexes = Enumerable.Range(0, length).ToList();
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
            var orFilter = GetOrFilter(filterExpression, ors);
            var currentFilter = hasOr ? orFilter.AddFilter(LogicalOperator.And) : filterExpression;

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
                    var ce = (ConditionExpression) columnNameAndValuePairs[i];
                    if (ce.Operator == ConditionOperator.On && ce.Values.Any())
                    {
                        // Handle Special Case with ConditionOperator.On not working correctly
                        var date = ((DateTime) ce.Values[0]).ToUniversalTime();
                        currentFilter.AddConditionsAnded(
                            new ConditionExpression(ce.AttributeName, ConditionOperator.GreaterEqual, date.Date),
                            new ConditionExpression(ce.AttributeName, ConditionOperator.LessThan, date.Date.AddDays(1d)));
                    }
                    else
                    {
                        currentFilter.AddCondition(ce);
                    }
                    i--; // There is no second parameter, so subtract 1 to look at the next set of pairs
                }
                else
                {
                    // Non Condition Expression
                    AddNameValuePairCondition(currentFilter,
                        (string)columnNameAndValuePairs[i], columnNameAndValuePairs[i + 1]);
                }
            }

            return filterExpression;
        }

        // ReSharper disable once UnusedParameter.Local
        private static void AddNameValuePairCondition(FilterExpression filter, string attributeName, object value)
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
        /// Helper function to get the or filter based on the FilterExpression.  If it already is or, just return it.
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
                        throw new ArgumentException("All LogicalOperator values must have at least one column value separating them");
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

        #endregion FilterExpression

        #region IOrganizationService

        #region GetAllEntities

        /// <summary>
        /// Gets all Entities where the columnNameAndValue Pairs match
        /// </summary>
        /// <param name="service"></param>
        /// <param name="logicalName">LogicalName of the Entity.</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name", "John Doe" </param>
        /// <returns></returns>
        public static IEnumerable<Entity> GetAllEntities(this IOrganizationService service, string logicalName,
                params object[] columnNameAndValuePairs)
        {
            return service.GetAllEntities<Entity>(QueryExpressionFactory.Create(logicalName, columnNameAndValuePairs));
        }

        /// <summary>
        /// Gets all Entities where the columnNameAndValue Pairs match
        /// </summary>
        /// <param name="service"></param>
        /// <param name="logicalName">LogicalName of the Entity.</param>
        /// <param name="columnSet">Columns to retrieve</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name", "John Doe" </param>
        /// <returns></returns>
        public static IEnumerable<Entity> GetAllEntities(this IOrganizationService service, string logicalName, ColumnSet columnSet,
                params object[] columnNameAndValuePairs)
        {
            return service.GetAllEntities<Entity>(QueryExpressionFactory.Create(logicalName, columnSet, columnNameAndValuePairs));
        }

        #endregion GetAllEntities

        #region GetAllEntities<T>

        /// <summary>
        /// Gets all Entities where the columnNameAndValue Pairs match
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return</typeparam>
        /// <param name="service"></param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name", "John Doe" </param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllEntities<T>(this IOrganizationService service,
                params object[] columnNameAndValuePairs) where T : Entity
        {
            return service.GetAllEntities(QueryExpressionFactory.Create<T>(columnNameAndValuePairs));
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
            var columnSet = AddColumns(new ColumnSet(), anonymousTypeInitializer);
            return service.GetAllEntities<T>(columnSet, columnNameAndValuePairs);
        }

        /// <summary>
        /// Gets all Entities where the columnNameAndValue Pairs match
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return</typeparam>
        /// <param name="service"></param>
        /// <param name="columnSet">Columns to retrieve</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name", "John Doe" </param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllEntities<T>(this IOrganizationService service, ColumnSet columnSet,
                params object[] columnNameAndValuePairs) where T : Entity
        {
            return service.GetAllEntities(QueryExpressionFactory.Create<T>(columnSet, columnNameAndValuePairs));
        }

        #endregion GetAllEntities<T>

        #region GetEntities

        /// <summary>
        /// Gets first 5000 Active Entities where the columnNameAndValue Pairs match
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="logicalName">LogicalName of the Entity.</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name", "John Doe"</param>
        /// <returns></returns>
        public static List<Entity> GetEntities(this IOrganizationService service, string logicalName,
                params object[] columnNameAndValuePairs)
        {
            return service.GetEntities<Entity>(QueryExpressionFactory.Create(logicalName, columnNameAndValuePairs));
        }

        /// <summary>
        /// Gets first 5000 Active Entities (with the given subset of columns only) 
        /// where the columnNameAndValue Pairs match
        /// </summary>
        /// <param name="service"></param>
        /// <param name="logicalName">LogicalName of the Entity.</param>
        /// <param name="columnSet">Columns to retrieve</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name", "John Doe" </param>
        /// <returns></returns>
        public static List<Entity> GetEntities(this IOrganizationService service, string logicalName, ColumnSet columnSet,
                params object[] columnNameAndValuePairs)
        {
            return service.GetEntities<Entity>(QueryExpressionFactory.Create(logicalName, columnSet, columnNameAndValuePairs));
        }

        #endregion GetEntities

        #region GetEntities<T>

        /// <summary>
        /// Gets first 5000 Active Entities where the columnNameAndValue Pairs match
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return</typeparam>
        /// <param name="service"></param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name", "John Doe" </param>
        /// <returns></returns>
        public static List<T> GetEntities<T>(this IOrganizationService service,
                params object[] columnNameAndValuePairs) where T : Entity
        {
            return service.GetEntities(QueryExpressionFactory.Create<T>(columnNameAndValuePairs));
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
            var columnSet = AddColumns(new ColumnSet(), anonymousTypeInitializer);
            return service.GetEntities<T>(columnSet, columnNameAndValuePairs);
        }

        /// <summary>
        /// Gets first 5000 Active Entities (with the given subset of columns only) 
        /// where the columnNameAndValue Pairs match
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return</typeparam>
        /// <param name="service"></param>
        /// <param name="columnSet">Columns to retrieve</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name", "John Doe" </param>
        /// <returns></returns>
        public static List<T> GetEntities<T>(this IOrganizationService service, ColumnSet columnSet,
                params object[] columnNameAndValuePairs) where T : Entity
        {
            return service.GetEntities(QueryExpressionFactory.Create<T>(columnSet, columnNameAndValuePairs));
        }

        #endregion GetEntities<T>

        #region GetFirst

        /// <summary>
        /// Retrieves the first Active entity where the columnNameAndValue Pairs match
        /// </summary>
        /// <param name="service">open IOrganizationService</param>
        /// <param name="logicalName">LogicalName of the Entity.</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name","John Doe" goes to entity.name = "John Doe"</param>
        /// <returns></returns>
        public static Entity GetFirst(this IOrganizationService service, string logicalName,
                params object[] columnNameAndValuePairs)
        {
            var entity = service.GetFirstOrDefault(logicalName, columnNameAndValuePairs);
            AssertExistsWhere(entity, logicalName, columnNameAndValuePairs);
            return entity;
        }

        /// <summary>
        /// Retrieves the first Active entity (with the given subset of columns only)
        /// where the columnNameAndValue Pairs match
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="logicalName">LogicalName of the Entity.</param>
        /// <param name="columnSet">Columns to retrieve</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name","John Doe" goes to entity.name = "John Doe"</param>
        /// <returns></returns>
        public static Entity GetFirst(this IOrganizationService service, string logicalName, ColumnSet columnSet,
                params object[] columnNameAndValuePairs)
        {
            var entity = service.GetFirstOrDefault(logicalName, columnSet, columnNameAndValuePairs);
            AssertExistsWhere(entity, logicalName, columnNameAndValuePairs);
            return entity;
        }
        private static void AssertExistsWhere(Entity entity, string logicalName, object[] columnNameAndValuePairs)
        {
            if (entity != null) { return; }
            throw new InvalidOperationException("No " + logicalName + " found where " +
                                                QueryExpressionFactory.Create(logicalName, null, true, columnNameAndValuePairs).GetSqlStatement());
        }

        #endregion GetFirst

        #region GetFirst<T>

        /// <summary>
        /// Retrieves the first Active entity where the columnNameAndValue Pairs match
        /// </summary>
        /// <typeparam name="T">An early bound Entity Type</typeparam>
        /// <param name="service">open IOrganizationService</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name","John Doe" goes to entity.name = "John Doe"
        /// </param>
        /// <returns></returns>
        public static T GetFirst<T>(this IOrganizationService service,
                params object[] columnNameAndValuePairs) where T : Entity
        {
            var entity = service.GetFirstOrDefault<T>(columnNameAndValuePairs);
            AssertExistsWhere(entity, columnNameAndValuePairs);
            return entity;
        }

        /// <summary>
        /// Retrieves the first Active entity (with the given subset of columns only) 
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
            var columnSet = AddColumns(new ColumnSet(), anonymousTypeInitializer);
            return service.GetFirst<T>(columnSet, columnNameAndValuePairs);
        }

        /// <summary>
        /// Retrieves the first Active entity (with the given subset of columns only) 
        /// where the columnNameAndValue Pairs match
        /// </summary>
        /// <typeparam name="T">An early bound Entity Type</typeparam>
        /// <param name="service"></param>
        /// <param name="columnSet">Columns to retrieve</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name","John Doe" goes to entity.name = "John Doe"
        /// </param>
        /// <returns></returns>
        public static T GetFirst<T>(this IOrganizationService service, ColumnSet columnSet,
                params object[] columnNameAndValuePairs) where T : Entity
        {
            var entity = service.GetFirstOrDefault<T>(columnSet, columnNameAndValuePairs);
            AssertExistsWhere(entity, columnNameAndValuePairs);
            return entity;
        }

        // ReSharper disable once UnusedParameter.Local
        private static void AssertExistsWhere<T>(T entity, object[] columnNameAndValuePairs) where T : Entity
        {
            if (entity != null) { return; }
            throw new InvalidOperationException("No " + EntityHelper.GetEntityLogicalName<T>() + " found where " +
                                                QueryExpressionFactory.Create<T>((ColumnSet)null, true, columnNameAndValuePairs).GetSqlStatement());
        }

        #endregion GetFirst<T>

        #region GetFirstOrDefault

        /// <summary>
        /// Retrieves the first Active entity where the columnNameAndValue Pairs match
        /// </summary>
        /// <param name="service">open IOrganizationService</param>
        /// <param name="logicalName">Logical Name of the Entity:</param>
        /// <param name="columnNameAndValuePairs"></param>
        /// <returns></returns>
        public static Entity GetFirstOrDefault(this IOrganizationService service, string logicalName, params object[] columnNameAndValuePairs)
        {
            var settings = new LateBoundQuerySettings(logicalName);
            return service.RetrieveMultiple(settings.CreateExpression(columnNameAndValuePairs)).Entities.FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the first Active entity (with the given subset of columns only) 
        /// where the columnNameAndValue Pairs match
        /// </summary>
        /// <param name="service"></param>
        /// <param name="logicalName"></param>
        /// <param name="columnSet">Columns to retrieve</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name","John Doe" goes to entity.name = "John Doe"
        /// </param>
        /// <returns></returns>
        public static Entity GetFirstOrDefault(this IOrganizationService service, string logicalName, ColumnSet columnSet,
                params object[] columnNameAndValuePairs)
        {
            var settings = new LateBoundQuerySettings(logicalName)
            {
                Columns = columnSet,
                First = true
            };
            return service.RetrieveMultiple(settings.CreateExpression(columnNameAndValuePairs)).Entities.FirstOrDefault();
        }

        #endregion GetFirstOrDefault

        #region GetFirstOrDefault<T>

        /// <summary>
        /// Retrieves the first Active entity where the columnNameAndValue Pairs match
        /// </summary>
        /// <typeparam name="T">An early bound Entity Type</typeparam>
        /// <param name="service">open IOrganizationService</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name","John Doe" goes to entity.name = "John Doe"
        /// </param>
        /// <returns></returns>
        public static T GetFirstOrDefault<T>(this IOrganizationService service,
              params object[] columnNameAndValuePairs) where T : Entity
        {
            var settings = new QuerySettings<T> { First = true };
            return service.GetEntities<T>(settings.CreateExpression(columnNameAndValuePairs)).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the first Active entity (with the given subset of columns only) 
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
        public static T GetFirstOrDefault<T>(this IOrganizationService service,
                Expression<Func<T, object>> anonymousTypeInitializer, params object[] columnNameAndValuePairs)
            where T : Entity
        {
            var columnSet = AddColumns(new ColumnSet(), anonymousTypeInitializer);
            return service.GetFirstOrDefault<T>(columnSet, columnNameAndValuePairs);
        }

        /// <summary>
        /// Retrieves the first Active entity (with the given subset of columns only) 
        /// where the columnNameAndValue Pairs match
        /// </summary>
        /// <typeparam name="T">An early bound Entity Type</typeparam>
        /// <param name="service"></param>
        /// <param name="columnSet">Columns to retrieve</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name","John Doe" goes to entity.name = "John Doe"
        /// </param>
        /// <returns></returns>
        public static T GetFirstOrDefault<T>(this IOrganizationService service, ColumnSet columnSet,
                params object[] columnNameAndValuePairs) where T : Entity
        {
            var settings = new QuerySettings<T>
            {
                Columns = columnSet,
                First = true
            };
            return service.GetEntities<T>(settings.CreateExpression(columnNameAndValuePairs)).FirstOrDefault();
        }

        #endregion GetFirstOrDefault<T>

        #region Equal Only Extensions

        // These Extension Methods only makes sense for equality, since they set the values if not found...
        /// <summary>
        /// Retrieves the entity from CRM where the columnNameAndValuePairs match.  If it doesn't exist, it creates it
        /// populating the entity with the columnNameAndValuePairs
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name","John Doe" goes to entity.name = "John Doe"
        /// </param>
        /// <returns></returns>
        public static T GetOrCreateEntity<T>(this IOrganizationService service, params object[] columnNameAndValuePairs)
                where T : Entity, new()
        {
            return service.GetOrCreateEntity<T>(SolutionCheckerAvoider.CreateColumnSetWithAllColumns(), columnNameAndValuePairs);
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
        public static T GetOrCreateEntity<T>(this IOrganizationService service,
                Expression<Func<T, object>> anonymousTypeInitializer, params object[] columnNameAndValuePairs)
            where T : Entity, new()
        {
            var columnSet = AddColumns(new ColumnSet(), anonymousTypeInitializer);
            return service.GetOrCreateEntity<T>(columnSet, columnNameAndValuePairs);
        }

        /// <summary>
        /// Retrieves the entity from CRM where the columnNameAndValuePairs match.  If it doesn't exist, it creates it
        /// populating the entity with the columnNameAndValuePairs
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        /// <param name="columnSet"></param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name","John Doe" goes to entity.name = "John Doe"
        /// </param>
        /// <returns></returns>
        public static T GetOrCreateEntity<T>(this IOrganizationService service, ColumnSet columnSet,
            params object[] columnNameAndValuePairs) where T : Entity, new()
        {
            int length = columnNameAndValuePairs.Length;
            if (length % 2 != 0)
            {
                throw new ArgumentException("Each Column Name must have a value after it.  Invalid " +
                    "columnNameAndValuePairs length");
            }

            var entity = new T();
            for (int i = 0; i < length; i += 2)
            {
                Type valueType = columnNameAndValuePairs[i + 1].GetType();
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

            var result = service.GetFirstOrDefault<T>(columnSet, columnNameAndValuePairs);
            if (result == null)
            {
                entity.Id = service.Create(entity);
                result = entity;
            }
            return result;
        }

        #endregion Equal Only Extensions

        #endregion IOrganizationService

        #region LinkEntity

        /// <summary>
        /// Adds the column name and condition operator as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="linkEntity"></param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        public static LinkEntity Where(this LinkEntity linkEntity, string attributeName, ConditionOperator conditionOperator)
        {
            linkEntity.LinkCriteria.WhereEqual(new ConditionExpression(attributeName, conditionOperator));
            return linkEntity;
        }

        /// <summary>
        /// Adds the column name, condition operator and value, as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="linkEntity"></param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        /// <param name="value">value to be compared with</param>
        public static LinkEntity Where(this LinkEntity linkEntity, string attributeName, ConditionOperator conditionOperator, object value)
        {
            linkEntity.LinkCriteria.WhereEqual(new ConditionExpression(attributeName, conditionOperator, value));
            return linkEntity;
        }

        /// <summary>
        /// Adds the column name and value pairs to the linkCriteria of the given LinkEntity
        /// </summary>
        /// <param name="linkEntity"></param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name", "John Doe" </param>
        public static LinkEntity WhereEqual(this LinkEntity linkEntity, params object[] columnNameAndValuePairs)
        {
            linkEntity.LinkCriteria.WhereEqual(columnNameAndValuePairs);
            return linkEntity;
        }

        #endregion LinkEntity

        #region OrganizationRequestCollection

        /// <summary>
        /// Adds a retrieve multiple request to the current OrganizationRequestCollection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requests">The requests.</param>
        /// <param name="columnNameAndValuePairs">The column name and value pairs.</param>
        public static void AddRetrieveMultiple<T>(this OrganizationRequestCollection requests, params object[] columnNameAndValuePairs) where T : Entity
        {
            requests.Add(new RetrieveMultipleRequest { Query = QueryExpressionFactory.Create<T>(columnNameAndValuePairs), });
        }

        /// <summary>
        /// Adds a retrieve multiple request to the current OrganizationRequestCollection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requests">The requests.</param>
        /// <param name="columnSet">The column set.</param>
        /// <param name="columnNameAndValuePairs">The column name and value pairs.</param>
        public static void AddRetrieveMultiple<T>(this OrganizationRequestCollection requests, ColumnSet columnSet,
                params object[] columnNameAndValuePairs) where T : Entity
        {
            requests.Add(new RetrieveMultipleRequest { Query = QueryExpressionFactory.Create<T>(columnSet, columnNameAndValuePairs), });
        }

        /// <summary>
        /// Adds a retrieve multiple request to the current OrganizationRequestCollection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requests">The requests.</param>
        /// <param name="anonymousTypeInitializer">The anonymous type initializer.</param>
        /// <param name="columnNameAndValuePairs">The column name and value pairs.</param>
        public static void AddRetrieveMultiple<T>(this OrganizationRequestCollection requests, Expression<Func<T, object>> anonymousTypeInitializer,
                params object[] columnNameAndValuePairs) where T : Entity
        {
            requests.Add(new RetrieveMultipleRequest { Query = QueryExpressionFactory.Create(anonymousTypeInitializer, columnNameAndValuePairs), });
        }

        #endregion OrganizationRequestCollection

        #region QueryExpression

        #region Where

        /// <summary>
        /// Adds the column name and condition operator as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="query">The Query</param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        /// <param name="values">list of values to be compared with</param>
        public static QueryExpression Where(this QueryExpression query, string attributeName, ConditionOperator conditionOperator, params object[] values)
        {
            query.Criteria.WhereEqual(new ConditionExpression(attributeName, conditionOperator, values));
            return query;
        }

        /// <summary>
        /// Adds the column name and condition operator as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="query">The Query</param>
        /// <param name="entityName">Name of the entity of attribute on which condition is to be defined on</param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        /// <param name="values">list of values to be compared with</param>
        public static QueryExpression Where(this QueryExpression query, string entityName, string attributeName, ConditionOperator conditionOperator, params object[] values)
        {
            query.Criteria.WhereEqual(new ConditionExpression(entityName, attributeName, conditionOperator, values));
            return query;
        }

#if !XRM_2013 && !XRM_2015 && !XRM_2016
        /// <summary>
        /// Adds the column name and condition operator as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="query">The Query</param>
        /// <param name="entityName">Name of the entity of attribute on which condition is to be defined on</param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        /// <param name="compareColumns">Boolean flag to define condition on attributes instead of condition on constant value(s)</param>
        /// <param name="values">list of values or attributes(if compareColumns is true) to be compared with</param>
        public static QueryExpression Where(this QueryExpression query, string entityName, string attributeName, ConditionOperator conditionOperator, bool compareColumns, object[] values)
        {
            query.Criteria.WhereEqual(new ConditionExpression(entityName, attributeName, conditionOperator, compareColumns, values));
            return query;
        }

        /// <summary>
        /// Adds the column name and condition operator as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="query">The Query</param>
        /// <param name="entityName">Name of the entity of attribute on which condition is to be defined on</param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        /// <param name="compareColumns">Boolean flag to define condition on attributes instead of condition on constant value(s)</param>
        /// <param name="value">value or attributes(if compareColumns is true) to be compared with</param>
        public static QueryExpression Where(this QueryExpression query, string entityName, string attributeName, ConditionOperator conditionOperator, bool compareColumns, object value)
        {
            query.Criteria.WhereEqual(new ConditionExpression(entityName, attributeName, conditionOperator, compareColumns, value));
            return query;
        }

        /// <summary>
        /// Adds the column name and condition operator as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="query">The Query</param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        /// <param name="compareColumns">Boolean flag to define condition on attributes instead of condition on constant value(s)</param>
        /// <param name="value">value or attributes(if compareColumns is true) to be compared with</param>
        public static QueryExpression Where(this QueryExpression query, string attributeName, ConditionOperator conditionOperator, bool compareColumns, object value)
        {
            query.Criteria.WhereEqual(new ConditionExpression(attributeName, conditionOperator, compareColumns, value));
            return query;
        }

        /// <summary>
        /// Adds the column name and condition operator as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="query">The Query</param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        /// <param name="compareColumns">Boolean flag to define condition on attributes instead of condition on constant value(s)</param>
        /// <param name="values">list of values or attributes(if compareColumns is true) to be compared with</param>
        public static QueryExpression Where(this QueryExpression query, string attributeName, ConditionOperator conditionOperator, bool compareColumns, object[] values)
        {
            query.Criteria.WhereEqual(new ConditionExpression(attributeName, conditionOperator, compareColumns, values));
            return query;
        }
#endif

        /// <summary>
        /// Adds the column name, condition operator and value, as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="query">The Query</param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        /// <param name="value">value to be compared with</param>
        public static QueryExpression Where(this QueryExpression query, string attributeName, ConditionOperator conditionOperator, object value)
        {
            query.Criteria.WhereEqual(new ConditionExpression(attributeName, conditionOperator, value));
            return query;
        }

        /// <summary>
        /// Adds the column name, condition operator and value, as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="query">The Query</param>
        /// <param name="entityName">Name of the entity of attribute on which condition is to be defined on</param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        /// <param name="value">value to be compared with</param>)
        public static QueryExpression Where(this QueryExpression query, string entityName,
            string attributeName,
            ConditionOperator conditionOperator,
            object value)
        {
            query.Criteria.WhereEqual(new ConditionExpression(entityName, attributeName, conditionOperator, value));
            return query;
        }

        /// <summary>
        /// Adds the column name and condition operator as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="query">The Query</param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        public static QueryExpression Where(this QueryExpression query, string attributeName, ConditionOperator conditionOperator)
        {
            query.Criteria.WhereEqual(new ConditionExpression(attributeName, conditionOperator));
            return query;
        }

        /// <summary>
        /// Adds the column name and condition operator as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="query">The Query</param>
        /// <param name="entityName">Name of the entity of attribute on which condition is to be defined on</param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        public static QueryExpression Where(this QueryExpression query, string entityName,
            string attributeName,
            ConditionOperator conditionOperator)
        {
            query.Criteria.WhereEqual(new ConditionExpression(entityName, attributeName, conditionOperator));
            return query;
        }

        /// <summary>
        /// Adds the column name and condition operator as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="query">The Query</param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        /// <param name="values">list of values to be compared with</param>
        /// <remarks>Need to handle collections differently. esp. Guid arrays.</remarks>
        public static QueryExpression Where(this QueryExpression query, string attributeName,
            ConditionOperator conditionOperator,
            ICollection values)
        {
            query.Criteria.WhereEqual(new ConditionExpression(attributeName, conditionOperator, values));
            return query;
        }

        #endregion Where

        /// <summary>
        /// Adds the column name and value pairs to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="query"></param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name", "John Doe" </param>
        public static QueryExpression WhereEqual(this QueryExpression query, params object[] columnNameAndValuePairs)
        {
            // Removing the active only condition is rather dangerous commented out for now, may change in the future
            // query.Criteria.WhereEqual(query.EntityName, columnNameAndValuePairs);
            query.Criteria.WhereEqual(columnNameAndValuePairs);
            return query;
        }
    }
}
        #endregion QueryExpression
