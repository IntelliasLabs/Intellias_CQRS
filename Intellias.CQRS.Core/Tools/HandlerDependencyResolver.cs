using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Core.Tools
{
    /// <summary>
    /// Used to return all handler instanses from handler's assembly
    /// </summary>
    public class HandlerDependencyResolver
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IEnumerable<Assembly> assemblies;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="assemblies"></param>
        public HandlerDependencyResolver(
            IServiceProvider serviceProvider,
            IEnumerable<Assembly> assemblies)
        {
            this.serviceProvider = serviceProvider;
            this.assemblies = assemblies;
        }


        /// <summary>
        /// Resolves all event handlers
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
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
        /// Resolves all event handlers
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
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
