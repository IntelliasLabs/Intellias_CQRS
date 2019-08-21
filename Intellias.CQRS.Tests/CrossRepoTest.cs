using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using LibGit2Sharp;
using Xunit;

namespace Intellias.CQRS.Tests
{
    /// <summary>
    /// CrossRepoTest
    /// </summary>
    public class CrossRepoTest
    {
        const string basePath = "https://IntelliasTS@dev.azure.com/IntelliasTS/IntelliGrowth/_git/";

        /// <summary>
        /// CrossRepoTest
        /// </summary>
        public CrossRepoTest()
        {
            
        }

        /// <summary>
        /// IntelliGrowthCompetenciesTest
        /// </summary>
        [Fact]
        public void IntelliGrowthCompetenciesTest()
        {
            //CloneRepo("IntelliGrowth_Competencies");
            var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var basePath = Path.GetFullPath(Path.Combine(currentPath, @"..\..\..\..\"));
            var sourceDirs = Directory.GetDirectories(basePath);
            var sourceFiles = sourceDirs.SelectMany(dir => Directory.GetFiles(dir, "*.csproj")).ToList();

            var dirs = Directory.GetDirectories($"repos/IntelliGrowth_Competencies");
            var projectFiles = dirs.SelectMany(dir => Directory.GetFiles(dir, "*.csproj")).ToList();

            // Solution file
            // dotnet sln todo.sln add todo-app/todo-app.csproj

            var slnPath = Directory.GetFiles($"repos/IntelliGrowth_Competencies", "*.sln").Single();

            foreach (var projectFile in projectFiles)
            {
                var projectDir = Path.GetDirectoryName(projectFile);
                var doc = new XmlDocument();
                doc.Load(projectFile);

                var packageReferences = doc.GetElementsByTagName("PackageReference").Cast<XmlNode>().ToList();

                foreach (var packageReference in packageReferences)
                {
                    var packageName = packageReference.Attributes["Include"].Value;
                    if (packageName.StartsWith("Intellias.CQRS.", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var sourceProject = sourceFiles.SingleOrDefault(f => f.Contains($"{packageName}.csproj", StringComparison.InvariantCultureIgnoreCase));
                        if (sourceProject != null)
                        {
                            var reference = doc.CreateElement("ProjectReference");
                            var relativePath = Path.GetRelativePath(projectDir, sourceProject);
                            reference.SetAttribute("Include", relativePath);

                            packageReference.ParentNode.ReplaceChild(reference, packageReference);

                            var process = Process.Start("dotnet", $"sln {slnPath} add {sourceProject}");
                            process.WaitForExit();
                        }
                    }
                }
                doc.Save(projectFile);
            }
        }

        private static void CloneRepo(string name)
        {
            var co = new CloneOptions
            {
                CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                {
                    Username = "sseletskyi@intellias.com",
                    Password = "wXdr56dr2"
                }
            };
            Repository.Clone($"{basePath}{name}", $"repos/{name}", co);
        }
    }
}
