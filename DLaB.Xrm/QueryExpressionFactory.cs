using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System.Collections;
using System.Linq.Expressions;

namespace DLaB.Xrm
{
    /// <summary>
    /// Factory class for creating a QueryExpression using the default and or specified value.
    /// </summary>
    public class QueryExpressionFactory
    {
        #region Create

        /// <summary>
        /// Returns a Query Expression for the given inputs
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name","John Doe" goes to entity.name = "John Doe"</param>
        /// <returns></returns>
        public static QueryExpression Create<T>(params object[] columnNameAndValuePairs) where T : Entity
        {
            return Create<T>(new QuerySettings<T>(), columnNameAndValuePairs);
        }

        /// <summary>
        /// Returns a Query Expression for the given inputs
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name","John Doe" goes to entity.name = "John Doe"</param>
        /// <returns></returns>
        public static QueryExpression Create<T>(Expression<Func<T, object>> anonymousTypeInitializer, params object[] columnNameAndValuePairs)
            where T : Entity
        {
            var columnSet = new ColumnSet().AddColumns<T>(anonymousTypeInitializer);
            return Create<T>(columnSet, columnNameAndValuePairs);
        }

        /// <summary>
        /// Returns a Query Expression for the given inputs
        /// </summary>
        /// <typeparam name="T">The type of Entity</typeparam>
        /// <param name="columnSet">Columns to retrieve</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name","John Doe" goes to entity.name = "John Doe"</param>
        /// <returns></returns>
        public static QueryExpression Create<T>(ColumnSet columnSet, params object[] columnNameAndValuePairs) where T : Entity
        {
            return Create<T>(new QuerySettings<T>() { Columns = columnSet }, columnNameAndValuePairs);
        }

        /// <summary>
        /// Returns a Query Expression for the given inputs
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <param name="first">Used to specificy that only one entity should be returned</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name","John Doe" goes to entity.name = "John Doe"</param>
        /// <returns></returns>
        public static QueryExpression Create<T>(Expression<Func<T, object>> anonymousTypeInitializer, bool first, params object[] columnNameAndValuePairs)
            where T : Entity
        {
            var columnSet = new ColumnSet().AddColumns<T>(anonymousTypeInitializer);
            return Create<T>(columnSet, first, columnNameAndValuePairs);
        }

        /// <summary>
        /// Returns a Query Expression for the given inputs
        /// </summary>
        /// <typeparam name="T">The type of Entity</typeparam>
        /// <param name="columnSet">Columns to retrieve</param>
        /// <param name="first">Used to specificy that only one entity should be returned</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name","John Doe" goes to entity.name = "John Doe"</param>
        /// <returns></returns>
        public static QueryExpression Create<T>(ColumnSet columnSet, bool first, params object[] columnNameAndValuePairs) where T : Entity
        {
            return Create<T>(new QuerySettings<T>() { Columns = columnSet, First = first }, columnNameAndValuePairs);
        }

        /// <summary>
        /// Returns a Query Expression for the given inputs
        /// </summary>
        /// <typeparam name="T">The type of Entity to use as the base for the QueryExpression</typeparam>
        /// <param name="settings">The query settings used to create the Query Expression.</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name","John Doe" goes to entity.name = "John Doe"</param>
        /// <returns></returns>
        public static QueryExpression Create<T>(QuerySettings<T> settings, params object[] columnNameAndValuePairs) where T : Entity
        {
            var qe = Create<T>(settings);
            qe.WhereEqual(columnNameAndValuePairs);
            return qe;
        }

        /// <summary>
        /// Returns a Query Expression for the given inputs
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="activeOnly">Specifies if only Active Records should be returned</param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <param name="first">Used to specificy that only one entity should be returned</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name","John Doe" goes to entity.name = "John Doe"</param>
        /// <returns></returns>
        public static QueryExpression Create<T>(bool activeOnly, Expression<Func<T, object>> anonymousTypeInitializer, bool first, params object[] columnNameAndValuePairs)
            where T : Entity
        {
            var columnSet = new ColumnSet().AddColumns<T>(anonymousTypeInitializer);
            return Create<T>(activeOnly, columnSet, first, columnNameAndValuePairs);
        }

