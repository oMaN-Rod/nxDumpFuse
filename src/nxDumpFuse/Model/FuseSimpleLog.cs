using System;
using System.Diagnostics;
using Avalonia.Media;

namespace nxDumpFuse.Model
{
    public enum FuseSimpleLogType
    {
        Error,
        Information
    }

    public class FuseSimpleLog
    {
        public FuseSimpleLog(FuseSimpleLogType type, DateTime time, string message)
        {
            Type = type;
            Time = time;
            Message = message;
            Color = Type == FuseSimpleLogType.Information ? Brushes.White : Brushes.Red;
            Debug.WriteLine($"Color is {Color}");
        }
        
        public FuseSimpleLogType Type {  get; }

        public DateTime Time { get; }

        public string Message { get; }

        public IBrush Color { get; }
    }
}
