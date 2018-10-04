using Intellias.CQRS.Core.Commands;

namespace Intellias.CQRS.Tests.Core.Commands
{
    /// <inheritdoc />
    public class TestUpdateCommand : Command
    {
        /// <summary>
        /// TestData
        /// </summary>
        public string TestData { get; set; }
    }
}
