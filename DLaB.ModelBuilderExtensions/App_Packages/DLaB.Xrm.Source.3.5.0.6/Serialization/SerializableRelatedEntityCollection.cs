using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xrm.Sdk;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Sandbox.Serialization
#else
namespace Source.DLaB.Xrm.Sandbox.Serialization
#endif
	
{
    /// <summary>
    /// Sandbox safe Serialization for Related Entity Collection
    /// </summary>
    [CollectionDataContract(Name = "RelatedEntityCollection", Namespace = "http://schemas.datacontract.org/2004/07/System.Collections.Generic")]
    public class SerializableRelatedEntityCollection: List<KeyValuePairOfRelationshipEntityCollectionX_PsK4FkN>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableRelatedEntityCollection"/> class.
        /// </summary>
        public SerializableRelatedEntityCollection()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableRelatedEntityCollection"/> class.
        /// </summary>
        /// <param name="col">The col.</param>
        public SerializableRelatedEntityCollection(RelatedEntityCollection col)
        {
            foreach (var related in col)
            {
                Add(new KeyValuePairOfRelationshipEntityCollectionX_PsK4FkN(related.Key, related.Value));
            }
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="SerializableRelatedEntityCollection"/> to <see cref="RelatedEntityCollection"/>.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator RelatedEntityCollection(SerializableRelatedEntityCollection collection)
        {
            if (collection == null)
            {
                return null;
            }
            var xrmCollection = new RelatedEntityCollection();
            xrmCollection.AddRange(collection.Select(v => (KeyValuePair<Relationship, EntityCollection>)v));
            return xrmCollection;
        }
    }
}
