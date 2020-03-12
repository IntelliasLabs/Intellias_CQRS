using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Core.Security;

namespace Intellias.CQRS.Core.Commands
{
    /// <inheritdoc />
    /// <summary>
    /// Domain command interface.
    /// </summary>
    public interface ICommand : IMessage
    {
        /// <summary>
        /// Expected version of aggregate root.
        /// </summary>
        int ExpectedVersion { get; }

        /// <summary>
        /// Contains a security context in which message is handled.
        /// </summary>
        Principal Principal { get; }

        /// <summary>
        /// Validate Command.
        /// </summary>
        /// <returns>Execution Result.</returns>
        IExecutionResult Validate();
    }
}
