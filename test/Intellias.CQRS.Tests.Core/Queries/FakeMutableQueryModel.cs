using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Core.Queries.Mutable;

namespace Intellias.CQRS.Tests.Core.Queries
{
    public class FakeMutableQueryModel : BaseMutableQueryModel
    {
        public FakeMutableQueryModel()
        {
            Id = Unified.NewCode();
            Data = Unified.NewCode();
        }

        public string Data { get; set; }
    }
}