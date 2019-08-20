using Intellias.CQRS.Tests.Utils.Fixtures;
using Xunit;

namespace Intellias.CQRS.Tests.Utils
{
    [Collection(nameof(StorageAccountTestCollection))]
    public class StorageAccountTestBase
    {
        private readonly StorageAccountFixture fixture;

        protected StorageAccountTestBase(StorageAccountFixture fixture)
        {
            this.fixture = fixture;

            this.Configuration = new TestsConfiguration();
            this.ExecutionContext = new TestsExecutionContext();
        }

        protected TestsConfiguration Configuration { get; }

        protected TestsExecutionContext ExecutionContext { get; }
    }
}