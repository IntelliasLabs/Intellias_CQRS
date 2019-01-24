using Microsoft.Extensions.DependencyInjection;

namespace Intellias.CQRS.Core.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public static class EventHandlerResolverExtensions
    {
        /// <summary>
        /// AddHandlerManager
        /// </summary>
        /// <param name="services"></param>
        public static void AddHandlerManager<T>(this IServiceCollection services)
        {
            services.AddTransient(_ =>
                new EventHandlerAssemblyResolver(() => typeof(T).Assembly));
            services.AddSingleton<EventHandlerDependencyResolver>();
        }
    }
}
