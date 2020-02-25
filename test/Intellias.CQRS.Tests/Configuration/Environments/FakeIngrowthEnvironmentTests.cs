using FluentAssertions;
using Intellias.CQRS.Configuration.Environments;
using Intellias.CQRS.Tests.Core.Configuration;
using Xunit;

namespace Intellias.CQRS.Tests.Configuration.Environments
{
    public class FakeIngrowthEnvironmentTests
    {
        private readonly FakeIngrowthEnvironment environment;

        public FakeIngrowthEnvironmentTests()
        {
            environment = new FakeIngrowthEnvironment(IngrowthEnvironment.Local);
        }

        [Fact]
        public void IsLocal_Local_True()
        {
            environment.SetEnvironmentName(IngrowthEnvironment.Local);

            environment.IsLocal()
                .Should()
                .BeTrue();
        }

        [Fact]
        public void IsDevelopment_Dev_True()
        {
            environment.SetEnvironmentName(IngrowthEnvironment.Development);

            environment.IsDevelopment()
                .Should()
                .BeTrue();
        }

        [Fact]
        public void IsStaging_Stage_True()
        {
            environment.SetEnvironmentName(IngrowthEnvironment.Staging);

            environment.IsStaging()
                .Should()
                .BeTrue();
        }

        [Fact]
        public void IsProduction_Prod_True()
        {
            environment.SetEnvironmentName(IngrowthEnvironment.Production);

            environment.IsProduction()
                .Should()
                .BeTrue();
        }

        [Theory]
        [InlineData(IngrowthEnvironment.Local)]
        [InlineData(IngrowthEnvironment.Development)]
        [InlineData(IngrowthEnvironment.Testing)]
        [InlineData(IngrowthEnvironment.Staging)]
        [InlineData(IngrowthEnvironment.Production)]
        public void IsEnvironment_EnvironmentName_True(string environmentName)
        {
            environment.SetEnvironmentName(environmentName);

            environment.IsEnvironment(environmentName)
                .Should()
                .BeTrue();
        }
    }
}
