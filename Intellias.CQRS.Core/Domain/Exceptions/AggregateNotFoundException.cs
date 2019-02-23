using System;

namespace Intellias.CQRS.Core.Domain.Exceptions
{
    /// <inheritdoc />
    public class AggregateNotFoundException : Exception
    {
        /// <inheritdoc />
        public AggregateNotFoundException(Type t, Guid id)
            : base($"Aggregate {id} of type {t.FullName} was not found")
        { }

        /// <inheritdoc />
        public AggregateNotFoundException()
        {
        }

        /// <inheritdoc />
        public AggregateNotFoundException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public AggregateNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
