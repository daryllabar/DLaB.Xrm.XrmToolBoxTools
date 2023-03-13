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
    /// Compares Paging Infos
    /// </summary>
    public class PagingInfoComparer : IEqualityComparer<PagingInfo>
    {
        private static IEqualityComparer<PagingInfo> Comparer { get; set; }

        static PagingInfoComparer()
        {
            Comparer = ProjectionEqualityComparer<PagingInfo>.Create(i => new { i.Count, i.PageNumber, i.PagingCookie, i.ReturnTotalRecordCount});
        }
        /// <summary>
        /// Compares the two page infos
        /// </summary>
        /// <param name="page1">The page1.</param>
        /// <param name="page2">The page2.</param>
        /// <returns></returns>
        public bool Equals(PagingInfo page1, PagingInfo page2)
        {
            return Comparer.Equals(page1, page2);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public int GetHashCode(PagingInfo info)
        {
            info.ThrowIfNull("info");
            return Comparer.GetHashCode(info);
        }
    }
}
