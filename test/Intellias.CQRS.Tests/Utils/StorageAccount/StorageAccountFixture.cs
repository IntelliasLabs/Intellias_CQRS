using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Intellias.CQRS.Tests.Core.Infrastructure;
using Microsoft.Azure.Cosmos.Table;

namespace Intellias.CQRS.Tests.Utils.StorageAccount
{
    public class StorageAccountFixture : IDisposable
    {
        private readonly BlobServiceClient blobClient;
        private readonly CloudTableClient tableClient;

        public StorageAccountFixture()
        {
            Configuration = new TestsConfiguration();
            ExecutionContext = new TestsExecutionContext();

            var storageAccount = CloudStorageAccount
                .Parse(Configuration.StorageAccount.ConnectionString);
            this.tableClient = storageAccount.CreateCloudTableClient();

            this.blobClient = new BlobServiceClient(Configuration.StorageAccount.ConnectionString);
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
            var pagenable = this.blobClient.GetBlobContainersAsync(prefix: prefix);

            await foreach (var blobItem in pagenable)
            {
                await this.blobClient.DeleteBlobContainerAsync(blobItem.Name);
            }
        }
    }
}