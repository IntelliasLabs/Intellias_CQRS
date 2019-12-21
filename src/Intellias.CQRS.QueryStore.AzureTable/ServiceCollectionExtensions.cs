using System;
using Intellias.CQRS.Core.Queries.Immutable.Interfaces;
using Intellias.CQRS.Core.Queries.Mutable;
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
        public static IServiceCollection AddTableQueryModelStorage(this IServiceCollection services, Action<TableStorageOptions> configure)
        {
            // Add required services.
            services.AddOptions();

            // Register Table Storage services.
            services.AddSingleton(typeof(IMutableQueryModelReader<>), typeof(MutableQueryModelTableStorage<>));
            services.AddSingleton(typeof(IMutableQueryModelWriter<>), typeof(MutableQueryModelTableStorage<>));
            services.AddSingleton(typeof(IImmutableQueryModelReader<>), typeof(ImmutableQueryModelTableStorage<>));
            services.AddSingleton(typeof(IImmutableQueryModelWriter<>), typeof(ImmutableQueryModelTableStorage<>));

            // Register Table Storage options.
            services.Configure(configure);
            services.AddSingleton<IValidateOptions<TableStorageOptions>>(new DataAnnotationValidateOptions<TableStorageOptions>(MicrosoftOptions.DefaultName));

            return services;
        }
    }
}