        /// <summary>
        /// Returns a Query Expression for the given inputs
        /// </summary>
        /// <typeparam name="T">The type of Entity</typeparam>
        /// <param name="columnSet">Columns to retrieve</param>
        /// <param name="first">Used to specificy that only one entity should be returned</param>
        /// <param name="activeOnly">Specifies if only Active Records should be returned</param>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name","John Doe" goes to entity.name = "John Doe"</param>
        /// <returns></returns>
        public static QueryExpression Create<T>(bool activeOnly, ColumnSet columnSet, bool first, params object[] columnNameAndValuePairs) where T : Entity
        {
            var qe = Create<T>(new QuerySettings<T>()
            {
                ActiveOnly = activeOnly,
                Columns = columnSet,
                First = first
            });
            qe.WhereEqual(columnNameAndValuePairs);
            return qe;
        }

        #endregion // Create

        #region CreateIn

        /// <summary>
        /// Returns a Query Expression for the given inputs
        /// </summary>
        /// <typeparam name="T">The type of Entity</typeparam>
        /// <param name="columnName">The name of the column to perform the in against</param>
        /// <param name="values">The list of values to search for being in the column name</param>
        public static QueryExpression CreateIn<T>(string columnName, IEnumerable values) where T : Entity
        {
            return CreateIn<T>(new QuerySettings<T>(), columnName, values);
        }

        /// <summary>
        /// Returns a Query Expression for the given inputs
        /// </summary>
        /// <typeparam name="T">The type of Entity</typeparam>
        /// <param name="columnName">The name of the column to perform the in against</param>
        /// <param name="values">The list of values to search for being in the column name</param>
        public static QueryExpression CreateIn<T>(string columnName, params object[] values) where T : Entity
        {
            return CreateIn<T>(new QuerySettings<T>(), columnName, values);
        }

        /// <summary>
        /// Returns a Query Expression for the given inputs
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <param name="columnName">The name of the column to perform the in against</param>
        /// <param name="values">The list of values to search for being in the column name</param>
        public static QueryExpression CreateIn<T>(Expression<Func<T, object>> anonymousTypeInitializer, string columnName, IEnumerable values)
            where T : Entity
        {
            var columnSet = new ColumnSet().AddColumns<T>(anonymousTypeInitializer);
            return CreateIn<T>(columnSet, columnName, values);
        }

        /// <summary>
        /// Returns a Query Expression for the given inputs
        /// </summary>
        /// <typeparam name="T">The type of Entity</typeparam>
        /// <param name="columnSet">Columns to retrieve</param>
        /// <param name="columnName">The name of the column to perform the in against</param>
        /// <param name="values">The list of values to search for being in the column name</param>
        public static QueryExpression CreateIn<T>(ColumnSet columnSet, string columnName, IEnumerable values)
            where T : Entity
        {
            return CreateIn<T>(new QuerySettings<T>() { Columns = columnSet }, columnName, values);
        }

        /// <summary>
        /// Returns a Query Expression for the given inputs
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <param name="columnName">The name of the column to perform the in against</param>
        /// <param name="values">The list of values to search for being in the column name</param>
        public static QueryExpression CreateIn<T>(Expression<Func<T, object>> anonymousTypeInitializer, string columnName, params object[] values)
            where T : Entity
        {
            var columnSet = new ColumnSet().AddColumns<T>(anonymousTypeInitializer);
            return CreateIn<T>(columnSet, columnName, values);
        }

