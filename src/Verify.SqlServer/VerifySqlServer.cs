﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
using Microsoft.Data.SqlClient;

namespace Verify.SqlServer
{
    public static class VerifySqlServer
    {
        public static void Enable()
        {
            SharedVerifySettings.RegisterFileConverter<SqlConnection>("sql", ConnectionToSql);
        }

        static IEnumerable<Stream> ConnectionToSql(DbConnection dbConnection, VerifySettings settings)
        {
            if (!(dbConnection is SqlConnection sqlConnection))
            {
                throw new Exception("Only verification of a SqlConnection is supported");
            }

            var schemaSettings = settings.GetSchemaSettings();
            var builder = new SqlScriptBuilder(schemaSettings);
            var sql = builder.BuildScript(sqlConnection);
            yield return StringToMemoryStream(sql);
        }

        static MemoryStream StringToMemoryStream(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text.Replace("\r\n", "\n"));
            return new MemoryStream(bytes);
        }
    }
}