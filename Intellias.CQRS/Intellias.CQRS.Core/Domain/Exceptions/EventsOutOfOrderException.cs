using System;

namespace Product.Domain.Core.Domain.Exceptions
{
    /// <inheritdoc />
    public class EventsOutOfOrderException : Exception
    {
        /// <inheritdoc />
        public EventsOutOfOrderException(string id)
            : base($"Eventstore gave event for aggregate {id} out of order")
        { }

        /// <inheritdoc />
        public EventsOutOfOrderException()
        {
        }

        /// <inheritdoc />
        public EventsOutOfOrderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}