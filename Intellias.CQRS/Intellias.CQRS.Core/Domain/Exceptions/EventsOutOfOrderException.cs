using System;

namespace Product.Domain.Core.Domain.Exceptions
{
    public class EventsOutOfOrderException : Exception
    {
        public EventsOutOfOrderException(string id)
            : base($"Eventstore gave event for aggregate {id} out of order")
        { }
    }
}