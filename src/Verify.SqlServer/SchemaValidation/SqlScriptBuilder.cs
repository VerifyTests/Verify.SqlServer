using System.Collections.Generic;
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

        var stringBuilder = new StringBuilder();
        if (settings.Tables && database.Tables.Count > 0)
        {
            stringBuilder.AppendLine("-- Tables");
            foreach (Table table in database.Tables)
            {
                if (!table.IsSystemObject)
                {
                    AppendItem(table.Name, table, options, stringBuilder);
                }
            }

            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
        }

        if (settings.Views && database.Views.Count > 0)
        {
            stringBuilder.AppendLine("-- Views");
            foreach (View view in database.Views)
            {
                if (!view.IsSystemObject)
                {
                    AppendItem(view.Name, view, options, stringBuilder);
                }
            }

            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
        }

        if (settings.StoredProcedures && database.StoredProcedures.Count > 0)
        {
            stringBuilder.AppendLine("-- Stored Procedures");
            foreach (StoredProcedure procedure in database.StoredProcedures)
            {
                if (!procedure.IsSystemObject)
                {
                    AppendItem(procedure.Name, procedure, options, stringBuilder);
                }
            }

            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
        }

        if (settings.UserDefinedFunctions && database.UserDefinedFunctions.Count > 0)
        {
            stringBuilder.AppendLine("-- User Defined Functions");
            foreach (UserDefinedFunction function in database.UserDefinedFunctions)
            {
                if (!function.IsSystemObject)
                {
                    AppendItem(function.Name, function, options, stringBuilder);
                }
            }

            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
        }

        if (settings.Synonyms && database.Synonyms.Count > 0)
        {
            stringBuilder.AppendLine("-- Synonyms");
            foreach (Synonym synonym in database.Synonyms)
            {
                AppendItem(synonym.Name, synonym, options, stringBuilder);
            }
        }

        return stringBuilder.ToString().TrimEnd();
    }

    void AppendItem<T>(string name, T scriptable, ScriptingOptions options, StringBuilder stringBuilder)
        where T : ScriptSchemaObjectBase, IScriptable
    {
        if (!settings.IncludeItem(name))
        {
            return;
        }

        stringBuilder.AppendLine();
        var lines = scriptable.Script(options)
            .Cast<string>()
            .Where(x => !IsSet(x))
            .ToList();
        if (lines.Count == 1)
        {
            stringBuilder.AppendLine(lines[0].Trim());
            return;
        }

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