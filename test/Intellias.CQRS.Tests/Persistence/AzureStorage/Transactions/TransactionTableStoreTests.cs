using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Persistence.AzureStorage.Common;
using Intellias.CQRS.Persistence.AzureStorage.Transactions;
using Intellias.CQRS.Tests.Core.Infrastructure.AssertionRules;
using Intellias.CQRS.Tests.Utils;
using Intellias.CQRS.Tests.Utils.Pipelines.Fakes;
using Intellias.CQRS.Tests.Utils.StorageAccount;
using Xunit;

namespace Intellias.CQRS.Tests.Persistence.AzureStorage.Transactions
{
    public class TransactionTableStoreTests : StorageAccountTestBase
    {
        private readonly TransactionTableStore store;

        public TransactionTableStoreTests(StorageAccountFixture fixture)
        {
            var options = new DefaultTableStorageOptions
            {
                TableNamePrefix = fixture.ExecutionContext.GetUniqueStorageTablePrefix(),
                ConnectionString = fixture.Configuration.StorageAccount.ConnectionString
            };

            store = new TransactionTableStore(options);
        }

        [Fact]
        public async Task PrepareAsync_Always_CreatesTransactionUncommitted()
        {
            var aggregateRoot = SetupAggregateRootWithEvents();
            var record = CreateTransactionRecord(aggregateRoot);

            await store.PrepareAsync(record.TransactionId, new[] { aggregateRoot }, record.IntegrationEvent);
            var savedTransaction = (await store.GetAllAsync()).First(r => r.TransactionId == record.TransactionId);

            savedTransaction.IntegrationEvent.Should().BeEquivalentTo(record.IntegrationEvent, options => options.ForMessage());
            savedTransaction.StateEvents.Should().BeEquivalentTo(record.StateEvents, options => options.ForMessage());
            savedTransaction.IsCommitted.Should().BeFalse();
        }

        [Fact]
        public async Task CommitAsync_Always_CommitsTransaction()
        {
            var aggregateRoot = SetupAggregateRootWithEvents();
            var record = CreateTransactionRecord(aggregateRoot);

            await store.PrepareAsync(record.TransactionId, new[] { aggregateRoot }, record.IntegrationEvent);
            await store.CommitAsync(record.TransactionId);

            var savedTransaction = (await store.GetAllAsync()).First(r => r.TransactionId == record.TransactionId);

            savedTransaction.IsCommitted.Should().BeTrue();
        }

        [Fact]
        public async Task GetAllAsync_Always_ReturnsAllTransactions()
        {
            var aggregateRoot1 = SetupAggregateRootWithEvents();
            var record1 = CreateTransactionRecord(aggregateRoot1);
            var aggregateRoot2 = SetupAggregateRootWithEvents();
            var record2 = CreateTransactionRecord(aggregateRoot2);

            await store.PrepareAsync(record1.TransactionId, new[] { aggregateRoot1 }, record1.IntegrationEvent);
            await store.PrepareAsync(record2.TransactionId, new[] { aggregateRoot2 }, record2.IntegrationEvent);

            var savedTransactions = (await store.GetAllAsync()).Select(r => r.TransactionId).ToArray();

            savedTransactions.Should().Contain(record1.TransactionId);
            savedTransactions.Should().Contain(record2.TransactionId);
        }

        private TransactionRecord CreateTransactionRecord(params FakeAggregateRoot[] aggregateRoots)
        {
            return new TransactionRecord
            {
                TransactionId = Unified.NewCode(),
                IntegrationEvent = Fixtures.Pipelines.FakeCreatedIntegrationEvent(),
                StateEvents = aggregateRoots.SelectMany(ar => ar.Events).ToList(),
                IsCommitted = false
            };
        }

        private FakeAggregateRoot SetupAggregateRootWithEvents()
        {
            var id = Unified.NewCode();
            var executionContext = new AggregateExecutionContext(Fixtures.Pipelines.FakeCreateCommand());
            var aggregateRoot = new FakeAggregateRoot(id, executionContext);
            aggregateRoot.Create(FixtureUtils.String());
            aggregateRoot.Update(FixtureUtils.String());

            return aggregateRoot;
        }
    }
}