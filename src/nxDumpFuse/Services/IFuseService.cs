using System.Collections.Generic;
using nxDumpFuse.Events;

namespace nxDumpFuse.Services
{
    public interface IFuseService
    {
        public event EventHandlers.FuseUpdateEventHandler? FuseUpdateEvent;
        public event EventHandlers.FuseSimpleLogEventHandler? FuseSimpleLogEvent;

        void Start(string inputFilePath, string outputDir);

        void Stop();

        void FuseFiles(List<string> inputFiles);
    }
}
