using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.EventBus.AzureServiceBus.Extensions;
using Microsoft.Azure.ServiceBus;

namespace Intellias.CQRS.EventBus.AzureServiceBus
{
    /// <inheritdoc />
    /// <summary>
    /// Publishing events to Azure Service Bus
    /// </summary>
    public class AzureServiceTopicEventBus : IEventBus
    {
        private readonly ITopicClient topicClient;
        
        /// <summary>
        /// Creates an instance of EventBus
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="topic"></param>
        public AzureServiceTopicEventBus(string connectionString, string topic)
        {
            topicClient = new TopicClient(connectionString, topic);
        }

        /// <inheritdoc />
        public async Task<IEventResult> PublishAsync(IEvent msg)
        {
            await topicClient.SendAsync(msg.ToBusMessage()).ConfigureAwait(false);
            return await Task.FromResult(EventResult.Success).ConfigureAwait(false);
        }
    }
}