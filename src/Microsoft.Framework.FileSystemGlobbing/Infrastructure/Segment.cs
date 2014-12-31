using System;
using System.Collections.Generic;

namespace Microsoft.Framework.FileSystemGlobbing.Infrastructure
{
    public abstract class PatternSegment
    {
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

    public class WildcardPathSegment : PatternSegment
    {
        public WildcardPathSegment(string beginsWith, List<string> contains, string endsWith)
        {
            BeginsWith = beginsWith;
            Contains = contains;
            EndsWith = endsWith;
        }

        public string BeginsWith { get; }
        public List<string> Contains { get; }
        public string EndsWith { get; }
    }
}

