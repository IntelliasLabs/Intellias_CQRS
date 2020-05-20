using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Persistence.AzureStorage.Common;
using Intellias.CQRS.QueryStore.AzureTable.Immutable;
using Intellias.CQRS.Tests.Core.EventHandlers;
using Intellias.CQRS.Tests.Core.Infrastructure.AssertionRules;
using Intellias.CQRS.Tests.Utils.StorageAccount;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Cosmos.Table.Protocol;
using Xunit;

namespace Intellias.CQRS.Tests.QueryStores
{
    public class ImmutableQueryModelTableStorage2Tests : StorageAccountTestBase
    {
        private readonly ImmutableQueryModelTableStorage2<FakeImmutableQueryModel> storage;

        public ImmutableQueryModelTableStorage2Tests(StorageAccountFixture fixture)
        {
            var options = new DefaultTableStorageOptions
            {
                TableNamePrefix = fixture.ExecutionContext.GetUniqueStorageTablePrefix(),
                ConnectionString = fixture.Configuration.StorageAccount.ConnectionString
            };

            storage = new ImmutableQueryModelTableStorage2<FakeImmutableQueryModel>(options);
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
        public async Task FindLatestAsync_NoQueryModel_ReturnsDefault()
        {
            (await storage.FindLatestAsync(Unified.NewCode())).Should().BeNull();
        }

        [Fact]
        public async Task FindLatestAsync_HasQueryModels_ReturnsLatest()
        {
            var id = Unified.NewCode();
            await storage.CreateAsync(new FakeImmutableQueryModel { Id = id, Version = 1 });
            var qm2 = await storage.CreateAsync(new FakeImmutableQueryModel { Id = id, Version = 2 });

            var latest = await storage.FindLatestAsync(id);

            latest.Should().BeEquivalentTo(qm2);
        }

        [Fact]
        public async Task GetLatestAsync_NoQueryModel_Throws()
        {
            await storage.Awaiting(s => s.GetLatestAsync(Unified.NewCode())).Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task GetLatestAsync_HasQueryModels_ReturnsLatest()
        {
            var id = Unified.NewCode();
            await storage.CreateAsync(new FakeImmutableQueryModel { Id = id, Version = 1 });
            var qm2 = await storage.CreateAsync(new FakeImmutableQueryModel { Id = id, Version = 2 });

            var latest = await storage.FindLatestAsync(id);

            latest.Should().BeEquivalentTo(qm2);
        }

        [Fact]
        public async Task FindEqualOrOlderAsync_NoQueryModels_ReturnsNull()
        {
            (await storage.FindEqualOrLessAsync(Unified.NewCode(), 1)).Should().BeNull();
        }

        [Fact]
        public async Task FindEqualOrOlderAsync_ExactlyVersion_ReturnsExactlyVersion()
        {
            var id = Unified.NewCode();

            await storage.CreateAsync(new FakeImmutableQueryModel { Id = id, Version = 1 });
            var queryModel = await storage.CreateAsync(new FakeImmutableQueryModel { Id = id, Version = 3 });

            var result = await storage.FindEqualOrLessAsync(id, 3);

            result.Should().BeEquivalentTo(queryModel);
        }

        [Fact]
        public async Task FindEqualOrOlderAsync_OlderVersion_ReturnsClosestOlderVersionVersion()
        {
            var id = Unified.NewCode();

            await storage.CreateAsync(new FakeImmutableQueryModel { Id = id, Version = 1 });
            var queryModel = await storage.CreateAsync(new FakeImmutableQueryModel { Id = id, Version = 4 });
            await storage.CreateAsync(new FakeImmutableQueryModel { Id = id, Version = 10 });

            var result = await storage.FindEqualOrLessAsync(id, 7);

            result.Should().BeEquivalentTo(queryModel);
        }

        [Fact]
        public async Task GetEqualOrOlderAsync_NoQueryModels_Throws()
        {
            await storage.Awaiting(s => s.GetEqualOrLessAsync(Unified.NewCode(), 1)).Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task GetEqualOrOlderAsync_ExactlyVersion_ReturnsExactlyVersion()
        {
            var id = Unified.NewCode();

            await storage.CreateAsync(new FakeImmutableQueryModel { Id = id, Version = 1 });
            var queryModel = await storage.CreateAsync(new FakeImmutableQueryModel { Id = id, Version = 3 });

            var result = await storage.GetEqualOrLessAsync(id, 3);

            result.Should().BeEquivalentTo(queryModel);
        }

        [Fact]
        public async Task GetEqualOrOlderAsync_OlderVersion_ReturnsClosestOlderVersionVersion()
        {
            var id = Unified.NewCode();

            await storage.CreateAsync(new FakeImmutableQueryModel { Id = id, Version = 1 });
            var queryModel = await storage.CreateAsync(new FakeImmutableQueryModel { Id = id, Version = 4 });
            await storage.CreateAsync(new FakeImmutableQueryModel { Id = id, Version = 10 });

            var result = await storage.GetEqualOrLessAsync(id, 7);

            result.Should().BeEquivalentTo(queryModel);
        }
    }
}