using System.Threading;
using System.Threading.Tasks;
using Intellias.CQRS.ProcessManager;
using Intellias.CQRS.ProcessManager.Pipelines.Requests;
using Intellias.CQRS.ProcessManager.Pipelines.Response;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Core.EventHandlers.Tests;

namespace Intellias.CQRS.Tests.ProcessManager.Infrastructure
{
    public class ProcessHandler1 : BaseProcessHandler,
        IProcessHandler<FakeCreatedIntegrationEvent>,
        IProcessHandler<FakeUpdatedIntegrationEvent>
    {
        public Task<ProcessResponse> Handle(ProcessRequest<FakeCreatedIntegrationEvent> request, CancellationToken cancellationToken)
        {
            var state = request.State;

            return ResponseAsync(request, new TestCreateCommand
            {
                AggregateRootId = state.AggregateRootId,
                TestData = state.Data
            });
        }

        public Task<ProcessResponse> Handle(ProcessRequest<FakeUpdatedIntegrationEvent> request, CancellationToken cancellationToken)
        {
            var state = request.State;

            return ResponseAsync(request, new TestUpdateCommand
            {
                AggregateRootId = state.AggregateRootId,
                TestData = state.Data
            });
        }
    }
}