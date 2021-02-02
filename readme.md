# <img src="/src/icon.png" height="30px"> Verify.SqlServer

[![Build status](https://ci.appveyor.com/api/projects/status/enh6mjugcbmoun0e?svg=true)](https://ci.appveyor.com/project/SimonCropp/verify-sqlserver)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.SqlServer.svg)](https://www.nuget.org/packages/Verify.SqlServer/)

Extends [Verify](https://github.com/VerifyTests/Verify) to allow verification of SqlServer bits.

Support is available via a [Tidelift Subscription](https://tidelift.com/subscription/pkg/nuget-verify?utm_source=nuget-verify&utm_medium=referral&utm_campaign=enterprise).

<a href='https://dotnetfoundation.org' alt='Part of the .NET Foundation'><img src='https://raw.githubusercontent.com/VerifyTests/Verify/master/docs/dotNetFoundation.svg' height='30px'></a><br>
Part of the <a href='https://dotnetfoundation.org' alt=''>.NET Foundation</a>

<!-- toc -->
## Contents

  * [Usage](#usage)
    * [SqlServer Schema](#sqlserver-schema)
    * [Recording](#recording)
  * [Security contact information](#security-contact-information)<!-- endToc -->


## NuGet package

https://nuget.org/packages/Verify.SqlServer/


## Usage

Enable VerifySqlServer once at assembly load time:

<!-- snippet: Enable -->
<a id='snippet-enable'></a>
```cs
VerifySqlServer.Enable();
```
<sup><a href='/src/Tests/Tests.cs#L17-L21' title='Snippet source file'>snippet source</a> | <a href='#snippet-enable' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### SqlServer Schema

This test:

<!-- snippet: SqlServerSchema -->
<a id='snippet-sqlserverschema'></a>
```cs
await Verifier.Verify(connection);
```
<sup><a href='/src/Tests/Tests.cs#L84-L88' title='Snippet source file'>snippet source</a> | <a href='#snippet-sqlserverschema' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Will result in the following verified file:

<!-- snippet: Tests.SqlServerSchema.verified.sql -->
<a id='snippet-Tests.SqlServerSchema.verified.sql'></a>
```sql
-- Tables

CREATE TABLE [dbo].[MyTable](
	[Value] [int] NULL
) ON [PRIMARY]


-- Views

create view MyView
as
  select Value
  from MyTable
  where (Value > 10);


-- StoredProcedures

create procedure MyProcedure
as
begin
  set nocount on;
  select Value
  from MyTable
  where (Value > 10);
end;


-- UserDefinedFunctions

create function MyFunction(
  @quantity int,
  @list_price dec(10,2),
  @discount dec(4,2)
)
returns dec(10,2)
as
begin
    return @quantity * @list_price * (1 - @discount);
end;


-- Synonyms

CREATE SYNONYM [dbo].[synonym1] FOR [MyTable]
```
<sup><a href='/src/Tests/Tests.SqlServerSchema.verified.sql#L1-L45' title='Snippet source file'>snippet source</a> | <a href='#snippet-Tests.SqlServerSchema.verified.sql' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Recording

Recording allows all commands executed to be captured and then (optionally) verified.

Call `SqlRecording.StartRecording()`:

<!-- snippet: Recording -->
<a id='snippet-recording'></a>
```cs
await using SqlConnection connection = new(connectionString);
await connection.OpenAsync();
SqlRecording.StartRecording();
await using var command = connection.CreateCommand();
command.CommandText = "select Value from MyTable";
var value = await command.ExecuteScalarAsync();
await Verifier.Verify(value);
```
<sup><a href='/src/Tests/Tests.cs#L137-L147' title='Snippet source file'>snippet source</a> | <a href='#snippet-recording' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Will result in the following verified file:

<!-- snippet: Tests.Recording.verified.txt -->
<a id='snippet-Tests.Recording.verified.txt'></a>
```txt
{
  target: 42,
  sql: [
    {
      Text: select Value from MyTable
    }
  ]
}
```
<sup><a href='/src/Tests/Tests.Recording.verified.txt#L1-L8' title='Snippet source file'>snippet source</a> | <a href='#snippet-Tests.Recording.verified.txt' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


Sql entries can be explicitly read using `SqlRecording.FinishRecording`, optionally filtered, and passed to Verify:

<!-- snippet: RecordingSpecific -->
<a id='snippet-recordingspecific'></a>
```cs
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
```
<sup><a href='/src/Tests/Tests.cs#L156-L172' title='Snippet source file'>snippet source</a> | <a href='#snippet-recordingspecific' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Security contact information

To report a security vulnerability, use the [Tidelift security contact](https://tidelift.com/security). Tidelift will coordinate the fix and disclosure.


## Icon

[Database](https://thenounproject.com/term/database/310841/) designed by [Creative Stall](https://thenounproject.com/creativestall/) from [The Noun Project](https://thenounproject.com/creativepriyanka).
