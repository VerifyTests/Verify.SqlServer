class SysParameterCollectionConverter :
    WriteOnlyJsonConverter<SysParameterCollection>
{
    public override void Write(VerifyJsonWriter writer, SysParameterCollection parameters)
    {
        writer.WriteStartObject();

        SysParameterConverter.OmitName();
        foreach (SysParameter parameter in parameters)
        {
            var name = parameter.ParameterName;
            object? value;
            if (SysParameterConverter.IsOnlyValue(parameter))
            {
                value = parameter.Value;
            }
            else
            {
                value = parameter;
            }

            writer.WriteMember(parameters, value, name);
        }

        SysParameterConverter.ClearOmitName();

        writer.WriteEndObject();
    }
}