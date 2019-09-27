using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Pipelines.Transactions;
using MediatR;

namespace Intellias.CQRS.Pipelines.CommandHandlers
{
    /// <summary>
    /// Pipeline command request.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command.</typeparam>
    public class CommandRequest<TCommand> : IRequest<IExecutionResult>
        where TCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandRequest{TCommand}"/> class.
        /// </summary>
        /// <param name="command">Value for <see cref="Command"/>.</param>
        public CommandRequest(TCommand command)
        {
            Command = command;
            Context = new AggregateExecutionContext(command);
            TransactionScope = new NullTransactionScope();
        }

        /// <summary>
        /// Executing command.
        /// </summary>
        public TCommand Command { get; }

        /// <summary>
        /// Aggregate execution context.
        /// </summary>
        public AggregateExecutionContext Context { get; }

        /// <summary>
        /// Scope of subdomain transaction.
        /// </summary>
        public ITransactionScope TransactionScope { get; private set; }

        /// <summary>
        /// Deconstructs request.
        /// </summary>
        /// <param name="command">Value of <see cref="Command"/>.</param>
        /// <param name="context">Value of <see cref="Context"/>.</param>
        /// <param name="transactionScope">Value of <see cref="TransactionScope"/>.</param>
        public void Deconstruct(out TCommand command, out AggregateExecutionContext context, out ITransactionScope transactionScope)
        {
            command = Command;
            context = Context;
            transactionScope = TransactionScope;
        }

        /// <summary>
        /// Starts transaction.
        /// </summary>
        /// <param name="transactionStore">Transaction store.</param>
        /// <param name="aggregateStore">Aggregate store.</param>
        /// <param name="integrationEventStore">Integration events store.</param>
        /// <returns>Scope of the transaction.</returns>
        public ITransactionScope CreateTransactionalScope(
            ITransactionStore transactionStore,
            IAggregateStore aggregateStore,
            IIntegrationEventStore integrationEventStore)
        {
            return CreateTransactionalScope(new TransactionScope(transactionStore, aggregateStore, integrationEventStore));
        }

        /// <summary>
        /// Starts transaction with scope that is controlled outside.
        /// </summary>
        /// <param name="transactionScope">Outer transaction scope.</param>
        /// <returns>Scope of the transaction.</returns>
        public ITransactionScope CreateTransactionalScope(ITransactionScope transactionScope)
        {
            if (TransactionScope is NullTransactionScope)
            {
                TransactionScope = transactionScope;
            }

            return TransactionScope;
        }
    }
}