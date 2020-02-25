using System;
using FluentAssertions;
using Intellias.CQRS.Configuration.Environments;
using Xunit;

namespace Intellias.CQRS.Tests.Configuration.Environments
{
    public class IngrowthEnvironmentTests
    {
        private const string VariableName = "INGROWTH_ENVIRONMENT";
        private readonly IngrowthHostEnvironment environment;

        public IngrowthEnvironmentTests()
        {
            environment = new IngrowthHostEnvironment();
        }

        [Fact]
        public void IsLocal_Local_True()
        {
            Environment.SetEnvironmentVariable(VariableName, IngrowthHostEnvironment.Local);

            environment.IsLocal()
                .Should()
                .BeTrue();
        }

        [Fact]
        public void IsDevelopment_Dev_True()
        {
            Environment.SetEnvironmentVariable(VariableName, IngrowthHostEnvironment.Development);

            environment.IsDevelopment()
                .Should()
                .BeTrue();
        }

        [Fact]
        public void IsStaging_Stage_True()
        {
            Environment.SetEnvironmentVariable(VariableName, IngrowthHostEnvironment.Staging);

            environment.IsStaging()
                .Should()
                .BeTrue();
        }

        [Fact]
        public void IsProduction_Prod_True()
        {
            Environment.SetEnvironmentVariable(VariableName, IngrowthHostEnvironment.Production);

            environment.IsProduction()
                .Should()
                .BeTrue();
        }

        [Theory]
        [InlineData(IngrowthHostEnvironment.Local)]
        [InlineData(IngrowthHostEnvironment.Development)]
        [InlineData(IngrowthHostEnvironment.Testing)]
        [InlineData(IngrowthHostEnvironment.Staging)]
        [InlineData(IngrowthHostEnvironment.Production)]
        public void IsEnvironment_EnvironmentName_True(string environmentName)
        {
            Environment.SetEnvironmentVariable(VariableName, environmentName);

            environment.IsEnvironment(environmentName)
                .Should()
                .BeTrue();
        }
    }
}
