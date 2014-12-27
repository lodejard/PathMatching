using System;
using System.IO;

namespace Microsoft.Framework.FileSystemGlobbing.Abstractions
{
    public class FileInfoWrapper : FileInfoBase
    {
        private FileInfo FileInfo;

        public FileInfoWrapper(FileInfo fileInfo)
        {
            FileInfo = fileInfo;
        }

        public override string Name
        {
            get { return FileInfo.Name; }
        }

        public override string FullName
        {
            get { return FileInfo.FullName; }
        }
    }

}