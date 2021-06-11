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
    /// Sandbox safe FormattedValueCollection
    /// </summary>
    [CollectionDataContract(Name = "FormattedValueCollection", Namespace = "http://schemas.datacontract.org/2004/07/System.Collections.Generic")]
    public class SerializableFormattedValueCollection: List<KeyValuePairOfstringstring>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableFormattedValueCollection"/> class.
        /// </summary>
        public SerializableFormattedValueCollection() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableFormattedValueCollection"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public SerializableFormattedValueCollection(FormattedValueCollection values)
        {
            foreach (var value in values)
            {
                Add(new KeyValuePairOfstringstring(value));
            }
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="SerializableFormattedValueCollection"/> to <see cref="FormattedValueCollection"/>.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator FormattedValueCollection(SerializableFormattedValueCollection collection)
        {
            if (collection == null)
            {
                return null;
            }
            var xrmCollection = new FormattedValueCollection();
            xrmCollection.AddRange(collection.Select(v =>(KeyValuePair<string, string>)v));
            return xrmCollection;
        }
    }
}
