using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Events;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.EventBus.AzureServiceBus.Extensions;
using Microsoft.Azure.ServiceBus;

namespace Intellias.CQRS.EventBus.AzureServiceBus
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class AzureServiceTopicReportBus : IReportBus
    {
        private readonly ITopicClient topicClient;

        /// <summary>
        /// Creates an instance of ReportBus
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="topic"></param>
        public AzureServiceTopicReportBus(string connectionString, string topic)
        {
            topicClient = new TopicClient(connectionString, topic);
        }

        /// <inheritdoc />
        public async Task<IExecutionResult> PublishAsync(IEvent msg)
        {
            var busMsg = msg.ToBusMessage();
            await topicClient.SendAsync(busMsg);
            return await Task.FromResult(ExecutionResult.Success);
        }
    }
}
