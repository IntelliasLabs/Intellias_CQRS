using System;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;

namespace Intellias.CQRS.Core.Domain
{
    /// <inheritdoc />
    public class AggregateStore : IAggregateStore
    {
        private readonly IEventStore store;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateStore"/> class.
        /// </summary>
        /// <param name="store">Store of the aggregate events.</param>
        public AggregateStore(IEventStore store)
        {
            this.store = store;
        }

        /// <inheritdoc />
        public async Task<TAggregateRoot> GetAsync<TAggregateRoot, TAggregateState>(
            string aggregateId,
            AggregateExecutionContext context)
            where TAggregateRoot : AggregateRoot<TAggregateState>
            where TAggregateState : AggregateState, new()
        {
            var aggregateRoot = CreateEmptyAggregate<TAggregateRoot, TAggregateState>(aggregateId, context);

            await FillAggregateHistoryAsync(aggregateRoot);

            return aggregateRoot;
        }

        /// <inheritdoc />
        public TAggregateRoot Create<TAggregateRoot, TAggregateState>(string aggregateId, AggregateExecutionContext context)
            where TAggregateRoot : AggregateRoot<TAggregateState>
            where TAggregateState : AggregateState, new()
        {
            return CreateEmptyAggregate<TAggregateRoot, TAggregateState>(aggregateId, context);
        }

        /// <inheritdoc />
        public async Task UpdatedAsync(IAggregateRoot aggregate)
        {
            if (aggregate.Events.Count == 0)
            {
                return;
            }

            await store.SaveAsync(aggregate);
        }

        private static TAggregateRoot CreateEmptyAggregate<TAggregateRoot, TAggregateState>(string aggregateId, AggregateExecutionContext context)
           where TAggregateRoot : AggregateRoot<TAggregateState>
           where TAggregateState : AggregateState, new()
        {
            var type = typeof(TAggregateRoot);
            var constructorInfo = type.GetConstructor(new[] { typeof(string), typeof(AggregateExecutionContext) })
                ?? throw new ArgumentNullException($"No contrsuctor with '{typeof(string)}, {typeof(AggregateExecutionContext)}' "
                    + $"arguments is found in '{typeof(TAggregateRoot)}'.");

            var aggregateRoot = (TAggregateRoot)constructorInfo.Invoke(new object[] { aggregateId, context });
            return aggregateRoot;
        }

        private async Task FillAggregateHistoryAsync(IAggregateRoot aggregate)
        {
            var history = await store.GetAsync(aggregate.Id, 0);
            aggregate.LoadFromHistory(history);
        }
    }
}