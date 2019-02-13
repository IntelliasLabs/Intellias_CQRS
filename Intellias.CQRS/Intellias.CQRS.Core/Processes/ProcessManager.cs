﻿using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Core.Processes
{
    /// <inheritdoc />
    public class ProcessManager<T> : IProcessManager<T> where T : class, IProcess
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="commandBus"></param>
        /// <param name="eventBus"></param>
        public ProcessManager(
            ICommandBus commandBus, 
            IEventBus eventBus)
        {
            CommandBus = commandBus;
            EventBus = eventBus;
        }

        /// <summary>
        /// CommandBus
        /// </summary>
        protected ICommandBus CommandBus { get; }

        /// <summary>
        /// EventBus
        /// </summary>
        protected IEventBus EventBus { get; }

        /// <inheritdoc />
        public async Task ApplyAsync(T process)
        {
            // send all commands
            await Task.WhenAll(process.Commands.Select(command => CommandBus.PublishAsync(command)));
            // send all events
            await Task.WhenAll(process.Events.Select(@event => EventBus.PublishAsync(@event)));
        }
    }
}