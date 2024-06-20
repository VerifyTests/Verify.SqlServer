class MsErrorConverter :
    WriteOnlyJsonConverter<MsError>
{
    public override void Write(VerifyJsonWriter writer, MsError error)
    {
        writer.WriteStartObject();
        writer.WriteMember(error, error.Message, "Message");
        writer.WriteMember(error, error.Number, "Number");
        writer.WriteMember(error, error.LineNumber, "Line");
        if (error.Procedure != "")
        {
            writer.WriteMember(error, error.Procedure, "Procedure");
        }

        writer.WriteEndObject();
    }
}