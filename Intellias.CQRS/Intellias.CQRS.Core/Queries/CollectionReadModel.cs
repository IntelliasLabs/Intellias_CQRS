using System.Collections.Generic;

namespace Intellias.CQRS.Core.Queries
{
    /// <summary>
    /// CollectionReadModel
    /// </summary>
    /// <typeparam name="TQueryModel">Read Model</typeparam>
    public class CollectionQueryModel<TQueryModel> where TQueryModel : class, IQueryModel
    {
        /// <summary>
        /// Model Items
        /// </summary>
        public IReadOnlyCollection<TQueryModel> Items { set; get; }

        /// <summary>
        /// Total count of items
        /// </summary>
        public int Total { set; get; }
    }
}
