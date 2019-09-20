using System;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Signals
{
    /// <summary>
    /// Signal that notifies that query model is updated.
    /// </summary>
    public class QueryModelUpdatedSignal : AbstractMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryModelUpdatedSignal"/> class.
        /// </summary>
        /// <param name="queryModelId">Value for <see cref="QueryModelId"/>.</param>
        /// <param name="queryModelVersion">Value for <see cref="QueryModelVersion"/>.</param>
        /// <param name="queryModelType">Value for <see cref="QueryModelType"/>.</param>
        public QueryModelUpdatedSignal(string queryModelId, int queryModelVersion, Type queryModelType)
        {
            QueryModelId = queryModelId;
            QueryModelVersion = queryModelVersion;
            QueryModelType = queryModelType;
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
    }
}