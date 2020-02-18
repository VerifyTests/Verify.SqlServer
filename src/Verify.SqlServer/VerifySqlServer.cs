using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Verify.SqlServer
{
    public static class VerifySqlServer
    {
        public static void Enable()
        {
            SharedVerifySettings.RegisterFileConverter<SqlConnection>("sql", ConnectionToSql);
            SharedVerifySettings.RegisterFileConverter<System.Data.SqlClient.SqlConnection>("sql", ConnectionToSql);
        }

        static ConversionResult ConnectionToSql(SqlConnection dbConnection, VerifySettings settings)
        {
            var schemaSettings = settings.GetSchemaSettings();
            var builder = new SqlScriptBuilder(schemaSettings);
            var sql = builder.BuildScript(dbConnection);
            return new ConversionResult(null, new Stream[] {StringToMemoryStream(sql)});
        }

        static async Task<ConversionResult> ConnectionToSql(System.Data.SqlClient.SqlConnection dbConnection, VerifySettings settings)
        {
            var schemaSettings = settings.GetSchemaSettings();
            var builder = new SqlScriptBuilder(schemaSettings);
            var sql = await builder.BuildScript(dbConnection);
            return new ConversionResult(null, new Stream[] {StringToMemoryStream(sql)});
        }

        static MemoryStream StringToMemoryStream(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text.Replace("\r\n", "\n"));
            return new MemoryStream(bytes);
        }
    }
}