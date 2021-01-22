using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Verify.SqlServer
{
    public class LogEntry
    {
        public LogEntry(DbCommand command, Exception? exception = null)
        {
            Parameters = command.Parameters.ToDictionary();
            Text = command.CommandText.Trim();
            HasTransaction = command.Transaction != null;
            Exception = exception;
        }

        public bool HasTransaction { get; }
        public Exception? Exception { get; }
        public IDictionary<string, object> Parameters { get; }
        public string Text { get; }
    }
}