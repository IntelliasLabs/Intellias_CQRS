using Intellias.CQRS.Core;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Queries.Immutable;
using Intellias.CQRS.Core.Queries.Mutable;
using Intellias.CQRS.Core.Signals;
using MediatR;

namespace Intellias.CQRS.Pipelines.EventHandlers.Notifications
{
    /// <summary>
    /// Integration event execution result.
    /// </summary>
    public class QueryModelUpdatedNotification : INotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryModelUpdatedNotification"/> class.
        /// </summary>
        /// <param name="signal">Query model changed signal. </param>
        public QueryModelUpdatedNotification(QueryModelUpdatedSignal signal)
        {
            Signal = signal;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryModelUpdatedNotification"/> class.
        /// </summary>
        /// <param name="event">Event that updated query model.</param>
        /// <param name="queryModel">Update query model.</param>
        public QueryModelUpdatedNotification(IEvent @event, IImmutableQueryModel queryModel)
        {
            Signal = new QueryModelUpdatedSignal(queryModel.Id, queryModel.Version, queryModel.GetType())
            {
                CorrelationId = @event.CorrelationId
            };

            @event.CopyMetadata(Signal);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryModelUpdatedNotification"/> class.
        /// </summary>
        /// <param name="event">Event that updated query model.</param>
        /// <param name="queryModel">Update query model.</param>
        public QueryModelUpdatedNotification(IEvent @event, IMutableQueryModel queryModel)
        {
            Signal = new QueryModelUpdatedSignal(queryModel.Id, 0, queryModel.GetType())
            {
                CorrelationId = @event.CorrelationId
            };

            @event.CopyMetadata(Signal);
        }

        /// <summary>
        /// Signal.
        /// </summary>
        public QueryModelUpdatedSignal Signal { get; }

        /// <summary>
        /// Indicates whether notification source is replay.
        /// </summary>
        public bool IsReplay { get; set; }

        /// <summary>
        /// Indicates whether notification is private, thus shouldn't be published.
        /// </summary>
        public bool IsPrivate { get; set; }
    }
}