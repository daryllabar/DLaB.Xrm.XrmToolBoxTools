using System.Collections.Generic;
using System.Runtime.Serialization;

// ReSharper disable InconsistentNaming
#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Sandbox.Serialization
#else
namespace Source.DLaB.Xrm.Sandbox.Serialization
#endif
	
{
    /// <summary>
    /// Sandbox Serialization Safe KeyValuePairOfstringstring
    /// </summary>
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/System.Collections.Generic")]
    public struct KeyValuePairOfstringstring
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string key { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValuePairOfstringstring"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public KeyValuePairOfstringstring(string key, string value)
        {
            this.key = key;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValuePairOfstringstring"/> class.
        /// </summary>
        /// <param name="kvp">The KVP.</param>
        public KeyValuePairOfstringstring(KeyValuePair<string, string> kvp) : this(kvp.Key, kvp.Value)
        {

        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="KeyValuePairOfstringstring"/> to KeyValuePair{System.String, System.String}.
        /// </summary>
        /// <param name="pair">The pair.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator KeyValuePair<string, string>(KeyValuePairOfstringstring pair)
        {
            return new KeyValuePair<string, string>(pair.key, pair.value);
        }
    }
}
