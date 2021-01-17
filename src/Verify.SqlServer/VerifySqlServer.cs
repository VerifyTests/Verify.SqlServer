using System.Collections.Generic;
using System.Threading.Tasks;
using MsConnection = Microsoft.Data.SqlClient.SqlConnection;
using SysConnection = System.Data.SqlClient.SqlConnection;

namespace VerifyTests
{
    public static class VerifySqlServer
    {
        public static void Enable()
        {
            VerifierSettings.ModifySerialization(settings =>
            {
                settings.AddExtraSettings(serializerSettings =>
                {
                    var converters = serializerSettings.Converters;
                    converters.Add(new MsConnectionConverter());
                    converters.Add(new SysConnectionConverter());
                });
            });

            VerifierSettings.RegisterJsonAppender(_ =>
            {
                if (!SqlRecording.TryFinishRecording(out var entries))
                {
                    return null;
                }

                return new("sql", entries!);
            });

            VerifierSettings.RegisterFileConverter<MsConnection>(ToSql);
            VerifierSettings.RegisterFileConverter<SysConnection>(ToSql);
        }

        static ConversionResult ToSql(MsConnection connection, IReadOnlyDictionary<string, object> context)
        {
            var schemaSettings = context.GetSchemaSettings();
            SqlScriptBuilder builder = new(schemaSettings);
            var sql = builder.BuildScript(connection);
            return new(null, "sql", sql);
        }

        static async Task<ConversionResult> ToSql(SysConnection connection, IReadOnlyDictionary<string, object> context)
        {
            var schemaSettings = context.GetSchemaSettings();
            SqlScriptBuilder builder = new(schemaSettings);
            var sql = await builder.BuildScript(connection);
            return new(null, "sql", sql);
        }
    }
}