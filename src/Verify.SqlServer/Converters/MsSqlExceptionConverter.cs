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

        writer.WriteEndObject();
    }
}