using System;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Domain
{
    /// <summary>
    /// Base class for aggregates.
    /// </summary>
    /// <typeparam name="TState">Type of the aggregate state.</typeparam>
    public abstract class BaseAggregateRoot<TState> : AggregateRoot<TState>
        where TState : AggregateState, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAggregateRoot{TState}"/> class.
        /// </summary>
        /// <param name="id">Aggregate id.</param>
        /// <param name="context">Aggregate execution context.</param>
        protected BaseAggregateRoot(string id, AggregateExecutionContext context)
            : base(id)
        {
            Context = context;
        }

        /// <summary>
        /// Aggregate snapshot id.
        /// </summary>
        public SnapshotId SnapshotId => new SnapshotId { EntryId = Id, EntryVersion = Version };

        /// <summary>
        /// Aggregate execution context.
        /// </summary>
        protected AggregateExecutionContext Context { get; }

        /// <summary>
        /// Publishes state event.
        /// </summary>
        /// <param name="setup">Configures state event.</param>
        /// <typeparam name="TEvent">Type of the state event.</typeparam>
        protected void PublishEvent<TEvent>(Action<TEvent> setup)
            where TEvent : Event, new()
        {
            var @event = new TEvent
            {
                Id = Unified.NewCode(),
                AggregateRootId = Id,
                CorrelationId = Context.CorrelationId,
                Principal = Context.Principal,
                Version = Version,
                SourceId = Context.SourceId
            };

            foreach (var pair in Context.Metadata)
            {
                @event.Metadata[pair.Key] = pair.Value;
            }

            setup(@event);

            PublishEvent(@event);
        }
    }
}