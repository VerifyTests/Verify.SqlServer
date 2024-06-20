using Microsoft.SqlServer.Management.Smo;

public class SchemaSettings()
{
    public bool StoredProcedures { get; init; } = true;
    public bool Tables { get; init; } = true;
    public bool Views { get; init; } = true;
    public bool Synonyms { get; init; } = true;
    public bool UserDefinedFunctions { get; init; } = true;
    public Func<NamedSmoObject, bool> IncludeItem { get; init; } = (_) => true;
}