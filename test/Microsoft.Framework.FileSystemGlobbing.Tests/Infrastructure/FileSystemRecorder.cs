using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Framework.FileSystemGlobbing.Tests.Infrastructure
{
    public class SystemIoRecorder
    {
        public IList<IDictionary<string, object>> Records = new List<IDictionary<string, object>>();

        public void Add(string action, object values)
        {
            var record = new Dictionary<string, object>
            {
                {"action", action }
            };

            foreach(var p in values.GetType().GetTypeInfo().DeclaredProperties)
            {
                record[p.Name] = p.GetValue(values);
            }
            Records.Add(record);
        }
    }
}
