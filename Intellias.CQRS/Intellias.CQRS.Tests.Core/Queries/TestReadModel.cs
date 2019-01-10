using Intellias.CQRS.Core.Queries;

namespace Intellias.CQRS.Tests.Core.Queries
{
    /// <summary>
    /// DemoQueryModel
    /// </summary>
    public class TestQueryModel : IQueryModel
    {
        /// <inheritdoc />
        public string Id { set; get; }

        /// <inheritdoc />
        public string ParentId { get; set; }

        /// <summary>
        /// TestData
        /// </summary>
        public string TestData { set; get; }

        /// <summary>
        /// Version of query model
        /// </summary>
        public int Version { set; get; }
    }
}
