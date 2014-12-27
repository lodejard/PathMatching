using System;
using Microsoft.Framework.FileSystemGlobbing.Abstractions;

namespace Microsoft.Framework.FileSystemGlobbing.Tests.Infrastructure
{
    public class FileInfoStub : FileInfoBase
    {
        public FileInfoStub(string fullName, string name)
        {
            FullName = fullName;
            Name = name;
        }

        public override string FullName { get; }

        public override string Name { get; }
    }
}