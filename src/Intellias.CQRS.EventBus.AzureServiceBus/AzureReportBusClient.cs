using System;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.EventBus.AzureServiceBus.Extensions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;

namespace Intellias.CQRS.EventBus.AzureServiceBus
{
    /// <inheritdoc />
    public class AzureReportBusClient : IReportBusClient
    {
        private readonly ISubscriptionClient sub;
        private readonly ILogger<AzureReportBusClient> log;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureReportBusClient"/> class.
        /// </summary>
        /// <param name="log">ILogger.</param>
        /// <param name="sub">Subscription Type.</param>
        public AzureReportBusClient(ILogger<AzureReportBusClient> log, ISubscriptionClient sub)
        {
            this.sub = sub;
            this.log = log;
        }

        /// <inheritdoc />
        public void Subscribe(Func<IMessage, Task> handler)
        {
            var options = new SessionHandlerOptions(ExceptionReceivedHandlerAsync)
            {
                AutoComplete = false
            };

            // Register the function that processes messages.
            sub.RegisterSessionHandler(
                async (session, msg, token) =>
                {
                    var message = msg.GetMessage();

                    // Invoke handler
                    if (handler != null)
                    {
                        await handler(message);
                    }

                    await session.CompleteAsync(msg.SystemProperties.LockToken);
                }, options);
        }

        /// <inheritdoc />
        public async Task UnsubscribeAllAsync()
        {
            await sub.CloseAsync();
            log.LogInformation($"'{nameof(AzureReportBusClient)}' is unsubscribed from all registered handlers.");
        }

        private Task ExceptionReceivedHandlerAsync(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            log.LogError(exceptionReceivedEventArgs.Exception, exceptionReceivedEventArgs.Exception.Message);
            return Task.CompletedTask;
        }
    }
}