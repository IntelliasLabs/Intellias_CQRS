using System.Threading;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Pipelines.Transactions;
using MediatR;

namespace Intellias.CQRS.Pipelines.CommandHandlers.Behaviors
{
    /// <summary>
    /// Transactionally saves changes in subdomain.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command to be handled.</typeparam>
    public class TransactionManagerBehavior<TCommand> : IPipelineBehavior<CommandRequest<TCommand>, IExecutionResult>
        where TCommand : Command
    {
        private readonly ITransactionStore transactionStore;
        private readonly IAggregateStore aggregateStore;
        private readonly IIntegrationEventStore integrationEventStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionManagerBehavior{TCommand}"/> class.
        /// </summary>
        /// <param name="transactionStore">Transaction store.</param>
        /// <param name="aggregateStore">Aggregate store.</param>
        /// <param name="integrationEventStore">Integration events store.</param>
        public TransactionManagerBehavior(ITransactionStore transactionStore, IAggregateStore aggregateStore, IIntegrationEventStore integrationEventStore)
        {
            this.transactionStore = transactionStore;
            this.aggregateStore = aggregateStore;
            this.integrationEventStore = integrationEventStore;
        }

        /// <inheritdoc />
        public async Task<IExecutionResult> Handle(
            CommandRequest<TCommand> request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<IExecutionResult> next)
        {
            var scope = request.CreateTransactionalScope(transactionStore, aggregateStore, integrationEventStore);
            var result = await next();
            if (!result.Success)
            {
                return result;
            }

            // If there are changes in domain, then there must be a domain event,
            // therefore we have something to save.
            if (result is IntegrationEventExecutionResult deer)
            {
                await scope.CommitAsync(request.Command.Id, deer.Event);
            }

            return result;
        }
    }
}