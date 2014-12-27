using System;

namespace Microsoft.Framework.FileSystemGlobbing.Abstractions
{
    public abstract class PatternSegment
    {
        public virtual void PredictInclude(MatcherContext.FrameData frame)
        {
        }
    }

    public class RecursiveWildcardSegment : PatternSegment
    {

    }

    public class ParentPathSegment : PatternSegment
    {

    }

    public class CurrentPathSegment : PatternSegment
    {

    }

    public class LiteralPathSegment : PatternSegment
    {
        public LiteralPathSegment(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}

