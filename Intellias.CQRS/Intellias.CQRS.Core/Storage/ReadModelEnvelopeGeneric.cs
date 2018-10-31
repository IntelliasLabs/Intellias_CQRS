using Intellias.CQRS.Core.Queries;

namespace Intellias.CQRS.Core.Storage
{
    /// <summary>
    /// Read Model
    /// </summary>
    /// <typeparam name="TReadModel">Type of ReadModel</typeparam>
    public class ReadModelEnvelope<TReadModel> : ReadModelEnvelope
        where TReadModel : class, IReadModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="readModelId"></param>
        /// <param name="readModel"></param>
        /// <param name="version"></param>
        public ReadModelEnvelope(
            string readModelId,
            TReadModel readModel,
            long? version)
            : base(readModelId, version)
        {
            ReadModel = readModel;
        }

        /// <summary>
        /// Read Model
        /// </summary>
        public TReadModel ReadModel { get; }
    }
}