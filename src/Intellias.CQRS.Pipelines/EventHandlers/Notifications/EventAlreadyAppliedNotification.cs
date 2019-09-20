using System;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Queries.Immutable;
using Intellias.CQRS.Core.Queries.Mutable;
using MediatR;

namespace Intellias.CQRS.Pipelines.EventHandlers.Notifications
{
    /// <summary>
    /// Integration event already applied execution result.
    /// </summary>
    public class EventAlreadyAppliedNotification : INotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventAlreadyAppliedNotification"/> class.
        /// </summary>
        /// <param name="event">Event that updated query model.</param>
        /// <param name="queryModel">Update query model.</param>
        public EventAlreadyAppliedNotification(IEvent @event, IImmutableQueryModel queryModel)
        {
            Event = @event;
            QueryModelType = queryModel.GetType();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventAlreadyAppliedNotification"/> class.
        /// </summary>
        /// <param name="event">Event that updated query model.</param>
        /// <param name="queryModel">Update query model.</param>
        public EventAlreadyAppliedNotification(IEvent @event, IMutableQueryModel queryModel)
        {
            Event = @event;
            QueryModelType = queryModel.GetType();
        }

        /// <summary>
        /// Integration event.
        /// </summary>
        public IEvent Event { get; }

        /// <summary>
        /// QueryModelType.
        /// </summary>
        public Type QueryModelType { get; }
    }
}