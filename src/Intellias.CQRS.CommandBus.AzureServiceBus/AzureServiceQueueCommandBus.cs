using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Intellias.CQRS.CommandBus.AzureServiceBus.Extensions;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Results;
using Microsoft.Azure.ServiceBus;

namespace Intellias.CQRS.CommandBus.AzureServiceBus
{
    /// <summary>
    /// Publishing events to Azure Service Bus Queue.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AzureServiceQueueCommandBus : ICommandBus
    {
        private readonly IQueueClient queueClient;
        private readonly ICommandStore commandStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureServiceQueueCommandBus"/> class.
        /// </summary>
        /// <param name="connectionString">Connection String.</param>
        /// <param name="queueName">Azure QueueName.</param>
        /// <param name="commandStore">Command Store.</param>
        public AzureServiceQueueCommandBus(
            string connectionString,
            string queueName,
            ICommandStore commandStore)
        {
            this.commandStore = commandStore;
            queueClient = new QueueClient(connectionString, queueName);
        }

        /// <inheritdoc />
        public async Task<IExecutionResult> PublishAsync(ICommand msg)
        {
            await commandStore.SaveAsync(msg);
            await queueClient.SendAsync(msg.ToBusMessage());
            return await Task.FromResult(new SuccessfulResult());
        }
    }
}
