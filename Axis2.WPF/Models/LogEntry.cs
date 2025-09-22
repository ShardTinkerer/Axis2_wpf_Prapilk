using System;

namespace Axis2.WPF.Models
{
    public class LogEntry
    {
        public DateTime Timestamp { get; }
        public LogSource Source { get; }
        public string Message { get; }
        public string FormattedMessage { get; }

        public LogEntry(LogSource source, string message)
        {
            Timestamp = DateTime.Now;
            Source = source;
            Message = message;
            FormattedMessage = $"[{Timestamp:HH:mm:ss.fff}] [{Source}] {Message}";
        }
    }
}
