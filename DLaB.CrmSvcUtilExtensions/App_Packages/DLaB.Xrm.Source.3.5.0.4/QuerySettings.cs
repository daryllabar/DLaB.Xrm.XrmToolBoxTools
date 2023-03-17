using System;
using System.Diagnostics;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System.Linq.Expressions;
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
    /// Typed QuerySettings class
    /// </summary>
    /// <typeparam name="T"></typeparam>
#if !DLAB_XRM_DEBUG
    [DebuggerNonUserCode]
#endif
    public class QuerySettings<T> where T: Entity
    {
        /// <summary>
        /// Columns to retrieve
        /// </summary>
        public ColumnSet Columns { get; set; }

        /// <summary>
        /// Adds the specified additional columns to the Columns ColumnSet
        /// </summary>
        public Expression<Func<T, object>> AdditionalColumns
        {
            set
            {
                if (Columns == null)
                {
                    Columns = new ColumnSet(false);
                }
                Columns.AddColumns(value);
            }
        }

        /// <summary>
        /// Used to specify that only one entity should be returned
        /// </summary>
        public bool First { get; set; }

        /// <summary>
        /// Specifies if only Active Records should be returned
        /// </summary>
        public bool ActiveOnly { get; set; }

        /// <summary>
        /// Specifies the Logical Operator of the Criteria
        /// </summary>
        public LogicalOperator CriteriaOperator { get; set; }

        /// <summary>
        /// Gets or sets the name of the logical.
        /// </summary>
        /// <value>
        /// The name of the logical.
        /// </value>
        public string LogicalName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuerySettings{T}"/> class.
        /// </summary>
        /// <exception cref="TypeArgumentException">'Entity' is an invalid type for T.  Please use the LateBoundQuerySettings.</exception>
        public QuerySettings()
        {
            var type = typeof(T);
            if(type == typeof(Entity))
            {
                throw new TypeArgumentException("'Entity' is an invalid type for T.  Please use the LateBoundQuerySettings.");
            }
            DefaultSettings(EntityHelper.GetEntityLogicalName<T>());
        }

        private void DefaultSettings(string logicalName)
        {
            Columns = SolutionCheckerAvoider.CreateColumnSetWithAllColumns();
            First = false;
            ActiveOnly = false;
            CriteriaOperator = LogicalOperator.And;
            LogicalName = logicalName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuerySettings{T}"/> class.
        /// </summary>
        /// <param name="logicalName">Name of the logical.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        protected QuerySettings(string logicalName){
            if (logicalName == null)
            {
                throw new ArgumentNullException(nameof(logicalName));
            }
            DefaultSettings(logicalName);
        }

        /// <summary>
        /// Creates the expression.
        /// </summary>
        /// <returns></returns>
        public QueryExpression CreateExpression(){
            return QueryExpressionFactory.Create(this);
        }

        /// <summary>
        /// </summary>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name","John Doe" goes to entity.name = "John Doe"</param>
        /// <returns></returns>
        public QueryExpression CreateExpression(params object[] columnNameAndValuePairs)
        {
            return QueryExpressionFactory.Create(this, columnNameAndValuePairs);
        }

        /// <summary>
        /// </summary>
        /// <param name="columnName">The name of the column to perform the in against</param>
        /// <param name="values">The list of values to search for being in the column name</param>
        /// <returns></returns>
        public QueryExpression CreateInExpression(string columnName, params object[] values)
        {
            return QueryExpressionFactory.CreateIn(this, columnName, values);
        }
    }

}
