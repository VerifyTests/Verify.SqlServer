using VerifyTests.SqlServer;

namespace VerifyTests;

public static class SqlRecording
{
    static Listener listener;

    static SqlRecording()
    {
        listener = new();
        // ReSharper disable once UnusedVariable
        var subscription = DiagnosticListener.AllListeners.Subscribe(listener);
    }

    public static void StartRecording() =>
        listener.Start();

    public static IReadOnlyList<LogEntry> FinishRecording() =>
        listener.Finish();

    public static bool TryFinishRecording(out IReadOnlyList<LogEntry>? entries) =>
        listener.TryFinish(out entries);
}