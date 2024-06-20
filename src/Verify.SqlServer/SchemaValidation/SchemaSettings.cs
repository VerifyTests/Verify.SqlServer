using Microsoft.SqlServer.Management.Smo;

class SchemaSettings
{
    public DbObjects Includes { get; set; } = DbObjects.All;
    public Format Format { get; set; } = Format.Md;
    public Func<NamedSmoObject, bool> IncludeItem { get; set; } = _ => true;
}