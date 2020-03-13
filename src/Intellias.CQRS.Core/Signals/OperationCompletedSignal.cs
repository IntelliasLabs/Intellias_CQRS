using System;
using System.Diagnostics.CodeAnalysis;
using Intellias.CQRS.Core.Messages;
using Newtonsoft.Json;

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
            Actor = source.Actor;

            source.CopyMetadata(this);
        }

        /// <summary>
        /// Source.
        /// </summary>
        [JsonProperty(TypeNameHandling = TypeNameHandling.Auto)]
        [SuppressMessage("Security", "SCS0028:TypeNameHandling is set to other value than 'None' that may lead to deserialization vulnerability", Justification = "Temporary")]
        public IMessage Source { get; private set; }
    }
}
