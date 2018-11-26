using Intellias.CQRS.Core.Queries;

namespace Intellias.CQRS.Core.Tests.Queries
{
    /// <summary>
    /// DemoQueryModel
    /// </summary>
    public class DemoQueryModel : IQueryModel
    {
        /// <inheritdoc />
        public string Id { set; get; }

        /// <summary>
        /// TestData
        /// </summary>
        public string TestData { set; get; }
    }
}
