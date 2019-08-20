using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Queries.Mutable;
using Intellias.CQRS.QueryStore.AzureTable;
using Intellias.CQRS.QueryStore.AzureTable.Mutable;
using Intellias.CQRS.QueryStore.AzureTable.Options;
using Intellias.CQRS.Tests.Fakes;
using Intellias.CQRS.Tests.Utils;
using Intellias.CQRS.Tests.Utils.Fixtures;
using Microsoft.WindowsAzure.Storage;
using Xunit;

namespace Intellias.CQRS.Tests.QueryStores
{
    public class MutableQueryModelTableStorageTests : StorageAccountTestBase
    {
        private const int SizeOfTheFilterChunk = 10;
        private readonly MutableQueryModelTableStorage<MutableQueryModel> storage;

        public MutableQueryModelTableStorageTests(StorageAccountFixture fixture)
            : base(fixture)
        {
            var options = new TableStorageOptions
            {
                QueryChunkSize = SizeOfTheFilterChunk,
                TableNamePrefix = fixture.ExecutionContext.GetUniqueStorageTablePrefix(),
                ConnectionString = fixture.Configuration.StorageAccount.ConnectionString
            };

            storage = new MutableQueryModelTableStorage<MutableQueryModel>(new OptionsMonitorFake<TableStorageOptions>(options));
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
            var queryModel = new MutableQueryModel { Id = Unified.NewCode() };

            await storage.CreateAsync(queryModel);
            var stored = await storage.FindAsync(queryModel.Id);

            stored.Should().BeEquivalentTo(queryModel, options => options.ForQueryModel());
        }

        [Fact]
        public async Task Create_QueryModelAlreadyExist_Throws()
        {
            var queryModel = new MutableQueryModel { Id = Unified.NewCode() };

            await storage.CreateAsync(queryModel);

            storage.Awaiting(s => s.CreateAsync(queryModel)).Should().Throw<StorageException>()
                .Which.RequestInformation.HttpStatusCode.Should().Be((int)HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task Replace_WhenETagIsInvalid_Throws()
        {
            var qm1 = new MutableQueryModel { Id = Unified.NewCode() };
            await storage.CreateAsync(qm1);

            var stored = await storage.GetAsync(qm1.Id);
            stored.SomeProperty = Unified.NewCode();

            // Update query model.
            await storage.ReplaceAsync(stored);

            // Trying to update again should fail as ETag is changed after first update.
            (await storage.Awaiting(s => s.ReplaceAsync(stored)).Should().ThrowAsync<StorageException>())
                .Which.RequestInformation.HttpStatusCode.Should().Be((int)HttpStatusCode.PreconditionFailed);
        }

        [Fact]
        public async Task Replace_WhenETagIsValid_Replaces()
        {
            var qm1 = new MutableQueryModel { Id = Unified.NewCode() };
            await storage.CreateAsync(qm1);

            // Update for the first time.
            var updated1 = await storage.GetAsync(qm1.Id);
            updated1.SomeProperty = Unified.NewCode();
            await storage.ReplaceAsync(updated1);

            // Update again with the valid changed ETag.
            var updated2 = await storage.GetAsync(qm1.Id);
            updated2.SomeProperty = Unified.NewCode();
            await storage.ReplaceAsync(updated2);

            var result = await storage.GetAsync(updated2.Id);

            result.Should().BeEquivalentTo(updated2, options => options.ForQueryModel());
        }

        [Fact]
        public async Task GetAll_NoQueryModels_ReturnsEmpty()
        {
            (await storage.GetAllAsync()).Should().BeEmpty();
        }

        [Fact]
        public async Task GetAll_HasQueryModels_ReturnsAll()
        {
            var qm1 = new MutableQueryModel { Id = Unified.NewCode() };
            var qm2 = new MutableQueryModel { Id = Unified.NewCode() };

            await storage.CreateAsync(qm1);
            await storage.CreateAsync(qm2);

            (await storage.GetAllAsync()).Should().BeEquivalentTo(new[] { qm1, qm2 }, options => options.ForQueryModel());
        }

        [Fact]
        public async Task GetAllByIds_QueriesAll_ReturnsAll()
        {
            var numberOfQueryModels = SizeOfTheFilterChunk + 1; // To ensure that chunked query is working.

            // Create query models to query by ids.
            var queryModels = new List<MutableQueryModel>();
            for (var i = 0; i < numberOfQueryModels; i++)
            {
                var qm = new MutableQueryModel { Id = Unified.NewCode() };
                await storage.CreateAsync(qm);
                queryModels.Add(await storage.GetAsync(qm.Id));
            }

            // Add one more which is not part of the filter.
            await storage.CreateAsync(new MutableQueryModel { Id = Unified.NewCode() });

            var results = await storage.GetAllAsync(queryModels.Select(qm => qm.Id).ToArray());

            // Ensure only queried models are returned.
            results.Should().BeEquivalentTo(queryModels, options => options.ForQueryModel());
        }

        private class MutableQueryModel : BaseMutableQueryModel
        {
            public string SomeProperty { get; set; } = Unified.NewCode();
        }
    }
}