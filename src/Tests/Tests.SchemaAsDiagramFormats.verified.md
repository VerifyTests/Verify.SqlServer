```mermaid
erDiagram
  TestSchema_SchemaTable["**TestSchema_SchemaTable**"] {
    int Id pk
    nvarchar(50) Name
  }
  ChildTable["**ChildTable**"] {
    int Id pk
    int ParentId
    int Value
  }
  ColumnTypes["**ColumnTypes**"] {
    int Id pk
    nvarchar(100) Name
    nvarchar(max)(nullable) Description
    varchar(20)(nullable) Code
    decimal(18,4) Amount
    bit IsActive
    datetime2 Created
    timestamp RowVersion
    money(nullable) Price
    float(nullable) Ratio
    smallint(nullable) SmallNum
    tinyint(nullable) TinyNum
    bigint(nullable) BigNum
    uniqueidentifier(nullable) UniqueCode
    text(nullable) Notes
    varbinary(500)(nullable) BinaryData
    varbinary(max)(nullable) MaxBinary
    char(10)(nullable) FixedChar
    binary(16)(nullable) FixedBinary
    time(nullable) TimeOnly
    date(nullable) DateOnly
    ntext(nullable) NText
    xml(nullable) Xml
  }
  CompositeKeyTable["**CompositeKeyTable**"] {
    int Key1 pk
    int Key2 pk
    nvarchar(200)(nullable) Data
  }
  MultiIndexTable["**MultiIndexTable**"] {
    int Id pk
    nvarchar(100) Name
    nvarchar(50) Category
    int Status
  }
  MultiTriggerTable["**MultiTriggerTable**"] {
    int Id pk
    nvarchar(100)(nullable) Name
  }
  MyOtherTable["**MyOtherTable**"] {
    int(nullable) Value
  }
  MyTable["**MyTable**"] {
    int(nullable) Value
  }
  ParentTable["**ParentTable**"] {
    int Id pk
    nvarchar(100) Name
  }
  WithComputed["**WithComputed**"] {
    int Id pk
    nvarchar(50) FirstName
    nvarchar(50) LastName
    nvarchar(101) FullName "computed"
    int Quantity
    decimal(10,2) UnitPrice
    decimal(21,2)(nullable) TotalPrice "computed"
  }
  WithDefaults["**WithDefaults**"] {
    int Id pk
    varchar(20) Status
    datetime2 Created
    int Score
  }
  ParentTable ||--o{ ChildTable : "FK_ChildTable_ParentTable"
```