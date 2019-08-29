using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Events
{
    /// <inheritdoc cref="IEvent" />
    public abstract class Event : AbstractMessage, IEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        protected Event()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        /// <param name="version">Version.</param>
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
