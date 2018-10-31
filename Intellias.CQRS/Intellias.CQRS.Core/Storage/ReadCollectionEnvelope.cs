using System.Collections.Generic;
using Intellias.CQRS.Core.Queries;

namespace Intellias.CQRS.Core.Storage
{
    /// <summary>
    /// Read Model
    /// </summary>
    /// <typeparam name="TReadModel"></typeparam>
    public class ReadCollectionEnvelope<TReadModel> : ReadModelEnvelope
        where TReadModel : class, IReadOnlyCollection<IReadModel>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="readModel"></param>
        public ReadCollectionEnvelope(
            TReadModel readModel)
        {
            ReadModel = readModel;
        }

        /// <summary>
        /// 
        /// </summary>
        public TReadModel ReadModel { get; }
    }
}