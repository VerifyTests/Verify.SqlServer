using System.Collections.Generic;
using System.Diagnostics;

namespace VerifyTests
{
    public static class SqlRecording
    {
        static Listener listener;

        static SqlRecording()
        {
            listener = new Listener();
            var subscription = DiagnosticListener.AllListeners.Subscribe(listener);
        }

        public static void StartRecording()
        {
            listener.Start();
        }

        public static IEnumerable<LogEntry> FinishRecording()
        {
            return listener.Finish();
        }
    }
}