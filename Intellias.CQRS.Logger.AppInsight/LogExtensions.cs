using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Logger.AppInsight
{
    /// <summary>
    /// LogExtensions
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// ToLogConfig
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static OperationConfig ToLogConfig(this IMessage message) =>
            new OperationConfig
            {
                OperationId = message.CorrelationId,
                OperationName = message.TypeName,
                UserId = message.Metadata[MetadataKey.UserId]
            };
    }
}
