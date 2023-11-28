using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System.Collections;
using System.Linq.Expressions;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm
#else
namespace Source.DLaB.Xrm
#endif
	
{
    public static partial class Extensions
    {
        #region FilterExpression

        /// <summary>
        /// Adds an In Condition to the FilterExpression
        /// </summary>
        /// <param name="filterExpression">The FilterExpression to apply the In Constraints to.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">An IEnumerable of values to search for being in the column name.</param>
        public static FilterExpression WhereIn(this FilterExpression filterExpression, string columnName, IEnumerable values)
        {
            // Since value type arrays are not covariant in C#, they will not be converted to an object[] if passed
            // into this method, but instead a single object of type array.  Manually convert this to an object array
            AddWhereInCondition(filterExpression, columnName, values.Cast<object>().ToArray());
            return filterExpression;
        }

        /// <summary>
        /// Adds an In Condition to the FilterExpression
        /// </summary>
        /// <param name="filterExpression">The FilterExpression to apply the In Constraints to.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        public static FilterExpression WhereIn(this FilterExpression filterExpression, string columnName, params object[] values)
        {
            if (values.Length == 1){
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
        /// to wrap the string in to an object array for the correct method overloading resolution
        /// </summary>
        /// <param name="filterExpression">The FilterExpression to apply the In Constraints to.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="value">The single value to search for being in the column name.</param>
        public static FilterExpression WhereIn(this FilterExpression filterExpression, string columnName, string value)
        {
            AddWhereInCondition(filterExpression, columnName, new object[] {value});
            return filterExpression;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterExpression">The FilterExpression to apply the In Constraints to.</param>
        /// <param name="columnName"></param>
        /// <param name="values"></param>
        private static void AddWhereInCondition(FilterExpression filterExpression, string columnName, object[] values)
        {
            if(values.Length == 0){
                // If no values are in our values array, create a constraint which causing nothing to get returned
                // ReSharper disable once InconsistentNaming
                var ANDing = new FilterExpression(LogicalOperator.And);
                ANDing.AddCondition(columnName, ConditionOperator.Null);
                ANDing.AddCondition(columnName, ConditionOperator.NotNull);
                filterExpression.AddFilter(ANDing);
            }
            else if (values.Any(v => v == null))
            {
                if (values.Any(v => v != null))
                {
                    // ReSharper disable once InconsistentNaming
                    var ORing = new FilterExpression(LogicalOperator.Or);
                    ORing.AddCondition(columnName, ConditionOperator.Null);
                    ORing.AddCondition(columnName, ConditionOperator.In, values.Where(v => v != null));
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

        #endregion FilterExpression

        #region IOrganizationService

        #region GetAllEntitiesIn

        /// <summary>
        /// Gets all Active Entities where the values are in the columnName
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return.</typeparam>
        /// <param name="service"></param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllEntitiesIn<T>(this IOrganizationService service,
                string columnName, IEnumerable values) where T : Entity
        {
            return RetrieveAllEntities<T>.GetAllEntities(service, QueryExpressionFactory.CreateIn<T>(columnName, values));
        }

        /// <summary>
        /// Gets all Active Entities where the values are in the columnName
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return.</typeparam>
        /// <param name="service"></param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllEntitiesIn<T>(this IOrganizationService service,
                string columnName, params object[] values) where T : Entity
        {
            return RetrieveAllEntities<T>.GetAllEntities(service, QueryExpressionFactory.CreateIn<T>(columnName, values));
        }

        /// <summary>
        /// Gets all Active Entities (with the given subset of columns only) 
        /// where the values are in the columnName
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return.</typeparam>
        /// <param name="service"></param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllEntitiesIn<T>(this IOrganizationService service, 
                Expression<Func<T, object>> anonymousTypeInitializer, string columnName, IEnumerable values)
            where T : Entity
        {
            var columnSet = AddColumns(new ColumnSet(), anonymousTypeInitializer);
            return service.GetAllEntitiesIn<T>(columnSet, columnName, values);
        }

        /// <summary>
        /// Gets all Active Entities (with the given subset of columns only) 
        /// where the values are in the columnName
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return.</typeparam>
        /// <param name="service"></param>
        /// <param name="columnSet">Columns to Return.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllEntitiesIn<T>(this IOrganizationService service, ColumnSet columnSet,
                string columnName, IEnumerable values) where T : Entity
        {
            return RetrieveAllEntities<T>.GetAllEntities(service, QueryExpressionFactory.CreateIn<T>(columnSet, columnName, values));
        }

        /// <summary>
        /// Gets all Active Entities (with the given subset of columns only) 
        /// where the values are in the columnName
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return.</typeparam>
        /// <param name="service"></param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllEntitiesIn<T>(this IOrganizationService service, 
                Expression<Func<T, object>> anonymousTypeInitializer, string columnName, params object[] values)
            where T : Entity
        {
            var columnSet = AddColumns(new ColumnSet(), anonymousTypeInitializer);
            return service.GetAllEntitiesIn<T>(columnSet, columnName, values);
        }

        /// <summary>
        /// Gets all Active Entities (with the given subset of columns only) 
        /// where the values are in the columnName
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return.</typeparam>
        /// <param name="service"></param>
        /// <param name="columnSet">Columns to Return.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllEntitiesIn<T>(this IOrganizationService service, ColumnSet columnSet,
                string columnName, params object[] values) where T : Entity
        {
            return RetrieveAllEntities<T>.GetAllEntities(service, QueryExpressionFactory.CreateIn<T>(columnSet, columnName, values));
        }

        #endregion GetAllEntitiesIn

        #region GetEntitiesIn

        /// <summary>
        /// Gets first 5000 Active Entities where the values are in the columnName
        /// </summary>
        /// <param name="service"></param>
        /// <param name="logicalName">The LogicalName of the Entity to query.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static List<Entity> GetEntitiesIn(this IOrganizationService service, string logicalName,
                string columnName, IEnumerable values) 
        {
            var settings = new LateBoundQuerySettings(logicalName);
            return service.RetrieveMultiple(settings.CreateInExpression(columnName, values)).Entities.ToList();
        }

        /// <summary>
        /// Gets first 5000 Active Entities where the values are in the columnName
        /// </summary>
        /// <param name="service"></param>
        /// <param name="logicalName">The LogicalName of the Entity to query.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static List<Entity> GetEntitiesIn(this IOrganizationService service, string logicalName,
                string columnName, params object[] values) 
        {
            var settings = new LateBoundQuerySettings(logicalName);
            return service.RetrieveMultiple(settings.CreateInExpression(columnName, values)).Entities.ToList();
        }

        /// <summary>
        /// Gets first 5000 Active Entities (with the given subset of columns only) 
        /// where the values are in the columnName
        /// </summary>
        /// <param name="service"></param>
        /// <param name="logicalName">The LogicalName of the Entity to query.</param>
        /// <param name="columnSet">Columns to Return.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static List<Entity> GetEntitiesIn(this IOrganizationService service, string logicalName,
            ColumnSet columnSet, string columnName, IEnumerable values) 
        {
            var settings = new LateBoundQuerySettings(logicalName)
            {
                Columns = columnSet,
            };
            return service.RetrieveMultiple(settings.CreateInExpression(columnName, values)).Entities.ToList();
        }

        /// <summary>
        /// Gets first 5000 Active Entities (with the given subset of columns only) 
        /// where the values are in the columnName
        /// </summary>
        /// <param name="service"></param>
        /// <param name="logicalName">The LogicalName of the Entity to query.</param>
        /// <param name="columnSet">Columns to Return.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static List<Entity> GetEntitiesIn(this IOrganizationService service, string logicalName,
                ColumnSet columnSet, string columnName, params object[] values) 
        {
            var settings = new LateBoundQuerySettings(logicalName)
            {
                Columns = columnSet,
            };
            return service.RetrieveMultiple(settings.CreateInExpression(columnName, values)).Entities.ToList();
        }

        #endregion GetEntitiesIn

        #region GetEntitiesIn<T>

        /// <summary>
        /// Gets first 5000 Active Entities where the values are in the columnName
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return.</typeparam>
        /// <param name="service"></param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static List<T> GetEntitiesIn<T>(this IOrganizationService service,
                string columnName, IEnumerable values) where T : Entity
        {
            return service.GetEntities(QueryExpressionFactory.CreateIn<T>(columnName, values));
        }

        /// <summary>
        /// Gets first 5000 Active Entities where the values are in the columnName
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return.</typeparam>
        /// <param name="service"></param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static List<T> GetEntitiesIn<T>(this IOrganizationService service,
                string columnName, params object[] values) where T : Entity
        {
            return service.GetEntities(QueryExpressionFactory.CreateIn<T>(columnName, values));
        }

        /// <summary>
        /// Gets first 5000 Active Entities (with the given subset of columns only) 
        /// where the values are in the columnName
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return.</typeparam>
        /// <param name="service"></param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static List<T> GetEntitiesIn<T>(this IOrganizationService service, 
                Expression<Func<T, object>> anonymousTypeInitializer, string columnName, IEnumerable values)
            where T : Entity
        {
            var columnSet = AddColumns(new ColumnSet(), anonymousTypeInitializer);
            return service.GetEntitiesIn<T>(columnSet, columnName, values);
        }

        /// <summary>
        /// Gets first 5000 Active Entities (with the given subset of columns only) 
        /// where the values are in the columnName
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return.</typeparam>
        /// <param name="service"></param>
        /// <param name="columnSet">Columns to Return.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static List<T> GetEntitiesIn<T>(this IOrganizationService service, ColumnSet columnSet,
                string columnName, IEnumerable values) where T : Entity
        {
            return service.GetEntities(QueryExpressionFactory.CreateIn<T>(columnSet, columnName, values));
        }

        /// <summary>
        /// Gets first 5000 Active Entities (with the given subset of columns only) 
        /// where the values are in the columnName
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return.</typeparam>
        /// <param name="service"></param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static List<T> GetEntitiesIn<T>(this IOrganizationService service, 
                Expression<Func<T, object>> anonymousTypeInitializer, string columnName, params object[] values)
            where T : Entity
        {
            var columnSet = AddColumns(new ColumnSet(), anonymousTypeInitializer);
            return service.GetEntitiesIn<T>(columnSet, columnName, values);
        }

        /// <summary>
        /// Gets first 5000 Active Entities (with the given subset of columns only) 
        /// where the values are in the columnName
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return.</typeparam>
        /// <param name="service"></param>
        /// <param name="columnSet">Columns to Return.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static List<T> GetEntitiesIn<T>(this IOrganizationService service, ColumnSet columnSet,
                string columnName, params object[] values) where T : Entity
        {
            return service.GetEntities(QueryExpressionFactory.CreateIn<T>(columnSet, columnName, values));
        }

        #endregion GetEntitiesIn<T>

        #region GetFirstOrDefaultIn

        /// <summary>
        /// Retrieves the first active entity (with the given subset of columns only) 
        /// where the values are in the columnName
        /// </summary>
        /// <typeparam name="T">An early bound Entity Type.</typeparam>
        /// <param name="service"></param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static T GetFirstOrDefaultIn<T>(this IOrganizationService service,
                string columnName, IEnumerable values) where T : Entity
        {
            var settings = new QuerySettings<T> { First = true };
            return service.GetEntities<T>(settings.CreateInExpression(columnName, values)).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the first active entity (with the given subset of columns only) 
        /// where the values are in the columnName
        /// </summary>
        /// <typeparam name="T">An early bound Entity Type.</typeparam>
        /// <param name="service"></param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static T GetFirstOrDefaultIn<T>(this IOrganizationService service,
                string columnName, params object[] values) where T : Entity
        {
            var settings = new QuerySettings<T> { First = true };
            return service.GetEntities<T>(settings.CreateInExpression(columnName, values)).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the first active entity (with the given subset of columns only) 
        /// where the values are in the columnName
        /// </summary>
        /// <typeparam name="T">An early bound Entity Type.</typeparam>
        /// <param name="service"></param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static T GetFirstOrDefaultIn<T>(this IOrganizationService service, 
                Expression<Func<T, object>> anonymousTypeInitializer, string columnName, IEnumerable values)
            where T : Entity
        {
            var columnSet = AddColumns(new ColumnSet(), anonymousTypeInitializer);
            return service.GetFirstOrDefaultIn<T>(columnSet, columnName, values);
        }

        /// <summary>
        /// Retrieves the first active entity (with the given subset of columns only) 
        /// where the values are in the columnName
        /// </summary>
        /// <typeparam name="T">An early bound Entity Type.</typeparam>
        /// <param name="service"></param>
        /// <param name="columnSet">Columns to Return.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static T GetFirstOrDefaultIn<T>(this IOrganizationService service, ColumnSet columnSet,
                string columnName, IEnumerable values) where T : Entity
        {
            var settings = new QuerySettings<T> { Columns = columnSet, First = true };
            return service.GetEntities<T>(settings.CreateInExpression(columnName, values)).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the first active entity (with the given subset of columns only) 
        /// where the values are in the columnName
        /// </summary>
        /// <typeparam name="T">An early bound Entity Type</typeparam>
        /// <param name="service"></param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static T GetFirstOrDefaultIn<T>(this IOrganizationService service, 
                Expression<Func<T, object>> anonymousTypeInitializer, string columnName, params object[] values)
            where T : Entity
        {
            var columnSet = AddColumns(new ColumnSet(), anonymousTypeInitializer);
            return service.GetFirstOrDefaultIn<T>(columnSet, columnName, values);
        }

        /// <summary>
        /// Retrieves the first active entity (with the given subset of columns only) 
        /// where the values are in the columnName
        /// </summary>
        /// <typeparam name="T">An early bound Entity Type</typeparam>
        /// <param name="service"></param>
        /// <param name="columnSet">Columns to Return.</param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        /// <returns></returns>
        public static T GetFirstOrDefaultIn<T>(this IOrganizationService service, ColumnSet columnSet,
                string columnName, params object[] values) where T : Entity
        {
            var settings = new QuerySettings<T> { Columns = columnSet, First = true };
            return service.GetEntities<T>(settings.CreateInExpression(columnName, values)).FirstOrDefault();
        }

        #endregion GetFirstOrDefaultIn

        #endregion IOrganizationService

        #region LinkEntity

        /// <summary>
        /// Adds an In Condition to the LinkEntity LinkCriteria
        /// </summary>
        /// <param name="linkEntity"></param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        public static LinkEntity WhereIn(this LinkEntity linkEntity, string columnName, IEnumerable values)
        {

            linkEntity.LinkCriteria.WhereIn(columnName, new Object[]{values});
            return linkEntity;
        }

        /// <summary>
        /// Adds an In Condition to the LinkEntity LinkCriteria
        /// </summary>
        /// <param name="linkEntity"></param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        public static LinkEntity WhereIn(this LinkEntity linkEntity, string columnName, params object[] values)
        {
            linkEntity.LinkCriteria.WhereIn(columnName, values);
            return linkEntity;
        }

        #endregion LinkEntity

        #region QueryExpression

        /// <summary>
        /// Adds an In Condition to the QueryExpression Criteria
        /// </summary>
        /// <param name="query"></param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        public static QueryExpression WhereIn(this QueryExpression query, string columnName, IEnumerable values)
        {
            query.Criteria.WhereIn(columnName, values);
            return query;
        }

        /// <summary>
        /// Adds an In Condition to the QueryExpression Criteria
        /// </summary>
        /// <param name="query"></param>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        public static QueryExpression WhereIn(this QueryExpression query, string columnName, params object[] values)
        {
            query.Criteria.WhereIn(columnName, values);
            return query;
        }

        #endregion QueryExpression
    }
}
