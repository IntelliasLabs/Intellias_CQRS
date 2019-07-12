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
            if(@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            if(string.IsNullOrWhiteSpace(@event.AggregateRootId))
            {
                @event.AggregateRootId = Id;
            }

            State.ApplyEvent(@event);
            pendingEvents.Add(@event);
        }

        /// <summary>
        /// Unhandled Error
        /// </summary>
        /// <param name="errorMessage">Error Message</param>
        /// <returns>Execution Result</returns>
        public IExecutionResult UnhandledError(string errorMessage)
        {
            return new FailedResult(ErrorCodes.UnhandledError, GetType().FullName, errorMessage);
        }

        /// <summary>
        /// Access Denied
        /// </summary>
        /// <param name="errorMessage">Error Message</param>
        /// <returns>Execution Result</returns>
        public IExecutionResult AccessDenied(string errorMessage)
        {
            return new FailedResult(ErrorCodes.AccessDenied, GetType().FullName, errorMessage);
        }

        /// <summary>
        /// Validation Failed
        /// </summary>
        /// <param name="errorMessage">Error Message</param>
        /// <returns>Execution Result</returns>
        public IExecutionResult ValidationFailed(string errorMessage)
        {
            return new FailedResult(ErrorCodes.ValidationFailed, GetType().FullName, errorMessage);
        }

        /// <summary>
        /// Failed custom domain logic
        /// </summary>
        /// <param name="errorCode">Specific error code</param>
        /// <param name="errorMessage">Error Message</param>
        /// <returns>Execution Result</returns>
        public IExecutionResult Failed(string errorCode, string errorMessage)
        {
            return new FailedResult(errorCode, GetType().FullName, errorMessage);
        }

        /// <summary>
        /// Successful result
        /// </summary>
        /// <returns>Execution Result</returns>
        public IExecutionResult Success()
        {
            return new SuccessfulResult();
        }
    }
}
