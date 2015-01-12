using System;
using Microsoft.Framework.FileSystemGlobbing.Abstractions;

namespace Microsoft.Framework.FileSystemGlobbing.Tests.Infrastructure
{
    public class FileInfoStub : FileInfoBase
    {
        public FileInfoStub(
            SystemIoRecorder recorder, 
            DirectoryInfoBase parentDirectory,
            string fullName, 
            string name)
        {
            Recorder = recorder;
            FullName = fullName;
            Name = name;
        }

        public SystemIoRecorder Recorder { get; }

        public override DirectoryInfoBase ParentDirectory { get; }

        public override string FullName { get; }

        public override string Name { get; }
    }
}