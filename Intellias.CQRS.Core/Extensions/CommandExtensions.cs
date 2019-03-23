using System;
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

        /// <summary>
        /// Validates if common command properties are filled and not empty
        /// </summary>
        /// <param name="command"></param>
        public static void Validate(this Command command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (string.IsNullOrEmpty(command.Id))
            {
                throw new ArgumentNullException($"Command id should be set");
            }

            if (string.IsNullOrEmpty(command.AggregateRootId))
            {
                throw new ArgumentNullException($"AggregateRoot id in the command '{command.Id}' should be set");
            }

            if (string.IsNullOrEmpty(command.CorrelationId))
            {
                throw new ArgumentNullException($"CorrelationId in the command '{command.Id}' should be set");
            }

            if (!command.Metadata.ContainsKey(MetadataKey.Roles))
            {
                throw new ArgumentNullException($"'{nameof(MetadataKey.Roles)}' should be set in the command '{command.Id}");
            }

            if (!command.Metadata.ContainsKey(MetadataKey.UserId))
            {
                throw new ArgumentNullException($"'{nameof(MetadataKey.UserId)}' should be set in the command '{command.Id}");
            }

            if (!Guid.TryParse(command.Metadata[MetadataKey.UserId], out _))
            {
                throw new FormatException($"'{nameof(MetadataKey.UserId)}' can't be parsed to guid in the command '{command.Id}");
            }
        }
    }
}
