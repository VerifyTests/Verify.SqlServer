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

        settings.Context.Add(
            "SqlServer",
            new SchemaSettings
            {
                Includes = include,
                IncludeItem = includeItem
            });
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