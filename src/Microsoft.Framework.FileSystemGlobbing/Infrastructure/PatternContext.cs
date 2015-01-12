using Microsoft.Framework.FileSystemGlobbing.Abstractions;
using System;
using System.Collections.Generic;

namespace Microsoft.Framework.FileSystemGlobbing.Infrastructure
{
    public abstract class PatternContextBase
    {
        public PatternContextBase(MatcherContext matcherContext, Pattern pattern)
        {
            MatcherContext = matcherContext;
            Pattern = pattern;
        }

        public MatcherContext MatcherContext { get; }
        public Pattern Pattern { get; }

        public abstract bool Test(DirectoryInfoBase directory);
        public abstract bool Test(FileInfoBase file);
        public abstract void PushFrame(DirectoryInfoBase directory);
        public abstract void PopFrame();
    }

    public abstract class PatternContextWithFrame<TFrame> : PatternContextBase
    {
        public PatternContextWithFrame(MatcherContext matcherContext, Pattern pattern) : base(matcherContext, pattern)
        {
        }

        public TFrame Frame { get; private set; }
        public Stack<TFrame> FrameStack { get; } = new Stack<TFrame>();

        public void PushFrame(TFrame frame)
        {
            FrameStack.Push(Frame);
            Frame = frame;
        }

        public override void PopFrame()
        {
            Frame = FrameStack.Pop();
        }
    }

    public abstract class PatternContextLinear : PatternContextWithFrame<PatternContextLinear.FrameData>
    {
        public PatternContextLinear(MatcherContext matcherContext, Pattern pattern) : base(matcherContext, pattern)
        {
        }

        public bool IsLastSegment
        {
            get { return Frame.SegmentIndex == Pattern.Segments.Count - 1; }
        }

        public bool TestMatchingSegment(string value)
        {
            if (Frame.SegmentIndex >= Pattern.Segments.Count)
            {
                return false;
            }
            return Pattern.Segments[Frame.SegmentIndex].TestMatchingSegment(value, StringComparison.Ordinal);
        }

        public override void PushFrame(DirectoryInfoBase directory)
        {
            var frame = Frame;
            if (FrameStack.Count == 0)
            {
                // initializing
            }
            else if (Frame.IsNotApplicable)
            {
                // no change
            }
            else if (!TestMatchingSegment(directory.Name))
            {
                // nothing down this path is affected by this pattern
                frame.IsNotApplicable = true;
            }
            else
            {
                // directory matches segment, advance position in pattern
                frame.SegmentIndex = frame.SegmentIndex + 1;
            }

            PushFrame(frame);
        }

        public struct FrameData
        {
            public bool IsNotApplicable;

            public int SegmentIndex;
        }
    }

    public class PatternContextLinearInclude : PatternContextLinear
    {
        public PatternContextLinearInclude(MatcherContext matcherContext, Pattern pattern) : base(matcherContext, pattern)
        {
        }

        public override bool Test(FileInfoBase file)
        {
            if (Frame.IsNotApplicable) { return false; }

            return IsLastSegment && TestMatchingSegment(file.Name);
        }

        public override bool Test(DirectoryInfoBase directory)
        {
            if (Frame.IsNotApplicable) { return false; }

            return !IsLastSegment && TestMatchingSegment(directory.Name);
        }
    }

    public class PatternContextLinearExclude : PatternContextLinear
    {
        public PatternContextLinearExclude(MatcherContext matcherContext, Pattern pattern) : base(matcherContext, pattern)
        {
        }

        public override bool Test(FileInfoBase file)
        {
            if (Frame.IsNotApplicable) { return false; }

            return IsLastSegment && TestMatchingSegment(file.Name);
        }

        public override bool Test(DirectoryInfoBase directory)
        {
            if (Frame.IsNotApplicable) { return false; }

            return IsLastSegment && TestMatchingSegment(directory.Name);
        }
    }

