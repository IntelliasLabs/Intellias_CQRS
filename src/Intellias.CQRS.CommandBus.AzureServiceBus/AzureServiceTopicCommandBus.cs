using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Intellias.CQRS.CommandBus.AzureServiceBus.Extensions;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Results;
using Microsoft.Azure.ServiceBus;

namespace Intellias.CQRS.CommandBus.AzureServiceBus
{
    /// <inheritdoc />
    /// <summary>
    /// Publishing events to Azure Service Bus.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AzureServiceTopicCommandBus : ICommandBus
    {
        private readonly ITopicClient topicClient;
        private readonly ICommandStore commandStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureServiceTopicCommandBus"/> class.
        /// </summary>
        /// <param name="connectionString">Connection String.</param>
        /// <param name="topic">Azure Topic.</param>
        /// <param name="commandStore">Command Store.</param>
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
            return await Task.FromResult(new SuccessfulResult());
        }
    }
}