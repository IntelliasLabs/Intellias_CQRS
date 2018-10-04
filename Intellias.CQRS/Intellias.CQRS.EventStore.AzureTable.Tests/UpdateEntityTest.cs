using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.EventStore.AzureTable.Tests.Core;

namespace Intellias.CQRS.EventStore.AzureTable.Tests
{
    internal class UpdateEntityTest : BaseTest
    {
        private readonly string testId = Unified.NewCode();
        private readonly string testData = "Test Data";
        public UpdateEntityTest()
        {
            CreateItem(testId, testData);

        }
    }
}
