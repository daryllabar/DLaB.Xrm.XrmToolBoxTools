using System;

#if DLAB_UNROOT_COMMON_NAMESPACE
namespace DLaB.Common.Exceptions
#else
namespace Source.DLaB.Common.Exceptions
#endif
{
    /// <summary>
    /// The exception that is thrown when an attempt is made to add a duplicate key to a dictionary.
    /// It's primary purpose is to provide a better error message than ArgumentException, An item with the smae key has already been added.
    /// </summary>
    [Serializable]
    public class DictionaryDuplicateKeyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryDuplicateKeyException"/> class.
        /// </summary>
        public DictionaryDuplicateKeyException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryDuplicateKeyException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DictionaryDuplicateKeyException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryDuplicateKeyException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public DictionaryDuplicateKeyException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryDuplicateKeyException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <remarks>
        /// A constructor is needed for serialization when an
        /// exception propagates from a remoting server to the client.
        /// </remarks>
        protected DictionaryDuplicateKeyException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }


    }
}
