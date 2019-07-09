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
            return JsonConvert.DeserializeObject<IMessage>(json, CqrsSettings.JsonConfig());
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
