using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Notifications;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Messaging.AzureServiceBus.Commands;
using Intellias.CQRS.ProcessManager.Pipelines.Requests;
using Intellias.CQRS.ProcessManager.Pipelines.Response;
using Intellias.CQRS.ProcessManager.Stores;

namespace Intellias.CQRS.ProcessManager.Pipelines.Middlewares
{
    /// <summary>
    /// Publish messages middleware.
    /// </summary>
    /// <typeparam name="TState">State of request.</typeparam>
    /// <typeparam name="TProcessHandler">Process handler type.</typeparam>
    public class PublishMessagesMiddleware<TState, TProcessHandler> : IProcessMiddleware<TState, TProcessHandler>
        where TState : class
        where TProcessHandler : BaseProcessHandler
    {
        private readonly ICommandBus<DefaultCommandBusOptions> commandBus;
        private readonly INotificationBus notificationBus;
        private readonly IProcessStore<TProcessHandler> store;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublishMessagesMiddleware{TState, TProcessHandler}"/> class.
        /// </summary>
        /// <param name="commandBus">Command bus.</param>
        /// <param name="notificationBus">Notification bus.</param>
        /// <param name="store">Process manager command store.</param>
        public PublishMessagesMiddleware(
            ICommandBus<DefaultCommandBusOptions> commandBus,
            INotificationBus notificationBus,
            IProcessStore<TProcessHandler> store)
        {
            this.commandBus = commandBus;
            this.notificationBus = notificationBus;
            this.store = store;
        }

        /// <inheritdoc/>
        public async Task<ProcessResponse> Execute(ProcessRequest<TState> request, ProcessMiddlewareDelegate next, CancellationToken token)
        {
            var response = await next();

            foreach (var message in response.ProcessMessages
                .Where(s => !s.IsPublished)
                .Select(s => s.Message))
            {
                var executionResult = await PublishMessageAsync(message);
                if (executionResult.Success)
                {
                    await store.MarkMessageAsPublishedAsync(request.Id, message);
                }
            }

            return response;

            Task<IExecutionResult> PublishMessageAsync(IMessage msg) =>
                msg switch
                {
                    ICommand cmd => commandBus.PublishAsync(cmd),
                    Notification notification => notificationBus.PublishAsync(notification),
                    _ => throw new ArgumentException($"Invalid message type '{msg.GetType()}'."),
                };
        }
    }
}
