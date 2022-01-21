using System.Data.SqlClient;

class SysConnectionConverter :
    WriteOnlyJsonConverter<SqlConnection>
{
    public override void Write(VerifyJsonWriter writer, SqlConnection connection)
    {
        var schemaSettings = writer.Context.GetSchemaSettings();
        var builder = new SqlScriptBuilder(schemaSettings);
        var script = builder.BuildScript(connection);
        writer.WriteValue(script);
    }
}