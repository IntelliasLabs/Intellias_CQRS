using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.QueryStore.AzureTable.Immutable;
using Intellias.CQRS.QueryStore.AzureTable.Options;
using Intellias.CQRS.Tests.Core.Infrastructure.AssertionRules;
using Intellias.CQRS.Tests.Core.Queries;
using Intellias.CQRS.Tests.Fakes;
using Intellias.CQRS.Tests.Utils.StorageAccount;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table.Protocol;
using Xunit;

namespace Intellias.CQRS.Tests.QueryStores
{
    public class ImmutableQueryModelTableStorageTests : StorageAccountTestBase
    {
        private readonly ImmutableQueryModelTableStorage<FakeImmutableQueryModel> storage;

        public ImmutableQueryModelTableStorageTests(StorageAccountFixture fixture)
        {
            var options = new TableStorageOptions
            {
                TableNamePrefix = fixture.ExecutionContext.GetUniqueStorageTablePrefix(),
                ConnectionString = fixture.Configuration.StorageAccount.ConnectionString
            };

            storage = new ImmutableQueryModelTableStorage<FakeImmutableQueryModel>(new OptionsMonitorFake<TableStorageOptions>(options));
        }

        [Fact]
        public async Task Get_NoQueryModel_Throws()
        {
            await storage.Awaiting(s => s.GetAsync(Unified.NewCode(), 1)).Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task Find_NoQueryModel_ReturnsDefault()
        {
            (await storage.FindAsync(Unified.NewCode(), 1)).Should().BeNull();
        }

        [Fact]
        public async Task Create_NoQueryModel_Creates()
        {
            var queryModel = new FakeImmutableQueryModel();

            var stored = await storage.CreateAsync(queryModel);

            stored.Should().BeEquivalentTo(queryModel, options => options.ForImmutableQueryModel());
        }

        [Fact]
        public async Task Create_QueryModelAlreadyExist_Throws()
        {
            var queryModel = new FakeImmutableQueryModel();

            await storage.CreateAsync(queryModel);

            storage.Awaiting(s => s.CreateAsync(queryModel)).Should().Throw<StorageException>()
                .Which.RequestInformation.ExtendedErrorInformation.ErrorCode.Should().Be(TableErrorCodeStrings.EntityAlreadyExists);
        }

        [Fact]
        public async Task GetLatestAsync_NoQueryModel_ReturnsDefault()
        {
            (await storage.GetLatestAsync(Unified.NewCode())).Should().BeNull();
        }

        [Fact]
        public async Task GetLatestAsync_HasQueryModels_ReturnsLatest()
        {
            var id = Unified.NewCode();
            await storage.CreateAsync(new FakeImmutableQueryModel { Id = id, Version = 1 });
            var qm2 = await storage.CreateAsync(new FakeImmutableQueryModel { Id = id, Version = 2 });

            var latest = await storage.GetLatestAsync(id);

            latest.Should().BeEquivalentTo(qm2);
        }
    }
}