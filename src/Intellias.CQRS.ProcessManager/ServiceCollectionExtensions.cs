using System;
using System.Linq;
using System.Reflection;
using Intellias.CQRS.ProcessManager.Pipelines;
using Intellias.CQRS.ProcessManager.Pipelines.Middlewares;
using Intellias.CQRS.ProcessManager.Stores;
using Intellias.CQRS.QueryStore.AzureTable.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Intellias.CQRS.ProcessManager
{
    /// <summary>
    /// Pipeline service collection extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers process handlers.
        /// </summary>
        /// <param name="services">Services collection.</param>
        /// <param name="handlersAssemby">Assembly where to look for process manger handlers.</param>
        /// <returns>Service collection with registered types.</returns>
        public static IServiceCollection AddProcessHandlers(
            this IServiceCollection services,
            Assembly handlersAssemby)
        {
            foreach (var handler in handlersAssemby.GetTypes()
                .Where(s => s.IsClass && !s.IsAbstract && s.GetInterfaces().Any(i => i.Name == typeof(IProcessHandler<>).Name)))
            {
                services.AddTransient(handler);
            }

            return services
                .AddTransient(typeof(IProcessMiddleware<,>), typeof(PersistMessagesMiddleware<,>))
                .AddTransient(typeof(IProcessMiddleware<,>), typeof(PublishMessagesMiddleware<,>))
                .AddTransient(typeof(IProcessMiddleware<,>), typeof(PrepareMessagesMiddleware<,>))
                .AddTransient(typeof(ProcessPipelineExecutor<,>))
                .AddTransient<ProcessHandlerExecutor>();
        }

        /// <summary>
        /// Add process store.
        /// </summary>
        /// <param name="services">Services collection.</param>
        /// <param name="configure">Configures <see cref="TableStorageOptions"/>.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddProcessStore(
            this IServiceCollection services, Action<TableStorageOptions> configure)
        {
            return services
                .AddOptions()
                .Configure(configure)
                .AddSingleton(typeof(IProcessStore<>), typeof(ProcessStore<>));
        }
    }
}
