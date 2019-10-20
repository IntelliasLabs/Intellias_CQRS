using Intellias.CQRS.Core.Commands;

namespace Intellias.CQRS.Tests.Core.Commands
{
    /// <summary>
    /// Test update command.
    /// </summary>
    public class TestUpdateCommand : Command
    {
        /// <summary>
        /// TestData.
        /// </summary>
        public string TestData { get; set; } = string.Empty;
    }
}
