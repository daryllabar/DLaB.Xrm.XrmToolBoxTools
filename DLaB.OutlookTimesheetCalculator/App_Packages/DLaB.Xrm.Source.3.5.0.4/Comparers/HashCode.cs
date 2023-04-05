using System.Collections.Generic;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Comparers
#else
namespace Source.DLaB.Xrm.Comparers
#endif
	
{

    /// <summary>
    /// Taken from http://stackoverflow.com/a/18613926/227436 an answer for http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
    /// Changed methods so it was a new, followed by Hash.
    /// Added overload for Comparers
    /// </summary>
    public struct HashCode
    {
        private const int SeedPrime = 17;
        private const int MultiperPrime = 23;
        private readonly int _hash;
        private readonly bool _hashIsSet;

        private HashCode(int hash)
        {
            _hash = hash;
            _hashIsSet = true;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="HashCode"/> to <see cref="System.Int32"/>.
        /// </summary>
        /// <param name="hashCode">The hash code.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator int(HashCode hashCode)
        {
            return hashCode.GetHashCode();
        }

        /// <summary>
        /// Creates a HashCode for the given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns></returns>
        public HashCode Hash<T>(T obj, IEqualityComparer<T> comparer = null)
        {
            var h = 0;
            // ReSharper disable once CompareNonConstrainedGenericWithNull
            if (obj != null) {
                h = comparer?.GetHashCode(obj) ?? obj.GetHashCode();   
            }
            unchecked { h += GetHashCode() * MultiperPrime; }
            return new HashCode(h);
        }


        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return _hashIsSet ? _hash : SeedPrime;
        }
    }
}
