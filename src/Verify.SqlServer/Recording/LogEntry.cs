namespace VerifyTests.SqlServer;

public class LogErrorEntry(DbCommand command, DbException exception)
{
    public DbException Exception { get; } = exception;
    public DbCommand Command { get; } = command;
}