    public abstract class PatternContextRagged : PatternContextWithFrame<PatternContextRagged.FrameData>
    {
        public PatternContextRagged(MatcherContext matcherContext, Pattern pattern) : base(matcherContext, pattern)
        {
        }

        public bool IsStartsWith
        {
            get { return Frame.SegmentGroupIndex == -1; }
        }
        public bool IsEndsWith
        {
            get { return Frame.SegmentGroupIndex == Pattern.Contains.Count; }
        }
        public bool IsContains
        {
            get { return !IsStartsWith && !IsContains; }
        }

        public override void PushFrame(DirectoryInfoBase directory)
        {
            var frame = Frame;
            if (FrameStack.Count == 0)
            {
                // initializing
                frame.SegmentGroupIndex = -1;
                frame.SegmentGroup = Pattern.StartsWith;
            }
            else if (Frame.IsNotApplicable)
            {
                // no change
            }
            else if (IsStartsWith)
            {
                if (!TestMatchingSegment(directory.Name))
                {
                    // nothing down this path is affected by this pattern
                    frame.IsNotApplicable = true;
                }
                else
                {
                    // starting path incrementally satisfied
                    frame.SegmentIndex += 1;
                }
            }
            else
            {
                // increase directory backtrack length
                frame.BacktrackAvailable += 1;
            }

            while (
                frame.SegmentIndex == frame.SegmentGroup.Count &&
                frame.SegmentGroupIndex != Pattern.Contains.Count)
            {
                frame.SegmentGroupIndex += 1;
                frame.SegmentIndex = 0;
                if (frame.SegmentGroupIndex < Pattern.Contains.Count)
                {
                    frame.SegmentGroup = Pattern.Contains[frame.SegmentGroupIndex];
                }
                else
                {
                    frame.SegmentGroup = Pattern.EndsWith;
                }
            }

            PushFrame(frame);
        }

        public bool TestMatchingSegment(string value)
        {
            if (Frame.SegmentIndex >= Frame.SegmentGroup.Count)
            {
                return false;
            }
            return Frame.SegmentGroup[Frame.SegmentIndex].TestMatchingSegment(value, StringComparison.Ordinal);
        }

        public bool TestMatchingGroup(FileSystemInfoBase value)
        {
            var groupLength = Frame.SegmentGroup.Count;
            var backtrackLength = Frame.BacktrackAvailable + 1;
            if (backtrackLength < groupLength)
            {
                return false;
            }

            var scan = value;
            for (int index = 0; index != groupLength; ++index)
            {
                var segment = Frame.SegmentGroup[groupLength - index - 1];
                if (!segment.TestMatchingSegment(scan.Name, StringComparison.Ordinal))
                {
                    return false;
                }
                scan = scan.ParentDirectory;
            }
            return true;
        }

        public struct FrameData
        {
            public bool IsNotApplicable;

            public int SegmentGroupIndex;

            public IList<PatternSegment> SegmentGroup;

            public int BacktrackAvailable;
            public int SegmentIndex;
            public PatternSegment Segment;
        }
    }

    public class PatternContextRaggedInclude : PatternContextRagged
    {
        public PatternContextRaggedInclude(MatcherContext matcherContext, Pattern pattern) : base(matcherContext, pattern)
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
            
            if (IsStartsWith && !TestMatchingSegment(directory.Name))
            {
                return false;
            }
            return true;
        }
    }

    public class PatternContextRaggedExclude : PatternContextRagged
    {
        public PatternContextRaggedExclude(MatcherContext matcherContext, Pattern pattern) : base(matcherContext, pattern)
        {
        }

        public override bool Test(FileInfoBase file)
        {
            if (Frame.IsNotApplicable) { return false; }
            throw new NotImplementedException();
        }

        public override bool Test(DirectoryInfoBase directory)
        {
            if (Frame.IsNotApplicable) { return false; }
            throw new NotImplementedException();
        }
    }
}
