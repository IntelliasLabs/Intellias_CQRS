using Product.Domain.Core.Messages;

namespace Product.Domain.Core.Commands
{
    /// <summary>
    /// Result of execution command handler
    /// </summary>
    public interface ICommandResult : IExecutionResult
    {
    }
}
