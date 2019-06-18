using System;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        /// <summary>
        /// Parse abstract message
        /// </summary>
        /// <param name="json">input json</param>
        /// <returns>object</returns>
        public static IMessage MessageFromJson(this string json)
        {
            var jObject = JObject.Parse(json);
            string typeName;
            typeName = jObject.SelectToken(nameof(typeName)).ToString();
            return (IMessage)jObject.ToObject(Type.GetType(typeName));
        }

        /// <summary>
        /// Copy metadata to another message instance
        /// </summary>
        /// <param name="from">source object</param>
        /// <param name="to">target object</param>
        public static void CopyMetadata(this IMessage from, IMessage to)
        {
            foreach (var key in from.Metadata.Keys)
            {
                to.Metadata[key] = from.Metadata[key];
            }
        }

        /// <summary>
        /// Converts abstract message to another type
        /// </summary>
        /// <param name="source">source message</param>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        public static TMessage ToType<TMessage>(this IMessage source)
            where TMessage : AbstractMessage, new()
        {
            var result = new TMessage
            {
                Id = Unified.NewCode(),
                AggregateRootId = source.AggregateRootId,
                CorrelationId = source.CorrelationId
            };

            source.CopyMetadata(result);

            return result;
        }
    }
}
