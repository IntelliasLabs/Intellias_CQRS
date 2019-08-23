using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace Intellias.CQRS.Tests.Utils.Fixtures
{
    public class StorageAccountFixture : IDisposable
    {
        private readonly CloudBlobClient blobClient;
        private readonly CloudTableClient tableClient;

        public StorageAccountFixture()
        {
            Configuration = new TestsConfiguration();
            ExecutionContext = new TestsExecutionContext();

            var storageAccount = CloudStorageAccount
                .Parse(Configuration.StorageAccount.ConnectionString);

            this.blobClient = storageAccount.CreateCloudBlobClient();
            this.tableClient = storageAccount.CreateCloudTableClient();
        }

        public TestsExecutionContext ExecutionContext { get; }

        public TestsConfiguration Configuration { get; }

        public void Dispose()
        {
            DeleteAllContainersAsync(ExecutionContext.GetSessionPrefix())
                .GetAwaiter().GetResult();
            DeleteAllTablesAsync(ExecutionContext.GetSessionPrefix())
                .GetAwaiter().GetResult();

            GC.SuppressFinalize(this);
        }

        private async Task DeleteAllTablesAsync(string prefix)
        {
            TableContinuationToken continuationToken = null;

            do
            {
                var response = await this.tableClient.ListTablesSegmentedAsync(prefix, continuationToken);
                foreach (var table in response.Results)
                {
                    await table.DeleteIfExistsAsync();
                }

                continuationToken = response.ContinuationToken;
            }
            while (continuationToken != null);
        }

        private async Task DeleteAllContainersAsync(string prefix)
        {
            BlobContinuationToken continuationToken = null;

            do
            {
                var response = await this.blobClient.ListContainersSegmentedAsync(prefix, continuationToken);
                foreach (var container in response.Results)
                {
                    await container.DeleteIfExistsAsync();
                }

                continuationToken = response.ContinuationToken;
            }
            while (continuationToken != null);
        }
    }
}