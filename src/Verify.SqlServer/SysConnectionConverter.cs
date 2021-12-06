﻿using System.Data.SqlClient;
using Newtonsoft.Json;

class SysConnectionConverter :
    WriteOnlyJsonConverter<SqlConnection>
{
    public override void WriteJson(
        JsonWriter writer,
        SqlConnection? connection,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        if (connection is null)
        {
            return;
        }

        var schemaSettings = context.GetSchemaSettings();
        SqlScriptBuilder builder = new(schemaSettings);
        var script = builder.BuildScript(connection);
        writer.WriteValue(script);
    }
}