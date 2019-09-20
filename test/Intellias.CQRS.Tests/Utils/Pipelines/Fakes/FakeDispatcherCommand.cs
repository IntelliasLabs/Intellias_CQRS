using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Results;
using MediatR;

namespace Intellias.CQRS.Tests.Utils.Pipelines.Fakes
{
    public class FakeDispatcherCommand : Command, IRequest<IExecutionResult>
    {
    }
}