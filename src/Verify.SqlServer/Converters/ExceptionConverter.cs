class ExceptionConverter :
    WriteOnlyJsonConverter<SqlException>
{
    public override void Write(VerifyJsonWriter writer, SqlException exception)
    {
        writer.WriteStartObject();

        var errors = exception.Errors;

        if (errors.Count == 1)
        {
            var error = errors[0];
            writer.WriteMember(error, error.Message, "Message");
            writer.WriteMember(error, error.Number, "Number");
            writer.WriteMember(error, error.LineNumber, "Line");
            if (exception.Procedure != "")
            {
                writer.WriteMember(error, error.Procedure, "Procedure");
            }
        }
        else
        {
            writer.WriteMember(exception, errors, "Errors");
        }

        var data = FilterData(exception);
        if (data != null)
        {
            writer.WriteMember(exception, data, "Data");
        }

        writer.WriteEndObject();
    }

    // SqlClient always adds HelpLink.* entries to Data. Those add nothing over the message.
    static Dictionary<object, object?>? FilterData(SqlException exception)
    {
        Dictionary<object, object?>? data = null;
        foreach (DictionaryEntry entry in exception.Data)
        {
            if (entry.Key is string key &&
                key.StartsWith("HelpLink.", StringComparison.Ordinal))
            {
                continue;
            }

            data ??= [];
            data.Add(entry.Key, entry.Value);
        }

        return data;
    }
}