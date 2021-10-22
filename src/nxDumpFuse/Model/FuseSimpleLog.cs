using System;
using Avalonia.Media;
using nxDumpFuse.Model.Enums;

namespace nxDumpFuse.Model
{
    public class FuseSimpleLog
    {
        public FuseSimpleLog(FuseSimpleLogType type, DateTime time, string message)
        {
            Type = type;
            Time = time;
            Message = message;
            // This is not working, seems to be an issue https://github.com/AvaloniaUI/Avalonia/issues/2482
            // needs to be reviewed. Setting Foreground property directly works fine, however binding fails
            Color = Type == FuseSimpleLogType.Information ? Brushes.White : Brushes.Red;
        }
        
        public FuseSimpleLogType Type {  get; }

        public DateTime Time { get; }

        public string Message { get; }

        public IBrush Color { get; }
    }
}
