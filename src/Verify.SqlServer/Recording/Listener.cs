#pragma warning disable CA1822
// https://github.com/dotnet/SqlClient/blob/master/src/Microsoft.Data.SqlClient/netcore/src/Microsoft/Data/SqlClient/SqlClientDiagnosticListenerExtensions.cs
class Listener :
    IObserver<DiagnosticListener>,
    IDisposable
{
    ConcurrentQueue<IDisposable> subscriptions = [];

    public void OnNext(DiagnosticListener value)
    {
        if (value.Name != "SqlClientDiagnosticListener")
        {
            return;
        }

        subscriptions.Enqueue(
            value.SubscribeWithAdapter(
                this,
                _ => Recording.IsRecording()));
    }

    [DiagnosticName("System.Data.SqlClient.WriteCommandAfter")]
    public void OnSystemCommandAfter(SysCommand command) =>
        Recording.TryAdd("sql", new LogEntry(command));

    [DiagnosticName("Microsoft.Data.SqlClient.WriteCommandAfter")]
    public void OnMsCommandAfter(MsCommand command) =>
        Recording.TryAdd("sql", new LogEntry(command));

    [DiagnosticName("System.Data.SqlClient.WriteCommandError")]
    public void OnSysErrorExecuteCommand(SysCommand command, SysException exception) =>
        Recording.TryAdd("sql", new LogEntry(command, exception));

    [DiagnosticName("Microsoft.Data.SqlClient.WriteCommandError")]
    public void OnMsErrorExecuteCommand(MsCommand command, MsException exception) =>
        Recording.TryAdd("sql", new LogEntry(command, exception));

    void Clear()
    {
        foreach (var subscription in subscriptions)
        {
            subscription.Dispose();
        }
    }

    public void Dispose() =>
        Clear();

    public void OnCompleted() =>
        Clear();

    public void OnError(Exception error)
    {
    }
}