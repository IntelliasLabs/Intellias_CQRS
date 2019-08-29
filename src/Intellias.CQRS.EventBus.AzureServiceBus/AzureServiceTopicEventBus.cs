using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.EventBus.AzureServiceBus.Extensions;
using Microsoft.Azure.ServiceBus;

namespace Intellias.CQRS.EventBus.AzureServiceBus
{
    /// <summary>
    /// Publishing events to Azure Service Bus.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AzureServiceTopicEventBus : IEventBus
    {
        private readonly ITopicClient topicClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureServiceTopicEventBus"/> class.
        /// </summary>
        /// <param name="connectionString">Connection String.</param>
        /// <param name="topic">Azure Topic.</param>
        public AzureServiceTopicEventBus(string connectionString, string topic)
        {
            topicClient = new TopicClient(connectionString, topic);
        }

        /// <inheritdoc />
        public async Task<IExecutionResult> PublishAsync(IEvent msg)
        {
            await topicClient.SendAsync(msg.ToBusMessage());
            return await Task.FromResult(new SuccessfulResult());
        }
    }
}