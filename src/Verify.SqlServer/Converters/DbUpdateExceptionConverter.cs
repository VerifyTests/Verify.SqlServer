//
// using Microsoft.Data.SqlClient;
//
// class MsSqlExceptionExceptionConverter :
//     WriteOnlyJsonConverter<SqlErrorCollection>
// {
//     public override void Write(VerifyJsonWriter writer, SqlErrorCollection errors)
//     {
//         if (errors.Count == 0)
//         {
//             return;
//         }
//         writer.WriteStartArray();
//
//         writer.WriteProperty(exception, exception.Message, "Message");
//         writer.WriteProperty(exception, exception.GetType(), "Type");
//         writer.WriteProperty(exception, exception.Errors, "Errors");
//         writer.WriteProperty(exception, exception.InnerException, "InnerException");
//
//         var entriesValue = exception.Errors
//             .Select(
//                 e => new
//                 {
//                     EntryProperties = e.Properties.ToDictionary(
//                         p => p.Metadata.Name,
//                         p => new
//                         {
//                             p.OriginalValue,
//                             p.CurrentValue,
//                             p.IsTemporary,
//                             p.IsModified,
//                         }),
//                     e.State,
//                 })
//             .ToList();
//
//         writer.WriteProperty(exception, entriesValue, "Entries");
//         writer.WriteProperty(exception, exception.StackTrace, "StackTrace");
//
//         writer.WriteEndArray();
//     }
// }