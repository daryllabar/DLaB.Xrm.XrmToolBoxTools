using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DLaB.Xrm.Common.Exceptions
{
    /// <summary>
    /// Exception that is thrown when the UpdationOption value of the import settings is set to UpdateOption.AssertUpdateOne, and more than one entity is found
    /// </summary>
    public class ImportMoreThanOneFoundException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportMoreThanOneFoundException"/> class.
        /// </summary>
        public ImportMoreThanOneFoundException() : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportMoreThanOneFoundException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ImportMoreThanOneFoundException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportMoreThanOneFoundException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public ImportMoreThanOneFoundException(string message, System.Exception inner) : base(message, inner) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportMoreThanOneFoundException"/> class.
        /// A constructor is needed for serialization when an
        /// exception propagates from a remoting server to the client.</summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        ///   
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected ImportMoreThanOneFoundException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }
}
