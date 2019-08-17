using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace Intellias.CQRS.Logger.AppInsight.AppInsightsLogger
{
    /// <summary>
    /// IAppInsightsLog.
    /// </summary>
    public interface IAppInsightsLog
    {
        /// <summary>
        /// TelemetryClient.
        /// </summary>
        TelemetryClient TelemetryClient { get; }

        /// <summary>
        /// Tracks trace message.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <returns>AppInsightsLog.</returns>
        AppInsightsLog LogInfo(AppInsightsLogMessage message);

        /// <summary>
        /// StartRecordDependency.
        /// </summary>
        /// <param name="name">Name of the command initiated with this dependency call. Examples are stored procedure name and URL path template.</param>
        /// <param name="dependencyType">Logical grouping of dependencies. Examples are SQL, Azure table, and HTTP.</param>
        /// <param name="message">Message to log.</param>
        /// <returns>DependencyTelemetry.</returns>
        DependencyTelemetry StartRecordDependency(string name, string dependencyType, AppInsightsLogMessage message);

        /// <summary>
        /// StopRecordDependency.
        /// </summary>
        /// <param name="dependency">DependencyTelemetry.</param>
        /// <returns>AppInsightsLog.</returns>
        AppInsightsLog StopRecordDependency(DependencyTelemetry dependency);

        /// <summary>
        /// App insights telemetry client uses batches for sending requests.
        /// We should force sending this cached data.
        /// </summary>
        void Flush();
    }
}