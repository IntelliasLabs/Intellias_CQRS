using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Results;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <inheritdoc />
    public class InProcessCommandBus : ICommandBus
    {
        private readonly Dictionary<Type, ICommandHandler<ICommand>> funcs;

        /// <summary>
        /// Creates command bus
        /// </summary>
        public InProcessCommandBus()
        {
            funcs = new Dictionary<Type, ICommandHandler<ICommand>>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="handler">command handler</param>
        /// <typeparam name="T"></typeparam>
        public void AddHandler<T>(ICommandHandler<T> handler) where T : ICommand
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var abstractHandler = (ICommandHandler<ICommand>)new CommandHandlerWrapper<T>(handler);

            if (funcs.ContainsKey(typeof(T)))
            {
                throw new InvalidOperationException($"Command Handler for command {typeof(T)} already exists");
            }
            else
            {
                funcs.Add(typeof(T), abstractHandler);
            }
        }

        /// <summary>
        /// Register all implementations of Command handlers
        /// </summary>
        /// <param name="commandHandler">command handler</param>
        public void AddAllHandlers(AbstractCommandHandler commandHandler)
        {
            if (commandHandler == null)
            {
                throw new ArgumentNullException(nameof(commandHandler));
            }

            // Here we werify that we take all interfaces ICommandHandler<ICommand>
            var interfaces = commandHandler.GetType().GetInterfaces()
                .Where(i => i.GetGenericTypeDefinition() == typeof(ICommandHandler<>));

            foreach (var t in interfaces)
            {
                var commandType = t.GenericTypeArguments.Single();

                var constructedWrapper = typeof(CommandHandlerWrapper<>).MakeGenericType(commandType);
                var abstractHandler = (ICommandHandler<ICommand>)Activator.CreateInstance(constructedWrapper, commandHandler);

                funcs.Add(commandType, abstractHandler);
            }
        }

        /// <inheritdoc />
        public async Task<IExecutionResult> PublishAsync(ICommand msg)
        {
            var func = funcs[msg.GetType()];

            var result = await func.HandleAsync(msg);

            return await Task.FromResult(result);
        }
    }
}
