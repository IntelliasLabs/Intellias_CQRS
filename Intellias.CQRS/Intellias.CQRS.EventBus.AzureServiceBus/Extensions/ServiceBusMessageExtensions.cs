using System.Text;
using Intellias.CQRS.Core.Config;
using Intellias.CQRS.Core.Events;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace Intellias.CQRS.EventBus.AzureServiceBus.Extensions
{
    /// <summary>
    /// ServiceBusMessageExtensions
    /// </summary>
    internal static class ServiceBusMessageExtensions
    {
        public static Message ToBusMessage(this IEvent @event) => 
            new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event, CqrsSettings.JsonConfig())))
            {
                MessageId = @event.Id,
                ContentType = @event.GetType().FullName,
                PartitionKey = @event.AggregateRootId
            };
    }
}
