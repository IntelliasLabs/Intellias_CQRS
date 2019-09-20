using FluentAssertions;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Signals;
using Intellias.CQRS.Tests.Utils;
using Newtonsoft.Json;
using Xunit;

namespace Intellias.CQRS.Tests.Core.Signals
{
    public class QueryModelUpdatedSignalTests
    {
        [Fact]
        public void Constructor_Always_UpdatesProperties()
        {
            var queryModelId = Unified.NewCode();
            var queryModelVersion = FixtureUtils.Int();
            var queryModelType = typeof(int);

            var signal = new QueryModelUpdatedSignal(queryModelId, queryModelVersion, queryModelType);

            signal.QueryModelId.Should().Be(queryModelId);
            signal.QueryModelVersion.Should().Be(queryModelVersion);
            signal.QueryModelType.Should().Be(queryModelType);
        }

        [Fact]
        public void QueryModelUpdatedSignal_Always_SerializedCorrectly()
        {
            var signal = new QueryModelUpdatedSignal(Unified.NewCode(), FixtureUtils.Int(), typeof(int));

            var json = JsonConvert.SerializeObject(signal);
            var deserialized = JsonConvert.DeserializeObject<QueryModelUpdatedSignal>(json);

            deserialized.Should().BeEquivalentTo(signal);
        }
    }
}