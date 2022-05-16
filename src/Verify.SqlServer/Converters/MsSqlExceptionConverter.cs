using Microsoft.Data.SqlClient;

class MsSqlExceptionConverter :
    WriteOnlyJsonConverter<SqlException>
{
    public override void Write(VerifyJsonWriter writer, SqlException exception)
    {
        writer.WriteStartObject();

        var errors = exception.Errors;

        if (errors.Count == 1)
        {
            var error = errors[0];
            writer.WriteProperty(error, error.Message, "Message");
            writer.WriteProperty(error, error.Number, "Number");
            writer.WriteProperty(error, error.LineNumber, "Line");
            if (exception.Procedure != "")
            {
                writer.WriteProperty(error, error.Procedure, "Procedure");
            }
        }
        else
        {
            writer.WriteProperty(exception, errors, "Errors");
        }

        writer.WriteEndObject();
    }
}