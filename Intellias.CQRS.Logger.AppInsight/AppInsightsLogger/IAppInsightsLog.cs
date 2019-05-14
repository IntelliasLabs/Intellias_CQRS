using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace Intellias.CQRS.Logger.AppInsight.AppInsightsLogger
{
    /// <summary>
    /// IAppInsightsLog
    /// </summary>
    public interface IAppInsightsLog
    {
        /// <summary>
        /// TelemetryClient
        /// </summary>
        TelemetryClient TelemetryClient { get; }

        /// <summary>
        /// Tracks trace message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        AppInsightsLog LogInfo(AppInsightsLogMessage message);

        /// <summary>
        /// StartRecordDependency
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dependencyType"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        DependencyTelemetry StartRecordDependency(string name, string dependencyType, AppInsightsLogMessage message);

        /// <summary>
        /// StopRecordDependency
        /// </summary>
        /// <param name="dependency"></param>
        /// <returns></returns>
        AppInsightsLog StopRecordDependency(DependencyTelemetry dependency);

        /// <summary>
        /// App insights telemetry client uses batches for sending requests.
        /// We should force sending this cached data
        /// </summary>
        void Flush();
    }
}