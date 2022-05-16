using Microsoft.Data.SqlClient;

class MsSqlErrorConverter :
    WriteOnlyJsonConverter<SqlError>
{
    public override void Write(VerifyJsonWriter writer, SqlError error)
    {
        writer.WriteStartObject();
        writer.WriteProperty(error, error.Message, "Message");
        writer.WriteProperty(error, error.Number, "Number");
        writer.WriteProperty(error, error.LineNumber, "LineNumber");
        writer.WriteProperty(error, error.Procedure, "Procedure");

        writer.WriteEndObject();
    }
}