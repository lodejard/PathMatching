using System;

namespace Microsoft.Framework.FileSystemGlobbing.Abstractions
{
    public abstract class FileSystemInfoBase
    {
        public abstract string Name { get; }
        public abstract string FullName { get; }
    }
}