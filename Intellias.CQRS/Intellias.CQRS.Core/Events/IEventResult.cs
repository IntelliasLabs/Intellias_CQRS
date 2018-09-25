using Product.Domain.Core.Messages;

namespace Product.Domain.Core.Events
{
    /// <summary>
    /// Result of event handler execution
    /// </summary>
    public interface IEventResult : IExecutionResult
    {
    }
}
