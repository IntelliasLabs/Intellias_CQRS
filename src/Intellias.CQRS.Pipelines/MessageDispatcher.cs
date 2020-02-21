using System;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Pipelines.CommandHandlers;
using Intellias.CQRS.Pipelines.EventHandlers.Notifications;
using MediatR;

namespace Intellias.CQRS.Pipelines
{
    /// <summary>
    /// Message dispatcher.
    /// </summary>
    public class MessageDispatcher : IMessageDispatcher
    {
        private readonly IMediator mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDispatcher"/> class.
        /// </summary>
        /// <param name="mediator">MediatR.</param>
        public MessageDispatcher(IMediator mediator)
        {
            this.mediator = mediator;
        }

        /// <inheritdoc/>
        public async Task DispatchCommandAsync(ICommand command)
        {
            try
            {
                var requestType = typeof(CommandRequest<>).MakeGenericType(command.GetType());
                var request = Activator.CreateInstance(requestType, command);
                await mediator.Send(request);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException(
                    $"Unhandled error occured during dispatching message with id '{command.Id}' of type '{command.GetType()}'.",
                    exception);
            }
        }

        /// <inheritdoc/>
        public async Task DispatchEventAsync(IEvent @event)
        {
            try
            {
                var notificationType = typeof(IntegrationEventNotification<>).MakeGenericType(@event.GetType());
                var notification = Activator.CreateInstance(notificationType, @event);
                await mediator.Publish(notification);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException(
                    $"Unhandled error occured during dispatching message with id '{@event.Id}' of type '{@event.GetType()}'.",
                    exception);
            }
        }
    }
}