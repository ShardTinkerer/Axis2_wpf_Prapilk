using System;
using System.IO;
using Axis2.WPF.Models; // Import the models

namespace Axis2.WPF.Services
{
    public static class Logger
    {
        private static readonly string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "axis2_wpf_debug.log");
        private static readonly object _lock = new object();
        public static event Action<LogEntry> OnLogMessage; // Changed to LogEntry

        public static void Init()
        {
            try
            {
                File.WriteAllText(logFilePath, string.Empty); // Clear log on start
            }
            catch { /* Ignore */ }
        }

        public static void Log(string message)
        {
            Log(LogSource.Unknown, message);
        }

        // Changed to accept a LogSource
        public static void Log(LogSource source, string message)
        {
            try
            {
                var logEntry = new LogEntry(source, message);
                lock (_lock)
                {
                    File.AppendAllText(logFilePath, logEntry.FormattedMessage + "\n");
                }
                OnLogMessage?.Invoke(logEntry);
            }
            catch { /* Ignore */ }
        }
    }
}