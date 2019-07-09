using System.Threading.Tasks;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.DomainServices;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Xunit;

namespace Intellias.CQRS.Tests.DomainServices
{
    public class UniqueConstraintServiceTests
    {
        private readonly UniqueConstraintService uniqueConstraintService;
        private readonly CloudTable table;

        public UniqueConstraintServiceTests()
        {
            var account = CloudStorageAccount.DevelopmentStorageAccount;
            uniqueConstraintService = new UniqueConstraintService(account);

            var client = account.CreateCloudTableClient();
            table = client.GetTableReference(typeof(UniqueConstraintService).Name);
        }

        [Fact]
        public async Task ReserveTestName()
        {
            var testId = Unified.NewCode();

            await uniqueConstraintService.ReserveConstraintAsync("TestIndex", testId);

            var testResult = await table.ExecuteAsync(TableOperation.Retrieve("TestIndex", testId));
            Assert.NotNull(testResult.Result);
        }

        [Fact]
        public async Task ReserveTestNameDuplicate()
        {
            var testId = Unified.NewCode();

            var result = await uniqueConstraintService.ReserveConstraintAsync("TestIndex", testId);
            Assert.True(result.Success);

            result = await uniqueConstraintService.ReserveConstraintAsync("TestIndex", testId);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task ReplaceTestName()
        {
            var testId = Unified.NewCode();
            var updatedTestId = Unified.NewCode();

            await uniqueConstraintService.ReserveConstraintAsync("TestIndex", testId);
            await uniqueConstraintService.UpdateConstraintAsync("TestIndex", testId, updatedTestId);

            var testResult = await table.ExecuteAsync(TableOperation.Retrieve("TestIndex", updatedTestId));
            Assert.NotNull(testResult.Result);
        }

        [Fact]
        public async Task ReplaceTestNameNoSource()
        {
            var testId = Unified.NewCode();
            var updatedTestId = Unified.NewCode();

            var result = await uniqueConstraintService.UpdateConstraintAsync("TestIndex", testId, updatedTestId);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task ReplaceTestNameDuplicateTarget()
        {
            var testId = Unified.NewCode();
            var updatedTestId = Unified.NewCode();

            await uniqueConstraintService.ReserveConstraintAsync("TestIndex", testId);
            await uniqueConstraintService.ReserveConstraintAsync("TestIndex", updatedTestId);
            var result = await uniqueConstraintService.UpdateConstraintAsync("TestIndex", testId, updatedTestId);
            Assert.False(result.Success);

            // Check original record is present
            var testResult = await table.ExecuteAsync(TableOperation.Retrieve("TestIndex", testId));
            Assert.NotNull(testResult.Result);
        }

        [Fact]
        public async Task ReplaceTestNameNotFoundTarget()
        {
            var testId = Unified.NewCode();
            var updatedTestId = Unified.NewCode();

            var result = await uniqueConstraintService.UpdateConstraintAsync("TestIndex", testId, updatedTestId);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task RemoveTestNameNoSource()
        {
            var testId = Unified.NewCode();
            var result = await uniqueConstraintService.RemoveConstraintAsync("TestIndex", testId);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task RemoveTestName()
        {
            var testId = Unified.NewCode();
            await uniqueConstraintService.ReserveConstraintAsync("TestIndex", testId);

            await uniqueConstraintService.RemoveConstraintAsync("TestIndex", testId);

            var testResult = await table.ExecuteAsync(TableOperation.Retrieve("TestIndex", testId));
            Assert.Null(testResult.Result);
        }
    }
}
