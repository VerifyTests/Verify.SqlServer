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
    * [Enable](#enable)
    * [Usage](#usage-1)
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
<sup><a href='/src/Tests/Tests.cs#L18-L20' title='Snippet source file'>snippet source</a> | <a href='#snippet-enable' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### SqlServer Schema

This test:

<!-- snippet: SqlServerSchema -->
<a id='snippet-sqlserverschema'></a>
```cs
[Test]
public async Task SqlServerSchema()
{
    await using var database = await sqlInstance.Build();
    await Verifier.Verify(database.Connection);
}
```
<sup><a href='/src/Tests/Tests.cs#L62-L71' title='Snippet source file'>snippet source</a> | <a href='#snippet-sqlserverschema' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Will result in the following verified file:

<!-- snippet: Tests.SqlServerSchema.verified.sql -->
<a id='snippet-Tests.SqlServerSchema.verified.sql'></a>
```sql
CREATE TABLE [dbo].[MyTable](
	[Value] [int] NULL
) ON [PRIMARY]

CREATE VIEW MyView
AS
  SELECT Value
  FROM MyTable
  WHERE (Value > 10);

CREATE PROCEDURE MyProcedure
AS
BEGIN
  SET NOCOUNT ON;
  SELECT Value
  FROM MyTable
  WHERE (Value > 10);
END;

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
<sup><a href='/src/Tests/Tests.SqlServerSchema.verified.sql#L1-L29' title='Snippet source file'>snippet source</a> | <a href='#snippet-Tests.SqlServerSchema.verified.sql' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Recording

Recording allows all commands executed to be captured and then (optionally) verified.


### Enable

Call `SqlRecording.EnableRecording()`.

xsnippet: EnableRecording

`EnableRecording` should only be called in the test context.


### Usage

On the `DbContext` call `SqlRecording.StartRecording()` to start recording.

xsnippet: Recording

Will result in the following verified file:

xsnippet: Recording.verified.txt


## Security contact information

To report a security vulnerability, use the [Tidelift security contact](https://tidelift.com/security). Tidelift will coordinate the fix and disclosure.


## Icon

[Database](https://thenounproject.com/term/database/310841/) designed by [Creative Stall](https://thenounproject.com/creativestall/) from [The Noun Project](https://thenounproject.com/creativepriyanka).
