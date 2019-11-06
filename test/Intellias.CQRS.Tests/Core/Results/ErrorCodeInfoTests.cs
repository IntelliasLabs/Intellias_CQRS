using FluentAssertions;
using Intellias.CQRS.Core.Results.Errors;
using Intellias.CQRS.Tests.Utils;
using Newtonsoft.Json;
using Xunit;

namespace Intellias.CQRS.Tests.Core.Results
{
    public class ErrorCodeInfoTests
    {
        [Fact]
        public void ErrorCodeInfo_FromCode_Serializable()
        {
            var code = FixtureUtils.String();

            var errorCodeInfo = SerializeDeserialize(new ErrorCodeInfo(code));

            errorCodeInfo.Should().BeEquivalentTo(new ErrorCodeInfo(code));
        }

        [Fact]
        public void ErrorCodeInfo_FromCodeAndMessage_Serializable()
        {
            var code = FixtureUtils.String();
            var message = FixtureUtils.String();

            var errorCodeInfo = SerializeDeserialize(new ErrorCodeInfo(code, message));

            errorCodeInfo.Should().BeEquivalentTo(new ErrorCodeInfo(code, message));
        }

        [Fact]
        public void ErrorCodeInfo_FromPrefixAndCodeAndMessage_Serializable()
        {
            var prefix = FixtureUtils.String();
            var code = FixtureUtils.String();
            var message = FixtureUtils.String();

            var errorCodeInfo = SerializeDeserialize(new ErrorCodeInfo(prefix, code, message));

            errorCodeInfo.Should().BeEquivalentTo(new ErrorCodeInfo(prefix, code, message));
        }

        [Fact]
        public void ExistingJsonData_Always_Deserializable()
        {
            var code = FixtureUtils.String();
            var message = FixtureUtils.String();
            var json = $@"{{ ""Code"": ""{code}"", ""Message"": ""{message}"" }}";

            var errorCodeInfo = JsonConvert.DeserializeObject<ErrorCodeInfo>(json);

            errorCodeInfo.Should().BeEquivalentTo(new ErrorCodeInfo(code, message));
        }

        [Fact]
        public void TwoErrorCodes_CodesAreEqual_ErrorCodesAreEqual()
        {
            var code = FixtureUtils.String();
            var message1 = FixtureUtils.String();
            var message2 = FixtureUtils.String();

            new ErrorCodeInfo(code, message1).Equals(new ErrorCodeInfo(code, message2))
                .Should().BeTrue();
        }

        private ErrorCodeInfo SerializeDeserialize(ErrorCodeInfo errorCodeInfo)
        {
            return JsonConvert.DeserializeObject<ErrorCodeInfo>(JsonConvert.SerializeObject(errorCodeInfo));
        }
    }
}