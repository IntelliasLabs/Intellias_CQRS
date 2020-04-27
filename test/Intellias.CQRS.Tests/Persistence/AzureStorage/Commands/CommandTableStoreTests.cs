using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Intellias.CQRS.Persistence.AzureStorage.Commands;
using Intellias.CQRS.Persistence.AzureStorage.Common;
using Intellias.CQRS.Tests.Core.Infrastructure.AssertionRules;
using Intellias.CQRS.Tests.Utils;
using Intellias.CQRS.Tests.Utils.StorageAccount;
using Microsoft.Azure.Cosmos.Table;
using Xunit;

namespace Intellias.CQRS.Tests.Persistence.AzureStorage.Commands
{
    public class CommandTableStoreTests : StorageAccountTestBase
    {
        private readonly CommandTableStore store;

        public CommandTableStoreTests(StorageAccountFixture fixture)
        {
            var options = new DefaultTableStorageOptions
            {
                TableNamePrefix = fixture.ExecutionContext.GetUniqueStorageTablePrefix(),
                ConnectionString = fixture.Configuration.StorageAccount.ConnectionString
            };

            store = new CommandTableStore(options);
        }

        [Fact]
        public async Task SaveAsync_CommandIsNew_SavesCommand()
        {
            var command = Fixtures.Pipelines.FakeCreateCommand();

            await store.SaveAsync(command);
            var savedCommand = (await store.GetAllAsync()).First(c => c.Id == command.Id);

            savedCommand.Should().BeEquivalentTo(command, options => options.ForMessage());
        }

        [Fact]
        public async Task SaveAsync_CommandIsAlreadyStored_Throws()
        {
            var command = Fixtures.Pipelines.FakeCreateCommand();

            await store.SaveAsync(command);

            await store.Awaiting(s => s.SaveAsync(command)).Should().ThrowAsync<StorageException>();
        }

        [Fact]
        public async Task GetAllAsync_MultipleCommandsAreStored_ReturnsAll()
        {
            var command1 = Fixtures.Pipelines.FakeCreateCommand();
            var command2 = Fixtures.Pipelines.FakeCreateCommand();

            await store.SaveAsync(command1);
            await store.SaveAsync(command2);
            var savedCommands = await store.GetAllAsync();

            savedCommands.Should().ContainEquivalentOf(command1, options => options.ForMessage());
            savedCommands.Should().ContainEquivalentOf(command2, options => options.ForMessage());
        }
    }
}