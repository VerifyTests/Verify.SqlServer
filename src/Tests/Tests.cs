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

        sqlInstance = new(
            "VerifySqlServer",
            connection =>
            {
                Server server = new(new ServerConnection((SqlConnection) connection));
                server.ConnectionContext.ExecuteNonQuery(@"
create table
MyTable(Value int);
go

insert into MyTable (Value)
values (42);
go

create trigger MyTrigger
on MyTable
after update
as raiserror ('Notify Customer Relations', 16, 10);
go

create view MyView
as
  select Value
  from MyTable
  where (Value > 10);
go

create synonym synonym1
    for MyTable;
go

create procedure MyProcedure
as
begin
  set nocount on;
  select Value
  from MyTable
  where (Value > 10);
end;
go

create function MyFunction(
  @quantity int,
  @list_price dec(10,2),
  @discount dec(4,2)
)
returns dec(10,2)
as
begin
    return @quantity * @list_price * (1 - @discount);
end;");
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
        await using System.Data.SqlClient.SqlConnection connection = new(connectionString);
        await connection.OpenAsync();
        await Verifier.Verify(connection);
    }

    [Test]
    public async Task RecordingError()
    {
        await using var database = await sqlInstance.Build();
        await using SqlConnection connection = new(database.ConnectionString);
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
        await Verifier.Verify(commands)
            .ScrubLinesContaining("HelpLink.ProdVer");
    }

    [Test]
    public async Task Recording()
    {
        await using var database = await sqlInstance.Build();
        var connectionString = database.ConnectionString;

        #region Recording

        await using SqlConnection connection = new(connectionString);
        await connection.OpenAsync();
        SqlRecording.StartRecording();
        await using var command = connection.CreateCommand();
        command.CommandText = "select Value from MyTable";
        var value = await command.ExecuteScalarAsync();
        await Verifier.Verify(value);

        #endregion
    }

    [Test]
    public async Task RecordingSpecific()
    {
        await using var database = await sqlInstance.Build();
        var connectionString = database.ConnectionString;

        #region RecordingSpecific

        await using SqlConnection connection = new(connectionString);
        await connection.OpenAsync();
        SqlRecording.StartRecording();
        await using var command = connection.CreateCommand();
        command.CommandText = "select Value from MyTable";
        var value = await command.ExecuteScalarAsync();
        var entries = SqlRecording.FinishRecording();
        //TODO: optionally filter the results
        await Verifier.Verify(new
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
        await using SqlConnection connection = new(database.ConnectionString);
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

        await Verifier.Verify(connection)
            .SchemaSettings(
                storedProcedures: true,
                tables: true,
                views: true,
                includeItem: itemName => itemName == "MyTable");

        #endregion
    }
}