using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Security;
using Intellias.CQRS.ProcessManager.Pipelines.Requests;
using Intellias.CQRS.ProcessManager.Pipelines.Response;

namespace Intellias.CQRS.ProcessManager.Pipelines.Middlewares
{
    /// <summary>
    /// Enriches process response messages with additional information.
    /// </summary>
    /// <typeparam name="TState">State of request.</typeparam>
    /// <typeparam name="TProcessHandler">Process handler type.</typeparam>
    public class EnrichMessagesMiddleware<TState, TProcessHandler> : IProcessMiddleware<TState, TProcessHandler>
        where TState : class
        where TProcessHandler : BaseProcessHandler
    {
        private readonly ProcessHandlerOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnrichMessagesMiddleware{TState,TProcessHandler}"/> class.
        /// </summary>
        /// <param name="options">Process handler options.</param>
        public EnrichMessagesMiddleware(ProcessHandlerOptions options)
        {
            this.options = options;
        }

        /// <inheritdoc />
        public async Task<ProcessResponse> Execute(ProcessRequest<TState> request, ProcessMiddlewareDelegate next, CancellationToken token)
        {
            var response = await next();

            var correlationId = request.State is IIntegrationEvent @event
                ? @event.CorrelationId
                : Unified.NewCode();

            var processMessages = response.ProcessMessages
                .Select(m =>
                {
                    m.Message.CorrelationId = correlationId;
                    EnrichWithUserId(m.Message, options.ActorId);
                    EnrichWithRoles(m.Message, "[]");

                    var principal = new Principal
                    {
                        IdentityId = options.ActorId,
                        UserId = options.ActorId,
                        IsProcessManager = true,
                    };

                    m.Message.Actor = principal.AsActor();
                    if (m.Message is Command cmd)
                    {
                        cmd.Principal = principal;
                    }

                    return new ProcessMessage(m.Message, m.IsPublished);
                })
                .ToArray();

            return new ProcessResponse(response.Id, processMessages, response.IsPersisted);
        }

        private void EnrichWithUserId(IMessage message, string userId)
            => EnrichWith(message, MetadataKey.UserId, userId);

        private void EnrichWithRoles(IMessage message, string roles)
            => EnrichWith(message, MetadataKey.Roles, roles);

        private void EnrichWith(IMessage message, MetadataKey key, string value)
        {
            var metadata = message.Metadata;
            if (metadata.ContainsKey(key))
            {
                metadata.Remove(key);
            }

            metadata.Add(key, value);
        }
    }
}