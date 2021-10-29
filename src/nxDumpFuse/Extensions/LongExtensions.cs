namespace nxDumpFuse.Extensions
{
    public static class LongExtensions
    {
        public static long ToMb(this long bytes)
        {
            return bytes / (1024 * 1024);
        }

        public static long ToSeconds(this long milliseconds)
        {
            return milliseconds / 1000;
        }

    }
}
