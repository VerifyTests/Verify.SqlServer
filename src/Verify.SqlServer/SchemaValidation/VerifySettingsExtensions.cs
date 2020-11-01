using System;
using System.Collections.Generic;

namespace VerifyTests
{
    public static class VerifySettingsExtensions
    {
        public static void SchemaSettings(
            this VerifySettings settings,
            bool storedProcedures = true,
            bool tables = true,
            bool views = true,
            bool userDefinedFunctions = true,
            Func<string, bool>? includeItem = null)
        {
            Guard.AgainstNull(settings, nameof(settings));
            includeItem ??= s => true;

            settings.Context.Add("EntityFramework",
                new SchemaSettings(
                    storedProcedures,
                    tables,
                    views,
                    userDefinedFunctions,
                    includeItem));
        }

        internal static SchemaSettings GetSchemaSettings(this IReadOnlyDictionary<string, object> context)
        {
            Guard.AgainstNull(context, nameof(context));
            if (context.TryGetValue("EntityFramework", out var value))
            {
                return (SchemaSettings) value;
            }

            return defaultSettings;
        }

        static SchemaSettings defaultSettings = new SchemaSettings(true, true, true, true, s => true);
    }
}