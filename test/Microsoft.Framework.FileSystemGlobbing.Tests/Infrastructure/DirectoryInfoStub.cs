using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Framework.FileSystemGlobbing.Abstractions;

namespace Microsoft.Framework.FileSystemGlobbing.Tests.Infrastructure
{
    public class DirectoryInfoStub : DirectoryInfoBase
    {
        public DirectoryInfoStub(string fullName, string name, string[] paths)
        {
            FullName = fullName;
            Name = name;
            Paths = paths;
        }

        public override string FullName { get; }

        public override string Name { get; }

        public string[] Paths { get; }

        public override IEnumerable<FileSystemInfoBase> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption)
        {
            foreach (var path in Paths)
            {
                if (!path.StartsWith(FullName))
                {
                    continue;
                }
                var beginPath = FullName.Length;
                var endPath = path.Length;

                var beginSegment = beginPath;
                var endSegment = NextIndex(path, new[] { '/', '\\' }, beginSegment, path.Length);

                if (endPath == endSegment)
                {
                    yield return new FileInfoStub(
                        fullName: path,
                        name: path.Substring(beginSegment, endSegment - beginSegment));
                }
                else
                {
                    yield return new DirectoryInfoStub(
                        fullName: path.Substring(0, endSegment + 1), 
                        name: path.Substring(beginSegment, endSegment - beginSegment),
                        paths: Paths);
                }
            }
        }

        private int NextIndex(string pattern, char[] anyOf, int startIndex, int endIndex)
        {
            var index = pattern.IndexOfAny(anyOf, startIndex, endIndex - startIndex);
            return index == -1 ? endIndex : index;
        }
    }
}
