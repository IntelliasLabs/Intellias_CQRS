using System.Threading;
using Intellias.CQRS.Core.Commands;
using Intellias.CQRS.Core.Messages;
using Intellias.CQRS.Logger.AppInsight;
using Intellias.CQRS.Logger.AppInsight.AppInsightsLogger;
using Intellias.CQRS.Tests.Core.Commands;
using Xunit;

namespace IntelliGrowth.Core.Tests.ApplicationLogger
{
    public class ApplicationLoggerTests
    {
        private readonly ICommand command;
        AppInsightsLog logger;

        public ApplicationLoggerTests()
        {
            logger = new AppInsightsLog("key");

            command = new TestCreateCommand
            {
                CorrelationId = Unified.NewCode()
            };

            command.Metadata[MetadataKey.UserId] = "Test.User@intellias.com";

            logger.ApplyOperationConfig(command.ToLogConfig());
        }


        /// <summary>
        /// LogInfoMessage
        /// </summary>
        [Fact]
        public void LogInfoMessage()
        {
            for (var i = 0; i < 10; i++)
            {
                logger.LogInfo(new AppInsightsLogMessage
                {
                    Data = command,
                    Message = $"Create command test #{i} for operation {command.CorrelationId}"
                });
            }

            logger.Flush();
        }


        /// <summary>
        /// Log Dependency
        /// </summary>
        [Fact]
        public void LogDependency()
        {
            for (var i = 0; i < 10; i++)
            {
                var dependancy = logger.StartRecordDependency(
                    "Test dependency",
                    "Azure Func",
                    new AppInsightsLogMessage
                    {
                        Data = command,
                        Message = $"Create command dependency test #{i} for operation {command.CorrelationId}"
                    });

                Thread.Sleep(100);

                logger.StopRecordDependency(dependancy);
            }

            logger.Flush();
        }
    }
}
