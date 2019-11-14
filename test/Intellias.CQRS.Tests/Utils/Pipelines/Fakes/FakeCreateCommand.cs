using Intellias.CQRS.Core.Commands;

namespace Intellias.CQRS.Tests.Utils.Pipelines.Fakes
{
    public class FakeCreateCommand : Command
    {
        public string Data { get; set; } = string.Empty;

        public SomeItem[] SomeArray { get; set; } = new SomeItem[3];
    }

    public class SomeItem
    {
        public SomeItem[] SomeInternalArray { get; set; } = new SomeItem[5];

        public int SomeInt { get; set; }
    }
}