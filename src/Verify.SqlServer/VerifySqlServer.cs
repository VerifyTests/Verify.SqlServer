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
            VerifierSettings.RegisterFileConverter<MsConnection>(ToSql);
            VerifierSettings.RegisterFileConverter<SysConnection>(ToSql);
        }

        static ConversionResult ToSql(MsConnection connection, VerifySettings settings)
        {
            var schemaSettings = settings.GetSchemaSettings();
            var builder = new SqlScriptBuilder(schemaSettings);
            var sql = builder.BuildScript(connection);
            return new ConversionResult(null, new[] {StringStream(sql)});
        }

        static async Task<ConversionResult> ToSql(SysConnection connection, VerifySettings settings)
        {
            var schemaSettings = settings.GetSchemaSettings();
            var builder = new SqlScriptBuilder(schemaSettings);
            var sql = await builder.BuildScript(connection);
            return new ConversionResult(null, new[] {StringStream(sql)});
        }

        static ConversionStream StringStream(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text.Replace("\r\n", "\n"));
            return new ConversionStream("sql", new MemoryStream(bytes));
        }
    }
}