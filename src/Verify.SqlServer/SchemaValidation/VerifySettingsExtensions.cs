using System;
using System.Collections.Generic;

namespace VerifyTests
{
    public static class VerifySettingsExtensions
    {
        public static SettingsTask SchemaSettings(
            this SettingsTask settings,
            bool storedProcedures = true,
            bool tables = true,
            bool views = true,
            bool userDefinedFunctions = true,
            bool synonyms = true,
            Func<string, bool>? includeItem = null)
        {
            settings.CurrentSettings.SchemaSettings(
                    storedProcedures,
                    tables,
                    views,
                    userDefinedFunctions,
                    synonyms,
                    includeItem);
            return settings;
        }

        public static void SchemaSettings(
            this VerifySettings settings,
            bool storedProcedures = true,
            bool tables = true,
            bool views = true,
            bool userDefinedFunctions = true,
            bool synonyms = true,
            Func<string, bool>? includeItem = null)
        {
            includeItem ??= _ => true;

            settings.Context.Add(
                "SqlServer",
                new SchemaSettings(
                    storedProcedures,
                    tables,
                    views,
                    userDefinedFunctions,
                    synonyms,
                    includeItem));
        }

        internal static SchemaSettings GetSchemaSettings(this IReadOnlyDictionary<string, object> context)
        {
            if (context.TryGetValue("SqlServer", out var value))
            {
                return (SchemaSettings) value;
            }

            return defaultSettings;
        }

        static SchemaSettings defaultSettings = new(true, true, true, true, true, _ => true);
    }
}