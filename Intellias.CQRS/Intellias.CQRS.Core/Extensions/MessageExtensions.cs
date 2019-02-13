using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Extensions
{
    /// <summary>
    /// Message extension methods collection
    /// </summary>
    public static class MessageExtensions
    {
        /// <summary>
        /// Converts abstract message to another type
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public static TMessage ToType<TMessage>(this AbstractMessage message)
            where TMessage: AbstractMessage, new ()
        {
            var result = new TMessage
            {
                Id = Unified.NewCode(),
                AggregateRootId = message.AggregateRootId,
                CorrelationId = message.CorrelationId
            };

            result.CopyMetadataFrom(message);

            return result;
        }
    }
}
