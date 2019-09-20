using System;
using System.Collections.Generic;
using System.Linq;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Domain
{
    /// <summary>
    /// Aggregate execution context.
    /// </summary>
    public class AggregateExecutionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateExecutionContext"/> class.
        /// </summary>
        /// <param name="command">Command which caused aggregate mutation.</param>
        public AggregateExecutionContext(Command command)
        {
            AggregateRootId = command.AggregateRootId;
            CorrelationId = command.CorrelationId;
            Metadata = command.Metadata.ToDictionary(k => k.Key, v => v.Value);
            ExpectedVersion = command.ExpectedVersion;
            SourceId = command.Id;
            UserId = Guid.Parse(Metadata[MetadataKey.UserId]);
        }

        /// <summary>
        /// Id of the affected aggregate.
        /// </summary>
        public string AggregateRootId { get; }

        /// <summary>
        /// Operation correlation id.
        /// </summary>
        public string CorrelationId { get; }

        /// <summary>
        /// Command metadata.
        /// </summary>
        public IReadOnlyDictionary<MetadataKey, string> Metadata { get; }

        /// <summary>
        /// Expected version of the aggregate.
        /// </summary>
        public int ExpectedVersion { get; }

        /// <summary>
        /// Command id.
        /// </summary>
        public string SourceId { get; }

        /// <summary>
        /// Id of the user who executes command.
        /// </summary>
        public Guid UserId { get; }

        /// <summary>
        /// Creates integration event from command context.
        /// </summary>
        /// <typeparam name="TIntegrationEvent">Type of the integration event.</typeparam>
        /// <returns>Integration event.</returns>
        public TIntegrationEvent CreateIntegrationEvent<TIntegrationEvent>()
            where TIntegrationEvent : IntegrationEvent, new()
        {
            var @event = new TIntegrationEvent
            {
                Id = Unified.NewCode(),
                AggregateRootId = AggregateRootId,
                CorrelationId = CorrelationId,
                Version = ExpectedVersion,
                SourceId = SourceId
            };

            foreach (var pair in Metadata)
            {
                @event.Metadata[pair.Key] = pair.Value;
            }

            return @event;
        }
    }
}