using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Commands
{
    public interface ICommandHandler<in T> : IHandler<T, ICommandResult> 
        where T : ICommand
    {}
}
