using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xrm.Sdk;
// ReSharper disable InconsistentNaming

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Sandbox.Serialization
#else
namespace Source.DLaB.Xrm.Sandbox.Serialization
#endif
	
{
    /// <summary>
    /// Sandbox Serialization Safe KeyValuePairOfRelationship
    /// </summary>
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/System.Collections.Generic")]
    public struct KeyValuePairOfRelationshipEntityCollectionX_PsK4FkN
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public Relationship key { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public SerializableEntityCollection value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValuePairOfRelationshipEntityCollectionX_PsK4FkN"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public KeyValuePairOfRelationshipEntityCollectionX_PsK4FkN(Relationship key, SerializableEntityCollection value)
        {
            this.key = key;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValuePairOfRelationshipEntityCollectionX_PsK4FkN"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public KeyValuePairOfRelationshipEntityCollectionX_PsK4FkN(Relationship key, EntityCollection value): this(key, new SerializableEntityCollection(value))
        {
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="KeyValuePairOfRelationshipEntityCollectionX_PsK4FkN"/> to <see cref="KeyValuePair{Relationship, EntityCollection}"/>.
        /// </summary>
        /// <param name="pair">The pair.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator KeyValuePair<Relationship, EntityCollection>(KeyValuePairOfRelationshipEntityCollectionX_PsK4FkN pair)
        {
            return new KeyValuePair<Relationship, EntityCollection>(pair.key, (EntityCollection)pair.value);
        }
    }
}
