using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.ProcessManager.Pipelines.Requests;
using Intellias.CQRS.ProcessManager.Pipelines.Response;
using Intellias.CQRS.ProcessManager.Stores;

namespace Intellias.CQRS.ProcessManager.Pipelines.Middlewares
{
    /// <summary>
    /// Persist messages middleware.
    /// </summary>
    /// <typeparam name="TState">State of request.</typeparam>
    public class PersistMessageMiddleware<TState> : IProcessMiddleware<TState>
        where TState : class
    {
        private readonly IProcessManagerStore store;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistMessageMiddleware{TState}"/> class.
        /// </summary>
        /// <param name="store">Process manager store.</param>
        public PersistMessageMiddleware(IProcessManagerStore store)
        {
            this.store = store;
        }

        /// <inheritdoc/>
        public async Task<ProcessResponse> Execute(ProcessRequest<TState> request, ProcessMiddlewareDelegate next, CancellationToken token)
        {
            var response = await next();

            var persistedMessages = await store.GetMessagesAsync(request.Id);
            var messagesToPersist = response.ProcessMessages
                .Select(s => s.Message)
                .Where(m => !persistedMessages.Any(s => IsMessagesEquals(s.Message, m)))
                .ToArray();

            if (messagesToPersist.Length > 0)
            {
                await store.PersistMessagesAsync(request.Id, messagesToPersist);
            }

            var processMessages = response.ProcessMessages
                .Select(s => ToPublishedMessage(persistedMessages, s))
                .ToArray();

            return new ProcessResponse(response.Id, processMessages);
        }

        private static ProcessMessage ToPublishedMessage(IReadOnlyCollection<ProcessMessage> persistedMessages, ProcessMessage current)
        {
            var persistedMessage = persistedMessages.FirstOrDefault(m => IsMessagesEquals(current.Message, m.Message));
            var isPublished = persistedMessage != null && persistedMessage.IsPublished;

            return new ProcessMessage(current.Message, isPublished);
        }

        private static bool IsMessagesEquals(IMessage x, IMessage y)
        {
            return x.AggregateRootId == y.AggregateRootId
                && x.GetType() == y.GetType();
        }
    }
}
