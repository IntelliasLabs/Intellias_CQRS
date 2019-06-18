using System;
using System.Runtime.Serialization;

namespace Intellias.CQRS.Core.Exceptions
{
    /// <summary>
    /// Business rule validation exception
    /// </summary>
    [Serializable]
    public class BusinessRuleValidationException : Exception
    {

        /// <summary>
        /// Constructor
        /// </summary>
        internal BusinessRuleValidationException()
        {
        }

        /// <summary>
        /// Exception constructor
        /// </summary>
        /// <param name="validationMessage"></param>
        public BusinessRuleValidationException(string validationMessage)
            : base(validationMessage)
        {
            ValidationMessage = validationMessage;
        }

        /// <summary>
        /// Exception constructor
        /// </summary>
        /// <param name="validationMessage"></param>
        /// <param name="innerException"></param>
        public BusinessRuleValidationException(string validationMessage, Exception innerException)
            : base(validationMessage, innerException)
        {
            ValidationMessage = validationMessage;
        }

        /// <summary>
        /// Serialization constructor
        /// </summary>
        /// <param name="serializationInfo"></param>
        /// <param name="streamingContext"></param>
        protected BusinessRuleValidationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
            serializationInfo.AddValue(nameof(ValidationMessage), ValidationMessage);
        }

        /// <summary>
        /// Business rule validation message
        /// </summary>
        public string ValidationMessage { get; private set; } = string.Empty;
    }
}
