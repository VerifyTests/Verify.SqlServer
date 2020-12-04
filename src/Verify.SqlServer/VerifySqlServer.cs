using System.Collections.Generic;
using System.IO;
using System.Text;
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
            return new(null, new[] {StringStream(sql)});
        }

        static async Task<ConversionResult> ToSql(SysConnection connection, IReadOnlyDictionary<string, object> context)
        {
            var schemaSettings = context.GetSchemaSettings();
            SqlScriptBuilder builder = new(schemaSettings);
            var sql = await builder.BuildScript(connection);
            return new(null, new[] {StringStream(sql)});
        }

        static ConversionStream StringStream(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text.Replace("\r\n", "\n"));
            return new("sql", new MemoryStream(bytes));
        }
    }
}