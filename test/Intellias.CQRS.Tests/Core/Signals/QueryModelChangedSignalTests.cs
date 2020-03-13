using FluentAssertions;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Signals;
using Intellias.CQRS.Tests.Core.Infrastructure.AssertionRules;
using Intellias.CQRS.Tests.Utils;
using Newtonsoft.Json;
using Xunit;

namespace Intellias.CQRS.Tests.Core.Signals
{
    public class QueryModelChangedSignalTests
    {
        [Fact]
        public void QueryModelChangedSignal_Always_SerializedCorrectly()
        {
            var signal = new QueryModelChangedSignal(Unified.NewCode(), FixtureUtils.Int(), typeof(int), FixtureUtils.FromEnum<QueryModelChangeOperation>())
            {
                CorrelationId = Unified.NewCode(),
                AggregateRootId = Unified.NewCode(),
                Metadata = { [MetadataKey.UserId] = Unified.NewCode() },
                Actor = { IdentityId = Unified.NewCode(), UserId = Unified.NewCode(), IsProcessManager = true }
            };

            var json = JsonConvert.SerializeObject(signal);
            var deserialized = JsonConvert.DeserializeObject<QueryModelChangedSignal>(json);

            deserialized.Should().BeEquivalentTo(signal);
        }

        [Fact]
        public void Constructor_Always_UpdatesProperties()
        {
            var queryModelId = Unified.NewCode();
            var queryModelVersion = FixtureUtils.Int();
            var queryModelType = typeof(int);
            var operation = FixtureUtils.FromEnum<QueryModelChangeOperation>();

            var signal = new QueryModelChangedSignal(queryModelId, queryModelVersion, queryModelType, operation);

            signal.QueryModelId.Should().Be(queryModelId);
            signal.QueryModelVersion.Should().Be(queryModelVersion);
            signal.QueryModelType.Should().Be(queryModelType);
            signal.Operation.Should().Be(operation);
        }

        [Fact]
        public void CreateFromSource1_Always_CopiesMessageCorrelationData()
        {
            var integrationEvent = Fixtures.Pipelines.FakeCreatedIntegrationEvent();

            var signal = QueryModelChangedSignal.CreateFromSource(
                integrationEvent,
                Unified.NewCode(),
                FixtureUtils.Int(),
                typeof(int),
                FixtureUtils.FromEnum<QueryModelChangeOperation>());

            signal.Should().BeEquivalentTo(integrationEvent, options => options.ForMessage());
        }

        [Fact]
        public void CreateFromSource2_Always_CopiesMessageCorrelationData()
        {
            var integrationEvent = Fixtures.Pipelines.FakeCreatedIntegrationEvent();

            var signal = QueryModelChangedSignal.CreateFromSource(
                integrationEvent,
                Unified.NewCode(),
                typeof(int),
                FixtureUtils.FromEnum<QueryModelChangeOperation>());

            signal.Should().BeEquivalentTo(integrationEvent, options => options.ForMessage());
        }
    }
}