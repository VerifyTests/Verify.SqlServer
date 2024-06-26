using Microsoft.SqlServer.Management.Smo;

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

    static SchemaSettings defaultSettings = new();
}