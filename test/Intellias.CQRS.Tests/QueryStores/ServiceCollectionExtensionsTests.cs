using System;
using FluentAssertions;
using Intellias.CQRS.Core.Queries.Immutable;
using Intellias.CQRS.Core.Queries.Mutable;
using Intellias.CQRS.Persistence.AzureStorage.Common;
using Intellias.CQRS.QueryStore.AzureTable;
using Intellias.CQRS.QueryStore.AzureTable.Options;
using Intellias.CQRS.Tests.Core.Queries;
using Intellias.CQRS.Tests.Utils.StorageAccount;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Intellias.CQRS.Tests.QueryStores
{
    public class ServiceCollectionExtensionsTests : StorageAccountTestBase
    {
        private readonly StorageAccountFixture fixture;

        public ServiceCollectionExtensionsTests(StorageAccountFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void AddTableQueryModelStorage_Always_HasConfiguredOptions()
        {
            var serviceProvider = RegisterStorage();
            var options = serviceProvider.GetRequiredService<IOptionsMonitor<TableStorageOptions>>();

            options.CurrentValue.Should().BeEquivalentTo(new TableStorageOptions
            {
                ConnectionString = Configuration.StorageAccount.ConnectionString,
                TableNamePrefix = ExecutionContext.GetSessionPrefix()
            });
        }

        [Fact]
        public void AddTableQueryModelReader_Always_HasConfiguredOptions()
        {
            var serviceProvider = RegisterReader();
            var options = serviceProvider.GetRequiredService<IOptionsMonitor<TableStorageOptions>>();

            options.CurrentValue.Should().BeEquivalentTo(new TableStorageOptions
            {
                ConnectionString = Configuration.StorageAccount.ConnectionString,
                TableNamePrefix = ExecutionContext.GetSessionPrefix()
            });
        }

        [Fact]
        public void AddTableQueryModelStorage_Always_HasRegisteredQueryModelsStorage()
        {
            var serviceProvider = RegisterStorage();
            serviceProvider.Invoking(sp => sp.GetRequiredService<IMutableQueryModelWriter<FakeMutableQueryModel>>()).Should().NotThrow();
            serviceProvider.Invoking(sp => sp.GetRequiredService<IMutableQueryModelReader<FakeMutableQueryModel>>()).Should().NotThrow();
            serviceProvider.Invoking(sp => sp.GetRequiredService<IImmutableQueryModelWriter<FakeImmutableQueryModel>>()).Should().NotThrow();
            serviceProvider.Invoking(sp => sp.GetRequiredService<IImmutableQueryModelReader<FakeImmutableQueryModel>>()).Should().NotThrow();
        }

        [Fact]
        public void AddTableQueryModelReader_Always_HasRegisteredOnlyQueryModelsReader()
        {
            var serviceProvider = RegisterReader();
            serviceProvider.Invoking(sp => sp.GetRequiredService<IMutableQueryModelReader<FakeMutableQueryModel>>()).Should().NotThrow();
            serviceProvider.Invoking(sp => sp.GetRequiredService<IImmutableQueryModelReader<FakeImmutableQueryModel>>()).Should().NotThrow();
            serviceProvider.GetService<IMutableQueryModelWriter<FakeMutableQueryModel>>().Should().BeNull();
            serviceProvider.GetService<IImmutableQueryModelWriter<FakeImmutableQueryModel>>().Should().BeNull();
        }

        [Fact]
        public void AddTableQueryModelStorage2_Always_HasConfiguredOptions()
        {
            var serviceProvider = RegisterStorage2();

            serviceProvider.GetRequiredService<ITableStorageOptions>().Should().BeEquivalentTo(new DefaultTableStorageOptions
            {
                ConnectionString = Configuration.StorageAccount.ConnectionString,
                TableNamePrefix = ExecutionContext.GetSessionPrefix()
            });
        }

        [Fact]
        public void AddTableQueryModelReader2_Always_HasConfiguredOptions()
        {
            var serviceProvider = RegisterReader2();

            serviceProvider.GetRequiredService<ITableStorageOptions>().Should().BeEquivalentTo(new DefaultTableStorageOptions
            {
                ConnectionString = Configuration.StorageAccount.ConnectionString,
                TableNamePrefix = ExecutionContext.GetSessionPrefix()
            });
        }

        [Fact]
        public void AddTableQueryModelStorage2_Always_HasRegisteredQueryModelsStorage()
        {
            var serviceProvider = RegisterStorage2();
            serviceProvider.Invoking(sp => sp.GetRequiredService<IMutableQueryModelWriter<FakeMutableQueryModel>>()).Should().NotThrow();
            serviceProvider.Invoking(sp => sp.GetRequiredService<IMutableQueryModelReader<FakeMutableQueryModel>>()).Should().NotThrow();
            serviceProvider.Invoking(sp => sp.GetRequiredService<IImmutableQueryModelWriter<FakeImmutableQueryModel>>()).Should().NotThrow();
            serviceProvider.Invoking(sp => sp.GetRequiredService<IImmutableQueryModelReader<FakeImmutableQueryModel>>()).Should().NotThrow();
        }

        [Fact]
        public void AddTableQueryModelReader2_Always_HasRegisteredOnlyQueryModelsReader()
        {
            var serviceProvider = RegisterReader2();
            serviceProvider.Invoking(sp => sp.GetRequiredService<IMutableQueryModelReader<FakeMutableQueryModel>>()).Should().NotThrow();
            serviceProvider.Invoking(sp => sp.GetRequiredService<IImmutableQueryModelReader<FakeImmutableQueryModel>>()).Should().NotThrow();
            serviceProvider.GetService<IMutableQueryModelWriter<FakeMutableQueryModel>>().Should().BeNull();
            serviceProvider.GetService<IImmutableQueryModelWriter<FakeImmutableQueryModel>>().Should().BeNull();
        }

        private IServiceProvider RegisterReader()
        {
            return new ServiceCollection()
                .AddTableQueryModelReader(o =>
                {
                    o.ConnectionString = fixture.Configuration.StorageAccount.ConnectionString;
                    o.TableNamePrefix = fixture.ExecutionContext.GetSessionPrefix();
                })
                .BuildServiceProvider();
        }

        private IServiceProvider RegisterStorage()
        {
            return new ServiceCollection()
                .AddTableQueryModelStorage(o =>
                {
                    o.ConnectionString = fixture.Configuration.StorageAccount.ConnectionString;
                    o.TableNamePrefix = fixture.ExecutionContext.GetSessionPrefix();
                })
                .BuildServiceProvider();
        }

        private IServiceProvider RegisterReader2()
        {
            return new ServiceCollection()
                .AddTableQueryModelReader2(o =>
                {
                    o.ConnectionString = fixture.Configuration.StorageAccount.ConnectionString;
                    o.TableNamePrefix = fixture.ExecutionContext.GetSessionPrefix();
                })
                .BuildServiceProvider();
        }

        private IServiceProvider RegisterStorage2()
        {
            return new ServiceCollection()
                .AddTableQueryModelStorage2(o =>
                {
                    o.ConnectionString = fixture.Configuration.StorageAccount.ConnectionString;
                    o.TableNamePrefix = fixture.ExecutionContext.GetSessionPrefix();
                })
                .BuildServiceProvider();
        }
    }
}