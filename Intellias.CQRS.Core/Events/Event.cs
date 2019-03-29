using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Events
{
    /// <inheritdoc cref="IEvent" />
    public abstract class Event : AbstractMessage, IEvent
    {
        /// <inheritdoc />
        protected Event() {}

        /// <inheritdoc />
        protected Event(int version)
        {
            Version = version;
        }

        /// <inheritdoc />
        public string SourceId { get; set; } = string.Empty;

        /// <inheritdoc />
        public int Version { get; set; }
    }
}
