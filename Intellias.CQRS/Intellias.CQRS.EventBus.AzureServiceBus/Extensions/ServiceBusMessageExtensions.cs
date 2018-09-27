using System.Text;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace Intellias.CQRS.EventBus.AzureServiceBus.Extensions
{
    internal static class ServiceBusMessageExtensions
    {
        public static Message ToBusMessage(this IEvent @event)=>
            new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)))
            {
                MessageId = Unified.NewCode(),
                PartitionKey = @event.AggregateRootId
            };
    }
}
