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
    /// Comparer for LinkEntities
    /// </summary>
    public class LinkEntityComparer : IEqualityComparer<LinkEntity>
    {
        /// <summary>
        /// Compares the two link entities.
        /// </summary>
        /// <param name="link1">The link1.</param>
        /// <param name="link2">The link2.</param>
        /// <returns></returns>
        public bool Equals(LinkEntity link1, LinkEntity link2)
        {
            if (link1 == link2) { return true; }
            if (link1 == null || link2 == null) { return false; }

            return 
                // Cheap checks First
                link1.EntityAlias == link2.EntityAlias &&
                link1.JoinOperator == link2.JoinOperator &&
                link1.LinkFromAttributeName == link2.LinkFromAttributeName &&
                link1.LinkFromEntityName == link2.LinkFromEntityName &&
                link1.LinkToAttributeName == link2.LinkToAttributeName &&
                link1.LinkToEntityName == link2.LinkToEntityName &&
                // More Expensive Second
                link1.Columns.IsEqual(link2.Columns) &&
                link1.LinkCriteria.IsEqual(link2.LinkCriteria) &&
                new EnumerableComparer<LinkEntity>(new LinkEntityComparer()).Equals(link1.LinkEntities, link2.LinkEntities);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public int GetHashCode(LinkEntity link)
        {
            link.ThrowIfNull("link");
            // Skip the more expensive checks for the hashCode.  They are more likely to mutate as well...
            return new HashCode()
                // .Hash(link.Columns, new ColumnSetComparer())
                .Hash(link.EntityAlias)
                .Hash(link.JoinOperator)
                // .Hash(link.LinkCriteria, new FilterExpressionComparer())
                // .Hash(link.LinkEntities, new EnumerableComparer<LinkEntity>(new LinkEntityComparer()))
                .Hash(link.LinkFromAttributeName)
                .Hash(link.LinkFromEntityName)
                .Hash(link.LinkToAttributeName)
                .Hash(link.LinkToEntityName);
        }
    }
}

