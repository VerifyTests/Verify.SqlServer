using Microsoft.SqlServer.Management.Smo;

class SqlScriptBuilder(SchemaSettings settings)
{
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
            "OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF",
            "SORT_IN_TEMPDB = OFF"
        ];
        tableSettingsToScrubLookup = new();
        foreach (var toScrub in defaultsToScrub)
        {
            tableSettingsToScrubLookup[$"({toScrub}, "] = "(";
            tableSettingsToScrubLookup[$"({toScrub})"] = "()";
            tableSettingsToScrubLookup[$", {toScrub}"] = "";
        }
    }

    public string BuildContent(MsConnection connection)
    {
        var builder = new SqlConnectionStringBuilder(connection.ConnectionString);
        var server = new Server(new ServerConnection(connection));

        return BuildContent(server, builder);
    }

    public async Task<string> BuildContent(SysConnection sqlConnection)
    {
        var builder = new SqlConnectionStringBuilder(sqlConnection.ConnectionString);
        using var connection = new MsConnection(sqlConnection.ConnectionString);
        await connection.OpenAsync();
        var server = new Server(new ServerConnection(connection));
        return BuildContent(server, builder);
    }

    string BuildContent(Server server, SqlConnectionStringBuilder builder)
    {
        server.SetDefaultInitFields(typeof(Table), "Name", "IsSystemObject");
        server.SetDefaultInitFields(typeof(View), "Name", "IsSystemObject");
        server.SetDefaultInitFields(typeof(StoredProcedure), "Name", "IsSystemObject");
        server.SetDefaultInitFields(typeof(UserDefinedFunction), "Name", "IsSystemObject");
        server.SetDefaultInitFields(typeof(Trigger), "Name", "IsSystemObject");
        server.SetDefaultInitFields(typeof(Synonym), "Name");
        var database = server.Databases[builder.InitialCatalog];
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

        var result = builder.ToString().TrimEnd();

        if (string.IsNullOrWhiteSpace(result))
        {
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

    void AppendType<T>(StringBuilder builder, ScriptingOptions options, SmoCollectionBase items, Func<T, bool> isSystem)
        where T : NamedSmoObject, IScriptable
    {
        var filtered = items.Cast<T>()
            .Where(_ => !isSystem(_) && settings.IncludeItem(_))
            .ToList();
        if (filtered.Count == 0)
        {
            return;
        }

        builder.AppendLineN($"-- {typeof(T).Name}s");
        foreach (var item in filtered)
        {
            AddItem(builder, options, item);
        }

        builder.AppendLineN();
        builder.AppendLineN();
    }

    static void AddItem<T>(StringBuilder builder, ScriptingOptions options, T item)
        where T : NamedSmoObject, IScriptable
    {
        builder.AppendLineN();
        var lines = ScriptLines(options, item);
        AppendLines(builder, lines);
    }

    static void AppendLines(StringBuilder builder, List<string> lines)
    {
        if (lines.Count == 1)
        {
            builder.AppendLineN(lines[0].AsSpan().Trim());
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
        item.Script(options)
            .Cast<string>()
            .Where(_ => !IsSet(_))
            .ToList();

    static bool IsSet(string script) =>
        script is
            "SET ANSI_NULLS ON" or
            "SET QUOTED_IDENTIFIER ON";
}