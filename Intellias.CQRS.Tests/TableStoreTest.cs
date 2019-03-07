using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.AzureTable.Core;
using Intellias.CQRS.Core.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Xunit;

namespace Intellias.CQRS.Tests
{
    public class TableStoreTest
    {
        private TableStore<TestStoreRecord> Store { get; }

        public TableStoreTest()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            var storeConnectionString = configuration.GetConnectionString("TableStorageConnection");
            Store = new TableStore<TestStoreRecord>(CloudStorageAccount.Parse(storeConnectionString));

        }


        [Fact]
        public async Task ShouldProcessCrudOperations()
        {
            var record = new TestStoreRecord { Data = "Test data" };

            // Insert test
            await Store.InsertAsync(record);

            var result = await Store.GetAsync(record.Id);
            Assert.True(result.Data == record.Data);

            // Update test
            record.Data = "Test data updated";
            await Store.UpdateAsync(record);

            var updatedresult = await Store.GetAsync(record.Id);
            Assert.True(updatedresult.Data == record.Data);

            // Delete test
            await Store.DeleteAsync(record);
            await Assert.ThrowsAsync<KeyNotFoundException>(() => Store.GetAsync(record.Id));
        }
    }

    public class TestStoreRecord : IIdentified
    {
        public string Id { get; } = Unified.NewCode();
        public string Data { get; set; }
    }
}
