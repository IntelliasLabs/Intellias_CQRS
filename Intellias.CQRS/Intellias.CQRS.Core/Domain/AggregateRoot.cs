using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Intellias.CQRS.Core.Domain.Exceptions;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Core.Domain
{
    /// <inheritdoc />
    public class AggregateRoot<T> : IAggregateRoot where T : AggregateState, new()
    {
        private readonly List<IEvent> pendingEvents = new List<IEvent>();

        /// <summary>
        /// Current State of Aggregate
        /// </summary>
        protected T State { get; } = new T();

        /// <inheritdoc />
        public string Id { get; protected set; }

        /// <inheritdoc />
        public ReadOnlyCollection<IEvent> Events => pendingEvents.AsReadOnly();

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
                throw new NullReferenceException("AR Id can not be null");
            }

            Id = id;
        }        

        /// <inheritdoc />
        public void LoadFromHistory(IEnumerable<IEvent> history)
        {
            foreach (var e in history.OrderBy(e=>e.Version))
            {
                // Event should always equal next state version
                if (e.Version != State.Version + 1)
                {
                    throw new EventsOutOfOrderException(e.AggregateRootId);
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
    }
}
