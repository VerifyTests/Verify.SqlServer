class ConnectionConverter :
    WriteOnlyJsonConverter<SqlConnection>
{
    public override void Write(VerifyJsonWriter writer, SqlConnection connection)
    {
        var schemaSettings = writer.Context.GetSchemaSettings();
        var builder = new SqlScriptBuilder(schemaSettings);
        var script = builder.BuildContent(connection);
        writer.WriteValue(script);
    }
}