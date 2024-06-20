namespace VerifyTests.SqlServer;

[Flags]
public enum DbObjects
{
    StoredProcedures = 0,
    Tables = 1,
    Views = 2,
    UserDefinedFunctions = 4,
    Synonyms = 8,
    All = StoredProcedures | Tables | Views | UserDefinedFunctions | Synonyms
}