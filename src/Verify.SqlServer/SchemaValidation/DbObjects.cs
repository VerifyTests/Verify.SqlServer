namespace VerifyTests.SqlServer;

[Flags]
public enum DbObjects
{
    StoredProcedures = 1,
    Synonyms = 2,
    Tables = 4,
    UserDefinedFunctions = 8,
    Views = 16,
    All = StoredProcedures | Synonyms | Tables | UserDefinedFunctions | Views
}