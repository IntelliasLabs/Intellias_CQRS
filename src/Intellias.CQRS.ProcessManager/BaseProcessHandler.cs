using System.Linq;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Security;
using Intellias.CQRS.ProcessManager.Pipelines.Requests;
using Intellias.CQRS.ProcessManager.Pipelines.Response;

namespace Intellias.CQRS.ProcessManager
{
    /// <summary>
    /// Base process handler.
    /// </summary>
    public abstract class BaseProcessHandler
    {
        private readonly string processManagerUserId = "5ee7b263-16a0-410f-ab68-f85eea17ca31";

        /// <summary>
        /// Create response.
        /// </summary>
        /// <typeparam name="TState">State type.</typeparam>
        /// <param name="porcessRequest">Process request.</param>
        /// <param name="messages">Messages.</param>
        /// <returns>Process response.</returns>
        protected ProcessResponse Response<TState>(ProcessRequest<TState> porcessRequest, params AbstractMessage[] messages)
            where TState : class
        {
            var state = porcessRequest.State;
            var correlationId = state is IIntegrationEvent @event
                ? @event.CorrelationId
                : Unified.NewCode();

            var processMessages = messages.Select(msg =>
            {
                var principal = new Principal
                {
                    IdentityId = processManagerUserId,
                    UserId = processManagerUserId,
                    IsProcessManager = true,
                };

                msg.CorrelationId = correlationId;
                msg.Actor = principal.AsActor();

                EnrichWithUserId(msg, processManagerUserId);
                EnrichWithRoles(msg, "[]");

                if (msg is Command cmd)
                {
                    cmd.Principal = principal;
                }

                return new ProcessMessage(msg);
            });

            return new ProcessResponse(porcessRequest.Id, processMessages.ToArray());
        }

        /// <summary>
        /// Create response async.
        /// </summary>
        /// <typeparam name="TState">State type.</typeparam>
        /// <param name="porcessRequest">Process request.</param>
        /// <param name="commands">Commands.</param>
        /// <returns>Task of process response.</returns>
        protected Task<ProcessResponse> ResponseAsync<TState>(ProcessRequest<TState> porcessRequest, params AbstractMessage[] commands)
            where TState : class
            => Task.FromResult(Response(porcessRequest, commands));

        /// <summary>
        /// Create empty response.
        /// </summary>
        /// <typeparam name="TState">State type.</typeparam>
        /// <param name="porcessRequest">Process request.</param>
        /// <returns>Empty response.</returns>
        protected ProcessResponse EmptyResponse<TState>(ProcessRequest<TState> porcessRequest)
            where TState : class
            => new ProcessResponse(porcessRequest.Id);

        /// <summary>
        /// Create empty response async.
        /// </summary>
        /// <typeparam name="TState">State type.</typeparam>
        /// <param name="porcessRequest">Process request.</param>
        /// <returns>Task of empty response.</returns>
        protected Task<ProcessResponse> EmptyResponseAsync<TState>(ProcessRequest<TState> porcessRequest)
            where TState : class
            => Task.FromResult(EmptyResponse(porcessRequest));

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
