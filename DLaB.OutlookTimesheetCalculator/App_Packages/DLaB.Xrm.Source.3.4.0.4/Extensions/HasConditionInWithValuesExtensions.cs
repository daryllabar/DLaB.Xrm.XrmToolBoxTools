using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xrm.Sdk.Query;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm
#else
namespace Source.DLaB.Xrm
#endif
	
{
    public partial class Extensions
    {
        #region ConditionExpression

        private static bool ValuesInConditionIn(this ConditionExpression c1, string attributeName, IEnumerable<object> values)
        {
            var list = values.ToList();
            return (c1 != null && attributeName != null && list.Any() &&
                c1.AttributeName == attributeName &&
                c1.Operator == ConditionOperator.In &&
                !list.Except(c1.Values).Any()); // http://stackoverflow.com/questions/332973/linq-check-whether-an-array-is-a-subset-of-another
        }

        #endregion ConditionExpression

        #region FilterExpression

        /// <summary>
        /// Determines whether current filter has the in condition defined by the columnNameAndValuePairs
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="columnNameAndValuePairs">The column name and value pairs.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">HasConditionInWithValues requires the first value in the columnNameAndValuePairs attribute to be the attribute name.;columnNameAndValuePairs</exception>
        public static bool HasConditionInWithValues(this FilterExpression filter, params object[] columnNameAndValuePairs)
        {
            if (!(columnNameAndValuePairs[0] is string attributeName))
            {
                throw new ArgumentException("HasConditionInWithValues requires the first value in the columnNameAndValuePairs attribute to be the attribute name.", nameof(columnNameAndValuePairs));
            }

            return filter.Conditions.Any(c => c.ValuesInConditionIn(attributeName, columnNameAndValuePairs.Skip(1))) ||
                filter.Filters.Any(f => f.HasConditionInWithValues(columnNameAndValuePairs));
        }

        #endregion FilterExpression

        #region LinkEntity

        /// <summary>
        /// Determines whether [has condition in with values] [the specified column name and value pairs].
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="columnNameAndValuePairs">The column name and value pairs.</param>
        /// <returns></returns>
        public static bool HasConditionInWithValues(this LinkEntity link, params object[] columnNameAndValuePairs)
        {
            return link.LinkCriteria.HasConditionInWithValues(columnNameAndValuePairs) || link.LinkEntities.Any(l => l.HasConditionInWithValues(columnNameAndValuePairs));
        }

        #endregion LinkEntity

        #region QueryExpression

        /// <summary>
        /// Determines whether current query expression has the in condition defined by the columnNameAndValuePairs
        /// </summary>
        /// <param name="qe">The qe.</param>
        /// <param name="columnNameAndValuePairs">The column name and value pairs.</param>
        /// <returns></returns>
        public static bool HasConditionInWithValues(this QueryExpression qe, params object[] columnNameAndValuePairs)
        {
            return qe.Criteria.HasConditionInWithValues(columnNameAndValuePairs) ||
                  qe.LinkEntities.Any(l => l.HasConditionInWithValues(columnNameAndValuePairs));
        }

        #endregion QueryExpression
    }
}
