namespace nxDumpFuse.Model
{
    public class FuseUpdateInfo
    {
        public double Progress { get; set; }
        
        public double ProgressPart { get; set; }

        public long Speed { get; set; }

        public int Part { get; set; }

        public int Parts { get; set; }

        public bool Complete { get; set; }
    }
}
