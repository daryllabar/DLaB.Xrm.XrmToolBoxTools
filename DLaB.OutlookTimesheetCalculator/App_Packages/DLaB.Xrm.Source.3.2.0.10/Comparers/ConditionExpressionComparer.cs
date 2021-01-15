using System;
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
    /// Comparer for Condition Expressions.
    /// </summary>
    public class ConditionExpressionComparer : IEqualityComparer<ConditionExpression>
    {
        /// <summary>
        /// Compares the two conditions.
        /// </summary>
        /// <param name="condition1">The condition1.</param>
        /// <param name="condition2">The condition2.</param>
        /// <returns></returns>
        public bool Equals(ConditionExpression condition1, ConditionExpression condition2)
        {
            if (condition1 == condition2) { return true; }
            if (condition1 == null || condition2 == null) { return false; }

            return condition1.AttributeName == condition2.AttributeName &&
                   condition1.Operator == condition2.Operator &&
                   new EnumerableComparer<Object>().Equals(condition1.Values, condition2.Values);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public int GetHashCode(ConditionExpression condition)
        {
            condition.ThrowIfNull("condition");

            // Skip the more expensive checks for the hash code.  They are more likely to mutate as well...
            return new HashCode().
                Hash(condition.AttributeName).
                Hash(condition.Operator);
            // Hash(condition.Values, new EnumerableComparer<Object>());
        }
    }
}
