# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Verify.SqlServer is a .NET library extending the [Verify](https://github.com/VerifyTests/Verify) snapshot testing framework with SQL Server support. It provides schema verification, SQL command recording via diagnostic listeners, and custom serialization for SQL types.

## Build & Test Commands

```shell
# Build
dotnet build src/Verify.SqlServer.slnx

# Run all tests (requires LocalDb)
dotnet test src/Tests/Tests.csproj

# Run a single test
dotnet test src/Tests/Tests.csproj --filter "FullyQualifiedName~Tests.Schema"
```

Tests use **NUnit** and **LocalDb** — a running SQL Server LocalDb instance is required.

## Target Frameworks

The library multi-targets: `net48`, `net8.0`, `net9.0`, `net10.0`. Tests target `net10.0` only.

## Architecture

Three main subsystems, all wired up via `VerifySqlServer.Initialize()` (called from a `[ModuleInitializer]`):

- **Converters/** — Custom Verify converters for `SqlCommand`, `SqlConnection`, `SqlParameter`, `SqlParameterCollection`, `SqlError`, `SqlException`. These control how SQL types are serialized in snapshot output.
- **Recording/** — `Listener` subscribes to `Microsoft.Data.SqlClient` diagnostic events to capture SQL commands and errors during test execution. Activated when `Recording.IsRecording()` is true.
- **SchemaValidation/** — Uses SMO (`Microsoft.SqlServer.Management.Smo`) to script database objects (tables, views, procs, functions, synonyms). Outputs as Markdown or SQL DDL. Configurable via `VerifySettings` extensions: `SchemaIncludes()`, `SchemaFilter()`, `SchemaAsMarkdown()`, `SchemaAsSql()`.

SQL parsing uses `Microsoft.SqlServer.TransactSql.ScriptDom` via visitors (`RemoveSquareBracketVisitor`, `InPredicateCollector`, `OrderByCollector`).

## Key Conventions

- Centralized package versions in `src/Directory.Packages.props`
- Assembly is strong-named (`key.snk`)
- C# 13 preview; warnings as errors
- Verified test output files (`.verified.md`, `.verified.sql`, `.verified.txt`) are checked in alongside tests — these are the snapshot baselines
