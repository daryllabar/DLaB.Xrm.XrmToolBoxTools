using System;

#if DLAB_UNROOT_COMMON_NAMESPACE
namespace DLaB.Common.Exceptions
#else
namespace Source.DLaB.Common.Exceptions
#endif
{
    /// <summary>
    /// The exception that is thrown when a switch statement has been defined for an Enum, but the actual value has not been defined as a case.
    /// Example:
    /// 	var c = BindingFlags.GetField;
    ///     try{
    ///     switch (c)
    ///     {
    ///        case BindingFlags.Default:
    ///        break;
    ///        default:
    ///        throw new EnumCaseUndefinedException&lt;BindingFlags&gt;(c, "Unable to perform reflection operation");
    ///     }
    /// </summary>
    [Serializable]

    public class EnumCaseUndefinedException<TEnum> : Exception where TEnum : struct
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        protected EnumCaseUndefinedException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception. </param><param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. </param>
        protected EnumCaseUndefinedException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumCaseUndefinedException{TEnum}"/> class.
        /// </summary>
        /// <param name="undefinedEnumValue">The undefined enum value.</param>
        public EnumCaseUndefinedException(TEnum undefinedEnumValue) : base(GetMessage(undefinedEnumValue)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumCaseUndefinedException{TEnum}"/> class.
        /// </summary>
        /// <param name="undefinedEnumValue">The undefined enum value.</param>
        /// <param name="message">The message.</param>
        public EnumCaseUndefinedException(TEnum undefinedEnumValue, string message) : base(GetMessage(undefinedEnumValue, message)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumCaseUndefinedException{TEnum}"/> class.
        /// </summary>
        /// <param name="undefinedEnumValue">The undefined enum value.</param>
        /// <param name="inner">The inner exception.</param>
        public EnumCaseUndefinedException(TEnum undefinedEnumValue, Exception inner) : base(GetMessage(undefinedEnumValue), inner) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumCaseUndefinedException{TEnum}"/> class.
        /// </summary>
        /// <param name="undefinedEnumValue">The undefined enum value.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public EnumCaseUndefinedException(TEnum undefinedEnumValue, string message, Exception inner) : base(GetMessage(undefinedEnumValue, message), inner) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumCaseUndefinedException{TEnum}"/> class.
        /// </summary>
        /// <param name="undefinedEnumValue">The undefined enum value.</param>
        public EnumCaseUndefinedException(int undefinedEnumValue) : base(GetMessage(undefinedEnumValue)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumCaseUndefinedException{TEnum}"/> class.
        /// </summary>
        /// <param name="undefinedEnumValue">The undefined enum value.</param>
        /// <param name="message">The message.</param>
        public EnumCaseUndefinedException(int undefinedEnumValue, string message) : base(GetMessage(undefinedEnumValue, message)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumCaseUndefinedException{TEnum}"/> class.
        /// </summary>
        /// <param name="undefinedEnumValue">The undefined enum value.</param>
        /// <param name="inner">The inner.</param>
        public EnumCaseUndefinedException(int undefinedEnumValue, Exception inner) : base(GetMessage(undefinedEnumValue), inner) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumCaseUndefinedException{TEnum}"/> class.
        /// </summary>
        /// <param name="undefinedEnumValue">The undefined enum value.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public EnumCaseUndefinedException(int undefinedEnumValue, string message, Exception inner) : base(GetMessage(undefinedEnumValue, message), inner) { }

        #endregion Constructors

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <param name="undefinedEnumValue">The undefined enum value.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        protected static string GetMessage(int undefinedEnumValue, string message = null)
        {
            var enumType = typeof(TEnum);
            if (!IsEnum(enumType, ref message)) { return message; }

            return FormatMessage(message, enumType, (TEnum)(object)undefinedEnumValue, undefinedEnumValue);
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <param name="undefinedEnumValue">The undefined enum value.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        protected static string GetMessage(TEnum undefinedEnumValue, string message = null)
        {
            var enumType = typeof(TEnum);
            if (!IsEnum(enumType, ref message)) { return message; }

            return FormatMessage(message, enumType, undefinedEnumValue, (int)(object)undefinedEnumValue);
        }

        /// <summary>
        /// Formats the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="enumType">Type of the enum.</param>
        /// <param name="undefinedEnumValue">The undefined enum value.</param>
        /// <param name="undefinedEnumIntValue">The undefined enum int value.</param>
        /// <returns></returns>
        protected static string FormatMessage(string message, Type enumType, TEnum undefinedEnumValue, int undefinedEnumIntValue)
        {
            return $"{message}No case statement for {enumType.FullName}.{undefinedEnumValue} ({undefinedEnumIntValue}) has been defined!";
        }

        /// <summary>
        /// Determines whether the specified enum type is enum.
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        protected static bool IsEnum(Type enumType, ref string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                message = string.Empty;
            }
            else
            {
                message += Environment.NewLine;
            }

            if (!enumType.IsEnum)
            {
                message = message + enumType.FullName + " is not an enum, this is an invalid use of UndefinedEnumCaseException";
                return false;
            }
            return true;
        }
    }
}