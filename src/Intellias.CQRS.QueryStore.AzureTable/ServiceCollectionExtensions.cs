using System;
using Intellias.CQRS.Core.Queries.Immutable;
using Intellias.CQRS.Core.Queries.Mutable;
using Intellias.CQRS.Persistence.AzureStorage.Common;
using Intellias.CQRS.QueryStore.AzureTable.Immutable;
using Intellias.CQRS.QueryStore.AzureTable.Mutable;
using Intellias.CQRS.QueryStore.AzureTable.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MicrosoftOptions = Microsoft.Extensions.Options.Options;

namespace Intellias.CQRS.QueryStore.AzureTable
{
    /// <summary>
    /// Query Models Storage extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Query Models Table Storage services.
        /// </summary>
        /// <param name="services">Services collection.</param>
        /// <param name="configure">Configures <see cref="TableStorageOptions"/>.</param>
        /// <returns>Services collection Query Model Table Storage services.</returns>
        public static IServiceCollection AddTableQueryModelReader(this IServiceCollection services, Action<TableStorageOptions> configure)
        {
            // Add required services.
            services.AddTableQueryModelOptions(configure);

            // Register Table Storage services.
            services.AddSingleton(typeof(IMutableQueryModelReader<>), typeof(MutableQueryModelTableStorage<>));
            services.AddSingleton(typeof(IImmutableQueryModelReader<>), typeof(ImmutableQueryModelStorage<>));

            return services;
        }

        /// <summary>
        /// Adds Query Models Table Storage services.
        /// </summary>
        /// <param name="services">Services collection.</param>
        /// <param name="configure">Configures <see cref="TableStorageOptions"/>.</param>
        /// <returns>Services collection Query Model Table Storage services.</returns>
        public static IServiceCollection AddTableQueryModelStorage(this IServiceCollection services, Action<TableStorageOptions> configure)
        {
            // Add required services.
            services.AddTableQueryModelOptions(configure);

            // Register Table Storage services.
            services.AddSingleton(typeof(IMutableQueryModelReader<>), typeof(MutableQueryModelTableStorage<>));
            services.AddSingleton(typeof(IMutableQueryModelWriter<>), typeof(MutableQueryModelTableStorage<>));
            services.AddSingleton(typeof(IImmutableQueryModelReader<>), typeof(ImmutableQueryModelStorage<>));
            services.AddSingleton(typeof(IImmutableQueryModelWriter<>), typeof(ImmutableQueryModelStorage<>));

            return services;
        }

        /// <summary>
        /// Adds Query Models Table Storage services that uses <see cref="AzureTableSerializer"/>.
        /// </summary>
        /// <param name="services">Services collection.</param>
        /// <param name="configure">Configures <see cref="TableStorageOptions"/>.</param>
        /// <returns>Services collection Query Model Table Storage services.</returns>
        public static IServiceCollection AddTableQueryModelReader2(this IServiceCollection services, Action<DefaultTableStorageOptions> configure)
        {
            // Add required services.
            services.AddTableStorageOptions(configure);

            // Register Table Storage services.
            services.AddSingleton(typeof(IMutableQueryModelReader<>), typeof(MutableQueryModelTableStorage2<>));
            services.AddSingleton(typeof(IImmutableQueryModelReader<>), typeof(ImmutableQueryModelTableStorage2<>));

            return services;
        }

        /// <summary>
        /// Adds Query Models Table Storage services that uses <see cref="AzureTableSerializer"/>.
        /// </summary>
        /// <param name="services">Services collection.</param>
        /// <param name="configure">Configures <see cref="DefaultTableStorageOptions"/>.</param>
        /// <returns>Services collection Query Model Table Storage services.</returns>
        public static IServiceCollection AddTableQueryModelStorage2(this IServiceCollection services, Action<DefaultTableStorageOptions> configure)
        {
            // Add required services.
            services.AddTableStorageOptions(configure);

            // Register Table Storage services.
            services.AddSingleton(typeof(IMutableQueryModelReader<>), typeof(MutableQueryModelTableStorage2<>));
            services.AddSingleton(typeof(IMutableQueryModelWriter<>), typeof(MutableQueryModelTableStorage2<>));
            services.AddSingleton(typeof(IImmutableQueryModelReader<>), typeof(ImmutableQueryModelTableStorage2<>));
            services.AddSingleton(typeof(IImmutableQueryModelWriter<>), typeof(ImmutableQueryModelTableStorage2<>));

            return services;
        }

        private static void AddTableStorageOptions(this IServiceCollection services, Action<DefaultTableStorageOptions> configure)
        {
            services.AddSingleton<ITableStorageOptions>(sp =>
            {
                var options = new DefaultTableStorageOptions();
                configure(options);
                return options;
            });
        }

        private static void AddTableQueryModelOptions(this IServiceCollection services, Action<TableStorageOptions> configure)
        {
            // Add required services.
            services.AddOptions();

            // Register Table Storage options.
            services.Configure(configure);
            services.AddSingleton<IValidateOptions<TableStorageOptions>>(new DataAnnotationValidateOptions<TableStorageOptions>(MicrosoftOptions.DefaultName));
        }
    }
}