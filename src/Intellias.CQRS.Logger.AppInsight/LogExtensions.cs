using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Logger.AppInsight
{
    /// <summary>
    /// LogExtensions.
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// ToLogConfig.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <returns>OperationConfig.</returns>
        public static OperationConfig ToLogConfig(this IMessage message)
        {
            return new OperationConfig
            {
                OperationId = message.CorrelationId,
                OperationName = message.TypeName,
                UserId = message.Metadata[MetadataKey.UserId],
            };
        }
    }
}
