using Intellias.CQRS.Core.Config;
using Newtonsoft.Json;

namespace Intellias.CQRS.Logger.AppInsight
{
    /// <summary>
    /// Standardized log message
    /// </summary>
    public class AppInsightsLogMessage
    {
        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Serialized data
        /// </summary>
        public string DataJson { get; private set; } = string.Empty;

        private object data = string.Empty;

        /// <summary>
        /// Data
        /// </summary>
        public object Data
        {
            get => data;
            set
            {
                data = value;
                DataJson = JsonConvert.SerializeObject(data, CqrsSettings.JsonConfig());
            }
        }
    }
}
