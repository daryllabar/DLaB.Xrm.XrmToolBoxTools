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
    /// Comparer for ColumnSets
    /// </summary>
    public class ColumnSetComparer : IEqualityComparer<ColumnSet>
    {
        /// <summary>
        /// Compares the two Column Sets
        /// </summary>
        /// <param name="cs1">The CS1.</param>
        /// <param name="cs2">The CS2.</param>
        /// <returns></returns>
        public bool Equals(ColumnSet cs1, ColumnSet cs2)
        {
            if (cs1 == cs2) { return true; }
            if (cs1 == null || cs2 == null) { return false; }

            return cs1.AllColumns == cs2.AllColumns
                && new EnumerableComparer<string>().Equals(cs1.Columns, cs2.Columns);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="cs">The cs.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public int GetHashCode(ColumnSet cs)
        {
            cs.ThrowIfNull("cs");
            return new EnumerableComparer<string>().GetHashCode(cs.Columns);
        }
    }
}
