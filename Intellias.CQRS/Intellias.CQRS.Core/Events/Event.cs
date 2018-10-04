using System;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Events
{
    /// <inheritdoc cref="IEvent" />
    public abstract class Event : AbstractMessage, IEvent
    {
        /// <inheritdoc />
        protected Event()
        {
            Id = $"{Version}";
        }

        /// <inheritdoc />
        public string AggregateRootId { get; set; }

        /// <inheritdoc />
        public int Version { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Date and time when event was created
        /// </summary>
        public DateTime Created { get; } = DateTime.UtcNow;
    }
}
