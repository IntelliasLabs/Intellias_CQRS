using System.Threading.Tasks;
using Intellias.CQRS.CommandBus.AzureServiceBus.Extensions;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Results;
using Microsoft.Azure.ServiceBus;

namespace Intellias.CQRS.CommandBus.AzureServiceBus
{
    /// <inheritdoc />
    /// <summary>
    /// Publishing events to Azure Service Bus
    /// </summary>
    public class AzureServiceTopicCommandBus : ICommandBus
    {
        private readonly ITopicClient topicClient;
        private readonly ICommandStore commandStore;

        /// <summary>
        /// Creates an instance of EventBus
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="topic"></param>
        /// <param name="commandStore"></param>
        public AzureServiceTopicCommandBus(
            string connectionString,
            string topic,
            ICommandStore commandStore)
        {
            this.commandStore = commandStore;
            topicClient = new TopicClient(connectionString, topic);
        }

        /// <inheritdoc />
        public async Task<IExecutionResult> PublishAsync(ICommand msg)
        {
            await commandStore.SaveAsync(msg);
            await topicClient.SendAsync(msg.ToBusMessage());
            return await Task.FromResult(ExecutionResult.Successful);
        }
    }
}