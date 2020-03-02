using Intellias.CQRS.Core.Events;
using MediatR;

namespace Intellias.CQRS.Pipelines.EventHandlers.Notifications
{
    /// <summary>
    /// Integration event notification.
    /// </summary>
    /// <typeparam name="TIntegrationEvent">Event.</typeparam>
    public class IntegrationEventNotification<TIntegrationEvent> : INotification
        where TIntegrationEvent : IIntegrationEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrationEventNotification{TIntegrationEvent}"/> class.
        /// </summary>
        /// <param name="event">Event.</param>
        public IntegrationEventNotification(TIntegrationEvent @event)
        {
            IntegrationEvent = @event;
        }

        /// <summary>
        /// Event.
        /// </summary>
        public TIntegrationEvent IntegrationEvent { get; }
    }
}