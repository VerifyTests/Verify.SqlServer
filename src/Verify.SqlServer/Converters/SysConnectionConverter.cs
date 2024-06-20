class SysConnectionConverter :
    WriteOnlyJsonConverter<SysConnection>
{
    public override void Write(VerifyJsonWriter writer, SysConnection connection)
    {
        var schemaSettings = writer.Context.GetSchemaSettings();
        var builder = new SqlScriptBuilder(schemaSettings);
        var script = builder.BuildContent(connection);
        writer.WriteValue(script);
    }
}