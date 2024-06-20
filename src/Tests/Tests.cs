using System.Data;
using System.Data.SqlTypes;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using VerifyTests.SqlServer;

[TestFixture]
public class Tests
{
    static SqlInstance sqlInstance;

    static Tests() =>
        sqlInstance = new(
            "VerifySqlServer",
            connection =>
            {
                var server = new Server(new ServerConnection(connection));
                server.ConnectionContext.ExecuteNonQuery(
                    """
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
                    END;
                    """);
                return Task.CompletedTask;
            });

    [Test]
    public async Task Schema()
    {
        await using var database = await sqlInstance.Build();
        var connection = database.Connection;

        #region SqlServerSchema

        await Verify(connection);

        #endregion
    } [Test]
    public async Task SchemaAsSql()
    {
        await using var database = await sqlInstance.Build();
        var connection = database.Connection;

        #region SqlServerSchemaAsSql

        await Verify(connection)
            .SchemaAsSql();

        #endregion
    }

    [Test]
    public async Task SchemaInDynamic()
    {
        await using var database = await sqlInstance.Build();
        var connection = database.Connection;
        await Verify(new
        {
            connection
        });
    }

    [Test]
    public async Task SchemaLegacy()
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
        Recording.Start();
        await using var command = connection.CreateCommand();
        command.CommandText = "select * from MyTabl2e";
        try
        {
            await using var dataReader = await command.ExecuteReaderAsync();
        }
        catch
        {
        }

        await Verify()
            .ScrubLinesContaining("HelpLink.ProdVer");
    }

    [Test]
    public async Task Exception()
    {
        await using var database = await sqlInstance.Build();
        await using var connection = new SqlConnection(database.ConnectionString);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "select * from MyTabl2e";
        await ThrowsTask(() => command.ExecuteReaderAsync());
    }

    [Test]
    public async Task Error()
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
    public async Task RecordingUsage()
    {
        await using var database = await sqlInstance.Build();
        var connectionString = database.ConnectionString;

        #region Recording

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        Recording.Start();
        await using var command = connection.CreateCommand();
        command.CommandText = "select Value from MyTable";
        var value = await command.ExecuteScalarAsync();
        await Verify(value!);

        #endregion
    }

    [Test]
    public async Task RecordingWithParameter()
    {
        await using var database = await sqlInstance.Build();
        var connectionString = database.ConnectionString;

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        Recording.Start();
        await using var command = connection.CreateCommand();
        command.Parameters.AddWithValue("param", 10);
        command.CommandText = "select Value from MyTable where Value = @param";
        var value = await command.ExecuteScalarAsync();
        await Verify(value!);
    }

    [Test]
    public Task MsParameterEmpty()
    {
        var parameter = new SqlParameter("name", SqlDbType.Date);
        return Verify(parameter);
    }

    [Test]
    public Task MsParameterFull()
    {
        var parameter = BuildFullMsParameter();

        return Verify(parameter);
    }
    [Test]
    public Task MsParameterCollectionFull()
    {
        var parameter = BuildFullSysParameter();

        var parameters = new System.Data.SqlClient.SqlCommand().Parameters;
        parameters.Add(parameter);
        return Verify(parameters);
    }

    static SqlParameter BuildFullMsParameter() =>
        new("name", SqlDbType.DateTime)
        {
            Direction = ParameterDirection.InputOutput,
            Offset = 5,
            Precision = 2,
            Scale = 3,
            Value = DateTime.Now,
            CompareInfo = SqlCompareOptions.BinarySort2,
            LocaleId = 10,
            Size = 4,
            IsNullable = false,
            SourceVersion = DataRowVersion.Proposed,
            ForceColumnEncryption = true,
            SourceColumn = "sourceColumn"
        };

    [Test]
    public Task SysParameterFull()
    {
        var parameter = BuildFullSysParameter();

        return Verify(parameter);
    }

    [Test]
    public Task SysParameterCollectionFull()
    {
        var parameter = BuildFullSysParameter();

        var parameters = new System.Data.SqlClient.SqlCommand().Parameters;
        parameters.Add(parameter);
        return Verify(parameters);
    }

    static System.Data.SqlClient.SqlParameter BuildFullSysParameter() =>
        new("name", SqlDbType.Date)
        {
            Direction = ParameterDirection.InputOutput,
            Offset = 5,
            Precision = 2,
            Scale = 3,
            Value = DateTime.Now,
            CompareInfo = SqlCompareOptions.BinarySort2,
            LocaleId = 10,
            Size = 4,
            IsNullable = false,
            SourceVersion = DataRowVersion.Proposed,
            SourceColumn = "sourceColumn"
        };

    [Test]
    public Task SysParameterEmpty()
    {
        var parameter = new System.Data.SqlClient.SqlParameter("name", SqlDbType.Date);
        return Verify(parameter);
    }

    [Test]
    public async Task RecordingSpecific()
    {
        await using var database = await sqlInstance.Build();
        var connectionString = database.ConnectionString;

        #region RecordingSpecific

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        Recording.Start();
        await using var command = connection.CreateCommand();
        command.CommandText = "select Value from MyTable";
        var value = await command.ExecuteScalarAsync();
        var entries = Recording
            .Stop()
            .Select(_ => _.Data);
        //TODO: optionally filter the results
        await Verify(
            new
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
        Recording.Start();
        await Execute(connection);
        await Execute(connection);
        await Execute(connection);
        await Verify();
    }

    [Test]
    public async Task SchemaInclude()
    {
        await using var database = await sqlInstance.Build();
        var connection = database.Connection;

        #region SchemaInclude

        await Verify(connection)
            // include only tables and views
            .SchemaIncludes(DbObjects.Tables | DbObjects.Views);

        #endregion
    }

    [Test]
    public async Task SchemaIncludeAll()
    {
        await using var database = await sqlInstance.Build();
        var connection = database.Connection;

        await Verify(connection)
            .SchemaIncludes(DbObjects.All);
    }

    [Test]
    public async Task SchemaFilter()
    {
        await using var database = await sqlInstance.Build();
        var connection = database.Connection;

        #region SchemaFilter

        await Verify(connection)
            // include tables & views, or named MyTrigger
            .SchemaFilter(
                _ => _ is TableViewBase ||
                     _.Name == "MyTrigger");

        #endregion
    }
}