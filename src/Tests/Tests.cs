using System.Threading.Tasks;
using LocalDb;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Verify;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class Tests :
    VerifyBase
{
    static SqlInstance sqlInstance;

    static Tests()
    {
        sqlInstance = new SqlInstance(
            "VerifySqlServer",
            connection =>
            {
                var server = new Server(new ServerConnection(connection));
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

");
                return Task.CompletedTask;
            });
    }

    #region SqlServerSchema
    [Fact]
    public async Task SqlServerSchema()
    {
        var database = await sqlInstance.Build("SqlServerSchema");
        await Verify(database.Connection);
    }
    #endregion

    #region SqlServerSchemaSettings
    [Fact]
    public async Task SqlServerSchemaSettings()
    {
        var database = await sqlInstance.Build("SqlServerSchemaSettings");
        var settings = new VerifySettings();
        settings.SchemaSettings(
            storedProcedures: true,
            tables: true,
            views: true,
            includeItem: itemName => itemName == "MyTable");
        await Verify(database.Connection, settings);
    }
    #endregion

    public Tests(ITestOutputHelper output) :
        base(output)
    {
    }
}