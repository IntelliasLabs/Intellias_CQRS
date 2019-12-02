using Intellias.CQRS.Tests.Core.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Intellias.CQRS.Compatibility.Tests
{
    public class CrossRepoTest
    {
        private readonly ITestOutputHelper output;

        public CrossRepoTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData("IntelliGrowth_Identity")]
        [InlineData("IntelliGrowth_Competencies")]
        [InlineData("IntelliGrowth_JobProfiles")]
        [InlineData("IntelliGrowth_Feedbacks")]
        [InlineData("IntelliGrowth_API")]
        public void RepoConsistencyTest(string repoName)
        {
            var test = new CompatibilityUtils(output);
            test.RepoConsistencyTest("Intellias.CQRS.", repoName);
        }
    }
}