using System;
using System.Collections.Generic;
using System.Linq;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Results;

namespace Intellias.CQRS.Core.Domain
{
    /// <inheritdoc />
    public class AggregateRoot<T> : IAggregateRoot where T : AggregateState, new()
    {
        private readonly List<IEvent> pendingEvents = new List<IEvent>();

        /// <summary>
        /// Current State of Aggregate
        /// </summary>
        public T State { get; } = new T();

        /// <inheritdoc />
        public string Id { get; protected set; }

        /// <inheritdoc />
        public IReadOnlyCollection<IEvent> Events => pendingEvents.AsReadOnly();

        /// <inheritdoc />
        public int Version => State.Version;

        /// <summary>
        /// Creates an existing aggregate-root
        /// </summary>
        /// <param name="id"></param>
        protected AggregateRoot(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            Id = id;
        }

        /// <inheritdoc />
        public void LoadFromHistory(IEnumerable<IEvent> history)
        {
            foreach (var e in history.OrderBy(e => e.Version))
            {
                // Event should always equal next state version
                if (e.Version != State.Version + 1)
                {
                    throw new DataMisalignedException($"Misaligned event version {e.Version} of {e.AggregateRootId} aggregate");
                }

                State.ApplyEvent(e);
            }
        }

        /// <summary>
        /// Apply an event
        /// </summary>
        /// <param name="event">Event</param>
        protected void PublishEvent(IEvent @event)
        {
            @event.AggregateRootId = Id;
            State.ApplyEvent(@event);
            pendingEvents.Add(@event);
        }

        /// <summary>
        /// Unhandled Error
        /// </summary>
        /// <param name="errorMessage">Error Message</param>
        /// <param name="ex"></param>
        /// <returns>Execution Result</returns>
        public IExecutionResult UnhandledError(string errorMessage, Exception ex = null)
        {
            return ExecutionResult.Failed(new ExecutionError(ErrorCodes.UnhandledError, this.GetType().FullName, errorMessage, ex));
        }

        /// <summary>
        /// Validation Failed
        /// </summary>
        /// <param name="errorMessage">Error Message</param>
        /// <returns>Execution Result</returns>
        public IExecutionResult ValidationFailed(string errorMessage)
        {
            return ExecutionResult.Failed(new ExecutionError(ErrorCodes.ValidationFailed, this.GetType().FullName, errorMessage));
        }

        /// <summary>
        /// Successful result
        /// </summary>
        /// <returns>Execution Result</returns>
        public IExecutionResult Success()
        {
            return ExecutionResult.Successful;
        }
    }
}
