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
    }
}
