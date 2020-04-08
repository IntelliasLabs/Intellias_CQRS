using System.Linq;
using System.Reflection;
using Intellias.CQRS.ProcessManager.Pipelines;
using Intellias.CQRS.ProcessManager.Pipelines.Middlewares;
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
                .AddTransient(typeof(IProcessMiddleware<>), typeof(PersistMessageMiddleware<>))
                .AddTransient(typeof(IProcessMiddleware<>), typeof(PublishMessageMiddleware<>))
                .AddTransient(typeof(IProcessMiddleware<>), typeof(ReplayMessageMiddleware<>))
                .AddTransient(typeof(ProcessPipelineExecutor<>))
                .AddTransient<ProcessHandlerExecutor>();
        }
    }
}
