using System;
using System.Collections.Generic;
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
    /// Taken from http://stackoverflow.com/questions/716552/can-you-create-a-simple-equalitycomparert-using-a-lamba-expression,
    /// which Jon Skeet copied from his MiscUtil.
    /// </summary>
    /// 
    public static class ProjectionEqualityComparer
    {
        /// <summary>
        /// Creates the comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="projection">The projection.</param>
        /// <returns></returns>
        public static ProjectionEqualityComparer<TSource, TKey> Create<TSource, TKey>(Func<TSource, TKey> projection)
        {
            return new ProjectionEqualityComparer<TSource, TKey>(projection);
        }

        /// <summary>
        /// Creates the comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="ignored">The ignored.</param>
        /// <param name="projection">The projection.</param>
        /// <returns></returns>
        public static ProjectionEqualityComparer<TSource, TKey> Create<TSource, TKey>(TSource ignored, Func<TSource, TKey> projection)
        {
            return new ProjectionEqualityComparer<TSource, TKey>(projection);
        }
    }

    /// <summary>
    /// Generic ProjectionEqualityComparer
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    public static class ProjectionEqualityComparer<TSource>
    {
        /// <summary>
        /// Creates the specified projection.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="projection">The projection.</param>
        /// <returns></returns>
        public static ProjectionEqualityComparer<TSource, TKey>
            Create<TKey>(Func<TSource, TKey> projection)
        {
            return new ProjectionEqualityComparer<TSource, TKey>(projection);
        }
    }

    /// <summary>
    /// Generic ProjectionEqualityComparer
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    public class ProjectionEqualityComparer<TSource, TKey>
        : IEqualityComparer<TSource>
    {
        readonly Func<TSource, TKey> _projection;
        readonly IEqualityComparer<TKey> _comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionEqualityComparer{TSource, TKey}"/> class.
        /// </summary>
        /// <param name="projection">The projection.</param>
        public ProjectionEqualityComparer(Func<TSource, TKey> projection)
            : this(projection, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionEqualityComparer{TSource, TKey}"/> class.
        /// </summary>
        /// <param name="projection">The projection.</param>
        /// <param name="comparer">The comparer.</param>
        public ProjectionEqualityComparer(
            Func<TSource, TKey> projection,
            IEqualityComparer<TKey> comparer)
        {
            projection.ThrowIfNull("projection");
            _comparer = comparer ?? EqualityComparer<TKey>.Default;
            _projection = projection;
        }

        /// <summary>
        /// Compares the two sources.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public bool Equals(TSource x, TSource y)
        {
            // ReSharper disable CompareNonConstrainedGenericWithNull
            if (x == null && y == null)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            // ReSharper restore CompareNonConstrainedGenericWithNull

            return _comparer.Equals(_projection(x), _projection(y));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        /// <exception cref="System.ArgumentNullException">obj</exception>
        public int GetHashCode(TSource obj)
        {
            // ReSharper disable once CompareNonConstrainedGenericWithNull
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            return _comparer.GetHashCode(_projection(obj));
        }
    }
}
