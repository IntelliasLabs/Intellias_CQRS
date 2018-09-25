using System;

namespace Product.Domain.Core.Domain.Exceptions
{
    /// <inheritdoc />
    public class AggregateOrEventMissingIdException : Exception
    {
        /// <inheritdoc />
        public AggregateOrEventMissingIdException(Type aggregateType, Type eventType)
            : base($"An event of type {eventType.FullName} was tried to save from {aggregateType.FullName} but no id where set on either")
        { }

        /// <inheritdoc />
        public AggregateOrEventMissingIdException()
        {
        }

        /// <inheritdoc />
        public AggregateOrEventMissingIdException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public AggregateOrEventMissingIdException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}