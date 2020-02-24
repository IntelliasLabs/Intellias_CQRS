using System;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Messaging.AzureServiceBus.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Intellias.CQRS.Messaging.AzureServiceBus
{
    /// <summary>
    /// Contains extensions to Service Collection for messaging abstractions based on Azure Service Bus.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers Command Bus.
        /// </summary>
        /// <param name="services">Services collection.</param>
        /// <param name="configure">Configures Command Bus options.</param>
        /// <typeparam name="TCommandBusOptions">Command Bus options type.</typeparam>
        /// <returns>Service collection with registered Command Bus.</returns>
        public static IServiceCollection AddCommandBus<TCommandBusOptions>(this IServiceCollection services, Action<TCommandBusOptions> configure)
            where TCommandBusOptions : class, ICommandBusOptions, new()
        {
            services.AddOptions();
            services.Configure(configure);

            services.AddTransient(sp => sp.GetRequiredService<IOptionsMonitor<TCommandBusOptions>>().CurrentValue);
            services.AddSingleton<ICommandBus<TCommandBusOptions>, QueueCommandBus<TCommandBusOptions>>();

            return services;
        }
    }
}