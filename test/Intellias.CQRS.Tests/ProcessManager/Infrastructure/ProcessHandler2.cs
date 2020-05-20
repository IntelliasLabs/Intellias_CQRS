using System.Threading;
using System.Threading.Tasks;
using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Queries.Mutable;
using Intellias.CQRS.ProcessManager;
using Intellias.CQRS.ProcessManager.Pipelines.Requests;
using Intellias.CQRS.ProcessManager.Pipelines.Response;
using Intellias.CQRS.Tests.Core.Commands;
using Intellias.CQRS.Tests.Core.EventHandlers;

namespace Intellias.CQRS.Tests.ProcessManager.Infrastructure
{
    public class ProcessHandler2 : BaseProcessHandler,
        IProcessHandler<FakeSnapshotQueryModel>,
        IProcessHandler<FakeUpdatedIntegrationEvent>,
        IProcessHandler<CustomState>
    {
        public Task<ProcessResponse> Handle(ProcessRequest<FakeSnapshotQueryModel> request, CancellationToken cancellationToken)
        {
            var state = request.State;

            return ResponseAsync(request, new TestCreateCommand
            {
                AggregateRootId = state.Id,
                TestData = state.Data
            });
        }

        public Task<ProcessResponse> Handle(ProcessRequest<FakeUpdatedIntegrationEvent> request, CancellationToken cancellationToken)
        {
            var state = request.State;

            return ResponseAsync(request, new TestCreateCommand
            {
                AggregateRootId = Unified.NewCode(),
                TestData = state.Data
            });
        }

        public Task<ProcessResponse> Handle(ProcessRequest<CustomState> request, CancellationToken cancellationToken)
        {
            var state = request.State;

            return ResponseAsync(request, new TestCreateCommand
            {
                AggregateRootId = Unified.NewCode(),
                TestData = state.Data
            });
        }
    }

    public class FakeSnapshotQueryModel : BaseMutableQueryModel
    {
        public SnapshotId First { get; set; }

        public string Data { get; set; }
    }

    public class CustomState
    {
        public string Id { get; set; }

        public string Data { get; set; }
    }
}