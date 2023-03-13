using Microsoft.Xrm.Sdk.Query;
#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
using DLaB.Xrm.Comparers;

namespace DLaB.Xrm
#else
using Source.DLaB.Xrm.Comparers;

namespace Source.DLaB.Xrm
#endif

{
    public static partial class Extensions
    {
        #region ColumnSet

        /// <summary>
        /// Determines whether the specified column set is equal.
        /// </summary>
        /// <param name="cs">The cs.</param>
        /// <param name="columnSet">The column set.</param>
        /// <returns></returns>
        public static bool IsEqual(this ColumnSet cs, ColumnSet columnSet)
        {
            return new ColumnSetComparer().Equals(cs, columnSet);
        }

        #endregion ColumnSet

        #region FilterExpression

        /// <summary>
        /// Determines whether the specified filter is equal.
        /// </summary>
        /// <param name="fe">The fe.</param>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public static bool IsEqual(this FilterExpression fe, FilterExpression filter)
        {
            return new FilterExpressionComparer().Equals(fe, filter);
        }

        #endregion FilterExpression

        #region PagingInfo

        /// <summary>
        /// Determines whether the specified paging info is equal.
        /// </summary>
        /// <param name="infoThis">The information this.</param>
        /// <param name="info">The paging info.</param>
        /// <returns></returns>
        public static bool IsEqual(this PagingInfo infoThis, PagingInfo info)
        {
            return new PagingInfoComparer().Equals(infoThis, info);
        }

        #endregion PagingInfo

        #region QueryExpression

        /// <summary>
        /// Determines whether the specified qe is equal.
        /// </summary>
        /// <param name="qeThis">The qe this.</param>
        /// <param name="qe">The qe.</param>
        /// <returns></returns>
        public static bool IsEqual(this QueryExpression qeThis, QueryExpression qe)
        {
            return new QueryExpressionComparer().Equals(qeThis, qe);
        }

        #endregion QueryExpression
    }
}
