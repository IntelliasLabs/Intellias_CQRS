﻿using System;
using FluentAssertions;
using Intellias.CQRS.Configuration.Environments;
using Xunit;

namespace Intellias.CQRS.Tests.Configuration.Environments
{
    public class IngrowthEnvironmentTests
    {
        private const string VariableName = "INGROWTH_ENVIRONMENT";
        private readonly IngrowthEnvironment environment;

        public IngrowthEnvironmentTests()
        {
            environment = new IngrowthEnvironment();
        }

        [Fact]
        public void IsLocal_Local_True()
        {
            Environment.SetEnvironmentVariable(VariableName, IngrowthEnvironment.Local);

            environment.IsLocal()
                .Should()
                .BeTrue();
        }

        [Fact]
        public void IsDevelopment_Dev_True()
        {
            Environment.SetEnvironmentVariable(VariableName, IngrowthEnvironment.Development);

            environment.IsDevelopment()
                .Should()
                .BeTrue();
        }

        [Fact]
        public void IsStaging_Stage_True()
        {
            Environment.SetEnvironmentVariable(VariableName, IngrowthEnvironment.Staging);

            environment.IsStaging()
                .Should()
                .BeTrue();
        }

        [Fact]
        public void IsProduction_Prod_True()
        {
            Environment.SetEnvironmentVariable(VariableName, IngrowthEnvironment.Production);

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
            Environment.SetEnvironmentVariable(VariableName, environmentName);

            environment.IsEnvironment(environmentName)
                .Should()
                .BeTrue();
        }
    }
}
