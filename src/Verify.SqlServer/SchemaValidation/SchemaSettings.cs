using Microsoft.SqlServer.Management.Smo;

class SchemaSettings(
    bool storedProcedures,
    bool tables,
    bool views,
    bool userDefinedFunctions,
    bool synonyms,
    Func<NamedSmoObject, bool> includeItem)
{
    public bool StoredProcedures { get; } = storedProcedures;
    public bool Tables { get; } = tables;
    public bool Views { get; } = views;
    public bool Synonyms { get; } = synonyms;
    public bool UserDefinedFunctions { get; } = userDefinedFunctions;
    public Func<NamedSmoObject, bool> IncludeItem { get; } = includeItem;
}