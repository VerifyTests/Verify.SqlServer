namespace VerifyTests.SqlServer;

public class LogEntry(DbCommand command, Exception? exception = null)
{
    public bool HasTransaction { get; } = command.Transaction != null;
    public Exception? Exception { get; } = exception;
    public IDictionary<string, object?> Parameters { get; } = command.Parameters.ToDictionary();
    public string Text { get; } = command.CommandText.Trim();
}