using System;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Results;

namespace Intellias.CQRS.Core.Signals
{
    /// <summary>
    /// Operation failed event.
    /// </summary>
    public class OperationFailedSignal : AbstractMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationFailedSignal"/> class.
        /// </summary>
        /// <param name="source">Source.</param>
        /// <param name="error">Error.</param>
        public OperationFailedSignal(IMessage source, FailedResult error)
        {
            Source = source ?? throw new ArgumentNullException($"Source in the OperationFailedSignal should be set");
            Error = error ?? throw new ArgumentNullException($"FailedResult in the OperationFailedSignal should be set");

            Id = Unified.NewCode();
            AggregateRootId = source.AggregateRootId;
            CorrelationId = source.CorrelationId;

            source.CopyMetadata(this);
        }

        /// <summary>
        /// Source.
        /// </summary>
        public IMessage Source { get; private set; }

        /// <summary>
        /// Failed reason.
        /// </summary>
        public FailedResult Error { get; private set; }
    }
}
