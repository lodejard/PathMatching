using System;

namespace Microsoft.Framework.FileSystemGlobbing.Abstractions
{
    public class PatternContext
    {
        public PatternContext(MatcherContext matcherContext, Pattern pattern)
        {
            MatcherContext = matcherContext;
            Pattern = pattern;
            PushFrame(0);
        }

        public MatcherContext MatcherContext { get; }
        public Pattern Pattern { get; }
        public Frame Focus { get; private set; }

        public LiteralPathSegment LiteralPathSegment { get { return Focus.Segment as LiteralPathSegment; } }

        public void PredictInclude()
        {
            var literal = LiteralPathSegment;
            if (literal != null)
            {
                MatcherContext.AddPredictIncludeLiteral(literal.Value);
                return;
            }
        }

        public void PredictExclude(MatcherContext.FrameData frame)
        {
            throw new NotImplementedException();
        }

        public bool TestIncludeFile()
        {
            var literal = LiteralPathSegment;
            if (literal != null)
            {
                if (string.Equals(literal.Value, MatcherContext.Frame.FileInfo.Name, StringComparison.Ordinal))
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public bool TestExcludeFile()
        {
            throw new NotImplementedException();
        }

        public void PushFrame(int segmentIndex)
        {
            var focus = new Frame { SegmentIndex = segmentIndex };
            if (segmentIndex < Pattern.Segments.Count)
            {
                focus.Segment = Pattern.Segments[segmentIndex];
            }
            Focus = focus;
        }

        public struct Frame
        {
            public int SegmentIndex;
            public PatternSegment Segment;
        }

    }

}