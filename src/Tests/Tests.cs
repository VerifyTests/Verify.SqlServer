using System.Threading.Tasks;
using LocalDb;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using VerifyNUnit;
using NUnit.Framework;
using VerifyTests;

[TestFixture]
public class Tests
{
    static SqlInstance sqlInstance;

    static Tests()
    {
        #region Enable
        VerifySqlServer.Enable();
        #endregion
        sqlInstance = new SqlInstance(
            "VerifySqlServer",
            connection =>
            {
                var server = new Server(new ServerConnection((SqlConnection) connection));
                server.ConnectionContext.ExecuteNonQuery(@"
create table
MyTable(Value int);
go

CREATE VIEW MyView
AS
  SELECT Value
  FROM MyTable
  WHERE (Value > 10);
go

CREATE PROCEDURE MyProcedure
AS
BEGIN
  SET NOCOUNT ON;
  SELECT Value
  FROM MyTable
  WHERE (Value > 10);
END;
go

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
        await Verifier.Verify(connection);
        #endregion
    }

    [Test]
    public async Task SqlServerSchemaInDynamic()
    {
        await using var database = await sqlInstance.Build();
        var connection = database.Connection;
        await Verifier.Verify(new {connection});
    }

    [Test]
    public async Task SqlServerSchemaLegacy()
    {
        var database = await sqlInstance.Build();
        var connectionString = database.ConnectionString;
        await using var connection = new System.Data.SqlClient.SqlConnection(connectionString);
        await connection.OpenAsync();
        await Verifier.Verify(connection);
    }

    [Test]
    public async Task RecordingError()
    {
        await using var database = await sqlInstance.Build();
        var connection = new SqlConnection(database.ConnectionString);
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
        await Verifier.Verify(commands);
    }

    [Test]
    public async Task Recording()
    {
        await using var database = await sqlInstance.Build();
        var connectionString = database.ConnectionString;

        #region Recording

        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        SqlRecording.StartRecording();
        await using var command = connection.CreateCommand();
        command.CommandText = "select * from MyTable";
        await using var dataReader = await command.ExecuteReaderAsync();
        var commands = SqlRecording.FinishRecording();
        await Verifier.Verify(commands);

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
        var connection = new SqlConnection(database.ConnectionString);
        await connection.OpenAsync();
        await Execute(connection);
        SqlRecording.StartRecording();
        await Execute(connection);
        await Execute(connection);
        var commands = SqlRecording.FinishRecording();
        await Execute(connection);
        await Verifier.Verify(commands);
    }

    [Test]
    public async Task SqlServerSchemaSettings()
    {
        await using var database = await sqlInstance.Build();
        var connection = database.Connection;

        #region SqlServerSchemaSettings

        var settings = new VerifySettings();
        settings.SchemaSettings(
            storedProcedures: true,
            tables: true,
            views: true,
            includeItem: itemName => itemName == "MyTable");
        await Verifier.Verify(connection, settings);

        #endregion
    }
}