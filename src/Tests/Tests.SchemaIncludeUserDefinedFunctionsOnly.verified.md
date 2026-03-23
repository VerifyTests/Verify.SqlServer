## UserDefinedFunctions

### InlineTableFunction

```sql
CREATE FUNCTION InlineTableFunction(@MinValue int)
RETURNS TABLE
AS
RETURN
  SELECT Value
  FROM MyTable
  WHERE Value >= @MinValue;
```

### MultiStatementTableFunction

```sql
CREATE FUNCTION MultiStatementTableFunction(@MinValue int)
RETURNS @Result TABLE (Value int, Category nvarchar(20))
AS
BEGIN
  INSERT INTO @Result
  SELECT Value,
    CASE
      WHEN Value < 10 THEN 'Low'
      WHEN Value < 100 THEN 'Medium'
      ELSE 'High'
    END
  FROM MyTable
  WHERE Value >= @MinValue;
  RETURN;
END;
```

### MyFunction

```sql
CREATE FUNCTION MyFunction(
  @quantity INT,
  @list_price DEC(10,2),
  @discount DEC(4,2)
)
RETURNS DEC(10,2)
AS
BEGIN
    RETURN @quantity * @list_price * (1 - @discount);
END;
```

### SchemaFunction

```sql
CREATE FUNCTION TestSchema.SchemaFunction(@Id int)
RETURNS nvarchar(50)
AS
BEGIN
  DECLARE @Name nvarchar(50);
  SELECT @Name = Name
  FROM TestSchema.SchemaTable
  WHERE Id = @Id;
  RETURN @Name;
END;
```
