## Views

### MyView

```sql
CREATE VIEW MyView
AS
  SELECT Value
  FROM MyTable
  WHERE (Value > 10);
```

### MyViewWithJoin

```sql
CREATE VIEW MyViewWithJoin
AS
  SELECT c.Id, c.Value, p.Name AS ParentName
  FROM ChildTable c
  INNER JOIN ParentTable p ON c.ParentId = p.Id;
```

### MyViewWithSchemabinding

```sql
CREATE VIEW MyViewWithSchemabinding
WITH SCHEMABINDING
AS
  SELECT Id, Name
  FROM dbo.ParentTable;
```

### SchemaView

```sql
CREATE VIEW TestSchema.SchemaView
AS
  SELECT Id, Name
  FROM TestSchema.SchemaTable;
```
