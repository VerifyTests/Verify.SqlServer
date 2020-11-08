using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using VerifyTests;

class MsConnectionConverter :
    WriteOnlyJsonConverter<SqlConnection>
{
    public override void WriteJson(
        JsonWriter writer,
        SqlConnection? connection,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        if (connection == null)
        {
            return;
        }

        var schemaSettings = context.GetSchemaSettings();
        var builder = new SqlScriptBuilder(schemaSettings);
        var script = builder.BuildScript(connection);
        writer.WriteValue(script);
    }
}