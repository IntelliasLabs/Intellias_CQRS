using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Intellias.CQRS.Tests.Utils;
using LibGit2Sharp;
using Xunit;
using Xunit.Abstractions;

namespace Intellias.CQRS.Compatibility.Tests
{
    public class CrossRepoTest
    {
        private const string BasePath = "https://IntelliasTS@dev.azure.com/IntelliasTS/IntelliGrowth/_git/";
        private readonly TestsConfiguration testsConfiguration;
        private readonly List<string> sourceFiles;
        private readonly ITestOutputHelper output;

        public CrossRepoTest(ITestOutputHelper output)
        {
            var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var basePath = Path.GetFullPath(Path.Combine(currentPath, "..", "..", "..", "..", ".."));
            sourceFiles = GetProjectFiles(basePath);
            testsConfiguration = new TestsConfiguration();
            this.output = output;
        }

        [Theory]
        [InlineData("IntelliGrowth_Identity")]
        [InlineData("IntelliGrowth_Competencies")]
        [InlineData("IntelliGrowth_JobProfiles")]
        [InlineData("IntelliGrowth_API")]
        public void RepoConsistencyTest(string repoName)
        {
            var repoPath = Path.Combine("repos", repoName);
            if (Directory.Exists(repoPath))
            {
                DeleteDirectory(repoPath);
            }

            try
            {
                CloneRepo(repoName, repoPath);
                var projectFiles = GetProjectFiles(repoPath);

                var solutionFile = Directory.GetFiles(repoPath, "*.sln").Single();

                var projectsToAdd = new StringBuilder();
                foreach (var projectFile in projectFiles)
                {
                    var packages = GetPackages(projectFile, "Intellias.CQRS.");
                    if (packages.Any())
                    {
                        var packagesToRemove = packages.Aggregate((i, j) => i + " " + j);
                        DotNet($"remove {projectFile} reference {packagesToRemove}");

                        var projectsRefsToAdd = packages.Select(p => sourceFiles.Single(f => f.Contains($"{p}.csproj", StringComparison.InvariantCultureIgnoreCase))).Aggregate((i, j) => i + " " + j);
                        DotNet($"add {projectFile} reference {projectsRefsToAdd}");

                        projectsToAdd.Append($"{projectsRefsToAdd} ");
                    }
                }

                DotNet($"sln {solutionFile} add {projectsToAdd}");

                DotNet($"build {solutionFile}");
                DotNet($"test {solutionFile}");
            }
            finally
            {
                DeleteDirectory(repoPath);
            }
        }

        private static List<string> GetPackages(string projectFile, string startWith)
        {
            var packages = new List<string>();
            var doc = new XmlDocument();
            doc.Load(projectFile);

            var packageReferences = doc.GetElementsByTagName("PackageReference").Cast<XmlNode>().ToList();
            foreach (var packageReference in packageReferences)
            {
                var packageName = packageReference.Attributes["Include"].Value;
                if (packageName.StartsWith(startWith, StringComparison.InvariantCultureIgnoreCase))
                {
                    packages.Add(packageName);
                }
            }

            return packages;
        }

        private static List<string> GetProjectFiles(string path)
        {
            var files = new List<string>();

            var dirs = Directory.GetDirectories(path).Select(x => Path.GetFullPath(x)).ToList();
            foreach (var dir in dirs)
            {
                files.AddRange(GetProjectFiles(dir));
            }

            files.AddRange(dirs.SelectMany(dir => Directory.GetFiles(dir, "*.csproj")).ToList());

            return files;
        }

        private static void DeleteDirectory(string directory)
        {
            var files = Directory.GetFiles(directory);
            var dirs = Directory.GetDirectories(directory);

            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (var dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(directory, false);
        }

        private void DotNet(string args)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = "dotnet";
                process.StartInfo.Arguments = args;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        output.WriteLine(e.Data);
                        Trace.WriteLine(e.Data);
                    }
                };
                process.Start();
                process.BeginOutputReadLine();
                if (!process.WaitForExit(60 * 1000))
                {
                    process.Kill();
                }

                Assert.Equal(0, process.ExitCode);
            }
        }

        private void CloneRepo(string name, string repoPath)
        {
            var accessToken = Environment.GetEnvironmentVariable("SYSTEM_ACCESSTOKEN") ?? testsConfiguration.AzureDevOpsAccessToken;

            var co = new CloneOptions
            {
                CredentialsProvider = (url, user, cred) => new UsernamePasswordCredentials
                {
                    Username = accessToken,
                    Password = accessToken
                }
            };

            var ro = new RepositoryOptions
            {
                Identity = new Identity(accessToken, "test@test.com"),
                WorkingDirectoryPath = repoPath
            };

            Directory.CreateDirectory(repoPath);
            var repoLink = Repository.Clone($"{BasePath}{name}", repoPath, co);

            using (var repo = new Repository(repoLink, ro))
            {
                var master = repo.Branches["master"];
                Commands.Checkout(repo, master);
            }
        }
    }
}