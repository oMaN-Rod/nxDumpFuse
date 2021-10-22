using nxDumpFuse.Model;

namespace nxDumpFuse.Events
{
    public class EventHandlers
    {
        public delegate void FuseUpdateEventHandler(FuseUpdateInfo fuseUpdateInfo);

        public delegate void FuseSimpleLogEventHandler(FuseSimpleLog log);
    }
}
