using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DLaB.Xrm.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace DLaB.Xrm
{
    public static partial class Extensions
    {
        #region FilterExpression

        /// <summary>
        /// Adds an In Condition to the FilterExpression.
        /// </summary>
        /// <param name="filterExpression">The filter expression.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">An IEnumerable of values to search for being in the column name.</param>
        /// <returns></returns>
        public static FilterExpression WhereIn(this FilterExpression filterExpression, string columnName, IEnumerable values)
        {

            // Since value type arrays are not covariant in C#, they will not be converted to an object[] if passed
            // into this method, but instead a single object of type array.  Manually convert this to an object array
            var list = new List<object>();

            foreach (var i in values)
            {
                list.Add(i);
            }
            AddWhereInCondition(filterExpression, columnName, list.ToArray());
            return filterExpression;
        }

        /// <summary>
        /// Adds an In Condition to the FilterExpression.
        /// </summary>
        /// <param name="filterExpression">The filter expression.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static FilterExpression WhereIn(this FilterExpression filterExpression, string columnName, params object[] values)
        {
            if (values.Length == 1)
            {
                var type = values[0].GetType();
                var eNum = values[0] as IEnumerable;
                if (type.Name != "String" && eNum != null)
                {
                    return filterExpression.WhereIn(columnName, eNum);
                }
            }

            AddWhereInCondition(filterExpression, columnName, values);

            return filterExpression;
        }

        /// <summary>
        /// Adds an In Condition to the FilterExpression.  Since a string is also IEnumerable, this method is required
        /// to wrap the string in to an object array for the correct method overloading resolution.
        /// </summary>
        /// <param name="filterExpression">The filter expression.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static FilterExpression WhereIn(this FilterExpression filterExpression, string columnName, string value)
        {
            AddWhereInCondition(filterExpression, columnName, new object[] { value });
            return filterExpression;
        }

        private static void AddWhereInCondition(FilterExpression filterExpression, string columnName, object[] values)
        {
            if (values.Length == 0)
            {
                // If no values are in our values array, create a constraint which causing nothing to get returned
                var ANDing = new FilterExpression(LogicalOperator.And);
                ANDing.AddCondition(columnName, ConditionOperator.Null);
                ANDing.AddCondition(columnName, ConditionOperator.NotNull);
                filterExpression.AddFilter(ANDing);
            }
            else if (values.Any(v => v == null))
            {
                if (values.Any(v => v != null))
                {
                    var ORing = new FilterExpression(LogicalOperator.Or);
                    ORing.AddCondition(columnName, ConditionOperator.Null);
                    ORing.AddCondition(columnName, ConditionOperator.In, values.Where(v => values != null));
                    filterExpression.AddFilter(ORing);
                }
                else
                {
                    filterExpression.AddCondition(columnName, ConditionOperator.Null);
                }
            }
            else
            {
                filterExpression.AddCondition(columnName, ConditionOperator.In, values);
            }
        }

        #endregion // FilterExpression

        #region IOrganizationService

        #region DeleteIn

        /// <summary>
        /// Attempts to delete the entity with the given id. If it doesn't exist, false is returned.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="settings">The crm service settings.</param>
        /// <param name="entities">The entities to delete if any exist.</param>
        public static void DeleteAnyIfExists(this IOrganizationService service, CrmService settings, IEnumerable<Entity> entities)
        {
            System.Threading.Tasks.Parallel.Invoke(
                entities.Select(e => (Action)(() => DeleteIfExists(settings, e.LogicalName, e.Id))).ToArray());
        }

        /// <summary>
        /// Attempts to delete the entity with the given id. If it doesn't exist, false is returned.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="settings">The crm service settings.</param>
        /// <param name="entityName">Logical name of the entity.</param>
        /// <param name="id">The list of ids to search for.</param>
        public static void DeleteAnyIfExists(this IOrganizationService service, CrmService settings, string entityName, IEnumerable<Guid> id)
        {
            System.Threading.Tasks.Parallel.Invoke(id.Select(i => (Action)(() => DeleteIfExists(settings, entityName, i))).ToArray());
        }

        /// <summary>
        /// Performs an Asynchronous delete, deleting entities that are in the given set of values
        /// Use the querySet overload if performing multiple deletes.
        /// </summary>
        /// <typeparam name="T">Type of Entity List to delete.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static BulkDeleteResult DeleteIn<T>(this IOrganizationService service, string columnName, IEnumerable values)
            where T : Entity
        {
            return BulkDelete.Delete(service, new QueryExpression[] { QueryExpressionFactory.CreateIn<T>((ColumnSet)null, false, columnName, values)});
        }

        /// <summary>
        /// Performs an Asynchronous delete, deleting entities that are in the given set of values
        /// Use the querySet overload if performing multiple deletes.
        /// </summary>
        /// <typeparam name="T">Type of Entity List to delete.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static BulkDeleteResult DeleteIn<T>(this IOrganizationService service, string columnName, params object[] values)
            where T : Entity
        {
            return BulkDelete.Delete(service, new QueryExpression[] { QueryExpressionFactory.CreateIn<T>((ColumnSet)null, false, columnName, values)});
        }

        #endregion // DeleteIn

        #region GetAllEntitiesIn

        /// <summary>
        /// Gets all active entities where the values are in the columnName.
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllEntitiesIn<T>(this IOrganizationService service, string columnName, IEnumerable values)
            where T : Entity
        {
            return service.RetrieveAllList<T>(QueryExpressionFactory.CreateIn<T>(columnName, values));
        }

        /// <summary>
        /// Gets all active entities where the values are in the columnName.
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllEntitiesIn<T>(this IOrganizationService service,string columnName, params object[] values)
            where T : Entity
        {
            return service.RetrieveAllList<T>(QueryExpressionFactory.CreateIn<T>(columnName, values));
        }

        /// <summary>
        /// Gets all Active Entities (with the given subset of columns only)
        /// where the values are in the columnName
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <param name="columnName">The name of the column to perform the in against</param>
        /// <param name="values">The list of values to search for being in the column name</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllEntitiesIn<T>(this IOrganizationService service, Expression<Func<T, object>> anonymousTypeInitializer, string columnName, IEnumerable values)
            where T : Entity
        {
            var columnSet = AddColumns<T>(new ColumnSet(), anonymousTypeInitializer);
            return service.GetAllEntitiesIn<T>(columnSet, columnName, values);
        }

        /// <summary>
        /// Gets all active entities (with the given subset of columns only)
        /// where the values are in the columnName.
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="columnSet">Columns to Return</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllEntitiesIn<T>(this IOrganizationService service, ColumnSet columnSet, string columnName, IEnumerable values)
            where T : Entity
        {
            return service.RetrieveAllList<T>(QueryExpressionFactory.CreateIn<T>(columnSet, columnName, values));
        }

        /// <summary>
        /// Gets all Active Entities (with the given subset of columns only)
        /// where the values are in the columnName
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <param name="columnName">The name of the column to perform the in against</param>
        /// <param name="values">The list of values to search for being in the column name</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllEntitiesIn<T>(this IOrganizationService service, Expression<Func<T, object>> anonymousTypeInitializer, string columnName, params object[] values)
            where T : Entity
        {
            var columnSet = AddColumns<T>(new ColumnSet(), anonymousTypeInitializer);
            return service.GetAllEntitiesIn<T>(columnSet, columnName, values);
        }

        /// <summary>
        /// Gets all active entities (with the given subset of columns only)
        /// where the values are in the columnName.
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="columnSet">Columns to Return</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllEntitiesIn<T>(this IOrganizationService service, ColumnSet columnSet, string columnName, params object[] values)
            where T : Entity
        {
            return service.RetrieveAllList<T>(QueryExpressionFactory.CreateIn<T>(columnSet, columnName, values));
        }

        #endregion // GetAllEntitiesIn

        #region GetEntitisIn

        /// <summary>
        /// Gets first 5000 active entities where the values are in the columnName.
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static List<T> GetEntitiesIn<T>(this IOrganizationService service, string columnName, IEnumerable values)
            where T : Entity
        {
            return service.RetrieveList<T>(QueryExpressionFactory.CreateIn<T>(columnName, values));
        }

        /// <summary>
        /// Gets first 5000 active entities where the values are in the columnName.
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static List<T> GetEntitiesIn<T>(this IOrganizationService service, string columnName, params object[] values)
            where T : Entity
        {
            return service.RetrieveList<T>(QueryExpressionFactory.CreateIn<T>(columnName, values));
        }

        /// <summary>
        /// Gets first 5000 Active Entities (with the given subset of columns only)
        /// where the values are in the columnName
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <param name="columnName">The name of the column to perform the in against</param>
        /// <param name="values">The list of values to search for being in the column name</param>
        /// <returns></returns>
        public static List<T> GetEntitiesIn<T>(this IOrganizationService service, Expression<Func<T, object>> anonymousTypeInitializer, string columnName, IEnumerable values)
            where T : Entity
        {
            var columnSet = AddColumns<T>(new ColumnSet(), anonymousTypeInitializer);
            return service.GetEntitiesIn<T>(columnSet, columnName, values);
        }

        /// <summary>
        /// Gets first 5000 active entities (with the given subset of columns only)
        /// where the values are in the columnName
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="columnSet">Columns to Return</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static List<T> GetEntitiesIn<T>(this IOrganizationService service, ColumnSet columnSet, string columnName, IEnumerable values)
            where T : Entity
        {
            return service.RetrieveList<T>(QueryExpressionFactory.CreateIn<T>(columnSet, columnName, values));
        }

        /// <summary>
        /// Gets first 5000 Active Entities (with the given subset of columns only)
        /// where the values are in the columnName
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <param name="columnName">The name of the column to perform the in against</param>
        /// <param name="values">The list of values to search for being in the column name</param>
        /// <returns></returns>
        public static List<T> GetEntitiesIn<T>(this IOrganizationService service, Expression<Func<T, object>> anonymousTypeInitializer, string columnName, params object[] values)
            where T : Entity
        {
            var columnSet = AddColumns<T>(new ColumnSet(), anonymousTypeInitializer);
            return service.GetEntitiesIn<T>(columnSet, columnName, values);
        }

        /// <summary>
        /// Gets first 5000 active entities (with the given subset of columns only)
        /// where the values are in the columnName
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="columnSet">Columns to Return</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static List<T> GetEntitiesIn<T>(this IOrganizationService service, ColumnSet columnSet, string columnName, params object[] values) where T : Entity
        {
            return service.RetrieveList<T>(QueryExpressionFactory.CreateIn<T>(columnSet, columnName, values));
        }

        #endregion // GetEntitisIn

        #region GetFirstOrDefaultIn

        /// <summary>
        /// Retreives the first active entity (with the given subset of columns only)
        /// where the values are in the columnName.
        /// </summary>
        /// <typeparam name="T">An early bound Entity Type.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static T GetFirstOrDefaultIn<T>(this IOrganizationService service, string columnName, IEnumerable values) where T : Entity
        {
            var settings = new QuerySettings<T>() { First = true };
            return service.RetrieveList<T>(settings.CreateInExpression(columnName, values)).FirstOrDefault();
        }

        /// <summary>
        /// Retreives the first active entity (with the given subset of columns only)
        /// where the values are in the columnName.
        /// </summary>
        /// <typeparam name="T">An early bound Entity Type.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static T GetFirstOrDefaultIn<T>(this IOrganizationService service, string columnName, params object[] values)
            where T : Entity
        {
            var settings = new QuerySettings<T>() { First = true };
            return service.RetrieveList<T>(settings.CreateInExpression(columnName, values)).FirstOrDefault();
        }

        /// <summary>
        /// Retreives the first active entity (with the given subset of columns only)
        /// where the values are in the columnName
        /// </summary>
        /// <typeparam name="T">An early bound Entity Type</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <param name="columnName">The name of the column to perform the in against</param>
        /// <param name="values">The list of values to search for being in the column name</param>
        /// <returns></returns>
        public static T GetFirstOrDefaultIn<T>(this IOrganizationService service, Expression<Func<T, object>> anonymousTypeInitializer, string columnName, IEnumerable values)
            where T : Entity
        {
            var columnSet = AddColumns<T>(new ColumnSet(), anonymousTypeInitializer);
            return service.GetFirstOrDefaultIn<T>(columnSet, columnName, values);
        }

        /// <summary>
        /// Retreives the first active entity (with the given subset of columns only)
        /// where the values are in the columnName.
        /// </summary>
        /// <typeparam name="T">An early bound Entity Type.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="columnSet">Columns to Return</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static T GetFirstOrDefaultIn<T>(this IOrganizationService service, ColumnSet columnSet, string columnName, IEnumerable values)
            where T : Entity
        {
            var settings = new QuerySettings<T>() { Columns = columnSet, First = true };
            return service.RetrieveList<T>(settings.CreateInExpression(columnName, values)).FirstOrDefault();
        }

        /// <summary>
        /// Retreives the first active entity (with the given subset of columns only)
        /// where the values are in the columnName
        /// </summary>
        /// <typeparam name="T">An early bound Entity Type</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <param name="columnName">The name of the column to perform the in against</param>
        /// <param name="values">The list of values to search for being in the column name</param>
        /// <returns></returns>
        public static T GetFirstOrDefaultIn<T>(this IOrganizationService service, Expression<Func<T, object>> anonymousTypeInitializer, string columnName, params object[] values)
            where T : Entity
        {
            var columnSet = AddColumns<T>(new ColumnSet(), anonymousTypeInitializer);
            return service.GetFirstOrDefaultIn<T>(columnSet, columnName, values);
        }

        /// <summary>
        /// Retreives the first active entity (with the given subset of columns only)
        /// where the values are in the columnName.
        /// </summary>
        /// <typeparam name="T">An early bound Entity Type.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="columnSet">Columns to Return</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static T GetFirstOrDefaultIn<T>(this IOrganizationService service, ColumnSet columnSet, string columnName, params object[] values)
            where T : Entity
        {
            var settings = new QuerySettings<T>() { Columns = columnSet, First = true };
            return service.RetrieveList<T>(settings.CreateInExpression(columnName, values)).FirstOrDefault();
        }

        #endregion // GetFirstOrDefaultIn

        #endregion // IOrganizationService

        #region LinkEntity

        /// <summary>
        /// Adds an In Condition to the LinkEntity LinkCriteria.
        /// </summary>
        /// <param name="linkEntity">The link entity.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static LinkEntity WhereIn(this LinkEntity linkEntity, string columnName, IEnumerable values)
        {
            linkEntity.LinkCriteria.WhereIn(columnName, values);
            return linkEntity;
        }
        /// <summary>
        /// Adds an In Condition to the LinkEntity LinkCriteria.
        /// </summary>
        /// <param name="linkEntity">The link entity.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static LinkEntity WhereIn(this LinkEntity linkEntity, string columnName, params object[] values)
        {
            linkEntity.LinkCriteria.WhereIn(columnName, values);
            return linkEntity;
        }

        #endregion // LinkEntity

        #region QueryExpression

        /// <summary>
        /// Adds an In Condition to the QueryExpression Criteria.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static QueryExpression WhereIn(this QueryExpression query, string columnName, IEnumerable values)
        {
            query.Criteria.WhereIn(columnName, values);
            return query;
        }

        /// <summary>
        /// Adds an In Condition to the QueryExpression Criteria.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static QueryExpression WhereIn(this QueryExpression query, string columnName, params object[] values)
        {
            query.Criteria.WhereIn(columnName, values);
            return query;
        }

        #endregion // QueryExpression
    }
}
