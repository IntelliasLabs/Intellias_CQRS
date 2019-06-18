using System.Threading.Tasks;
using Intellias.CQRS.Core.Exceptions;
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

            await uniqueConstraintService.ReserveConstraintAsync("TestIndex", testId);
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => uniqueConstraintService.ReserveConstraintAsync("TestIndex", testId));
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

            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => uniqueConstraintService.UpdateConstraintAsync("TestIndex", testId, updatedTestId));
        }

        [Fact]
        public async Task ReplaceTestNameDuplicateTarget()
        {
            var testId = Unified.NewCode();
            var updatedTestId = Unified.NewCode();

            await uniqueConstraintService.ReserveConstraintAsync("TestIndex", testId);
            await uniqueConstraintService.ReserveConstraintAsync("TestIndex", updatedTestId);
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => uniqueConstraintService.UpdateConstraintAsync("TestIndex", testId, updatedTestId));

            // Check original record is present
            var testResult = await table.ExecuteAsync(TableOperation.Retrieve("TestIndex", testId));
            Assert.NotNull(testResult.Result);
        }

        [Fact]
        public async Task ReplaceTestNameNotFoundTarget()
        {
            var testId = Unified.NewCode();
            var updatedTestId = Unified.NewCode();

            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => uniqueConstraintService.UpdateConstraintAsync("TestIndex", testId, updatedTestId));
        }

        [Fact]
        public async Task RemoveTestNameNoSource()
        {
            var testId = Unified.NewCode();
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => uniqueConstraintService.RemoveConstraintAsync("TestIndex", testId));
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
