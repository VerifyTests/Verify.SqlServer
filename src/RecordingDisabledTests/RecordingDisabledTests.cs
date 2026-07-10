[TestFixture]
public class RecordingDisabledTests
{
    static SqlInstance sqlInstance = new(
        "VerifySqlServerRecordingDisabled",
        _ => Task.CompletedTask);

    [Test]
    public async Task ExecutedCommandIsNotRecorded()
    {
        await using var database = await sqlInstance.Build();
        await using var connection = new SqlConnection(database.ConnectionString);
        await connection.OpenAsync();
        Recording.Start();
        await using var command = connection.CreateCommand();
        command.CommandText = "select 1";
        await command.ExecuteScalarAsync();
        var entries = Recording.Stop();
        Assert.That(entries, Is.Empty);
    }

    // recordCommands only disables the diagnostic listener, not the converters
    [Test]
    public Task ConvertersAreStillRegistered() =>
        Verify(
            new SqlCommand
            {
                CommandText = "select * from MyTable"
            });
}
