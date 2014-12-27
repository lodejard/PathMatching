using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Framework.FileSystemGlobbing.Abstractions
{
    public class MatcherContext
    {
        public MatcherContext(Matcher matcher, DirectoryInfoBase directoryInfo)
        {
            Matcher = matcher;
            DirectoryInfo = directoryInfo;
            foreach (var pattern in matcher.IncludePatterns)
            {
                IncludePatternContexts.Add(new PatternContext(this, pattern));
            }
            foreach (var pattern in matcher.ExcludePatterns)
            {
                ExcludePatternContexts.Add(new PatternContext(this, pattern));
            }
        }

        public Matcher Matcher { get; }
        public DirectoryInfoBase DirectoryInfo { get; }
        public IList<PatternContext> IncludePatternContexts { get; } = new List<PatternContext>();
        public IList<PatternContext> ExcludePatternContexts { get; } = new List<PatternContext>();
        public List<string> Files { get; private set; }

        public FrameData Frame;

        public PatternMatchingResult Execute()
        {
            Files = new List<string>();
            PushFrame(Stage.Predicting, DirectoryInfo);

            DoPredicting();
            DoEnumerating();

            //foreach (var file in Frame.PredictLiteralIncludes)
            //{
            //    Files.Add(file);
            //}
            return new PatternMatchingResult(Files);
        }

        void DoPredicting()
        {
            if (Frame.Stage != Stage.Predicting)
            {
                return;
            }
            //foreach (var patternContext in IncludePatternContexts)
            //{
            //    patternContext.PredictInclude();
            //}
            //foreach (var patternContext in ExcludePatternContexts)
            //{
            //    //patternContext.PredictExclude();
            //}
            Frame.Stage = Stage.Enumerating;
        }

        void DoEnumerating()
        {
            if (Frame.Stage != Stage.Enumerating)
            {
                return;
            }
            foreach (var fileSystemInfo in Frame.DirectoryInfo.EnumerateFileSystemInfos(
                "*",
                SearchOption.TopDirectoryOnly))
            {
                var directoryInfo = fileSystemInfo as DirectoryInfoBase;
                if (directoryInfo != null)
                {
                    if (Frame.ActualDirectories == null)
                    {
                        Frame.ActualDirectories = new List<DirectoryInfoBase>();
                    }
                    Frame.ActualDirectories.Add(directoryInfo);
                    continue;
                }
                var fileInfo = fileSystemInfo as FileInfoBase;
                if (fileInfo != null)
                {
                    Frame.FileInfo = fileInfo;
                    var include = false;
                    foreach (var pattern in IncludePatternContexts)
                    {
                        if (pattern.TestIncludeFile())
                        {
                            include = true;
                            continue;
                        }
                    }
                    if (include)
                    {
                        foreach (var pattern in ExcludePatternContexts)
                        {
                            if (pattern.TestExcludeFile())
                            {
                                include = false;
                                continue;
                            }
                        }
                    }
                    if (include)
                    {
                        Files.Add(fileInfo.Name);
                    }
                    continue;
                }
            }
            Frame.Stage = Stage.Iterating;
        }



        public void AddPredictIncludeLiteral(string value)
        {
            if (Frame.PredictLiteralIncludes == null)
            {
                Frame.PredictLiteralIncludes = new List<string>();
            }
            //TODO: string comparisons
            if (!Frame.PredictLiteralIncludes.Contains(value))
            {
                Frame.PredictLiteralIncludes.Add(value);
            }
        }

        public void PushFrame(Stage stage, DirectoryInfoBase directoryInfo)
        {
            Frame = new FrameData
            {
                Stage = stage,
                DirectoryInfo = directoryInfo,
            };
        }

        public enum Stage
        {
            Initialized,
            Predicting,
            Enumerating,
            Iterating,
        }

        public struct FrameData
        {
            public Stage Stage;
            public List<string> PredictLiteralIncludes;

            public DirectoryInfoBase DirectoryInfo;
            public string RelativePath;

            public FileInfoBase FileInfo;
            public List<DirectoryInfoBase> ActualDirectories;
        }
    }
}
