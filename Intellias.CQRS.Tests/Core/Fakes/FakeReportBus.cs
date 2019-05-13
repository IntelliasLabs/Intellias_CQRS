using System.Collections.Generic;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <summary>
    /// Fake implementation of IReportBus, that store event in memory and add possibility to read this event
    /// </summary>
    public class FakeReportBus : IReportBus
    {
        private readonly IDictionary<string, IEvent> _store;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="store"></param>
        public FakeReportBus(IDictionary<string, IEvent> store)
        {
            _store = store ?? new Dictionary<string, IEvent>();
        }

        /// <summary>
        /// Save event to store
        /// </summary>
        /// <param name="msg">Event</param>
        /// <returns></returns>
        public Task<IExecutionResult> PublishAsync(IEvent msg)
        {
            _store.Add(msg.AggregateRootId, msg);

            return Task.FromResult<IExecutionResult>(ExecutionResult.Success);
        }

        /// <summary>
        /// Get event from store
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<IEvent> GetEventAsync(string id)
        {
            return _store.TryGetValue(id, out var @event)
                ? Task.FromResult(@event)
                : throw new KeyNotFoundException($"Event with id = {id}  hasn't been found");

        }
    }
}
