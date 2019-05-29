using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Commands
{
    /// <inheritdoc cref="ICommand" />
    public abstract class Command : AbstractMessage, ICommand
    {
        /// <inheritdoc />
        public int ExpectedVersion { get; set; }

        /// <summary>
        /// Converts common command data to event
        /// </summary>
        /// <typeparam name="TEvent">event without specific properties</typeparam>
        /// <returns></returns>
        public TEvent ToEvent<TEvent>()
            where TEvent : Event, new()
        {
            var e = this.ToType<TEvent>();
            e.SourceId = Id;
            e.Version = ExpectedVersion;

            return e;
        }
    }
}
