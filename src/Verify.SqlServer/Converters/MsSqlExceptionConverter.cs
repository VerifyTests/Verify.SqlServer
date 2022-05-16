using Microsoft.Data.SqlClient;

class MsSqlExceptionConverter :
    WriteOnlyJsonConverter<SqlException>
{
    public override void Write(VerifyJsonWriter writer, SqlException exception)
    {
        writer.WriteStartObject();

        var errors = exception.Errors;
        writer.WriteProperty(exception, exception.Message, "Message");
        writer.WriteProperty(exception, exception.Number, "Number");
        writer.WriteProperty(exception, exception.LineNumber, "Line");
        if (exception.Procedure != "")
        {
            writer.WriteProperty(exception, exception.Procedure, "Procedure");
        }

        if (errors.Count == 1)
        {
            var error = errors[0];
            if (exception.Message != error.Message)
            {
                writer.WriteProperty(exception, error, "Error");
            }
        }
        else if (errors.Count > 1)
        {
            writer.WriteProperty(exception, errors, "Errors");
        }

        writer.WriteEndObject();
    }
}