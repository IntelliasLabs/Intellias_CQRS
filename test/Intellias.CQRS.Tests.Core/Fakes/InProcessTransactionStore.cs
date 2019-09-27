using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Pipelines.Transactions;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    public class InProcessTransactionStore : ITransactionStore
    {
        private readonly Dictionary<string, TransactionEntry> store = new Dictionary<string, TransactionEntry>();

        public Task PrepareAsync(string transactionId, IReadOnlyCollection<IAggregateRoot> aggregateRoots, IIntegrationEvent integrationEvent)
        {
            store[transactionId] = new TransactionEntry(transactionId, aggregateRoots, integrationEvent);
            return Task.CompletedTask;
        }

        public Task CommitAsync(string transactionId)
        {
            store[transactionId].IsCommited = true;
            return Task.CompletedTask;
        }

        private class TransactionEntry
        {
            public TransactionEntry(string transactionId, IReadOnlyCollection<IAggregateRoot> aggregateRoots, IIntegrationEvent integrationEvent)
            {
                TransactionId = transactionId;
                AggregateRoots = aggregateRoots;
                IntegrationEvent = integrationEvent;
            }

            public string TransactionId { get; }

            public IReadOnlyCollection<IAggregateRoot> AggregateRoots { get; }

            public IIntegrationEvent IntegrationEvent { get; }

            public bool IsCommited { get; set; }
        }
    }
}