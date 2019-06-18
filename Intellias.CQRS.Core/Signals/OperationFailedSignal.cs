using System;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Signals
{
    /// <summary>
    /// Operation failed event
    /// </summary>
    public class OperationFailedSignal : AbstractMessage
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="source"></param>
        /// <param name="error"></param>
        public OperationFailedSignal(IMessage source, string error)
        {
            Source = source ?? throw new ArgumentNullException($"Source in the OperationFailedSignal should be set");
            Error = error;

            Id = Unified.NewCode();
            AggregateRootId = source.AggregateRootId;
            CorrelationId = source.CorrelationId;

            source.CopyMetadata(this);
        }

        /// <summary>
        /// Source
        /// </summary>
        public IMessage Source { get; private set; }

        /// <summary>
        /// Failed reason
        /// </summary>
        public string Error { get; private set; }

    }
}
