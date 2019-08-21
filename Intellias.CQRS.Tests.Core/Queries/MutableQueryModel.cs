using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Queries.Mutable;

namespace Intellias.CQRS.Tests.Core.Queries
{
    public class MutableQueryModel : BaseMutableQueryModel
    {
        public MutableQueryModel()
        {
            Id = Unified.NewCode();
            SomeProperty = Unified.NewCode();
        }

        public string SomeProperty { get; set; }
    }
}