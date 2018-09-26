using Intellias.CQRS.Core.Messages;

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
    }
}
