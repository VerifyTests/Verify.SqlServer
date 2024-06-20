# <img src="/src/icon.png" height="30px"> Verify.SqlServer

[![Discussions](https://img.shields.io/badge/Verify-Discussions-yellow?svg=true&label=)](https://github.com/orgs/VerifyTests/discussions)
[![Build status](https://ci.appveyor.com/api/projects/status/enh6mjugcbmoun0e?svg=true)](https://ci.appveyor.com/project/SimonCropp/verify-sqlserver)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.SqlServer.svg)](https://www.nuget.org/packages/Verify.SqlServer/)

Extends [Verify](https://github.com/VerifyTests/Verify) to allow verification of SqlServer bits.

**See [Milestones](../../milestones?state=closed) for release notes.**


## NuGet package

https://nuget.org/packages/Verify.SqlServer/


## Usage

<!-- snippet: Enable -->
<a id='snippet-Enable'></a>
```cs
[ModuleInitializer]
public static void Init() =>
    VerifySqlServer.Initialize();
```
<sup><a href='/src/Tests/ModuleInit.cs#L3-L9' title='Snippet source file'>snippet source</a> | <a href='#snippet-Enable' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### SqlServer Schema

This test:

<!-- snippet: SqlServerSchema -->
<a id='snippet-SqlServerSchema'></a>
```cs
await Verify(connection);
```
<sup><a href='/src/Tests/Tests.cs#L80-L84' title='Snippet source file'>snippet source</a> | <a href='#snippet-SqlServerSchema' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Will result in the following verified file:

## Tables<!-- include: Tests.Schema.verified.md -->

### MyTable

```sql
CREATE TABLE [dbo].[MyTable](
	[Value] [int] NULL
) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [MyIndex] ON [dbo].[MyTable]
(
	[Value] ASC
) ON [PRIMARY]

CREATE TRIGGER MyTrigger
ON MyTable
AFTER UPDATE
AS RAISERROR ('Notify Customer Relations', 16, 10);

ALTER TABLE [dbo].[MyTable] ENABLE TRIGGER [MyTrigger]
```

## Views

### MyView

```sql
CREATE VIEW MyView
AS
  SELECT Value
  FROM MyTable
  WHERE (Value > 10);
```

## StoredProcedures

### MyProcedure

```sql
CREATE PROCEDURE MyProcedure
AS
BEGIN
  SET NOCOUNT ON;
  SELECT Value
  FROM MyTable
  WHERE (Value > 10);
END;
```

## UserDefinedFunctions

### MyFunction

```sql
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
```

## Synonyms

### synonym1

```sql
CREATE SYNONYM [dbo].[synonym1] FOR [MyTable]
```
<!-- endInclude -->


#### Object types to include

<!-- snippet: SchemaInclude -->
<a id='snippet-SchemaInclude'></a>
```cs
await Verify(connection)
    // include only tables and views
    .SchemaIncludes(DbObjects.Tables | DbObjects.Views);
```
<sup><a href='/src/Tests/Tests.cs#L344-L350' title='Snippet source file'>snippet source</a> | <a href='#snippet-SchemaInclude' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Available values:

<!-- snippet: DbObjects.cs -->
<a id='snippet-DbObjects.cs'></a>
```cs
namespace VerifyTests.SqlServer;

[Flags]
public enum DbObjects
{
    StoredProcedures = 1,
    Synonyms = 2,
    Tables = 4,
    UserDefinedFunctions = 8,
    Views = 16,
    All = StoredProcedures | Synonyms | Tables | UserDefinedFunctions | Views
}
```
<sup><a href='/src/Verify.SqlServer/SchemaValidation/DbObjects.cs#L1-L12' title='Snippet source file'>snippet source</a> | <a href='#snippet-DbObjects.cs' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### Filtering

Objects can be dynamically filtered:

<!-- snippet: SchemaFilter -->
<a id='snippet-SchemaFilter'></a>
```cs
await Verify(connection)
    // include tables & views, or named MyTrigger
    .SchemaFilter(
        _ => _ is TableViewBase ||
             _.Name == "MyTrigger");
```
<sup><a href='/src/Tests/Tests.cs#L369-L377' title='Snippet source file'>snippet source</a> | <a href='#snippet-SchemaFilter' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->



### Recording

Recording allows all commands executed to be captured and then (optionally) verified.

Call `SqlRecording.StartRecording()`:

<!-- snippet: Recording -->
<a id='snippet-Recording'></a>
```cs
await using var connection = new SqlConnection(connectionString);
await connection.OpenAsync();
Recording.Start();
await using var command = connection.CreateCommand();
command.CommandText = "select Value from MyTable";
var value = await command.ExecuteScalarAsync();
await Verify(value!);
```
<sup><a href='/src/Tests/Tests.cs#L178-L188' title='Snippet source file'>snippet source</a> | <a href='#snippet-Recording' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Will result in the following verified file:

<!-- snippet: Tests.RecordingUsage.verified.txt -->
<a id='snippet-Tests.RecordingUsage.verified.txt'></a>
```txt
{
  target: 42,
  sql: {
    HasTransaction: false,
    Text: select Value from MyTable
  }
}
```
<sup><a href='/src/Tests/Tests.RecordingUsage.verified.txt#L1-L7' title='Snippet source file'>snippet source</a> | <a href='#snippet-Tests.RecordingUsage.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


Sql entries can be explicitly read using `SqlRecording.FinishRecording`, optionally filtered, and passed to Verify:

<!-- snippet: RecordingSpecific -->
<a id='snippet-RecordingSpecific'></a>
```cs
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
```
<sup><a href='/src/Tests/Tests.cs#L295-L314' title='Snippet source file'>snippet source</a> | <a href='#snippet-RecordingSpecific' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Icon

[Database](https://thenounproject.com/term/database/310841/) designed by [Creative Stall](https://thenounproject.com/creativestall/) from [The Noun Project](https://thenounproject.com).
