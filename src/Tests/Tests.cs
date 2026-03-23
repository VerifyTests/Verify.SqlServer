[TestFixture]
public class Tests
{
    static SqlInstance sqlInstance;

    static Tests() =>
        sqlInstance = new(
            "VerifySqlServer",
            connection =>
            {
                var serverConnection = new ServerConnection
                {
                    ConnectionString = connection.ConnectionString,
                };
                var server = new Server(serverConnection);
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

                    CREATE TABLE
                    MyOtherTable(Value int);
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
                    GO

                    CREATE TABLE ColumnTypes(
                      Id int NOT NULL IDENTITY(1,1),
                      Name nvarchar(100) NOT NULL,
                      Description nvarchar(max) NULL,
                      Code varchar(20) NULL,
                      Amount decimal(18,4) NOT NULL,
                      IsActive bit NOT NULL,
                      Created datetime2 NOT NULL,
                      RowVersion rowversion NOT NULL,
                      Price money NULL,
                      Ratio float NULL,
                      SmallNum smallint NULL,
                      TinyNum tinyint NULL,
                      BigNum bigint NULL,
                      UniqueCode uniqueidentifier NULL,
                      Notes text NULL,
                      BinaryData varbinary(500) NULL,
                      MaxBinary varbinary(max) NULL,
                      FixedChar char(10) NULL,
                      FixedBinary binary(16) NULL,
                      TimeOnly time NULL,
                      DateOnly date NULL,
                      NText ntext NULL,
                      Xml xml NULL,
                      CONSTRAINT PK_ColumnTypes PRIMARY KEY CLUSTERED (Id)
                    );
                    GO

                    CREATE TABLE WithDefaults(
                      Id int NOT NULL IDENTITY(1,1),
                      Status varchar(20) NOT NULL CONSTRAINT DF_WithDefaults_Status DEFAULT ('active'),
                      Created datetime2 NOT NULL CONSTRAINT DF_WithDefaults_Created DEFAULT (GETUTCDATE()),
                      Score int NOT NULL CONSTRAINT DF_WithDefaults_Score DEFAULT (0),
                      CONSTRAINT PK_WithDefaults PRIMARY KEY CLUSTERED (Id)
                    );
                    GO

                    CREATE TABLE WithComputed(
                      Id int NOT NULL IDENTITY(1,1),
                      FirstName nvarchar(50) NOT NULL,
                      LastName nvarchar(50) NOT NULL,
                      FullName AS (FirstName + ' ' + LastName),
                      Quantity int NOT NULL,
                      UnitPrice decimal(10,2) NOT NULL,
                      TotalPrice AS (Quantity * UnitPrice) PERSISTED,
                      CONSTRAINT PK_WithComputed PRIMARY KEY CLUSTERED (Id)
                    );
                    GO

                    CREATE TABLE ParentTable(
                      Id int NOT NULL IDENTITY(1,1),
                      Name nvarchar(100) NOT NULL,
                      CONSTRAINT PK_ParentTable PRIMARY KEY CLUSTERED (Id),
                      CONSTRAINT UQ_ParentTable_Name UNIQUE NONCLUSTERED (Name)
                    );
                    GO

                    CREATE TABLE ChildTable(
                      Id int NOT NULL IDENTITY(1,1),
                      ParentId int NOT NULL,
                      Value int NOT NULL,
                      CONSTRAINT PK_ChildTable PRIMARY KEY CLUSTERED (Id),
                      CONSTRAINT FK_ChildTable_ParentTable FOREIGN KEY (ParentId)
                        REFERENCES ParentTable(Id),
                      CONSTRAINT CK_ChildTable_Value CHECK (Value >= 0)
                    );
                    GO

                    CREATE TABLE CompositeKeyTable(
                      Key1 int NOT NULL,
                      Key2 int NOT NULL,
                      Data nvarchar(200) NULL,
                      CONSTRAINT PK_CompositeKeyTable PRIMARY KEY CLUSTERED (Key1, Key2)
                    );
                    GO

                    CREATE TABLE MultiIndexTable(
                      Id int NOT NULL IDENTITY(1,1),
                      Name nvarchar(100) NOT NULL,
                      Category nvarchar(50) NOT NULL,
                      Status int NOT NULL,
                      CONSTRAINT PK_MultiIndexTable PRIMARY KEY CLUSTERED (Id)
                    );
                    GO

                    CREATE INDEX IX_MultiIndexTable_Name
                    ON MultiIndexTable (Name);
                    GO

                    CREATE UNIQUE INDEX IX_MultiIndexTable_Category_Name
                    ON MultiIndexTable (Category, Name);
                    GO

                    CREATE INDEX IX_MultiIndexTable_Status_Desc
                    ON MultiIndexTable (Status DESC, Name ASC);
                    GO

                    CREATE TABLE MultiTriggerTable(
                      Id int NOT NULL IDENTITY(1,1),
                      Name nvarchar(100) NULL,
                      CONSTRAINT PK_MultiTriggerTable PRIMARY KEY CLUSTERED (Id)
                    );
                    GO

                    CREATE TRIGGER TR_MultiTrigger_Insert
                    ON MultiTriggerTable
                    AFTER INSERT
                    AS RAISERROR ('Insert detected', 16, 10);
                    GO

                    CREATE TRIGGER TR_MultiTrigger_Update
                    ON MultiTriggerTable
                    AFTER UPDATE
                    AS RAISERROR ('Update detected', 16, 10);
                    GO

                    CREATE TRIGGER TR_MultiTrigger_Delete
                    ON MultiTriggerTable
                    AFTER DELETE
                    AS RAISERROR ('Delete detected', 16, 10);
                    GO

                    CREATE VIEW MyViewWithJoin
                    AS
                      SELECT c.Id, c.Value, p.Name AS ParentName
                      FROM ChildTable c
                      INNER JOIN ParentTable p ON c.ParentId = p.Id;
                    GO

                    CREATE VIEW MyViewWithSchemabinding
                    WITH SCHEMABINDING
                    AS
                      SELECT Id, Name
                      FROM dbo.ParentTable;
                    GO

                    CREATE PROCEDURE ProcWithParams
                      @Id int,
                      @Name nvarchar(100),
                      @Count int OUTPUT
                    AS
                    BEGIN
                      SET NOCOUNT ON;
                      SELECT @Count = COUNT(*)
                      FROM ParentTable
                      WHERE Id = @Id AND Name = @Name;
                    END;
                    GO

                    CREATE FUNCTION InlineTableFunction(@MinValue int)
                    RETURNS TABLE
                    AS
                    RETURN
                      SELECT Value
                      FROM MyTable
                      WHERE Value >= @MinValue;
                    GO

                    CREATE FUNCTION MultiStatementTableFunction(@MinValue int)
                    RETURNS @Result TABLE (Value int, Category nvarchar(20))
                    AS
                    BEGIN
                      INSERT INTO @Result
                      SELECT Value,
                        CASE
                          WHEN Value < 10 THEN 'Low'
                          WHEN Value < 100 THEN 'Medium'
                          ELSE 'High'
                        END
                      FROM MyTable
                      WHERE Value >= @MinValue;
                      RETURN;
                    END;
                    GO

                    create synonym synonym2
                        for ParentTable;
                    GO

                    CREATE SCHEMA TestSchema;
                    GO

                    CREATE TABLE TestSchema.SchemaTable(
                      Id int NOT NULL IDENTITY(1,1),
                      Name nvarchar(50) NOT NULL,
                      CONSTRAINT PK_SchemaTable PRIMARY KEY CLUSTERED (Id)
                    );
                    GO

                    CREATE VIEW TestSchema.SchemaView
                    AS
                      SELECT Id, Name
                      FROM TestSchema.SchemaTable;
                    GO

                    CREATE PROCEDURE TestSchema.SchemaProc
                    AS
                    BEGIN
                      SELECT Id, Name
                      FROM TestSchema.SchemaTable;
                    END;
                    GO

                    CREATE FUNCTION TestSchema.SchemaFunction(@Id int)
                    RETURNS nvarchar(50)
                    AS
                    BEGIN
                      DECLARE @Name nvarchar(50);
                      SELECT @Name = Name
                      FROM TestSchema.SchemaTable
                      WHERE Id = @Id;
                      RETURN @Name;
                    END;
                    GO

                    create synonym TestSchema.SchemaSynonym
                        for TestSchema.SchemaTable;
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
    }

    [Test]
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
    public async Task CommandEscaped()
    {
        var command = new SqlCommand
        {
            CommandText = "select * from [MyTable]"
        };
        await Verify(command);
    }

    [Test]
    public async Task CommandEmpty()
    {
        var command = new SqlCommand
        {
            CommandText = "select * from MyTable"
        };
        await Verify(command);
    }

    [Test]
    public async Task CommandFull()
    {
        var command = new SqlCommand
        {
            CommandText = "select * from MyTable",
            CommandTimeout = 10,
            CommandType = CommandType.StoredProcedure,
            DesignTimeVisible = true,
            UpdatedRowSource = UpdateRowSource.FirstReturnedRecord,
            EnableOptimizedParameterBinding = true,
            Notification = new("user data", "options", 10)
        };
        command.Parameters.AddWithValue("name", 10);
        await Verify(command);
    }

    [Test]
    public async Task CommandOrderBySingleColumn()
    {
        var command = new SqlCommand
        {
            CommandText = "select Value from MyTable order by Value"
        };
        await Verify(command);
    }

    [Test]
    public async Task CommandOrderByMultipleColumns()
    {
        var command = new SqlCommand
        {
            CommandText = "select Id, Name from MyTable order by Name, Id"
        };
        await Verify(command);
    }

    [Test]
    public async Task CommandOrderByMultipleColumnsDesc()
    {
        var command = new SqlCommand
        {
            CommandText = "select Id, Name from MyTable order by Name desc, Id asc"
        };
        await Verify(command);
    }

    [Test]
    public async Task CommandInClause()
    {
        var command = new SqlCommand
        {
            CommandText = "select Id, Name from MyTable where Id in (1, 2, 4, 6)"
        };
        await Verify(command);
    }

    [Test]
    public async Task CommandNotInClause()
    {
        var command = new SqlCommand
        {
            CommandText = "select Id, Name from MyTable where Id not in (1, 2, 4)"
        };
        await Verify(command);
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
    public Task ParameterEmpty()
    {
        var parameter = new SqlParameter("name", SqlDbType.Date);
        return Verify(parameter);
    }

    [Test]
    public Task ParameterFull()
    {
        var parameter = BuildFullParameter();

        return Verify(parameter);
    }

    [Test]
    public Task ParameterCollectionFull()
    {
        var parameter = BuildFullParameter();

        var parameters = new SqlCommand().Parameters;
        parameters.Add(parameter);
        return Verify(parameters);
    }

    static SqlParameter BuildFullParameter() =>
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

        await using var errorCommand = connection.CreateCommand();
        errorCommand.CommandText = "select Value from BadTable";
        try
        {
            await errorCommand.ExecuteScalarAsync();
        }
        catch
        {
        }

        var entries = Recording
            .Stop()
            .Select(_ => _.Data);
        //Optionally filter results
        await Verify(
            new
            {
                value,
                sqlEntries = entries
            });

        #endregion
    }

    [Test]
    public async Task RecordingReadingResults()
    {
        await using var database = await sqlInstance.Build();
        var connectionString = database.ConnectionString;

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        Recording.Start();
        await using var command = connection.CreateCommand();
        command.CommandText = "select Value from MyTable";
        await command.ExecuteScalarAsync();

        await using var errorCommand = connection.CreateCommand();
        errorCommand.CommandText = "select Value from BadTable";
        try
        {
            await errorCommand.ExecuteScalarAsync();
        }
        catch
        {
        }

        #region RecordingReadingResults

        var entries = Recording.Stop();

        // all sql entries via key
        var sqlEntries = entries
            .Where(_ => _.Name == "sql")
            .Select(_ => _.Data);

        // successful Commands via Type
        var sqlCommandsViaType = entries
            .Select(_ => _.Data)
            .OfType<SqlCommand>();

        // failed Commands via Type
        var sqlErrorsViaType = entries
            .Select(_ => _.Data)
            .OfType<ErrorEntry>();

        #endregion

        await Verify(
            new
            {
                sqlEntries,
                sqlCommandsViaType,
                sqlErrorsViaType,
            });

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

    [Test]
    public async Task SchemaFilterByName()
    {
        await using var database = await sqlInstance.Build();
        var connection = database.Connection;

        await Verify(connection)
            .SchemaFilter(_ => _.Name is "MyTable" or "MyView" or "MyProcedure");
    }

    [Test]
    public async Task SchemaFilterExcludesAll()
    {
        await using var database = await sqlInstance.Build();
        var connection = database.Connection;

        await Verify(connection)
            .SchemaFilter(_ => false);
    }

    [Test]
    public async Task SchemaFilterExcludesAllSql()
    {
        await using var database = await sqlInstance.Build();
        var connection = database.Connection;

        await Verify(connection)
            .SchemaFilter(_ => false)
            .SchemaAsSql();
    }

    [Test]
    public async Task SchemaIncludeTablesOnly()
    {
        await using var database = await sqlInstance.Build();
        var connection = database.Connection;

        await Verify(connection)
            .SchemaIncludes(DbObjects.Tables);
    }

    [Test]
    public async Task SchemaIncludeViewsOnly()
    {
        await using var database = await sqlInstance.Build();
        var connection = database.Connection;

        await Verify(connection)
            .SchemaIncludes(DbObjects.Views);
    }

    [Test]
    public async Task SchemaIncludeStoredProceduresOnly()
    {
        await using var database = await sqlInstance.Build();
        var connection = database.Connection;

        await Verify(connection)
            .SchemaIncludes(DbObjects.StoredProcedures);
    }

    [Test]
    public async Task SchemaIncludeUserDefinedFunctionsOnly()
    {
        await using var database = await sqlInstance.Build();
        var connection = database.Connection;

        await Verify(connection)
            .SchemaIncludes(DbObjects.UserDefinedFunctions);
    }

    [Test]
    public async Task SchemaIncludeSynonymsOnly()
    {
        await using var database = await sqlInstance.Build();
        var connection = database.Connection;

        await Verify(connection)
            .SchemaIncludes(DbObjects.Synonyms);
    }

    [Test]
    public async Task SchemaAsMarkdown()
    {
        await using var database = await sqlInstance.Build();
        var connection = database.Connection;

        await Verify(connection)
            .SchemaAsMarkdown();
    }
}