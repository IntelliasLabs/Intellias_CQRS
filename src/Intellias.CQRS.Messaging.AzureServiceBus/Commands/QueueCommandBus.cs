using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;
using Intellias.CQRS.Core;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Results;
using Microsoft.Azure.ServiceBus;

namespace Intellias.CQRS.Messaging.AzureServiceBus.Commands
{
    /// <summary>
    /// Command bus implementation over Azure Service Bus Queue.
    /// </summary>
    /// <typeparam name="TCommandBusOptions">Command bus options.</typeparam>
    [ExcludeFromCodeCoverage]
    public class QueueCommandBus<TCommandBusOptions> : ICommandBus<TCommandBusOptions>
        where TCommandBusOptions : ICommandBusOptions
    {
        private readonly ICommandStore commandStore;
        private readonly TCommandBusOptions options;
        private readonly QueueClient queueClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueCommandBus{TCommandBusOptions}"/> class.
        /// </summary>
        /// <param name="commandStore">Commands store.</param>
        /// <param name="options">Command bus options.</param>
        public QueueCommandBus(ICommandStore commandStore, TCommandBusOptions options)
        {
            this.commandStore = commandStore;
            this.options = options;

            queueClient = new QueueClient(options.ConnectionString, options.Name);
        }

        /// <inheritdoc />
        public async Task<IExecutionResult> PublishAsync(ICommand msg)
        {
            await commandStore.SaveAsync(msg);
            await queueClient.SendAsync(ConvertToMessage(msg));

            return new SuccessfulResult();
        }

        private Message ConvertToMessage(ICommand command)
        {
            return new Message(Encoding.UTF8.GetBytes(command.ToJson()))
            {
                MessageId = command.Id,
                ContentType = command.GetType().AssemblyQualifiedName,
                PartitionKey = command.AggregateRootId,
                CorrelationId = command.CorrelationId,
                SessionId = options.GetPartition(command),
                Label = command.GetType().Name
            };
        }
    }
}