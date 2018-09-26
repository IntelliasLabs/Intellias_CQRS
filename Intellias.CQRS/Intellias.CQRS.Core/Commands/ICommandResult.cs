using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Commands
{
    /// <inheritdoc />
    /// <summary>
    /// Result of execution command handler
    /// </summary>
    public interface ICommandResult : IExecutionResult
    {
    }
}
