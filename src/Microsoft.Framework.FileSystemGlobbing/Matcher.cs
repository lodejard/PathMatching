using Microsoft.Framework.FileSystemGlobbing.Abstractions;
using System;
using System.Collections.Generic;

namespace Microsoft.Framework.FileSystemGlobbing.Infrastructure
{
    public class Matcher
    {
        public IList<Pattern> IncludePatterns = new List<Pattern>();
        public IList<Pattern> ExcludePatterns = new List<Pattern>();

        public Matcher AddInclude(string pattern)
        {
            IncludePatterns.Add(new Pattern(pattern));
            return this;
        }

        public Matcher AddExclude(string pattern)
        {
            ExcludePatterns.Add(new Pattern(pattern));
            return this;
        }

        public PatternMatchingResult Execute(DirectoryInfoBase directoryInfo)
        {
            var context = new MatcherContext(this, directoryInfo);
            return context.Execute();
        }
    }
}
