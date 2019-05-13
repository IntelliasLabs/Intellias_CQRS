using System.Text;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Microsoft.Azure.ServiceBus;

namespace Intellias.CQRS.EventBus.AzureServiceBus.Extensions
{
    /// <summary>
    /// ServiceBusMessageExtensions
    /// </summary>
    internal static class ServiceBusMessageExtensions
    {
        public static Message ToBusMessage(this IEvent @event) =>
            new Message(Encoding.UTF8.GetBytes(@event.ToJson()))
            {
                MessageId = @event.Id,
                ContentType = @event.GetType().FullName,
                PartitionKey = @event.AggregateRootId
            };
    }
}
