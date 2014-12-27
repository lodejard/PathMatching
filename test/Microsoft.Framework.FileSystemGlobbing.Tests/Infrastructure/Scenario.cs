using Microsoft.Framework.FileSystemGlobbing.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Microsoft.Framework.FileSystemGlobbing.Tests.Infrastructure
{
    public class Scenario
    {
        public Scenario(string basePath)
        {
            BasePath = basePath;
            DirectoryInfo = new DirectoryInfoStub(
                fullName: BasePath, 
                name: ".",
                paths: new string[0]);
        }

        public Matcher PatternMatching { get; set; } = new Matcher();
        public DirectoryInfoStub DirectoryInfo { get; set; } 
        public string BasePath { get; set; }
        public PatternMatchingResult Result { get; set; }

        public Scenario Include(params string[] patterns)
        {
            foreach (var pattern in patterns)
            {
                PatternMatching.AddInclude(pattern);
            }
            return this;
        }

        public Scenario Exclude(params string[] patterns)
        {
            foreach (var pattern in patterns)
            {
                PatternMatching.AddExclude(pattern);
            }
            return this;
        }

        public Scenario Files(params string[] files)
        {
            DirectoryInfo = new DirectoryInfoStub(
                DirectoryInfo.FullName,
                DirectoryInfo.Name,
                DirectoryInfo.Paths.Concat(files.Select(file => BasePath + file)).ToArray());
            return this;
        }

        public Scenario Execute()
        {
            Result = PatternMatching.Execute(DirectoryInfo);
            return this;
        }

        public Scenario AssertExact(params string[] files)
        {
            Assert.Subset(new HashSet<string>(files), new HashSet<string>(Result.Files));
            Assert.Superset(new HashSet<string>(files), new HashSet<string>(Result.Files));
            return this;
        }

    }
}