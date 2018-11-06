using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Tests.Core.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Xunit;

namespace Intellias.CQRS.CommandBus.Azure.Tests
{
    /// <summary>
    /// Azure Command Bus Integration Tests
    /// </summary>
    public class AzureCommandBusTests
    {
        /// <summary>
        /// Test publishing command to azure table and queue
        /// </summary>
        [Fact]
        public void PublishCommandTest()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            var storeConnectionString = configuration.GetConnectionString("TableStorageConnection");
            var account = CloudStorageAccount.Parse(storeConnectionString);

            var commandBus = new AzureCommandBus(account);

            var cmd = new TestUpdateCommand
            {
                AggregateRootId = "12345.1",
                ExpectedVersion = 1,
                TestData = "test data string",
                UserId = "test@user.com",
                Id = Unified.NewCode()
            };
            cmd.Metadata.Add(MetadataKey.AgreegateType, "competency");
            var executionResult = commandBus.PublishAsync(cmd).Result;
            Assert.True(executionResult.IsSuccess);

            // Cleanup
            var tableClient = account.CreateCloudTableClient().GetTableReference(cmd.Metadata[MetadataKey.AgreegateType]);
            var op = TableOperation.Delete(new CommandTableEntity(cmd, JsonConvert.SerializeObject(cmd, Formatting.Indented)));
            tableClient.ExecuteAsync(op).Wait();

            var queueClient = account.CreateCloudQueueClient().GetQueueReference(cmd.Metadata[MetadataKey.AgreegateType]);
            queueClient.ClearAsync();
        }
    }
}
