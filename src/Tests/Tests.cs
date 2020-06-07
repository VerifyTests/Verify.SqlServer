using System.Threading.Tasks;
using LocalDb;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Verify;
using VerifyNUnit;
using NUnit.Framework;

[TestFixture]
public class Tests
{
    static SqlInstance sqlInstance;

    static Tests()
    {
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

    #region SqlServerSchema

    [Test]
    public async Task SqlServerSchema()
    {
        await using var database = await sqlInstance.Build();
        await Verifier.Verify(database.Connection);
    }

    #endregion

    [Test]
    public async Task SqlServerSchemaLegacy()
    {
        var database = await sqlInstance.Build();
        var connectionString = database.ConnectionString;
        await using var connection = new System.Data.SqlClient.SqlConnection(connectionString);
        await connection.OpenAsync();
        await Verifier.Verify(connection);
    }

    #region SqlServerSchemaSettings

    [Test]
    public async Task SqlServerSchemaSettings()
    {
        await using var database = await sqlInstance.Build();
        var settings = new VerifySettings();
        settings.SchemaSettings(
            storedProcedures: true,
            tables: true,
            views: true,
            includeItem: itemName => itemName == "MyTable");
        await Verifier.Verify(database.Connection, settings);
    }

    #endregion
}