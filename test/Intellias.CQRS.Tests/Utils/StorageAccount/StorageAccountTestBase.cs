using Xunit;

namespace Intellias.CQRS.Tests.Utils.StorageAccount
{
    [Collection(nameof(StorageAccountTestCollection))]
    public class StorageAccountTestBase
    {
        protected StorageAccountTestBase()
        {
            this.Configuration = new TestsConfiguration();
            this.ExecutionContext = new TestsExecutionContext();
        }

        protected TestsConfiguration Configuration { get; }

        protected TestsExecutionContext ExecutionContext { get; }
    }
}