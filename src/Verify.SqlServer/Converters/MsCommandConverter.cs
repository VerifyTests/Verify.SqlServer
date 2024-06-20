class MsCommandConverter :
    WriteOnlyJsonConverter<MsCommand>
{
    public override void Write(VerifyJsonWriter writer, MsCommand command)
    {
        writer.WriteStartObject();
        writer.WriteMember(command, command.CommandText, "CommandText");
        writer.WriteMember(command, command.Parameters, "Parameters");
        writer.WriteMember(command, command.Transaction != null, "HasTransaction");

        if (command.CommandTimeout != 30)
        {
            writer.WriteMember(command, command.CommandTimeout, "CommandTimeout");
        }

        if (command.CommandType != CommandType.Text)
        {
            writer.WriteMember(command, command.CommandType, "CommandType");
        }

        if (!command.DesignTimeVisible)
        {
            writer.WriteMember(command, command.DesignTimeVisible, "DesignTimeVisible");
        }

        writer.WriteMember(command, command.Notification, "Notification");

        if (command.UpdatedRowSource != UpdateRowSource.Both)
        {
            writer.WriteMember(command, command.UpdatedRowSource, "UpdatedRowSource");
        }

        if (command.EnableOptimizedParameterBinding)
        {
            writer.WriteMember(command, command.EnableOptimizedParameterBinding, "EnableOptimizedParameterBinding");
        }

        writer.WriteEndObject();
    }
}