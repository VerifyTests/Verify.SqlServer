using System.Data.SqlTypes;

class MsParameterConverter :
    WriteOnlyJsonConverter<MsParameter>
{
    static AsyncLocal<bool> omitName = new();

    public static void OmitName() =>
        omitName.Value = true;

    public static void ClearOmitName() =>
        omitName.Value = false;

    public override void Write(VerifyJsonWriter writer, MsParameter parameter)
    {
        writer.WriteStartObject();
        if (!omitName.Value)
        {
            writer.WriteMember(parameter, parameter.ParameterName, "Name");
        }
        writer.WriteMember(parameter, parameter.Value, "Value");

        var (tempDbType, tempSqlDbType, tempSqlValue) = InferExpectedProperties(parameter);
        if (parameter.SqlValue != parameter.Value &&
            !Equals(parameter.SqlValue, tempSqlValue))
        {
            writer.WriteMember(parameter, parameter.SqlValue, "SqlValue");
        }

        if (tempDbType != parameter.DbType)
        {
            writer.WriteMember(parameter, parameter.DbType, "DbType");
        }

        if (tempSqlDbType != parameter.SqlDbType &&
            parameter.SqlDbType != parameter.DbType.ToSqlDbType())
        {
            writer.WriteMember(parameter, parameter.SqlDbType, "SqlDbType");
        }

        if (parameter.Direction != ParameterDirection.Input)
        {
            writer.WriteMember(parameter, parameter.Direction, "Direction");
        }

        if (parameter.Offset != 0)
        {
            writer.WriteMember(parameter, parameter.Offset, "Offset");
        }

        if (parameter.Precision != 0)
        {
            writer.WriteMember(parameter, parameter.Precision, "Precision");
        }

        if (parameter.Scale != 0)
        {
            writer.WriteMember(parameter, parameter.Scale, "Scale");
        }

        if (parameter.Size != 0)
        {
            writer.WriteMember(parameter, parameter.Size, "Size");
        }

        if (parameter.CompareInfo != SqlCompareOptions.None)
        {
            writer.WriteMember(parameter, parameter.CompareInfo, "CompareInfo");
        }

        if (parameter.IsNullable)
        {
            writer.WriteMember(parameter, parameter.IsNullable, "IsNullable");
        }

        if (parameter.LocaleId != 0)
        {
            writer.WriteMember(parameter, parameter.LocaleId, "LocaleId");
        }

        if (parameter.SourceColumn != "")
        {
            writer.WriteMember(parameter, parameter.SourceColumn, "SourceColumn");
        }

        if (parameter.SourceVersion != DataRowVersion.Current)
        {
            writer.WriteMember(parameter, parameter.SourceVersion, "SourceVersion");
        }

        if (parameter.TypeName != "")
        {
            writer.WriteMember(parameter, parameter.TypeName, "TypeName");
        }

        if (parameter.ForceColumnEncryption)
        {
            writer.WriteMember(parameter, parameter.ForceColumnEncryption, "ForceColumnEncryption");
        }

        if (parameter.UdtTypeName != "")
        {
            writer.WriteMember(parameter, parameter.UdtTypeName, "UdtTypeName");
        }

        if (parameter.SourceColumnNullMapping)
        {
            writer.WriteMember(parameter, parameter.SourceColumnNullMapping, "SourceColumnNullMapping");
        }

        if (parameter.XmlSchemaCollectionDatabase != "")
        {
            writer.WriteMember(parameter, parameter.XmlSchemaCollectionDatabase, "XmlSchemaCollectionDatabase");
        }

        if (parameter.XmlSchemaCollectionName != "")
        {
            writer.WriteMember(parameter, parameter.XmlSchemaCollectionName, "XmlSchemaCollectionName");
        }

        if (parameter.XmlSchemaCollectionOwningSchema != "")
        {
            writer.WriteMember(parameter, parameter.XmlSchemaCollectionOwningSchema, "XmlSchemaCollectionOwningSchema");
        }

        writer.WriteEndObject();
    }

    internal static bool IsOnlyValue(MsParameter parameter)
    {
        var (tempDbType, tempSqlDbType, tempSqlValue) = InferExpectedProperties(parameter);
        return (parameter.SqlValue == parameter.Value ||
                Equals(parameter.SqlValue, tempSqlValue)) &&
               tempDbType == parameter.DbType &&
               (tempSqlDbType == parameter.SqlDbType ||
                parameter.SqlDbType == parameter.DbType.ToSqlDbType()) &&
               parameter is
               {
                   Direction: ParameterDirection.Input,
                   Offset: 0,
                   Precision: 0,
                   Scale: 0,
                   Size: 0,
                   CompareInfo: SqlCompareOptions.None,
                   IsNullable: false,
                   LocaleId: 0,
                   SourceColumn: "",
                   SourceVersion: DataRowVersion.Current,
                   TypeName: "",
                   ForceColumnEncryption: false,
                   UdtTypeName: "",
                   SourceColumnNullMapping: false,
                   XmlSchemaCollectionDatabase: "",
                   XmlSchemaCollectionName: "",
                   XmlSchemaCollectionOwningSchema: ""
               };
    }

    static (DbType? dbType, SqlDbType? sqlDbType, object? sqlValue) InferExpectedProperties(MsParameter parameter)
    {
        if (parameter.Value == null)
        {
            return (null, null, null);
        }

        var temp = new MsParameter("temp", parameter.Value);
        return (temp.DbType, temp.SqlDbType, temp.SqlValue);
    }
}