using VerifyTests.SqlServer;

namespace VerifyTests;

public static class SqlRecording
{
    static Listener listener;

    static SqlRecording()
    {
        listener = new();
        var subscription = DiagnosticListener.AllListeners.Subscribe(listener);
    }

    public static void StartRecording() =>
        listener.Start();

    public static IEnumerable<LogEntry> FinishRecording() =>
        listener.Finish();

    public static bool TryFinishRecording(out IEnumerable<LogEntry>? entries) =>
        listener.TryFinish(out entries);
}