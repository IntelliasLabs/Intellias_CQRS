using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Extensions
{
    /// <summary>
    /// Extension for commands
    /// </summary>
    public static class CommandExtensions
    {
        /// <summary>
        /// Converts common command data to event
        /// </summary>
        /// <typeparam name="E">event without specific properties</typeparam>
        /// <param name="c">command</param>
        /// <returns></returns>
        public static E ToSimpleDomainEvent<E>(this Command c)
            where E : Event, new()
        {
            var @event = new E
            {
                AggregateRootId = c.AggregateRootId,
                Version = c.ExpectedVersion,
                SourceId = c.Id
            };

            @event.CopyMetadataFrom(c);

            return @event;
        }
    }
}
