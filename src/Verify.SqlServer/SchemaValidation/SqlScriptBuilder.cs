using System.Collections.Generic;
using System.Linq;
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
        var builder = new SqlConnectionStringBuilder(sqlConnection.ConnectionString);
        var server = new Server(new ServerConnection(sqlConnection));

        server.SetDefaultInitFields(typeof(Table), "Name", "IsSystemObject");
        server.SetDefaultInitFields(typeof(View), "Name", "IsSystemObject");
        server.SetDefaultInitFields(typeof(StoredProcedure), "Name", "IsSystemObject");
        server.SetDefaultInitFields(typeof(UserDefinedFunction), "Name", "IsSystemObject");
        var database = server.Databases[builder.InitialCatalog];
        database.Tables.Refresh();
        return string.Join("\r\n\r\n", GetScripts(database));
    }

    IEnumerable<string> GetScripts(Database database)
    {
        foreach (var scriptable in GetScriptingObjects(database))
        {
            if (((dynamic)scriptable).IsSystemObject)
            {
                continue;
            }

            yield return Script(scriptable).Trim();
        }
    }

    static string Script(IScriptable scriptable)
    {
        var options = new ScriptingOptions
        {
            ChangeTracking = true,
            NoCollation = true
        };
        return string.Join("\n\n", scriptable.Script(options)
            .Cast<string>()
            .Where(ShouldInclude));
    }

    IEnumerable<IScriptable> GetScriptingObjects(Database database)
    {
        if (settings.Tables)
        {
            foreach (Table table in database.Tables)
            {
                if (settings.IncludeItem(table.Name))
                {
                    yield return table;
                }
            }
        }

        if (settings.Views)
        {
            foreach (View view in database.Views)
            {
                if (settings.IncludeItem(view.Name))
                {
                    yield return view;
                }
            }
        }

        if (settings.StoredProcedures)
        {
            foreach (StoredProcedure procedure in database.StoredProcedures)
            {
                if (settings.IncludeItem(procedure.Name))
                {
                    yield return procedure;
                }
            }
        }

        if (settings.UserDefinedFunctions)
        {
            foreach (UserDefinedFunction function in database.UserDefinedFunctions)
            {
                if (settings.IncludeItem(function.Name))
                {
                    yield return function;
                }
            }
        }
    }

    static bool ShouldInclude(string script)
    {
        if (script == "SET ANSI_NULLS ON")
        {
            return false;
        }

        if (script == "SET QUOTED_IDENTIFIER ON")
        {
            return false;
        }

        return true;
    }
}