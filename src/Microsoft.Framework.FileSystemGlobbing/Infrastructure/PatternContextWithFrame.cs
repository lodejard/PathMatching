
using System.Collections.Generic;

namespace Microsoft.Framework.FileSystemGlobbing.Infrastructure
{
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
}
