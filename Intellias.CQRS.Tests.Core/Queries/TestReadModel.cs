using System;
using Intellias.CQRS.Core.Queries;

namespace Intellias.CQRS.Tests.Core.Queries
{
    /// <summary>
    /// DemoQueryModel
    /// </summary>
    public class TestQueryModel : IQueryModel
    {
        /// <inheritdoc />
        public string Id { set; get; } = string.Empty;

        /// <summary>
        /// ParentId
        /// </summary>
        public string ParentId { get; set; } = string.Empty;

        /// <summary>
        /// TestData
        /// </summary>
        public string TestData { set; get; } = string.Empty;

        /// <inheritdoc />
        public int Version { set; get; }

        /// <inheritdoc />
        public DateTime Timestamp { get; set; }
    }
}
