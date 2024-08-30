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
            converters.Add(new ErrorConverter());
            converters.Add(new ConnectionConverter());
            converters.Add(new CommandConverter());
            converters.Add(new ExceptionConverter());
            converters.Add(new ParameterConverter());
            converters.Add(new ParameterCollectionConverter());
        });

        VerifierSettings.RegisterFileConverter<SqlConnection>(ToSql);
        // ReSharper disable once UnusedVariable
        var subscription = DiagnosticListener.AllListeners.Subscribe(listener);
    }

    static ConversionResult ToSql(SqlConnection connection, IReadOnlyDictionary<string, object> context)
    {
        var settings = context.GetSchemaSettings();
        var builder = new SqlScriptBuilder(settings);
        var content = builder.BuildContent(connection);
        var extension = GetExtension(settings);
        return new(null, extension, content);
    }

    static string GetExtension(SchemaSettings settings)
    {
        var format = settings.Format;
        return format switch
        {
            Format.Md => "md",
            Format.Sql => "sql",
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }
}