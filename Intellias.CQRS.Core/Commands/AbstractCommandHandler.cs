using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Core.Commands
{
    /// <summary>
    /// Abstract command handler with basic functionality.
    /// </summary>
    public abstract class AbstractCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractCommandHandler"/> class.
        /// </summary>
        /// <param name="store">Events Store.</param>
        /// <param name="bus">Event Bus.</param>
        protected AbstractCommandHandler(IEventStore store, IEventBus bus)
        {
            this.Store = store;
            this.Bus = bus;
        }

        /// <summary>
        /// Event store instance.
        /// </summary>
        protected IEventStore Store { get; }

        /// <summary>
        /// Event bus instance.
        /// </summary>
        protected IEventBus Bus { get; }

        /// <summary>
        /// Get existing Aggregate Root from store.
        /// </summary>
        /// <typeparam name="T">type of Aggregate Root.</typeparam>
        /// <typeparam name="TR">type of Aggregate State.</typeparam>
        /// <param name="aggregateRootId">Id of Aggreagate root to be loaded.</param>
        /// <returns>Aggregate.</returns>
        /// <exception cref="KeyNotFoundException">Aggregate Root Id not found in event store.</exception>
        /// <exception cref="ArgumentNullException">Aggregate should have constructor with string parameter that is id of AR.</exception>
        protected async Task<T> GetAggregateAsync<T, TR>(string aggregateRootId)
            where T : AggregateRoot<TR>
            where TR : AggregateState, new()
        {
            var type = typeof(T);
            var ctor = type.GetConstructor(new[] { typeof(string) })
                ?? throw new ArgumentNullException($"'{typeof(T)}' should have constructor with string parameter that is id of AR");

            var aggregateRoot = (T)ctor.Invoke(new object[] { aggregateRootId });

            await FillAggregateHistoryAsync(aggregateRoot);

            return aggregateRoot;
        }

        private async Task FillAggregateHistoryAsync(IAggregateRoot aggregate)
        {
            var history = await Store.GetAsync(aggregate.Id, 0);
            aggregate.LoadFromHistory(history);
        }
    }
}
