using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Core.Results.Errors;
using Intellias.CQRS.DomainServices;
using Intellias.CQRS.Tests.Core.Infrastructure;
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
            var cfg = new TestsConfiguration();
            var account = CloudStorageAccount.Parse(cfg.StorageAccount.ConnectionString);
            uniqueConstraintService = new UniqueConstraintService(account);

            var client = account.CreateCloudTableClient();
            table = client.GetTableReference(typeof(UniqueConstraintService).Name);
        }

        [Fact]
        public async Task ReserveTestName()
        {
            var testId = Unified.NewCode();

            await uniqueConstraintService.ReserveConstraintAsync("TestIndex", testId);

            var hash = Unified.NewCode(Unified.NewHash(Encoding.UTF8.GetBytes(testId)));
            var testResult = await table.ExecuteAsync(TableOperation.Retrieve("TestIndex", hash));
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

            var failedResult = (FailedResult)result;

            failedResult.CodeInfo.Should().BeEquivalentTo(CoreErrorCodes.ValidationFailed);
            failedResult.Details.Single().CodeInfo.Should().BeEquivalentTo(CoreErrorCodes.NameIsInUse);
        }

        [Fact]
        public async Task ReplaceTestName()
        {
            var testId = Unified.NewCode();
            var updatedTestId = Unified.NewCode();

            await uniqueConstraintService.ReserveConstraintAsync("TestIndex", testId);
            await uniqueConstraintService.UpdateConstraintAsync("TestIndex", testId, updatedTestId);

            var hash = Unified.NewCode(Unified.NewHash(Encoding.UTF8.GetBytes(updatedTestId)));
            var testResult = await table.ExecuteAsync(TableOperation.Retrieve("TestIndex", hash));
            Assert.NotNull(testResult.Result);
        }

        [Fact]
        public async Task ReplaceTestNameNoSource()
        {
            var testId = Unified.NewCode();
            var updatedTestId = Unified.NewCode();

            var result = await uniqueConstraintService.UpdateConstraintAsync("TestIndex", testId, updatedTestId);

            Assert.False(result.Success);

            var failedResult = (FailedResult)result;

            failedResult.CodeInfo.Should().BeEquivalentTo(CoreErrorCodes.UnhandledError);
            failedResult.Details.Single().CodeInfo.Should().BeEquivalentTo(CoreErrorCodes.NameIsNotFound);
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
            var oldHash = Unified.NewCode(Unified.NewHash(Encoding.UTF8.GetBytes(testId)));
            var testResult = await table.ExecuteAsync(TableOperation.Retrieve("TestIndex", oldHash));
            Assert.NotNull(testResult.Result);
        }

        [Fact]
        public async Task ReplaceTestNameNotFoundTarget()
        {
            var testId = Unified.NewCode();
            var updatedTestId = Unified.NewCode();

            var result = await uniqueConstraintService.UpdateConstraintAsync("TestIndex", testId, updatedTestId);

            Assert.False(result.Success);

            var failedResult = (FailedResult)result;

            failedResult.CodeInfo.Should().BeEquivalentTo(CoreErrorCodes.UnhandledError);
            failedResult.Details.Single().CodeInfo.Should().BeEquivalentTo(CoreErrorCodes.NameIsNotFound);
        }

        [Fact]
        public async Task RemoveTestNameNoSource()
        {
            var testId = Unified.NewCode();
            var result = await uniqueConstraintService.RemoveConstraintAsync("TestIndex", testId);

            Assert.False(result.Success);

            var failedResult = (FailedResult)result;

            failedResult.CodeInfo.Should().BeEquivalentTo(CoreErrorCodes.UnhandledError);
            failedResult.Details.Single().CodeInfo.Should().BeEquivalentTo(CoreErrorCodes.NameIsNotFound);
        }

        [Fact]
        public async Task RemoveTestName()
        {
            var testId = Unified.NewCode();
            await uniqueConstraintService.ReserveConstraintAsync("TestIndex", testId);

            await uniqueConstraintService.RemoveConstraintAsync("TestIndex", testId);

            var result = await table.ExecuteAsync(TableOperation.Retrieve("TestIndex", testId));

            Assert.Null(result.Result);
        }

        [Fact]
        public async Task ReserveDisallowedCharsName()
        {
            var dissalowedCharsName = $"{Unified.NewCode()}_#\t\n?";

            var result = await uniqueConstraintService.ReserveConstraintAsync("TestIndex", dissalowedCharsName);

            result.Success.Should().BeTrue();
        }
    }
}
