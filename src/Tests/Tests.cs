using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

[TestFixture]
public class Tests
{
    static SqlInstance sqlInstance;

    static Tests()
    {
        #region Enable

        VerifySqlServer.Enable();

        #endregion

        sqlInstance = new(
            "VerifySqlServer",
            connection =>
            {
                var server = new Server(new ServerConnection(connection));
                server.ConnectionContext.ExecuteNonQuery(@"
CREATE TABLE
MyTable(Value int);
GO

CREATE INDEX MyIndex
ON MyTable (Value);
GO

INSERT INTO MyTable (Value)
VALUES (42);
GO

CREATE TRIGGER MyTrigger
ON MyTable
AFTER UPDATE
AS RAISERROR ('Notify Customer Relations', 16, 10);
GO


CREATE VIEW MyView
AS
  SELECT Value
  FROM MyTable
  WHERE (Value > 10);
GO

create synonym synonym1
    for MyTable;
GO

CREATE PROCEDURE MyProcedure
AS
BEGIN
  SET NOCOUNT ON;
  SELECT Value
  FROM MyTable
  WHERE (Value > 10);
END;
GO

CREATE FUNCTION MyFunction(
  @quantity INT,
  @list_price DEC(10,2),
  @discount DEC(4,2)
)
RETURNS DEC(10,2)
AS
BEGIN
    RETURN @quantity * @list_price * (1 - @discount);
END;");
                return Task.CompletedTask;
            });
    }

    [Test]
    public async Task SqlServerSchema()
    {
        await using var database = await sqlInstance.Build();
        var connection = database.Connection;

        #region SqlServerSchema

        await Verify(connection);

        #endregion
    }

    [Test]
    public async Task SqlServerSchemaInDynamic()
    {
        await using var database = await sqlInstance.Build();
        var connection = database.Connection;
        await Verify(new {connection});
    }

    [Test]
    public async Task SqlServerSchemaLegacy()
    {
        var database = await sqlInstance.Build();
        var connectionString = database.ConnectionString;
        await using var connection = new System.Data.SqlClient.SqlConnection(connectionString);
        await connection.OpenAsync();
        await Verify(connection);
    }

    [Test]
    public async Task RecordingError()
    {
        await using var database = await sqlInstance.Build();
        await using var connection = new SqlConnection(database.ConnectionString);
        await connection.OpenAsync();
        SqlRecording.StartRecording();
        await using var command = connection.CreateCommand();
        command.CommandText = "select * from MyTabl2e";
        try
        {
            await using var dataReader = await command.ExecuteReaderAsync();
        }
        catch
        {
        }

        var commands = SqlRecording.FinishRecording();
        await Verify(commands)
            .ScrubLinesContaining("HelpLink.ProdVer");
    }

    [Test]
    public async Task SqlException()
    {
        await using var database = await sqlInstance.Build();
        await using var connection = new SqlConnection(database.ConnectionString);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "select * from MyTabl2e";
        await ThrowsTask(() => command.ExecuteReaderAsync());
    }

    [Test]
    public async Task SqlError()
    {
        await using var database = await sqlInstance.Build();
        await using var connection = new SqlConnection(database.ConnectionString);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "select * from MyTabl2e";
        try
        {
            await command.ExecuteReaderAsync();
        }
        catch (SqlException exception)
        {
            await Verify(exception.Errors[0]);
        }
    }

    [Test]
    public async Task Recording()
    {
        await using var database = await sqlInstance.Build();
        var connectionString = database.ConnectionString;

        #region Recording

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        SqlRecording.StartRecording();
        await using var command = connection.CreateCommand();
        command.CommandText = "select Value from MyTable";
        var value = await command.ExecuteScalarAsync();
        await Verify(value);

        #endregion
    }

    [Test]
    public async Task RecordingSpecific()
    {
        await using var database = await sqlInstance.Build();
        var connectionString = database.ConnectionString;

        #region RecordingSpecific

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        SqlRecording.StartRecording();
        await using var command = connection.CreateCommand();
        command.CommandText = "select Value from MyTable";
        var value = await command.ExecuteScalarAsync();
        var entries = SqlRecording.FinishRecording();
        //TODO: optionally filter the results
        await Verify(new
        {
            value,
            sql = entries
        });

        #endregion
    }

    [Test]
    public async Task RecordingTest()
    {
        static async Task Execute(SqlConnection sqlConnection)
        {
            await using var command = sqlConnection.CreateCommand();
            command.CommandText = "select * from MyTable";
            await using var dataReader = await command.ExecuteReaderAsync();
        }

        await using var database = await sqlInstance.Build();
        await using var connection = new SqlConnection(database.ConnectionString);
        await connection.OpenAsync();
        await Execute(connection);
        SqlRecording.StartRecording();
        await Execute(connection);
        await Execute(connection);
        var commands = SqlRecording.FinishRecording();
        await Execute(connection);
        await Verify(commands);
    }

    [Test]
    public async Task SqlServerSchemaSettings()
    {
        await using var database = await sqlInstance.Build();
        var connection = database.Connection;

        #region SqlServerSchemaSettings

        await Verify(connection)
            .SchemaSettings(
                storedProcedures: true,
                tables: true,
                views: true,
                includeItem: itemName => itemName == "MyTable");

        #endregion
    }
}