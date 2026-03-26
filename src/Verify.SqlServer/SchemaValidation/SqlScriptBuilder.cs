class SqlScriptBuilder(SchemaSettings settings)
{
    // TODO: when Microsoft.Data.SqlClient 7.0.1 adds TypeForwardedTo for SqlAuthenticationMethod,
    // revert to using new ServerConnection(SqlConnection) and remove this reflection workaround.
    //
    // SMO 181.15.0 ServerConnection(SqlConnection) constructor calls InitFromSqlConnection
    // which references SqlAuthenticationMethod — a type moved from Microsoft.Data.SqlClient
    // to Microsoft.Data.SqlClient.Extensions.Abstractions in SqlClient 7.0. The CLR can't
    // resolve the type in the original assembly, causing a TypeLoadException.
    //
    // Workaround: construct ServerConnection() with default constructor (no InitFromSqlConnection),
    // then set the internal m_SqlConnectionObject field via reflection to reuse the open connection.
    // SMO detects the connection is already open and uses it directly.
    internal static readonly FieldInfo SqlConnectionObjectField =
        typeof(ConnectionManager).GetField("m_SqlConnectionObject", BindingFlags.NonPublic | BindingFlags.Instance) ??
        throw new("Could not find field m_SqlConnectionObject on ConnectionManager. The SMO internals may have changed.");

    static Dictionary<string, string> tableSettingsToScrubLookup;

    static SqlScriptBuilder()
    {
        string[] defaultsToScrub =
        [
            "PAD_INDEX = OFF",
            "STATISTICS_NORECOMPUTE = OFF",
            "SORT_IN_TEMPDB = OFF",
            "DROP_EXISTING = OFF",
            "ONLINE = OFF",
            "ALLOW_ROW_LOCKS = ON",
            "IGNORE_DUP_KEY = OFF",
            "ALLOW_PAGE_LOCKS = ON",
            "OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF"
        ];
        tableSettingsToScrubLookup = [];
        foreach (var toScrub in defaultsToScrub)
        {
            tableSettingsToScrubLookup[$"({toScrub}, "] = "(";
            tableSettingsToScrubLookup[$"({toScrub})"] = "()";
            tableSettingsToScrubLookup[$", {toScrub}"] = "";
        }
    }

    public string BuildContent(SqlConnection connection)
    {
        var builder = new SqlConnectionStringBuilder(connection.ConnectionString);
        var serverConnection = new ServerConnection
        {
            NonPooledConnection = true,
            ConnectionString = connection.ConnectionString,
        };
        try
        {
            SqlConnectionObjectField.SetValue(serverConnection, connection);
            var server = new Server(serverConnection);
            return BuildContent(server, builder);
        }
        finally
        {
            serverConnection.Disconnect();
        }
    }

    string BuildContent(Server server, SqlConnectionStringBuilder builder)
    {
        var initialCatalog = builder.InitialCatalog;
        if (string.IsNullOrWhiteSpace(initialCatalog))
        {
            throw new("The connection string must specify an Initial Catalog (database name) for schema verification.");
        }

        server.SetDefaultInitFields(typeof(Table), "Name", "IsSystemObject");
        server.SetDefaultInitFields(typeof(View), "Name", "IsSystemObject");
        server.SetDefaultInitFields(typeof(StoredProcedure), "Name", "IsSystemObject");
        server.SetDefaultInitFields(typeof(UserDefinedFunction), "Name", "IsSystemObject");
        server.SetDefaultInitFields(typeof(Trigger), "Name", "IsSystemObject");
        server.SetDefaultInitFields(typeof(Synonym), "Name");
        var database = server.Databases[initialCatalog];
        database.Tables.Refresh();

        return GetScriptingObjects(database);
    }

    string GetScriptingObjects(Database database)
    {
        var options = new ScriptingOptions
        {
            ChangeTracking = true,
            NoCollation = true,
            Triggers = true,
            Indexes = true,
        };

        var builder = new StringBuilder();

        if (settings.Includes.HasFlag(DbObjects.Tables))
        {
            AppendType<Table>(builder, options, database.Tables, _ => _.IsSystemObject);
            ScrubTableSettings(builder);
        }

        if (settings.Includes.HasFlag(DbObjects.Views))
        {
            AppendType<View>(builder, options, database.Views, _ => _.IsSystemObject);
        }

        if (settings.Includes.HasFlag(DbObjects.StoredProcedures))
        {
            AppendType<StoredProcedure>(builder, options, database.StoredProcedures, _ => _.IsSystemObject);
        }

        if (settings.Includes.HasFlag(DbObjects.UserDefinedFunctions))
        {
            AppendType<UserDefinedFunction>(builder, options, database.UserDefinedFunctions, _ => _.IsSystemObject);
        }

        if (settings.Includes.HasFlag(DbObjects.Synonyms))
        {
            AppendType<Synonym>(builder, options, database.Synonyms, _ => false);
        }

        var result = builder
            .ToString()
            .TrimEnd();

        if (string.IsNullOrWhiteSpace(result))
        {
            if (settings.IsMd)
            {
                return "## No matching items found";
            }

            return "-- No matching items found";
        }

        return result;
    }

    static void ScrubTableSettings(StringBuilder builder)
    {
        foreach (var toScrub in tableSettingsToScrubLookup)
        {
            builder.Replace(toScrub.Key, toScrub.Value);
        }

        builder.Replace(")WITH () ", ") ");
    }

    void AppendType<T>(StringBuilder builder, ScriptingOptions options, IEnumerable<T> items, Func<T, bool> isSystem)
        where T : NamedSmoObject, IScriptable
    {
        var filtered = items
            .Where(_ => !isSystem(_) && settings.IncludeItem(_))
            .OrderBy(_ => _.Name, StringComparer.Ordinal)
            .ToList();
        if (filtered.Count == 0)
        {
            return;
        }

        if (settings.IsMd)
        {
            builder.AppendLineN($"## {typeof(T).Name}s");
        }
        else
        {
            builder.AppendLineN($"-- {typeof(T).Name}s");
        }
        builder.AppendLineN();

        foreach (var item in filtered)
        {
            AddItem(builder, options, item);
        }
    }

    void AddItem<T>(StringBuilder builder, ScriptingOptions options, T item)
        where T : NamedSmoObject, IScriptable
    {
        var lines = ScriptLines(options, item);
        if (settings.IsMd)
        {
            builder.AppendLineN($"### {item.Name}");
            builder.AppendLineN();
            builder.AppendLineN("```sql");
            AppendLines(builder, lines);
            builder.AppendLineN("```");
        }
        else
        {
            AppendLines(builder, lines);
        }

        builder.AppendLineN();
    }

    static void AppendLines(StringBuilder builder, List<string> lines)
    {
        if (lines.Count == 1)
        {
            builder.AppendLineN(
                lines[0]
                    .AsSpan()
                    .Trim());
            return;
        }

        for (var index = 0; index < lines.Count; index++)
        {
            var line = lines[index];
            var span = line.AsSpan();
            if (index == 0)
            {
                builder.AppendLineN(span.TrimStart());
                continue;
            }

            if (index == lines.Count - 1)
            {
                builder.AppendLineN(span.TrimEnd());
                continue;
            }

            builder.AppendLineN(span);
        }
    }

    static List<string> ScriptLines<T>(ScriptingOptions options, T item)
        where T : NamedSmoObject, IScriptable =>
        item
            .Script(options)
            .Cast<string>()
            .Where(_ => !IsSet(_))
            .Select(ScrubSetStatements)
            .ToList();

    static string ScrubSetStatements(string script)
    {
        if (!script.Contains("SET ANSI_PADDING "))
        {
            return script;
        }

        return setAnsiPaddingRegex.Replace(script, "");
    }

    static readonly Regex setAnsiPaddingRegex = new(@"^\s*SET ANSI_PADDING (ON|OFF)\s*$\n?",
        RegexOptions.Multiline | RegexOptions.Compiled);

    static bool IsSet(string script) =>
        script is
            "SET ANSI_NULLS ON" or
            "SET ANSI_NULLS OFF" or
            "SET QUOTED_IDENTIFIER ON" or
            "SET QUOTED_IDENTIFIER OFF" or
            "SET ANSI_PADDING ON" or
            "SET ANSI_PADDING OFF";
}
