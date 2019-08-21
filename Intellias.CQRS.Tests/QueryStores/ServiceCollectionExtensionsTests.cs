using FluentAssertions;
using Intellias.CQRS.Core.Queries.Immutable;
using Intellias.CQRS.Core.Queries.Mutable;
using Intellias.CQRS.QueryStore.AzureTable;
using Intellias.CQRS.QueryStore.AzureTable.Options;
using Intellias.CQRS.Tests.Core.Queries;
using Intellias.CQRS.Tests.Utils;
using Intellias.CQRS.Tests.Utils.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Intellias.CQRS.Tests.QueryStores
{
    public class ServiceCollectionExtensionsTests : StorageAccountTestBase
    {
        private readonly ServiceProvider serviceProvider;

        public ServiceCollectionExtensionsTests(StorageAccountFixture fixture)
            : base(fixture)
        {
            serviceProvider = new ServiceCollection()
                .AddTableQueryModelStorage(o =>
                {
                    o.ConnectionString = fixture.Configuration.StorageAccount.ConnectionString;
                    o.TableNamePrefix = fixture.ExecutionContext.GetSessionPrefix();
                })
                .BuildServiceProvider();
        }

        [Fact]
        public void ServiceProvider_Always_HasConfiguredOptions()
        {
            var options = serviceProvider.GetRequiredService<IOptionsMonitor<TableStorageOptions>>();

            options.CurrentValue.Should().BeEquivalentTo(new TableStorageOptions
            {
                ConnectionString = Configuration.StorageAccount.ConnectionString,
                TableNamePrefix = ExecutionContext.GetSessionPrefix()
            });
        }

        [Fact]
        public void ServiceProvider_Always_HasRegisteredMutableQueryModelsStorage()
        {
            serviceProvider.Invoking(sp => sp.GetRequiredService<IMutableQueryModelWriter<MutableQueryModel>>()).Should().NotThrow();
            serviceProvider.Invoking(sp => sp.GetRequiredService<IMutableQueryModelReader<MutableQueryModel>>()).Should().NotThrow();
        }

        [Fact]
        public void ServiceProvider_Always_HasRegisteredImmutableQueryModelsStorage()
        {
            serviceProvider.Invoking(sp => sp.GetRequiredService<IImmutableQueryModelWriter<ImmutableQueryModel>>()).Should().NotThrow();
            serviceProvider.Invoking(sp => sp.GetRequiredService<IImmutableQueryModelReader<ImmutableQueryModel>>()).Should().NotThrow();
        }
    }
}