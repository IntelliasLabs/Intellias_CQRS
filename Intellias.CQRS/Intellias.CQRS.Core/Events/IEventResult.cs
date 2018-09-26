using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Events
{
    /// <inheritdoc />
    /// <summary>
    /// Result of event handler execution
    /// </summary>
    public interface IEventResult : IExecutionResult
    {
    }
}
