using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Pipelines.CommandHandlers;
using Intellias.CQRS.Pipelines.EventHandlers.Notifications;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        /// <inheritdoc />
        public async Task DispatchCommandAsync(string message)
        {
            var messageObject = DeserializeMessage(message);
            await DispatchCommandAsync((ICommand)messageObject);
        }

        /// <inheritdoc />
        public async Task DispatchEventAsync(string message)
        {
            var messageObject = DeserializeMessage(message);
            await DispatchEventAsync((IEvent)messageObject);
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

        private static IMessage DeserializeMessage(string message)
        {
            using (var reader = new JsonTextReader(new StringReader(message)))
            {
                var messageJObject = JObject.Load(reader);
                var messageTypeName = messageJObject.TryGetValue(nameof(IMessage.TypeName), StringComparison.OrdinalIgnoreCase, out var value)
                    ? value.Value<string>()
                    : null;

                if (string.IsNullOrWhiteSpace(messageTypeName))
                {
                    throw new ArgumentException($"Message type name at json path '{messageTypeName}' is not found in json '{message}'.");
                }

                var messageType = Type.GetType(messageTypeName);

                if (!(messageJObject.ToObject(messageType) is IMessage messageObject))
                {
                    throw new ArgumentException($"Expected message of type '{typeof(IMessage)}' but was '${messageType}'.");
                }

                return messageObject;
            }
        }
    }
}