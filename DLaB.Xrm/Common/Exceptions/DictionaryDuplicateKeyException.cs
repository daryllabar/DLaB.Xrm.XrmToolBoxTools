using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DLaB.Xrm.Common.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an attempt is made to add a duplicate key to a dictionary.
    /// It's primary purpose is to provide a better error message than ArgumentException, An item with the same key has already been added.
    /// </summary>
    [Serializable()]
    public class DictionaryDuplicateKeyException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryDuplicateKeyException"/> class.
        /// </summary>
        public DictionaryDuplicateKeyException() : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryDuplicateKeyException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public DictionaryDuplicateKeyException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryDuplicateKeyException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public DictionaryDuplicateKeyException(string message, System.Exception inner) : base(message, inner) { }


        /// <summary>
        /// A constructor is needed for serialization when an
        /// exception propagates from a remoting server to the client.</summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        ///   
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected DictionaryDuplicateKeyException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) {}


    }
}
