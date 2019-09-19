using Intellias.CQRS.Core.Commands;

namespace Intellias.CQRS.Tests.Utils.Pipelines.Fakes
{
    public class FakeCreateCommand : Command
    {
        public string Data { get; set; }
    }
}