using System;
using System.Collections.Generic;
using Intellias.CQRS.Core.Queries;

namespace Intellias.CQRS.Core.Storage
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ReadModelEnvelope
    {
        /// <summary>
        /// Used for collection reads
        /// </summary>
        protected ReadModelEnvelope()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readModelId"></param>
        /// <param name="version"></param>
        protected ReadModelEnvelope(
            string readModelId,
            long? version)
        {
            if (string.IsNullOrEmpty(readModelId))
            {
                throw new ArgumentNullException(nameof(readModelId));
            }

            ReadModelId = readModelId;
            Version = version;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ReadModelId { get; }

        /// <summary>
        /// 
        /// </summary>
        public long? Version { get; }
    }

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

    /// <summary>
    /// 
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
