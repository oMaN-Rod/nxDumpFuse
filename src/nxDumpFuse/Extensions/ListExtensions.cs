using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace nxDumpFuse.Extensions
{
    public static class ListExtensions
    {
        public static long GetOutputFileSize(this List<string> files)
        {
            long totalFileSize = 0;
            files.Select(f => f).ToList().ForEach(f => totalFileSize += new FileInfo(f).Length);
            return totalFileSize;
        }
    }
}
