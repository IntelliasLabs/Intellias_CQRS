using System.Collections.Generic;
using Intellias.CQRS.Core.Queries;

namespace Intellias.CQRS.Tests.Core.Queries
{
    /// <summary>
    /// 
    /// </summary>
    public class DemoCollectionReadModel : IReadModel
    {
        /// <summary>
        /// Model Items
        /// </summary>
        public IReadOnlyCollection<DemoReadModel> Items { set; get; }

        /// <summary>
        /// Total count of items
        /// </summary>
        public int Total { set; get; }
    }
}
