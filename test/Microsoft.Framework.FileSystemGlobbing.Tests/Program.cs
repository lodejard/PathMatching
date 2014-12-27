using Microsoft.Framework.Runtime;
using System;

namespace Microsoft.Framework.FileSystemGlobbing.Tests
{
    public class Program
    {
        Xunit.KRunner.Program _program;

        public Program(
            IServiceProvider services,
            IApplicationEnvironment environment,
            IFileMonitor fileMonitor)
        {
            _program = new Xunit.KRunner.Program(services, environment, fileMonitor);
        }

        public int Main(string[] args)
        {
            _program.Main(args);
            return 0;
        }
    }
}