        /// <summary>
        /// Returns a Query Expression for the given inputs
        /// </summary>
        /// <typeparam name="T">The type of Entity</typeparam>
        /// <param name="columnSet">Columns to retrieve</param>
        /// <param name="columnName">The name of the column to perform the in against</param>
        /// <param name="values">The list of values to search for being in the column name</param>
        public static QueryExpression CreateIn<T>(ColumnSet columnSet, string columnName, params object[] values) 
            where T : Entity
        {
            return CreateIn<T>(new QuerySettings<T>() { Columns = columnSet }, columnName, values);
        }

        /// <summary>
        /// Returns a Query Expression for the given inputs
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <param name="first">Used to specificy that only one entity should be returned</param>
        /// <param name="columnName">The name of the column to perform the in against</param>
        /// <param name="values">The list of values to search for being in the column name</param>
        public static QueryExpression CreateIn<T>(Expression<Func<T, object>> anonymousTypeInitializer, bool first, string columnName, IEnumerable values)
            where T : Entity
        {
            var columnSet = new ColumnSet().AddColumns<T>(anonymousTypeInitializer);
            return CreateIn<T>(columnSet, first, columnName, values);
        }

        /// <summary>
        /// Returns a Query Expression for the given inputs
        /// </summary>
        /// <typeparam name="T">The type of Entity</typeparam>
        /// <param name="columnSet">Columns to retrieve</param>
        /// <param name="first">Used to specificy that only one entity should be returned</param>
        /// <param name="columnName">The name of the column to perform the in against</param>
        /// <param name="values">The list of values to search for being in the column name</param>
        public static QueryExpression CreateIn<T>(ColumnSet columnSet, bool first, string columnName, IEnumerable values)
            where T : Entity
        {
            return CreateIn<T>(new QuerySettings<T>() { Columns = columnSet, First = first }, columnName, values);
        }

        /// <summary>
        /// Returns a Query Expression for the given inputs
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <param name="first">Used to specificy that only one entity should be returned</param>
        /// <param name="columnName">The name of the column to perform the in against</param>
        /// <param name="values">The list of values to search for being in the column name</param>
        public static QueryExpression CreateIn<T>(Expression<Func<T, object>> anonymousTypeInitializer, bool first, string columnName, params object[] values)
            where T : Entity
        {
            var columnSet = new ColumnSet().AddColumns<T>(anonymousTypeInitializer);
            return CreateIn<T>(columnSet, first, columnName, values);
        }

        /// <summary>
        /// Returns a Query Expression for the given inputs
        /// </summary>
        /// <typeparam name="T">The type of Entity</typeparam>
        /// <param name="columnSet">Columns to retrieve</param>
        /// <param name="first">Used to specificy that only one entity should be returned</param>
        /// <param name="columnName">The name of the column to perform the in against</param>
        /// <param name="values">The list of values to search for being in the column name</param>
        public static QueryExpression CreateIn<T>(ColumnSet columnSet, bool first, string columnName, params object[] values)
            where T : Entity
        {
            return CreateIn<T>(new QuerySettings<T>() { Columns = columnSet, First = first }, columnName, values);
        }

        /// <summary>
        /// Returns a Query Expression for the given inputs
        /// </summary>
        /// <typeparam name="T">The type of Entity to use as the base for the QueryExpression</typeparam>
        /// <param name="settings">The query settings used to create the Query Expression.</param>
        /// <param name="columnName">The name of the column to perform the in against</param>
        /// <param name="values">The list of values to search for being in the column name</param>
        /// <returns></returns>
        public static QueryExpression CreateIn<T>(QuerySettings<T> settings, string columnName, params object[] values) where T : Entity
        {
            var qe = Create<T>(settings);
            qe.WhereIn(columnName, values);
            return qe;
        }

