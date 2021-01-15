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
    /// Sandbox safe Serializable Attribute Collection
    /// </summary>
    [CollectionDataContract(Name = "AttributeCollection", Namespace = "http://schemas.microsoft.com/xrm/2011/Contracts")]
    public class SerializableAttributeCollection : List<KeyValuePairOfstringanyType>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableAttributeCollection"/> class.
        /// </summary>
        public SerializableAttributeCollection() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableAttributeCollection"/> class.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        public SerializableAttributeCollection(AttributeCollection attributes)
        {
            foreach (var att in attributes)
            {
                Add(new KeyValuePairOfstringanyType(att));
            }
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="SerializableAttributeCollection"/> to <see cref="AttributeCollection"/>.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator AttributeCollection(SerializableAttributeCollection collection)
        {
            if (collection == null)
            {
                return null;
            }
            var xrmCollection = new AttributeCollection();
            xrmCollection.AddRange(collection.Select(v => (KeyValuePair<string, object>) v));
            return  xrmCollection;
        }
    }
}
