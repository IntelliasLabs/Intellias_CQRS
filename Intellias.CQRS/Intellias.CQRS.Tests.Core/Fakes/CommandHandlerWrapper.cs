using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <inheritdoc />
    public class CommandHandlerWrapper<T> : ICommandHandler<ICommand> where T : ICommand
    {
        private readonly ICommandHandler<T> handler;

        /// <summary>
        /// Constructs handler wrapper from func
        /// </summary>
        /// <param name="handler"></param>
        public CommandHandlerWrapper(ICommandHandler<T> handler)
        {
            this.handler = handler;
        }

        /// <inheritdoc />
        public Task<ICommandResult> HandleAsync(ICommand command)
        {
            return handler.HandleAsync((T)command);
        }
    }
}
