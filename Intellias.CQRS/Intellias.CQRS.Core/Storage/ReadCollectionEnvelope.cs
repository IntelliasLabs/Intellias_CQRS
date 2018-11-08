using System.Collections.Generic;
using Intellias.CQRS.Core.Queries;

namespace Intellias.CQRS.Core.Storage
{
    /// <summary>
    /// Read Model
    /// </summary>
    /// <typeparam name="TReadModel"></typeparam>
    public class ReadCollectionEnvelope<TReadModel> : ReadModelEnvelope
        where TReadModel : class, IReadModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public ReadCollectionEnvelope(IReadOnlyCollection<TReadModel> items)
        {
            Items = items;
        }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyCollection<TReadModel> Items { get; }

        /// <summary>
        /// Total items in collection
        /// </summary>
        public int Total { set; get; }
    }
}