// ReSharper disable UnusedParameter.Global

namespace VerifyTests;

public static partial class VerifySettingsSqlExtensions
{
    [Obsolete($"Use {nameof(SchemaIncludes)} or {nameof(SchemaFilter)}", true)]
    public static SettingsTask SchemaSettings(
        this SettingsTask settings,
        bool storedProcedures = true,
        bool tables = true,
        bool views = true,
        bool userDefinedFunctions = true,
        bool synonyms = true,
        Func<string, bool>? includeItem = null) =>
        throw new();

    [Obsolete($"Use {nameof(SchemaIncludes)} or {nameof(SchemaFilter)}", true)]
    public static void SchemaSettings(
        this VerifySettings settings,
        bool storedProcedures = true,
        bool tables = true,
        bool views = true,
        bool userDefinedFunctions = true,
        bool synonyms = true,
        Func<string, bool>? includeItem = null)
    {
    }
}