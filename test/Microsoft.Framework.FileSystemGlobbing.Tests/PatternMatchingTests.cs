using Microsoft.Framework.FileSystemGlobbing.Tests.Infrastructure;
using System;
using System.Linq;
using Xunit;

namespace Microsoft.Framework.FileSystemGlobbing.Tests
{
    public class PatternMatchingTests
    {
        [Fact]
        public void EmptyCollectionWhenNoFilesPresent()
        {
            var scenario = new Scenario(@"c:\files\")
                .Include("alpha.txt")
                .Execute();

            scenario.AssertExact();
        }

        [Fact]
        public void MatchingFileIsFound()
        {
            var scenario = new Scenario(@"c:\files\")
                .Include("alpha.txt")
                .Files("alpha.txt")
                .Execute();

            scenario.AssertExact("alpha.txt");
        }

        [Fact]
        public void MismatchedFileIsIgnored()
        {
            var scenario = new Scenario(@"c:\files\")
                .Include("alpha.txt")
                .Files("omega.txt")
                .Execute();

            scenario.AssertExact();
        }

        [Fact]
        public void FolderNamesAreTraversed()
        {
            var scenario = new Scenario(@"c:\files\")
                .Include("beta/alpha.txt")
                .Files("beta/alpha.txt")
                .Execute();

            scenario.AssertExact("beta/alpha.txt");
        }

        [Theory]
        [InlineData(@"beta/alpha.txt", @"beta/alpha.txt")]
        [InlineData(@"beta\alpha.txt", @"beta/alpha.txt")]
        [InlineData(@"beta/alpha.txt", @"beta\alpha.txt")]
        [InlineData(@"beta\alpha.txt", @"beta\alpha.txt")]
        public void SlashPolarityIsIgnored(string includePattern, string filePath)
        {
            var scenario = new Scenario(@"c:\files\")
                .Include(includePattern)
                .Files("one/two.txt", filePath, "three/four.txt")
                .Execute();

            scenario.AssertExact("beta/alpha.txt");
        }

        [Theory]
        [InlineData(@"*.txt", new[] { "alpha.txt", "beta.txt" })]
        [InlineData(@"alpha.*", new[] { "alpha.txt" })]
        [InlineData(@"*.*", new[] { "alpha.txt", "beta.txt", "gamma.dat" })]
        [InlineData(@"*", new[] { "alpha.txt", "beta.txt", "gamma.dat" })]
        [InlineData(@"*et*", new[] { "beta.txt" })]
        [InlineData(@"b*et*t", new[] { "beta.txt" })]
        [InlineData(@"b*et*x", new string[0])]
        public void PatternMatchingWorks(string includePattern, string[] matchesExpected)
        {
            var scenario = new Scenario(@"c:\files\")
                .Include(includePattern)
                .Files("alpha.txt", "beta.txt", "gamma.dat")
                .Execute();

            scenario.AssertExact(matchesExpected);
        }

        [Theory]
        [InlineData(@"1234*5678", new[] { "12345678" })]
        [InlineData(@"12345*5678", new string[0])]
        [InlineData(@"12*3456*78", new[] { "12345678" })]
        [InlineData(@"12*23*", new string[0])]
        [InlineData(@"*67*78", new string[0])]
        [InlineData(@"*45*56", new string[0])]
        public void PatternBeginAndEndCantOverlap(string includePattern, string[] matchesExpected)
        {
            var scenario = new Scenario(@"c:\files\")
                .Include(includePattern)
                .Files("12345678")
                .Execute();

            scenario.AssertExact(matchesExpected);
        }


        [Theory]
        [InlineData(@"*mm*/*", new[] { "gamma/hello.txt" })]
        [InlineData(@"*alpha*/*", new[] { "alpha/hello.txt" })]
        [InlineData(@"*/*", new[] { "alpha/hello.txt", "beta/hello.txt", "gamma/hello.txt" })]
        [InlineData(@"*.*/*", new[] { "alpha/hello.txt", "beta/hello.txt", "gamma/hello.txt" })]
        public void PatternMatchingWorksInFolders(string includePattern, string[] matchesExpected)
        {
            var scenario = new Scenario(@"c:\files\")
                .Include(includePattern)
                .Files("alpha/hello.txt", "beta/hello.txt", "gamma/hello.txt")
                .Execute();

            scenario.AssertExact(matchesExpected);
        }
        [Fact]
        public void StarDotStarIsSameAsStar()
        {
            var scenario = new Scenario(@"c:\files\")
                .Include("*.*")
                .Files("alpha.txt", "alpha.", ".txt", ".", "alpha", "txt")
                .Execute();

            scenario.AssertExact("alpha.txt", "alpha.", ".txt", ".", "alpha", "txt");
        }

    }
}
