using Intellias.CQRS.Core.Commands;

namespace Intellias.CQRS.Tests.Core.Commands
{
    /// <inheritdoc />
    public class TestCreateCommand : Command
    {
        /// <summary>
        /// TestData
        /// </summary>
        public string TestData { get; set; }
    }
}
