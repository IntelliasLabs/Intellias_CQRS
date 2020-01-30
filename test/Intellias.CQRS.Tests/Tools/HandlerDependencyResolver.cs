using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Tests.Tools
{
    /// <summary>
    /// Used to return all handler instances from handler's assembly.
    /// </summary>
    public class HandlerDependencyResolver
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IEnumerable<Assembly> assemblies;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDependencyResolver"/> class.
        /// </summary>
        /// <param name="serviceProvider">Service Provider.</param>
        /// <param name="assemblies">Collection of assemblies.</param>
        public HandlerDependencyResolver(
            IServiceProvider serviceProvider,
            IEnumerable<Assembly> assemblies)
        {
            this.serviceProvider = serviceProvider;
            this.assemblies = assemblies;
        }

        /// <summary>
        /// Resolves all event handlers.
        /// </summary>
        /// <typeparam name="T">Event Type.</typeparam>
        /// <param name="event">Event.</param>
        /// <returns>Event Hanlder.</returns>
        public IEnumerable<IEventHandler<T>> ResolveEvent<T>(T @event)
            where T : IEvent
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            var handlerType = typeof(IEventHandler<T>);

            return Select<IEventHandler<T>>(handlerType);
        }

        /// <summary>
        /// Resolves all event handlers.
        /// </summary>
        /// <param name="command">Command.</param>
        /// <typeparam name="T">Command Type.</typeparam>
        /// <returns>Command Handler.</returns>
        public ICommandHandler<T> ResolveCommand<T>(T command)
            where T : ICommand
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var handlerType = typeof(ICommandHandler<T>);

            return Select<ICommandHandler<T>>(handlerType).First();
        }

        private IEnumerable<THandlerType> Select<THandlerType>(Type handlerType) =>
            assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.GetInterfaces().Contains(handlerType))
            .Select(type =>
            {
                var service = (THandlerType)serviceProvider.GetService(type);

                if (service == null)
                {
                    throw new ArgumentNullException($"'{nameof(service)}'. Type '{type.Name}' cannot be resolved by service provider.");
                }

                return service;
            });
    }
}
