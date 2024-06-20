namespace VerifyTests.SqlServer;

public class ErrorEntry(DbCommand command, DbException exception)
{
    public DbException Exception { get; } = exception;
    public DbCommand Command { get; } = command;
}