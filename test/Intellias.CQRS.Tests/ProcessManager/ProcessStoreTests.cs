using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.ProcessManager.Pipelines.Response;
using Intellias.CQRS.ProcessManager.Stores;
using Intellias.CQRS.QueryStore.AzureTable.Options;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Fakes;
using Intellias.CQRS.Tests.ProcessManager.Infrastructure;
using Intellias.CQRS.Tests.Utils.StorageAccount;
using Microsoft.Azure.Cosmos.Table;
using Xunit;

namespace Intellias.CQRS.Tests.ProcessManager
{
    public class ProcessStoreTests : StorageAccountTestBase
    {
        private ProcessStore<ProcessHandler1> storage;
        private Fixture fixture;

        public ProcessStoreTests(StorageAccountFixture storageFixture)
        {
            var options = new TableStorageOptions
            {
                TableNamePrefix = storageFixture.ExecutionContext.GetUniqueStorageTablePrefix(),
                ConnectionString = storageFixture.Configuration.StorageAccount.ConnectionString
            };

            storage = new ProcessStore<ProcessHandler1>(new OptionsMonitorFake<TableStorageOptions>(options));
            fixture = new Fixture();
        }

        [Fact]
        public async Task PersistCommands_All_CommandsSaved()
        {
            var id = Unified.NewCode();
            var commands = fixture.CreateMany<TestCreateCommand>()
                .ToArray();
            var expectedProcessCommands = commands.Select(cmd => new ProcessMessage(cmd));

            await storage.PersistMessagesAsync(id, commands);

            var processCommands = await storage.GetMessagesAsync(id);
            processCommands.Should().BeEquivalentTo(expectedProcessCommands);
        }

        [Fact]
        public async Task MarkCommnadAsPublished_All_CommandsPublished()
        {
            var id = Unified.NewCode();
            var commands = fixture.CreateMany<TestCreateCommand>()
                .ToArray();
            var expectedProcessCommands = commands.Select(cmd => new ProcessMessage(cmd, true));

            await storage.PersistMessagesAsync(id, commands);
            foreach (var cmd in commands)
            {
                await storage.MarkMessageAsPublishedAsync(id, cmd);
            }

            var processCommands = await storage.GetMessagesAsync(id);
            processCommands.Should().BeEquivalentTo(expectedProcessCommands);
        }

        [Fact]
        public async Task MarkCommnadAsPublished_One_OneCommandPublished()
        {
            var id = Unified.NewCode();
            var commands = fixture.CreateMany<TestCreateCommand>()
                .ToArray();
            var publishedCommand = commands.First();
            var expectedProcessCommands = commands
                .Skip(1)
                .Select(cmd => new ProcessMessage(cmd))
                .Union(new[] { new ProcessMessage(publishedCommand, true) });

            await storage.PersistMessagesAsync(id, commands);
            await storage.MarkMessageAsPublishedAsync(id, publishedCommand);

            var processCommands = await storage.GetMessagesAsync(id);
            processCommands.Should().BeEquivalentTo(expectedProcessCommands);
        }

        [Fact]
        public async Task GetCommands_NoCommnds_EmptyCollcetionReturned()
        {
            var id = Unified.NewCode();

            var processCommands = await storage.GetMessagesAsync(id);

            processCommands.Should().BeEmpty();
        }

        [Fact]
        public async Task MarkCommnadAsPublished_NoPersistedCommnds_ExpectedException()
        {
            var id = Unified.NewCode();
            var command = fixture.Create<TestCreateCommand>();

            Func<Task> act = async () => await storage.MarkMessageAsPublishedAsync(id, command);

            await act.Should().ThrowAsync<StorageException>();
        }
    }
}