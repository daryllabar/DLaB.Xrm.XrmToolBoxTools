using System.Collections.Generic;
using System.Linq;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Comparers
#else
namespace Source.DLaB.Xrm.Comparers
#endif
	
{

    /// <summary>
    /// Taken from Stake Overflow question: http://stackoverflow.com/questions/50098/comparing-two-collections-for-equality-irrespective-of-the-order-of-items-in-them
    /// Checks for Value Comparison of a collection, allowing for the collection to not be in order, as long as they contain the same count.
    /// <para>The ability for it to accept an IEqualityComparer for type T was added.</para>
    ///  </summary>
    /// <typeparam name="T"></typeparam>
    public class EnumerableComparer<T> : IEqualityComparer<IEnumerable<T>>
    {

        private IEqualityComparer<T> Comparer { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumerableComparer{T}"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public EnumerableComparer(IEqualityComparer<T> comparer = null)
        {
            Comparer = comparer;
        }

        /// <summary>
        /// Checks for the give nlists to be equal
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns></returns>
        public bool Equals(IEnumerable<T> first, IEnumerable<T> second)
        {
            if (first == null)
                return second == null;

            if (second == null)
                return false;

            if (ReferenceEquals(first, second))
                return true;

            var secondCollection = second as ICollection<T>;
            if (!(first is ICollection<T> firstCollection) || secondCollection == null)
            {
                return !HaveMismatchedElement(first, second, Comparer);
            }
            if (firstCollection.Count != secondCollection.Count)
                return false;

            if (firstCollection.Count == 0)
                return true;

            return !HaveMismatchedElement(first, second, Comparer);
        }

        private static bool HaveMismatchedElement(IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer = null)
        {

            var firstElementCounts = GetElementCounts(first, out int firstCount, comparer);
            var secondElementCounts = GetElementCounts(second, out int secondCount, comparer);

            return firstCount != secondCount 
                   || 
                   firstElementCounts.Any(kvp => !secondElementCounts.TryGetValue(kvp.Key, out secondCount) 
                                                 ||
                                                 kvp.Value != secondCount);
        }

        private static Dictionary<T, int> GetElementCounts(IEnumerable<T> enumerable, out int nullCount, IEqualityComparer<T> comparer = null)
        {
            var dictionary = new Dictionary<T, int>(comparer);
            nullCount = 0;

            foreach (var element in enumerable)
            {
                // ReSharper disable once CompareNonConstrainedGenericWithNull
                if (element == null)
                {
                    nullCount++;
                }
                else
                {
                    dictionary.TryGetValue(element, out int num);
                    num++;
                    dictionary[element] = num;
                }
            }

            return dictionary;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public int GetHashCode(IEnumerable<T> enumerable)
        {
            if (Comparer == null)
            {
                return enumerable.OrderBy(x => x).
                     Aggregate(new HashCode(), (current, val) => current.Hash(val));   
            }

            // Since we have to ensure the items are sorted in the same order, we could require a comparer as well, 
            // but this seems more correct, although it may be more costly.

            // ReSharper disable once CompareNonConstrainedGenericWithNull
            return enumerable.OrderBy(x => (x == null ? new HashCode() : Comparer.GetHashCode(x))).
                              Aggregate(new HashCode(), (current, val) => current.Hash(val, Comparer));
        }
    }
}
