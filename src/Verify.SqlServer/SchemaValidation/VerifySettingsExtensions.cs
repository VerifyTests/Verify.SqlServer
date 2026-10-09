namespace VerifyTests;

public static partial class VerifySettingsSqlExtensions
{
    public static SettingsTask SchemaIncludes(
        this SettingsTask settings,
        DbObjects includes)
    {
        settings.CurrentSettings.SchemaIncludes(includes);
        return settings;
    }

    public static void SchemaIncludes(
        this VerifySettings settings,
        DbObjects includes) =>
        GetOrAddSettings(settings)
            .Includes = includes;

    public static SettingsTask SchemaFilter(
        this SettingsTask settings,
        Func<NamedSmoObject, bool> filter)
    {
        settings.CurrentSettings.SchemaFilter(filter);
        return settings;
    }

    public static void SchemaFilter(
        this VerifySettings settings,
        Func<NamedSmoObject, bool> filter) =>
        GetOrAddSettings(settings)
            .IncludeItem = filter;

    public static SettingsTask SchemaAsMarkdown(
        this SettingsTask settings)
    {
        settings.CurrentSettings.SchemaAsMarkdown();
        return settings;
    }

    public static void SchemaAsMarkdown(
        this VerifySettings settings) =>
        GetOrAddSettings(settings)
            .Format = Format.Md;

    public static SettingsTask SchemaAsSql(
        this SettingsTask settings)
    {
        settings.CurrentSettings.SchemaAsSql();
        return settings;
    }

    public static void SchemaAsSql(
        this VerifySettings settings) =>
        GetOrAddSettings(settings)
            .Format = Format.Sql;

#if NET10_0_OR_GREATER
    /// <summary>
    /// Verify the schema as a Mermaid ER diagram of the tables, instead of as scripts.
    /// </summary>
    /// <param name="settings">The settings to apply to.</param>
    /// <param name="formats">The files to produce. Can be combined, with one file verified per format.</param>
    public static SettingsTask SchemaAsDiagram(
        this SettingsTask settings,
        DiagramFormat formats = DiagramFormat.Markdown)
    {
        settings.CurrentSettings.SchemaAsDiagram(formats);
        return settings;
    }

    /// <inheritdoc cref="SchemaAsDiagram(SettingsTask, DiagramFormat)" />
    public static void SchemaAsDiagram(
        this VerifySettings settings,
        DiagramFormat formats = DiagramFormat.Markdown)
    {
        if ((formats & DiagramFormat.All) == 0)
        {
            throw new ArgumentException("At least one format is required.", nameof(formats));
        }

        GetOrAddSettings(settings)
            .Diagram = formats;
    }
#endif

    static SchemaSettings GetOrAddSettings(VerifySettings settings)
    {
        var context = settings.Context;
        if (context.TryGetValue("SqlServer", out var value))
        {
            return (SchemaSettings) value;
        }

        var schemaSettings = new SchemaSettings();
        context["SqlServer"] = schemaSettings;
        return schemaSettings;
    }

    internal static SchemaSettings GetSchemaSettings(this IReadOnlyDictionary<string, object> context)
    {
        if (context.TryGetValue("SqlServer", out var value))
        {
            return (SchemaSettings) value;
        }

        return defaultSettings;
    }

    // Shared instance is safe: callers only read from it, never mutate
    static SchemaSettings defaultSettings = new();
}
