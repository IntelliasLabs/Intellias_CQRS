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
        public int Version
        {
            get => version;
            set
            {
                version = value;
                Id = Unified.NewCode(Version);
            }
        }
    }
}
