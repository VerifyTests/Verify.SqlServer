using MsConnection = Microsoft.Data.SqlClient.SqlConnection;
using SysConnection = System.Data.SqlClient.SqlConnection;

namespace VerifyTests;

public static class VerifySqlServer
{
    public static void Enable()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        VerifierSettings.AddExtraSettings(settings =>
        {
            var converters = settings.Converters;
            converters.Add(new MsSqlErrorConverter());
            converters.Add(new MsConnectionConverter());
            converters.Add(new MsSqlExceptionConverter());
            converters.Add(new SysSqlErrorConverter());
            converters.Add(new SysConnectionConverter());
            converters.Add(new SysSqlExceptionConverter());
        });

        VerifierSettings.RegisterJsonAppender(_ =>
        {
            if (!SqlRecording.TryFinishRecording(out var entries))
            {
                return null;
            }

            return new ToAppend("sql", entries!);
        });

        VerifierSettings.RegisterFileConverter<MsConnection>(ToSql);
        VerifierSettings.RegisterFileConverter<SysConnection>(ToSql);
    }

    static ConversionResult ToSql(MsConnection connection, IReadOnlyDictionary<string, object> context)
    {
        var schemaSettings = context.GetSchemaSettings();
        var builder = new SqlScriptBuilder(schemaSettings);
        var sql = builder.BuildScript(connection);
        return new(null, "sql", sql);
    }

    static async Task<ConversionResult> ToSql(SysConnection connection, IReadOnlyDictionary<string, object> context)
    {
        var schemaSettings = context.GetSchemaSettings();
        var builder = new SqlScriptBuilder(schemaSettings);
        var sql = await builder.BuildScript(connection);
        return new(null, "sql", sql);
    }
}