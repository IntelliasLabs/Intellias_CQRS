﻿using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Results;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <inheritdoc />
    public class CommandHandlerWrapper<T> : ICommandHandler<ICommand>
        where T : ICommand
    {
        private readonly ICommandHandler<T> handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandlerWrapper{T}"/> class.
        /// </summary>
        /// <param name="handler">Command Handler.</param>
        public CommandHandlerWrapper(ICommandHandler<T> handler)
        {
            this.handler = handler;
        }

        /// <inheritdoc />
        public Task<IExecutionResult> HandleAsync(ICommand command)
        {
            return handler.HandleAsync((T)command);
        }
    }
}
