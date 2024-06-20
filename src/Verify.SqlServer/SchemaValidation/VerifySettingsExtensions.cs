using Microsoft.SqlServer.Management.Smo;

namespace VerifyTests;

public static class VerifySettingsSqlExtensions
{
    public static SettingsTask SchemaSettings(
        this SettingsTask settings,
        DbObjects include = DbObjects.All,
        Func<NamedSmoObject, bool>? includeItem = null)
    {
        settings.CurrentSettings.SchemaSettings(
            include,
            includeItem);
        return settings;
    }

    public static void SchemaSettings(
        this VerifySettings settings,
        DbObjects include = DbObjects.All,
        Func<NamedSmoObject, bool>? includeItem = null)
    {
        includeItem ??= _ => true;

        var schemaSettings = GetOrAddSettings(settings);

        schemaSettings.Includes = include;
        schemaSettings.IncludeItem = includeItem;
    }

    public static SettingsTask SchemaIncludes(
        this SettingsTask settings,
        DbObjects include)
    {
        settings.CurrentSettings.SchemaIncludes(include);
        return settings;
    }
    public static void SchemaIncludes(
        this VerifySettings settings,
        DbObjects include) =>
        GetOrAddSettings(settings).Includes = include;

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