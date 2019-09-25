using System;
using FluentAssertions;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Pipelines.Transactions;
using Intellias.CQRS.Tests.Utils;
using Intellias.CQRS.Tests.Utils.Pipelines.Fakes;
using Xunit;

namespace Intellias.CQRS.Tests.Pipelines.Transactions
{
    public class NullTransactionScopeTests
    {
        private readonly NullTransactionScope scope = new NullTransactionScope();

        [Fact]
        public void FindAggregateAsync_Always_Throws()
        {
            var context = new AggregateExecutionContext(Fixtures.Pipelines.FakeCreateCommand());

            scope.Awaiting(s => s.FindAggregateAsync<FakeAggregateRoot, FakeAggregateState>(Unified.NewCode(), context))
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void CreateAggregate_Always_Throws()
        {
            var context = new AggregateExecutionContext(Fixtures.Pipelines.FakeCreateCommand());

            scope.Invoking(s => s.CreateAggregate<FakeAggregateRoot, FakeAggregateState>(Unified.NewCode(), context))
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void CommitAsync_Always_Throws()
        {
            scope.Awaiting(s => s.CommitAsync(Unified.NewCode(), Fixtures.Pipelines.FakeCreatedIntegrationEvent()))
                .Should().Throw<InvalidOperationException>();
        }
    }
}