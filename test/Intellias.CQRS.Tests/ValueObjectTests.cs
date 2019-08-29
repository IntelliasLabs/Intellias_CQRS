using Intellias.CQRS.Tests.Core;
using Intellias.CQRS.Tests.Core.ValueObjects;
using Xunit;

namespace Intellias.CQRS.Tests
{
    public class ValueObjectTests : BaseTest
    {
        [Fact]
        public void ValueObjectEqualityTest()
        {
            var address1 = new Address
            {
                AddressLine = "test1"
            };

            var address2 = new Address
            {
                AddressLine = "test2"
            };

            Assert.NotEqual(address1, address2);

            address2.AddressLine = address1.AddressLine;

            Assert.Equal(address1, address2);
        }
    }
}
