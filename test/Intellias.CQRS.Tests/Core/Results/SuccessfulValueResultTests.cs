using FluentAssertions;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Tests.Utils;
using Newtonsoft.Json;
using Xunit;

namespace Intellias.CQRS.Tests.Core.Results
{
    public class SuccessfulValueResultTests
    {
        [Fact]
        public void Constructor_Always_Serializable()
        {
            var result = new SuccessfulValueResult(1);

            var serialized = JsonConvert.SerializeObject(result);
            var deserialized = JsonConvert.DeserializeObject<SuccessfulValueResult>(serialized);

            deserialized.Should().BeEquivalentTo(result);
        }

        [Fact]
        public void Constructor_Always_SetsValue()
        {
            var value = FixtureUtils.String();
            new SuccessfulValueResult(value).Value.Should().Be(value);
            new SuccessfulValueResult(value).Success.Should().BeTrue();
        }
    }
}