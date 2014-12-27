using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Framework.FileSystemGlobbing.Abstractions
{
    public abstract class DirectoryInfoBase : FileSystemInfoBase
    {
        public abstract IEnumerable<FileSystemInfoBase> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption);
    }
}