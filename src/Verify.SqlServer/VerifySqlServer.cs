using MsConnection = Microsoft.Data.SqlClient.SqlConnection;
using SysConnection = System.Data.SqlClient.SqlConnection;

namespace VerifyTests;

public static class VerifySqlServer
{
    static Listener listener = new();

    public static bool Initialized { get; private set; }

    public static void Initialize()
    {
        if (Initialized)
        {
            throw new("Already Initialized");
        }

        Initialized = true;

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

        VerifierSettings.RegisterFileConverter<MsConnection>(ToSql);
        VerifierSettings.RegisterFileConverter<SysConnection>(ToSql);
        // ReSharper disable once UnusedVariable
        var subscription = DiagnosticListener.AllListeners.Subscribe(listener);
    }

    static ConversionResult ToSql(MsConnection connection, IReadOnlyDictionary<string, object> context)
    {
        var settings = context.GetSchemaSettings();
        var builder = new SqlScriptBuilder(settings);
        var sql = builder.BuildScript(connection);
        return new(null, "sql", sql);
    }

    static async Task<ConversionResult> ToSql(SysConnection connection, IReadOnlyDictionary<string, object> context)
    {
        var settings = context.GetSchemaSettings();
        var builder = new SqlScriptBuilder(settings);
        var sql = await builder.BuildScript(connection);
        return new(null, "sql", sql);
    }
}