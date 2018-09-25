using System;

namespace Product.Domain.Core.Events
{
    /// <inheritdoc />
    public abstract class Event : IEvent
    {
        /// <inheritdoc />
        public string Id { get; set; }

        /// <inheritdoc />
        public int Version { get; set; }

        /// <summary>
        /// Date and time when event was created
        /// </summary>
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
