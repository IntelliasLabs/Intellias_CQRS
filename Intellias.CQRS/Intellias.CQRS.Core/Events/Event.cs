using System;
using System.ComponentModel.DataAnnotations;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Events
{
    /// <inheritdoc cref="IEvent" />
    public abstract class Event : AbstractMessage, IEvent
    {
        private int version;

        /// <inheritdoc />
        protected Event()
        {
        }

        /// <inheritdoc />
        protected Event(int version)
        {
            Version = version;
        }

        /// <inheritdoc />
        public string AggregateRootId { get; set; }

        /// <inheritdoc />
        public int Version
        {
            get => version;
            set
            {
                version = value;
                Id = Unified.NewCode(Version);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Date and time when event was created
        /// </summary>
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Created { get; } = DateTime.UtcNow;
    }
}
