using System.Threading.Tasks;
using LocalDb;
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
            async connection =>
            {
                await using var command = connection.CreateCommand();
                command.CommandText = "create table MyTable (Value int);";
                await command.ExecuteNonQueryAsync();
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