namespace VerifyTests;

[Obsolete("Use VerifyTests.Recording")]
public static class SqlRecording
{
    static Listener listener = new();

    [ModuleInitializer]
    public static void Initialize()
    {
        // ReSharper disable once UnusedVariable
        var subscription = DiagnosticListener.AllListeners.Subscribe(listener);
    }
}