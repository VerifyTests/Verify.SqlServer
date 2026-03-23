## StoredProcedures

### MyProcedure

```sql
CREATE PROCEDURE MyProcedure
AS
BEGIN
  SET NOCOUNT ON;
  SELECT Value
  FROM MyTable
  WHERE (Value > 10);
END;
```

### ProcWithParams

```sql
CREATE PROCEDURE ProcWithParams
  @Id int,
  @Name nvarchar(100),
  @Count int OUTPUT
AS
BEGIN
  SET NOCOUNT ON;
  SELECT @Count = COUNT(*)
  FROM ParentTable
  WHERE Id = @Id AND Name = @Name;
END;
```

### SchemaProc

```sql
CREATE PROCEDURE TestSchema.SchemaProc
AS
BEGIN
  SELECT Id, Name
  FROM TestSchema.SchemaTable;
END;
```
