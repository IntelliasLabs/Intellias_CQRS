using System;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Config;
using Newtonsoft.Json;

namespace Intellias.CQRS.Core.Messages
{
    /// <summary>
    /// Extension for commands
    /// </summary>
    public static class MessageExtensions
    {
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

        /// <summary>
        /// IMessage to JSON
        /// </summary>
        /// <param name="msg">IMessage</param>
        /// <returns>JSON</returns>
        public static string ToJson(this IMessage msg)
        {
            return JsonConvert.SerializeObject(msg, CqrsSettings.JsonConfig());
        }
    }
}
