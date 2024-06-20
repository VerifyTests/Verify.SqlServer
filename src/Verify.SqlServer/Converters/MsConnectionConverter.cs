class MsConnectionConverter :
    WriteOnlyJsonConverter<MsConnection>
{
    public override void Write(VerifyJsonWriter writer, MsConnection connection)
    {
        var schemaSettings = writer.Context.GetSchemaSettings();
        var builder = new SqlScriptBuilder(schemaSettings);
        var script = builder.BuildScript(connection);
        writer.WriteValue(script);
    }
}