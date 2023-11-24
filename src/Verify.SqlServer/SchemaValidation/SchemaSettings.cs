class SchemaSettings(
    bool storedProcedures,
    bool tables,
    bool views,
    bool userDefinedFunctions,
    bool synonyms,
    Func<string, bool> includeItem)
{
    public bool StoredProcedures { get; } = storedProcedures;
    public bool Tables { get; } = tables;
    public bool Views { get; } = views;
    public bool Synonyms { get; } = synonyms;
    public bool UserDefinedFunctions { get; } = userDefinedFunctions;
    public Func<string, bool> IncludeItem { get; } = includeItem;
}