using System;
using System.Collections.Generic;

namespace Microsoft.Framework.FileSystemGlobbing.Abstractions
{
    public class Pattern
    {
        public Pattern(string pattern)
        {
            var endPattern = pattern.Length;
            for (int scanPattern = 0; scanPattern < endPattern;)
            {
                var beginSegment = scanPattern;
                var endSegment = NextIndex(pattern, new[] { '/', '\\' }, beginSegment, endPattern);

                PatternSegment segment = null;

                if (segment == null && endSegment - beginSegment == 2)
                {
                    if (pattern[beginSegment] == '*' && pattern[beginSegment + 1] == '*')
                    {
                        segment = new RecursiveWildcardSegment();
                    }
                    else if (pattern[beginSegment] == '.' && pattern[beginSegment + 1] == '.')
                    {
                        segment = new ParentPathSegment();
                    }
                }

                if (segment == null && endSegment - beginSegment == 1)
                {
                    if (pattern[beginSegment] == '.')
                    {
                        segment = new CurrentPathSegment();
                    }
                }

                if (segment == null)
                {
                    segment = new LiteralPathSegment(pattern.Substring(beginSegment, endSegment - beginSegment));
                }

                Segments.Add(segment);

                scanPattern = endSegment + 1;
            }
        }

        public IList<PatternSegment> Segments { get; } = new List<PatternSegment>();


        private int NextIndex(string pattern, char[] anyOf, int startIndex, int endIndex)
        {
            var index = pattern.IndexOfAny(anyOf, startIndex, endIndex - startIndex);
            return index == -1 ? endIndex : index;
        }
    }
}