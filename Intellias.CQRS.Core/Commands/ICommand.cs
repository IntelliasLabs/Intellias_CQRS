using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Results;

namespace Intellias.CQRS.Core.Commands
{
    /// <inheritdoc />
    /// <summary>
    /// Domain command interface
    /// </summary>
    public interface ICommand : IMessage
    {
        /// <summary>
        /// Expected version of aggregate root
        /// </summary>
        int ExpectedVersion { get; set; }

        /// <summary>
        /// Validate Command
        /// </summary>
        /// <returns>Execution Result</returns>
        IExecutionResult Validate();
    }
}
