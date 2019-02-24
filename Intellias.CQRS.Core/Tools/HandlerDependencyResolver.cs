using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly HandlerAssemblyResolver assemblyResolver;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="assemblyResolver"></param>
        public HandlerDependencyResolver(
            IServiceProvider serviceProvider,
            HandlerAssemblyResolver assemblyResolver)
        {
            this.serviceProvider = serviceProvider;
            this.assemblyResolver = assemblyResolver;
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

            return Select<IEventHandler<T>> (handlerType);
        }

        /// <summary>
        /// Resolves all event handlers
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public IEnumerable<ICommandHandler<T>> ResolveCommand<T>(T command)
            where T : ICommand
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var handlerType = typeof(ICommandHandler<T>);

            return Select<ICommandHandler<T>>(handlerType);
        }

        private IEnumerable<THandlerType> Select<THandlerType>(Type handlerType) => 
            assemblyResolver
                .Assembly
                .GetTypes()
                .Where(handlerType.IsAssignableFrom)
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
