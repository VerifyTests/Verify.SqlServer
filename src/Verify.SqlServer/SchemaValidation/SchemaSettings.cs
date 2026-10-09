class SchemaSettings
{
    public DbObjects Includes { get; set; } = DbObjects.All;
    public Format Format { get; set; } = Format.Md;
    public bool IsMd => Format == Format.Md;
#if NET10_0_OR_GREATER
    // when set, the schema is verified as a diagram instead of as scripts
    public DiagramFormat? Diagram { get; set; }
#endif
    public Func<NamedSmoObject, bool> IncludeItem { get; set; } = _ => true;
}