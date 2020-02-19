using System;
using System.Threading;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Results;
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
            var sendMethodInfo = mediator.GetType().GetMethod(nameof(IMediator.Send));
            if (sendMethodInfo == null)
            {
                throw new InvalidOperationException($"Unable to resolve '{nameof(IMediator.Send)}' from '{mediator.GetType()}'.");
            }

            try
            {
                var requestType = typeof(CommandRequest<>).MakeGenericType(command.GetType());
                var request = Activator.CreateInstance(requestType, command);

                var sendGenericMethodInfo = sendMethodInfo.MakeGenericMethod(typeof(IExecutionResult));
                var result = (Task<IExecutionResult>)sendGenericMethodInfo.Invoke(mediator, new[] { request, CancellationToken.None });

                await result;
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
            var sendMethodInfo = mediator.GetType().GetMethod(nameof(IMediator.Publish), new[] { typeof(INotification), typeof(CancellationToken) });
            if (sendMethodInfo == null)
            {
                throw new InvalidOperationException($"Unable to resolve '{nameof(IMediator.Publish)}' from '{mediator.GetType()}'.");
            }

            try
            {
                var notificationType = typeof(IntegrationEventNotification<>).MakeGenericType(@event.GetType());
                var notification = Activator.CreateInstance(notificationType, @event);
                var result = sendMethodInfo.Invoke(mediator, new[] { notification, CancellationToken.None })
                    ?? throw new NullReferenceException($"Invocation result of '{sendMethodInfo}' can't be null.");

                await (Task)result;
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