using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;

namespace DLaB.Xrm
{
    /// <summary>
    /// DTO class used to specify the defaults that a QueryExpression should be created with
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QuerySettings<T> where T : Entity
    {
        /// <summary>
        /// Columns to retrieve
        /// </summary>
        public ColumnSet Columns { get; set; }

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

        private bool _allCustomColumns;
        private bool _allCustomColumnsRemovedAllColumnsFlag;
        /// <summary>
        /// Adds all custom columns to the columns set (columns that have an underscore for their 4th character)
        /// </summary>
        public bool AllCustomColumns
        {
            get
            {
                return _allCustomColumns;
            }
            set
            {
                // If All Custom Columns is being enabled, and All Columns is set to true, remove the flag and keep record that it was done
                // That way if the value ever gets disabled, the All Columns flag can get added back
                if (value && !_allCustomColumns && Columns.AllColumns)
                {
                    Columns.AllColumns = false;
                    _allCustomColumnsRemovedAllColumnsFlag = true;
                }
                else if (!value && _allCustomColumns && _allCustomColumnsRemovedAllColumnsFlag)
                {
                    Columns.AllColumns = true;
                    _allCustomColumnsRemovedAllColumnsFlag = false;
                }
                _allCustomColumns = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuerySettings&lt;T&gt;"/> class, setting the default property values
        /// </summary>
        public QuerySettings()
        {
            Columns = new ColumnSet(true);
            First = false;
            ActiveOnly = true;
            AllCustomColumns = false;
            CriteriaOperator = LogicalOperator.And;
        }

        /// <summary>
        /// Creates a query expression.
        /// </summary>
        /// <returns></returns>
        public QueryExpression CreateExpression()
        {
            return QueryExpressionFactory.Create<T>(this);
        }

        /// <summary>
        /// Creates a query expression, containing a criteria for the specified columnNameAndValuePairs
        /// </summary>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name","John Doe" goes to entity.name = "John Doe"</param>
        /// <returns></returns>
        public QueryExpression CreateExpression(params object[] columnNameAndValuePairs)
        {
            return QueryExpressionFactory.Create<T>(this, columnNameAndValuePairs);
        }

        /// <summary>
        /// Creates a query expression, containing a criteria for the specified in values
        /// </summary>
        /// <param name="columnName">The name of the column to perform the in against</param>
        /// <param name="values">The list of values to search for being in the column name</param>
        /// <returns></returns>
        public QueryExpression CreateInExpression(string columnName, params object[] values)
        {
            return QueryExpressionFactory.CreateIn<T>(this, columnName, values);
        }
    }

}
