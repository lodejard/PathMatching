using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Framework.FileSystemGlobbing.Abstractions
{
    public class DirectoryInfoWrapper : DirectoryInfoBase
    {
        private DirectoryInfo DirectoryInfo;

        public DirectoryInfoWrapper(DirectoryInfo directoryInfo)
        {
            DirectoryInfo = directoryInfo;
        }

        public override IEnumerable<FileSystemInfoBase> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption)
        {
            foreach (var fileSystemInfo in DirectoryInfo.EnumerateFileSystemInfos(searchPattern, searchOption))
            {
                var directoryInfo = fileSystemInfo as DirectoryInfo;
                if (directoryInfo != null)
                {
                    yield return new DirectoryInfoWrapper(directoryInfo);
                }
                else
                {
                    yield return new FileInfoWrapper((FileInfo)fileSystemInfo);
                }
            }
        }

        public override string Name
        {
            get { return DirectoryInfo.Name; }
        }

        public override string FullName
        {
            get { return DirectoryInfo.FullName; }
        }

    }
}
