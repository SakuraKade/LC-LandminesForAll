#if DEBUG
using BepInEx.Logging;
using System;
using System.IO;

namespace LC_LandminesForAll.Debugging
{
    internal class DebugLoggerRedirect : IDisposable
    {
        private StreamWriter _writer;
        private ManualLogSource _logSource;
        private readonly object _lock = new object();
        private bool _disposed = false;

        public DebugLoggerRedirect(ManualLogSource logSource)
        {
            _logSource = logSource;
            _writer = new StreamWriter(Path.Combine(Plugin.ThisPluginFolder, "log.txt"));
            _writer.AutoFlush = true;
            _logSource.LogEvent += LogSource_LogEvent;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _logSource.LogEvent -= LogSource_LogEvent;
            _writer.Flush();
            _writer.Close();
            _writer.Dispose();
            _disposed = true;
        }

        private void LogSource_LogEvent(object sender, LogEventArgs e)
        {
            _writer.WriteLine($"[{e.Level}] {e.Data}");
            _writer.Flush(); // Flush after every write to ensure we don't lose any data
        }
    }
}
#endif