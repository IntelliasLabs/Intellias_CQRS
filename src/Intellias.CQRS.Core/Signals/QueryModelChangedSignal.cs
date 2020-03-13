using System;
using Intellias.CQRS.Core.Messages;
using Newtonsoft.Json;

namespace Intellias.CQRS.Core.Signals
{
    /// <summary>
    /// Signal that notifies that query model is changed.
    /// </summary>
    public class QueryModelChangedSignal : AbstractMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryModelChangedSignal"/> class.
        /// </summary>
        /// <param name="queryModelId">Value for <see cref="QueryModelId"/>.</param>
        /// <param name="queryModelVersion">Value for <see cref="QueryModelVersion"/>.</param>
        /// <param name="queryModelType">Value for <see cref="QueryModelType"/>.</param>
        /// <param name="operation">Value for <see cref="Operation"/>.</param>
        [JsonConstructor]
        public QueryModelChangedSignal(string queryModelId, int queryModelVersion, Type queryModelType, QueryModelChangeOperation operation)
        {
            QueryModelId = queryModelId;
            QueryModelVersion = queryModelVersion;
            QueryModelType = queryModelType;
            Operation = operation;
        }

        /// <summary>
        /// Query model id.
        /// </summary>
        public string QueryModelId { get; }

        /// <summary>
        /// Query model version.
        /// </summary>
        public int QueryModelVersion { get; }

        /// <summary>
        /// Query model type.
        /// </summary>
        public Type QueryModelType { get; }

        /// <summary>
        /// Query model change operation.
        /// </summary>
        public QueryModelChangeOperation Operation { get; }

        /// <summary>
        /// Creates signal from integration event when query model has no version.
        /// </summary>
        /// <param name="message">Source message.</param>
        /// <param name="queryModelId">Value for <see cref="QueryModelId"/>.</param>
        /// <param name="queryModelType">Value for <see cref="QueryModelType"/>.</param>
        /// <param name="operation">Value for <see cref="Operation"/>.</param>
        /// <typeparam name="TSourceMessage">Source message type.</typeparam>
        /// <returns>Created signal.</returns>
        public static QueryModelChangedSignal CreateFromSource<TSourceMessage>(
            TSourceMessage message,
            string queryModelId,
            Type queryModelType,
            QueryModelChangeOperation operation)
            where TSourceMessage : IMessage
        {
            return CreateFromSource(message, queryModelId, 0, queryModelType, operation);
        }

        /// <summary>
        /// Creates signal from integration event.
        /// </summary>
        /// <param name="message">Source message.</param>
        /// <param name="queryModelId">Value for <see cref="QueryModelId"/>.</param>
        /// <param name="queryModelVersion">Value for <see cref="QueryModelVersion"/>.</param>
        /// <param name="queryModelType">Value for <see cref="QueryModelType"/>.</param>
        /// <param name="operation">Value for <see cref="Operation"/>.</param>
        /// <typeparam name="TSourceMessage">Source message type.</typeparam>
        /// <returns>Created signal.</returns>
        public static QueryModelChangedSignal CreateFromSource<TSourceMessage>(
            TSourceMessage message,
            string queryModelId,
            int queryModelVersion,
            Type queryModelType,
            QueryModelChangeOperation operation)
            where TSourceMessage : IMessage
        {
            var signal = new QueryModelChangedSignal(queryModelId, queryModelVersion, queryModelType, operation)
            {
                CorrelationId = message.CorrelationId,
                AggregateRootId = message.AggregateRootId,
                Actor = message.Actor
            };

            message.CopyMetadata(signal);

            return signal;
        }
    }
}