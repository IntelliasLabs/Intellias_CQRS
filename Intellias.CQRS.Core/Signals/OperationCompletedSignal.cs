using System;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Signals
{
    /// <summary>
    /// Operation failed event.
    /// </summary>
    public class OperationCompletedSignal : AbstractMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationCompletedSignal"/> class.
        /// </summary>
        /// <param name="source">Message.</param>
        public OperationCompletedSignal(IMessage source)
        {
            Source = source ?? throw new ArgumentNullException($"Source in the OperationCompletedSignal should be set");

            Id = Unified.NewCode();
            AggregateRootId = source.AggregateRootId;
            CorrelationId = source.CorrelationId;

            source.CopyMetadata(this);
        }

        /// <summary>
        /// Source.
        /// </summary>
        public IMessage Source { get; private set; }
    }
}
