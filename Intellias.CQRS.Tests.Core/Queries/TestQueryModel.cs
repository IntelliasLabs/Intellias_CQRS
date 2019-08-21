using System;
using System.Collections.Generic;
using Intellias.CQRS.Core.Queries;

namespace Intellias.CQRS.Tests.Core.Queries
{
    /// <summary>
    /// DemoQueryModel.
    /// </summary>
    public class TestQueryModel : IQueryModel
    {
        /// <inheritdoc />
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// ParentId.
        /// </summary>
        public string ParentId { get; set; } = string.Empty;

        /// <summary>
        /// TestData.
        /// </summary>
        public string TestData { get; set; } = string.Empty;

        /// <summary>
        /// Array of strings.
        /// </summary>
#pragma warning disable CA2227 // We can't deserialize model without setter
        public List<string> TestList { get; set; } = new List<string>();
#pragma warning restore CA2227 // We can't deserialize model without setter

        /// <inheritdoc />
        public int Version { get; set; }

        /// <inheritdoc />
        public DateTime Timestamp { get; set; }
    }
}
