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
        private readonly IDictionary<string, IMessage> _store;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="store"></param>
        public FakeReportBus(IDictionary<string, IMessage> store)
        {
            _store = store ?? new Dictionary<string, IMessage>();
        }

        /// <summary>
        /// Save event to store
        /// </summary>
        /// <param name="message">message</param>
        /// <returns></returns>
        public Task PublishAsync<TMessage>(TMessage message) where TMessage : IMessage
        {
            _store.Add(message.AggregateRootId, message);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Get message from store
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<IMessage> GetMessageAsync(string id)
        {
            return _store.TryGetValue(id, out var @event)
                ? Task.FromResult(@event)
                : throw new KeyNotFoundException($"Event with id = {id}  hasn't been found");

        }
    }
}