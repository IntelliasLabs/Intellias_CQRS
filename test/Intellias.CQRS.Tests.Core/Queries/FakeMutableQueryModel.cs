using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Queries.Mutable;

namespace Intellias.CQRS.Tests.Core.Queries
{
    public class FakeMutableQueryModel : BaseMutableQueryModel
    {
        public FakeMutableQueryModel()
        {
            Id = Unified.NewCode();
            SomeProperty = Unified.NewCode();
        }

        public string SomeProperty { get; set; }
    }
}