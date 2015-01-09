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
            var names = new HashSet<string>();

            foreach (var path in Paths)
            {
                if (!path.Replace('\\','/').StartsWith(FullName.Replace('\\', '/')))
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
                    var name = path.Substring(beginSegment, endSegment - beginSegment);
                    if (!names.Contains(name))
                    {
                        names.Add(name);
                        yield return new DirectoryInfoStub(
                            fullName: path.Substring(0, endSegment + 1),
                            name: name,
                            paths: Paths);
                    }
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
