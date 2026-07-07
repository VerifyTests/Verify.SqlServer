## StoredProcedures

### ProcWithAnsiNullsOff

```sql
CREATE PROCEDURE ProcWithAnsiNullsOff
AS
BEGIN
  SELECT Value FROM MyTable;
END;
```