using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.DiagnosticAdapter;
using VerifyTests;

class Listener :
    IObserver<DiagnosticListener>,
    IDisposable
{
    ConcurrentQueue<IDisposable> subscriptions = new();
    AsyncLocal<List<LogEntry>?> local = new();

    public void Start()
    {
        local.Value = new List<LogEntry>();
    }

    public IEnumerable<LogEntry> Finish()
    {
        var localValue = local.Value;

        if (localValue == null)
        {
            throw new("SqlRecording.StartRecording must be called prior to FinishRecording.");
        }

        local.Value = null;
        return localValue;
    }

    public void OnNext(DiagnosticListener value)
    {
        if (value.Name != "SqlClientDiagnosticListener")
        {
            return;
        }

        subscriptions.Enqueue(value.SubscribeWithAdapter(this, _ => local.Value != null));
    }

    [DiagnosticName("System.Data.SqlClient.WriteCommandAfter")]
    public void OnSystemCommandAfter(DbCommand command)
    {
        local.Value!.Add(new(command));
    }

    [DiagnosticName("Microsoft.Data.SqlClient.WriteCommandAfter")]
    public void OnMsCommandAfter(DbCommand command)
    {
        local.Value!.Add(new(command));
    }

    [DiagnosticName("System.Data.SqlClient.WriteCommandError")]
    public void OnSysErrorExecuteCommand(DbCommand command, Exception exception)
    {
        local.Value!.Add(new(command, exception));
    }

    [DiagnosticName("Microsoft.Data.SqlClient.WriteCommandError")]
    public void OnMsErrorExecuteCommand(DbCommand command, Exception exception)
    {
        local.Value!.Add(new(command, exception));
    }

    void Clear()
    {
        foreach (var subscription in subscriptions)
        {
            subscription.Dispose();
        }
    }

    public void Dispose()
    {
        Clear();
    }

    public void OnCompleted()
    {
        Clear();
    }

    public void OnError(Exception error)
    {
    }
}