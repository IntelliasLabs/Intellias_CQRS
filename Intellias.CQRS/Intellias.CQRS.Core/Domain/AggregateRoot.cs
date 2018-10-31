using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Intellias.CQRS.Core.Domain.Exceptions;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Core.Domain
{
    /// <inheritdoc />
    public class AggregateRoot : IAggregateRoot
    {
        private readonly List<IEvent> pendingEvents = new List<IEvent>();
        private readonly Dictionary<Type, Action<IEvent>> handlers = new Dictionary<Type, Action<IEvent>>();

        /// <inheritdoc />
        public string Id { get; protected set; }
        /// <inheritdoc />
        public int Version { get; private set; }
        /// <inheritdoc />
        public ReadOnlyCollection<IEvent> Events => pendingEvents.AsReadOnly();

        /// <summary>
        /// Call to empty constructor assembles brand-new Aggregate-root entity
        /// </summary>
        protected AggregateRoot()
        {
            Id = Unified.NewCode();
        }

        /// <summary>
        /// Creates an existing aggregate-root
        /// </summary>
        /// <param name="id"></param>
        protected AggregateRoot(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = Unified.NewCode();
            }

            Id = id;
        }        

        /// <inheritdoc />
        public void LoadFromHistory(IEnumerable<IEvent> history)
        {
            foreach (var e in history.OrderBy(e=>e.Version))
            {
                if (e.Version != ++Version)
                {
                    throw new EventsOutOfOrderException(e.AggregateRootId);
                }
                handlers[e.GetType()].Invoke(e);
            }
        }

        /// <summary>
        /// Configures a handler method for an event. 
        /// </summary>
        protected void Handles<TEvent>(Action<TEvent> handler)
            where TEvent : IEvent
        {
            handlers.Add(typeof(TEvent), @event => handler((TEvent)@event));
        }

        /// <summary>
        /// Apply an event
        /// </summary>
        /// <param name="event">Event</param>
        private void ApplyEvent(IEvent @event)
        {
            @event.Version = ++Version;
            handlers[@event.GetType()].Invoke(@event);
        }

        /// <summary>
        /// Apply an event
        /// </summary>
        /// <param name="event">Event</param>
        protected void PublishEvent(IEvent @event)
        {
            @event.AggregateRootId = Id;
            ApplyEvent(@event);
            pendingEvents.Add(@event);
        }
    }
}
