class SchemaSettings
{
    public bool StoredProcedures { get; }
    public bool Tables { get; }
    public bool Views { get; }
    public bool Synonyms { get; }
    public bool UserDefinedFunctions { get; }
    public Func<string, bool> IncludeItem { get; }

    public SchemaSettings(
        bool storedProcedures,
        bool tables,
        bool views,
        bool userDefinedFunctions,
        bool synonyms,
        Func<string, bool> includeItem)
    {
        StoredProcedures = storedProcedures;
        Tables = tables;
        Views = views;
        UserDefinedFunctions = userDefinedFunctions;
        Synonyms = synonyms;
        IncludeItem = includeItem;
    }
}