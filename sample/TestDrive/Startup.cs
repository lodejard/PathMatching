using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Framework.FileSystemGlobbing.Infrastructure;
using Microsoft.Framework.FileSystemGlobbing.Tests.Infrastructure;
using System.Linq;

namespace TestDrive
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseFileServer();

            app.Map("/api", map => map.Run(async context =>
            {
                var form = await context.Request.GetFormAsync();

                var matcher = new Matcher();
                foreach (var include in form["include"].Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    matcher.AddInclude(include);
                }
                foreach (var exclude in form["exclude"].Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    matcher.AddExclude(exclude);
                }
                var recorder = new SystemIoRecorder();
                var directoryInfo = new DirectoryInfoStub(
                    recorder,
                    null,
                    @"c:\test\",
                    ".",
                    form["files"]
                        .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => @"c:\test\" + x)
                        .ToArray());

                var result = matcher.Execute(directoryInfo);

                var recorderData = recorder.Records
                    .Select(record => record.Aggregate("", (acc, kv) => acc + kv.Key + ":" + kv.Value + " "))
                    .Aggregate("", (acc, line) => acc + line + ",");

                context.Response.Headers["recorder"] = recorderData;

                await context.Response.WriteAsync(string.Join("\r\n", result.Files));
            }));
        }
    }
}
