using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;
using Intellias.CQRS.Core;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;

namespace Intellias.CQRS.EventBus.AzureServiceBus
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
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
                    try
                    {
                        var json = Encoding.UTF8.GetString(msg.Body);
                        var message = json.FromJson<IMessage>();

                        // Invoke handler
                        if (handler != null)
                        {
                            await handler(message);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.LogError(ex, ex.Message);
                    }
                    finally
                    {
                        await session.CompleteAsync(msg.SystemProperties.LockToken);
                    }
                }, options);
        }

        private Task ExceptionReceivedHandlerAsync(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            log.LogError(exceptionReceivedEventArgs.Exception, exceptionReceivedEventArgs.Exception.Message);
            return Task.CompletedTask;
        }
    }
}