using Intellias.CQRS.Core.Domain;
using Intellias.CQRS.Core.Results;

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
                return ValidationFailed("Data can't be empty.");
            }

            PublishEvent<FakeUpdatedStateEvent>(e => e.Data = data);
            return Success();
        }
    }
}