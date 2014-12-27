using System;
using System.Collections.Generic;

namespace Microsoft.Framework.FileSystemGlobbing.Abstractions
{
    public class PatternMatchingResult
    {
        public PatternMatchingResult(IEnumerable<string> files)
        {
            Files = files;
        }

        public IEnumerable<string> Files { get; set; }
    }
}