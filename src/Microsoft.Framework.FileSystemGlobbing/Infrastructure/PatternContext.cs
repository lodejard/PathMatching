using System;
using System.Collections.Generic;

namespace Microsoft.Framework.FileSystemGlobbing.Abstractions
{
    public class PatternContext
    {
        public PatternContext(MatcherContext matcherContext, Pattern pattern)
        {
            MatcherContext = matcherContext;
            Pattern = pattern;
        }

        public MatcherContext MatcherContext { get; }
        public Pattern Pattern { get; }
        public FrameData Frame { get; private set; }
        public Stack<FrameData> FrameStack { get; } = new Stack<FrameData>();

        public LiteralPathSegment LiteralPathSegment { get { return Frame.Segment as LiteralPathSegment; } }

        public void PredictInclude()
        {
            if (Frame.PatternNotApplicable) { return; }

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

        public bool TestIncludeFile(FileInfoBase fileInfo)
        {
            if (Frame.PatternNotApplicable) { return false; }

            var literal = LiteralPathSegment;
            if (literal != null)
            {
                if (string.Equals(literal.Value, fileInfo.Name, StringComparison.Ordinal))
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public bool TestExcludeFile(FileInfoBase fileInfo)
        {
            if (Frame.PatternNotApplicable) { return false; }

            throw new NotImplementedException();
        }

        public bool TestIncludeDirectory(DirectoryInfoBase directoryInfo)
        {
            if (Frame.PatternNotApplicable) { return false; }

            return TestMatchingDirectory(directoryInfo);
        }

        public bool TestExcludeDirectory(DirectoryInfoBase directoryInfo)
        {
            if (Frame.PatternNotApplicable) { return false; }

            return TestMatchingDirectory(directoryInfo);
        }

        public bool TestMatchingDirectory(DirectoryInfoBase directoryInfo)
        {
            var literal = LiteralPathSegment;
            if (literal != null)
            {
                if (string.Equals(literal.Value, directoryInfo.Name, StringComparison.Ordinal))
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public void PushFrame(int segmentIndex)
        {
            if (Frame.PatternNotApplicable)
            {
                PushFrame(new FrameData { PatternNotApplicable = true });
                return;
            }
            var frame = new FrameData { SegmentIndex = segmentIndex };
            if (frame.SegmentIndex < Pattern.Segments.Count)
            {
                frame.Segment = Pattern.Segments[segmentIndex];
            }
            PushFrame(frame);
        }

        public void PushFrame(DirectoryInfoBase directoryInfo)
        {
            if (FrameStack.Count == 0)
            {
                PushFrame(0);
                return;
            }
            if (Frame.PatternNotApplicable || 
                !TestMatchingDirectory(directoryInfo))
            {
                PushFrame(new FrameData { PatternNotApplicable = true });
                return;
            }

            var frame = new FrameData { SegmentIndex = Frame.SegmentIndex + 1 };
            if (frame.SegmentIndex < Pattern.Segments.Count)
            {
                frame.Segment = Pattern.Segments[frame.SegmentIndex];
            }
            PushFrame(frame);
        }

        public void PushFrame(FrameData frame)
        {
            FrameStack.Push(frame);
            Frame = frame;
        }

        public void PopFrame()
        {
            Frame = FrameStack.Pop();
        }

        public struct FrameData
        {
            public bool PatternNotApplicable;

            public int SegmentIndex;
            public PatternSegment Segment;
        }
    }

}