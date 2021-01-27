using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

class SqlScriptBuilder
{
    SchemaSettings settings;

    public SqlScriptBuilder(SchemaSettings settings)
    {
        this.settings = settings;
    }

    public string BuildScript(SqlConnection sqlConnection)
    {
        SqlConnectionStringBuilder builder = new(sqlConnection.ConnectionString);
        Server server = new(new ServerConnection(sqlConnection));

        return BuildScript(server, builder);
    }

    public async Task<string> BuildScript(System.Data.SqlClient.SqlConnection sqlConnection)
    {
        SqlConnectionStringBuilder builder = new(sqlConnection.ConnectionString);
        using SqlConnection connection = new(sqlConnection.ConnectionString);
        await connection.OpenAsync();
        Server server = new(new ServerConnection(connection));
        return BuildScript(server, builder);
    }

    string BuildScript(Server server, SqlConnectionStringBuilder builder)
    {
        server.SetDefaultInitFields(typeof(Table), "Name", "IsSystemObject");
        server.SetDefaultInitFields(typeof(View), "Name", "IsSystemObject");
        server.SetDefaultInitFields(typeof(StoredProcedure), "Name", "IsSystemObject");
        server.SetDefaultInitFields(typeof(UserDefinedFunction), "Name", "IsSystemObject");
        server.SetDefaultInitFields(typeof(Synonym), "Name");
        var database = server.Databases[builder.InitialCatalog];
        database.Tables.Refresh();

        return GetScriptingObjects(database);
    }

    string GetScriptingObjects(Database database)
    {
        ScriptingOptions options = new()
        {
            ChangeTracking = true,
            NoCollation = true
        };

        var builder = new StringBuilder();

        if (settings.Tables)
        {
            AppendType<Table>(builder, options, database.Tables, _ => _.IsSystemObject);
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

    private void AppendType<T>(StringBuilder stringBuilder, ScriptingOptions options, SchemaCollectionBase items, Func<T, bool> isSystem)
        where T : ScriptSchemaObjectBase, IScriptable
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

    static bool IsSet(string script)
    {
        if (script == "SET ANSI_NULLS ON")
        {
            return true;
        }

        if (script == "SET QUOTED_IDENTIFIER ON")
        {
            return true;
        }

        return false;
    }
}