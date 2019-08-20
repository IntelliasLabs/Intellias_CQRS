using Intellias.CQRS.Tests.Utils.Fixtures;
using Xunit;

namespace Intellias.CQRS.Tests.Utils
{
    [CollectionDefinition(nameof(StorageAccountTestCollection))]
    public class StorageAccountTestCollection : ICollectionFixture<StorageAccountFixture>
    {
    }
}