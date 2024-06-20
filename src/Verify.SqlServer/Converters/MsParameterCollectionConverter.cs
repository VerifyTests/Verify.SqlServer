class MsParameterCollectionConverter :
    WriteOnlyJsonConverter<MsParameterCollection>
{
    public override void Write(VerifyJsonWriter writer, MsParameterCollection parameters)
    {
        writer.WriteStartObject();

        MsParameterConverter.OmitName();
        foreach (MsParameter parameter in parameters)
        {
            var name = parameter.ParameterName;
            object? value;
            if (MsParameterConverter.IsOnlyValue(parameter))
            {
                value = parameter.Value;
            }
            else
            {
                value = parameter;
            }

            writer.WriteMember(parameter, value, name);
        }

        MsParameterConverter.ClearOmitName();

        writer.WriteEndObject();
    }
}