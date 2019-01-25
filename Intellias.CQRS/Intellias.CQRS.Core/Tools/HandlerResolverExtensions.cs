using Microsoft.Extensions.DependencyInjection;

namespace Intellias.CQRS.Core.Tools
{
    /// <summary>
    /// HandlerResolverExtensions
    /// </summary>
    public static class HandlerResolverExtensions
    {
        /// <summary>
        /// AddHandlerManager
        /// </summary>
        /// <param name="services"></param>
        public static void AddHandlerManager<T>(this IServiceCollection services)
        {
            services.AddTransient(_ =>
                new HandlerAssemblyResolver(() => typeof(T).Assembly));
            services.AddSingleton<HandlerDependencyResolver>();
            services.AddSingleton<HandlerManager>();
        }
    }
}
