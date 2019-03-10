using System;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Core.Commands
{
    /// <summary>
    /// Abstract command handler with basic functionality
    /// </summary>
    public abstract class AbstractCommandHandler
    {
        /// <summary>
        /// Lazy event store instance
        /// </summary>
        protected IEventStore store { get; }

        /// <summary>
        /// Initialize base command handler
        /// </summary>
        /// <param name="store"></param>
        protected AbstractCommandHandler(IEventStore store)
        {
            this.store = store;
        }

        /// <summary>
        /// Get existing Aggregate Root from store
        /// </summary>
        /// <typeparam name="T">type of Aggregate Root</typeparam>
        /// <typeparam name="TR">type of Aggregate State</typeparam>
        /// <param name="aggregateRootId">Id of Aggreagate root to be loaded</param>
        /// <returns></returns>
        protected async Task<T> GetAggregateAsync<T, TR>(string aggregateRootId)
            where T: AggregateRoot<TR>
            where TR: AggregateState, new()
        {
            var type = typeof(T);
            var ctor = type.GetConstructor(new[] { typeof(string) })
                ?? throw new ArgumentNullException($"'{typeof(T)}' should have constructor with string parameter that is id of AR");

            var aggregateRoot = (T)ctor.Invoke(new object[] { aggregateRootId });

            await FillAggregateHistoryAsync(aggregateRoot);

            return aggregateRoot;
        }

        private async Task<IAggregateRoot> FillAggregateHistoryAsync(IAggregateRoot aggregate)
        {
            var history = await store.GetAsync(aggregate.Id, 0);
            aggregate.LoadFromHistory(history);
            return aggregate;
        }
    }
}
