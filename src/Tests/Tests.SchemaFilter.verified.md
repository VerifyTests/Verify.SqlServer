## Tables

### MyOtherTable

```sql
CREATE TABLE [dbo].[MyOtherTable](
	[Value] [int] NULL
) ON [PRIMARY]
```

### MyTable

```sql
CREATE TABLE [dbo].[MyTable](
	[Value] [int] NULL
) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [MyIndex] ON [dbo].[MyTable]
(
	[Value] ASC
) ON [PRIMARY]

CREATE TRIGGER MyTrigger
ON MyTable
AFTER UPDATE
AS RAISERROR ('Notify Customer Relations', 16, 10);

ALTER TABLE [dbo].[MyTable] ENABLE TRIGGER [MyTrigger]
```

## Views

### MyView

```sql
CREATE VIEW MyView
AS
  SELECT Value
  FROM MyTable
  WHERE (Value > 10);
```
