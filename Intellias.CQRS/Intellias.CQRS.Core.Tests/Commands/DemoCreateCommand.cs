using Intellias.CQRS.Core.Commands;

namespace Intellias.CQRS.Core.Tests.Commands
{
    /// <summary>
    /// Demo create command
    /// </summary>
    public class DemoCreateCommand : Command
    {
        /// <summary>
        /// Demo name property
        /// </summary>
        public string Name { get; set; }
    }
}
