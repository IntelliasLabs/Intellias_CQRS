using FluentAssertions;
using Intellias.CQRS.Core.Config;
using Xunit;

namespace Intellias.CQRS.Tests.Core.Config
{
    public class MessageWithTypeNameJsonConverterTests
    {
        [Fact]
        public void CanWriteWithAnyTypeShoudBeFalse()
        {
            var converter = new MessageWithTypeNameJsonConverter();

            converter.CanWrite.Should().BeFalse();
        }
    }
}
