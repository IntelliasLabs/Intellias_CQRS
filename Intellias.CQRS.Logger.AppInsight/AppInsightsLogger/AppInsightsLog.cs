using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace Intellias.CQRS.Logger.AppInsight.AppInsightsLogger
{
    /// <summary>
    /// Usage:
    /// First - we have to register config for operation, so
    /// 1. AppInsightsLog.ApplyOperationConfig(config)
    /// Now we are able to log messages, which belong to particular operation
    /// 2. AppInsightsLog -> LogInfo/LogError/Any other log
    /// </summary>
    public class AppInsightsLog : IAppInsightsLog
    {
        /// <summary>
        /// TelemetryClient
        /// </summary>
        public TelemetryClient TelemetryClient { get; }

        /// <summary>
        /// AppInsightsLog
        /// </summary>
        /// <param name="telemetryKey"></param>
        public AppInsightsLog(string telemetryKey)
        {
            TelemetryClient = new TelemetryClient
            {
                InstrumentationKey = telemetryKey
            };
        }

        /// <summary>
        /// ApplyOperationConfig
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public AppInsightsLog ApplyOperationConfig(OperationConfig config)
        {
            TelemetryClient.Context.Operation.Id = config.OperationId;
            TelemetryClient.Context.Operation.Name = config.OperationName;
            TelemetryClient.Context.User.AuthenticatedUserId = config.UserId;

            return this;
        }

        /// <summary>
        /// Tracks trace message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public AppInsightsLog LogInfo(AppInsightsLogMessage message)
        {
            TelemetryClient.TrackTrace(
                message.Message,
                SeverityLevel.Information,
                new Dictionary<string, string> { { "Payload", message.DataJson } });

            return this;
        }

        /// <summary>
        /// StartRecordDependency
        /// </summary>
        /// <param name="name">Name of the command initiated with this dependency call. Examples are stored procedure name and URL path template.</param>
        /// <param name="dependencyType">Logical grouping of dependencies. Examples are SQL, Azure table, and HTTP. </param>
        /// <param name="message"></param>
        /// <returns></returns>
        public DependencyTelemetry StartRecordDependency(string name, string dependencyType, AppInsightsLogMessage message)
        {
            return new DependencyTelemetry
            {
                Name = name,
                Type = dependencyType,
                Data = message.DataJson,
                Timestamp = DateTimeOffset.Now,
                Success = false
            };
        }

        /// <summary>
        /// StopRecordDependency
        /// </summary>
        /// <param name="dependency"></param>
        /// <returns></returns>
        public AppInsightsLog StopRecordDependency(DependencyTelemetry dependency)
        {
            dependency.Success = true;
            dependency.Duration = DateTimeOffset.Now.Subtract(dependency.Timestamp);
            TelemetryClient.TrackDependency(dependency);

            return this;
        }

        /// <summary>
        /// App insights telemetry client uses batches for sending requests.
        /// We should force sending this cached data
        /// </summary>
        public void Flush()
        {
            TelemetryClient.Flush();
        }
    }
}
