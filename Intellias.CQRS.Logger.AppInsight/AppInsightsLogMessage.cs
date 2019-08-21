using Intellias.CQRS.Core;

namespace Intellias.CQRS.Logger.AppInsight
{
    /// <summary>
    /// Standardized log message.
    /// </summary>
    public class AppInsightsLogMessage
    {
        private object internalData = string.Empty;

        /// <summary>
        /// Message.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Serialized data.
        /// </summary>
        public string DataJson { get; private set; } = string.Empty;

        /// <summary>
        /// Data.
        /// </summary>
        public object Data
        {
            get => internalData;
            set
            {
                internalData = value;
                DataJson = internalData.ToJson();
            }
        }
    }
}
