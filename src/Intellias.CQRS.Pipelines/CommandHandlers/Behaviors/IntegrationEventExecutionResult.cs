using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Results;

namespace Intellias.CQRS.Pipelines.CommandHandlers.Behaviors
{
    /// <summary>
    /// Integration event execution result.
    /// </summary>
    public class IntegrationEventExecutionResult : IExecutionResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrationEventExecutionResult"/> class.
        /// </summary>
        /// <param name="event">Value for <see cref="Event"/>.</param>
        public IntegrationEventExecutionResult(IIntegrationEvent @event)
        {
            Event = @event;
        }

        /// <summary>
        /// Integration event result.
        /// </summary>
        public IIntegrationEvent Event { get; }

        /// <inheritdoc />
        public bool Success => true;
    }
}