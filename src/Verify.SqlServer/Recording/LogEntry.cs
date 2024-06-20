namespace VerifyTests.SqlServer;

public class LogEntry(DbCommand command, DbException? exception = null)
{
    public bool HasTransaction { get; } = command.Transaction != null;
    public DbException? Exception { get; } = exception;
    public DbParameterCollection Parameters { get; } = command.Parameters;
    public string Text { get; } = command.CommandText.Trim();
}