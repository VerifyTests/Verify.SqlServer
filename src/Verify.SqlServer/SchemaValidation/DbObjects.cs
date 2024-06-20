namespace VerifyTests.SqlServer;

[Flags]
public enum DbObjects
{
    StoredProcedures = 1,
    Tables = 2,
    Views = 4,
    UserDefinedFunctions = 8,
    Synonyms = 16,
    All = StoredProcedures | Tables | Views | UserDefinedFunctions | Synonyms
}