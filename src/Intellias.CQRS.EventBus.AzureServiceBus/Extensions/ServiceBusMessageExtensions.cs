using System;
using System.Text;
using Intellias.CQRS.Core;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Microsoft.Azure.ServiceBus;

namespace Intellias.CQRS.EventBus.AzureServiceBus.Extensions
{
    /// <summary>
    /// Service bus extensions.
    /// </summary>
    public static class ServiceBusMessageExtensions
    {
        /// <summary>
        /// Event to service-bus message.
        /// </summary>
        /// <param name="event">event.</param>
        /// <returns>Message.</returns>
        public static Message ToBusMessage(this IEvent @event) =>
            new Message(Encoding.UTF8.GetBytes(@event.ToJson()))
            {
                MessageId = @event.Id,
                ContentType = @event.GetType().AssemblyQualifiedName,
                PartitionKey = @event.AggregateRootId,
                CorrelationId = @event.CorrelationId,
                SessionId = AbstractMessage.GlobalSessionId,
                Label = @event.GetType().Name
            };

        /// <summary>
        /// GetMessage.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <returns>IMessage.</returns>
        public static IMessage GetMessage(this Message message)
        {
            var messageType = Type.GetType(message.ContentType);
            var json = Encoding.UTF8.GetString(message.Body);
            return (IMessage)json.FromJson(messageType);
        }

        /// <summary>
        /// GetCommand.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <returns>ICommand.</returns>
        public static ICommand GetCommand(this Message message)
        {
            return (ICommand)GetMessage(message);
        }

        /// <summary>
        /// GetEvent.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <returns>IEvent.</returns>
        public static IEvent GetEvent(this Message message)
        {
            return (IEvent)GetMessage(message);
        }
    }
}
