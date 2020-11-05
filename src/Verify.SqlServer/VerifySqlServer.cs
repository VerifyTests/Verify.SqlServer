using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DiagnosticAdapter;
using MsConnection = Microsoft.Data.SqlClient.SqlConnection;
using SysConnection = System.Data.SqlClient.SqlConnection;

public class SqlClientListener :
    IObserver<KeyValuePair<string, object>>,
    IObserver<DiagnosticListener>,
    IDisposable
{
    ConcurrentQueue<IDisposable> subscriptions = new ConcurrentQueue<IDisposable>();

    private const string SqlClientPrefix = "System.Data.SqlClient.";
    private const string SqlMicrosoftClientPrefix = "Microsoft.Data.SqlClient.";

    void IObserver<DiagnosticListener>.OnNext(DiagnosticListener value)
    {
        if (value.Name == "SqlClientDiagnosticListener")
        {
            subscriptions.Enqueue(value.SubscribeWithAdapter(this));
        }
    }

    void IObserver<KeyValuePair<string, object>>.OnNext(KeyValuePair<string, object> pair)
    {
        var key = pair.Key;
        if (key.StartsWith(SqlClientPrefix) ||
            key.StartsWith(SqlMicrosoftClientPrefix))
        {
            if (key.EndsWith("WriteCommandAfter"))
            {
                var type = pair.GetType();
                Debug.WriteLine(pair);
            }
        }
    }

    [DiagnosticName("System.Data.SqlClient.WriteCommandAfter")]
    public void OnSystemCommandAfter(DbCommand command)
    {
        Console.WriteLine($"CommandText: {command.CommandText}");
        Console.WriteLine();
    }

    [DiagnosticName("Microsoft.Data.SqlClient.WriteCommandAfter")]
    public void OnMsCommandAfter(DbCommand command)
    {
        Console.WriteLine($"CommandText: {command.CommandText}");
        Console.WriteLine();
    }

    public void Dispose()
    {
        foreach (var subscription in subscriptions)
        {
            subscription.Dispose();
        }
    }

    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }
}

namespace VerifyTests
{
    public static class VerifySqlServer
    {
        public static void Enable()
        {
            var subscription = DiagnosticListener.AllListeners.Subscribe(new SqlClientListener());
            VerifierSettings.RegisterFileConverter<MsConnection>(ToSql);
            VerifierSettings.RegisterFileConverter<SysConnection>(ToSql);
        }

        static ConversionResult ToSql(MsConnection connection, IReadOnlyDictionary<string, object> context)
        {
            var schemaSettings = context.GetSchemaSettings();
            var builder = new SqlScriptBuilder(schemaSettings);
            var sql = builder.BuildScript(connection);
            return new ConversionResult(null, new[] {StringStream(sql)});
        }

        static async Task<ConversionResult> ToSql(SysConnection connection, IReadOnlyDictionary<string, object> context)
        {
            var schemaSettings = context.GetSchemaSettings();
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