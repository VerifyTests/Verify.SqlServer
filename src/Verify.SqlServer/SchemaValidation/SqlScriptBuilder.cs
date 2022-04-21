using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

class SqlScriptBuilder
{
    SchemaSettings settings;
    static Dictionary<string, string> tableSettingsToScrubLookup;

    static SqlScriptBuilder()
    {
        string[] defaultsToScrub =
        {
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
        };
        tableSettingsToScrubLookup = new();
        foreach (var toScrub in defaultsToScrub)
        {
            tableSettingsToScrubLookup[$"({toScrub}, "] = "(";
            tableSettingsToScrubLookup[$"({toScrub})"] = "()";
            tableSettingsToScrubLookup[$", {toScrub}"] = "";
        }
    }
    public SqlScriptBuilder(SchemaSettings settings) =>
        this.settings = settings;

    public string BuildScript(SqlConnection sqlConnection)
    {
        var builder = new SqlConnectionStringBuilder(sqlConnection.ConnectionString);
        var server = new Server(new ServerConnection(sqlConnection));

        return BuildScript(server, builder);
    }

    public async Task<string> BuildScript(System.Data.SqlClient.SqlConnection sqlConnection)
    {
        var builder = new SqlConnectionStringBuilder(sqlConnection.ConnectionString);
        using var connection = new SqlConnection(sqlConnection.ConnectionString);
        await connection.OpenAsync();
        var server = new Server(new ServerConnection(connection));
        return BuildScript(server, builder);
    }

    string BuildScript(Server server, SqlConnectionStringBuilder builder)
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

        if (settings.Tables)
        {
            AppendType<Table>(builder, options, database.Tables, _ => _.IsSystemObject);
            ScrubTableSettings(builder);
        }

        if (settings.Views)
        {
            AppendType<View>(builder, options, database.Views, _ => _.IsSystemObject);
        }

        if (settings.StoredProcedures)
        {
            AppendType<StoredProcedure>(builder, options, database.StoredProcedures, _ => _.IsSystemObject);
        }

        if (settings.UserDefinedFunctions)
        {
            AppendType<UserDefinedFunction>(builder, options, database.UserDefinedFunctions, _ => _.IsSystemObject);
        }

        if (settings.Synonyms)
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

    void AppendType<T>(StringBuilder stringBuilder, ScriptingOptions options, SmoCollectionBase items, Func<T, bool> isSystem)
        where T : NamedSmoObject, IScriptable
    {
        var filtered = items.Cast<T>()
            .Where(x => !isSystem(x) && settings.IncludeItem(x.Name))
            .ToList();
        if (!filtered.Any())
        {
            return;
        }

        stringBuilder.AppendLine($"-- {typeof(T).Name}s");
        foreach (var item in filtered)
        {
            stringBuilder.AppendLine();
            var lines = item.Script(options)
                .Cast<string>()
                .Where(x => !IsSet(x))
                .ToList();
            if (lines.Count == 1)
            {
                stringBuilder.AppendLine(lines[0].Trim());
            }
            else
            {
                for (var index = 0; index < lines.Count; index++)
                {
                    var line = lines[index];
                    if (index == 0)
                    {
                        stringBuilder.AppendLine(line.TrimStart());
                        continue;
                    }

                    if (index == lines.Count - 1)
                    {
                        stringBuilder.AppendLine(line.TrimEnd());
                        continue;
                    }

                    stringBuilder.AppendLine(line);
                }
            }
        }

        stringBuilder.AppendLine();
        stringBuilder.AppendLine();
    }

    static bool IsSet(string script) =>
        script is
            "SET ANSI_NULLS ON" or
            "SET QUOTED_IDENTIFIER ON";
}