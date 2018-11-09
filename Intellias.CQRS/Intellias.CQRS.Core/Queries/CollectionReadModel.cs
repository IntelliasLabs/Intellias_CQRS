using System.Collections.Generic;

namespace Intellias.CQRS.Core.Queries
{
    /// <summary>
    /// CollectionReadModel
    /// </summary>
    /// <typeparam name="TReadModel">Read Model</typeparam>
    public class CollectionReadModel<TReadModel> where TReadModel : class, IReadModel
    {
        /// <summary>
        /// Model Items
        /// </summary>
        public IReadOnlyCollection<TReadModel> Items { set; get; }

        /// <summary>
        /// Total count of items
        /// </summary>
        public int Total { set; get; }
    }
}
