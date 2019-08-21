using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Queries.Immutable;

namespace Intellias.CQRS.Tests.Core.Queries
{
    public class ImmutableQueryModel : BaseImmutableQueryModel
    {
        public ImmutableQueryModel()
        {
            Id = Unified.NewCode();
            Version = 1;
            SomeProperty = Unified.NewCode();
        }

        public string SomeProperty { get; set; }
    }
}