class ParameterCollectionConverter :
    WriteOnlyJsonConverter<SqlParameterCollection>
{
    public override void Write(VerifyJsonWriter writer, SqlParameterCollection parameters)
    {
        writer.WriteStartObject();

        ParameterConverter.OmitName();
        foreach (SqlParameter parameter in parameters)
        {
            var name = parameter.ParameterName;
            object? value;
            if (ParameterConverter.IsOnlyValue(parameter))
            {
                value = parameter.Value;
            }
            else
            {
                value = parameter;
            }

            writer.WriteMember(parameters, value, name);
        }

        ParameterConverter.ClearOmitName();

        writer.WriteEndObject();
    }
}