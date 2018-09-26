using System;

namespace Intellias.CQRS.Core.Events
{
    /// <inheritdoc />
    public abstract class Event : IEvent
    {
        /// <inheritdoc />
        public string AggregateRootId { get; set; }

        /// <inheritdoc />
        public int Version { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Date and time when event was created
        /// </summary>
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
