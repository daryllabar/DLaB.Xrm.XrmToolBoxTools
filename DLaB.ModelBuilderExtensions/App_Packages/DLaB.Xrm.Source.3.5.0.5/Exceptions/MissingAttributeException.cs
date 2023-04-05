using System;
using System.Runtime.Serialization;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Exceptions
#else
namespace Source.DLaB.Xrm.Exceptions
#endif
	
{
    /// <summary>
    /// Thrown when an Entity is expected to contain an Attribute, but the attribute isn't found
    /// </summary>
    [Serializable]
    public class MissingAttributeException : Exception
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingAttributeException"/> class.
        /// </summary>
        public MissingAttributeException()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingAttributeException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public MissingAttributeException(string message) : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingAttributeException"/> class.
        /// </summary>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="messageFormatArgs">The message format arguments.</param>
        public MissingAttributeException(string messageFormat, params object[] messageFormatArgs)
            : base(string.Format(messageFormat, messageFormatArgs))
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingAttributeException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public MissingAttributeException(string message, Exception innerException) : base(message, innerException)
        {

        }

        /// <summary>
        /// Without this constructor, deserialization will fail
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected MissingAttributeException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }

        #endregion Constructors
    }
}
