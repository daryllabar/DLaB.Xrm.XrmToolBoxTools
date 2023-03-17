using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;
#if DLAB_UNROOT_COMMON_NAMESPACE
using DLaB.Common;
#else
using Source.DLaB.Common;
#endif

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Comparers
#else
namespace Source.DLaB.Xrm.Comparers
#endif
{
    /// <summary>
    /// Comparer for FilterExpressions
    /// </summary>
    public class FilterExpressionComparer : IEqualityComparer<FilterExpression>
    {
        /// <summary>
        /// Compares the two filters
        /// </summary>
        /// <param name="filter1">The filter1.</param>
        /// <param name="filter2">The filter2.</param>
        /// <returns></returns>
        public bool Equals(FilterExpression filter1, FilterExpression filter2)
        {
            if (filter1 == filter2) { return true; }
            if (filter1 == null || filter2 == null) { return false; }

            return new EnumerableComparer<ConditionExpression>(new ConditionExpressionComparer()).Equals(filter1.Conditions, filter2.Conditions) &&
                   filter1.FilterOperator == filter2.FilterOperator &&
                   new EnumerableComparer<FilterExpression>(new FilterExpressionComparer()).Equals(filter1.Filters, filter2.Filters) &&
                   filter1.IsQuickFindFilter == filter2.IsQuickFindFilter;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public int GetHashCode(FilterExpression filter)
        {
            filter.ThrowIfNull("filter");
            return new HashCode()
                .Hash(filter.Conditions, new EnumerableComparer<ConditionExpression>(new ConditionExpressionComparer()))
                .Hash(filter.FilterOperator)
                .Hash(filter.Filters, new EnumerableComparer<FilterExpression>(new FilterExpressionComparer()))
                .Hash(filter.IsQuickFindFilter);
        }
    }
}
