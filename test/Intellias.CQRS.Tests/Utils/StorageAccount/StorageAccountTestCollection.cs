using Xunit;

namespace Intellias.CQRS.Tests.Utils.StorageAccount
{
    [CollectionDefinition(nameof(StorageAccountTestCollection))]
    public class StorageAccountTestCollection : ICollectionFixture<StorageAccountFixture>
    {
    }
}