using System.Text;
using Intellias.CQRS.Core.Events;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace Intellias.CQRS.EventBus.AzureServiceBus.Extensions
{
    internal static class ServiceBusMessageExtensions
    {
        public static Message ToBusMessage(this IEvent @event)=>
            new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)))
            {
                MessageId = @event.Id,
                PartitionKey = @event.AggregateRootId
            };
    }
}
