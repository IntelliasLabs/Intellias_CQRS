using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.QueryStore.AzureTable.Mutable;
using Intellias.CQRS.QueryStore.AzureTable.Options;
using Intellias.CQRS.Tests.Core.Infrastructure.AssertionRules;
using Intellias.CQRS.Tests.Core.Queries;
using Intellias.CQRS.Tests.Fakes;
using Intellias.CQRS.Tests.Utils;
using Intellias.CQRS.Tests.Utils.StorageAccount;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using Microsoft.WindowsAzure.Storage.Table.Protocol;
using Xunit;

namespace Intellias.CQRS.Tests.QueryStores
{
    public class MutableQueryModelTableStorageTests : StorageAccountTestBase
    {
        private const int SizeOfTheFilterChunk = 10;
        private readonly MutableQueryModelTableStorage<FakeMutableQueryModel> storage;

        public MutableQueryModelTableStorageTests(StorageAccountFixture fixture)
        {
            var options = new TableStorageOptions
            {
                QueryChunkSize = SizeOfTheFilterChunk,
                TableNamePrefix = fixture.ExecutionContext.GetUniqueStorageTablePrefix(),
                ConnectionString = fixture.Configuration.StorageAccount.ConnectionString
            };

            storage = new MutableQueryModelTableStorage<FakeMutableQueryModel>(new OptionsMonitorFake<TableStorageOptions>(options));
        }

        [Fact]
        public async Task Get_NoQueryModel_Throws()
        {
            await storage.Awaiting(s => s.GetAsync(Unified.NewCode())).Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task Find_NoQueryModel_ReturnsDefault()
        {
            (await storage.FindAsync(Unified.NewCode())).Should().BeNull();
        }

        [Fact]
        public async Task Create_NoQueryModel_Creates()
        {
            var queryModel = new FakeMutableQueryModel();

            var stored = await storage.CreateAsync(queryModel);

            stored.Should().BeEquivalentTo(queryModel, options => options.ForMutableQueryModel());
        }

        [Fact]
        public async Task Create_QueryModelAlreadyExist_Throws()
        {
            var queryModel = new FakeMutableQueryModel();

            await storage.CreateAsync(queryModel);

            storage.Awaiting(s => s.CreateAsync(queryModel)).Should().Throw<StorageException>()
                .Which.RequestInformation.ExtendedErrorInformation.ErrorCode.Should().Be(TableErrorCodeStrings.EntityAlreadyExists);
        }

        [Fact]
        public async Task Replace_WhenETagIsInvalid_Throws()
        {
            var qm1 = await storage.CreateAsync(new FakeMutableQueryModel());

            // Update query model.
            qm1.Data = Unified.NewCode();
            await storage.ReplaceAsync(qm1);

            // Trying to update again should fail as ETag is changed after first update.
            (await storage.Awaiting(s => s.ReplaceAsync(qm1)).Should().ThrowAsync<StorageException>())
                .Which.RequestInformation.ExtendedErrorInformation.ErrorCode.Should().Be(TableErrorCodeStrings.UpdateConditionNotSatisfied);
        }

        [Fact]
        public async Task Replace_QueryModelDoesntExist_Throws()
        {
            (await storage.Awaiting(s => s.ReplaceAsync(new FakeMutableQueryModel())).Should().ThrowAsync<StorageException>())
                .Which.RequestInformation.ExtendedErrorInformation.ErrorCode.Should().Be(StorageErrorCodeStrings.ResourceNotFound);
        }

        [Fact]
        public async Task Replace_WhenETagIsValid_Replaces()
        {
            var qm1 = await storage.CreateAsync(new FakeMutableQueryModel());

            // Update for the first time.
            qm1.Data = Unified.NewCode();
            var updated1 = await storage.ReplaceAsync(qm1);

            // Update again with the valid changed ETag.
            updated1.Data = Unified.NewCode();
            var updated2 = await storage.ReplaceAsync(updated1);

            updated2.Should().BeEquivalentTo(updated1, options => options.ForMutableQueryModel());
        }

        [Fact]
        public async Task GetAll_NoQueryModels_ReturnsEmpty()
        {
            (await storage.GetAllAsync()).Should().BeEmpty();
        }

        [Fact]
        public async Task GetAll_HasQueryModels_ReturnsAll()
        {
            var qm1 = await storage.CreateAsync(new FakeMutableQueryModel());
            var qm2 = await storage.CreateAsync(new FakeMutableQueryModel());

            (await storage.GetAllAsync()).Should().BeEquivalentTo(new[] { qm1, qm2 }, options => options.ForMutableQueryModel());
        }

        [Fact]
        public async Task GetAllByIds_QueriesAll_ReturnsAll()
        {
            var numberOfQueryModels = SizeOfTheFilterChunk + 1; // To ensure that chunked query is working.

            // Create query models to query by ids.
            var queryModels = new List<FakeMutableQueryModel>();
            for (var i = 0; i < numberOfQueryModels; i++)
            {
                queryModels.Add(await storage.CreateAsync(new FakeMutableQueryModel()));
            }

            // Add one more which is not part of the filter.
            await storage.CreateAsync(new FakeMutableQueryModel());

            var results = await storage.GetAllAsync(queryModels.Select(qm => qm.Id).ToArray());

            // Ensure only queried models are returned.
            results.Should().BeEquivalentTo(queryModels, options => options.ForMutableQueryModel());
        }
    }
}