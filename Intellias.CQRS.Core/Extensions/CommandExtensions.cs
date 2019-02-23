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
        /// <typeparam name="TEvent">event without specific properties</typeparam>
        /// <param name="c">command</param>
        /// <returns></returns>
        public static TEvent ToSimpleDomainEvent<TEvent>(this Command c)
            where TEvent : Event, new()
        {
            var @event = new TEvent
            {
                AggregateRootId = c.AggregateRootId,
                Version = c.ExpectedVersion,
                SourceId = c.Id,
                CorrelationId = c.CorrelationId
            };

            @event.CopyMetadataFrom(c);

            return @event;
        }

        /// <summary>
        /// Converts some ProcessCommand type to another
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        public static TCommand ToType<TCommand>(this ProcessCommand command)
            where TCommand: ProcessCommand, new()
        {
            var result = ((AbstractMessage)command).ToType<TCommand>();
            result.ExpectedVersion = command.ExpectedVersion;
            result.Process = command.Process;

            return result;
        }
    }
}
