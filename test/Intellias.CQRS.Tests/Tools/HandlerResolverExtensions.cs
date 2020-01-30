using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Intellias.CQRS.Tests.Tools
{
    /// <summary>
    /// HandlerResolverExtensions.
    /// </summary>
    public static class HandlerResolverExtensions
    {
        /// <summary>
        /// Adds Handler Dependency Resolver with HandlerManager for resolving all event and command handlers functions invoking.
        /// </summary>
        /// <param name="services">Services.</param>
        /// <param name="assemblies">Assemblies.</param>
        public static void AddHandlerManager(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            services.AddSingleton(assemblies);
            services.AddSingleton<HandlerDependencyResolver>();
            services.AddSingleton<HandlerManager>();
        }

        /// <summary>
        /// Adds Handler Dependency Resolver with HandlerManager for resolving all event and command handlers functions invoking.
        /// </summary>
        /// <param name="services">Services.</param>
        /// <param name="assembly">Assembly.</param>
        public static void AddHandlerManager(this IServiceCollection services, Assembly assembly)
            => AddHandlerManager(services, new[] { assembly });
    }
}
