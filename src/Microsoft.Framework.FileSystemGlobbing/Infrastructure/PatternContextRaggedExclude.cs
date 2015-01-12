
using Microsoft.Framework.FileSystemGlobbing.Abstractions;

namespace Microsoft.Framework.FileSystemGlobbing.Infrastructure
{
    public class PatternContextRaggedExclude : PatternContextRagged
    {
        public PatternContextRaggedExclude(MatcherContext matcherContext, Pattern pattern) : base(matcherContext, pattern)
        {
        }

        public override bool Test(FileInfoBase file)
        {
            if (Frame.IsNotApplicable) { return false; }

            return IsEndsWith && TestMatchingGroup(file);
        }

        public override bool Test(DirectoryInfoBase directory)
        {
            if (Frame.IsNotApplicable) { return false; }

            if (IsEndsWith && TestMatchingGroup(directory))
            {
                // directory excluded with file-like pattern
                return true;
            }
            if (Pattern.EndsWith.Count == 0 &&
                Frame.SegmentGroupIndex == Pattern.Contains.Count - 1 &&
                TestMatchingGroup(directory))
            {
                // directory excluded by matching up to final '/**'
                return true;
            }
            return false;
        }
    }
}
