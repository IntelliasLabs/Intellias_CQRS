using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Results;
using Intellias.CQRS.Core.Results.Errors;
using Intellias.CQRS.Core.Results.Extensions;

namespace Intellias.CQRS.Tests.Utils.Pipelines.Fakes
{
    public class FakeAggregateRoot : BaseAggregateRoot<FakeAggregateState>
    {
        public FakeAggregateRoot(string id, AggregateExecutionContext context)
            : base(id, context)
        {
        }

        public IExecutionResult Create(string data)
        {
            PublishEvent<FakeCreateStateEvent>(e => e.Data = data);
            return Success();
        }

        public IExecutionResult Update(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                var errorCode = new ErrorCodeInfo(nameof(Core), "InputCantBeNullOrEmpty", "Data can't be empty.");

                return ValidationFailed(errorCode)
                    .ForCommand<FakeUpdateCommand>(c => c.Data);
            }

            PublishEvent<FakeUpdatedStateEvent>(e => e.Data = data);
            return Success();
        }
    }
}