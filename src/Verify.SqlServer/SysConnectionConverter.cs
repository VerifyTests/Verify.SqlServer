using System.Data.SqlClient;
using Newtonsoft.Json;

class SysConnectionConverter :
    WriteOnlyJsonConverter<SqlConnection>
{
    public override void Write(VerifyJsonWriter writer, SqlConnection connection, JsonSerializer serializer)
    {
        var schemaSettings = writer.Context.GetSchemaSettings();
        var builder = new SqlScriptBuilder(schemaSettings);
        var script = builder.BuildScript(connection);
        writer.WriteValue(script);
    }
}