        /// <summary>
        /// Returns a Query Expression for the given inputs
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <param name="first">Used to specificy that only one entity should be returned</param>
        /// <param name="activeOnly">Specifies if only Active Records should be returned</param>
        /// <param name="columnName">The name of the column to perform the in against</param>
        /// <param name="values">The list of values to search for being in the column name</param>
        public static QueryExpression CreateIn<T>(bool activeOnly, Expression<Func<T, object>> anonymousTypeInitializer, bool first, string columnName, IEnumerable values)
            where T : Entity
        {
            var columnSet = new ColumnSet().AddColumns<T>(anonymousTypeInitializer);
            return CreateIn<T>(activeOnly, columnSet, first, columnName, values);
        }

        /// <summary>
        /// Returns a Query Expression for the given inputs
        /// </summary>
        /// <typeparam name="T">The type of Entity to use as the base for the QueryExpression</typeparam>
        /// <param name="columnSet">Columns to retrieve</param>
        /// <param name="first">Used to specificy that only one entity should be returned</param>
        /// <param name="activeOnly">Specifies if only Active Records should be returned</param>
        /// <param name="columnName">The name of the column to perform the in against</param>
        /// <param name="values">The list of values to search for being in the column name</param>
        public static QueryExpression CreateIn<T>(bool activeOnly, ColumnSet columnSet, bool first, string columnName, IEnumerable values)
            where T : Entity
        {
            var qe = Create<T>(new QuerySettings<T>()
            {
                ActiveOnly = activeOnly,
                Columns = columnSet,
                First = first
            });
            qe.WhereIn(columnName, values);
            return qe;
        }

        /// <summary>
        /// Returns a Query Expression for the given inputs
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <param name="first">Used to specificy that only one entity should be returned</param>
        /// <param name="activeOnly">Specifies if only Active Records should be returned</param>
        /// <param name="columnName">The name of the column to perform the in against</param>
        /// <param name="values">The list of values to search for being in the column name</param>
        public static QueryExpression CreateIn<T>(bool activeOnly, Expression<Func<T, object>> anonymousTypeInitializer, bool first, string columnName, params object[] values)
            where T : Entity
        {
            var columnSet = new ColumnSet().AddColumns<T>(anonymousTypeInitializer);
            return CreateIn<T>(activeOnly, columnSet, first, columnName, values);
        }

        /// <summary>
        /// Returns a Query Expression for the given inputs
        /// </summary>
        /// <typeparam name="T">The type of Entity to use as the base for the QueryExpression</typeparam>
        /// <param name="columnSet">Columns to retrieve</param>
        /// <param name="first">Used to specificy that only one entity should be returned</param>
        /// <param name="activeOnly">Specifies if only Active Records should be returned</param>
        /// <param name="columnName">The name of the column to perform the in against</param>
        /// <param name="values">The list of values to search for being in the column name</param>
        public static QueryExpression CreateIn<T>(bool activeOnly, ColumnSet columnSet, bool first, string columnName, params object[] values)
            where T : Entity
        {
            var qe = Create<T>(new QuerySettings<T>()
            {
                ActiveOnly = activeOnly,
                Columns = columnSet,
                First = first
            });
            qe.WhereIn(columnName, values);
            return qe;
        }

        /// <summary>
        /// Creates a Query Expression using the specified settings.
        /// </summary>
        /// <typeparam name="T">The type of Entity to use as the base for the QueryExpression</typeparam>
        /// <param name="settings">The query settings used to create the Query Expression.</param>
        /// <returns></returns>
        public static QueryExpression Create<T>(QuerySettings<T> settings) where T : Entity
        {
            var qe = new QueryExpression()
            {
                EntityName = Extensions.GetEntityLogicalName<T>(),
                ColumnSet = settings.Columns
            };

            qe.Criteria.FilterOperator = settings.CriteriaOperator;

            if (settings.First)
            {
                qe.First();
            }

            if (settings.ActiveOnly)
            {
                qe.ActiveOnly<T>();
            }

            if (settings.AllCustomColumns)
            {
                qe.AllCustomColumns<T>();
            }

            return qe;
        }

        #endregion // CreateIn
    }
}
