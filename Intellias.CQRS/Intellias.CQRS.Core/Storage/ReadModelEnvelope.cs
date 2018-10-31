using System;

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
}
