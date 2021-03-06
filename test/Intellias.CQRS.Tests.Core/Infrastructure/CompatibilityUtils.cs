﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using LibGit2Sharp;
using Xunit;
using Xunit.Abstractions;

namespace Intellias.CQRS.Tests.Core.Infrastructure
{
    /// <summary>
    /// Compatibility Utils.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CompatibilityUtils
    {
        private const string BasePath = "https://IntelliasTS@dev.azure.com/IntelliasTS/IntelliGrowth/_git/";
        private readonly TestsConfiguration testsConfiguration;
        private readonly List<string> sourceFiles;
        private readonly ITestOutputHelper output;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompatibilityUtils"/> class.
        /// </summary>
        /// <param name="output">ITestOutputHelper.</param>
        public CompatibilityUtils(ITestOutputHelper output)
        {
            var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var basePath = Path.GetFullPath(Path.Combine(currentPath, "..", "..", "..", "..", ".."));
            sourceFiles = GetProjectFiles(basePath);
            testsConfiguration = new TestsConfiguration();
            this.output = output;
        }

        /// <summary>
        /// Repo Consistency Test.
        /// </summary>
        /// <param name="startWith">startWith.</param>
        /// <param name="repoName">repoName.</param>
        public void RepoConsistencyTest(string startWith, string repoName)
        {
            var repoPath = Path.Combine("../../repos", repoName);
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
                    var packages = GetPackages(projectFile, startWith);
                    if (packages.Any())
                    {
                        var packagesToRemove = packages.Aggregate((i, j) => i + " " + j);
                        DotNet($"remove {projectFile} reference {packagesToRemove}");

                        var projectsRefsToAdd = packages.Select(p => sourceFiles.Single(f => f.Contains($"{p}.csproj"))).Aggregate((i, j) => i + " " + j);
                        DotNet($"add {projectFile} reference {projectsRefsToAdd}");

                        projectsToAdd.Append($"{projectsRefsToAdd} ");
                    }
                }

                DotNet($"sln {solutionFile} add {projectsToAdd}");

                DotNet($"build {solutionFile}");

                var testProjects = projectFiles
                    .Where(x =>
                     {
                         var testProjectFileName = Path.GetFileName(x);
                         return testProjectFileName != null
                             && testProjectFileName.Contains("Tests")
                             && !testProjectFileName.Contains("Compatibility");
                     });

                foreach (var testProject in testProjects)
                {
                    DotNet($"test {testProject}");
                }
            }
            finally
            {
                DeleteDirectory(repoPath);
            }
        }

        private static List<string> GetPackages(string projectFile, string startWith)
        {
            if (string.IsNullOrWhiteSpace(startWith))
            {
                throw new ArgumentNullException(nameof(startWith));
            }

            var packages = new List<string>();
            var doc = new XmlDocument();
            doc.Load(projectFile);

            var packageReferences = doc.GetElementsByTagName("PackageReference")
                .Cast<XmlNode>()
                .ToList();

            foreach (var packageName in packageReferences
                .Where(x => x.Attributes != null)
                .Select(x => x.Attributes["Include"])
                .Select(x => x.Value))
            {
                if (packageName != null && packageName.StartsWith(startWith, StringComparison.InvariantCultureIgnoreCase))
                {
                    packages.Add(packageName);
                }
            }

            return packages;
        }

        private static List<string> GetProjectFiles(string path)
        {
            var files = new List<string>();

            if (Directory.Exists(path) && !path.Contains("_git2_"))
            {
                var dirs = Directory.GetDirectories(path).Select(x => Path.GetFullPath(x)).ToList();
                foreach (var dir in dirs)
                {
                    if (Directory.Exists(dir))
                    {
                        files.AddRange(GetProjectFiles(dir));
                    }
                }

                files.AddRange(dirs
                    .Where(dir => Directory.Exists(dir))
                    .Where(dir => !dir.Contains("_git2_"))
                    .SelectMany(dir => Directory.GetFiles(dir, "*.csproj"))
                    .ToList());
            }

            return files;
        }

        private void DeleteDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }

            var files = ExecuteSkippingExceptions(() => Directory.GetFiles(directory), Array.Empty<string>());
            foreach (var file in files)
            {
                ExecuteSkippingExceptions(() =>
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                });
            }

            var dirs = ExecuteSkippingExceptions(() => Directory.GetDirectories(directory), Array.Empty<string>());
            foreach (var dir in dirs)
            {
                DeleteDirectory(dir);
            }

            ExecuteSkippingExceptions(() => Directory.Delete(directory, false));
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
                LibGit2Sharp.Commands.Checkout(repo, master);
            }
        }

        private void ExecuteSkippingExceptions(Action execute)
        {
            try
            {
                execute();
            }
            catch (Exception exception)
            {
                LogSkippedException(exception);
            }
        }

        private TResult ExecuteSkippingExceptions<TResult>(Func<TResult> execute, TResult defaultValue)
        {
            try
            {
                return execute();
            }
            catch (Exception exception)
            {
                LogSkippedException(exception);
                return defaultValue;
            }
        }

        private void LogSkippedException(Exception exception)
        {
            this.output.WriteLine($"Skipping exception '{exception.GetType().Name}: {exception.Message}'.");
        }
    }
}
