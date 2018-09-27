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

        #region Private members

        
        private readonly List<IEvent> pendingEvents = new List<IEvent>();
        private readonly Dictionary<Type, Action<IEvent>> handlers = new Dictionary<Type, Action<IEvent>>();

        /// <inheritdoc />
        public int Version { get; private set; }


        #endregion

        #region Public members


        /// <inheritdoc />
        public string Id { get; }

        /// <inheritdoc />
        public ReadOnlyCollection<IEvent> Events => pendingEvents.AsReadOnly();


        #endregion

        #region Constructors


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


        #endregion



     
        /// <inheritdoc />
        public void LoadFromHistory(IEnumerable<IEvent> pastEvents)
        {
            foreach (var e in pastEvents.OrderBy(e=>e.Version))
            {
                if (e.Version != ++Version)
                {
                    throw new EventsOutOfOrderException(e.AggregateRootId);
                }
                handlers[e.GetType()].Invoke(e);
            }
        }


        /// <summary>
        /// Configures a handler for an event. 
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
        protected void ApplyChange(IEvent @event)
        {
            @event.Version = ++Version;
            handlers[@event.GetType()].Invoke(@event);
            pendingEvents.Add(@event);
        }
    }
}
