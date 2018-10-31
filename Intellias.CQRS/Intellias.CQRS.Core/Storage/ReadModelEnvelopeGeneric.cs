using Intellias.CQRS.Core.Queries;

namespace Intellias.CQRS.Core.Storage
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TReadModel"></typeparam>
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
        /// 
        /// </summary>
        public TReadModel ReadModel { get; }
    }
}