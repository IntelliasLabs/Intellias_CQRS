using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Persistence.AzureStorage.Common;
using Intellias.CQRS.QueryStore.AzureTable.Mutable;
using Intellias.CQRS.Tests.Core.EventHandlers;
using Intellias.CQRS.Tests.Core.Infrastructure.AssertionRules;
using Intellias.CQRS.Tests.Utils.StorageAccount;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Cosmos.Table.Protocol;
using Microsoft.Azure.Cosmos.Tables.SharedFiles;
using Xunit;

namespace Intellias.CQRS.Tests.QueryStores
{
    public class MutableQueryModelTableStorage2Tests : StorageAccountTestBase
    {
        private const int SizeOfTheFilterChunk = 10;
        private readonly MutableQueryModelTableStorage2<FakeMutableQueryModel> storage;

        public MutableQueryModelTableStorage2Tests(StorageAccountFixture fixture)
        {
            var options = new DefaultTableStorageOptions
            {
                TableNamePrefix = fixture.ExecutionContext.GetUniqueStorageTablePrefix(),
                ConnectionString = fixture.Configuration.StorageAccount.ConnectionString
            };

            storage = new MutableQueryModelTableStorage2<FakeMutableQueryModel>(options);
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
            var queryModel = GetQueryModel();

            var stored = await storage.CreateAsync(queryModel);

            stored.Should().BeEquivalentTo(queryModel, options => options.ForMutableQueryModel());
        }

        [Fact]
        public async Task Create_QueryModelAlreadyExist_Throws()
        {
            var queryModel = GetQueryModel();

            await storage.CreateAsync(queryModel);

            storage.Awaiting(s => s.CreateAsync(queryModel)).Should().Throw<StorageException>()
                .Which.RequestInformation.ExtendedErrorInformation.ErrorCode.Should()
                .Be(TableErrorCodeStrings.EntityAlreadyExists);
        }

        [Fact]
        public async Task Replace_WhenETagIsInvalid_Throws()
        {
            var qm1 = await storage.CreateAsync(GetQueryModel());

            // Update query model.
            qm1.Data = Unified.NewCode();
            await storage.ReplaceAsync(qm1);

            // Trying to update again should fail as ETag is changed after first update.
            (await storage.Awaiting(s => s.ReplaceAsync(qm1)).Should().ThrowAsync<StorageException>())
                .Which.RequestInformation.ExtendedErrorInformation.ErrorCode.Should()
                .Be(TableErrorCodeStrings.UpdateConditionNotSatisfied);
        }

        [Fact]
        public async Task Replace_QueryModelDoesntExist_Throws()
        {
            (await storage.Awaiting(s => s.ReplaceAsync(GetQueryModel())).Should().ThrowAsync<StorageException>())
                .Which.RequestInformation.ExtendedErrorInformation.ErrorCode.Should()
                .Be(StorageErrorCodeStrings.ResourceNotFound);
        }

        [Fact]
        public async Task Replace_WhenETagIsValid_Replaces()
        {
            var qm1 = await storage.CreateAsync(GetQueryModel());

            // Update for the first time.
            qm1.Data = Unified.NewCode();
            var updated1 = await storage.ReplaceAsync(qm1);

            // Update again with the valid changed ETag.
            updated1.Data = Unified.NewCode();
            var updated2 = await storage.ReplaceAsync(updated1);

            updated2.Should().BeEquivalentTo(updated1, options => options.ForMutableQueryModel());
        }

        [Fact]
        public async Task Delete_QueryModelExist_Deletes()
        {
            var queryModel = GetQueryModel();
            var created = await storage.CreateAsync(queryModel);

            await storage.DeleteAsync(created.Id);

            var deleted = await storage.FindAsync(queryModel.Id);

            deleted.Should().BeNull();
        }

        [Fact]
        public async Task Delete_NoQueryModel_DoesNothing()
        {
            await storage.Awaiting(s => s.DeleteAsync(Unified.NewCode())).Should().NotThrowAsync();
        }

        [Fact]
        public async Task GetAll_NoQueryModels_ReturnsEmpty()
        {
            (await storage.GetAllAsync()).Should().BeEmpty();
        }

        [Fact]
        public async Task GetAll_HasQueryModels_ReturnsAll()
        {
            var qm1 = await storage.CreateAsync(GetQueryModel());
            var qm2 = await storage.CreateAsync(GetQueryModel());

            (await storage.GetAllAsync()).Should()
                .BeEquivalentTo(new[] { qm1, qm2 }, options => options.ForMutableQueryModel());
        }

        [Fact]
        public async Task GetAllByIds_QueriesAll_ReturnsAll()
        {
            var numberOfQueryModels = SizeOfTheFilterChunk + 1; // To ensure that chunked query is working.

            // Create query models to query by ids.
            var queryModels = new List<FakeMutableQueryModel>();
            for (var i = 0; i < numberOfQueryModels; i++)
            {
                queryModels.Add(await storage.CreateAsync(GetQueryModel()));
            }

            // Add one more which is not part of the filter.
            await storage.CreateAsync(GetQueryModel());

            var results = await storage.GetAllAsync(queryModels.Select(qm => qm.Id).ToArray());

            // Ensure only queried models are returned.
            results.Should().BeEquivalentTo(queryModels, options => options.ForMutableQueryModel());
        }

        private static FakeMutableQueryModel GetQueryModel()
        {
            var queryModel = new Fixture().Create<FakeMutableQueryModel>();

            queryModel.ETag = $"W/\"datetime'{DateTime.UtcNow:O}'\"";

            return queryModel;
        }
    }
}