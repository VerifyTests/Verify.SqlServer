static class Extensions
{
    public static SqlDbType ToSqlDbType(this DbType dbType) =>
        dbType switch
        {
            DbType.AnsiString => SqlDbType.VarChar,
            DbType.Binary => SqlDbType.Binary,
            DbType.Byte => SqlDbType.TinyInt,
            DbType.Boolean => SqlDbType.Bit,
            DbType.Currency => SqlDbType.Money,
            DbType.Date => SqlDbType.Date,
            DbType.DateTime => SqlDbType.DateTime,
            DbType.Decimal => SqlDbType.Decimal,
            DbType.Double => SqlDbType.Float,
            DbType.Guid => SqlDbType.UniqueIdentifier,
            DbType.Int16 => SqlDbType.SmallInt,
            DbType.Int32 => SqlDbType.Int,
            DbType.Int64 => SqlDbType.BigInt,
            DbType.Object => SqlDbType.Variant,
            DbType.String => SqlDbType.NVarChar,
            DbType.Time => SqlDbType.Time,
            DbType.AnsiStringFixedLength => SqlDbType.Char,
            DbType.StringFixedLength => SqlDbType.NChar,
            DbType.Xml => SqlDbType.Xml,
            DbType.DateTime2 => SqlDbType.DateTime2,
            DbType.DateTimeOffset => SqlDbType.DateTimeOffset,
            _ => throw new ArgumentOutOfRangeException(nameof(dbType), dbType, null)
        };
}