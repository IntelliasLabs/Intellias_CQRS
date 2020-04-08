using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.ProcessManager.Pipelines.Response;
using Intellias.CQRS.ProcessManager.Stores;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <summary>
    /// Fake process manager.
    /// </summary>
    public class FakeProcessManagerStore : IProcessManagerStore
    {
        private readonly Dictionary<string, List<ProcessMessage>> store
            = new Dictionary<string, List<ProcessMessage>>();

        /// <inheritdoc/>
        public Task<IReadOnlyCollection<ProcessMessage>> GetMessagesAsync(string id)
        {
            var processMessages = store.TryGetValue(id, out var value)
                ? value.ToArray()
                : Array.Empty<ProcessMessage>();

            return Task.FromResult<IReadOnlyCollection<ProcessMessage>>(processMessages);
        }

        /// <inheritdoc/>
        public Task PersistMessagesAsync(string id, IReadOnlyCollection<IMessage> messages)
        {
            if (store.TryGetValue(id, out var processMessages))
            {
                store.Remove(id);
            }
            else
            {
                processMessages = new List<ProcessMessage>();
            }

            store.Add(id, processMessages
                .Union(messages.Select(msg => new ProcessMessage(msg)))
                .ToList());

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task MarkMessageAsPublishedAsync(string id, IMessage message)
        {
            var processMessages = store.TryGetValue(id, out var value) ? value : new List<ProcessMessage>();

            var persistedMessage = processMessages.FirstOrDefault(s => s.Message.Id == message.Id);
            if (persistedMessage != null)
            {
                processMessages.Remove(persistedMessage);
                processMessages.Add(new ProcessMessage(message, true));
            }

            return Task.CompletedTask;
        }
    }
}