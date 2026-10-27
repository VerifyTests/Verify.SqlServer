```mermaid
erDiagram
  "TestSchema_SchemaTable"["**TestSchema_SchemaTable**"]
  "ChildTable"["**ChildTable**"]
  "ColumnTypes"["**ColumnTypes**"]
  "CompositeKeyTable"["**CompositeKeyTable**"]
  "MultiIndexTable"["**MultiIndexTable**"]
  "MultiTriggerTable"["**MultiTriggerTable**"]
  "MyOtherTable"["**MyOtherTable**"]
  "MyTable"["**MyTable**"]
  "ParentTable"["**ParentTable**"]
  "WithComputed"["**WithComputed**"]
  "WithDefaults"["**WithDefaults**"]
  "ParentTable" ||--o{ "ChildTable" : "FK_ChildTable_ParentTable"
```